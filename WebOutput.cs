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
	public delegate void Progress (double progress);

	/// <summary>
	/// Serve web pages on demand. Also generates web pages to file for export or upload.
	/// </summary>
	public class WebOutput: IDisposable
	{
		WebServer ws;

		public Holders Leagues { get; set; }
		public Holder MostRecentHolder { get; set; }  // This is the league that owns the game with the most recent DateTime.
		public Game MostRecentGame { get; set; }
		public FixtureGame NextGame { get; set; }
		public ServerGame MostRecentServerGame { get; set; }

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

		static string RootPage(List<Holder> leagues)
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

		static ZoomReports OverviewReports(Holder holder, bool includeSecret, GameHyper gameHyper)
		{
			ZoomReports reports = new ZoomReports(holder.League.Title);
			
			if (holder.ReportTemplates == null || holder.ReportTemplates.Count == 0)
			{
				var rt = new ReportTemplate(ReportType.None, new string[] { "ChartType=bar with rug", "description" });
				reports.Add(Reports.TeamLadder(holder.League, includeSecret, rt));
				reports.Add(Reports.GamesList(holder.League, includeSecret, rt, gameHyper));

				if (!string.IsNullOrEmpty(holder.League.Title) && (holder.League.Title.Contains("Solo") || holder.League.Title.Contains("solo") || holder.League.Title.Contains("oubles") ||
				                                                   holder.League.Title.Contains("riples") || holder.League.Title.Contains("ripples") || holder.League.Title.Contains("rippples")))
					rt.ReportType = ReportType.Pyramid;
				else
					rt.ReportType = ReportType.GameGrid;

				reports.Add(Reports.GamesGrid(holder.League, includeSecret, rt, gameHyper));

				reports.Add(Reports.SoloLadder(holder.League, includeSecret, rt));
			}
			else
				foreach (ReportTemplate rt in holder.ReportTemplates)
			{
				bool description = rt.Settings.Contains("Description");
				switch (rt.ReportType)
				{
					case ReportType.TeamLadder:   reports.Add(Reports.TeamLadder(holder.League, includeSecret, rt)); break;
					case ReportType.TeamsVsTeams: reports.Add(Reports.TeamsVsTeams(holder.League, includeSecret, rt, gameHyper)); break;
					case ReportType.SoloLadder:   reports.Add(Reports.SoloLadder(holder.League, includeSecret, rt)); break;
					case ReportType.GameByGame:   reports.Add(Reports.GamesList(holder.League, includeSecret, rt, gameHyper)); break;
					case ReportType.GameGrid: case ReportType.Ascension: case ReportType.Pyramid: 
						reports.Add(Reports.GamesGrid(holder.League, includeSecret, rt, gameHyper)); break;
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

		static string OverviewPage(Holder holder, bool includeSecret, GameHyper gameHyper)
		{
			return OverviewReports(holder, includeSecret, gameHyper).ToSvg();
		}

		static string GamePage(League league, Game game)
		{
			ZoomReports reports = new ZoomReports();
			reports.Colors.BackgroundColor = Color.Empty;
			reports.Colors.OddColor = Color.Empty;
			reports.Add(Reports.OneGame(league, game));
			reports.Add(new ZoomHtmlInclusion("</div><br/><a href=\"index.html\">Index</a><div>"));
			return reports.ToSvg();
		}

		static string TeamPage(League league, bool includeSecret, LeagueTeam leagueTeam, GameHyper gameHyper)
		{
			ZoomReports reports = new ZoomReports();
			reports.Add(Reports.OneTeam(league, includeSecret, leagueTeam, DateTime.MinValue, DateTime.MaxValue, true, gameHyper));
			reports.Add(new ZoomHtmlInclusion("</div><br/><a href=\"index.html\">Index</a><div>"));
			return reports.ToSvg();
		}

		static string FixturePage(Fixture fixture, League league, string teamId = null)
		{
			string s = teamId;
			if (s != null && s.StartsWith("fixture", StringComparison.OrdinalIgnoreCase))
				s = s.Remove(0, 7);
			if (s != null && s.EndsWith(".html", StringComparison.OrdinalIgnoreCase))
				s = s.Remove(s.Length - 5, 5);

			int id;
			if (!int.TryParse(s, out id))
				id = -1;

			return FixturePage(fixture, league, id);
		}

		static string FixturePage(Fixture fixture, League league, int teamId)
		{
			ZoomReports reports = new ZoomReports();
			//if (teamId == -1) reports.Add(Reports.FixtureGrid(fixture, league));
			reports.Add(Reports.FixtureList(fixture, league, teamId));
			reports.Add(Reports.FixtureGrid(fixture, league, teamId));
			reports.Add(new ZoomHtmlInclusion("</div><br/><a href=\"index.html\">Index</a> <a href=\"fixture.html\">Fixture</a><div>"));
			return reports.ToHtml();
		}

		static string GameHyper(Game game)
		{
			return "games" + game.Time.ToString("yyyyMMdd", CultureInfo.InvariantCulture) + ".html#game" + game.Time.ToString("HHmm", CultureInfo.InvariantCulture);
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
					sb.Append(MostRecentServerGame.InProgress ? "Now Playing: " : "Just Played: " +
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
						return OverviewPage(holder, false, GameHyper);
					else
						return RootPage(Leagues);
				}
				else if (lastPart == "index.html")
					return OverviewPage(holder, false, GameHyper);
				else if (lastPart.StartsWith("game", StringComparison.OrdinalIgnoreCase))
				{
					DateTime dt = DateTime.ParseExact(lastPart.Substring(4, 12), "yyyyMMddHHmm", CultureInfo.InvariantCulture);
					Game game = holder.League.AllGames.Find(x => x.Time.Subtract(dt).TotalSeconds < 60);
					if (game == null)
						return string.Format(CultureInfo.InvariantCulture, "<html><body>Invalid game: <br>{0}</body></html>", request.RawUrl);
					else
						return GamePage(holder.League, game);
				}
				else if (lastPart.StartsWith("team", StringComparison.OrdinalIgnoreCase))
				{
					int teamId;
					if (int.TryParse(lastPart.Substring(4, 2), out teamId))
					{
						LeagueTeam leagueTeam = holder.League.Teams.Find(x => x.Id == teamId);
						if (leagueTeam == null)
							return string.Format(CultureInfo.InvariantCulture, "<html><body>Invalid team number: <br>{0}</body></html>", request.RawUrl);
						else
							return TeamPage(holder.League, false, leagueTeam, GameHyper);
					}
					else
						return string.Format(CultureInfo.InvariantCulture, "<html><body>Invalid team: <br>{0}</body></html>", request.RawUrl);
				}
				else if (lastPart.StartsWith("fixture", StringComparison.OrdinalIgnoreCase))
				{
					return FixturePage(holder.Fixture, holder.League, lastPart);
				}
				else if (lastPart == "now.html")
					return NowPage();
				else
					return string.Format(CultureInfo.InvariantCulture, "<html><body>Invalid path: <br>{0}</body></html>", request.RawUrl);
			}
			catch (Exception ex)
			{
				return "<html><body>\n" + ex.Message + "\n<br/><br/>\n" + ex.StackTrace + "</body></html>";
				throw;
			}
		}

		void DummyProgress(double progress) {}

		/// <summary>Generate reports for the selected leagues, and write them to disk.</summary>
		public void ExportReports(string path, bool includeSecret, List<Holder> selected, Progress progress = null)
		{
			if (path != null)
			{
				int denominator = selected.Count * 4 + 1;
				double numerator = 0.0;
				if (progress == null)
					progress = DummyProgress;

				using (StreamWriter sw = File.CreateText(Path.Combine(path, "index.html")))
					sw.Write(RootPage(selected));
				progress(++numerator / denominator);

				foreach (Holder holder in selected)
				{
					League league = holder.League;

					Directory.CreateDirectory(Path.Combine(path, holder.Key));

					using (StreamWriter sw = File.CreateText(Path.Combine(path, holder.Key, "index.html")))
						sw.Write(OverviewPage(holder, includeSecret, GameHyper));
					progress(++numerator / denominator);

					ExportGames(league, path, holder.Key);
					progress(++numerator / denominator);

					ExportPlayers(league, path, holder.Key);
					progress(++numerator / denominator);

					foreach (LeagueTeam leagueTeam in league.Teams)
						using (StreamWriter sw = File.CreateText(Path.Combine(path, holder.Key, "team" + leagueTeam.Id.ToString("D2", CultureInfo.InvariantCulture) + ".html")))
							sw.Write(TeamPage(league, includeSecret, leagueTeam, GameHyper));
					progress(++numerator / denominator);
				}
			}
		}

		void ExportGames(League league, string path, string key)
		{
			if (league.AllGames.Count > 0)
				for (DateTime day = league.AllGames[0].Time.Date; day <= league.AllGames[league.AllGames.Count - 1].Time.Date; day = day.AddDays(1)) 
				{
					ZoomReports reports = new ZoomReports(league.Title + " games on " + day.ToShortDateString());
					reports.Colors.BackgroundColor = Color.Empty;
					reports.Colors.OddColor = Color.Empty;
					league.AllGames.Sort();
				    bool heatMap = false;

					foreach (Game game in league.AllGames)
						if (game.Time.Date == day)
						{
							reports.Add(new ZoomHtmlInclusion("<a name=\"game" + game.Time.ToString("HHmm", CultureInfo.InvariantCulture) + "\">"));
							reports.Add(Reports.OneGame(league, game));
							if (game.ServerGame != null && game.ServerGame.Events.Count > 0)
							{
								reports.Add(Reports.GameHeatMap(league, game));
								var bitmap = Reports.GameWorm(league, game, true);
								string fileName = "score" + game.Time.ToString("yyyyMMdd_HHmm", CultureInfo.InvariantCulture) + ".png";
								bitmap.Save(Path.Combine(path, key, fileName), System.Drawing.Imaging.ImageFormat.Png);
								reports.Add(new ZoomHtmlInclusion("<img src=\"" + fileName + "\">"));
								heatMap = true;
							}
						}
					if (heatMap)
						reports.Add(new ZoomHtmlInclusion("</div><p>\u25cb and \u2b24 are hit and destroyed bases.<br/>\u2300 and \u29bb are one- and two-shot denies;<br/>\U0001f61e and \U0001f620 are one- and two-shot denied.<br/>\u25af and \u25ae are warning and termination.</p><div>"));

					reports.Add(new ZoomHtmlInclusion("</div><a href=\"index.html\">Index</a><div>"));
					if (reports.Count > 1)  // There were games this day.
						using (StreamWriter sw = File.CreateText(Path.Combine(path, key, "games" + day.ToString("yyyyMMdd", CultureInfo.InvariantCulture) + ".html")))
							sw.Write(reports.ToSvg());
				}
		}

		void ExportPlayers(League league, string path, string key)
		{
			var playerTeams = league.BuildPlayerTeamList();
			ZoomReports playerReports = new ZoomReports("Players in " + league.Title);
			playerReports.Colors.BackgroundColor = Color.Empty;
			playerReports.Colors.OddColor = Color.Empty;

			foreach (var pt in playerTeams)
			{
				playerReports.Add(new ZoomHtmlInclusion("<a name=\"player" + pt.Key.Id + "\">"));
				playerReports.Add(Reports.OnePlayer(league, pt.Key, pt.Value, GameHyper));
			}

			playerReports.Add(new ZoomHtmlInclusion("<br/><a href=\"index.html\">Index</a>"));

			if (playerReports.Count > 1)
				using (StreamWriter sw = File.CreateText(Path.Combine(path, key, "players.html")))
					sw.Write(playerReports.ToSvg());
		}

		/// <summary>Generate reports and write them to disk as TSV instead of as HTML/SVG.</summary>
		public void ExportReportsAsTsv(string path, bool includeSecret, List<Holder> selected)
		{
			if (path != null)
				foreach (Holder holder in selected)
			{
				League league = holder.League;

				Directory.CreateDirectory(Path.Combine(path, holder.Key));

				using (StreamWriter sw = File.CreateText(Path.Combine(path, holder.Key, "output.tsv")))
					sw.Write(OverviewReports(holder, includeSecret, game => "").ToTsv());

			}
		}

		/// <summary>Write fixtures for the selected leagues out as HTML/SVG.</summary>
		public void ExportFixtures(string path, List<Holder> selected)
		{
			if (path != null)
				foreach (Holder holder in selected)
			{
				Directory.CreateDirectory(Path.Combine(path, holder.Key));

				using (StreamWriter sw = File.CreateText(Path.Combine(path, holder.Key, "fixture.html")))
					sw.Write(FixturePage(holder.Fixture, holder.League));

				foreach (var ft in holder.Fixture.Teams)
					using (StreamWriter sw = File.CreateText(Path.Combine(path, holder.Key, "fixture" + ft.Id.ToString("D2", CultureInfo.InvariantCulture) + ".html")))
						sw.Write(FixturePage(holder.Fixture, holder.League, ft.Id));
			}
		}

		void UploadFile(WebClient client, string to, string from, string file)
		{
			Application.DoEvents();
			client.UploadFile(to + file, Path.Combine(from, file));
		}

		/// <summary>Upload files from the named path via FTP to the internet.</summary>
		public void UploadFiles(string uploadMethod, string uploadSite, string username, string password, string localPath, bool includeSecret, List<Holder> selected, Progress progress = null)
		{
			Cursor.Current = Cursors.WaitCursor;
			try
			{
				using (WebClient client = new WebClient())
				{
					client.Credentials = new NetworkCredential(username.Normalize(), password.Normalize());

					string url = uploadMethod + "://" + uploadSite + "/";
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
							progress((1.0 * i / files.Count() + h) / selected.Count);
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
		public void PackReport(string path, List<League> leagues)
		{
			if (path != null)
			{
				var soloGames = new List<Game>();
				foreach (var league in leagues)
					soloGames.AddRange(league.AllGames.Where(g => g.Title == "Round Robin" || g.Title == "Round 1" ||
					                                         g.Title == "Rep 1" || g.Title == "Repechage 1"));

				var reports = new ZoomReports();
				reports.Add(Reports.PackReport(leagues, soloGames, null, null, null, ChartType.KernelDensityEstimate | ChartType.Rug, true));

				using (StreamWriter sw = File.CreateText(Path.Combine(path, "packreport.html")))
					sw.Write(reports.ToSvg());
			}
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
				nowText = MostRecentServerGame.InProgress ? "Now Playing: " : "Just Played: " +
					MostRecentHolder.League.GameString(MostRecentServerGame);

			FixtureGame fg = MostRecentHolder == null ? null : MostRecentHolder.Fixture.BestMatch(MostRecentGame);
			if (fg != null && NextGame != null)
			{
				nowText += "\nUp Next:";
				foreach (var ft in fg.Teams)
					nowText += ft.Key.LeagueTeam + "; ";
			}

			if (fg != null)
				nowText += fg.ToString();

			return nowText;
		}
	}
}
