using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Torn;
using Zoom;

namespace Torn.Report
{
	public delegate void Progress (double progress, string status = "");

	/// <summary>Build web pages for WebOutput and ExportPages.</summary>
	public class ReportPages
	{
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

		public static ZoomReports OverviewReports(Holder holder, bool includeSecret, GameHyper gameHyper)
		{
			ZoomReports reports = new ZoomReports(holder.League.Title);
			
			if (holder.ReportTemplates == null || holder.ReportTemplates.Count == 0)
			{
				var rt = new ReportTemplate(ReportType.None, new string[] { "ChartType=bar with rug", "description" });
				reports.Add(Reports.TeamLadder(holder.League, includeSecret, rt));
				reports.Add(Reports.GamesList(holder.League, includeSecret, rt, ReportPages.GameHyper));

				if (!string.IsNullOrEmpty(holder.League.Title) && (holder.League.Title.Contains("Solo") || holder.League.Title.Contains("solo") || holder.League.Title.Contains("oubles") ||
				                                                   holder.League.Title.Contains("riples") || holder.League.Title.Contains("ripples") || holder.League.Title.Contains("rippples")))
					rt.ReportType = ReportType.Pyramid;
				else
					rt.ReportType = ReportType.GameGrid;

				reports.Add(Reports.GamesGrid(holder.League, includeSecret, rt, ReportPages.GameHyper));

				reports.Add(Reports.SoloLadder(holder.League, includeSecret, rt));
			}
			else
				foreach (ReportTemplate rt in holder.ReportTemplates)
			{
				bool description = rt.Settings.Contains("Description");
				switch (rt.ReportType)
				{
					case ReportType.TeamLadder:   reports.Add(Reports.TeamLadder(holder.League, includeSecret, rt)); break;
					case ReportType.TeamsVsTeams: reports.Add(Reports.TeamsVsTeams(holder.League, includeSecret, rt, ReportPages.GameHyper)); break;
					case ReportType.SoloLadder:   reports.Add(Reports.SoloLadder(holder.League, includeSecret, rt)); break;
					case ReportType.GameByGame:   reports.Add(Reports.GamesList(holder.League, includeSecret, rt, ReportPages.GameHyper)); break;
					case ReportType.GameGrid: case ReportType.Ascension: case ReportType.Pyramid: 
						reports.Add(Reports.GamesGrid(holder.League, includeSecret, rt, ReportPages.GameHyper)); break;
					case ReportType.GameGridCondensed: case ReportType.PyramidCondensed: 
						reports.Add(Reports.GamesGridCondensed(holder.League, includeSecret, rt, ReportPages.GameHyper)); break;
					case ReportType.Packs:
						var x = new List<League>();
						x.Add(holder.League);
						reports.Add(Reports.PackReport(x, holder.League.Games(includeSecret), rt.Title, rt.From, rt.To, ChartTypeExtensions.ToChartType(rt.Setting("ChartType")), description));
						break;
					case ReportType.Everything: reports.Add(Reports.EverythingReport(holder.League, rt.Title, rt.From, rt.To, description)); break;
				}
			}

			reports.Add(new ZoomHtmlInclusion("</div><br/><a href=\"../now.html\">Now Playing</a><br/><a href=\"fixture.html\">Fixture</a><br/><a href=\"/\">Index</a><div>"));

			return reports;
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

		public static string TeamPage(League league, bool includeSecret, LeagueTeam leagueTeam, GameHyper gameHyper, OutputFormat outputFormat)
		{
			ZoomReports reports = new ZoomReports();
			reports.Add(Reports.OneTeam(league, includeSecret, leagueTeam, DateTime.MinValue, DateTime.MaxValue, true, ReportPages.GameHyper));
			reports.Add(new ZoomHtmlInclusion("</div><br/><a href=\"index.html\">Index</a><div>"));
			return reports.ToOutput(outputFormat);
		}

		public static string FixturePage(Fixture fixture, League league, OutputFormat outputFormat = OutputFormat.Svg)
		{
			ZoomReports reports = new ZoomReports();
			reports.Add(Reports.FixtureList(fixture, league));
			reports.Add(Reports.FixtureGrid(fixture, league));
			reports.Add(new ZoomHtmlInclusion(@"
</div><br/><a href=""index.html"">Index</a> <a href=""fixture.html"">Fixture</a>

<script>
  var url = new URL(window.location.href);
  var team = url.searchParams.get('team');

  if (team) {
    var tables = document.querySelectorAll('.fixturelist');
      for (const table of tables)
        for (const tbody of table.querySelectorAll('tbody'))
          for (const tr of tbody.querySelectorAll('tr:not(.t' + team + ')'))
            tr.style = 'display:none';

    var tables = document.querySelectorAll('.fixturegrid');
      for (const table of tables)
        for (const tr of table.querySelectorAll('tr'))
          for (const td of tr.querySelectorAll('td.t' + team))
            if (td.innerHTML == '')
              td.style.backgroundColor = 'gainsboro';
  }
</script>
"));
			return outputFormat == OutputFormat.Svg ? reports.ToHtml() : reports.ToOutput(outputFormat);
		}

		public static string GameHyper(Game game)
		{
			return "games" + game.Time.ToString("yyyyMMdd", CultureInfo.InvariantCulture) + ".html#game" + game.Time.ToString("HHmm", CultureInfo.InvariantCulture);
		}
	}

	/// <summary>Serve web pages on demand.</summary>
	public class WebOutput: IDisposable
	{
		WebServer ws;

		public Holders Leagues { get; set; }
		public Holder MostRecentHolder { get; set; }  // This is the league that owns the game with the most recent DateTime.
		public Game MostRecentGame { get; set; }
		public FixtureGame NextGame { get; set; }
		public ServerGame MostRecentServerGame { get; set; }
		public Func<int> Elapsed { get; set; }  // Learn you Func Prog on five minute quick!
		public Func<List<ServerGame>> Games { get; set; }

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
				sb.Remove(sb.Length - 2, 2);
			}
			// Add a hyperlink for each team: "team" + team.Id.ToString("D2", CultureInfo.InvariantCulture)

			sb.Append("</p></body></html>");
			return sb.ToString();
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
					return RestResponse(request.RawUrl);
			}
			catch (Exception ex)
			{
				return "<html><body>\n" + ex.Message + "\n<br/><br/>\n" + ex.StackTrace + "</body></html>";
				throw;
			}
		}

		string HtmlResponse(string rawUrl, string lastPart, Holder holder)
		{
			if (lastPart == "index.html")
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
				int teamId;
				if (int.TryParse(lastPart.Substring(4, 2), out teamId))
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
			else if (lastPart == "now.html")
				return NowPage();
			else
				return string.Format(CultureInfo.InvariantCulture, "<html><body>Invalid path: <br>{0}</body></html>", rawUrl);
		}

		string RestGames()
		{
			var sb = new StringBuilder();
			sb.Append("\n  \"games\":[\n");
			foreach (var game in Games())
				game.ToJson(sb, 1);
			sb.Append("]\n");
			return sb.ToString();
		}

		string RestResponse(string rawUrl)
		{
			if (rawUrl == "elapsed")
				return "{\n  \"elapsed\": " + Elapsed().ToString() + "\n}";
			else if (rawUrl == "games")
				return "{\n  \"games\": " + RestGames() + "\n}";
			else if (rawUrl == "game")
				return "{\n  \"game\": 0\n}";  // return one detailed game: all the players, all the details.
			else if (rawUrl == "players")
				return "{\n  \"players\": 0\n}";  // return the list of all players available rom this lasergame server.
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
		static void DummyProgress(double progress, string status = "") {}

		/// <summary>Generate reports for the selected leagues, and write them to disk.</summary>
		public static void ExportReports(string path, bool includeSecret, List<Holder> selected, Progress progress = null)
		{
			if (path != null)
			{
				int denominator = selected.Count * 4 + 1;
				double numerator = 0.0;
				if (progress == null)
					progress = DummyProgress;

				if (selected.Any())
					using (StreamWriter sw = File.CreateText(Path.Combine(path, "index." + selected[0].ReportTemplates.OutputFormat.ToExtension())))
						sw.Write(ReportPages.RootPage(selected));
				progress(++numerator / denominator, "Root page exported.");

				foreach (Holder holder in selected)
				{
					Directory.CreateDirectory(Path.Combine(path, holder.Key));

					using (StreamWriter sw = File.CreateText(Path.Combine(path, holder.Key, "index." + holder.ReportTemplates.OutputFormat.ToExtension())))
						sw.Write(ReportPages.OverviewPage(holder, includeSecret, ReportPages.GameHyper, holder.ReportTemplates.OutputFormat));
					progress(++numerator / denominator, "Overview page exported.");

					ExportPlayers(holder, path);
					progress(++numerator / denominator, "Players pages exported.");

					foreach (LeagueTeam leagueTeam in holder.League.Teams)
						using (StreamWriter sw = File.CreateText(Path.Combine(path, holder.Key, "team" + leagueTeam.TeamId.ToString("D2", CultureInfo.InvariantCulture) + "." + holder.ReportTemplates.OutputFormat.ToExtension())))
							sw.Write(ReportPages.TeamPage(holder.League, includeSecret, leagueTeam, ReportPages.GameHyper, holder.ReportTemplates.OutputFormat));
					progress(++numerator / denominator, "Team pages exported.");

					ExportGames(holder, path);
					progress(++numerator / denominator, "Games pages exported.");
				}
			}
		}

		static void ExportGames(Holder holder, string path)
		{
			League league = holder.League;

			var dates = league.AllGames.Select(g => g.Time.Date).Distinct().ToList();
			foreach (var date in dates)
			{
				var dayGames = league.AllGames.Where(g => g.Time.Date == date);
				if (dayGames.Any(g => !g.Reported))  // Some of the games for this day are not marked as reported. Report on them.
				{
					ZoomReports reports = new ZoomReports(league.Title + " games on " + date.ToShortDateString());
					reports.Colors.BackgroundColor = Color.Empty;
					reports.Colors.OddColor = Color.Empty;
					league.AllGames.Sort();
				    bool heatMap = false;

					foreach (Game game in dayGames)
					{
						reports.Add(new ZoomHtmlInclusion("<a name=\"game" + game.Time.ToString("HHmm", CultureInfo.InvariantCulture) + "\">"));
						reports.Add(Reports.OneGame(league, game));
						if (game.ServerGame != null && game.ServerGame.Events.Any() && !game.ServerGame.InProgress)
						{
							reports.Add(Reports.GameHeatMap(league, game));
							string fileName = "score" + game.Time.ToString("yyyyMMdd_HHmm", CultureInfo.InvariantCulture) + ".png";
							if (!game.Reported)
							{
								var bitmap = Reports.GameWorm(league, game, true);
								string filePath = Path.Combine(path, holder.Key, fileName);
								if (bitmap != null && bitmap.Height > 1 || !File.Exists(filePath))
									bitmap.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
							}
							reports.Add(new ZoomHtmlInclusion("<img src=\"" + fileName + "\">"));
							game.Reported = true;
							heatMap = true;
						}
					}
					if (heatMap)
						reports.Add(new ZoomHtmlInclusion("</div><p>\u25cb and \u2b24 are hit and destroyed bases.<br/>\u2300 and &olcross; are one- and two-shot denies;<br/>\U0001f61e and \U0001f620 are one- and two-shot denied.<br/>\u25af and \u25ae are warning and termination.<br/>Tags+ includes shots on bases and teammates.</p><div>"));

					reports.Add(new ZoomHtmlInclusion("</div><a href=\"index.html\">Index</a><div>"));
					if (reports.Count > 1)  // There were games this day.
						using (StreamWriter sw = File.CreateText(Path.Combine(path, holder.Key, "games" + date.ToString("yyyyMMdd", CultureInfo.InvariantCulture) + 
						                                                      "." + holder.ReportTemplates.OutputFormat.ToExtension())))
							sw.Write(reports.ToOutput(holder.ReportTemplates.OutputFormat));
				}
			}
		}

		static void ExportPlayers(Holder holder, string path)
		{
			League league = holder.League;

			var playerTeams = league.BuildPlayerTeamList();
			ZoomReports playerReports = new ZoomReports("Players in " + league.Title);
			playerReports.Colors.BackgroundColor = Color.Empty;
			playerReports.Colors.OddColor = Color.Empty;

			foreach (var pt in playerTeams)
			{
				playerReports.Add(new ZoomHtmlInclusion("<a name=\"player" + pt.Key.Id + "\">"));
				playerReports.Add(Reports.OnePlayer(league, pt.Key, pt.Value, ReportPages.GameHyper));
			}

			playerReports.Add(new ZoomHtmlInclusion("<br/><a href=\"index.html\">Index</a>"));

			if (playerReports.Count > 1)
				using (StreamWriter sw = File.CreateText(Path.Combine(path, holder.Key, "players." + holder.ReportTemplates.OutputFormat.ToExtension())))
					sw.Write(playerReports.ToOutput(holder.ReportTemplates.OutputFormat));
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
		public static void UploadFiles(string uploadMethod, string uploadSite, string username, string password, string localPath, bool includeSecret, List<Holder> selected, Progress progress = null)
		{
			Cursor.Current = Cursors.WaitCursor;
			try
			{
				using (WebClient client = new WebClient())
				{
					client.Credentials = new NetworkCredential(username.Normalize(), password.Normalize());

					string url = uploadMethod + "://" + uploadSite;
					if (url.Last() != '/')
						url += '/';

					if (progress == null)
						progress = DummyProgress;

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
							progress((1.0 * i / files.Count() + h) / selected.Count, "Uploaded " + files[i].Name);
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

				var reports = new ZoomReports();
				reports.Add(Reports.PackReport(leagues, round1Games, reportTemplate.Title, reportTemplate.From, reportTemplate.To, ChartTypeExtensions.ToChartType(reportTemplate.Setting("ChartType")), true));

				using (StreamWriter sw = File.CreateText(Path.Combine(path, "packreport." + outputFormat.ToExtension())))
					sw.Write(reports.ToOutput(outputFormat));
			}
		}
	}
}
