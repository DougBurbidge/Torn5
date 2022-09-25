using System;
using System.Collections.Generic;
using System.Globalization;
using System.Drawing;
using System.Linq;
using System.Text;
using Zoom;

namespace Torn.Report
{
	/// <summary>Store the number or percentage of game scores or score ratios that should be dropped from each player's or team's list before calculating their average.</summary>
	public class Drops
	{
		/// <summary>Percentage of games to drop from bottom end of list -- e.g. if set to 10, the players'/teams' worst 10% of games will be dropped.</summary>
		public double PercentWorst { get; set; }
		/// <summary>Percentage of games to drop from top end of list -- e.g. if set to 10, the players'/teams' best 10% of games will be dropped.</summary>
		public double PercentBest { get; set; }
		/// <summary>Number of games to drop from bottom end of list -- e.g. if set to 2, the players'/teams' worst 2 games will be dropped.</summary>
		public int CountWorst { get; set; }
		/// <summary>Number of games to drop from bottom end of list -- e.g. if set to 2, the players'/teams' best 2 games will be dropped.</summary>
		public int CountBest { get; set; }
		
		public Drops() 
		{
			PercentWorst = 0.0;
			PercentBest = 0.0;
			CountWorst = 0;
			CountBest = 0;
		}

		public int CountAfterDrops(int count)
		{
			return count - DropWorst(count) - DropBest(count);
		}

		public List<T> Drop<T>(List<T> scores)
		{
			scores.Sort();

			int count = scores.Count();

			int dropBest = DropBest(count);
			if (dropBest > 0)
				scores.RemoveRange(count - dropBest, dropBest);

			int dropWorst = DropWorst(count);
			if (dropWorst > 0)
				scores.RemoveRange(0, dropWorst);

			return scores;
		}

		public int DropWorst(int count)
		{
			return Math.Min(Math.Max(CountWorst, (int)Math.Truncate(PercentWorst / 100 * count)), count - 1);
		}

		public int DropBest(int count)
		{
			return Math.Min(Math.Max(CountBest, (int)Math.Truncate(PercentBest / 100 * count)), count - 1);
		}
		
		public bool HasDrops()
		{
			return PercentWorst > 0 || PercentBest > 0 || CountWorst > 0 || CountBest > 0;
		}
	}

	/// <summary>Used to hold data for one team while calculating and sorting a team ladder.</summary>
	public class TeamLadderEntry : IComparable<TeamLadderEntry>
	{
		public LeagueTeam Team { get; set; }
		public double Score { get; set; } // Scores and ScoreList entries can be actual scores, or score ratios, as needed.
		public double Points { get; set; }
		public List<double> ScoreList { get; set; }
		public int Dropped { get; set; }  // Number of scores dropped. Maintained by caller.

		public TeamLadderEntry()
		{
			ScoreList = new List<double>();
			Score = 0;
			Points = 0;
			Dropped = 0;
		}

		public int CompareTo(TeamLadderEntry entry)
		{
			double result = entry.Points - Points;

			if (result == 0)
				result = entry.Score - Score;

			return Math.Sign(result);
		}

		public override string ToString()
		{
			return "TeamLadderEntry " + (Team == null ? "null Team" : "Team " + Team.ToString()) + ": " + Score.ToString() + ", " + Points.ToString();
		}
	}

	public class CompareTeamsScaled : IComparer<TeamLadderEntry>
	{
		public int Compare(TeamLadderEntry entry1, TeamLadderEntry entry2)
		{
			int entry1Games = entry1.ScoreList.Count();
			int entry2Games = entry2.ScoreList.Count();

			double result = entry1Games == 0 && entry2Games == 0 ? 0 :
							entry1Games == 0 ? 1 :
							entry2Games == 0 ? -1 :
							entry2.Points / entry2Games - entry1.Points / entry1Games;

			if (result == 0)
				result = entry2.Score - entry1.Score;

			return Math.Sign(result);
		}
	}

	/// <summary>Ladders, game-by-game, etc. Functions in this class mostly build and return a ZoomReport.</summary>
	public static class Reports
	{
		/// <summary>How many times does each colour come 1st, 2nd, 3rd (etc)?</summary>
		public static ZoomReport ColourReport(List<League> leagues, bool includeSecret, ReportTemplate rt)
		{
			ChartType chartType = ChartTypeExtensions.ToChartType(rt.Setting("ChartType"));

			ZoomReport report = new ZoomReport(ReportTitle("Colour Performance", (leagues.Count == 1 ? leagues[0].Title: ""), rt), "Rank", "center");

			var games = new List<Game>();
			foreach (var league in leagues)
				games.AddRange(Games(league, includeSecret, rt));
			games.Sort();

			var coloursUsed = games.SelectMany(g => g.Teams.Select(t => t.Colour)).Distinct();
			var colourTotals = new Dictionary<Colour, List<int>>();
			foreach (Colour c in coloursUsed)
				colourTotals.Add(c, new List<int>());

			foreach (var league in leagues)
				foreach (Game game in Games(league, includeSecret, rt))
					foreach (GameTeam gameTeam in game.Teams)
					{
						// Add the team's rank in this game to the colourTotals.
						int rank = league.Game(gameTeam).Teams.IndexOf(gameTeam);

						while (colourTotals[gameTeam.Colour].Count <= rank)
							colourTotals[gameTeam.Colour].Add(0);
						if (rank > -1)
							colourTotals[gameTeam.Colour][rank]++;
					}

			coloursUsed = coloursUsed.OrderBy(c => -colourTotals[c].FirstOrDefault());

			foreach (Colour c in coloursUsed)
				report.AddColumn(new ZColumn(c.ToString(), ZAlignment.Integer));

			int maxRank = games.Any() ? games.Max(g => g.Teams.Count) : 0;

			for (int rank = 0; rank < maxRank; rank++)
			{
				ZRow row = report.AddRow(new ZRow());
				row.Add(new ZCell(Utility.Ordinate(rank + 1)));

				foreach (Colour c in coloursUsed)
					if (colourTotals[c].Count > rank)
						row.Add(new ZCell(colourTotals[c][rank].ToString(), c.ToColor()));
					else
						row.Add(new ZCell("0", c.ToColor()));

				var sameWidths = new List<ZColumn>();
				for (int i = 1; i < report.Columns.Count; i++)
					sameWidths.Add(report.Columns[i]);
				report.SameWidths.Add(sameWidths);
			}

			if (rt.Settings.Contains("Description"))
				report.Description = "This report shows the total number of firsts, seconds and thirds that were scored by each colour.";

			FinishReport(report, games, rt);

			return report;
		}

		/// <summary>Every player in every game. Useful for data export.</summary>
		public static ZoomReport EverythingReport(League league, string title, DateTime? from, DateTime? to, bool description)
		{
			var report = new ZoomReport(string.IsNullOrEmpty(title) ? league.Title + " Everything Report" : title + FromTo(league.AllGames, from, to),
											   "Player,Pack,Team,Rank,Score,Tags +,Tags -,Tag Ratio,Score Ratio,TR\u00D7SR,Destroys,Denies,Denied,Yellow,Red",
											   "left,left,left,integer,integer,integer,integer,float,float,float,integer,integer,integer,integer,integer,integer",
											   ",,,,,Tags,Tags,Ratio,Ratio,Ratio,Base,Base,Base,Penalties,Penalties")
			{
				MaxChartByColumn = true,
				MultiColumnOK = true,
				NumberStyle = ZNumberStyle.Plain
			};

			foreach (var game in league.AllGames.FindAll(x => x.Time.CompareTo(from ?? DateTime.MinValue) >= 0 &&
														 x.Time.CompareTo(to ?? DateTime.MaxValue) <= 0))
			{
				var gameTotal = new GamePlayer();

				foreach (var player in game.SortedAllPlayers())
				{

					Console.WriteLine(player.ToString());
					var playerRow = new ZRow();

					Color color = player.Colour.ToColor();
					playerRow.AddCell(new ZCell(league.LeaguePlayer(player) == null ? player.PlayerId : league.LeaguePlayer(player).Name))
						.Hyper = league.LeaguePlayer(player) != null ? PlayerHyper(league.LeagueTeam(player), league.LeaguePlayer(player)) : null;
					playerRow.Add(new ZCell(player.Pack));
					playerRow.Add(new ZCell(league.GameTeamFromPlayer(player) == null ? " " : league.LeagueTeam(league.GameTeamFromPlayer(player)).Name));
					playerRow.Add(new ZCell(player.Rank));

					FillDetails(playerRow, player, color, (double)game.TotalScore() / game.Players().Count);

					gameTotal.Add(player);

					report.Rows.Add(playerRow);
				}

				var gameRow = new ZRow
				{
					new ZCell("", Color.LightGray),
					new ZCell("", Color.LightGray),
					new ZCell(game.LongTitle(), Color.LightGray),
					new ZCell()
				};

				FillDetails(gameRow, gameTotal, Color.Empty, game.TotalScore());

				report.Rows.Add(gameRow);
			}

			return report;
		}

		/// <summary>Fixtures. Each row is a team. Each column is a game.</summary>
		public static ZoomReport FixtureGrid(Fixture fixture, League league)
		{
			ZoomReport report = new ZoomReport("Fixtures for " + league.Title, "Team", "left")
			{
				CssClass = "fixturegrid"
			};

			// Create a row for each team, with team name at left. 
			foreach (var ft in fixture.Teams)
			{
				var row = new ZRow();
				var teamCell = new ZCell(ft.Name)
				{
					Hyper = "fixture.html?team=" + ft.Id().ToString(CultureInfo.InvariantCulture)
				};
				row.Add(teamCell);
				report.Rows.Add(row);
			}

			bool multiDay = fixture.Games.Count > 1 && fixture.Games.First().Time.Date < fixture.Games.Last().Time.Date;

			foreach (var fg in fixture.Games)
			{
				// Create a column for each game.
				var column = report.AddColumn(new ZColumn(fg.Time.ToShortTimeString(), ZAlignment.Center));
				if (multiDay)
					column.GroupHeading = fg.Time.ToLongDateString();

				// Add cells for this game, to this column.
				for (int i = 0; i < fixture.Teams.Count; i++)
				{
					var ft = fixture.Teams[i];
					ZCell cell;

					if (fg.Teams.ContainsKey(ft))
						cell = new ZCell(fg.Teams[ft].ToString()[0].ToString(), fg.Teams[fixture.Teams[i]].ToColor());
					else
						cell = new ZCell();

					cell.CssClass = "t" + ft.Id().ToString(CultureInfo.InvariantCulture) +
						String.Concat(fg.Teams.Keys.Select(k => k.Id() == ft.Id() ? "" : " t" + k.Id().ToString(CultureInfo.InvariantCulture)));

					report.Rows[i].Add(cell);
				}
			}

			// Add one final column for the team name again.
			report.AddColumn(new ZColumn("Team", ZAlignment.Left));
			for (int i = 0; i < fixture.Teams.Count; i++)
			{
				var ft = fixture.Teams[i];
				var teamCell = new ZCell(ft.Name)
				{
					Hyper = "fixture.html?team=" + ft.Id().ToString(CultureInfo.InvariantCulture)
				};
				report.Rows[i].Add(teamCell);
			}

			return report;
		}

		/// <summary>Fixtures. Each row is a game. CSS stuff is consumed by Javascript added in WebOutput.cs' ReportPages.FixturePage.</summary>
		public static ZoomReport FixtureList(Fixture fixture, League league)
		{
			ZoomReport report = new ZoomReport("Fixtures for " + league.Title, "Time", "left")
			{
				CssClass = "fixturelist",
				MultiColumnOK = true
			};

			var match = new Dictionary<FixtureGame, Game>();
			foreach (Game game in league.Games(false))
			{
				var fg = fixture.BestMatch(game, out double score);
				if (score > 0.5 && !match.ContainsKey(fg))
					match.Add(fg, game);
			}

			int maxTeams = fixture.Games.Count == 0 ? 0 : fixture.Games.Max(x => x.Teams.Count());

			for (int i = 0; i < maxTeams; i++)
				report.AddColumn(new ZColumn("Team", ZAlignment.Left));

			foreach (var fg in fixture.Games)
			{
				ZRow row = new ZRow();

				ZCell timeCell = new ZCell(fg.Time.ToString("yyyy/MM/dd HH:mm"))
				{
					CssClass = "time"
				};
				if (match.ContainsKey(fg))
					timeCell.Hyper = GameHyper(match[fg]);
				row.Add(timeCell);

				foreach (var kv in fg.Teams.OrderBy(t => t.Value).ThenBy(t => t.Key.Name))
				{
					ZCell teamCell = new ZCell(kv.Key.Name, kv.Value.ToColor())
					{
						Hyper = "fixture.html?team=" + kv.Key.Id().ToString(CultureInfo.InvariantCulture)
					};
					row.Add(teamCell);
				}

				row.CssClass = String.Concat(fg.Teams.Keys.Select(k => " t" + k.Id().ToString(CultureInfo.InvariantCulture)));

				report.Rows.Add(row);
			}

			return report;
		}

		public static ZoomReport FixtureCombined(Fixture fixture, League league)
        {
			ZoomReport report = new ZoomReport("Fixtures for " + league.Title, "Time", "left")
			{
				CssClass = "fixturelist",
				MultiColumnOK = true
			};

			//list

			var match = new Dictionary<FixtureGame, Game>();
			foreach (Game game in league.Games(false))
			{
				var fg = fixture.BestMatch(game, out double score);
				if (score > 0.5 && !match.ContainsKey(fg))
					match.Add(fg, game);
			}

			int maxTeams = fixture.Games.Count == 0 ? 0 : fixture.Games.Max(x => x.Teams.Count());

			for (int i = 0; i < maxTeams; i++)
				report.AddColumn(new ZColumn("Team", ZAlignment.Left));

			FixtureTeams sortedTeams = new FixtureTeams();

			foreach (var fg in fixture.Games)
			{
				ZRow row = new ZRow();

				ZCell timeCell = new ZCell(fg.Time.ToString("yyyy/MM/dd HH:mm"))
				{
					CssClass = "time"
				};
				if (match.ContainsKey(fg))
					timeCell.Hyper = GameHyper(match[fg]);
				row.Add(timeCell);

				int index = 0;

				foreach (var kv in fg.Teams.OrderBy(t => t.Value).ThenBy(t => t.Key.Name))
				{
					ZCell teamCell = new ZCell(kv.Key.Name, kv.Value.ToColor())
					{
						Hyper = "fixture.html?team=" + kv.Key.Id().ToString(CultureInfo.InvariantCulture)
					};
					row.Add(teamCell);
					if (index == fg.Teams.Count - 1)
					{
						sortedTeams.Add(kv.Key);
					}

					index++;
				}

				row.CssClass = String.Concat(fg.Teams.Keys.Select(k => " t" + k.Id().ToString(CultureInfo.InvariantCulture)));

				report.Rows.Add(row);
			}

			// grid

			/*foreach (var ft in fixture.Teams)
			{
				var row = new ZRow();
				var teamCell = new ZCell(ft.Name)
				{
					Hyper = "fixture.html?team=" + ft.Id().ToString(CultureInfo.InvariantCulture)
				};
				row.Add(teamCell);
				report.Rows.Add(row);
			}*/

			bool multiDay = fixture.Games.Count > 1 && fixture.Games.First().Time.Date < fixture.Games.Last().Time.Date;

			foreach (var fg in fixture.Games)
			{
				// Create a column for each game.
				var column = report.AddColumn(new ZColumn(fg.Time.ToShortTimeString(), ZAlignment.Center));
				if (multiDay)
					column.GroupHeading = fg.Time.ToLongDateString();

				// Add cells for this game, to this column.
				for (int i = 0; i < sortedTeams.Count; i++)
				{
					var ft = sortedTeams[i];
					ZCell cell;

					if (fg.Teams.ContainsKey(ft))
						cell = new ZCell(fg.Teams[ft].ToString()[0].ToString(), fg.Teams[sortedTeams[i]].ToColor());
					else
						cell = new ZCell();

					cell.CssClass = "t" + ft.Id().ToString(CultureInfo.InvariantCulture) +
						String.Concat(fg.Teams.Keys.Select(k => k.Id() == ft.Id() ? "" : " t" + k.Id().ToString(CultureInfo.InvariantCulture)));

					report.Rows[i].Add(cell);
				}
			}

			// Add one final column for the team name again.
			/*report.AddColumn(new ZColumn("Team", ZAlignment.Left));
			for (int i = 0; i < fixture.Teams.Count; i++)
			{
				var ft = fixture.Teams[i];
				var teamCell = new ZCell(ft.Name)
				{
					Hyper = "fixture.html?team=" + ft.Id().ToString(CultureInfo.InvariantCulture)
				};
				report.Rows[i].Add(teamCell);
			}*/

			return report;

		}

		/// <summary> Build a grid of games. One team per row; one game per column.
		public static ZoomReport GamesGrid(League league, bool includeSecret, ReportTemplate rt)
		{
			return GamesGrid(league, Games(league, includeSecret, rt), rt);
		}

		static ZoomReport GamesGrid(League league, List<Game> games, ReportTemplate rt)
		{
			bool hasHits = rt.FindSetting("showHits") >= 0;
			bool isDecimal = rt.FindSetting("isDecimal") >= 0;
			ZoomReport report = new ZoomReport("", "Rank,Team", "center,left");

			DateTime thisgametime = DateTime.MinValue;
			// Create columns.
			foreach (Game game in games)
			{
				thisgametime = game.Time;
				ZColumn column = new ZColumn(
					game.Time.Date < thisgametime.Date ?
					game.Time.ToShortDateString() + " " + game.Time.ToShortTimeString() :
					game.Time.ToShortTimeString(),
					ZAlignment.Integer)
				{
					GroupHeading = game.Title,
					Hyper = GameHyper(game),
					Tag = game
				};
				report.AddColumn(column);

				if(hasHits)
                {
					column = new ZColumn("Hits")
					{
						GroupHeading = game.Title,
						Tag = game
					};
					report.AddColumn(column);
                }

				if (league.IsPoints(games))
				{
					column = new ZColumn("Pts")
					{
						GroupHeading = game.Title,
						Tag = game
					};
					report.AddColumn(column);
				}
			}

			int averageCol = report.Columns.Count();
			int hitsCol = averageCol + 1;
			int pointsCol = averageCol + 1;
			if (rt.ReportType == ReportType.GameGrid)
			{
				report.AddColumn(new ZColumn("Average"));
				if(hasHits)
                {
					report.AddColumn(new ZColumn("Hits"));
					hitsCol = report.Columns.Count() - 1;

				}
				if (league.IsPoints(games))
				{
					report.AddColumn(new ZColumn("Pts"));
					pointsCol = report.Columns.Count() - 1;
				}

				if (rt.Drops != null && (rt.Drops.DropWorst(100) > 0 || rt.Drops.DropBest(100) > 0))
					report.AddColumn(new ZColumn("Dropped", ZAlignment.Integer));
			}

			foreach (LeagueTeam leagueTeam in league.Teams)
			{
				List<double> scoresList = new List<double>();
				List<double> pointsList = new List<double>();
				List<double> hitsList = new List<double>();

				ZRow row = new ZRow(); ;

				row.Add(new ZCell(0, ChartType.None, "N0")); // Temporary blank rank.

				row.AddCell(TeamCell(leagueTeam)).Tag = leagueTeam;

				// Add a cell for each game for this team. If the team is not in this game, add a blank.
				foreach (Game game in games)
				{
					GameTeam gameTeam = game.Teams.Find(x => league.LeagueTeam(x) == leagueTeam);
					if (gameTeam == null)
					{
						row.Add(new ZCell());
						if (hasHits)
							row.Add(new ZCell());
						if (league.IsPoints(games))
							row.Add(new ZCell());
					}
					else
					{
						row.Add(new ZCell(gameTeam.Score, ChartType.Bar, "N0", gameTeam.Colour.ToColor()));
						if (hasHits)
						{
							row.Add(new ZCell(gameTeam.GetHitsBy(), ChartType.None, "", gameTeam.Colour.ToColor()));
							hitsList.Add(gameTeam.GetHitsBy());
						}
						if (league.IsPoints(games))
							row.Add(new ZCell(gameTeam.Points, ChartType.None, "", gameTeam.Colour.ToColor()));
						scoresList.Add(gameTeam.Score);
						pointsList.Add(gameTeam.Points);
					}
				}  // foreach gameTeam

				if (scoresList.Any())
				{
					if (rt.ReportType == ReportType.GameGrid || rt.ReportType == ReportType.GameGridCondensed)
						AddAverageAndDrops(league, row, rt.Drops, scoresList, pointsList, hitsList, isDecimal);

					report.Rows.Add(row);
				}
			}  // foreach leagueTeam

			SortGridReport(league, report, rt, games, averageCol, pointsCol, false, hitsCol);

			if (rt.Settings.Contains("Description"))
				report.Description = "This is a grid of games. Each row in the table is one team. Each column is one game.";
			FinishReport(report, games, rt);

			return report;
		}

		/// <summary> Build a list of games. One team per row.</summary>
		public static ZoomReport GamesGridCondensed(League league, bool includeSecret, ReportTemplate rt)
		{
			bool isDecimal = rt.FindSetting("isDecimal") >= 0;
			ZoomReport report = new ZoomReport("", "Rank,Team", "center,left");

			List<Game> games = Games(league, includeSecret, rt);
			DateTime newTo = rt.To ?? DateTime.MaxValue;
			Game after = league.Games(includeSecret).Exists(x => x.Time > newTo) ? games.Find(x => x.Time > newTo) : null;
			List<string> titles = games.Select(x => x.Title).Distinct().ToList();
			var titleCount = new Dictionary<string, int>();

			// Find the number of columns we need for each title -- this is the maximum number of games any single team has played with that title. 
			foreach (string gameTitle in titles)
			{
				// select max(count(team_id)) from (select game_id, team_id from games join teams where game.title = title)
				int maxCount = 0;
				foreach (var team in league.Teams)
					maxCount = Math.Max(maxCount, games.FindAll(x => x.Title == gameTitle && x.Teams.Exists(y => league.LeagueTeam(y) == team)).Count);

				if (gameTitle != null)
					titleCount.Add(gameTitle, maxCount);
				else if (titles.Count == 1)
					titleCount.Add("", maxCount);
			}

			// Figure out if this set of games ends in a finals series. It's a finals series if all the teams in the last game are in the second-last game, all the teams in the second-last are in the third-last, etc.
			var finals = titles.Count == 1 ? Finals(games) : new List<Game>();

			// Create columns.
			foreach (var tc in titleCount)
				for (int i = 0; i < tc.Value; i++)
				{
					report.AddColumn(new ZColumn("Time") { GroupHeading = tc.Key });
					report.AddColumn(new ZColumn("Score", ZAlignment.Right) { GroupHeading = tc.Key });
					report.AddColumn(new ZColumn(league.IsPoints() ? "Pts" : "Rank", ZAlignment.Right) { GroupHeading = tc.Key });
				}

			int averageCol = report.Columns.Count();
			int pointsCol = averageCol + 1;
			if (rt.ReportType == ReportType.GameGridCondensed)
			{
				report.AddColumn(new ZColumn("Average"));
				if (league.IsPoints())
				{
					report.AddColumn(new ZColumn("Pts"));
					pointsCol = report.Columns.Count() - 1;
				}

				if (rt.Drops != null && (rt.Drops.DropWorst(100) > 0 || rt.Drops.DropBest(100) > 0))
					report.AddColumn(new ZColumn("Dropped", ZAlignment.Integer));
			}

			foreach (LeagueTeam leagueTeam in league.Teams)
			{
				var scoresList = new List<double>();
				var pointsList = new List<double>();

				ZRow row = new ZRow(); ;

				row.Add(new ZCell(0, ChartType.None, "N0")); // Temporary blank rank.

				row.Add(TeamCell(leagueTeam));

				int col = 2;
				while (col < averageCol)
				{
					string gameTitle = report.Columns[col].GroupHeading;
					// Add a cell for each game for this team.
					foreach (Game game in games.Where(x => x.Title == gameTitle || x.Title == null && gameTitle == ""))
					{
						GameTeam gameTeam = game.Teams.Find(x => league.LeagueTeam(x) == leagueTeam);
						if (gameTeam != null)
						{
							row.Add(new ZCell(game.Time.ToShortTimeString())
							{
								Hyper = GameHyper(game)
							}
							);
							row.Add(new ZCell(gameTeam.Score, ChartType.Bar, "N0", gameTeam.Colour.ToColor()));
							row.Add(new ZCell(league.IsPoints() ? gameTeam.Points : game.Teams.IndexOf(gameTeam) + 1, ChartType.None, "",
											  gameTeam.Colour.ToColor()));
							scoresList.Add(gameTeam.Score);
							pointsList.Add(gameTeam.Points);
							col += 3;
						}
					}  // foreach game in title

					// Fill in blanks to the end of this title.
					int finalsGames = finals.Count(g => g.Teams.Any(t => t.TeamId == leagueTeam.TeamId));
					if (finals.Count > 1 && finalsGames > 0)
					{
						int insertAt = col - finalsGames * 3;
						while (col < report.Columns.Count + (finalsGames - finals.Count) * 3)
						{
							row.Insert(insertAt, new ZCell());
							row.Insert(insertAt, new ZCell());
							row.Insert(insertAt, new ZCell());
							col += 3;
						}
					}

					while (col < report.Columns.Count && report.Columns[col].GroupHeading == gameTitle)
					{
						row.Add(new ZCell());
						row.Add(new ZCell());
						row.Add(new ZCell());
						col += 3;
					}
				}

				if (scoresList.Any())
				{
					if (rt.ReportType == ReportType.GameGridCondensed)
						AddAverageAndDrops(league, row, rt.Drops, scoresList, pointsList, isDecimal);

					report.Rows.Add(row);
				}
			}  // foreach leagueTeam

			SortGridReport(league, report, rt, games, averageCol, pointsCol, !league.IsPoints());
			if (rt.Settings.Contains("Description"))
				report.Description = "This is a grid of games. Each row in the table is one team.";
			FinishReport(report, games, rt);

			return report;
		}

		/// <summary>Build a list of games. One game per row.</summary>
		public static ZoomReport GamesList(League league, bool includeSecret, ReportTemplate rt)
		{
			bool hasHits = rt.FindSetting("showHits") >= 0;

			ZoomReport report = new ZoomReport("", "Game", "right")
			{
				MultiColumnOK = true
			};

			int thisgame = 0;
			List<Game> games = Games(league, includeSecret, rt);
			games.Sort();

			int mostTeams = games.Count == 0 ? 0 : games.Max(g => g.Teams.Count);

			while (thisgame < games.Count)// && (to == null || games[thisgame].Time < to))  // loop through each game, and create a row for it
			{
				Game game = games[thisgame];
				if (thisgame == 0 || (games[thisgame - 1].Time.Date < game.Time.Date))  // We've crossed a date boundary, so
					report.AddRow(new ZRow()).Add(new ZCell(game.Time.ToShortDateString()));  // Create a row to show the new date.

				ZRow row = report.AddRow(new ZRow());

				ZCell dateCell = new ZCell(game.ShortTitle())
				{
					Hyper = GameHyper(game)
				};
				row.Add(dateCell);

				foreach (GameTeam gameTeam in game.Teams)
				{
					ZCell teamCell = new ZCell(league.LeagueTeam(gameTeam) == null ? "Team ?" : league.LeagueTeam(gameTeam).Name,
											   gameTeam.Colour.ToColor())
					{
						Hyper = "team" + (gameTeam.TeamId ?? -1).ToString("D2", CultureInfo.InvariantCulture) + ".html"
					};  // team name
					row.Add(teamCell);
					ZCell scoreCell = new ZCell(gameTeam.Score, ChartType.Bar, "N0", gameTeam.Colour.ToColor());
					
					row.Add(scoreCell);
					teamCell.ChartCell = scoreCell;
					scoreCell.ChartCell = scoreCell;
					if (hasHits)
					{
						ZCell hitsCell = new ZCell(gameTeam.GetHitsBy(), ChartType.Bar, "N0", gameTeam.Colour.ToColor());
						row.Add(hitsCell);
						hitsCell.ChartCell = hitsCell;
					}
					if (league.IsPoints())  // there are victory points for this league
					{
						if (game.IsPoints())
							row.Add(new ZCell(gameTeam.Points, ChartType.None, "", gameTeam.Colour.ToColor()));
						else
							row.Add(new ZCell());  // This whole game has no victory points, so don't show any.
						row.Last().ChartCell = scoreCell;
					}
					else
						row.Add(new ZCell());
				}  // foreach gameTeam

				if (league.VictoryPointsHighScore != 0 && game.Players().Any() && league.LeaguePlayer(game.SortedPlayers().First()) != null)  // there is a highscore entry at the end of each row
				{
					for (int i = game.Teams.Count; i < mostTeams; i++)
					{
						row.Add(new ZCell());
						row.Add(new ZCell());
						row.Add(new ZCell());
					}

					var highPlayer = game.SortedPlayers().First();

					row.Add(new ZCell());
					Color highScoreColor = Color.Empty;
					foreach (GameTeam gameTeam in game.Teams)
						if (gameTeam.TeamId == highPlayer.TeamId)
							highScoreColor = gameTeam.Colour.ToColor();

					row.Add(new ZCell(league.LeaguePlayer(highPlayer).Name, highScoreColor));
					row.Add(new ZCell(highPlayer.Score, ChartType.Bar, "N0", highScoreColor));
					row[row.Count - 2].ChartCell = row.Last();
					row.Last().ChartCell = row.Last();
				}

				thisgame++;
			}  // while date/time <= Too

			if (games.Any() && games.First().Time.Date == games.Last().Time.Date)
				report.Rows.RemoveAt(0);  // The first entry in rows is a date line; since there's only one date for the whole report, we can delete it.
			report.Title = (ReportTitle("Games", league.Title, rt));

			for (int i = 0; i < mostTeams; i++)  // set up the Headings text, to cater for however many columns the report has turned out to be
			{
				report.AddColumn(new ZColumn("Team", ZAlignment.Left));
				report.AddColumn(new ZColumn("Score", ZAlignment.Right));
				if(hasHits)
					report.AddColumn(new ZColumn("Hits", ZAlignment.Right));
				if (league.IsPoints())  // there are victory points for this league
					report.AddColumn(new ZColumn("Pts", ZAlignment.Right));
				else
					report.AddColumn(new ZColumn(""));
			}

			if (league.VictoryPointsHighScore != 0)  // set up Headings text to show the highscoring player
			{
				report.AddColumn(new ZColumn("", ZAlignment.Right));
				report.AddColumn(new ZColumn("Best player", ZAlignment.Right));
				report.AddColumn(new ZColumn("Score", ZAlignment.Right));
			}

			if (rt.Settings.Contains("Description"))
			{
				report.Description = "This is a list of games. Each row in the table is one game. Across each row, you see the teams that were in that game (with the team that placed first listed first, and so on), and the score for each team.";

				if (league.VictoryPointsHighScore != 0)
					report.Description += " At the end of each row, you see the high-scoring player for that game, and their score.";
			}

			FinishReport(report, games, rt);
			return report;
		}

		/// <summary>Build a report showing games. Each game fills several rows: one row for team scores, followed by rows for player scores, in the same column as their team.</summary>
		public static ZoomReport DetailedGamesList(League league, bool includeSecret, ReportTemplate rt)
		{
			bool hasHits = rt.FindSetting("showHits") >= 0;
			ZoomReport report = new ZoomReport("", "Game", "right")
			{
				MultiColumnOK = true
			};

			int thisgame = 0;
			List<Game> games = Games(league, includeSecret, rt);
			games.Sort();

			int mostTeams = games.Count == 0 ? 0 : games.Max(g => g.Teams.Count);

			while (thisgame < games.Count)  // loop through each game, and create a row for it
			{
				Game game = games[thisgame];
				if (thisgame == 0 || (games[thisgame - 1].Time.Date < game.Time.Date))  // We've crossed a date boundary, so
					report.AddRow(new ZRow()).Add(new ZCell(game.Time.ToShortDateString()) { Color = report.Colors.OddColor });  // Create a row to show the new date.

				ZRow teamsRow = report.AddRow(new ZRow() { Color = report.Colors.OddColor } );

				ZCell dateCell = new ZCell(game.ShortTitle())
				{
					Hyper = GameHyper(game)
				};
				teamsRow.Add(dateCell);

				int largestTeam = game.Teams.Max(gt => gt.Players.Count);

				// Add team names and team scores.
				foreach (GameTeam gameTeam in game.Teams)
				{
					var leagueTeam = league.LeagueTeam(gameTeam);

					ZCell teamCell = new ZCell(leagueTeam == null ? "Team ?" : leagueTeam.Name, gameTeam.Colour.ToColor())
					{
						Hyper = TeamHyper(leagueTeam)
					};  // team name
					teamsRow.Add(teamCell);
					ZCell scoreCell = new ZCell(gameTeam.Score, ChartType.Bar, "N0", gameTeam.Colour.ToColor());
					teamsRow.Add(scoreCell);
					teamCell.ChartCell = scoreCell;
					scoreCell.ChartCell = scoreCell;
					if (hasHits)
					{
						ZCell hitsCell = new ZCell(gameTeam.GetHitsBy(), ChartType.Bar, "N0", gameTeam.Colour.ToColor());
						teamsRow.Add(hitsCell);
						hitsCell.ChartCell = hitsCell;
					}
					if (league.IsPoints())  // there are victory points for this league
					{
						if (game.IsPoints())
							teamsRow.Add(new ZCell(gameTeam.Points, ChartType.None, "", gameTeam.Colour.ToColor()));
						else
							teamsRow.Add(new ZCell());  // This whole game has no victory points, so don't show any.
						teamsRow.Last().ChartCell = scoreCell;
					}
					else
						teamsRow.Add(new ZCell());
				}  // foreach gameTeam

				int currentPlayer = 0;
				var maxScore = game.Teams.Max(t => t.Players.Max(p => p.Score));

				// Add player aliases and player scores.
				while (currentPlayer < largestTeam)
				{
					ZRow playersRow = report.AddRow(new ZRow());

					foreach (GameTeam gameTeam in game.Teams)
					{
						if (currentPlayer < gameTeam.Players.Count())
						{
							playersRow.Add(new ZCell());
							GamePlayer player = gameTeam.Players[currentPlayer];
							ZCell aliasCell = new ZCell(league.LeaguePlayer(player) == null ? "Player ?" : league.LeaguePlayer(player).Name, gameTeam.Colour.ToColor());
							playersRow.Add(aliasCell);

							ZCell scoreCell = new ZCell(player.Score, ChartType.Bar, "N0", gameTeam.Colour.ToColor())
							{
								Tag = maxScore
							};
							if (league.VictoryPointsHighScore != 0 && player.Score == maxScore)
								scoreCell.Border = Color.Black;

							playersRow.Add(scoreCell);
							if (hasHits)
							{
								ZCell hitsCell = new ZCell(player.HitsBy, ChartType.Bar, "N0", gameTeam.Colour.ToColor());
								playersRow.Add(hitsCell);
							}
						}
						else
                        {
							playersRow.Add(new ZCell());
							playersRow.Add(new ZCell("", gameTeam.Colour.ToColor()));
							playersRow.Add(new ZCell("", gameTeam.Colour.ToColor()));
							if (hasHits)
								playersRow.Add(new ZCell("", gameTeam.Colour.ToColor()));
						}
					}

					currentPlayer++;
				}

				if (league.VictoryPointsHighScore != 0 && game.Players().Any() && league.LeaguePlayer(game.SortedPlayers().First()) != null)  // there is a highscore entry at the end of each row
				{
					// Add blank cells as necessary to fill to the end of the regular part of the row.
					for (int i = game.Teams.Count; i < mostTeams; i++)
					{
						teamsRow.Add(new ZCell());
						teamsRow.Add(new ZCell());
						teamsRow.Add(new ZCell());
					}

					var highPlayer = game.SortedPlayers().First();

					teamsRow.Add(new ZCell());
					Color highScoreColor = Color.Empty;
					foreach (GameTeam gameTeam in game.Teams)
						if (gameTeam.TeamId == highPlayer.TeamId)
							highScoreColor = gameTeam.Colour.ToColor();

					teamsRow.Add(new ZCell(league.LeaguePlayer(highPlayer).Name, highScoreColor));
					teamsRow.Add(new ZCell(highPlayer.Score, ChartType.Bar, "N0", highScoreColor) { Tag = maxScore });

					teamsRow[teamsRow.Count - 2].ChartCell = teamsRow.Last();
					teamsRow.Last().ChartCell = teamsRow.Last();
				}

				thisgame++;
			}  // while date/time <= Too

			if (games.Any() && games.First().Time.Date == games.Last().Time.Date)
				report.Rows.RemoveAt(0);  // The first entry in rows is a date line; since there's only one date for the whole report, we can delete it.
			report.Title = (ReportTitle("Games", league.Title, rt));

			for (int i = 0; i < mostTeams; i++)  // set up the Headings text, to cater for however many columns the report has turned out to be
			{
				report.AddColumn(new ZColumn("Team", ZAlignment.Left));
				report.AddColumn(new ZColumn("Score", ZAlignment.Right));
				if (hasHits)
					report.AddColumn(new ZColumn("Hits", ZAlignment.Right));
				if (league.IsPoints())  // there are victory points for this league
					report.AddColumn(new ZColumn("Pts", ZAlignment.Right));
				else
					report.AddColumn(new ZColumn(""));
			}

			if (league.VictoryPointsHighScore != 0)  // set up Headings text to show the highscoring player
			{
				report.AddColumn(new ZColumn("", ZAlignment.Right));
				report.AddColumn(new ZColumn("Best player", ZAlignment.Right));
				report.AddColumn(new ZColumn("Score", ZAlignment.Right));
			}

			report.CalculateFill = delegate (ZRow row, int col, double chartMin, double chartMax, ref double? fill)
			{
				if (row[col].Tag is double)
					fill = row[col].Number / (double)row[col].Tag;
			};

			report.Colors.OddColor = default;

			if (rt.Settings.Contains("Description"))
			{
				report.Description = "This is a list of detailed games. Each row in the table is one game. Across each row, you see the teams that were in that game (with the team that placed first listed first, and so on), and the score for each team. Under each team is a list of players on the team and what they scored in the game";

				if (league.VictoryPointsHighScore != 0)
					report.Description += " At the end of each row, you see the high-scoring player for that game, and their score.";
			}

			FinishReport(report, games, rt);
			return report;
		}

		/// <summary>Show hyperlinks to games. One row per day; one game per cell.</summary>
		public static ZoomReport GamesToc(League league, bool includeSecret, ReportTemplate rt)
		{
			ZoomReport report = new ZoomReport("Table of Contents");
			report.Columns.Add(new ZColumn("", ZAlignment.Right));

			List<Game> games = Games(league, includeSecret, rt);
			games.Sort();

			var dates = games.Select(g => g.Time.Date).Distinct().ToList();
			if (dates.Count == 1)
				report.Title += ": Games on " + dates[0].ToShortDateString();

			var gameGroups = new List<List<Game>>(); // Groups of games. Games in a group will all be on the same day and have the same title. No group is empty.

			// Populate gameGroups.
			foreach (var date in dates)
			{
				var dayGames = league.AllGames.Where(g => g.Time.Date == date);
				if (dayGames.Any())
				{
					var lastGame = dayGames.First();
					gameGroups.Add(new List<Game>());

					foreach (var game in dayGames)
					{
						if (lastGame.EndTime().AddHours(0.5) < game.Time || lastGame.Title != game.Title)  // If there's at least a 0.5 hour break between games, or the title has changed,
							gameGroups.Add(new List<Game>());  // it's time for a new group.

						gameGroups.Last().Add(game);
						lastGame = game;
					}
				}
			}

			foreach (var group in gameGroups)
			{
				int rows = (int)Math.Ceiling((decimal)group.Count / 16);  // If there are more than 16 games in this group, we want to spread the group over multiple rows.
				int gamesPerRow = (int)Math.Ceiling((decimal)group.Count / rows);
				int game = 0;
				for (int i = 0; i < rows; i++)
				{
					ZRow row = report.AddRow(new ZRow());
					if (string.IsNullOrEmpty(group.First().Title))  // No titles?
						row.Add(new ZCell(group.First().Time.Date.ToShortDateString()));  // Show the date.
					else
						row.Add(new ZCell(group.First().Title));  // Show the title.

					for (int j = 0; j < gamesPerRow; j++)
					{
						if (game >= group.Count)
							break;

						row.Add(new ZCell((group[game].Time.ToShortTimeString()).Trim())
						{
							Hyper = GameHyper(group[game])
						}
						);

						game++;
					}
				}
			}

			int columns = report.Rows.Count == 0 ? 0 : report.Rows.Max(r => r.Count());
			for (; report.Columns.Count < columns;)
				report.AddColumn(new ZColumn(""));

			if (rt.Settings.Contains("Description"))
				report.Description = "This is a list of games. Each cell in the table is one game. Click the cell for details.";

			FinishReport(report, games, rt);
			return report;
		}

		/// <summary>List each team and their rank, several times over: once for each titled group of games.</summary>
		public static ZoomReport MultiLadder(League league, bool includeSecret, ReportTemplate rt)
		{
			ChartType chartType = ChartTypeExtensions.ToChartType(rt.Setting("ChartType"));
			bool ratio = rt.Setting("OrderBy") == "score ratio";
			bool scaled = rt.FindSetting("OrderBy") > 0 && rt.Setting("OrderBy").StartsWith("scaled");

			ZoomReport report = new ZoomReport(ReportTitle("Team Ladders", league.Title, rt), "Rank", "center")
			{
				MaxChartByColumn = true
			};
			report.Columns[0].Rotate = true;

			var games = Games(league, includeSecret, rt);
			var groups = games.Select(g => g.Title).Distinct().ToList();
			var groupGames = new List<Game>();

			int columnsPerGroup = league.IsPoints() ? 5 : 4;

			foreach (var team in league.Teams)
				report.AddRow(new ZRow()).Add(new ZCell(report.Rows.Count));  // Rank

			List<TeamLadderEntry> previousLadder = null;
			string previousGroupName = "";
			ZColumn teamColumn = null;
			for (int group = 0; group < groups.Count(); group++)  // for each group of games
			{
				string groupName = groups[group]?.ToLower() ?? "";

				if ((groupName.Contains("final") && !groupName.Contains("semi")) || groupName.StartsWith("rep ") || groupName.Contains("repechage") || groupName.Contains("repêchage") || 
						previousGroupName.StartsWith("rep ") || previousGroupName.Contains("repechage") || previousGroupName.Contains("repêchage"))
					groupGames.Clear();  // Disregard previous results -- use only results from this round to rank.

				var thisGroupGames = games.Where(g => g.Title == groups[group]).ToList();
				groupGames.AddRange(thisGroupGames);

				teamColumn = report.AddColumn(new ZColumn("Team", ZAlignment.Left, groups[group]));
				if (league.IsPoints())
					report.AddColumn(new ZColumn("Points", ZAlignment.Right, groups[group]));

				if (groupName.Contains("semifinal") || groupName.Contains("semi final") ||
					groupName.Contains("ascension") || groupName.Contains("format ") || groupName.Contains("track"))
				{
					// Rank this round as an Ascension: teams that survive longer are ranked higher.

					report.AddColumn(new ZColumn("Placings", ZAlignment.Right, groups[group]));
					report.AddColumn(new ZColumn("Games", ZAlignment.Integer, groups[group]));
					report.AddColumn(new ZColumn());  // This column is for arrows.

					var ascension = GamesGrid(league, thisGroupGames, new ReportTemplate(ReportType.Ascension, new string[] { }));

					// Find if there are any teams that topped the previous round but aren't in this round. If so, provide blanks for them (because they're skipping this round).
					int offset = 0;
					if (previousLadder != null)
						for (offset = 0; offset < previousLadder.Count; offset++)
							if (ascension.Rows.Exists(r => r[1].Tag == previousLadder[offset].Team))
								break;
					previousLadder = null;

					for (int team = 0; team < offset; team++)
						AddBlankCells(report.Rows[team], columnsPerGroup);

					// For teams that are in this round, add cells for them.
					for (int team = 0; team < Math.Min(report.Rows.Count, ascension.Rows.Count); team++)
					{
						ZRow ascensionRow = ascension.Rows[team];
						ZRow row = report.Rows[team + offset];
						ZCell teamCell = row.AddCell(ascensionRow[1]);  // Team
						row.Add(new ZCell());  // Blank spacer

						var placings = new List<string>();
						var teamId = league.Teams.Find(t => t.Name == teamCell.Text).TeamId;
						foreach (var game in thisGroupGames)
						{
							int rank = game.Teams.FindIndex(t => t.TeamId == teamId);
							if (rank > -1)
								placings.Add(Utility.Ordinate(rank + 1));
						}
						row.Add(new ZCell(string.Join(", ", placings.ToArray())));  // Placings

						row.Add(new ZCell(ascensionRow.Count(c => !c.Empty()) - 2));  // Games
						row.Add(new ZCell());  // Arrow

						MultiLadderArrow(report, teamCell, group, columnsPerGroup, team + offset);
					}
				}
				else
				{
					// Rank this round based on score and/or victory points accumulated to date.

					report.AddColumn(new ZColumn(ratio ? "Score Ratio" : "Average score", ZAlignment.Float, groups[group]));
					report.AddColumn(new ZColumn("Games", ZAlignment.Integer, groups[group]));
					report.AddColumn(new ZColumn());  // This column is for arrows.

					var ladder = Ladder(league, groupGames, rt);
					int offset = 0;
					if (previousLadder != null)
						for (offset = 0; offset < previousLadder.Count; offset++)
							if (ladder.Exists(x => x.Team == previousLadder[offset].Team))
								break;
					previousLadder = ladder;
					for (int team = 0; team < offset; team++)
						AddBlankCells(report.Rows[team], columnsPerGroup);

					for (int t = 0; t < ladder.Count; t++)
					{
						var entry = ladder[t];
						var team = entry.Team;

						var row = report.Rows[t + offset];

						var gamesPlayedThisGroup = league.Played(thisGroupGames, team).Count;
						if (gamesPlayedThisGroup == 0)
							AddBlankCells(row, columnsPerGroup);
						else
						{
							ZCell teamCell = row.AddCell(TeamCell(team));  // Team

							if (league.IsPoints())
								row.Add(new ZCell(ladder[t].Points, chartType));  // Points

							ZCell scoreCell;
							if (entry.ScoreList.Count == 0)
								scoreCell = new ZCell("-");
							else
							{
								scoreCell = new ZCell(null, chartType)
								{
									Number = entry.ScoreList.Average(),
									NumberFormat = ratio ? "P1" : "N0"
								};

								if (ratio)
									scoreCell.Data.AddRange(entry.ScoreList.Select(x => x / entry.ScoreList.Average()));  // average game score ratio
								else
									scoreCell.Data.AddRange(entry.ScoreList);  // average game score
							}
							row.Add(scoreCell);  // Score
							row.Add(new ZCell(gamesPlayedThisGroup));  // Games
							row.Add(new ZCell());  // Arrow

							MultiLadderArrow(report, teamCell, group, columnsPerGroup, t + offset);
						}
					}
				}
				previousGroupName = groupName;
			}

			report.RemoveZeroColumns();

			int? topN = rt.SettingInt("ShowTopN");
			if (topN != null)
			{
				for (int i = report.Rows.Count - 1; i >= topN; i--)
					report.Rows.RemoveAt(i);

				foreach (var column in report.Columns)
					for (int i = column.Arrows.Count - 1; i > 0; i--)
						if (column.Arrows[i].From.Min(x => x.Row) > topN && column.Arrows[i].To.Min(x => x.Row) > topN)
							column.Arrows.RemoveAt(i);
			}

			if (rt.Settings.Contains("Description"))
			{
				report.Description = "This report shows each team's progress through the tournament. Between each of the " + groups.Count() +
					" rounds, follow a team's rise or fall by following their arrow(s). To see a team's final placing, look for the right-most mention of their name";
				int teamIndex = report.Columns.IndexOf(teamColumn);
				if (report.Rows[0].Valid(teamIndex))
					report.Description += "; e.g. " + report.Rows[0][teamIndex].Text + " placed 1st.";
			}

			return report;
		}

		static void MultiLadderArrow(ZoomReport report, ZCell teamCell, int group, int columnsPerGroup, int rank)
		{
			if (group > 0)
			{
				int previousRank = -1;
				int previousCol = (group - 1) * columnsPerGroup + 1;
				while (previousCol >= 0 && previousRank == -1)
				{
					previousRank = report.Rows.FindIndex(r => r.Valid(previousCol) && r[previousCol].Hyper == teamCell.Hyper);
					previousCol -= columnsPerGroup;
				}
				if (previousRank > -1)
				{
					var arrow = new Arrow();
					arrow.From.Add(new ZArrowEnd(previousRank, 5) { Expand = true });
					arrow.To.Add(new ZArrowEnd(rank, 5));
					arrow.Color = Utility.StringToColor(teamCell.Text);
					report.Columns[group * columnsPerGroup].Arrows.Add(arrow);
				}
			}
		}

		static void AddBlankCells(ZRow row, int i)
		{
			for (int j = 0; j < i; j++)
				row.Add(new ZCell());
		}

		/// <summary>Detailed report of a single game. One row for each player, plus total rows for each team and for the whole game.</summary>
		public static ZoomReport OneGame(League league, Game game)
		{
			ZoomReport report = new ZoomReport(game.LongTitle(),
											   "Rank,Name,Score,Tags +,Tags -,Tag Ratio,Score Ratio,TR\u00D7SR,Destroys,Denies,Got Denied,Yellow Card,Red Card,ID",
											   "center,left,integer,integer,integer,float,float,float,integer,integer,integer,integer,integer,integer,left",
											   ",,,Tags,Tags,Ratio,Ratio,Ratio,Base,Base,Base,Penalties,Penalties,")
			{
				MaxChartByColumn = true
			};
			report.Colors.TitleBackColor = Color.DarkGray;  // DarkGray is lighter than Gray.
			report.Colors.TitleFontColor = Color.Black;
			report.Colors.BackgroundColor = default;
			report.Colors.OddColor = default;

			var sameWidths = new List<ZColumn>();
			report.SameWidths.Add(sameWidths);

			for (int i = 3; i < report.Columns.Count; i++)
				report.Columns[i].Rotate = true;

			bool solo = 1.0 * game.Players().Count / game.Teams.Count < 1.5;  // True if most "teams" have one player.

			var gameTotal = new GamePlayer();

			double maxScore = double.MinValue;
			int maxTags = 1;
			foreach (GameTeam gameTeam in game.Teams)
			{
				var teamTotal = new GamePlayer();

				ZRow playerRow = null;
				foreach (GamePlayer gamePlayer in gameTeam.Players)
				{
					// Add a row for each player on the team.
					playerRow = new ZRow();

					Color color = (gamePlayer.Colour == Colour.None ? gameTeam.Colour : gamePlayer.Colour).ToColor();
					playerRow.Add(new ZCell(game.Rank(gamePlayer.PlayerId), ChartType.None, "N0", color));  // Rank

					if (league.LeaguePlayer(gamePlayer) == null)
						playerRow.Add(new ZCell("Player " + gamePlayer.PlayerId, color));
					else
					{
						var leaguePlayer = league.LeaguePlayer(gamePlayer);
						playerRow.AddCell(new ZCell(leaguePlayer.Name, color)).Hyper = PlayerHyper(league.LeagueTeam(gameTeam), leaguePlayer);
					}

					FillDetails(playerRow, gamePlayer, color, (double)game.TotalScore() / game.Players().Count);
					playerRow.Add(new ZCell(gamePlayer.PlayerId));
					maxScore = Math.Max(maxScore, gamePlayer.Score);
					maxTags = Math.Max(maxTags, Math.Max(gamePlayer.HitsOn, gamePlayer.HitsBy));

					teamTotal.Add(gamePlayer);
					gameTotal.Add(gamePlayer);

					report.Rows.Add(playerRow);
				}

				if (gameTeam.Adjustment != 0)
				{
					ZRow adjustmentRow = new ZRow();
					Color color = gameTeam.Colour.ToColor();

					adjustmentRow.Add(new ZCell());  // Rank.
					adjustmentRow.Add(new ZCell("Adjustment", color));

					var adjustment = new GamePlayer
					{
						Score = gameTeam.Adjustment
					};
					FillDetails(adjustmentRow, adjustment, color, 0);

					report.Rows.Add(adjustmentRow);
				}

				// Add a row for the team.
				if (!solo || gameTeam.Players.Count > 1 || gameTeam.Adjustment != 0 || (gameTeam.Players.Any() && gameTeam.Players[0].Score != gameTeam.Score))
				{
					ZRow teamRow = new ZRow();
					Color teamColor = gameTeam.Colour.ToColor();
					teamColor = Color.FromArgb(teamColor.A, (int)(teamColor.R * 0.75), (int)(teamColor.G * 0.75), (int)(teamColor.B * 0.75));

					teamRow.Add(new ZCell());  // Rank.

					if (league.LeagueTeam(gameTeam) == null)
						teamRow.Add(new ZCell("Team " + (gameTeam.TeamId ?? -1).ToString(CultureInfo.InvariantCulture), teamColor));
					else
					{
						ZCell teamCell = new ZCell(league.LeagueTeam(gameTeam).Name, teamColor)
						{
							Hyper = "team" + (gameTeam.TeamId ?? -1).ToString("D2", CultureInfo.InvariantCulture) + ".html"
						};
						teamRow.Add(teamCell);
					}

					teamTotal.Score = gameTeam.Score;
					FillDetails(teamRow, teamTotal, teamColor, (double)game.TotalScore() / game.Players().Count * gameTeam.Players.Count);

					report.Rows.Add(teamRow);
				}
				else
				{
					string teamName = league.LeagueTeam(gameTeam).Name;
					if (playerRow != null)
					{
						if (playerRow[1].Text != teamName)
							playerRow[1].Text = teamName + "(" + playerRow[1].Text + ")";
						playerRow[1].Hyper = "team" + (gameTeam.TeamId ?? -1).ToString("D2", CultureInfo.InvariantCulture) + ".html";
					}
				}
			}

			// Add an overall game average row.
			ZRow gameRow = new ZRow
			{
				new ZCell(),  // Rank
				new ZCell("Teams Average")  // Name
			};
			gameTotal.DivideBy(game.Teams.Count);
			FillDetails(gameRow, gameTotal, Color.Empty, game.TotalScore() / game.Teams.Count);

			report.Rows.Add(gameRow);

			report.RemoveZeroColumns();

			int maxHits = 1;
			var idCol = report.Columns.FindIndex(c => c.Text == "ID");

			if (game?.ServerGame?.Events?.Any() ?? false)
			{
				foreach (var gameTeam in game.Teams)
				{
					var leagueTeam = league.LeagueTeam(gameTeam);

					foreach (var player1 in gameTeam.Players)
					{
						var leaguePlayer = league.LeaguePlayer(player1);
						string name = leaguePlayer == null ? player1.PlayerId : leaguePlayer.Name;
						Color color = (player1.Colour == Colour.None ? gameTeam.Colour : player1.Colour).ToColor();

						// Add a column for each player on the team.
						var column = new ZColumn(name)
						{
							Alignment = ZAlignment.Integer,
							Rotate = true,
							Color = color
						};
						if (!solo || gameTeam.Players.Count > 1)
							column.GroupHeading = leagueTeam == null ? "Team ??" : leagueTeam.Name;
						report.AddColumn(column);
						sameWidths.Add(column);
					}
				}

				foreach (var row in report.Rows.Where(r => r.Valid(idCol)))
				{
					var player1 = game.Players().Find(p => p.PlayerId == row[idCol].Text);
					if (player1 == null)
						continue;

					// Add who-shot-who cells.
					var gameTeam = player1.GameTeam(league);
					foreach (var gameTeam2 in game.Teams)
						foreach (var player2 in gameTeam2.Players)
						{
							ZCell cell;
							if (player1 == player2)
								cell = new ZCell("\u2572", row[0].Color);  // Diagonal Upper Left to Lower Right.
							else if (player1 is ServerPlayer p1 && player2 is ServerPlayer p2)
							{
								int count = game.ServerGame.Events.Count(x => x.ServerPlayerId == p1.ServerPlayerId && x.OtherPlayer == p2.ServerPlayerId && x.Event_Type < 14);
								cell = BlankZero(count, ChartType.Bar, gameTeam == gameTeam2 ? row[0].Color : Color.Empty);
								if (gameTeam != gameTeam2)
									cell.BarColor = ZReportColors.Darken(row[0].Color);
								maxHits = Math.Max(maxHits, count);
							}
							else
								cell = new ZCell();

							row.Add(cell);
						}

					if (game.ServerGame?.Events != null && game.ServerGame.Events.Any())
					{
						var text = new StringBuilder();
						var html = new StringBuilder();
						var svg = new StringBuilder();
						var startTime = game.ServerGame.Events.FirstOrDefault().Time;
						int minutes = 0;  // How many whole minutes into the game are we?
						Colour currentColour = Colour.None;
						if (player1 is ServerPlayer player1sp)
						{
							foreach (var eevent in game.ServerGame.Events.Where(x => x.ServerPlayerId == player1sp.ServerPlayerId && ((x.Event_Type >= 28 && x.Event_Type <= 34) || (x.Event_Type >= 37 && x.Event_Type <= 1404))))
							{
								int now = (int)Math.Truncate(eevent.Time.Subtract(startTime).TotalMinutes);
								if (now - minutes > 1)
								{
									ColourSymbol(text, html, svg, ref currentColour, Colour.None, new string('\u00B7', now - minutes));  // Add one dot for each whole minute of the game.
									minutes = now;
								}

								Colour otherTeam = (Colour)(eevent.OtherTeam + 1);
								switch (eevent.Event_Type)
								{
									case 28: ColourSymbol(text, html, svg, ref currentColour, Colour.None, "\U0001f7e8"); break;  // warning: yellow square.
									case 29: ColourSymbol(text, html, svg, ref currentColour, Colour.None, "\U0001f7e5"); break;  // terminated: red square.
									case 30: ColourSymbol(text, html, svg, ref currentColour, otherTeam, "\u25cb"); break;  // hit base: open circle
									case 31: ColourSymbol(text, html, svg, ref currentColour, otherTeam, "\u2b24"); break;  // destroyed base: filled circle.
									case 32: ColourSymbol(text, html, svg, ref currentColour, otherTeam, "\U0001f480"); break;  // eliminated: skull
									case 33: ColourSymbol(text, html, svg, ref currentColour, otherTeam, "!"); break;  // hit by base
									case 34: ColourSymbol(text, html, svg, ref currentColour, Colour.None, "!"); break;  // hit by mine
									case 37: case 38: case 39: case 40: case 41: case 42: case 43: case 44: case 45: case 46: ColourSymbol(text, html, svg, ref currentColour, Colour.None, "!"); break;  // player tagged target
									case 1401:
									case 1402:  // score denial points: circle with cross, circle with slash
										ColourSymbol(text, html, svg, ref currentColour, otherTeam, new string('\u29bb', eevent.ShotsDenied / 2));  // If this is a game where you can deny for many shots (e.g. 10 shots to destroy a base or whatever) show a double-deny mark for each two shots denied.
										if (eevent.ShotsDenied % 2 == 1) ColourSymbol(text, html, svg, ref currentColour, otherTeam, "\u2300");  // Show remaining one deny hit if necessary.
										break;
									case 1403: case 1404: ColourSymbol(text, html, svg, ref currentColour, Colour.None, eevent.ShotsDenied == 1 ? "\U0001f61e" : "\U0001f620"); break;  // lose points for being denied: sad face, angry face
									default:
										ColourSymbol(text, html, svg, ref currentColour, Colour.None, "?"); break;
								}
							}
						}
						ColourSymbol(text, html, svg, ref currentColour, Colour.None, new string('\u00B7', (int)Math.Truncate(game.ServerGame.Events.LastOrDefault().Time.Subtract(startTime).TotalMinutes) - minutes) + ".");  // Add one dot for each whole minute of the game.
						row.Add(new ZCell(text.ToString())
						{
							Html = html.ToString(),
							Svg = svg.ToString()
						}
							);
					}
				}

				report.AddColumn(new ZColumn("Base hits etc.", ZAlignment.Left)
				{
					FillWidth = true
				}
				);
			}

			report.RemoveColumn(idCol);

			report.CalculateFill = delegate (ZRow row, int col, double chartMin, double chartMax, ref double? fill) 
			{
				if (report.Columns[col].Text == "Score")
					fill = row[col].Number / maxScore;
				if (report.Columns[col].GroupHeading == "Tags")
					fill = row[col].Number / maxTags;
				if (report.Columns[col].Color != Color.Empty)
					fill = row[col].Number / maxHits; 
			};

			return report;
		}

		/// <summary>Detailed report of a single player. One row for each game they played.</summary>
		public static ZoomReport OnePlayer(League league, LeaguePlayer player, List<LeagueTeam> teams)
		{
			ZoomReport report = new ZoomReport(string.IsNullOrEmpty(player.Name) ? "Player " + player.Id : player.Name,
											   "Time,Rank,Score,Tags +,Tags -,Tag Ratio,Score Ratio,TR\u00D7SR,Destroys,Denies,Denied,Yellow,Red",
											   "left,integer,integer,integer,integer,float,float,float,integer,integer,integer,integer,integer,integer",
											   ",,,Tags,Tags,Ratios,Ratios,Ratios,Base,Base,Base,Penalties,Penalties,");

			report.Title += " \u2014 " + string.Join(", ", teams.Select(t => t.Name));
			if (teams.Count == 1)
				report.TitleHyper = TeamHyper(teams[0]);
			else
				report.Columns.Insert(1, new ZColumn("Team"));

			report.MaxChartByColumn = true;

			var totals = new GamePlayer();
			double totalScore = 0;
			int totalCount = 0;

			foreach (GamePlayer gamePlayer in league.Played(player))
			{
				// Add a row for each game the player played.
				ZRow row = new ZRow();
				Color color = (gamePlayer.Colour == Colour.None ? gamePlayer.Colour : gamePlayer.Colour).ToColor();

				Game game = league.Game(gamePlayer);
				if (game == null)
					row.Add(new ZCell("Game ??", color));
				else
				{
					var gameCell = new ZCell(game.LongTitle(), color)
					{
						Hyper = GameHyper(game)
					};  // Game time
					row.Add(gameCell);
				}

				row.Add(new ZCell(gamePlayer.Rank, ChartType.Bar, "N0", color));

				if (teams.Count > 1)
					row.Add(TeamCell(league.LeagueTeam(gamePlayer)));

				FillDetails(row, gamePlayer, color, game == null ? double.NaN : (double)game.TotalScore() / game.Players().Count);

				totals.Score += gamePlayer.Score;
				totals.HitsBy += gamePlayer.HitsBy;
				totals.HitsOn += gamePlayer.HitsOn;
				totals.BaseHits += gamePlayer.BaseHits;
				totals.BaseDestroys += gamePlayer.BaseDestroys;
				totals.BaseDenies += gamePlayer.BaseDenies;
				totals.BaseDenied += gamePlayer.BaseDenied;
				totals.YellowCards += gamePlayer.YellowCards;
				totals.RedCards += gamePlayer.RedCards;
				totalScore += game == null ? 0 : game.TotalScore();
				totalCount += game == null ? 1 : game.Players().Count;

				report.Rows.Add(row);
			}

			// Add an overall average row.
			ZRow totalRow = new ZRow
			{
				new ZCell("Average of " + league.Played(player).Count().ToString() + " games")
			};
			if (teams.Count > 1)
				totalRow.Add(new ZCell());  // Team name

			var played = league.Played(player);
			if (played.Any())
			{
				totalRow.Add(new ZCell(league.Played(player).Average(gp => gp.Rank), ChartType.Bar, "N1"));
				totals.Score /= played.Count;
			}
			else
			{
				totalRow.Add(new ZCell());
				totals.Score = 0;
			}

			FillDetails(totalRow, totals, default, (double)totalScore / totalCount);

			report.Rows.Add(totalRow);

			report.RemoveZeroColumns();
			return report;
		}

		/// <summary> List a team and its players' performance in each game.  One player per column; one game per row.</summary>
		public static ZoomReport OneTeam(League league, bool includeSecret, LeagueTeam team, DateTime from, DateTime to, bool description)
		{
			ZoomReport report = new ZoomReport(team.Name);

			var evenColor = report.Colors.BackgroundColor;
			var oddColor = report.Colors.OddColor;

			report.Colors.BackgroundColor = default;
			report.Colors.OddColor = default;

			report.AddColumn(new ZColumn("Game", ZAlignment.Left));

			var averages = new Dictionary<LeaguePlayer, double>();
			foreach (LeaguePlayer leaguePlayer in team.Players)
				if (leaguePlayer != null)
				{
					double score = 0;
					int count = 0;

					foreach (GameTeam gameTeam in league.Played(team, includeSecret))
						if (league.Game(gameTeam).Time > from && league.Game(gameTeam).Time < to)
						{
							GamePlayer gamePlayer = gameTeam.Players.Find(x => x.PlayerId == leaguePlayer.Id);
							if (gamePlayer != null)
							{
								score += gamePlayer.Score;
								count++;
							}
						}

					if (averages.Keys.Contains(leaguePlayer))
						throw (new Exception("Player " + leaguePlayer.Name + " already in averages."));
					averages.Add(leaguePlayer, count == 0 ? 0 : score / count);
				}

			List<KeyValuePair<LeaguePlayer, double>> averagesList = averages.ToList();
			averagesList.Sort(delegate (KeyValuePair<LeaguePlayer, double> x, KeyValuePair<LeaguePlayer, double> y)
			{
				return Math.Sign(y.Value - x.Value);
			}
							);
			List<LeaguePlayer> players = new List<LeaguePlayer>();
			foreach (var x in averagesList)
				players.Add(x.Key);

			team.Players.Clear();
			team.Players.AddRange(players);

			// Add a column for each player on this team.
			foreach (LeaguePlayer leaguePlayer in players)
			{
				report.AddColumn(new ZColumn(leaguePlayer.Name, ZAlignment.Integer, "Players")).Hyper = PlayerHyper(team, leaguePlayer);
			}
			report.AddColumn(new ZColumn("Total", ZAlignment.Integer, "Score"));
			if (league.IsPoints())
				report.AddColumn(new ZColumn("Pts", ZAlignment.Float, "Score"));

			report.AddColumns("Score again,Tags +,Tags -,Tag Ratio,Score Ratio,TR\u00D7SR,Destroys,Conceded,Ratio,Denies,Denied,Yellow,Red,Played Against",
							  "integer,integer,integer,float,float,float,integer,integer,integer,integer,integer,integer,left",
							  ",Tags,Tags,Ratios,Ratios,Ratios,Base,Base,Base,Base,Base,Penalties,Penalties,");

			// TODO: add columns for total points for/against, total team tags for/against, total team denials for/against (from game.ServerGame.Events?)

			List<GameTeam> played = league.Played(team, includeSecret);
			DateTime previousGameDate = DateTime.MinValue;

			// Add a row for each of this team's games. Fill in values for team score and player scores.
			foreach (GameTeam gameTeam in played.OrderBy(gt => league.Game(gt).Time))
				if (from < league.Game(gameTeam).Time && league.Game(gameTeam).Time < to)
				{
					var game = league.Game(gameTeam);
					if (game.Time.Date > previousGameDate)  // We've crossed a date boundary, so
						report.AddRow(new ZRow()).Add(new ZCell(game.Time.ToShortDateString()));  // Create a row to show the new date.

					ZRow gameRow = new ZRow()
					{
						Color = report.Rows.Count % 2 == 0 ? evenColor : oddColor
					};
					report.Rows.Add(gameRow);
					var timeCell = new ZCell(game.LongTitle())
					{
						Hyper = GameHyper(game)
					};
					gameRow.Add(timeCell);

					// Add player scores to player columns.
					foreach (LeaguePlayer leaguePlayer in players)
					{
						GamePlayer gamePlayer = gameTeam.Players.Find(x => league.LeaguePlayer(x) != null && league.LeaguePlayer(x).Id == leaguePlayer.Id);
						if (gamePlayer == null)
							gameRow.Add(new ZCell());
						else
							gameRow.Add(new ZCell(gamePlayer.Score, ChartType.Bar, "N0", gamePlayer.Colour.ToColor()));
					}

					gameRow.Add(new ZCell(gameTeam.Score, ChartType.Bar, "N0", gameTeam.Colour.ToColor()));

					if (league.IsPoints())
					{
						if (game.IsPoints())
							gameRow.Add(new ZCell(gameTeam.Points, ChartType.None, "N0", gameTeam.Colour.ToColor()));
						else
							gameRow.Add(new ZCell());
					}

					var teamTotal = new GamePlayer();
					foreach (GamePlayer gamePlayer in gameTeam.Players)
						teamTotal.Add(gamePlayer);
					FillDetails(gameRow, teamTotal, Color.Empty, game.TotalScore() / game.Teams.Count);

					// Base Ratio: bases destroyed / bases conceded
					ZCell basesConceded;
					ZCell baseRatio;
					if (game.ServerGame == null || game.ServerGame.Events == null)
					{
						basesConceded = new ZCell();
						baseRatio = new ZCell();
					}
					else
					{
						var basesTotal = game.Teams.Sum(t => t.Players.Sum(p => p.BaseDestroys));
						var thisDestroyed = game.ServerGame.Events.Count(e => e.Event_Type == 31 && e.OtherTeam == (int)gameTeam.Colour - 1);
						var basesThisTeam = gameTeam.Players.Sum(p => p.BaseDestroys);
						basesConceded = new ZCell(thisDestroyed, ChartType.Bar, "N0");
						if (basesThisTeam == 0 && thisDestroyed == 0)
							baseRatio = new ZCell((string)null);
						else if (thisDestroyed == 0)
							baseRatio = new ZCell(double.PositiveInfinity);
						else
							baseRatio = new ZCell(1.0 * basesThisTeam / thisDestroyed, ChartType.Bar, "P0");
					}
					gameRow.Insert(gameRow.Count - 4, basesConceded);
					gameRow.Insert(gameRow.Count - 4, baseRatio);

					var playedAgainst = game.Teams.Select(gt => league.LeagueTeam(gt)).ToList();
					if (playedAgainst.Count <= 10)
					{
						playedAgainst.Remove(league.LeagueTeam(gameTeam));
						gameRow.Add(new ZCell(string.Join(", ", playedAgainst)));
					}

					previousGameDate = game.Time.Date;
				}  // if from..to; for Played

			ZRow averageRow = new ZRow()
			{
				new ZCell("Average of " + played.Count.ToString() + " games")
			};
			averageRow.Color = report.Rows.Count % 2 == 0 ? evenColor : oddColor;
			FillAverages(report, averageRow);

			DoRatio(report, averageRow, "Tags +", "Tags -", "Tag Ratio");
			double averageTotalScore = played.Any() ? played.Average(gt => league.Game(gt).TotalScore() / league.Game(gt).Teams.Count) : 0;
			if (averageTotalScore != 0)
				report.Cell(averageRow, "Score Ratio").Number = played.Average(gt => gt.Score) / averageTotalScore;
			report.Cell(averageRow, "TR\u00D7SR").Number = report.Cell(averageRow, "Tag Ratio").Number * report.Cell(averageRow, "Score Ratio").Number;
			DoRatio(report, averageRow, "Destroys", "Conceded", "Ratio");

			report.Rows.Add(averageRow);

			report.MaxChartByColumn = true;
			//			TODO: replace the above line with report.OnCalcBar = KombiReportCalcBar; TeamLadderCalcBar;

			if (description)
				if (team.Players.Count == 1 && team.Name.Trim().ToLower() == team.Players[0].Name.Trim().ToLower())
					report.Description = "This report shows the team and player " + team.Name + ".  Each row is one game.";
				else
					report.Description = "This report shows the team " + team.Name + " and its players.  Each row is one game.";

			report.RemoveColumn(report.Columns.IndexOf(report.Columns.Find(c => c.Text == "Score again")));
			report.RemoveZeroColumns();
			return report;
		}

		/// <summary>Show overall stats for each pack, including a t test comparing it to all other packs combined.</summary>
		// Things I'm not doing right in PackReport:
		// I'm taking the ratio of two things (hits by / hit on). This is wrong -- hits by and hits on are each normally distributed, but a ratio is not. Instead I should log() it, because log(hits by / hit on) _is_ normally distributed (I think).
		// (I'm already doing something like this (but not exactly this) for Longitudinal.)
		// I'm scaling each pack score ratio and pack tag ratio by the player's tag ratio. Instead, I should do multivariate analysis where player identity is one of the variables.
		// I'm calculating _n_ different p values, and then dividing the p threshold by _n_ to avoid false positives. But this causes false negatives. See notes below for possible ways around this. 
		// False discovery rate. multivariate repeated measures. logit = p/(1-p). poisson model. 
		// Do some histograms of: #hits by each pack, #hits on each pack, #hitsby minus #hitson, ratios of each pack, logits of each pack, and see what's normally distributed.
		// Applied Linear Statistical Models, Kutner et al: ch 7, ch 11.5 bootstrapping, p 1034 questions of interest, ch27
		// Overlapping 95% confidence intervals. If top pack's confidence interval overlaps bottom pack's interval, there are no outliers.
		// Ellipse containing 95% of values on a scatter plot. (X and y axes are hits by and hits on.) Draw an ellipse for each pack; see if they overlap.
		public static ZoomReport PackReport(List<League> leagues, List<Game> round1Games, string title, DateTime? from, DateTime? to, ChartType chartType, bool description, bool longitudinal)
		{
			// Build a list of player IDs.
			var solos = new List<string>();

			foreach (var league in leagues)
				foreach (var player in league.Players)
					if (player.Id != null && !solos.Contains(player.Id))
						solos.Add(player.Id);

			// Then build a solo ladder, showing tag ratios for each player ID, so we can normalise the data for the pack report.
			var soloLadder = new Dictionary<string, double>();
			foreach (var solo in solos)
			{
				int hitsBy = 0;
				int hitsOn = 0;
				double scoreSum = 0;
				double gameAverageSum = 0;
				foreach (var league in leagues)
				{
					var player = league.Players.Find(p => p.Id == solo);
					var played = league.Played(player);

					if (player != null && played.Any())
					{
						hitsBy += played.Sum(x => x.HitsBy);
						hitsOn += played.Sum(x => x.HitsOn);
						scoreSum += played.Sum(x => x.Score);
						scoreSum += played.Average(x => x.Score);
						gameAverageSum += played.Average(x => league.Game(x) == null ? 0 : league.Game(x).TotalScore() / league.Game(x).Players().Count);
					}
				}
				soloLadder.Add(solo, hitsBy == 0 && hitsOn == 0 ?
							   (gameAverageSum == 0 ? 1000000 : scoreSum / gameAverageSum) :  // When there's no hit data, fall back to score data.
							   hitsOn == 0 ? 1000000 : (double)hitsBy / hitsOn);
			}

			// Assign each league a color, for longitudinal chart points.
			var leagueColors = new Dictionary<League, Color>();
			for (int i = 0; i < leagues.Count; i++)
				leagueColors.Add(leagues[i], boyntonColors[i % 9]);

			// Build list of games to report on.
			var games = new List<Game>();
			var gameColors = new Dictionary<Game, Color>();
			foreach (var league in leagues)
			{
				var games2 = league.AllGames.Where(g => g.Time > (from ?? DateTime.MinValue) && g.Time < (to ?? DateTime.MaxValue));
				games.AddRange(games2);
				foreach (var game in games2)
					gameColors.Add(game, leagueColors[league]);
			}

			games.Sort((x, y) => DateTime.Compare(x.Time, y.Time));

			// Now build the pack report.
			var report = new ZoomReport((string.IsNullOrEmpty(title) ? (leagues.Count == 1 ? leagues[0].Title + " " : "") + "Pack Report" : title),
										"Rank,Pack,Score Ratio,t,p,Count,Tag Ratio,t,p,Count",
										"center,left,integer,float,float,integer,float,float,float,integer");

			if (longitudinal)
				report.Columns.Add(new ZColumn("Longitudinal", ZAlignment.Float));

			report.MaxChartByColumn = true;

			var packs = games.SelectMany(game => game.Players().Select(player => player.Pack)).Distinct().ToList();
			bool missingTags = false;  // True if any pack is missing some tag ratios.

			foreach (string pack in packs)
			{
				ZRow row = report.AddRow(new ZRow());

				row.Add(new ZCell(0, ChartType.None));  // 0: put in a temporary Rank
				row.Add(new ZCell(pack));  // 1: set up pack name

				// Sample 1 is "this pack".
				var scoreRatios = new List<double>();
				var tagRatios = new List<double>();
				var tagDifferences = new List<double>();
				var pointScoreRatios = new List<ChartPoint>();
				var pointTagRatios = new List<ChartPoint>();

				// Sample 2 is "all the other packs".
				int n2 = 0;
				int tagRatio2Count = 0;
				double scoreRatio2Sum = 0;
				double scoreRatio2SquaredSum = 0;
				double tagRatio2Sum = 0;
				double tagRatio2SquaredSum = 0;
				double tagDifference2Sum = 0;
				double tagDifference2SquaredSum = 0;
				int tagDifference2Count = 0;

				foreach (var game in games.Where(g => g.Players().Exists(p => p.Pack == pack)))
				{
					double gameTotalScore = 0;
					double gameTotalScale = 0;
					var players = game.Players();
					foreach (var player in players)
					{
						double scale = player.PlayerId != null && soloLadder.ContainsKey(player.PlayerId) ? soloLadder[player.PlayerId] : 1;
						gameTotalScore += 1.0 * player.Score / scale;
						if (player.HitsOn != 0)
							gameTotalScale += (double)player.HitsBy / player.HitsOn / scale;
					}

					foreach (var player in players)
					{
						double scale = player.PlayerId != null && soloLadder.ContainsKey(player.PlayerId) ? soloLadder[player.PlayerId] : 1;
						double scoreRatio = 1.0 * player.Score / (gameTotalScore - player.Score / scale) * (players.Count - 1) / scale;  // Scale this score ratio by this player's scale value, and by the average scaled scores of everyone else in the game.
						double logScoreRatio = ClampedLog(scoreRatio);  // I'm pretty sure that converting the ratio to a log makes it normally distributed again.
						double tagRatio = (double)player.HitsBy / player.HitsOn / scale;
						double logTagRatio = ClampedLog(tagRatio);

						if (player.Pack == pack)
						{
							scoreRatios.Add(logScoreRatio);
							pointScoreRatios.Add(new ChartPoint(game.Time, scoreRatio / (scoreRatio + 1), gameColors[game]));
							if (player.HitsBy != 0 || player.HitsOn != 0)
							{
								tagRatios.Add(logTagRatio);

								if (player.HitsOn == 0)
									pointTagRatios.Add(new ChartPoint(game.Time, 1.0, gameColors[game]));
								else
									pointTagRatios.Add(new ChartPoint(game.Time, tagRatio / (tagRatio + 1), gameColors[game]));

								tagDifferences.Add(player.HitsBy / scale - player.HitsOn);
							}
						}
						else
						{
							n2++;
							scoreRatio2Sum += logScoreRatio;
							scoreRatio2SquaredSum += logScoreRatio * logScoreRatio;
							if (player.HitsBy != 0 || player.HitsOn != 0)
							{
								tagRatio2Sum += logTagRatio;
								tagRatio2SquaredSum += logTagRatio * logTagRatio;
								tagRatio2Count++;
							}
							if (player.HitsBy != 0 || player.HitsOn != 0)
							{
								double tagDifference = player.HitsBy / scale - player.HitsOn;
								tagDifference2Sum += tagDifference;
								tagDifference2SquaredSum += Math.Pow(tagDifference, 2);
								tagDifference2Count++;
							}
						}
					}
				}

				row.AddCell(new ZCell(scoreRatios.Sum(x => Math.Exp(x)) / scoreRatios.Count, chartType, "P0")).Data.AddRange(scoreRatios);  // 2: Average score ratio. Convert log'ed back to un-log'ed for display value in cell.

				double t = tStatistic(scoreRatios.Count, scoreRatios.Sum(), scoreRatios.Sum(x => Math.Pow(x, 2)), n2, scoreRatio2Sum, scoreRatio2SquaredSum);
				double pValue = 1 - Erf(Math.Abs(t) / Math.Sqrt(2));

				if (scoreRatios.Count * 0.9 > tagRatios.Count) // Less than 90% of the results for this pack have tag ratios, so show score ratio statistics.
				{
					row.Add(new ZCell(t, ChartType.None, "F2"));  // 3: t statistic
					row.Add(new ZCell(pValue, ChartType.None, "G2"));  // 4: p value

					if (Math.Abs(pValue) < 0.05 / packs.Count)
						row.Last().Color = Color.FromArgb(0xFF, 0xC0, 0xC0);
					else if (Math.Abs(pValue) < 0.05)
						row.Last().Color = Color.FromArgb(0xFF, 0xF0, 0xF0);
				}
				else
				{
					row.Add(new ZCell("-"));  // t statistic
					row.Add(new ZCell("-"));  // p value
				}

				row.Add(new ZCell(scoreRatios.Count, ChartType.None, "F0"));  // 5: Total games played.

				if (tagRatios.Count == 0)
				{
					row.Add(new ZCell("-"));  // 6: Average tag ratio.
					row.Add(new ZCell("-"));  // 7: t statistic
					row.Add(new ZCell("-"));  // 8: p value
				}
				else
				{
					row.AddCell(new ZCell(tagRatios.Sum(x => Math.Exp(x)) / tagRatios.Count, chartType, "P0")).Data.AddRange(tagRatios);  // 6: Average tag ratio. Convert log'ed back to un-log'ed for display value in cell.

					t = tStatistic(tagRatios.Count, tagRatios.Sum(), tagRatios.Sum(x => Math.Pow(x, 2)), tagRatio2Count, tagRatio2Sum, tagRatio2SquaredSum);
					pValue = 1 - Erf(Math.Abs(t) / Math.Sqrt(2));

					row.Add(new ZCell(t, ChartType.None, "F2"));  // 7: t statistic
					row.Add(new ZCell(pValue, ChartType.None, "G2"));  // 8: p value

					if (Math.Abs(pValue) < 0.05 / packs.Count)
						row.Last().Color = Color.FromArgb(0xFF, 0xC0, 0xC0);
					else if (Math.Abs(pValue) < 0.05)
						row.Last().Color = Color.FromArgb(0xFF, 0xF0, 0xF0);
				}
				missingTags |= tagRatios.Count < scoreRatios.Count;

				row.Add(new ZCell(tagRatios.Count, ChartType.None, "F0"));  // 9: Total games used for tag ratio statistics.

				if (longitudinal)
				{
					if (tagRatios.Any())
						row.AddCell(new ZCell(null, ChartType.XYScatter, "P0")).Tag = pointTagRatios;  // 10: longitudinal scatter of tag ratios
					else
						row.AddCell(new ZCell(null, ChartType.XYScatter, "P0")).Tag = pointScoreRatios;  // 10: longitudinal scatter of score ratios
				}
			}  // foreach pack

			report.Rows = report.Rows.OrderByDescending(x => x[7].Number).ThenByDescending(x => x[3].Number).ToList();

			var tagRatiosCount = report.Rows.Sum(row => row[9].Number);

			// Assign ranks.
			for (int i = 0; i < report.Rows.Count; i++)
				report.Rows[i][0].Number = i + 1;

			// Do footer row.
			var averages = new ZRow
			{
				new ZCell("", Color.Gray),
				new ZCell("Averages", Color.Gray)
			};
			if (report.Rows.Any())
				for (int i = 2; i < report.Rows[0].Count; i++)
					if (report.Columns[i].Text == "Score Ratio" || report.Columns[i].Text == "Tag Ratio" || report.Columns[i].Text == "Count")
						averages.Add(new ZCell(report.Rows.Average(row => row[i].Number),
											   ChartType.None, report.Rows[0][i].NumberFormat, Color.Gray));
					else
						averages.Add(new ZCell("", Color.Gray));

			report.Rows.Add(averages);

			if (tagRatiosCount == 0)  // No tag ratios for any pack.
			{
				for (int i = 12; i >= 6; i--)
					report.RemoveColumn(i);
				foreach (var row in report.Rows)
					foreach (var cell in row)
						cell.ChartCell = report.Cell(row, "Score Ratio");
			}
			else if (report.Rows.Sum(row => row[5].Number) * 0.9 <= tagRatiosCount) // More than 90% of the results for this pack have tag ratios, so don't show score ratio statistics.
			{
				report.RemoveColumn(5);
				report.RemoveColumn(4);
				report.RemoveColumn(3);
				report.RemoveColumn(2);
				for (int i = 0; i < report.Rows.Count; i++)
				{
					report.Rows[i][0].ChartCell = report.Rows[i][2];
					report.Rows[i][1].ChartCell = report.Rows[i][2];
				}
			}

			if (description)
			{
				report.Description = "This report shows the performance of each pack, each time it is used by a logged-on player. " +
					"Each score the pack gains is scaled by the player's ratio, effectively 'handicapping'. " +
					"You should ignore results from a pack where most of its games are played by a particular player. " +
					"You should ignore results from a pack with less than 20 or so games. <br/>" +
					"'t' is the result of Student's t test -- the further the number is from 0, the more this pack's average deviates from the average of all the rest of the packs. " +
					"'p' is the likelihood of the t test result occurring by chance." +
					"You should pay attention to any pack with a p value smaller than " + (0.05 / packs.Count).ToString("G2", CultureInfo.CurrentCulture) +
					" -- these results are statistically significant, meaning that there is less than a 5% chance that such results occurred by chance. " +
					"Once you know which packs are at the ends of the curve, you should remove any unusually good or bad packs. " +
					"In a team event, you can try to balance the colours, by removing the best pack from this colour, the worst from that colour, etc. <br/>";

				if (missingTags)
					report.Description += " Note that on Nexus or Helios, in .Torn files created by Torn 4, only games committed with \"Calculate scores by Torn\" selected in Preferences will show tag ratios. <br/>";

				if (longitudinal)
					report.Description += " The \"Longitudinal\" column shows the performance of each pack in each game over time -- higher means the pack did better. <br/>";

				if (from != null || to != null)
					report.Description += " The report has been limited to games" + FromTo(games, from, to) + ".";
			}

			return report;
		}

		/// <summary>Try to identify common errors in committed games.</summary>
		public static ZoomReport SanityReport(List<League> leagues, string title, DateTime? from, DateTime? to, bool description)
		{
			var report = new ZoomReport(string.IsNullOrEmpty(title) ? (leagues.Count == 1 ? leagues[0].Title + " " : "") + "Sanity Check Report" : title,
										"Game,Team,Issue",
										"left,left,left");

			foreach (var league in leagues)
			{
				var games = league.AllGames.Where(g => g.Time > (from ?? DateTime.MinValue) && g.Time < (to ?? DateTime.MaxValue));
				double averageTeamPlayers = games.Average(g => g.Teams.Average(t => t.Players.Count));
				double playersLoggedOn = games.Average(g => g.Teams.Average(t => t.Players.Average(p => string.IsNullOrEmpty(p.PlayerId) ? 0 : 1)));
				int gamesWithPoints = games.Count(g => g.Teams.Any(t => t.Points != 0));

				foreach (var game in games)
				{
					var teams = game.Teams.Where(t => t.Players.Count < Math.Truncate(averageTeamPlayers));
					foreach (var team in teams)
						AddSanityCheckRow(report, league, game, team, string.Format("Team has only {0} players", team.Players.Count));

					teams = game.Teams.Where(t => t.Players.Count > Math.Round(averageTeamPlayers));
					foreach (var team in teams)
						AddSanityCheckRow(report, league, game, team, string.Format("Team has {0} players. (A player might have switched packs.)", team.Players.Count));

					if (playersLoggedOn > 0.5)
						foreach (var team in game.Teams)
							foreach (var player in team.Players.Where(p => string.IsNullOrEmpty(p.PlayerId)))
								AddSanityCheckRow(report, league, game, team, string.Format("Pack {0} did not log on. Score: {1}", player.Pack, player.Score));

					string gameTitle = game.Title?.ToLower();
					if (gamesWithPoints > games.Count() / 2 && (string.IsNullOrEmpty(game.Title) ||
						(!gameTitle.Contains("ascension") && !gameTitle.Contains("format") && !gameTitle.Contains("final") && !gameTitle.Contains("track"))) &&
						!game.Teams.Any(t => t.Points != 0))
						AddSanityCheckRow(report, league, game, null, "Game does not have victory points set.");
				}
			}

			if (description)
				report.Description = "This report lists possible problems with committed games.\nTake remedial action (e.g. by fixing the problem and recommitting the game) where appropriate.";

			return report;
		}

		/// <summary>List each player and their number of games, average score, tag ratio, etc.</summary>
		public static ZoomReport SoloLadder(League league, bool includeSecret, ReportTemplate rt)
		{
			bool isDecimal = rt.FindSetting("isDecimal") >= 0;
			ChartType chartType = ChartTypeExtensions.ToChartType(rt.Setting("ChartType"));

			ZoomReport report = new ZoomReport(ReportTitle("Solo Ladder", league.Title, rt),
											   "Rank,Player,Team,Average Score,Average Rank,Tags +,Tags-,Tag Ratio,Score Ratio,TR\u00D7SR,Destroys,Denies,Denied,Yellow,Red,Games,Dropped,Grade,Comments,Longitudinal",
											   "center,left,left,integer,integer,integer,integer,float,float,float,integer,integer,integer,integer,integer,integer,integer,float",
											   ",,,,,Tags,Tags,Ratios,Ratios,Ratios,Base,Base,Base,Penalties,Penalties,,,")
			{
				MaxChartByColumn = true,
				MultiColumnOK = true
			};

			report.Columns.First().Rotate = true;
			report.Columns.First(c => c.Text == "Games").Rotate = true;
			report.Columns.First(c => c.Text == "Dropped").Rotate = true;

			double bestScoreRatio = 0;
			string bestScoreRatioText = "";
			double bestTagRatio = 0;
			string bestTagRatioText = "";
			int atLeastN = rt.SettingInt("AtLeastN") ?? 1;
			bool showGrades = rt.FindSetting("ShowGrades") >= 0;
			bool showComments = rt.FindSetting("ShowComments") >= 0;

			var playerTeams = league.BuildPlayerTeamList();
			foreach (var pt in playerTeams)
			{
				var player = pt.Key;
				var games = Games(league, includeSecret, rt).Where(x => x.Players().Exists(y => y.PlayerId == player.Id));

				if (games.Count() >= atLeastN && player.Name != null)
				{
					ZRow row = report.AddRow(new ZRow());
					row.Add(new ZCell(0, ChartType.None, "N0"));  // Temporary rank
					row.AddCell(new ZCell(player.Name)).Hyper = PlayerHyper(pt.Value[0], player);  // Player alias

					if (pt.Value.Count() == 1)
						row.Add(TeamCell(pt.Value.First()));
					else
						row.Add(new ZCell(string.Join(", ", pt.Value.Select(x => x.Name))));  // Team(s) played for

					var played = League.Played(games, player, includeSecret);

					row.Add(DataCell(played.Select(x => (double)x.Score).ToList(), rt.Drops, chartType, isDecimal ? "N1" : "N0"));  // Av score
					row.Add(DataCell(played.Select(x => (double)x.Rank).ToList(), rt.Drops, chartType, "N2"));  // Av rank
					row.Add(DataCell(played.Select(x => (double)x.HitsBy).ToList(), rt.Drops, chartType, "N0"));  // Tags +
					row.Add(DataCell(played.Select(x => (double)x.HitsOn).ToList(), rt.Drops, chartType, "N0"));  // Tags -
					if (played.Max(x => x.HitsOn) == 0 && played.Max(x => x.HitsBy) == 0)
						row.Add(new ZCell(double.NaN));  // Tag ratio
					else
						row.Add(DataCell(played.Select(x => (double)x.HitsBy / x.HitsOn).ToList(), rt.Drops, chartType, "P0"));  // Tag ratio

					List<double> scoreRatios = new List<double>();
					List<double> srxTrs = new List<double>();
					var pointRatios = new List<ChartPoint>();
					foreach (var play in played)
					{
						var game = league.Game(play);
						if (game != null)
						{
							var playerCount = game.Players().Count;
							var scoreRatio = (double)play.Score / game.TotalScore() * playerCount;
							var tagRatio = 1.0 * play.HitsBy / play.HitsOn;
							scoreRatios.Add(scoreRatio);
							srxTrs.Add(((double)play.Score) / game.TotalScore() * playerCount * tagRatio);
							pointRatios.Add(new ChartPoint(game.Time, scoreRatio / (scoreRatio + 1), Color.FromArgb(0xFF, 0x33, 0x00)));  // scarlet
							pointRatios.Add(new ChartPoint(game.Time, tagRatio / (tagRatio + 1), Color.FromArgb(0x3D, 0x42, 0x8B)));  // royal blue

							if (bestScoreRatio == scoreRatio)
								bestScoreRatioText += ", " + player.Name;
							else if (bestScoreRatio < scoreRatio)
							{
								bestScoreRatio = scoreRatio;
								bestScoreRatioText = player.Name;
							}

							if (bestTagRatio == tagRatio)
								bestTagRatioText += ", " + player.Name;
							else if (bestTagRatio < tagRatio)
							{
								bestTagRatio = tagRatio;
								bestTagRatioText = player.Name;
							}
						}
					}

					row.Add(DataCell(scoreRatios, rt.Drops, chartType, "P0"));  // Score ratio
					row.Add(DataCell(srxTrs, rt.Drops, chartType, "N2"));  // SR x TR

					row.Add(DataCell(played.Select(x => (double)x.BaseDestroys).ToList(), rt.Drops, ChartType.Bar, "n1"));
					row.Add(DataCell(played.Select(x => (double)x.BaseDenies).ToList(), rt.Drops, ChartType.Bar, "n1"));
					row.Add(DataCell(played.Select(x => (double)x.BaseDenied).ToList(), rt.Drops, ChartType.Bar, "n1"));
					row.Add(DataCell(played.Select(x => (double)x.YellowCards).ToList(), rt.Drops, ChartType.Bar, "n2"));
					row.Add(DataCell(played.Select(x => (double)x.RedCards).ToList(), rt.Drops, ChartType.Bar, "n2"));

					row.Add(new ZCell(played.Count(), ChartType.None, "N0"));  // Games

					if (rt.Drops == null)
						row.Add(new ZCell());  // games dropped
					else
					{
						int countAfterDrops = rt.Drops.CountAfterDrops(played.Count);
						if (countAfterDrops < played.Count)
							row.Add(new ZCell(played.Count - countAfterDrops, ChartType.None, "N0"));  // games dropped
						else
							row.Add(new ZCell());  // games dropped
					}

					if(showGrades)
						row.AddCell(new ZCell(player.Grade));  // Player grade

					if (showComments)
						row.AddCell(new ZCell(player.Comment));  // Player comment

					if (rt.Settings.Contains("Longitudinal"))
						row.AddCell(new ZCell(null, ChartType.XYScatter, "P0")).Tag = pointRatios;  // Longitudinal scatter of score ratios and tag ratios.

					row[0].ChartCell = row[3];
					row[1].ChartCell = row[3];
					row[2].ChartCell = row[3];
					row[3].ChartCell = row[3];
				}
			}

			report.Rows.Sort(delegate (ZRow x, ZRow y)
			{
				double? result = y[3].Number - x[3].Number;
				return Math.Sign((double)result);
			}
							);

			for (int i = 0; i < report.Rows.Count; i++)
				report.Rows[i][0].Number = i + 1;

			if (bestTagRatio > 0)
				report.Description += string.Format("Best tag ratio was {0:P0} by {1}. ", bestTagRatio, bestTagRatioText);
			if (bestScoreRatio > 0)
				report.Description += string.Format("Best score ratio was {0:P0} by {1}. ", bestScoreRatio, bestScoreRatioText);

			int? topN = rt.SettingInt("ShowTopN");
			if (topN != null)
				for (int i = report.Rows.Count - 1; i >= topN; i--)
					report.Rows.RemoveAt(i);

			if (rt.Settings.Contains("Longitudinal"))
			{
				report.Rows[0].Last().Text = "1";
				report.RemoveZeroColumns();
				report.Rows[0].Last().Text = null;
				report.Description += " The Longitudinal column shows each game for each player. Red is score ratio; blue is tag ratio.";
			}
			else
				report.RemoveZeroColumns();

			return report;
		}

		/// <summary>List each team and their number of games, average score, victory points, etc.</summary>
		public static ZoomReport TeamLadder(League league, bool includeSecret, ReportTemplate rt)
		{
			bool isDecimal = rt.FindSetting("isDecimal") >= 0;
			ChartType chartType = ChartTypeExtensions.ToChartType(rt.Setting("ChartType"));
			bool ratio = rt.Setting("OrderBy") == "score ratio";
			bool scaled = rt.FindSetting("OrderBy") > 0 && rt.Setting("OrderBy").StartsWith("scaled");
			bool showColours = rt.Settings.Contains("ShowColours");

			ZoomReport report = new ZoomReport(ReportTitle("Team Ladder", league.Title, rt), "Rank,Team", "center,left")
			{
				MaxChartByColumn = true,
				MultiColumnOK = true
			};

			List<Game> games = Games(league, includeSecret, rt);
			var ladder = Ladder(league, games, rt);

			int atLeastN = rt.SettingInt("AtLeastN") ?? 1;

			var coloursUsed = games.SelectMany(g => g.Teams.Select(t => t.Colour)).Distinct().OrderBy(c => c);
			var colourTotals = new Dictionary<Colour, List<int>>();

			if (showColours)
			{
				var colourColumns = new List<ZColumn>();
				foreach (Colour c in coloursUsed)
				{
					colourColumns.Add(report.AddColumn(new ZColumn("1st", ZAlignment.Integer, c.ToString())));
					colourColumns.Add(report.AddColumn(new ZColumn("2nd", ZAlignment.Integer, c.ToString())));
					colourColumns.Add(report.AddColumn(new ZColumn("3rd", ZAlignment.Integer, c.ToString())));

					colourTotals.Add(c, new List<int>());
				}
				report.SameWidths.Add(colourColumns);
			}

			if (league.IsPoints())
				report.AddColumn(new ZColumn("Points", ZAlignment.Float));

			double scoreMin = games.Min(g => g.Teams.Min(t => t.Score));
			double scoreRange = games.Max(g => g.Teams.Max(t => t.Score)) - scoreMin;
			double victoryPointsMin = games.Min(g => g.Teams.Min(t => t.Points));
			double victoryPointsRange = games.Max(g => g.Teams.Max(t => t.Points)) - victoryPointsMin;

			report.AddColumn(new ZColumn(ratio ? "Score Ratio" : "Average score", ZAlignment.Float));
			report.AddColumn(new ZColumn("Games", ZAlignment.Integer));

			if (rt.Drops != null && rt.Drops.HasDrops())
				report.AddColumn(new ZColumn("Dropped", ZAlignment.Integer));

			if (rt.Settings.Contains("Longitudinal"))
				report.AddColumn(new ZColumn("Longitudinal"));  // Longitudinal scatter of score ratios and tag ratios.

			ZCell barCell = null;

			List<int> countList = new List<int>();  // Number of games each team has played, for scaling.
			int maxPlace = 1;

			foreach (TeamLadderEntry entry in ladder)  // Create a row for each League team.
			{
				var team = entry.Team;

				ZRow row = new ZRow
				{
					new ZCell(0),  // put in a temporary Rank

					TeamCell(team)
				};

				var colourCounts = new Dictionary<Colour, List<int>>();

				if (showColours)
				{
					foreach (Colour c in coloursUsed)
						colourCounts.Add(c, new List<int>());

					foreach (GameTeam gameTeam in league.Played(games, team))  // Roll through this team's games.
					{
						int rank = league.Game(gameTeam).Teams.IndexOf(gameTeam);

						// Add the team's rank in this game to the colourCounts,
						while (colourCounts[gameTeam.Colour].Count <= rank)
							colourCounts[gameTeam.Colour].Add(0);
						colourCounts[gameTeam.Colour][rank]++;

						// and to colourTotals.
						while (colourTotals[gameTeam.Colour].Count <= rank)
							colourTotals[gameTeam.Colour].Add(0);
						colourTotals[gameTeam.Colour][rank]++;
					}

					foreach (Colour c in coloursUsed)
						for (int rank = 0; rank < 3; rank++)
							if (colourCounts[c].Count > rank && colourCounts[c][rank] > 0)
							{
								row.Add(new ZCell(colourCounts[c][rank], ChartType.Bar, "N0", c.ToColor()));
								maxPlace = Math.Max(maxPlace, colourCounts[c][rank]);
							}
							else
								row.Add(new ZCell("0", c.ToColor()));
				}

				if (league.IsPoints())
				{
					barCell = new ZCell(entry.Points);  // league points scored
					row.Add(barCell);  // league points scored
				}

				ZCell scoreCell;
				if (entry.ScoreList.Count == 0)
					scoreCell = new ZCell("-");     // average game scores
				else
				{
					//scoreCell = DataCell(scoreList, rt.Drops, chartType, "N0");
					scoreCell = new ZCell(0, chartType)
					{
						Number = entry.ScoreList.Average(),
						NumberFormat = ratio ? "P1" : isDecimal ? "N1" : "N0"
					};

					if (ratio)
						scoreCell.Data.AddRange(entry.ScoreList.Select(x => x / entry.ScoreList.Average()));  // average game score ratio
					else
						scoreCell.Data.AddRange(entry.ScoreList);  // average game score
				}
				row.Add(scoreCell);     // average game scores
				if (!league.IsPoints())
					barCell = scoreCell;

				countList.Add(entry.ScoreList.Count + entry.Dropped);

				row.Add(new ZCell(entry.ScoreList.Count + entry.Dropped, ChartType.None, "N0"));  // games played

				if (entry.Dropped > 0)
					row.Add(new ZCell(entry.Dropped, ChartType.None, "N0"));  // games dropped

				if (showColours)
				{
					for (int i = 2; i < coloursUsed.Count() * 3 + 1; i++)
						row[i].ChartCell = row[i];
					for (int i = coloursUsed.Count() * 3 + 2; i < row.Count; i++)
						row[i].ChartCell = barCell;
				}
				else
					foreach (ZCell cell in  row)
						cell.ChartCell = barCell;

				if (rt.Settings.Contains("Longitudinal"))
				{
					var pointRatios = new List<ChartPoint>();
					foreach (GameTeam gameTeam in league.Played(games, team))  // Roll through this team's games.
					{
						var game = league.Game(gameTeam);

						var scoreRatio = gameTeam.Score / game.Teams.Average(t => t.Score);
						pointRatios.Add(new ChartPoint(game.Time, (gameTeam.Score - scoreMin) / scoreRange * 0.9 + 0.05, Color.FromArgb(0xFF, 0x33, 0x00)));  // scarlet

						if (league.IsPoints())
						{
							if (game.Teams.Any(t => t.Points != 0) && victoryPointsRange > 0)
								pointRatios.Add(new ChartPoint(game.Time, (gameTeam.Points - victoryPointsMin) / victoryPointsRange * 0.9 + 0.05, Color.FromArgb(0x3D, 0x42, 0x8B)));  // royal blue
							else
								pointRatios.Add(new ChartPoint(game.Time, 1 - (game.Teams.IndexOf(gameTeam) / (game.Teams.Count - 1) * 0.9 + 0.05), Color.FromArgb(0x3D, 0x42, 0x8B)));  // rank, royal blue
						}
					}
					row.AddCell(new ZCell(null, ChartType.XYScatter, "P0")).Tag = pointRatios;  // Longitudinal scatter of score ratios and tag ratios.
				}

				if (entry.ScoreList.Count + entry.Dropped >= atLeastN)
					report.Rows.Add(row);
			}  // foreach team in league.Teams

			bool more = false;  // Are there teams with more than the mode number of games?
			bool less = false;  // Are there teams with less than the mode number of games?
			int mode = 0;

			if (rt.Settings.Contains("ScaleGames") && countList.Any() && countList.Min() != countList.Max())  // Find the mode, and add scaled columns.
			{
				// Calculate mode, the number of games _most_ teams have played.
				var groups = countList.GroupBy(x => x);
				int maxCount = groups.Max(g => g.Count());
				mode = groups.Last(g => g.Count() == maxCount).Key;

				if (league.IsPoints())
					report.AddColumn(new ZColumn("Scaled points", ZAlignment.Right));

				int gamesColumn = report.Columns.FindIndex(x => x.Text == "Games");

				foreach (ZRow row in report.Rows)
				{
					int count = gamesColumn == -1 ? 1 : (int)row[gamesColumn].Number;

					if (league.IsPoints())
					{
						if (mode != count && count > 0) 
						{
							row.Add(new ZCell((report.Cell(row, "Points").Number * mode / count), ChartType.None, "F1"));  // scaled points
							if (mode < count)
								more = true;
							else
								less = true;
						}
						else
							row.Add(new ZCell());  // scaled points
					}
				}

//				report.OnCalcBar = TeamLadderScaledCalcBar;
			}  // if ScaleGames

			for (int i = 0; i < report.Rows.Count; i++)
				report.Rows[i][0].Number = i + 1;

			int maxTotal = 1;
			if (showColours)  // Add a Totals row at the bottom of the report.
			{
				ZRow row = report.AddRow(new ZRow());
				row.Add(new ZCell("", Color.Gray));  // Blank rank.
				row.Add(new ZCell("Totals", Color.Gray));  // "Team" name.

				foreach (Colour c in coloursUsed)
					for (int rank = 0; rank < 3; rank++)
					{
						Color dark = Color.FromArgb((c.ToColor().R + Color.Gray.R) / 2, (c.ToColor().G + Color.Gray.G) / 2, (c.ToColor().B + Color.Gray.B) / 2);
						if (colourTotals[c].Count > rank)
						{
							row.Add(new ZCell(colourTotals[c][rank], ChartType.Bar, null, dark));
							maxTotal = Math.Max(maxTotal, colourTotals[c][rank]);
						}
		 				else
		 					row.Add(new ZCell("0", dark));
		 			}

				if (league.IsPoints())
					row.Add(new ZCell(ladder.Sum(e => e.Points) / ladder.Count(), ChartType.None, "f1", Color.Gray));  // average league points scored

				if (ratio)
					row.Add(new ZCell(ladder.Sum(e => e.Score) * 100.0 / ladder.Count(), ChartType.None, "P1", Color.Gray));  // average game score ratio
				else
					row.Add(new ZCell(ladder.Sum(e => e.Score) / ladder.Count(), ChartType.None, isDecimal ? "N1" : "N0", Color.Gray));  // average game score

				row.Add(new ZCell(countList.Average(), ChartType.None, "f1", Color.Gray));  // average games played

				if (rt.Drops != null && rt.Drops.HasDrops())
					row.Add(new ZCell());  // games dropped
			}

			int? topN = rt.SettingInt("ShowTopN");
			if (topN != null)
				for (int i = report.Rows.Count - 1; i >= topN; i--)
					report.Rows.RemoveAt(i);

			for (int i = 0; i < report.Rows.Count; i++)
				report.Rows[i][0].Number = i + 1;

			if (rt.Settings.Contains("Description"))
			{
				report.Description = "This report ranks teams.";

				FinishReport(report, games, rt);

				report.CalculateFill = delegate (ZRow row, int col, double chartMin, double chartMax, ref double? fill)
				{
					if (string.IsNullOrEmpty(report.Columns[col].GroupHeading))
						return;
					if (row == report.Rows.Last())
						fill = row[col].Number / maxTotal;
					else
						fill = row[col].Number / maxPlace;
				};

				if (more && less)
					report.Description += " Teams with more or less than " + mode.ToString(CultureInfo.CurrentCulture) + " games have been scaled down or up. This is shown in the Scaled column.";
				else if (more)
					report.Description += " Teams with more than " + mode.ToString(CultureInfo.CurrentCulture) + " games have been scaled down. This is shown in the Scaled column.";
				else if (less)
					report.Description += " Teams with less than " + mode.ToString(CultureInfo.CurrentCulture) + " games have been scaled up. This is shown in the Scaled column.";
	
				if (showColours)
					report.Description += " For each team, the report shows the number of times they placed first, second and third on each colour. The Totals row shows the total number of firsts, seconds and thirds that were scored by each colour.";

				if (rt.Settings.Contains("Longitudinal"))
					report.Description += " The Longitudinal column shows each game for each team. Red is score ratio; blue is victory points or rank.";

				if (report.Description == "This report ranks teams. ")  // No interesting boxes are checked,
			  			report.Description = "";  // so this description is too boring to show.
			}
			else
				FinishReport(report, games, rt);

			return report;
		}

		/// <summary>Build a square table showing how many times each team has played (and beaten) each other team.</summary>
		public static ZoomReport TeamsVsTeams(League league, bool includeSecret, ReportTemplate rt)
		{
			ZoomReport report = new ZoomReport(ReportTitle("Teams vs Teams", league.Title, rt), "Team", "left");
			
			List<LeagueTeam> teams = new List<LeagueTeam>();
			teams.AddRange(league.Teams);
			teams.Sort(delegate(LeagueTeam x, LeagueTeam y)
			           {
			           	double result = league.AveragePoints(y, includeSecret) - league.AveragePoints(x, includeSecret);
			           	return Math.Sign(result == 0 ? league.AverageScore(y, includeSecret) - league.AverageScore(x, includeSecret) : result);
			           });

			List<Game> games = Games(league, includeSecret, rt);

			foreach (var team1 in teams)
			{
				var column = new ZColumn(team1.Name, ZAlignment.Center)
				{
					Hyper = TeamHyper(team1),
					Rotate = true
				};
				report.AddColumn(column);

				ZRow row = report.AddRow(new ZRow());

				row.Add(TeamCell(team1));

				foreach (var team2 in teams)
				{
					var cellGames = games.FindAll(x => x.Teams.Any(y => league.LeagueTeam(y) == team1) &&
					                                   x.Teams.Any(z => league.LeagueTeam(z) == team2));  // Get games that include these two teams.
					ZCell cell;
					if (team1 == team2)
					{
						cell = new ZCell("\u2572", Color.Gray)
						{
							Number = 1,  // Ensure that ZoomReport.Widths will set sensible widths for all columns.
							NumberFormat = "F1"  // Ensure that ZoomReport.Widths will set sensible widths for all columns.
						};
					}
					else if (cellGames.Count == 0)
						cell = new ZCell((string)null);
					else
					{
						var wins = cellGames.Count(x => x.Teams.FindIndex(y => league.LeagueTeam(y) == team1) < x.Teams.FindIndex(z => league.LeagueTeam(z) == team2));
						cell = new ZCell(wins.ToString(CultureInfo.CurrentCulture) +
											 "/" + cellGames.Count().ToString(CultureInfo.CurrentCulture))
						{
							Number = (double)wins / cellGames.Count(),
							ChartType = ChartType.Bar
						};

						if (cellGames.Count == 1)
							cell.Hyper = GameHyper(cellGames[0]);
					}
					row.Add(cell);
				}
			}

			if (rt.Settings.Contains("Description"))
				report.Description = "This report shows how may times each team has played and beaten each other team.";

			FinishReport(report, games, rt);
			return report;
		}

		/// <summary>Show overall hits on each pack. Use to identify possible pack faults.</summary>
		public static ZoomReport TechReport(List<League> leagues, string title, DateTime? from, DateTime? to, ChartType chartType, bool description)
		{
			var games = new List<Game>();

			foreach (var league in leagues)
				games.AddRange(league.AllGames.Where(g => g.Time > (from ?? DateTime.MinValue) && g.Time < (to ?? DateTime.MaxValue) && g.ServerGame != null));

			var packsUsed = games.Select(g => g.ServerGame).SelectMany(sg => sg.Events.Select(e => e.ServerPlayerId)).Distinct().OrderBy(id => String.Format("{0,3}", id));

			var report = new ZoomReport((string.IsNullOrEmpty(title) ? (leagues.Count == 1 ? leagues[0].Title + " " : "") + "Tech Report" : title) + FromTo(games, from, to),
										"Pack,Name, Games,Front,Back,Left,Right,Laser,Total",
										"center,left,integer,integer,integer,integer,integer,integer,integer,integer",
										",,,Hits On Per Game,Hits On Per Game,Hits On Per Game,Hits On Per Game,Hits On Per Game,Hits On Per Game")
			{
				MaxChartByColumn = true
			};

			var evenColor = report.Colors.BackgroundColor;
			var oddColor = report.Colors.OddColor;
			var barColor = report.colors.BarNone;
			report.Colors.BackgroundColor = default;
			report.Colors.OddColor = default;

			// EventType (see P&C ng_event_types):
			//  14..20: tagged by foe (in various hit locations: laser, front, left shoulder, right shoulder, left back shoulder (never used), right back shoulder (never used), back);
			//  21..27: tagged by ally.

			foreach (var pack in packsUsed)
			{
				var gamesThisPack = games.Where(g => g.Players().Any(p => p is ServerPlayer sp && sp.ServerPlayerId == pack) && g.ServerGame.Events.Any(e => e.ServerPlayerId == pack));
				int gameCount = gamesThisPack.Count();

				var row = new ZRow
				{
					new ZCell(pack),
					new ZCell(gamesThisPack.First().Players().Find(p => p is ServerPlayer sp && sp.ServerPlayerId == pack).Pack),
					new ZCell(gameCount, ChartType.None),
					DataCell(gamesThisPack.Select(g => (double)g.ServerGame.Events.Count(e => e.ServerPlayerId == pack && e.Event_Type == 15 || e.Event_Type == 22)).ToList(), null, chartType, "N1"),
					DataCell(gamesThisPack.Select(g => (double)g.ServerGame.Events.Count(e => e.ServerPlayerId == pack && e.Event_Type == 20 || e.Event_Type == 27)).ToList(), null, chartType, "N1"),
					DataCell(gamesThisPack.Select(g => (double)g.ServerGame.Events.Count(e => e.ServerPlayerId == pack && e.Event_Type == 16 || e.Event_Type == 23)).ToList(), null, chartType, "N1"),
					DataCell(gamesThisPack.Select(g => (double)g.ServerGame.Events.Count(e => e.ServerPlayerId == pack && e.Event_Type == 17 || e.Event_Type == 24)).ToList(), null, chartType, "N1"),
					DataCell(gamesThisPack.Select(g => (double)g.ServerGame.Events.Count(e => e.ServerPlayerId == pack && e.Event_Type == 14 || e.Event_Type == 21)).ToList(), null, chartType, "N1"),
					DataCell(gamesThisPack.Select(g => (double)g.ServerGame.Events.Count(e => e.ServerPlayerId == pack && e.Event_Type >= 14 && e.Event_Type <= 27)).ToList(), null, chartType, "N1")
				};

				report.Rows.Add(row);

				foreach (var cell in row)
				{
					cell.Color = report.Rows.Count % 2 == 0 ? evenColor : oddColor;
					cell.BarColor = barColor;
				}
			}

			// Do it all again for a summary row.
			int gameTotal = games.Select(g => g.ServerGame).Count(g => g.Events.Any());
			if (gameTotal > 0)
			{
				var row = new ZRow
				{
					new ZCell(),
					new ZCell("Total"),
					new ZCell(gameTotal, ChartType.None),
					new ZCell(0, ChartType.Histogram, "N1"),
					new ZCell(0, ChartType.Histogram, "N1"),
					new ZCell(0, ChartType.Histogram, "N1"),
					new ZCell(0, ChartType.Histogram, "N1"),
					new ZCell(0, ChartType.Histogram, "N1"),
					new ZCell(0, ChartType.Histogram, "N1")
				};

				for (int i = 3; i < report.Columns.Count; i++)
				{
					foreach (var packRow in report.Rows)
						row[i].Data.Add((double)packRow[i].Number);
					row[i].Number = row[i].Data.Average();
				}

				report.Rows.Add(row);

				foreach (var cell in row)
				{
					cell.Color = Color.DarkGray;
					cell.BarColor = barColor;
				}
			}

			if (description)
				report.Description = @"This report shows the proportion of hits on each pack in each location. If a pack is hit less in a location, there may be a fault. Ignore packs with few games.
Where a large percentage of a pack's games are played by a particular unusual player (a good player, a bad player, a left-hander, etc), this may affect that pack's results.
Histograms show the count of times each pack got shot in each location in each game. Histograms in the Total row shows the count of packs that had each average.
Tiny numbers at the bottom of the bottom row show the minimum, bin size, and maximum of each histogram.";

			return report;
		}

		static void FillDetails(ZRow row, GamePlayer gamePlayer, Color color, double averageScore)
		{
			row.Add(new ZCell(gamePlayer.Score, ChartType.Bar, "N0", color));
			row.Add(BlankZero(gamePlayer.HitsBy, ChartType.Bar, color));
			row.Add(BlankZero(gamePlayer.HitsOn, ChartType.Bar, color));

			if (gamePlayer.HitsOn == 0 && gamePlayer.HitsBy == 0)  // Tag ratio
				row.Add(new ZCell("", color));
			else if (gamePlayer.HitsOn == 0)
				row.Add(new ZCell("\u221e", color));
			else
				row.Add(new ZCell((double)gamePlayer.HitsBy / gamePlayer.HitsOn, ChartType.Bar, "P0", color));

			if (averageScore == 0)  // Score ratio
				row.Add(new ZCell("", color));
			else
				row.Add(new ZCell(gamePlayer.Score / averageScore, ChartType.Bar, "P0", color));

			if (gamePlayer.HitsOn == 0 && gamePlayer.HitsBy == 0)  // TR x SR
				row.Add(new ZCell("", color));
			else if (gamePlayer.HitsOn == 0)
				row.Add(new ZCell("infinite", color));
			else
			    row.Add(new ZCell((double)gamePlayer.HitsBy / gamePlayer.HitsOn * gamePlayer.Score / averageScore, ChartType.Bar, "N2", color));

			row.Add(BlankZero(gamePlayer.BaseDestroys, ChartType.Bar, color));
			row.Add(BlankZero(gamePlayer.BaseDenies, ChartType.Bar, color));
			row.Add(BlankZero(gamePlayer.BaseDenied, ChartType.Bar, color));
			row.Add(BlankZero(gamePlayer.YellowCards, ChartType.Bar, color));
			row.Add(BlankZero(gamePlayer.RedCards, ChartType.Bar, color));
		}

		static void FillAverages(ZoomReport report, ZRow averageRow)
		{
			for (int col = averageRow.Count; col < report.Columns.Count; col++)
			{
				double total = 0.0;
				int count = 0;
				string format = "N0";
				foreach (var row in report.Rows)
				{
					if (col < row.Count && row[col].Number != null)
					{
						total += (double)row[col].Number;
						count++;
						if (!string.IsNullOrEmpty(row[col].NumberFormat))
							format = row[col].NumberFormat;
					}
				}
				if (count == 0)
					averageRow.Add(new ZCell());
				else if (format == "N0" && total / count < 100)
					averageRow.Add(new ZCell(total / count, ChartType.Bar, "N1"));
				else
					averageRow.Add(new ZCell(total / count, ChartType.Bar, format));
			}
		}

		/// <summary>Append the specified symbol to the StringBuilders. If the current colour has changed, emit <div> and <tspan> or </div> and </tspan> as appropriate.</summary>
		static void ColourSymbol(StringBuilder text, StringBuilder html, StringBuilder svg, ref Colour currentColour, Colour newColour, string symbol)
		{
			string c = ColorTranslator.ToHtml(newColour.ToDarkColor());

			if (currentColour == Colour.None && newColour != Colour.None)
			{
				html.Append("<div style=\"color:");
				html.Append(c);
				html.Append("\">");

				svg.Append("<tspan fill=\"");
				svg.Append(c);
				svg.Append("\">");
			}
			else if (currentColour != Colour.None && newColour == Colour.None)
			{
				html.Append("</div>");
				svg.Append("</tspan>");
			}
			else if (currentColour != newColour)
			{
				html.Append("</div><div style=\"color:");
				html.Append(c);
				html.Append("\">");

				svg.Append("</tspan><tspan fill=\"");
				svg.Append(c);
				svg.Append("\">");
			} // else currentColour == newColour: do nothing.

			text.Append(symbol);
			html.Append(symbol);
			svg.Append(symbol);
			currentColour = newColour;
		}

		// value, scaleMin and scaleMax are all in the before-scaling ordinate system. outputRange gives the range of the after-scaling ordinate system.
		static float Scale(double value, float outputRange, double scaleMin, double scaleMax)
		{
			return (float)((value - scaleMin) / (scaleMax - scaleMin) * outputRange);
		}

		static float Scale(DateTime value, double outputRange, DateTime scaleMin, DateTime scaleMax)
		{
			return (float)(value.Subtract(scaleMin).TotalMilliseconds / scaleMax.Subtract(scaleMin).TotalMilliseconds * outputRange);
		}

		// If i > 10000, show i / 1000 + "K". If i > 1E6, show i / 1E6 + "M".
		static string ToK(double i)
		{
			if (Math.Abs(i) < 9999)
				return i.ToString("F0");
			if (Math.Abs(i) < 999999)
				return (i / 1000.0).ToString("F0") + "K";
			if (Math.Abs(i) < 9999999)
				return (i / 1000000.0).ToString("F1") + "M";
			return (i / 1000000.0).ToString("F0") + "M";
		}

		/// <summary>Create a bitmap showing the score of each team over time.</summary>
		public static Bitmap GameWorm(League league, Game game, bool includeSecret)
		{
			var scoreRange = league.Games(includeSecret).Max(g => g.Teams.Max(t => t.Score)) - league.Games(includeSecret).Min(g => Math.Min(g.Teams.Min(t => t.Score), 0));
			var minScore = Math.Min(game.Teams.Min(x => x.Score), 0);  // Handle games where all teams scores are positive, some are negative, 
			var maxScore = Math.Max(game.Teams.Max(x => x.Score), 0);  // or all are negative (e.g. Lord of the Ring).
			double duration = game.ServerGame.EndTime.Subtract(game.Time).TotalSeconds;
			var height = Math.Max((int)Math.Ceiling(duration * (maxScore - minScore) / scoreRange), 1);
			var skew = Scale(game.ServerGame.Events.Sum(e => e.Event_Type < 28 ? e.Score : 0) / game.Teams.Count, height, minScore, maxScore) / duration;  // In points per second, or points per pixel.

			if (duration < 2 || height < 2 || double.IsInfinity(skew))
				return null;

			var bitmap = new Bitmap((int)duration, height + (int)(skew * duration));  // Widthwise, 1 pixel = 1 second.
			var graphics = Graphics.FromImage(bitmap);

			var font = new Font("Arial", 12);
			graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

			// Draw "horizontal" gridlines and labels. 
			var gridPen = new Pen(Color.Gray);
			var dashLength =  (float)Math.Sqrt(skew * skew + 1) * 4;  // Dashlength is 4 seconds.
			gridPen.DashPattern = new float[] { dashLength, dashLength * 2 };
			gridPen.DashOffset = dashLength * 2;

			var interval = Math.Pow(10, Math.Floor(Math.Log10(maxScore - minScore)));  // Interval will be a round number of points like 100, 1000, 10000, e.
			for (var i = (Math.Truncate(minScore / interval) - 1) * interval; i <= maxScore; i += interval)
			{
				graphics.DrawLine(gridPen, 0, height - Scale(i, height, minScore, maxScore), bitmap.Width, height - Scale(i, height, minScore, maxScore) + (float)(skew * duration)); // "Horizontal" gridlines showing score values.
				string caption = ToK(i);
				graphics.DrawString(caption, font, Brushes.Gray, 0, height - Scale(i, height, minScore, maxScore) - graphics.MeasureString(caption, font).Height);
				graphics.DrawString(caption, font, Brushes.Gray, bitmap.Width - graphics.MeasureString(caption, font).Width, height - Scale(i, height, minScore, maxScore) + (float)(skew * duration));
			}

			// Draw vertical gridlines, once per minute.
			dashLength = Scale(interval, height, minScore, maxScore) / 30;
			gridPen.DashPattern = new float[] { dashLength, dashLength * 2 };
			gridPen.DashOffset = 0;
			for (var i = game.Time; i < game.ServerGame.EndTime; i = i.AddMinutes(1))
			{
				var x = Scale(i, duration, game.Time, game.ServerGame.EndTime);
				graphics.DrawLine(gridPen, x, 0, x, bitmap.Height); // Vertical gridlines showing time values.
			}
			graphics.DrawLine(gridPen, bitmap.Width - 1, 0, bitmap.Width - 1, bitmap.Height); // Last vertical gridline.

			// Write vertical gridline labels.
			var minutes = (int)Math.Round(duration / 60);
			int spacing = minutes < 6 ? 1 : minutes % 2 == 0 ? 2 : minutes % 3 == 0 ? 3 : minutes % 5 == 0 ? 5 : 1;
			for (var i = spacing; i < minutes; i += spacing)
				graphics.DrawString(i.ToString(), font, Brushes.Gray, Scale(i, bitmap.Width, 0, minutes), height - Scale(0, height, minScore, maxScore) - graphics.MeasureString("0", font).Height);  // Time labels.

			var playerIds = game.ServerGame.Players.Select(p => p.ServerPlayerId).Distinct();
			var playerTeams = new Dictionary<string, GameTeam>();  // Dictionary to let us quickly find a player's team.
			foreach (var id in playerIds)
				if (id != null)
					playerTeams.Add(id, game.Teams.Find(t => t.Players.Exists(gp => gp.PlayerId == game.ServerGame.Players.Find(sp => sp.ServerPlayerId == id).PlayerId)));

			bool teamsByColour = game.Teams.All(t => t.Players.All(p => p.Colour == t.Colour)) && game.Teams.Select(t => t.Colour).Distinct().Count() == game.Teams.Count;  // true if each team has players only of a single colour, and all teams are different colours.

			var currents = new Dictionary<GameTeam, KeyValuePair<DateTime, int>>();  // Dictionary of gameTeam -> <time, team score>.
			
			foreach (var gameTeam in game.Teams)
				currents.Add(gameTeam, new KeyValuePair<DateTime, int>(game.Time, 0));

			float yMin = height;
			float yMax = 0;

			foreach (var oneEvent in game.ServerGame.Events)
				if (oneEvent.Score != 0)
				{
					var serverPlayer = game.ServerGame.Players.Find(sp => sp.ServerPlayerId == oneEvent.ServerPlayerId);

					GameTeam gameTeam = null;
					if (serverPlayer != null)
						gameTeam = game.Teams.Find(t => t.Players.Exists(gp => gp.PlayerId == serverPlayer.PlayerId));

					if (gameTeam == null && teamsByColour)
						gameTeam = game.Teams.Find(t => t.Colour == (Colour)(oneEvent.ServerTeamId + 1));

					if (gameTeam == null)
						continue;

					var pen = new Pen(gameTeam.Colour.ToSaturatedColor(), 3);
					var oldPoint = new PointF(Scale(currents[gameTeam].Key, duration, game.Time, game.ServerGame.EndTime), height - Scale(currents[gameTeam].Value, height, minScore, maxScore) + (float)(skew * currents[gameTeam].Key.Subtract(game.Time).TotalSeconds));
					currents[gameTeam] = new KeyValuePair<DateTime, int>(oneEvent.Time, currents[gameTeam].Value + oneEvent.Score);
					float y = height - Scale(currents[gameTeam].Value, height, minScore, maxScore) + (float)(skew * currents[gameTeam].Key.Subtract(game.Time).TotalSeconds);
					yMin = Math.Min(y, yMin);
					yMax = Math.Max(y, yMax);
					var newPoint = new PointF(Scale(currents[gameTeam].Key, duration, game.Time, game.ServerGame.EndTime), y);

					if (oneEvent.Event_Type == 30) // Base hit: show in the base's colour not the player's colour.
						pen.Color = ((Colour)(oneEvent.OtherTeam + 1)).ToSaturatedColor();

					graphics.DrawLine(pen, oldPoint, newPoint);  // Show the hit.

					if (oneEvent.Event_Type == 31) // Base destroyed.
					{
						// Show the hit in a dashed line, half player colour, half base colour.
						var dashPen = new Pen(((Colour)(oneEvent.OtherTeam + 1)).ToSaturatedColor(), 3);
						float baseHeight = Math.Abs(oldPoint.Y - newPoint.Y);
						dashPen.DashPattern = new float[] { baseHeight * 0.11F, baseHeight * 0.22F };
						dashPen.DashOffset = baseHeight * -0.11F;
						graphics.DrawLine(dashPen, oldPoint, newPoint);

						// Show the alias of the player who destroyed the base.
						var brush = new SolidBrush(gameTeam.Colour.ToDarkColor());
	
						if (currents.Max(x => x.Value.Value) == currents[gameTeam].Value)
							graphics.DrawString(serverPlayer.Alias, font, brush, newPoint.X - graphics.MeasureString(serverPlayer.Alias, font).Width, newPoint.Y);
						else
							graphics.DrawString(serverPlayer.Alias, font, brush, newPoint.X, (oldPoint.Y + newPoint.Y) / 2);
					}
				}

			// Crop unused space off top and/or bottom of bitmap.
			yMin = Math.Max(yMin - 1, 0);
			bitmap =  bitmap.Clone(new RectangleF(0, yMin, bitmap.Width, Math.Max(Math.Min(yMax + 1, bitmap.Height) - yMin, 1)), bitmap.PixelFormat);
			graphics = Graphics.FromImage(bitmap);
			graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

			// Write team names and scores in upper left corner.
			if (game.Teams.Count <= 10)
				for (int i = 0; i < game.Teams.Count; i++)
				{
					var leagueTeam = league.LeagueTeam(game.Teams[i]);
					if (leagueTeam != null)
					{
						var brush = new SolidBrush(game.Teams[i].Colour.ToDarkColor());
						graphics.DrawString(leagueTeam.Name + " " + game.Teams[i].Score.ToString("N0", CultureInfo.CurrentCulture), 
						                    font, brush, 30, graphics.MeasureString("0", font).Height * i);
					}
				}

			return bitmap;
		}

		static void DoRatio(ZoomReport report, ZRow row, string numerator, string denominator, string result)
		{
			double? numeratorValue = report.Cell(row, numerator)?.Number;
			double? denominatorValue = report.Cell(row, denominator)?.Number;
			if (numeratorValue != null && denominatorValue != null && !double.IsInfinity((double)denominatorValue))
				report.Cell(row, result).Number = numeratorValue / denominatorValue;
		}

		// https://stackoverflow.com/questions/470690/how-to-automatically-generate-n-distinct-colors
		static readonly List<Color> boyntonColors = new List<Color>
		{
			Color.FromArgb(32, 32, 255),    // Blue
			Color.FromArgb(255, 0, 0),      // Red
			Color.FromArgb(0, 224, 0),      // Green
			Color.FromArgb(128, 160, 0),    // Yellow
			Color.FromArgb(255, 0, 255),    // Magenta
			Color.FromArgb(128, 128, 128),  // Gray
			Color.FromArgb(128, 0, 0),      // Brown
			Color.FromArgb(255, 128, 128),  // Pink
			Color.FromArgb(255, 128, 0),    // Orange
		};

		static void AddSanityCheckRow(ZoomReport report, League league, Game game, GameTeam team, string message)
		{
			report.Rows.Add(
				new ZRow()
				{
					new ZCell(game.LongTitle()) { Hyper = GameHyper(game) },
					team == null ? new ZCell(string.Empty) : new ZCell(league.LeagueTeam(team).Name, team.Colour.ToColor()),
					new ZCell(message)
				}
			);
		}

		static List<Game> Games(League league, bool includeSecret, ReportTemplate rt)
		{
			string group = rt.Setting("Group");
			return league.Games(includeSecret)
				.Where(g => g.Time > (rt.From ?? DateTime.MinValue) && g.Time < (rt.To ?? DateTime.MaxValue) && (string.IsNullOrEmpty(group) || (g.Title ?? "").Contains(group)))
				.ToList();
		}

		/// <summary>If i is 0, return a cell which has "" as its text (but still 0 as its number). Otherwise return a cell with this number.</summary>
		static ZCell BlankZero(int i, ChartType chartType, Color color)
		{
			return i == 0 ?
				new ZCell("", color) { Number = 0 }:
				new ZCell(i, chartType, null, color);
		}

		static ZCell DataCell(List<double> dataList, Drops drops, ChartType chartType, string numberFormat)
		{
			var dataCell = new ZCell(0, chartType, numberFormat);
			dataCell.Data.AddRange(dataList);
			if (drops != null)
				DropScores(dataList, drops);
			dataCell.Number = dataList.Where(x => !double.IsNaN(x)).DefaultIfEmpty(0).Average();

			return dataCell;
		}

		static ZCell TeamCell(LeagueTeam leagueTeam, Color color = default)
		{
			if (leagueTeam == null)
				return new ZCell("", color);

			ZCell teamcell = new ZCell(leagueTeam.Name, color)
			{
				Hyper = TeamHyper(leagueTeam)
			};
			return teamcell;
		}

		/// <summary>Callback passed to various reports to generate HTML fragment with URL of a game.</summary>
		static string GameHyper(Game game)
		{
			return "games" + game.Time.ToString("yyyyMMdd", CultureInfo.InvariantCulture) + ".html#game" + game.Time.ToString("HHmm", CultureInfo.InvariantCulture);
		}

		static string TeamHyper(LeagueTeam leagueTeam)
		{
			if (leagueTeam == null)
				return "team_not_found.html";
			else
				return "team" + leagueTeam.TeamId.ToString("D2", CultureInfo.InvariantCulture) + ".html";
		}

		static string PlayerHyper(LeagueTeam leagueTeam, LeaguePlayer leaguePlayer)
		{
			return TeamHyper(leagueTeam) + "#player" + leaguePlayer.Id;
		}

		/// <summary>Return the natural log of x. If x is out of bounds, return a "clamped" value instead.</summary>
		static double ClampedLog(double x)
		{
			if (x <= 0)
				return -3;
			else if (double.IsInfinity(x))
				return 3;
			else
				return Math.Log(x);
		}

		static double Erf(double x)
		{
			// Abramowitz, M. and Stegun, I. (1964). _Handbook of Mathematical Functions with Formulas, Graphs, and Mathematical Tables_. p.299.
			// http://people.math.sfu.ca/~cbm/aands/page_299.htm
			// erf(x) = 1 - 1 / (a1*x + a2*x^2 + a3*x^3 + a4*x^4 + a5*x^5 + a6*x^6) + epsilon(x)
			// a1 = 0.0705230784; a2 = 0.0422820123; a3 = 0.0092705272; a4 = 0.0001520143; a5 = 0.0002765672; a6 = 0.0000430638
			// epsilon(x) < 3e-7
			const double a1 = 0.0705230784;
			const double a2 = 0.0422820123;
			const double a3 = 0.0092705272;
			const double a4 = 0.0001520143;
			const double a5 = 0.0002765672;
			const double a6 = 0.0000430638;
			return 1 - Math.Pow(1 + a1 * x + a2 * Math.Pow(x, 2) + a3 * Math.Pow(x, 3) + a4 * Math.Pow(x, 4) + a5 * Math.Pow(x, 5) + a6 * Math.Pow(x, 6), -16);
		}

		/// <summary>Used for t-test. https://en.wikipedia.org/wiki/T-statistic </summary>
		static double tStatistic(double n1, double sum1, double squaredSum1, double n2, double sum2, double squaredSum2)
		{
			double v1 = (squaredSum1 - (sum1 * sum1 / n1)) / (n1 - 1);  // Variance.
			double v2 = (squaredSum2 - (sum2 * sum2 / n2)) / (n2 - 1);
			double sp = Math.Sqrt(((n1 - 1) * v1 + (n2 - 1) * v2) / (n1 + n2 - 2));  //  sp is the pooled standard deviation.
			return (sum1 / n1 - sum2 / n2) / (sp * Math.Sqrt(1.0/n1 + 1.0/n2));  // Student's t test statistic.
		}

		public static List<TeamLadderEntry> Ladder(League league, List<Game> games, ReportTemplate rt)
		{
			bool ratio = rt.Setting("OrderBy") == "score ratio";
			var ladder = new List<TeamLadderEntry>();

			foreach (LeagueTeam team in league.Teams)  // Create a ladder entry for each League team.
			{
				var playedByThisTeam = league.Played(games, team);
				if (playedByThisTeam.Any())
				{
					var entry = new TeamLadderEntry()
					{
						Team = team
					};

					foreach (GameTeam gameTeam in playedByThisTeam)  // Roll through this team's games.
					{
						Game game = league.Game(gameTeam);

						if (!ratio)
							entry.ScoreList.Add(gameTeam.Score);
						else if (game.TotalScore() != 0)
							entry.ScoreList.Add(gameTeam.Score / game.TotalScore() * game.Teams.Count);

						entry.Points += gameTeam.Points;
					}

					if (entry.ScoreList.Count > 0)
					{
						entry.Dropped = DropScores(entry.ScoreList, rt.Drops);
						entry.Score = entry.ScoreList.Average();
					}

					ladder.Add(entry);
				}
			}

			if (rt.Settings.Contains("ScaleGames"))
				ladder.Sort(new CompareTeamsScaled());
			else
				ladder.Sort();

			return ladder;
		}

		/// <summary>Figure out if this set of games ends in a finals series. It's a finals series if all the teams in the last game are in the second-last game, all the teams in the second-last are in the third-last, etc.</summary>
		static List<Game> Finals(List<Game> games)
		{
			var finals = new List<Game>
			{
				games.Last()
			};
			int current = games.Count - 2;

			bool matched = true;
			while (current >= 0 && matched)
			{
				foreach (var game in finals)
					foreach (var team in game.Teams)  // Check that every team in "game" is in "current". ("current" can be a superset of "game" if e.g. a team has dropped out due to injury.)
						matched &= games[current].Teams.Any(t => t.TeamId == team.TeamId);

				if (matched)  // If all teams in this game are in all subsequent finals games,
					finals.Add(games[current]); // add this game to finals.

				current--;
			}

			finals.Sort();

			return finals;
		}

		/// <summary>Returns text description like " from 1-1-2020 to 2/2/2020".</summary>
		static string FromTo(List<Game> games, DateTime? from, DateTime? to)
		{
			if (games.Count == 0)
				return " from " + (from == null ? "(none)" : ((DateTime)from).ToShortDateString()) +
				       " to " + (to == null ? "(none)" : ((DateTime)to).ToShortDateString());

			DateTime first = from == null ? DateTime.MinValue : (DateTime)from;
			first = games.First().Time > first ? games.First().Time : first;
			DateTime last = to == null ? DateTime.MaxValue : (DateTime)to;
			last = games.Last().Time < last ? games.Last().Time : last;

			if (first.Date == last.Date)  // This report is for games on a single day.
				return " for " + first.Date.ToShortDateString();

			return " from " + first.ToShortDateString() + " to " + last.ToShortDateString();  // This report is for games over multiple days.
		}

		static int DropScores<T>(List<T> scores, Drops drops)
		{
			int count = scores.Count();
			if (drops != null)
			{
				scores.Sort();

				int dropBest = drops.DropBest(count);
				int dropWorst = drops.DropWorst(count);

				if (drops.CountAfterDrops(count) < 1 && count > 0)
				{
					T closest = scores[(int)Math.Round(1.0 * dropWorst / (dropBest + dropWorst) * (count - 1))];
					scores.Clear();
					scores.Add(closest);
					return count - scores.Count;
				}

				if (dropBest > 0)
					scores.RemoveRange(count - dropBest, dropBest);

				if (dropWorst > 0)
					scores.RemoveRange(0, dropWorst);
			}
			return count - scores.Count;
		}

		static void AddAverageAndDrops(League league, ZRow row, Drops drops, List<double> scoresList, List<double> pointsList, List<double> hitsList, bool isDecimal)
		{
			int count = scoresList.Count;
			
			DropScores(scoresList, drops);
			DropScores(pointsList, drops);
			row.Add(new ZCell(scoresList.Average(), ChartType.Bar, isDecimal ? "N1" : "N0"));  // average game score
			if (hitsList != null && hitsList.Count() > 0)
				row.Add(new ZCell(hitsList.Average(), ChartType.Bar, isDecimal ? "N1" : "N0")); // average hits
			if (league.IsPoints())
				row.Add(new ZCell(pointsList.Sum(), ChartType.None, "N0"));  // total points
			if (scoresList.Count < count)
				row.Add(new ZCell(count - scoresList.Count, ChartType.None, "N0"));  // games dropped
			else
				row.Add(new ZCell());  // games dropped
		}

		static void AddAverageAndDrops(League league, ZRow row, Drops drops, List<double> scoresList, List<double> pointsList, bool isDecimal)
		{
			int count = scoresList.Count;

			DropScores(scoresList, drops);
			DropScores(pointsList, drops);
			row.Add(new ZCell(scoresList.Average(), ChartType.Bar, isDecimal ? "N1" : "N0"));  // average game score
			if (league.IsPoints())
				row.Add(new ZCell(pointsList.Sum(), ChartType.None, "N0"));  // total points
			if (scoresList.Count < count)
				row.Add(new ZCell(count - scoresList.Count, ChartType.None, "N0"));  // games dropped
			else
				row.Add(new ZCell());  // games dropped
		}

		/// <summary>Return the team's rank in this game (if any).</summary>
		static int Rank(League league, Game game, LeagueTeam team)
		{
			return game == null ? -1 : game.Teams.FindIndex(t => t.TeamId == team.TeamId) + 1;
		}

		static void SortGridReport(League league, ZoomReport report, ReportTemplate rt, List<Game> games, int averageCol, int pointsCol, bool reversed, int hitsCol = 0)
		{
			switch (rt.ReportType)
			{
				case ReportType.GameGrid: case ReportType.GameGridCondensed:
				report.Rows.Sort(delegate(ZRow x, ZRow y)
			                 {
			                 	double? result = 0;
								 
								 if (league.IsPoints(games))
								 {
									 if (x.Count <= pointsCol || x[pointsCol].Number == null)
										return 1;
			                 		if (y.Count <= pointsCol || y[pointsCol].Number == null)
										return -1;

									 result = y[pointsCol].Number - x[pointsCol].Number;
			                 	}
			                 	 if (result == 0)
			                 	{
									 if (x.Count <= averageCol || x[averageCol].Number == null)
										return 1;
			                 		if (y.Count <= averageCol || y[averageCol].Number == null)
										return -1;

									 result = y[averageCol].Number - x[averageCol].Number;
			                 	}
								 if (result == 0 && hitsCol > 0)
								 {
									 if (x.Count <= hitsCol || x[hitsCol].Number == null)
										return 1;
									if (y.Count <= hitsCol || y[hitsCol].Number == null)
										return -1;

									result = y[hitsCol].Number - x[hitsCol].Number;
								}
								return Math.Sign(result ?? 0);
			                 }
			                );
					break;
				case ReportType.Ascension:
					report.AddColumn(new ZColumn("Last game index"));
					foreach (ZRow row in report.Rows)
					{
						for (int col = Math.Min(averageCol, row.Count()) - 1; col >= 0; col--)
							if (col >= 0 && row[col].Number != null)
							{
								row.Add(new ZCell(col, ChartType.None, "N0"));
								break;
							}
					}

					try
					{  // Sort by comparing teams' score in the last game they played each other.
						report.Rows.Sort(delegate(ZRow x, ZRow y)
							{
								int lastx = (int)x.Last().Number;  // Index of x's last game, as a cell in this row.
								int lasty = (int)y.Last().Number;  // Index of y's last game, as a cell in this row.

								int xRank = Rank(league, (Game)report.Columns[lastx].Tag, (LeagueTeam)x[1].Tag);
								int yRank = Rank(league, (Game)report.Columns[lasty].Tag, (LeagueTeam)y[1].Tag);

								int result = Math.Sign(lasty - lastx);
								if (result == 0)
									result = Math.Sign(xRank - yRank);

								if (!(xRank == 1) && !(yRank == 1))
									return result;
								else if (xRank == 1 && yRank == 1)
									return -result;
								else if (xRank == 1)
									return -1;
								else // yRank == 1
									return +1;
							}
						);
					}
					catch
					{  // If the above produces an ambiguous result, sort teams by whichever one played last.
						report.Rows.Sort(delegate(ZRow x, ZRow y)
							{
								int result = Math.Sign((double)y.Last().Number - (double)x.Last().Number);
								return result == 0 ? Math.Sign((double)y[(int)y.Last().Number].Number - (double)x[(int)x.Last().Number].Number) : result;
							}
						);
					}

					report.RemoveColumn(report.Columns.Count - 1);  // Last game index
					break;
				
				case ReportType.Pyramid: case ReportType.PyramidCondensed:
					PyramidComparer pc = new PyramidComparer
					{
						Columns = report.Columns,
						Reversed = reversed,
						IsPoints = league.IsPoints(games) || rt.ReportType == ReportType.PyramidCondensed
					};
					pc.Setup(report);
					try 
					{
						report.Rows.Sort(pc);
					}
					catch
					{
						report.Title += "...";
					}

					pc.DoColor(report);
					pc.Cleanup();

					break;
			}

			for (int i = 0; i < report.Rows.Count; i++)
				report.Rows[i][0].Number = i + 1;

			report.Title = ReportTitle(rt.ReportType == ReportType.Ascension ? "Ascension" : 
			                           rt.ReportType == ReportType.Pyramid   ? "Pyramid" : 
			                                                                   "Games", league.Title, rt);
		}

		/// <summary>Return a title suitable for use at the top of a report. Remove duplicated words where appropriate.</summary>
		static string ReportTitle(string reportName, string leagueName, ReportTemplate rt)
		{
			string templateName = rt.Title;
			if (!string.IsNullOrEmpty(templateName))
				return templateName;

			string group = rt.Setting("Group");
			if (string.IsNullOrEmpty(group))
				return Utility.JoinWithoutDuplicate(leagueName, reportName);
			else
				return leagueName + " " + Utility.JoinWithoutDuplicate(group, reportName);
		}

		static void FinishReport(ZoomReport report, List<Game> games, ReportTemplate rt)
		{
			report.Title += FromTo(games, rt.From, rt.To);

			if (games.Count == 0)
			{
				if (rt.From == null && rt.To == null)
					report.Description += " No games were found.";
				else
					report.Description += " No games were found in the specified date/time range.";
			}

			if (rt.Settings.Contains("Description"))
			{
				if (rt.From != null && rt.To != null)
					report.Description += " The report has been limited to games " + FromTo(games, rt.From, rt.To) + ".";
				else if (rt.From != null && games.Any())
					report.Description += " The report has been limited to games after " + games.First().Time.ToShortDateString() + ".";
				else if (rt.To != null && games.Any())
					report.Description += " The report has been limited to games before " + games.Last().Time.ToShortDateString() + ".";

				string group = rt.Setting("Group");
				if (!string.IsNullOrEmpty(group))
					report.Description += " The report has been limited to games with title \"" + group + "\".";
			}
		}
	}

	class PyramidComparer: IComparer<ZRow>
	{
		public List<ZColumn> Columns { get; set; }
		/// <summary>Set this to true if the last column is rank -- larger ranks should be sorted lower.</summary>
		public bool Reversed { get; set; }
		public bool IsPoints { get; set; }
		
		List<string> groups;
		int lastGameIndex;
		int lastGroupIndex;
		int averageScoreIndex;
		int averagePointsIndex;
		int previousScoreIndex;
		int previousPointsIndex;

		public void Setup(ZoomReport report)
		{
			groups = Columns.Select(c => c.GroupHeading).Distinct().ToList();
			
			int lastCol = Columns.Count() - 1;
			lastGameIndex = lastCol + 1;
			lastGroupIndex = lastCol + 2;
			averageScoreIndex = lastCol + 3;
			averagePointsIndex = lastCol + 4;
			previousScoreIndex = lastCol + 5;
			previousPointsIndex = lastCol + 6;
			
			report.AddColumn(new ZColumn("Last game index"));
			report.AddColumn(new ZColumn("Last group index"));
			report.AddColumn(new ZColumn("Group av score"));
			report.AddColumn(new ZColumn("Group av points"));
			report.AddColumn(new ZColumn("Previous av score"));
			report.AddColumn(new ZColumn("Previous av points"));

			foreach (ZRow row in report.Rows)
			{
				int lastGame = LastGame(row, lastCol);
				while (row.Count < lastCol) 
					row.Add(new ZCell());  // Pad to end of row.

				row.Add(new ZCell(lastGame));

				string lastGroup = Columns[lastGame].GroupHeading;
				var groupIndex = groups.IndexOf(lastGroup);
				string previousGroup = (groupIndex > 0) ? groups[groupIndex - 1] : null;
				row.Add(new ZCell(groups.IndexOf(lastGroup)));


				CalcRoundScores(row, lastCol, lastGroup, out double averageScore, out double averagePoints);
				row.Add(new ZCell(averageScore));
				row.Add(new ZCell(averagePoints));

				if (previousGroup != null)
					CalcRoundScores(row, lastCol, previousGroup, out averageScore, out averagePoints);
				row.Add(new ZCell(averageScore));
				row.Add(new ZCell(averagePoints));
			}
		}

		void CalcRoundScores(ZRow row, int lastCol, string group, out double averageScore, out double averagePoints)
		{
			int totalScore = 0;
			double totalPoints = 0.0;
			int scoreCount = 0;

			for (int i = 0; i <= lastCol; i++)
				if (Columns[i].GroupHeading == group && row[i].Number.HasValue)
				{
					if (Columns[i].Text == "Pts" || Columns[i].Text == "Points" || Columns[i].Text == "Rank")
						totalPoints += (double)row[i].Number;
					else
					{
						totalScore += (int)row[i].Number;
						scoreCount++;
					}
				}

			if (scoreCount == 0)
			{
				averageScore = double.MinValue;
				averagePoints = double.MinValue;
			}
			else
			{
				averageScore = totalScore / scoreCount;
				averagePoints = totalPoints / scoreCount;
			}
		}

		public void Cleanup()
		{
			for (int i = Columns.Count - 1; i >= lastGameIndex; i--)
				Columns.RemoveAt(i);
		}
		
		int LastGame(ZRow row, int lastCol)
		{
			int col = lastCol;

			while (col > 0 && (col >= Columns.Count || Columns[col].Text == "Average" || Columns[col].Text == "Pts"))
				col--;

			while (col > 0 && row[col].Number == null)
				col--;

			return col;
		}

		public int Compare(ZRow x, ZRow y)
		{
			int xLast = (int)x[lastGroupIndex].Number;
			int yLast = (int)y[lastGroupIndex].Number;

			if (xLast == yLast)  // These teams' last games are in the same round.
			{
				if (IsPoints && y[averagePointsIndex].Number != x[averagePointsIndex].Number)
					return (Reversed ? -1 : 1) * Math.Sign((double)(y[averagePointsIndex].Number - x[averagePointsIndex].Number));  // Compare their last round average points.
				else if (y[averageScoreIndex].Number != null && x[averageScoreIndex].Number != null)
					return Math.Sign((double)(y[averageScoreIndex].Number - x[averageScoreIndex].Number));  // Points were an exact match? Try scores.
				else
					return 0;
			}
			else
			{
				if (xLast == yLast + 1 && (groups[xLast].Contains("repechage") || groups[xLast].Contains("repêchage")))  // x's last game is a repechage.
				{
					if (IsPoints && y[averagePointsIndex].Number != x[previousPointsIndex].Number)
						return (Reversed ? -1 : 1) * Math.Sign((double)(y[averagePointsIndex].Number - x[previousPointsIndex].Number));  // Compare their previous round average points.
					else if (y[averageScoreIndex].Number != null && x[previousScoreIndex].Number != null)
						return Math.Sign((double)(y[averageScoreIndex].Number - x[previousScoreIndex].Number));  // Points were an exact match? Try scores.
				}

				if (xLast + 1 == yLast && (groups[yLast].Contains("repechage") || groups[yLast].Contains("repêchage")))  // y's last game is a repechage.
				{
					if (IsPoints && y[previousPointsIndex].Number != x[averagePointsIndex].Number)
						return (Reversed ? -1 : 1) * Math.Sign((double)(y[previousPointsIndex].Number - x[averagePointsIndex].Number));  // Compare their previous round average points.
					else if (y[previousScoreIndex].Number != null && x[averageScoreIndex].Number != null)
						return Math.Sign((double)(y[previousScoreIndex].Number - x[averageScoreIndex].Number));  // Points were an exact match? Try scores.
				}

				return yLast - xLast;  // Whoever survived longest ranks highest.
			}
		}

		// Mark alternate rounds in slightly different background colours.
		public void DoColor(ZoomReport report)
		{
			foreach (ZRow row in report.Rows)
				for (int i = 0; i < row.Count && i< Columns.Count && groups.IndexOf(Columns[i].GroupHeading) <= (int)row[lastGroupIndex].Number; i++)
					if (!string.IsNullOrEmpty(Columns[i].GroupHeading) && groups.IndexOf(Columns[i].GroupHeading) % 2 == 0 && row[i].Color == Color.Empty)
						row[i].Color = Color.FromArgb(0xF0, 0xF0, 0xFF);
		}
#region Old code
/*
	class PyramidComparerOld: IComparer<ZRow>
	{
		// This version of PyramidComparer attempts to figure out which games are in which round by itself, but I never got it working.
		// The in-use version relies on the user labeling the rounds.
		public HashSet<int> PyramidSet = new HashSet<int>();
		
		public int Compare(ZRow x, ZRow y)
		{
			int lastx = LastGame(x);
			int lasty = LastGame(y);
			HashSet<int> xySet = new HashSet<int>();
			for (int i = Math.Min(lastx, lasty); i < Math.Max(lastx, lasty); i++)
				xySet.Add(i);

			if (PyramidSet.IsSupersetOf(xySet))  // These teams' last games are in the same round.
				return Math.Sign((double)(y[lasty].Number - x[lastx].Number));  // Compare their last scores.
			else
			    return lasty - lastx;  // Whoever survived longest ranks highest.
		}

		// Mark alternate rounds in slightly different background colours.
		public void DoColor(League league, ZoomReport report)
		{
			int step = league.IsPoints() ? 2 : 1;

			foreach (ZRow row in report.Rows)
			{
				int last = LastGame(row);

				while (PyramidSet.Contains(last) && last < row.Count - 1)
					last++;  // Move last out to the end of this round (or repechage).

				bool even = false;
				int col = 2;

				while (col <= last)
				{
					if (even && row[col].Color == Color.Empty)
						row[col].Color = Color.FromArgb(0xF0, 0xF0, 0xFF);

					if (!PyramidSet.Contains(col))
					{
						even = !even;
						col += step;  // Move j out to the end of this round (or repechage).
					}
					else
				    	col++;
				}
			}
		}

		// An element in PyramidSet is set if teams in this game do _not_ play in the next game; i.e. this game and the next are likely part of the same round.
		public void Populate(League league, ZoomReport report)
		{
			int step = league.IsPoints() ? 2 : 1;

			int col = 2;
			if (report.Rows.Count > 1)
				while (col < report.Rows[1].Count - step)
				{
					bool found = false;
					foreach (ZRow row in report.Rows)
						if (row[col].Number != null && row[col + step].Number != null)
						{
					        found = true;
					        break;
						}

					if (!found)
						for (int i = 0; i <= step; i++)
							PyramidSet.Add(col + i);

					col += step + 1;
				}
		}
	}
*/
#endregion
	}
}
