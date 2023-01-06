using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using Zoom;
using Newtonsoft.Json.Linq;

namespace Torn.Report
{

	public delegate void ShowProgress (double progress, string status = "");

	public class Progress
	{
		public double Numerator { get; set; }
		public double Denominator { get; set; }
		public ShowProgress ShowProgress { get; set; }

		public Progress()
		{
			Numerator = 0;
		}

		public void Increment(string status = "")
		{
			Advance(1.0, status);
		}

		public void Advance(double value, string status = "")
		{
			Numerator += value;
			ShowProgress?.Invoke(Math.Min(Numerator / Denominator, 1.0), status);
		}
	}

	/// <summary>Build web pages for WebOutput and ExportPages.</summary>
	public class ReportPages
	{
		/// <summary>Generate a page with clickable links to each league in the list.</summary>
		public static string RootPage(List<Holder> leagues)
		{
			if (leagues.Count == 0)
				return "<html><body>No league file loaded.</body></html>";

			StringBuilder sb = new StringBuilder();
			sb.Append("<html><head><title>Leagues</title></head><body style=\"background-color: #EEF\">\n");

			foreach(Holder item in leagues)
			{
				sb.Append("<a href=\"");
				sb.Append(item.Key);
				sb.Append("/index.html\">");
				sb.Append(item.League.Title);
				sb.Append("</a><br/>\n");
			}

			sb.Append("<br/><a href=\"now.html\">Now Playing</a></body></html>\n");
			return sb.ToString();
		}

		/// <summary>Generate a page full of reports for a league. If no ReportTemplates, use a default set of reports.</summary>
		public static ZoomReports OverviewReports(Holder holder, bool includeSecret)
		{
			ZoomReports reports = new ZoomReports(holder.League.Title);

			ReportTemplates reportTemplates;
			if (holder.ReportTemplates == null || holder.ReportTemplates.Count == 0)
			{
				reportTemplates = new ReportTemplates();
				reportTemplates.AddDefaults(holder.League);
			}
			else
				reportTemplates = holder.ReportTemplates;

			foreach (ReportTemplate rt in reportTemplates)
				reports.Add(Report(holder.League, includeSecret, rt));

			reports.Add(new ZoomHtmlInclusion("</div><br/><a href=\"../now.html\">Now Playing</a><br/><a href=\"fixture.html\">Fixture</a><br/><a href=\"../index.html\">Index</a><div>"));

			return reports;
		}

		/// <summary>Generate one report. The type of report generated is specified in the ReportTemplate.</summary>
		public static ZoomReportBase Report(League league, bool includeSecret, ReportTemplate rt)
		{
			bool description = rt.Settings.Contains("Description");
			switch (rt.ReportType)
			{
				case ReportType.TeamLadder: return Reports.TeamLadder(league, includeSecret, rt);
				case ReportType.MultiLadder: return Reports.MultiLadder(league, includeSecret, rt);
				case ReportType.TeamsVsTeams: return Reports.TeamsVsTeams(league, includeSecret, rt);
				case ReportType.ColourPerformance: return Reports.ColourReport(new List<League> { league }, includeSecret, rt);
				case ReportType.SoloLadder: return Reports.SoloLadder(league, includeSecret, rt);
				case ReportType.GameByGame: return Reports.GamesList(league, includeSecret, rt);
				case ReportType.DetailedGames: return Reports.DetailedGamesList(league, includeSecret, rt);
				case ReportType.GameGrid:
				case ReportType.Ascension:
				case ReportType.Pyramid:
					return Reports.GamesGrid(league, includeSecret, rt);
				case ReportType.GameGridCondensed:
				case ReportType.PyramidCondensed:
					return Reports.GamesGridCondensed(league, includeSecret, rt);
				case ReportType.Packs:
					return Reports.PackReport(new List<League> { league }, league.Games(includeSecret), rt.Title, rt.From, rt.To,
						ChartTypeExtensions.ToChartType(rt.Setting("ChartType")), description, rt.Settings.Contains("Longitudinal"));
				case ReportType.Tech:
					return Reports.TechReport(new List<League> { league }, rt.Title, rt.From, rt.To,
						ChartTypeExtensions.ToChartType(rt.Setting("ChartType")), description);
				case ReportType.SanityCheck:
					return Reports.SanityReport(new List<League> { league }, rt.Title, rt.From, rt.To, description);
				case ReportType.Everything: return Reports.EverythingReport(league, rt.Title, rt.From, rt.To, description);
				case ReportType.PageBreak: return new ZoomSeparator();
				default: return null;
			}
		}

		/// <summary>Generate a report on data from multiple league files. The type of report generated is specified in the ReportTemplate.</summary>
		public static ZoomReportBase Report(List<League> leagues, bool includeSecret, ReportTemplate rt)
		{
			bool description = rt.Settings.Contains("Description");
			switch (rt.ReportType)
			{
				case ReportType.ColourPerformance:
					return Reports.ColourReport(leagues, includeSecret, rt);
				case ReportType.Packs:
					return Reports.PackReport(leagues, null, rt.Title, rt.From, rt.To,
						ChartTypeExtensions.ToChartType(rt.Setting("ChartType")), description, rt.Settings.Contains("Longitudinal"));
				case ReportType.Tech:
					return Reports.TechReport(leagues, rt.Title, rt.From, rt.To,
						ChartTypeExtensions.ToChartType(rt.Setting("ChartType")), description);
				case ReportType.SanityCheck:
					return Reports.SanityReport(leagues, rt.Title, rt.From, rt.To, description);
				default: return Report(leagues.FirstOrDefault(), includeSecret, rt);
			}
		}

		public static string OverviewPage(Holder holder, bool includeSecret, OutputFormat outputFormat)
		{
			return OverviewReports(holder, includeSecret).ToOutput(outputFormat);
		}

		public static string GamePage(League league, Game game, OutputFormat outputFormat = OutputFormat.Svg)
		{
			ZoomReports reports = new ZoomReports();
			reports.Colors.BackgroundColor = Color.Empty;
			reports.Colors.OddColor = Color.Empty;
			reports.Add(Reports.OneGame(league, game));
			reports.Add(new ZoomHtmlInclusion("</div><br/><a href=\"index.html\">Index</a><div>"));
			return reports.ToOutput(outputFormat);
		}

		/// <summary>Generate a page with results for a team.</summary>
		public static string TeamPage(League league, bool includeSecret, LeagueTeam leagueTeam, OutputFormat outputFormat)
		{
			ZoomReports reports = new ZoomReports(leagueTeam.Name)
			{
				Reports.OneTeam(league, includeSecret, leagueTeam, DateTime.MinValue, DateTime.MaxValue, true)
			};

			foreach (var player in leagueTeam.Players)
			{
				reports.Add(new ZoomHtmlInclusion("<a name=\"player" + player.Id + "\">"));
				reports.Add(Reports.OnePlayer(league, player, new List<LeagueTeam>() { leagueTeam }));
			}

			reports.Add(new ZoomHtmlInclusion("</div><br/><a href=\"fixture.html?team=" + leagueTeam.TeamId.ToString(CultureInfo.InvariantCulture) + "\">Fixture</a><br/><a href=\"index.html\">Index</a><div>"));

			return reports.ToOutput(outputFormat);
		}

		/// <summary>Display fixtures for a league, both as a list and as a grid.</summary>
		public static string FixturePage(Fixture fixture, League league, OutputFormat outputFormat = OutputFormat.Svg)
		{
			ZoomReports reports = new ZoomReports
			{
				Reports.FixtureList(fixture, league),
				Reports.FixtureGrid(fixture, league),
				new ZoomHtmlInclusion(@"
</div><br/><a href=""index.html"">Index</a> <a href=""fixture.html"">Fixture</a>

<script>
  var tables = document.querySelectorAll('.fixturelist');
    for (const table of tables)
      for (const tr of table.querySelectorAll('tr'))
        for (const td of tr.querySelectorAll('.time')) {
          var year = parseInt(td.innerHTML.substr(0, 4), 10);
          var month = parseInt(td.innerHTML.substr(5, 2), 10);
          var day = parseInt(td.innerHTML.substr(8, 2), 10);
          var hour = parseInt(td.innerHTML.substr(11, 2), 10);
          var minute = parseInt(td.innerHTML.substr(14, 2), 10);
          var gameTime = new Date(year, month - 1, day, hour, minute, 0);
          if (td.innerHTML.substr(0, 3) == '<a ' || gameTime < Date.now())
            td.style.backgroundColor = 'gainsboro';
        }

  var url = new URL(window.location.href);
  var team = url.searchParams.get('team');

  if (team) {
    var tables = document.querySelectorAll('.fixturelist');
      for (const table of tables)
        for (const tr of table.querySelectorAll('tr:not(.t' + team + ')'))
          tr.style = 'display:none';

    var tables = document.querySelectorAll('.fixturegrid');
      for (const table of tables)
        for (const tr of table.querySelectorAll('tr'))
          for (const td of tr.querySelectorAll('td.t' + team))
            if (td.innerHTML == '')
              td.style.backgroundColor = 'gainsboro';
  }
</script>
")
			};
			return outputFormat == OutputFormat.Svg ? reports.ToHtml() : reports.ToOutput(outputFormat);
		}
	}

	class Error
	{
		[JsonPropertyName("error")]
		public string Message { get; set; }
	}

	/// <summary>Serve web pages on demand.</summary>
	public class WebOutput: IDisposable
	{
		WebServer ws;
		List<ServerGame> serverGames;

		public Holders Leagues { get; set; }
		public Holder MostRecentHolder { get; set; }  // This is the league that owns the game with the most recent DateTime.
		public Game MostRecentGame { get; set; }
		public FixtureGame NextGame { get; set; }
		public ServerGame MostRecentServerGame { get; set; }
		public string ExportFolder { get; set; }
		public Func<int> Elapsed { get; set; }  // Learn you Func Prog on five minute quick!
		public Func<List<ServerGame>> GetGames { get; set; }
		public Action<ServerGame> PopulateGame { get; set; }
		public Func<string, List<LaserGamePlayer>> Players { get; set; }

		public WebOutput(int port = 8080)
		{
			Start(port);
		}

		public void Dispose()
		{
			if (ws != null)
				ws.Stop();
		}

		public void Start(int port)
		{
			if (port != 0)
			{
				ws = new WebServer(SendResponse, "http://localhost:" + port.ToString(CultureInfo.InvariantCulture) + "/");
				ws.Run();
			}
		}

		/// <summary>Restart the web server, listening on a new port number.</summary>
		public void Restart(int port)
		{
			if (ws != null)
			{
				ws.Stop();
				Start(port);
			}
		}

		string NowPage()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("<html><body style=\"background-color: #EEF\"><p>");

			if (MostRecentGame == null)
				Update();

			if (MostRecentGame == null)
				sb.Append("No game found.");
			else
			{
				if (MostRecentServerGame == null)
					sb.Append("<a href=\"" + MostRecentHolder?.Key + "/game" +
					          MostRecentGame.Time.ToString("yyyyMMddHHmm", CultureInfo.InvariantCulture) + ".html\">Just Played</a>: " +
					          MostRecentHolder?.League.GameString(MostRecentGame));
				else
					sb.Append((MostRecentServerGame.InProgress ? "Now Playing: " : Utility.JustPlayed(MostRecentServerGame.EndTime) + ": " ) +
					          MostRecentHolder?.League.GameString(MostRecentServerGame));
			}

			if (NextGame != null)
			{
				sb.Append("<br/><a href=\"fixture.html\">Up Next</a>:");

				foreach (var ft in NextGame.Teams)
					sb.Append(ft.Key.LeagueTeam.Name + "; ");
				sb.Length -= 2;
			}
			// Add a hyperlink for each team: "team" + team.Id.ToString("D2", CultureInfo.InvariantCulture)

			sb.Append("</p></body></html>");
			return sb.ToString();
		}

		/// <summary>Set up a page using XMLHttpRequest to request the actual scoreboard report into the 'scoreboard' div.</summary>
		string ScoreboardPage(OutputFormat outputFormat = OutputFormat.Svg)
		{
			ZoomReports reports = new ZoomReports();
			reports.Colors.BackgroundColor = Color.Empty;
			reports.Colors.OddColor = Color.Empty;
			reports.Add(new ZoomHtmlInclusion("<div id='scoreboard'>" + XmlHttpRequestScoreBoard(null) + "</div>"));
			reports.Add(new ZoomSeparator());

			// Two XmlHttpRequest objects: one to ask the date/time of the latest game; the second to request an HTML fragment containing an SVG of the scoreboard.
			// But the second one passes the date/time of the game the scoreboard is _currently_ displaying as a parameter in the X-Previous header.
			// XmlHttpRequestScoreBoard() (C# code, further down) checks that parameter, and while it's the same as Torn's MostRecentGame, it sleeps.
			// When a game gets committed in Torn, MostRecentGame changes, and XmlHttpRequestScoreBoard() sends the new scoreboard SVG to that XmlHttpRequest object.
			// Note that each onreadystatechange function calls the _other_ XmlHttpRequest object's send() --
			// when a reply as to the latest game date/time comes in, the Javascript immediately calls xhrScoreboard.send() with that date/time as parameter,
			// and when a reply as to the latest scoreboard data comes in, the Javascript immediately requeries the game date/time (because if we're getting scoreboard data, MostRecentGame must have changed).
			// The final few lines of Javascript set off an initial request for game date/time, in order to get that first xhrLatest.onreadystatechange to fire.
			// We also svg.setAttribute('width') so that the scoreboard SVG mostly fills the browser window -- full width minus 2 pixels, or 90% of height (to leave room for extra text at the bottom).
			reports.Add(new ZoomHtmlInclusion(@"</div>
<br/><a href=\'index.html\'>Index</a><div>
<script>
var latest = '(none)';
var xhrLatest = new XMLHttpRequest();
var xhrScoreboard = new XMLHttpRequest();

xhrLatest.onreadystatechange = function () {
	if (this.readyState == 4 && this.status == 200)
	{
		xhrScoreboard.open('POST', 'scoreboard', true);
		xhrScoreboard.setRequestHeader('X-Previous', latest);
		latest = xhrLatest.responseText;
		xhrScoreboard.send();		
	}
};

xhrScoreboard.onreadystatechange = function () {
	if (this.readyState == 4)
	{
		if (this.status == 200)
		{
			document.getElementById('scoreboard').innerHTML = xhrScoreboard.responseText;
			window.onload();

			for (const svg of document.querySelectorAll('svg'))
				svg.setAttribute('width', Math.min(document.documentElement.clientWidth - 2, document.documentElement.clientHeight / svg.clientHeight * svg.clientWidth * 0.9));
		}
		else
			document.getElementById('scoreboard').innerHTML = '<p>' + this.status + ': ' + this.statusText + '</p>';

		xhrLatest.open('GET', 'latest', true);
		xhrLatest.send();
	}
};

xhrScoreboard.open('GET', 'scoreboard', true);
xhrScoreboard.setRequestHeader('X-Previous', '(none)');
xhrScoreboard.send();
</script>
"));
			return reports.ToOutput(outputFormat);
		}

		void SendResponse(HttpListenerContext context)
		{
			var request = context.Request;
			try
			{
				string[] urlParts = request.RawUrl.Split('/');
				string lastPart = urlParts.Length > 0? urlParts[urlParts.Length - 1] : null;
				Holder holder = null;

				if (Leagues.Count == 1)
					holder = Leagues.First();
				else if (urlParts.Length > 1)
					holder = Leagues.Find(x => x.Key == urlParts[1]);

				if (request.RawUrl == "/")
				{
					if (Leagues.Count == 1)
						Respond(context, ReportPages.OverviewPage(holder, false, OutputFormat.Svg));
					else
						Respond(context, ReportPages.RootPage(Leagues));
				}
				else if (lastPart.EndsWith(".html"))
					Respond(context, HtmlResponse(context, lastPart, holder));
				else if (lastPart.EndsWith(".png"))
					ImageResponse(context, lastPart, holder);
				else
					Respond(context, RestResponse(request));
			}
			catch (Exception ex)
			{
				Respond(context, "<html><body>\n" + ex.Message + "\n<br/><br/>\n" + ex.StackTrace + "</body></html>");
				context.Response.StatusCode = 500;
				context.Response.StatusDescription = ex.Message + "\n<br/><br/>\n" + ex.StackTrace;
				throw;
			}
		}

		/// <summary>Take a string response and stuff it into an HttpListenerContext.Response.</summary>
		void Respond(HttpListenerContext context, string s)
		{
			byte[] buf = Encoding.UTF8.GetBytes(s);
			context.Response.ContentLength64 = buf.Length;
			context.Response.OutputStream.Write(buf, 0, buf.Length);
		}

		string HtmlResponse(HttpListenerContext context, string lastPart, Holder holder)
		{
			string rawUrl = context.Request.RawUrl;
			if (lastPart == "now.html")
				return NowPage();
			else if (lastPart == "scoreboard.html")
				return ScoreboardPage();
			else if (holder == null)
				return string.Format(CultureInfo.InvariantCulture, "<html><body>Couldn't find a league key in \"<br>{0}\". Try <a href=\"now.html\">Now Playing</a> instead.</body></html>", rawUrl);
			else if (lastPart == "index.html")
				return ReportPages.OverviewPage(holder, false, OutputFormat.Svg);
			else if (lastPart.StartsWith("games", StringComparison.OrdinalIgnoreCase))
			{
				DateTime dt = DateTime.ParseExact(lastPart.Substring(5, 8), "yyyyMMdd", CultureInfo.InvariantCulture);
				Game game = holder.League.AllGames.Find(x => x.Time.Subtract(dt).TotalSeconds < 60);
				if (game == null)
					return string.Format(CultureInfo.InvariantCulture, "<html><body>Invalid game: <br>{0}</body></html>", rawUrl);
				else
					return ReportPages.GamePage(holder.League, game);
			}
			else if (lastPart.StartsWith("game2", StringComparison.OrdinalIgnoreCase))
			{
				DateTime dt = DateTime.ParseExact(lastPart.Substring(4, 12), "yyyyMMddHHmm", CultureInfo.InvariantCulture);
				Game game = holder.League.AllGames.Find(x => x.Time.Subtract(dt).TotalSeconds < 60);
				if (game == null)
					return string.Format(CultureInfo.InvariantCulture, "<html><body>Invalid game: <br>{0}</body></html>", rawUrl);
				else
					return ReportPages.GamePage(holder.League, game);
			}
			else if (lastPart.StartsWith("team", StringComparison.OrdinalIgnoreCase))
			{
				if (int.TryParse(lastPart.Substring(4, 2), out int teamId))
				{
					LeagueTeam leagueTeam = holder.League.Teams.Find(x => x.TeamId == teamId);
					if (leagueTeam == null)
						return string.Format(CultureInfo.InvariantCulture, "<html><body>Invalid team number: <br>{0}</body></html>", rawUrl);
					else
						return ReportPages.TeamPage(holder.League, false, leagueTeam, OutputFormat.Svg);
				}
				else
					return string.Format(CultureInfo.InvariantCulture, "<html><body>Invalid team: <br>{0}</body></html>", rawUrl);
			}
			else if (lastPart.StartsWith("fixture", StringComparison.OrdinalIgnoreCase))
			{
				return ReportPages.FixturePage(holder.Fixture, holder.League);
			}
			else
				return string.Format(CultureInfo.InvariantCulture, "<html><body>Invalid path: <br>{0}</body></html>", rawUrl);
		}

		/// <summary>Put the specified image into an HttpListenerContext.Response.</summary>
		void ImageResponse(HttpListenerContext context, string lastPart, Holder holder)
		{
			if (holder == null)
			{
				context.Response.StatusCode = 404;
				context.Response.StatusDescription = "That league is not currently open in Torn.";
				return;
			}

			string imagePath = Path.Combine(ExportFolder, holder.Key, lastPart);
			if (File.Exists(imagePath))
			{
				context.Response.Headers.Set("Content-Type", "image/png");
				byte[] buf = File.ReadAllBytes(imagePath);
				context.Response.ContentLength64 = buf.Length;
				context.Response.OutputStream.Write(buf, 0, buf.Length);
			}
			else
			{
				context.Response.StatusCode = 404;
				context.Response.StatusDescription = "Image '" + lastPart + "' not found in league '" + holder.Key + "'.";
			}
		}

		private string RestGame(string gameTime)
		{
			if (serverGames == null)
				serverGames = GetGames?.Invoke();
			if (serverGames == null)
				return JsonSerializer.Serialize(new Error { Message = "Games list not initialised. (Perhaps no lasergame server is configured.)" });

			if (gameTime.EndsWith(".json"))
				gameTime = gameTime.Substring(0, gameTime.Length - 5);

			var game = serverGames.Find(g => g.Time.ToString("s") == gameTime);
			if (game == null)
				return JsonSerializer.Serialize(new Error { Message = "Game " + gameTime + " not found." });

			return RestGame(game);
		}

		private string RestGame(ServerGame game)
		{
			PopulateGame(game);
			return JsonSerializer.Serialize<ServerGame>(game);
		}

		private string RestGames()
		{
			var serverGames = GetGames?.Invoke();
			if (serverGames.Any())
			{
				var jsonGames = new List<JsonGame>();
				foreach (var game in serverGames)
					jsonGames.Add(JsonGame.ShallowClone(game));

				return JsonSerializer.Serialize<List<JsonGame>>(jsonGames);
			}
			else
				return JsonSerializer.Serialize(new Error { Message = "Games list not initialised. (Perhaps no lasergame server is configured.)" });
		}

		string RestPlayers(string mask)
		{
			return JsonSerializer.Serialize<List<LaserGamePlayer>>(Players(mask));
		}

		/// <summary>Return an HTML fragment to be displayed within the 'scoreboard' div on scoreboard.html.</summary>
		string XmlHttpRequestScoreBoard(HttpListenerRequest request)  // Get parameters from InputStream, QueryString, ContentType, or Headers.
		{
			if (MostRecentGame == null)
				Update();

			if (MostRecentGame == null)
				return "No game found.";

			// X-Previous is the game the scoreboard is already displaying when it sends us this request. We don't want to respond to the request until we have a new game for it to show.
			// (If there's no X-Previous header, request.Headers["X-Previous"] returns the null string.)
			while (request != null && JsonSerializer.Serialize<DateTime>(MostRecentGame.Time) == request.Headers["X-Previous"])
				System.Threading.Thread.Sleep(1000);

			ZoomReports reports = new ZoomReports();
			reports.Colors.BackgroundColor = Color.Empty;
			reports.Colors.OddColor = Color.Empty;
			reports.Add(Reports.OneGame(MostRecentHolder?.League, MostRecentGame));

			return
				reports[0].ToSvg() + NextGameHtml();
		}

		string NextGameHtml()
		{
			if (NextGame == null)
				return null;

			StringBuilder sb = new StringBuilder();
			{
				sb.Append("</div><br/><a href=\"fixture.html\">Up Next</a>:");

				foreach (var ft in NextGame.Teams)
					sb.Append(ft.Key.LeagueTeam.Name + "; ");
				sb.Length -= 2;
				sb.Append("<div>");
			}
			return sb.ToString();
		}

		string RestResponse(HttpListenerRequest request)
		{
			string rawUrl = request.RawUrl;
			if (rawUrl.EndsWith("elapsed"))
				return JsonSerializer.Serialize<int>(Elapsed());
			if (rawUrl.EndsWith("latest"))
			{
				if (MostRecentGame == null)
					Update();
				return JsonSerializer.Serialize<DateTime>(MostRecentGame == null ? default : MostRecentGame.Time);
			}
			else if (rawUrl.EndsWith("games.json"))
				return RestGames();
			else if (rawUrl.Contains("game2"))
				return RestGame(rawUrl.Substring(rawUrl.IndexOf("game") + 4));  // return one detailed game: all the players, all the details.
			else if (rawUrl.EndsWith("players.json"))
				return RestPlayers("");  // return the list of all players available from this lasergame server.
			else if (rawUrl.EndsWith("scoreboard"))
				return XmlHttpRequestScoreBoard(request);
			else
				return string.Format(CultureInfo.InvariantCulture, "<html><body>Invalid path: <br>{0}</body></html>", rawUrl);
		}

		/// <summary>Call this when we have switched from playing to idle or vice versa.</summary>
		public void Update()
		{
			if (Leagues != null)
			{
				MostRecentHolder = Leagues.MostRecent();
				if (MostRecentHolder == null || MostRecentHolder.League.AllGames.Count() == 0)
					return;

				MostRecentGame = MostRecentHolder.League.AllGames.Last();

				FixtureGame fg = MostRecentHolder.Fixture.BestMatch(MostRecentGame);
				if (fg == null)
					NextGame = null;
				else
				{
					int i = MostRecentHolder.Fixture.Games.FindIndex(x => x == fg);

					NextGame = MostRecentHolder.Fixture.Games.Count > i + 1 ? MostRecentHolder.Fixture.Games[i + 1] : null;
				}
			}
		}

		/// <summary>Return a text description of the current and next games.</summary>
		public string NowText()
		{
			string nowText = "";
			if (MostRecentServerGame == null)
			{
				if (MostRecentHolder != null && MostRecentGame != null)
					nowText = "Just Played: " + MostRecentHolder.League.GameString(MostRecentGame);
			}
			else if (MostRecentHolder != null)
				nowText = (MostRecentServerGame.InProgress ? "Now Playing: " : "Just Played: ") +
					MostRecentHolder.League.GameString(MostRecentServerGame);

			if (NextGame != null)
			{
				nowText += "\nUp Next:";
				foreach (var ft in NextGame.Teams)
					nowText += ft.Key.LeagueTeam + "; ";
			}

			return nowText;
		}

		public void ExportJson(string path, ShowProgress progress = null)
		{
			if (serverGames == null)
				serverGames = GetGames?.Invoke();

			if (path != null)
			{
				Progress myProgress = new Progress() { Denominator = serverGames.Count + 2, ShowProgress = progress };

				Directory.CreateDirectory(Path.Combine(path, "json"));

				using (StreamWriter sw = File.CreateText(Path.Combine(path, "json\\games.json")))
					sw.Write(RestGames());
				myProgress.Increment("Games list exported.");

				foreach (var game in serverGames)
				{
					using (StreamWriter sw = File.CreateText(Path.Combine(path, "json\\game" + game.Time.ToString("yyyy-MM-ddTHH_mm_ss") + ".json")))
						sw.Write(RestGame(game));
					myProgress.Increment("Game" + game.Time.ToString("yyyy-MM-ddTHH-mm-ss") + " exported.");
				}

				using (StreamWriter sw = File.CreateText(Path.Combine(path, "json\\players.json")))
					sw.Write(RestPlayers(""));
				myProgress.Increment("Players list exported.");
			}
		}

		public void ExportGamesToJSON(string path, List<ServerGame> games, ShowProgress progress = null)
        {
			if (path != null)
			{
				Progress myProgress = new Progress() { Denominator = games.Count + 2, ShowProgress = progress };

				Directory.CreateDirectory(Path.Combine(path, "json"));

				Console.WriteLine("Here");

				foreach (var game in games)
				{

					JObject gameJSON = new JObject();

					JArray eventsJSON = new JArray();
					JArray playersJSON = new JArray();

					List<Event> sortedEvents = game.Events.OrderBy(e => e.Time).ToList();
					List<ServerPlayer> sortedPlayers = game.Players.OrderBy(p => p.Colour).ThenBy(p => p.Rank).ToList();

					foreach (Event ev in sortedEvents)
					{
						JObject obj = JObject.FromObject(ev);

                        string playerAlias = game.Players.Find(p => p.ServerPlayerId == ev.ServerPlayerId)?.Alias;
                        string otherPlayerAlias = game.Players.Find(p => p.ServerPlayerId == ev.OtherPlayer)?.Alias;

						obj.Add(new JProperty("alias", playerAlias));
						obj.Add(new JProperty("otherPlayerAlias", otherPlayerAlias));

						eventsJSON.Add(obj);
					}
					foreach (ServerPlayer player in sortedPlayers)
                    {
						JObject obj = JObject.FromObject(player);
						playersJSON.Add(obj);
					}

					gameJSON.Add(new JProperty("Title", game.Description));
					gameJSON.Add(new JProperty("Time", game.Time));
					gameJSON.Add(new JProperty("Players", playersJSON));
					gameJSON.Add(new JProperty("Events", eventsJSON));

					using (StreamWriter sw = File.CreateText(Path.Combine(path, "json\\game" + game.Time.ToString("yyyy-MM-ddTHH_mm_ss") + ".json")))
						sw.Write(gameJSON.ToString());
					myProgress.Increment("Game" + game.Time.ToString("yyyy-MM-ddTHH-mm-ss") + " exported.");
				}
			}
		}
	}

	/// <summary>Generates web pages to file for export or upload.</summary>
	public class ExportPages
	{
		/// <summary>Generate reports for the selected leagues, and write them to disk.</summary>
		public static void ExportReports(string path, bool includeSecret, List<Holder> selected, ShowProgress progress = null)
		{
			if (path != null)
			{
				Progress myProgress = new Progress() { Denominator = selected.Count * 3 + 1, ShowProgress = progress };

				if (selected.Any())
					using (StreamWriter sw = File.CreateText(Path.Combine(path, "index." + selected[0].ReportTemplates.OutputFormat.ToExtension())))
						sw.Write(ReportPages.RootPage(selected));
				myProgress.Increment("Root page exported.");

				foreach (Holder holder in selected)
				{
					Directory.CreateDirectory(Path.Combine(path, holder.Key));

					using (StreamWriter sw = File.CreateText(Path.Combine(path, holder.Key, "index." + holder.ReportTemplates.OutputFormat.ToExtension())))
						sw.Write(ReportPages.OverviewPage(holder, includeSecret, holder.ReportTemplates.OutputFormat));
					myProgress.Increment(holder.League.Title + " Overview page exported.");

					foreach (LeagueTeam leagueTeam in holder.League.Teams)
					{
						using (StreamWriter sw = File.CreateText(Path.Combine(path, holder.Key, "team" + leagueTeam.TeamId.ToString("D2", CultureInfo.InvariantCulture) + "." + holder.ReportTemplates.OutputFormat.ToExtension())))
							sw.Write(ReportPages.TeamPage(holder.League, includeSecret, leagueTeam, holder.ReportTemplates.OutputFormat));
						myProgress.Advance(1 / holder.League.Teams.Count, "Team " + leagueTeam.Name + " page exported.");
					}
					myProgress.Advance(0, "Team pages exported.");

					ExportGames(holder, path, myProgress);
					myProgress.Advance(0, "Games pages exported.");
				}
			}
		}

		/// <summary>Generate game reports for every game in a league.</summary>
		static void ExportGames(Holder holder, string path, Progress progress)
		{
			var dates = holder.League.AllGames.Select(g => g.Time.Date).Distinct().ToList();
			foreach (var date in dates)
			{
				ExportDay(holder, path, date);

				progress.Advance(1.0 / dates.Count, "Exported games for " + date.ToShortDateString());
			}
		}

		/// <summary>Generate game reports for a single day and write them to disk, but only if necessary.</summary>
		static void ExportDay(Holder holder, string path, DateTime day)
		{
			League league = holder.League;
			string fileName = Path.Combine(path, holder.Key, "games" + day.ToString("yyyyMMdd", CultureInfo.InvariantCulture) +
																		"." + holder.ReportTemplates.OutputFormat.ToExtension());

			var dayGames = league.AllGames.Where(g => g.Time.Date == day);
			if (dayGames.Any(g => !g.Reported) || !File.Exists(fileName))  // Some of the games for this day are not marked as reported, or the file we're going to report to doesn't exist. So let's report.
			{
				ZoomReports reports = new ZoomReports(league.Title + " games on " + day.ToShortDateString());
				reports.Colors.BackgroundColor = Color.Empty;
				reports.Colors.OddColor = Color.Empty;
				league.AllGames.Sort();
				bool detailed = false;

				var rt = new ReportTemplate() { From = day, To = day.AddSeconds(86399) };
				reports.Add(Reports.GamesToc(league, false, rt));
				string gameTitle = "";

				foreach (Game game in dayGames)
				{
					if (gameTitle != game.Title)
					{
						reports.Add(new ZoomSeparator());
						gameTitle = game.Title;
					}

					reports.Add(new ZoomHtmlInclusion("<a name=\"game" + game.Time.ToString("HHmm", CultureInfo.InvariantCulture) + "\"><div style=\"display: flex; flex-flow: row wrap; justify-content: space-around; \">\n"));
					reports.Add(Reports.OneGame(league, game));
					if (game.ServerGame != null && game.ServerGame.Events.Any() && !game.ServerGame.InProgress)
					{
						string imageName = "score" + game.Time.ToString("yyyyMMdd_HHmm", CultureInfo.InvariantCulture) + ".png";
						string imagePath = Path.Combine(path, holder.Key, imageName);
						if (!game.Reported || !File.Exists(imagePath))
						{
							var bitmap = Reports.GameWorm(league, game, true);
							if (bitmap != null && (bitmap.Height > 1 || !File.Exists(imagePath)))
								bitmap.Save(imagePath, System.Drawing.Imaging.ImageFormat.Png);
						}
						reports.Add(new ZoomHtmlInclusion("\n<div><p> </p></div><div><p> </p></div>\n<div><img src=\"" + imageName + "\"></div></div>\n"));
						game.Reported = true;
						detailed = true;
					}
				}
				if (detailed)
				{
					var eventsUsed = dayGames.Where(g => g.ServerGame != null).SelectMany(g => g.ServerGame.Events.Select(e => e.Event_Type)).Distinct();
					var sb = new StringBuilder("</div>\n<p>");
					if (eventsUsed.Contains(30) || eventsUsed.Contains(31)) sb.Append("\u25cb and \u2b24 are hit and destroyed bases.<br/>");
					if (eventsUsed.Contains(1403) || eventsUsed.Contains(1404)) sb.Append("\U0001f61e and \U0001f620 are one- and two-shot denied.<br/>");
					if (eventsUsed.Contains(1401) || eventsUsed.Contains(1402)) sb.Append("\u2300 and \u29bb are denied another player.<br/>");
					if (eventsUsed.Contains(28)) sb.Append("\U0001f7e8 is warning (yellow card). ");
					if (eventsUsed.Contains(28) && !eventsUsed.Contains(29)) sb.Append("<br/>");
					if (eventsUsed.Contains(29)) sb.Append("\U0001f7e5 is termination (red card).<br/>");
					if (eventsUsed.Contains(32)) sb.Append("\U0001f480 is player eliminated.<br/>");
					if (eventsUsed.Contains(33) && !eventsUsed.Contains(34) && !eventsUsed.Any(t => t >= 37 && t <= 46)) sb.Append("! is hit by base, or player self-denied.<br/>");
					if (eventsUsed.Contains(34) || eventsUsed.Any(t => t >= 37 && t <= 46)) sb.Append("! is hit by base or mine, or player self-denied, or player tagged target.<br/>");
					sb.Append("\u00B7 shows each minute elapsed.<br/>Tags+ includes shots on bases and teammates.</p>\n");
					sb.Append(@"<p>""Worm"" charts show coloured lines for each team. Vertical dashed lines show time in minutes. <br/>
Sloped dashed lines show lines of constant score: 0 points, 10K points, etc. The slope of these lines shows the average rate of scoring of ""field points"" 
during the game. Field points are points not derived from shooting bases, getting penalised by a referee, etc. A team whose score line is horizontal is 
scoring points  at the average field pointing rate for the game. <br/>
Base hits and destroys are shown with a mark in the colour of the base hit. Base destroys have the alias of the player destroying the base next to them.</p>
<div>");
					reports.Add(new ZoomHtmlInclusion(sb.ToString()));
				}

				reports.Add(new ZoomHtmlInclusion("</div><a href=\"index.html\">Index</a><div>"));
				if (reports.Count > 1)  // There were games this day.
					using (StreamWriter sw = File.CreateText(fileName))
						sw.Write(reports.ToOutput(holder.ReportTemplates.OutputFormat));
			}
		}

		static string OneWorm(Game game, string path, Holder holder)
		{
			if (game.ServerGame != null && game.ServerGame.Events.Any() && !game.ServerGame.InProgress)
			{
				string imageName = "score" + game.Time.ToString("yyyyMMdd_HHmm", CultureInfo.InvariantCulture) + ".png";
				string imagePath = Path.Combine(path, holder.Key, imageName);
				if (game.Reported && File.Exists(imagePath))
					return "<img src=\"" + holder.Key + "/" + imageName + "\"/>";
			}
			return null;
		}

		/// <summary>Write out fixtures for the selected leagues.</summary>
		public static void ExportFixtures(string path, List<Holder> leagues)
		{
			foreach (Holder holder in leagues)
				ExportFixture(path, holder);
		}

		public static void ExportFixture(string path, Holder holder)
		{
			if (path != null)
			{
				Directory.CreateDirectory(Path.Combine(path, holder.Key));

				using (StreamWriter sw = File.CreateText(Path.Combine(path, holder.Key, "fixture." + holder.ReportTemplates.OutputFormat.ToExtension())))
					sw.Write(ReportPages.FixturePage(holder.Fixture, holder.League));
			}
		}

		static void UploadFile(WebClient client, string to, string from, string file)
		{
			Application.DoEvents();
			client.UploadFile(to + file, Path.Combine(from, file));
		}

		/// <summary>Upload files from the named path via FTP to the internet.</summary>
		public static void UploadFiles(string uploadMethod, string uploadSite, string username, string password, string localPath, List<Holder> selected, ShowProgress progress = null)
		{

			string url = uploadMethod + "://" + uploadSite;
			if (url.Last() != '/')
				url += '/';

			Cursor.Current = Cursors.WaitCursor;
			try
			{
				using (WebClient client = new WebClient())
				{
					client.Credentials = new NetworkCredential(username.Normalize(), password.Normalize());

					UploadFile(client, url, localPath, "index.html");

					for (int h = 0; h < selected.Count; h++)
					{
						string key = selected[h].Key;

						DirectoryInfo di = new DirectoryInfo(Path.Combine(localPath, key));

						// Create a directory on FTP site:
//					    WebRequest wr = WebRequest.Create(url + key);
//						wr.Method = WebRequestMethods.Ftp.MakeDirectory;
//						wr.Credentials = client.Credentials;
//						wr.GetResponse();

						FileInfo[] files = di.GetFiles("*.html");
						for (int i = 0; i < files.Count(); i++)
						{
							UploadFile(client, url, Path.Combine(localPath, key), files[i].Name);
							progress?.Invoke((1.0 * i / files.Count() + h) / selected.Count, "Uploaded " + files[i].Name);
						}
					}
				}
			}
			catch (WebException we)
			{
				MessageBox.Show(we.Message, we.Status.ToString());
			}
			finally
			{
				Cursor.Current = Cursors.Default;
			}
		}

		/// <summary>Write a single pack report incorporating data from all the selected leagues.</summary>
		public static void PackReport(string path, List<League> leagues, ReportTemplate reportTemplate, OutputFormat outputFormat)
		{
			if (path != null)
			{
				var round1Games = new List<Game>();
				foreach (var league in leagues)
					round1Games.AddRange(league.AllGames.Where(g => g.Title == "Round Robin" || g.Title == "Round 1" ||
					                                         g.Title == "Rep 1" || g.Title == "Repechage 1" || g.Title == "Repêchage 1"));
				
				if (round1Games.Count == 0)
					foreach (var league in leagues)
						round1Games.AddRange(league.AllGames);

				using (StreamWriter sw = File.CreateText(Path.Combine(path, "packreport." + outputFormat.ToExtension())))
					sw.Write(new ZoomReports
						{
							Reports.PackReport(leagues, round1Games, reportTemplate.Title, reportTemplate.From, reportTemplate.To, 
								ChartTypeExtensions.ToChartType(reportTemplate.Setting("ChartType")), reportTemplate.Settings.Contains("Description"), reportTemplate.Settings.Contains("Longitudinal"))
						}.ToOutput(outputFormat));
			}
		}

		/// <summary>Write a single tech report incorporating data from all the selected leagues.</summary>
		public static void TechReport(string path, List<League> leagues, ReportTemplate reportTemplate, OutputFormat outputFormat)
		{
			if (path != null)
				using (StreamWriter sw = File.CreateText(Path.Combine(path, "techreport." + outputFormat.ToExtension())))
					sw.Write(new ZoomReports
						{
							Reports.TechReport(leagues, reportTemplate.Title, reportTemplate.From, reportTemplate.To,
								ChartTypeExtensions.ToChartType(reportTemplate.Setting("ChartType")), reportTemplate.Settings.Contains("Description"))
						}.ToOutput(outputFormat));
		}
	}
}
