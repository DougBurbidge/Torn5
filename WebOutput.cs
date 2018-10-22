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
	/// <summary>
	/// Description of Class1.
	/// </summary>
	public class WebOutput: IDisposable
	{
		WebServer ws;

		public Holders Leagues { get; set; }
		public Holder MostRecentHolder { get; set; }  // This is the league that owns the game with the most recent DateTime.
		public Game MostRecentGame { get; set; }
		public FixtureGame NextGame { get; set; }
		public bool Playing { get; set; }

		public WebOutput(int port = 8080)
		{
			ws = new WebServer(SendResponse, "http://localhost:" + port.ToString(CultureInfo.InvariantCulture) + "/");
	        ws.Run();
	        Playing = true;
		}

		public void Dispose()
		{
			ws.Stop();
		}

		static string RootPage(List<Holder> leagues)
		{
			if (leagues.Count == 0)
    			return "<html><body>No league file loaded.</body></html>";

			StringBuilder sb = new StringBuilder();
			sb.Append("<html><head><title>Leagues</title></head><body>\n");

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
	    		reports.Add(Reports.TeamLadder(holder.League, includeSecret, null, null, null, ChartType.Bar | ChartType.Rug, false, false, false, true));
				reports.Add(Reports.GamesList(holder.League, includeSecret, DateTime.MinValue, DateTime.MaxValue, true, gameHyper));
				if (!string.IsNullOrEmpty(holder.League.Title) && (holder.League.Title.Contains("Solo") || holder.League.Title.Contains("solo") || holder.League.Title.Contains("oubles") ||
				                                                   holder.League.Title.Contains("riples") || holder.League.Title.Contains("ripples") || holder.League.Title.Contains("rippples")))
					reports.Add(Reports.GamesGrid(holder.League, includeSecret, null, null, null, true, GridType.Pyramid, gameHyper));
				else
				{
					reports.Add(Reports.GamesGrid(holder.League, includeSecret, null, null, null, true, GridType.GameGrid, gameHyper));
					reports.Add(Reports.GamesGrid(holder.League, includeSecret, null, null, null, true, GridType.Ascension, gameHyper));
				}
				reports.Add(Reports.SoloLadder(holder.League, includeSecret, false, DateTime.MinValue, DateTime.MaxValue, null, ChartType.Bar | ChartType.Rug, true));
			}
			else
				foreach (ReportTemplate r in holder.ReportTemplates)
			{
				bool description = r.Settings.Contains("Description");
				switch (r.ReportType)
				{
						case ReportType.TeamLadder: reports.Add(Reports.TeamLadder(holder.League, includeSecret, r.From, r.To, r.Drops, ChartTypeExtensions.ToChartType(r.Setting("ChartType")),
						                                                  r.Settings.Contains("ScaleGames"), false, r.Settings.Contains("ShowColours"), description)); break;
					case ReportType.TeamsVsTeams: reports.Add(Reports.TeamsVsTeams(holder.League, includeSecret, r.From, r.To, description, gameHyper)); break;
					case ReportType.SoloLadder: reports.Add(Reports.SoloLadder(holder.League, includeSecret, r.Settings.Contains("ShowComments"), r.From, r.To, r.Drops, ChartTypeExtensions.ToChartType(r.Setting("ChartType")), description)); break;
					case ReportType.GameByGame: reports.Add(Reports.GamesList(holder.League, includeSecret, r.From, r.To, description, gameHyper)); break;
					case ReportType.GameGrid:   reports.Add(Reports.GamesGrid(holder.League, includeSecret, r.From, r.To, r.Drops, description, GridType.GameGrid, gameHyper)); break;
					case ReportType.Ascension:  reports.Add(Reports.GamesGrid(holder.League, includeSecret, r.From, r.To, r.Drops, description, GridType.Ascension, gameHyper)); break;
					case ReportType.Pyramid:    reports.Add(Reports.GamesGrid(holder.League, includeSecret, r.From, r.To, r.Drops, description, GridType.Pyramid, gameHyper)); break;
					case ReportType.Packs:
						var x = new List<League>();
						x.Add(holder.League);
						reports.Add(Reports.PackReport(x, holder.League.Games(includeSecret), r.From, r.To, ChartTypeExtensions.ToChartType(r.Setting("ChartType")), description));
						break;
					case ReportType.Everything: reports.Add(Reports.EverythingReport(holder.League, r.From, r.To, description)); break;
				}
			}

			reports.Add(new ZoomHtmlInclusion("<br/><a href=\"../now.html\">Now Playing</a><br/><a href=\"fixture.html\">Fixture</a><br/><a href=\"/\">Index</a>"));

			return reports;
		}

		static string OverviewPage(Holder holder, bool includeSecret, GameHyper gameHyper)
		{
			return OverviewReports(holder, includeSecret, gameHyper).ToSvg();
		}

		static string GamePage(League league, Game game)
		{
			ZoomReports reports = new ZoomReports();
			reports.Add(Reports.OneGame(league, game));
			reports.Add(new ZoomHtmlInclusion("<br/><a href=\"index.html\">Index</a>"));
			return reports.ToSvg();
		}

		static string TeamPage(League league, bool includeSecret, LeagueTeam leagueTeam, GameHyper gameHyper)
		{
			ZoomReports reports = new ZoomReports();
			reports.Add(Reports.OneTeam(league, includeSecret, leagueTeam, DateTime.MinValue, DateTime.MaxValue, true, gameHyper));
			reports.Add(new ZoomHtmlInclusion("<br/><a href=\"index.html\">Index</a>"));
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
			reports.Add(new ZoomHtmlInclusion("<br/><a href=\"index.html\">Index</a> <a href=\"fixture.html\">Fixture</a>"));
			return reports.ToHtml();
		}

		static string GameHyper(Game game)
		{
			return "games" + game.Time.ToString("yyyyMMdd", CultureInfo.InvariantCulture) + ".html#game" + game.Time.ToString("HHmmss", CultureInfo.InvariantCulture);
		}

		string NowPage()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("<html><body><p>");

			if (MostRecentGame == null)
				sb.Append("No game found.");
			else
			{
				if (Playing)
					sb.Append("Now Playing: ");
				else
					sb.Append("<a href=\"" + MostRecentHolder.Key + "/game" + 
					          MostRecentGame.Time.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture) + ".html\">Just Played</a>: ");

				sb.Append(MostRecentGame.ToString());
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
					DateTime dt = DateTime.ParseExact(lastPart.Substring(4, 14), "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
					Game game = holder.League.AllGames.Find(x => x.Time == dt);
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

		/// <summary>Generate reports for the selected leagues, and write them to disk.</summary>
		public void ExportReports(string path, bool includeSecret, List<Holder> selected)
		{
			if (path != null)
			{
				using (StreamWriter sw = File.CreateText(Path.Combine(path, "index.html")))
					sw.Write(RootPage(selected));
				
				foreach (Holder holder in selected)
				{
					League league = holder.League;

					Directory.CreateDirectory(Path.Combine(path, holder.Key));

					using (StreamWriter sw = File.CreateText(Path.Combine(path, holder.Key, "index.html")))
						sw.Write(OverviewPage(holder, includeSecret, GameHyper));

					if (league.AllGames.Count > 0)
						for (DateTime day = league.AllGames[0].Time.Date; day <= league.AllGames[league.AllGames.Count - 1].Time.Date; day = day.AddDays(1))
						{
							ZoomReports reports = new ZoomReports();
							league.AllGames.Sort();
							foreach (Game game in league.AllGames)
								if (game.Time.Date == day)
								{
									reports.Add(new ZoomHtmlInclusion("<a name=\"game" + game.Time.ToString("HHmmss", CultureInfo.InvariantCulture) + "\"><div/>"));
									reports.Add(Reports.OneGame(league, game));
								}
							reports.Add(new ZoomHtmlInclusion("<br/><a href=\"index.html\">Index</a>"));
							if (reports.Count > 1)  // There were games this day.
								using (StreamWriter sw = File.CreateText(Path.Combine(path, holder.Key, "games" + day.ToString("yyyyMMdd", CultureInfo.InvariantCulture) + ".html")))
									sw.Write(reports.ToSvg());
						}

					foreach (LeagueTeam leagueTeam in league.Teams)
						using (StreamWriter sw = File.CreateText(Path.Combine(path, holder.Key, "team" + leagueTeam.Id.ToString("D2", CultureInfo.InvariantCulture) + ".html")))
							sw.Write(TeamPage(league, includeSecret, leagueTeam, GameHyper));
				}
			}
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

		void UploadFile(WebClient client, string to, string from, string file, double progress)
		{
			//labelStatus.Text = file;
			//progressBar1.Value = (int)(progress * progressBar1.Maximum);
			Application.DoEvents();
		    client.UploadFile(to + file, Path.Combine(from, file));
		}

		/// <summary>Upload files from the named path via FTP to the internet.</summary>
		public void UploadFiles(string path, bool includeSecret, List<Holder> selected)
		{
			Cursor.Current = Cursors.WaitCursor;
			//progressBar1.Visible = true;
			try
			{
				using (WebClient client = new WebClient())
				{
					client.Credentials = new NetworkCredential("doug@dougburbidge.com".Normalize(), "swordfish".Normalize());

					UploadFile(client, "ftp://dougburbidge.com/", path, "index.html", 0);

					foreach (Holder holder in selected)
					{
						string key = holder.Key;

						DirectoryInfo di = new DirectoryInfo(Path.Combine(path, key));

// Create a directory on FTP site:
//					    WebRequest wr = WebRequest.Create("ftp://dougburbidge.com/" + key);
//						wr.Method = WebRequestMethods.Ftp.MakeDirectory;
//						wr.Credentials = client.Credentials;
//						wr.GetResponse();

						FileInfo[] files = di.GetFiles("*.html");
						for (int i = 0; i < files.Count(); i++)
							UploadFile(client, "ftp://dougburbidge.com/" + key + "/", Path.Combine(path, key), files[i].Name, 1.0 * i / files.Count());
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
				//labelStatus.Text = "";
				//progressBar1.Visible = false;
			}
					
		}

		/// <summary>Write a single pack report incorporating data from al the selected leagues.</summary>
		public void PackReport(string path, List<League> leagues)
		{
			if (path != null)
			{
				var soloGames = new List<Game>();
				foreach (var league in leagues)
					soloGames.AddRange(league.AllGames.Where(g => g.Title == "Round Robin" || g.Title == "Round 1" || 
					                                      g.Title == "Rep 1" || g.Title == "Repechage 1"));

				var reports = new ZoomReports();
				reports.Add(Reports.PackReport(leagues, soloGames, null, null, ChartType.KernelDensityEstimate | ChartType.Rug, true));

				using (StreamWriter sw = File.CreateText(Path.Combine(path, "packreport.html")))
					sw.Write(reports.ToSvg());
			}
		}

		/// <summary>Restart the web server, listening on a new port number.</summary>
		public void Restart(int port)
		{
			ws.Stop();
			ws = new WebServer(SendResponse, "http://localhost:" + port.ToString(CultureInfo.InvariantCulture) + "/");
	        ws.Run();
		}

		/// <summary>Call this when time remaining in the game has changed.</summary>
		public void Tick(TimeSpan timeElapsed)
		{
			bool playingChanged = (timeElapsed == TimeSpan.Zero ^ !Playing);
			Playing = timeElapsed != TimeSpan.Zero;
			if (playingChanged && Leagues != null)
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
			string nowText = Playing ? "Now Playing: " : "Just Played: ";

			if (MostRecentGame != null)
				nowText += MostRecentGame.ToString();

			
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
