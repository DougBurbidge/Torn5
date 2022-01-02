using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Windows.Forms;
using Zoom;

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
		public static ZoomReports OverviewReports(Holder holder, bool includeSecret, GameHyper gameHyper)
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
		public static ZoomReport Report(League league, bool includeSecret, ReportTemplate rt)
		{
			bool description = rt.Settings.Contains("Description");
			switch (rt.ReportType)
			{
				case ReportType.TeamLadder: return Reports.TeamLadder(league, includeSecret, rt);
				case ReportType.TeamsVsTeams: return Reports.TeamsVsTeams(league, includeSecret, rt, ReportPages.GameHyper);
				case ReportType.ColourPerformance: return Reports.ColourReport(league, includeSecret, rt);
				case ReportType.SoloLadder: return Reports.SoloLadder(league, includeSecret, rt);
				case ReportType.GameByGame: return Reports.GamesList(league, includeSecret, rt, ReportPages.GameHyper);
				case ReportType.GameGrid:
				case ReportType.Ascension:
				case ReportType.Pyramid:
					return Reports.GamesGrid(league, includeSecret, rt, ReportPages.GameHyper);
				case ReportType.GameGridCondensed:
				case ReportType.PyramidCondensed:
					return Reports.GamesGridCondensed(league, includeSecret, rt, ReportPages.GameHyper);
				case ReportType.Packs:
					return Reports.PackReport(new List<League> { league }, league.Games(includeSecret), rt.Title, rt.From, rt.To,
						ChartTypeExtensions.ToChartType(rt.Setting("ChartType")), description, rt.Settings.Contains("Longitudinal"));
				case ReportType.Everything: return Reports.EverythingReport(league, rt.Title, rt.From, rt.To, description);
				default: return null;
			}
		}

		public static string OverviewPage(Holder holder, bool includeSecret, GameHyper gameHyper, OutputFormat outputFormat)
		{
			return OverviewReports(holder, includeSecret, ReportPages.GameHyper).ToOutput(outputFormat);
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
		public static string TeamPage(League league, bool includeSecret, LeagueTeam leagueTeam, GameHyper gameHyper, OutputFormat outputFormat)
		{
			ZoomReports reports = new ZoomReports(leagueTeam.Name)
			{
				Reports.OneTeam(league, includeSecret, leagueTeam, DateTime.MinValue, DateTime.MaxValue, true, ReportPages.GameHyper)
			};

			foreach (var player in leagueTeam.Players)
			{
				reports.Add(new ZoomHtmlInclusion("<a name=\"player" + player.Id + "\">"));
				reports.Add(Reports.OnePlayer(league, player, new List<LeagueTeam>() { leagueTeam }, gameHyper));
			}

			reports.Add(new ZoomHtmlInclusion("</div><br/><a href=\"fixture.html?team=" + leagueTeam.TeamId.ToString(CultureInfo.InvariantCulture) + "\">Fixture</a><br/><a href=\"index.html\">Index</a><div>"));

			return reports.ToOutput(outputFormat);
		}

		/// <summary>Display fixtures for a league, both as a list and as a grid.</summary>
		public static string FixturePage(Fixture fixture, League league, OutputFormat outputFormat = OutputFormat.Svg)
		{
			ZoomReports reports = new ZoomReports
			{
				Reports.FixtureList(fixture, league, GameHyper),
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

		/// <summary>Callback passed to various reports to generate HTML fragment with URL of a game.</summary>
		public static string GameHyper(Game game)
		{
			return "games" + game.Time.ToString("yyyyMMdd", CultureInfo.InvariantCulture) + ".html#game" + game.Time.ToString("HHmm", CultureInfo.InvariantCulture);
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
		public Func<int> Elapsed { get; set; }  // Learn you Func Prog on five minute quick!
		public Func<List<ServerGame>> Games { get; set; }
		public Action<ServerGame> PopulateGame { get; set; }
		public Func<string, List<LaserGamePlayer>> Players { get; set; }

		public WebOutput(int port = 8080)
		{
			if (port != 0)
			{
				ws = new WebServer(SendResponse, "http://localhost:" + port.ToString(CultureInfo.InvariantCulture) + "/");
				ws.Run();
			}
		}

		public void Dispose()
		{
			if (ws != null)
				ws.Stop();
		}

		/// <summary>Restart the web server, listening on a new port number.</summary>
		public void Restart(int port)
		{
			if (ws != null)
			{
				ws.Stop();
				if (port != 0)
				{
					ws = new WebServer(SendResponse, "http://localhost:" + port.ToString(CultureInfo.InvariantCulture) + "/");
					ws.Run();
				}
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
					sb.Append("<a href=\"" + MostRecentHolder.Key + "/game" +
					          MostRecentGame.Time.ToString("yyyyMMddHHmm", CultureInfo.InvariantCulture) + ".html\">Just Played</a>: " +
					          MostRecentHolder.League.GameString(MostRecentGame));
				else
					sb.Append((MostRecentServerGame.InProgress ? "Now Playing: " : Utility.JustPlayed(MostRecentServerGame.EndTime) + ": " ) +
					          MostRecentHolder.League.GameString(MostRecentServerGame));
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

		string ScoreboardPage(League league, OutputFormat outputFormat = OutputFormat.Svg)
		{
			if (MostRecentGame == null)
				Update();

			if (MostRecentGame == null)
				return NowPage();

			ZoomReports reports = new ZoomReports();
			reports.Colors.BackgroundColor = Color.Empty;
			reports.Colors.OddColor = Color.Empty;
			reports.Add(Reports.OneGame(league, MostRecentGame));

			if (NextGame != null)
			{
				StringBuilder sb = new StringBuilder();
				sb.Append("</div><br/><a href=\"fixture.html\">Up Next</a>:");

				foreach (var ft in NextGame.Teams)
					sb.Append(ft.Key.LeagueTeam.Name + "; ");
				sb.Length -= 2;
				sb.Append("<div>");
				reports.Add(new ZoomHtmlInclusion(sb.ToString()));
			}

			reports.Add(new ZoomHtmlInclusion("</div><br/><a href=\"index.html\">Index</a><div>"));
/*
			reports.Add(new ZoomHtmlInclusion(@"
<script>
</script>
"));
*/
			return reports.ToOutput(outputFormat);

		}

		string SendResponse(HttpListenerRequest request)
		{
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
						return ReportPages.OverviewPage(holder, false, ReportPages.GameHyper, OutputFormat.Svg);
					else
						return ReportPages.RootPage(Leagues);
				}
				else if (lastPart.EndsWith(".html"))
					return HtmlResponse(request.RawUrl, lastPart, holder);
				else
					return RestResponse(request);
			}
			catch (Exception ex)
			{
				return "<html><body>\n" + ex.Message + "\n<br/><br/>\n" + ex.StackTrace + "</body></html>";
				throw;
			}
		}

		string HtmlResponse(string rawUrl, string lastPart, Holder holder)
		{
			if (lastPart == "now.html")
				return NowPage();
			else if (holder == null)
				return string.Format(CultureInfo.InvariantCulture, "<html><body>Couldn't find a league key in \"<br>{0}\". Try <a href=\"now.html\">Now Playing</a> instead.</body></html>", rawUrl);
			else if (lastPart == "index.html")
				return ReportPages.OverviewPage(holder, false, ReportPages.GameHyper, OutputFormat.Svg);
			else if (lastPart.StartsWith("game", StringComparison.OrdinalIgnoreCase))
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
						return ReportPages.TeamPage(holder.League, false, leagueTeam, ReportPages.GameHyper, OutputFormat.Svg);
				}
				else
					return string.Format(CultureInfo.InvariantCulture, "<html><body>Invalid team: <br>{0}</body></html>", rawUrl);
			}
			else if (lastPart.StartsWith("fixture", StringComparison.OrdinalIgnoreCase))
			{
				return ReportPages.FixturePage(holder.Fixture, holder.League);
			}
			else if (lastPart == "scoreboard.html")
				return ScoreboardPage(holder.League);
			else
				return string.Format(CultureInfo.InvariantCulture, "<html><body>Invalid path: <br>{0}</body></html>", rawUrl);
		}

		private string RestGame(string gameTime)
		{
			var sb = new StringBuilder();
			if (serverGames == null)
				serverGames = Games == null ? null : Games();
			if (serverGames == null)
				return JsonSerializer.Serialize(new Error { Message = "Games list not initialised. (Perhaps no lasergame server is configured.)" });

			var game = serverGames.Find(g => g.Time.ToString("s") == gameTime);
			if (game == null)
				return JsonSerializer.Serialize(new Error { Message = "Game " + gameTime + " not found." });

			PopulateGame(game);
			return JsonSerializer.Serialize<ServerGame>(game);
		}

		private string RestGames()
		{
			var serverGames = Games == null ? null :Games();
			if (serverGames == null)
				return JsonSerializer.Serialize(new Error { Message = "Games list not initialised. (Perhaps no lasergame server is configured.)" });
			else
				return JsonSerializer.Serialize<List<ServerGame>>(Games());

		}

		string RestPlayers(string mask)
		{
			var sb = new StringBuilder();
			sb.Append("\"players\":[\n");
			foreach (var player in Players(mask))
			{
				player.ToJson(sb, 1);
				sb.Append(",\n");
			}
			sb.Length -= 2;
			sb.Append("\n]\n");
			return sb.ToString();
		}

		string RestResponse(HttpListenerRequest request)
		{
			string rawUrl = request.RawUrl;
			if (rawUrl.EndsWith("elapsed"))
				return JsonSerializer.Serialize<int>(Elapsed());
			else if (rawUrl.EndsWith("games"))
				return RestGames();
			else if (rawUrl.Contains("game2"))
				return RestGame(rawUrl.Substring(rawUrl.IndexOf("game") + 4));  // return one detailed game: all the players, all the details.
			else if (rawUrl.EndsWith("players"))
				return RestPlayers("");  // return the list of all players available from this lasergame server.
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
						sw.Write(ReportPages.OverviewPage(holder, includeSecret, ReportPages.GameHyper, holder.ReportTemplates.OutputFormat));
					myProgress.Increment(holder.League.Title + " Overview page exported.");

					foreach (LeagueTeam leagueTeam in holder.League.Teams)
					{
						using (StreamWriter sw = File.CreateText(Path.Combine(path, holder.Key, "team" + leagueTeam.TeamId.ToString("D2", CultureInfo.InvariantCulture) + "." + holder.ReportTemplates.OutputFormat.ToExtension())))
							sw.Write(ReportPages.TeamPage(holder.League, includeSecret, leagueTeam, ReportPages.GameHyper, holder.ReportTemplates.OutputFormat));
						myProgress.Advance(1 / holder.League.Teams.Count, "Team " + leagueTeam.Name + " page exported.");
					}
					myProgress.Advance(0, "Team pages exported.");

					ExportGames(holder, path, myProgress);
					myProgress.Advance(0, "Games pages exported.");
				}
			}
		}

		static void ExportGames(Holder holder, string path, Progress progress)
		{
			League league = holder.League;

			var dates = league.AllGames.Select(g => g.Time.Date).Distinct().ToList();
			foreach (var date in dates)
			{
				string fileName = Path.Combine(path, holder.Key, "games" + date.ToString("yyyyMMdd", CultureInfo.InvariantCulture) + 
                                                      "." + holder.ReportTemplates.OutputFormat.ToExtension());

				var dayGames = league.AllGames.Where(g => g.Time.Date == date);
				if (dayGames.Any(g => !g.Reported) || !File.Exists(fileName))  // Some of the games for this day are not marked as reported. Report on them.
				{
					ZoomReports reports = new ZoomReports(league.Title + " games on " + date.ToShortDateString());
					reports.Colors.BackgroundColor = Color.Empty;
					reports.Colors.OddColor = Color.Empty;
					league.AllGames.Sort();
					bool detailed = false;

					var rt = new ReportTemplate() { From = date, To = date.AddSeconds(86399) };
					reports.Add(Reports.GamesToc(league, false, rt, ReportPages.GameHyper));
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
						var sb = new StringBuilder("</div><p>");
						if (eventsUsed.Contains(30) || eventsUsed.Contains(31)) sb.Append("\u25cb and \u2b24 are hit and destroyed bases.<br/>");
						if (eventsUsed.Contains(1403) || eventsUsed.Contains(1404)) sb.Append("\U0001f61e and \U0001f620 are one- and two-shot denied.<br/>");
						if (eventsUsed.Contains(1401) || eventsUsed.Contains(1402)) sb.Append("\u2300 and \u29bb are denied another player.<br/>");
						if (eventsUsed.Contains(28)) sb.Append("\U0001f7e8 is warning (yellow card). ");
						if (eventsUsed.Contains(28) && !eventsUsed.Contains(29)) sb.Append("<br/>");
						if (eventsUsed.Contains(29)) sb.Append("\U0001f7e5 is termination (red card).<br/>");
						if (eventsUsed.Contains(32)) sb.Append("\U0001f480 is player eliminated.<br/>");
						if (eventsUsed.Contains(33) && !eventsUsed.Contains(34) && !eventsUsed.Any(t => t >= 37 && t <= 46)) sb.Append("! is hit by base, or player self-denied.<br/>");
						if (eventsUsed.Contains(34) || eventsUsed.Any(t => t >= 37 && t <= 46)) sb.Append("! is hit by base or mine, or player self-denied, or player tagged target.<br/>");
						sb.Append("\u00B7 shows each minute elapsed.<br/>Tags+ includes shots on bases and teammates.</p><div>");
						reports.Add(new ZoomHtmlInclusion(sb.ToString()));
					}

					reports.Add(new ZoomHtmlInclusion("</div><a href=\"index.html\">Index</a><div>"));
					if (reports.Count > 1)  // There were games this day.
						using (StreamWriter sw = File.CreateText(fileName))
							sw.Write(reports.ToOutput(holder.ReportTemplates.OutputFormat));

					progress.Advance(1.0 / dates.Count, "Exported games for " + date.ToShortDateString());
				}
			}
		}

		/// <summary>Write out fixtures for the selected leagues.</summary>
		public static void ExportFixtures(string path, List<Holder> leagues)
		{
			if (path != null)
				foreach (Holder holder in leagues)
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
		public static void UploadFiles(string uploadMethod, string uploadSite, string username, string password, string localPath, bool includeSecret, List<Holder> selected, ShowProgress progress = null)
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
					                                         g.Title == "Rep 1" || g.Title == "Repechage 1"));
				
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
	}
}
