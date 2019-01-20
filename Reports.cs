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

	public delegate string GameHyper (Game game);

	/// <summary>Ladders, game-by-game, etc.</summary>
	public class Reports
	{
		private Reports() {}

		public static ZoomReport TeamLadder(League league, bool includeSecret, ReportTemplate rt)
		{
			ChartType chartType = ChartTypeExtensions.ToChartType(rt.Setting("ChartType"));
		    bool ratio = rt.Setting("OrderBy") == "score ratio";
		    bool showColours = rt.Settings.Contains("ShowColours");

			ZoomReport report = new ZoomReport(string.IsNullOrEmpty(rt.Title) ? league.Title + " Team Ladder" : rt.Title, "Rank,Team", "center,left");
			report.MaxChartByColumn = true;

	 		var colourTotals = new Dictionary<Colour, List<int>>();
			if (showColours)
		 		for (Colour c = Colour.Red; c <= Colour.Orange; c++)
				{
					report.Columns.Add(new ZColumn("1st", ZAlignment.Integer, c.ToString()));
					report.Columns.Add(new ZColumn("2nd", ZAlignment.Integer, c.ToString()));
					report.Columns.Add(new ZColumn("3rd", ZAlignment.Integer, c.ToString()));

					colourTotals.Add(c, new List<int>());
				}

			if (league.IsPoints())
				report.Columns.Add(new ZColumn("Points", ZAlignment.Float));

			report.Columns.Add(new ZColumn(ratio ? "Score Ratio" : "Average score", 
			                               ZAlignment.Float));
			report.Columns.Add(new ZColumn("Games", ZAlignment.Integer));

			if (rt.Drops != null && rt.Drops.HasDrops())
				report.Columns.Add(new ZColumn("Dropped", ZAlignment.Integer));

			List<Game> games = league.Games(includeSecret);

			ZCell barCell = null;
			double scoreTotal = 0;
			double pointsTotal = 0.0;

			List<int> countList = new List<int>();  // Number of games each team has played, for scaling.

			foreach (LeagueTeam team in league.Teams)  // Create a row for each League team.
			{
				ZRow row = new ZRow();

				row.Add(new ZCell(""));  // put in a temporary Rank

				row.Add(TeamCell(team));

				double points = 0;

				var colourCounts = new Dictionary<Colour, List<int>>();
				for (Colour c = Colour.Red; c <= Colour.Orange; c++)
					colourCounts.Add(c, new List<int>());

				List<double> scoreList = new List<double>();  // Holds either scores or score ratios.

				ZCell scoreCell;
				foreach (GameTeam gameTeam in league.Played(team, includeSecret))  // Roll through this team's games.
				{
					Game game = league.Game(gameTeam);
					if ((rt.From == null || game.Time >= rt.From) && (rt.To == null || game.Time <= rt.To))
					{
						if (ratio)
						{
							if (league.Game(gameTeam).TotalScore() != 0)
								scoreList.Add(1.0 * gameTeam.Score / league.Game(gameTeam).TotalScore() * league.Game(gameTeam).Teams.Count);
						}
						else
							scoreList.Add(gameTeam.Score);

						points += gameTeam.Points;

						if (showColours)
						{
							// Add the team's rank in this game to the colourCounts.
							int rank = league.Game(gameTeam).Teams.IndexOf(gameTeam);

							while (colourCounts[gameTeam.Colour].Count <= rank)
								colourCounts[gameTeam.Colour].Add(0);
							colourCounts[gameTeam.Colour][rank]++;

							while (colourTotals[gameTeam.Colour].Count <= rank)
								colourTotals[gameTeam.Colour].Add(0);
							colourTotals[gameTeam.Colour][rank]++;
						}
					}
				}

				if (showColours)
					for (Colour c = Colour.Red; c <= Colour.Orange; c++)
						for (int rank = 0; rank < 3; rank++)
							if (colourCounts[c].Count > rank && colourCounts[c][rank] > 0)
							row.Add(new ZCell(colourCounts[c][rank], ChartType.Bar, "N0", c.ToColor()));
							else
								row.Add(new ZCell("", c.ToColor()));

				if (league.IsPoints())
				{
					barCell = new ZCell(points);  // league points scored
					row.Add(barCell);  // league points scored
				}
				pointsTotal += points;

				int count = scoreList.Count;  // Number of games _before_ any are dropped.

				if (scoreList.Count == 0)
					scoreCell = new ZCell("-");     // average game scores
				else
				{
					//scoreCell = DataCell(scoreList, rt.Drops, chartType, "N0");
					scoreCell = new ZCell(0, chartType);
					DropScores(scoreList, rt.Drops);

					if (ratio)
					{
						scoreCell.Number = (scoreList.Average());
						scoreCell.NumberFormat = "P1";  // average game score ratio
						scoreCell.Data.AddRange(scoreList.Select(x => x / scoreList.Average()));
					}
					else
					{
						scoreCell.Number = (scoreList.Average());
						scoreCell.NumberFormat = "N0";  // average game score
						scoreCell.Data.AddRange(scoreList);
					}

					scoreTotal += scoreList.Average();
				}
				row.Add(scoreCell);     // average game scores
				if (!league.IsPoints())
					barCell = scoreCell;

				row.Add(new ZCell(count, ChartType.None, "N0"));  // games played

				if (scoreList.Count < count)
					row.Add(new ZCell(count - scoreList.Count, ChartType.None, "N0"));  // games dropped

				countList.Add(count);

				if (showColours)
				{
					for (int i = 2; i < 25; i++)
						row[i].ChartCell = row[i];
					for (int i = 26; i < row.Count; i++)
						row[i].ChartCell = barCell;
				}
				else
					foreach (ZCell cell in  row)
						cell.ChartCell = barCell;

				if (count > 0)
					report.Rows.Add(row);
			}  // foreach team in league.Teams

			bool more = false;
			bool less = false;
			int mode = 0;

			if (rt.Settings.Contains("ScaleGames") && countList.Count > 0 && countList.Min() != countList.Max())  // Find the mode, and add scaled columns.
			{
				// Calculate mode, the number of games _most_ teams have played.
				var groups = countList.GroupBy(x => x);
				int maxCount = groups.Max(g => g.Count());
				mode = groups.Last(g => g.Count() == maxCount).Key;

				if (league.IsPoints())
					report.Columns.Add(new ZColumn("Scaled points", ZAlignment.Right));

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
							row.Add(new ZCell(""));  // scaled points
					}
				}

//				report.OnCalcBar = TeamLadderScaledCalcBar;
			}  // if ScaleGames

			int scoreColumn = report.Columns.FindIndex(x => x.Text.Contains("core"));
			int pointsColumn = report.Columns.FindIndex(x => x.Text == "Points");

			report.Rows.Sort(delegate(ZRow x, ZRow y)
			                 {
								double? result = 0;
								if (pointsColumn != -1)
								{
									if (x[pointsColumn].Number == null)
										return 1;
									if (y[pointsColumn].Number == null)
										return -1;

									result = y[pointsColumn].Number - x[pointsColumn].Number;
								}
								if (result == 0 && scoreColumn != -1)
								{
									if (x[scoreColumn].Number == null)
										return 1;
									if (y[scoreColumn].Number == null)
										return -1;

									result = y[scoreColumn].Number - x[scoreColumn].Number;
								}
								return Math.Sign(result ?? 0);
			                 }
			                );
			
			for (int i = 0; i < report.Rows.Count; i++)
				report.Rows[i][0].Number = i + 1;

			if (showColours)  // Add a Totals row at the bottom of the report.
			{
				ZRow row = new ZRow();
				report.Rows.Add(row);
				row.Add(new ZCell("", Color.Gray));  // Blank rank.
				row.Add(new ZCell("Totals", Color.Gray));  // "Team" name.

		 		for (Colour c = Colour.Red; c <= Colour.Orange; c++)
		 			for (int rank = 0; rank < 3; rank++)
		 			{
		 			Color dark = Color.FromArgb((c.ToColor().R + Color.Gray.R) / 2, (c.ToColor().G + Color.Gray.G) / 2, (c.ToColor().B + Color.Gray.B) / 2);
		 				if (colourTotals[c].Count > rank)
		 					row.Add(BlankZero(colourTotals[c][rank], ChartType.Bar, dark));
		 				else
		 					row.Add(new ZCell("", dark));
		 			}

				if (league.IsPoints())
					row.Add(new ZCell(pointsTotal / report.Rows.Count(), ChartType.None, "F1", Color.Gray));  // average league points scored

				if (ratio)
					row.Add(new ZCell(scoreTotal * 100.0 / report.Rows.Count(), ChartType.None, "P1", Color.Gray));     // average game score ratio
				else
					row.Add(new ZCell(scoreTotal / report.Rows.Count(), ChartType.None, "N0", Color.Gray));     // average game score

				row.Add(new ZCell(countList.Average(), ChartType.None, "F1", Color.Gray));  // average games played

				if (rt.Drops != null && rt.Drops.HasDrops())
					row.Add(new ZCell(""));  // games dropped

				// Clear groups of three columns that contain only '0's.
				for (int i = 23; i > 0; i -= 3)
					if (report.ColumnZero(i) && report.ColumnZero(i + 1) && report.ColumnZero(i + 2))
					{
						report.RemoveColumn(i + 2);
						report.RemoveColumn(i + 1);
						report.RemoveColumn(i);
					}

//			report.OnCalcBar = TeamLadderColourCalcBar;
			}
			//report.Rows.PurgeBlankRows;

			if (rt.Settings.Contains("Description"))
			{
				report.Description = "This report ranks teams.";

				FinishReport(league, report, games, rt);
		
				if (more && less)
					report.Description += " Teams with more or less than " + mode.ToString(CultureInfo.CurrentCulture) + " games have been scaled down or up. This is shown in the Scaled column.";
				else if (more)
					report.Description += " Teams with more than " + mode.ToString(CultureInfo.CurrentCulture) + " games have been scaled down. This is shown in the Scaled column.";
				else if (less)
					report.Description += " Teams with less than " + mode.ToString(CultureInfo.CurrentCulture) + " games have been scaled up. This is shown in the Scaled column.";
	
				if (showColours)
					report.Description += " For each team, the report shows the number of times they place first, second and third on each colour. The Totals row shows the total number of firsts, seconds and thirds that were scored by each colour.";

				if (report.Description == "This report ranks teams. ")  // No interesting boxes are checked,
			  			report.Description = "";  // so this description is too boring to show.

			}
			else
				FinishReport(league, report, games, rt);

			return report;
		}  // BuildTeamLadder()

		/// <summary>Build a square tale showing how many times each team has played (and beaten) each other team.</summary>
		public static ZoomReport TeamsVsTeams(League league, bool includeSecret, ReportTemplate rt, GameHyper gameHyper)
		{
			ZoomReport report = new ZoomReport(string.IsNullOrEmpty(rt.Title) ? league.Title + " Teams vs Teams" : rt.Title, "Team", "left");
			
			List<LeagueTeam> teams = new List<LeagueTeam>();
			teams.AddRange(league.Teams);
			teams.Sort(delegate(LeagueTeam x, LeagueTeam y)
			           {
			           	double result = league.AveragePoints(y, includeSecret) - league.AveragePoints(x, includeSecret);
			           	return Math.Sign(result == 0 ? league.AverageScore(y, includeSecret) - league.AverageScore(x, includeSecret) : result);
			           });

			List<Game> games = league.Games(includeSecret).Where(g => g.Time > (rt.From ?? DateTime.MinValue) && g.Time < (rt.To ?? DateTime.MaxValue)).ToList();

			foreach (var team1 in teams)
			{
				var column = new ZColumn(team1.Name, ZAlignment.Center);
				column.Hyper = "team" + team1.Id.ToString("D2", CultureInfo.InvariantCulture) + ".html";
				column.Rotate = true;
				report.Columns.Add(column);

				var row = new ZRow();
				report.Rows.Add(row);

				row.Add(TeamCell(team1));

				foreach (var team2 in teams)
				{
					var cellGames = games.FindAll(x => x.Teams.Any(y => league.LeagueTeam(y) == team1) &&
					                                   x.Teams.Any(z => league.LeagueTeam(z) == team2));  // Get games that include these two teams.
					ZCell cell;
					if (team1 == team2)
						cell = new ZCell("\u2572", Color.Gray);
					else if (cellGames.Count == 0)
						cell = new ZCell((string)null);
					else
					{
						var wins = cellGames.Count(x => x.Teams.FindIndex(y => league.LeagueTeam(y) == team1) < x.Teams.FindIndex(z => league.LeagueTeam(z) == team2));
						cell = new ZCell(wins.ToString(CultureInfo.CurrentCulture) +
						                     "/" + cellGames.Count().ToString(CultureInfo.CurrentCulture));
						cell.Number = (double)wins / cellGames.Count();
						cell.ChartType = ChartType.Bar;
						if (cellGames.Count == 1)
							cell.Hyper = gameHyper(cellGames[0]);
					}
					row.Add(cell);
				}
			}

			if (rt.Settings.Contains("Description"))
				report.Description = "This report shows how may times each team has played and beaten each other team." + FromTo(games, rt.From, rt.To); 
			return report;
		}  // TeamsVsTeams

		/// <summary>Build a list of games over a specified date/time range. One game per row.</summary>
		public static ZoomReport GamesList(League league, bool includeSecret, ReportTemplate rt, GameHyper gameHyper)
		{
			ZoomReport report = new ZoomReport("", "Game", "right");

//			report.MultiColumnOK = true;

			int thisgame = 0;
			List<Game> games = league.Games(includeSecret);
			games = games.Where(g => g.Time > (rt.From ?? DateTime.MinValue) && g.Time < (rt.To ?? DateTime.MaxValue)).ToList();
			games.Sort();

			int mostTeams = games.Count == 0 ? 0 : games.Max(g => g.Teams.Count);

	 		while (thisgame < games.Count)// && (to == null || games[thisgame].Time < to))  // loop through each game, and create a row for it
			{
				Game game = games[thisgame];
				if (thisgame == 0 || (games[thisgame - 1].Time.Date < game.Time.Date))  // We've crossed a date boundary, so
				{
					ZRow dateRow = new ZRow();  // create a row
					report.Rows.Add(dateRow);
					dateRow.Add(new ZCell(game.Time.ToShortDateString()));  // to show the new date.
				}

				ZRow row = new ZRow();;
				report.Rows.Add(row);

				ZCell dateCell = new ZCell((game.Title + " " + game.Time.ToShortTimeString()).Trim());
				dateCell.Hyper = gameHyper(game);
				row.Add(dateCell);

				foreach (GameTeam gameTeam in game.Teams)
				{
					ZCell teamCell = new ZCell(league.LeagueTeam(gameTeam) == null ? "Team ?" : league.LeagueTeam(gameTeam).Name,
					                           gameTeam.Colour.ToColor());  // team name
					teamCell.Hyper = "team" + (gameTeam.TeamId ?? -1).ToString("D2", CultureInfo.InvariantCulture) + ".html";
					row.Add(teamCell);
					ZCell scoreCell = new ZCell(gameTeam.Score, ChartType.Bar, "N0", gameTeam.Colour.ToColor());
					row.Add(scoreCell);
					teamCell.ChartCell = scoreCell;
					scoreCell.ChartCell = scoreCell;
					if (league.IsPoints())  // there are victory points for this league
					{
						if (game.IsPoints())
							row.Add(new ZCell(gameTeam.Points, ChartType.None, "", gameTeam.Colour.ToColor()));
						else
							row.Add(new ZCell(""));  // This whole game has no victory points, so don't show any.
						row.Last().ChartCell = scoreCell;
					}
					else
						row.Add(new ZCell(""));
				}  // foreach gameTeam

				if (league.VictoryPointsHighScore != 0 && game.Players.Count > 0 && league.LeaguePlayer(game.Players[0]) != null)  // there is a highscore entry at the end of each row
				{
					for (int i = game.Teams.Count; i < mostTeams; i++)
					{
						row.Add(new ZCell());
						row.Add(new ZCell());
						row.Add(new ZCell());
					}

					row.Add(new ZCell(""));
					Color highScoreColor = Color.Empty;
					foreach (GameTeam gameTeam in game.Teams)
						if (gameTeam.TeamId == game.Players[0].GameTeamId)
							highScoreColor = gameTeam.Colour.ToColor();
						
					row.Add(new ZCell(league.LeaguePlayer(game.Players[0]).Name, highScoreColor));
					row.Add(new ZCell(game.Players[0].Score, ChartType.Bar, "N0", highScoreColor));
					row[row.Count - 2].ChartCell = row.Last();
					row.Last().ChartCell = row.Last();
				}

			    thisgame++;
			}  // while date/time <= Too

	 		if (games.Count > 0 && games.First().Time.Date == games.Last().Time.Date)
			    report.Rows.RemoveAt(0);  // The first entry in rows is a date line; since there's only one date for the whole report, we can delete it.
	 		report.Title = (string.IsNullOrEmpty(rt.Title) ? league.Title + " Games " : rt.Title) + FromTo(games, rt.From, rt.To);
			
			for (int i = 0; i < mostTeams; i++)  // set up the Headings text, to cater for however many columns the report has turned out to be
			{
				report.Columns.Add(new ZColumn("Team", ZAlignment.Left));
				report.Columns.Add(new ZColumn("Score", ZAlignment.Right));
				if (league.IsPoints())  // there are victory points for this league
					report.Columns.Add(new ZColumn("Pts", ZAlignment.Right));
				else
					report.Columns.Add(new ZColumn(""));
			}

				if (league.VictoryPointsHighScore != 0)  // set up Headings text to show the highscoring player
				{
		            report.Columns.Add(new ZColumn("", ZAlignment.Right));
		            report.Columns.Add(new ZColumn("Best player", ZAlignment.Right));
		            report.Columns.Add(new ZColumn("Score", ZAlignment.Right));
				}

				if (rt.Settings.Contains("Description"))
				{
					report.Description = "This is a list of games. Each row in the table is one game. Across each row, you see the teams that were in that game (with the team that placed first listed first, and so on), and the score for each team.";

					if (league.VictoryPointsHighScore != 0)
						report.Description += " At the end of each row, you see the high-scoring player for that game, and their score.";

					if (rt.From != null || rt.To != null)
						report.Description += "  The report has been limited to games " + FromTo(games, rt.From, rt.To) + ".";
				}
			return report;
		}  // GamesList

		/// <summary> Build a list of games over a specified date/time range. One team per row; one game per column.
		public static ZoomReport GamesGrid(League league, bool includeSecret, ReportTemplate rt, GameHyper gameHyper)
		{
			ZoomReport report = new ZoomReport("", "Rank,Team", "center,left");

			DateTime thisgametime = DateTime.MinValue;
			List<Game> games = league.Games(includeSecret).Where(g => g.Time > (rt.From ?? DateTime.MinValue) && g.Time < (rt.To ?? DateTime.MaxValue)).ToList();

	 		// Create columns.
	 		foreach (Game game in games)
			{
				thisgametime = game.Time;
				ZColumn column = new ZColumn(
					game.Time.Date < thisgametime.Date ? 
					game.Time.ToShortDateString() + " " + game.Time.ToShortTimeString() :
					game.Time.ToShortTimeString(),
					ZAlignment.Integer);
				column.GroupHeading = game.Title;
				column.Hyper = gameHyper(game);
				report.Columns.Add(column);

				if (league.IsPoints(games))
				{
					column = new ZColumn("Pts");
					column.GroupHeading = game.Title;
					report.Columns.Add(column);
				}
	 		}

	 		report.Columns.Add(new ZColumn("Average"));
			int averageCol = report.Columns.Count() - 1;
			int pointsCol = averageCol;
			if (league.IsPoints(games))
			{
				report.Columns.Add(new ZColumn("Pts"));
				pointsCol = report.Columns.Count() - 1;
			}

			if (rt.Drops != null && (rt.Drops.DropWorst(100) > 0 || rt.Drops.DropBest(100) > 0))
				report.Columns.Add(new ZColumn("Dropped", ZAlignment.Integer));

			foreach (LeagueTeam leagueTeam in league.Teams)
			{
				List<double> scoresList = new List<double>();
				List<double> pointsList = new List<double>();

				ZRow row = new ZRow();;

				row.Add(new ZCell(0, ChartType.None, "N0")); // Temporary blank rank.

				row.Add(TeamCell(leagueTeam));
				
				// Add a cell for each game for this team. If the team is not in this game, add a blank.
		 		foreach (Game game in games)
	 			{
	 				GameTeam gameTeam = game.Teams.Find(x => league.LeagueTeam(x) == leagueTeam);
	 				if (gameTeam == null)
	 				{
	 					row.Add(new ZCell(""));
						if (league.IsPoints(games))
							row.Add(new ZCell(""));
	 				}
	 				else
	 				{
	 					row.Add(new ZCell(gameTeam.Score, ChartType.Bar, "N0", gameTeam.Colour.ToColor()));
						if (league.IsPoints(games))
							row.Add(new ZCell(gameTeam.Points, ChartType.None, "", gameTeam.Colour.ToColor()));
						scoresList.Add(gameTeam.Score);
						pointsList.Add(gameTeam.Points);
	 				}
				}  // foreach gameTeam

				if (scoresList.Count > 0)
				{
					AddAverageAndDrops(league, row, rt.Drops, scoresList, pointsList);
					report.Rows.Add(row);
				}
			}  // foreach leagueTeam

			SortGridReport(league, report, rt.ReportType, games, averageCol, pointsCol, false);

			if (rt.Settings.Contains("Description"))
				report.Description = "This is a grid of games. Each row in the table is one team. Each column is one game.";
			FinishReport(league, report, games, rt);

			return report;
		}  // GamesGrid

		/// <summary> Build a list of games over a specified date/time range. One team per row.</summary>
		public static ZoomReport GamesGridCondensed(League league, bool includeSecret, ReportTemplate rt, GameHyper gameHyper)
		{
			ZoomReport report = new ZoomReport("", "Rank,Team", "center,left");

			DateTime newTo = rt.To ?? DateTime.MaxValue;
			List<Game> games = league.Games(includeSecret).Where(g => g.Time > (rt.From ?? DateTime.MinValue) && g.Time < newTo).ToList();

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

				titleCount.Add(gameTitle, maxCount);
			}

			// Create columns.
			foreach (var tc in titleCount)
				for (int i = 0; i < tc.Value; i++)
				{
					ZColumn column = new ZColumn("Time");
					column.GroupHeading = tc.Key;
					report.Columns.Add(column);
					column = new ZColumn("Score", ZAlignment.Right);
					column.GroupHeading = tc.Key;
					report.Columns.Add(column);
					column = new ZColumn(league.IsPoints() ? "Pts" : "Rank", ZAlignment.Right);
					column.GroupHeading = tc.Key;
					report.Columns.Add(column);
		 		}

	 		report.Columns.Add(new ZColumn("Average"));
			int averageCol = report.Columns.Count() - 1;
			int pointsCol = averageCol;
			if (league.IsPoints())
			{
				report.Columns.Add(new ZColumn("Pts"));
				pointsCol = report.Columns.Count() - 1;
			}

			if (rt.Drops != null && (rt.Drops.DropWorst(100) > 0 || rt.Drops.DropBest(100) > 0))
				report.Columns.Add(new ZColumn("Dropped", ZAlignment.Integer));

			foreach (LeagueTeam leagueTeam in league.Teams)
			{
				var scoresList = new List<double>();
				var pointsList = new List<double>();

				ZRow row = new ZRow();;

				row.Add(new ZCell(0, ChartType.None, "N0")); // Temporary blank rank.

				row.Add(TeamCell(leagueTeam));
				
				int col = 2;
				while (col < averageCol)
				{
					string gameTitle = report.Columns[col].GroupHeading;
					// Add a cell for each game for this team.
					foreach (Game game in games.Where(x => x.Title == gameTitle))
		 			{
						GameTeam gameTeam = game.Teams.Find(x => league.LeagueTeam(x) == leagueTeam);
		 				if (gameTeam != null)
		 				{
		 					row.Add(new ZCell(game.Time.ToShortTimeString()));
		 					row.Add(new ZCell(gameTeam.Score, ChartType.Bar, "N0", gameTeam.Colour.ToColor()));
		 					row.Add(new ZCell(league.IsPoints() ? gameTeam.Points : game.Teams.IndexOf(gameTeam) + 1, ChartType.None, "", 
		 					                  gameTeam.Colour.ToColor()));
							scoresList.Add(gameTeam.Score);
							pointsList.Add(gameTeam.Points);
							col += 3;
		 				}
					}  // foreach game in title

					// Fill in blanks to the end of this title.
					while (report.Columns[col].GroupHeading == gameTitle)
					{
	 					row.Add(new ZCell(""));
	 					row.Add(new ZCell(""));
	 					row.Add(new ZCell(""));
	 					col += 3;
					}
				}

				if (scoresList.Count > 0)
				{
					AddAverageAndDrops(league, row, rt.Drops, scoresList, pointsList);
					report.Rows.Add(row);
				}
			}  // foreach leagueTeam

			SortGridReport(league, report, rt.ReportType, games, averageCol, pointsCol, !league.IsPoints());
			if (rt.Settings.Contains("Description"))
				report.Description = "This is a grid of games. Each row in the table is one team.";
			FinishReport(league, report, games, rt);

			return report;
		}  // GamesGridCondensed

		public static ZoomReport SoloLadder(League league, bool includeSecret, ReportTemplate rt)
		{
			ChartType chartType = ChartTypeExtensions.ToChartType(rt.Setting("ChartType"));

			ZoomReport report = new ZoomReport(string.IsNullOrEmpty(rt.Title) ? league.Title + " Solo Ladder" : rt.Title,
			                                   "Rank,Player,Team,Average Score,Average Rank,Tags +,Tags-,Tag Ratio,Score Ratio,TR\u00D7SR,Destroys,Denies,Denied,Yellow,Red,Games",
			                                   "center,left,left,integer,integer,integer,integer,float,float,float,integer,integer,integer,integer,integer,integer,integer",
			                                   ",,,,,Tags,Tags,Ratios,Ratios,Ratios,Base,Base,Base,Penalties,Penalties,");
			report.MaxChartByColumn = true;

			if (rt.Drops != null && rt.Drops.HasDrops())
				report.Columns.Add(new ZColumn("Dropped", ZAlignment.Integer));

			var playerTeams = league.BuildPlayerTeamList();
			foreach (var pt in playerTeams)
			{
				var player = pt.Key;
				var games = league.Games(includeSecret).Where(x => 
				                                              (rt.From ?? DateTime.MinValue) < x.Time &&
				                                              x.Time < (rt.To ?? DateTime.MaxValue) &&
				                                              x.Players.Exists(y => y.PlayerId == player.Id));

				if (games.Count() > 0)
				{
					ZRow row = new ZRow();
					report.Rows.Add(row);
					row.Add(new ZCell(0, ChartType.None, "N0"));  // Temporary rank
					row.Add(new ZCell(player.Name));  // Player alias
					row.Last().Hyper = "players.html#player" + player.Id;

					if (pt.Value.Count() == 1)
						row.Add(TeamCell(pt.Value.First()));
					else
						row.Add(new ZCell(string.Join(", ", pt.Value.Select(x => x.Name))));  // Team(s) played for

					row.Add(DataCell(player.Played.Select(x => (double)x.Score).ToList(), rt.Drops, chartType, "N0"));  // Av score
					row.Add(DataCell(player.Played.Select(x => (double)x.Rank).ToList(), rt.Drops, chartType, "N2"));  // Av rank
					row.Add(DataCell(player.Played.Select(x => (double)x.HitsBy).ToList(), rt.Drops, chartType, "N0"));  // Tags +
					row.Add(DataCell(player.Played.Select(x => (double)x.HitsOn).ToList(), rt.Drops, chartType, "N0"));  // Tags -
					row.Add(DataCell(player.Played.Select(x => (double)x.HitsBy / x.HitsOn).ToList(), rt.Drops, chartType, "P1"));  // Tag ratio

					List<double> scoreRatios = new List<double>();
					List<double> srxTrs = new List<double>();
					foreach (var played in player.Played)
					{
						var game = league.Game(played);
						if (game != null)
						{
							scoreRatios.Add(((double)played.Score) / game.TotalScore() * game.Players.Count);
							srxTrs.Add(((double)played.Score) / game.TotalScore() * game.Players.Count * played.HitsBy / played.HitsOn);
						}
					}
					row.Add(DataCell(scoreRatios, rt.Drops, chartType, "P1"));  // Score ratio
					row.Add(DataCell(srxTrs, rt.Drops, chartType, "P1"));  // SR x TR

					row.Add(DataCell(player.Played.Select(x => (double)x.BaseHits).ToList(), rt.Drops, ChartType.Bar, "N1"));
					row.Add(DataCell(player.Played.Select(x => (double)x.BaseDestroys).ToList(), rt.Drops, ChartType.Bar, "N1"));
					row.Add(DataCell(player.Played.Select(x => (double)x.BaseDenies).ToList(), rt.Drops, ChartType.Bar, "N1"));
					row.Add(DataCell(player.Played.Select(x => (double)x.BaseDenied).ToList(), rt.Drops, ChartType.Bar, "N1"));
					row.Add(DataCell(player.Played.Select(x => (double)x.YellowCards).ToList(), rt.Drops, ChartType.Bar, "N1"));
					row.Add(DataCell(player.Played.Select(x => (double)x.RedCards).ToList(), rt.Drops, ChartType.Bar, "N1"));

					row.Add(new ZCell(games.Count(), ChartType.None, "N0"));  // Games

					if (rt.Drops == null)
						row.Add(new ZCell(""));  // games dropped
					else
					{
						int countAfterDrops = rt.Drops.CountAfterDrops(player.Played.Count);
						if (countAfterDrops < player.Played.Count)
				            row.Add(new ZCell(player.Played.Count - countAfterDrops, ChartType.None, "N0"));  // games dropped
			    	    else
							row.Add(new ZCell(""));  // games dropped
					}

 					row[0].ChartCell = row[3];
 					row[1].ChartCell = row[3];
 					row[2].ChartCell = row[3];
 					row[3].ChartCell = row[3];
				}
			}

			report.Rows.Sort(delegate(ZRow x, ZRow y)
			                 {
			                 	double? result = y[3].Number - x[3].Number;
			                 	return Math.Sign((double)result);
			                 }
			                );

			for (int i = 0; i < report.Rows.Count; i++)
				report.Rows[i][0].Number = i + 1;

			report.RemoveZeroColumns();
			return report;
		}  // SoloLadder

		public static void FillDetails(ZRow row, GamePlayer gamePlayer, Color color, double averageScore)
		{
			row.Add(new ZCell(gamePlayer.Score, ChartType.Bar, "N0", color));
			row.Add(BlankZero(gamePlayer.HitsBy, ChartType.Bar, color));
			row.Add(BlankZero(gamePlayer.HitsOn, ChartType.Bar, color));

			if (gamePlayer.HitsOn == 0 && gamePlayer.HitsBy == 0)  // Tag ratio
				row.Add(new ZCell("", color));
			else if (gamePlayer.HitsOn == 0)
				row.Add(new ZCell("infinite", color));
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
			    row.Add(new ZCell((double)gamePlayer.HitsBy / gamePlayer.HitsOn * gamePlayer.Score / averageScore, ChartType.Bar, "P0", color));

			row.Add(BlankZero(gamePlayer.BaseDestroys, ChartType.Bar, color));
			row.Add(BlankZero(gamePlayer.BaseDenies, ChartType.Bar, color));
			row.Add(BlankZero(gamePlayer.BaseDenied, ChartType.Bar, color));
			row.Add(BlankZero(gamePlayer.YellowCards, ChartType.Bar, color));
			row.Add(BlankZero(gamePlayer.RedCards, ChartType.Bar, color));
		}

		public static void FillAverages(ZoomReport report, ZRow averageRow)
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
					averageRow.Add(new ZCell(""));
				else if (format == "N0" && total / count < 100)
					averageRow.Add(new ZCell(total / count, ChartType.Bar, "N1"));
				else
					averageRow.Add(new ZCell(total / count, ChartType.Bar, format));
			}
		}

		public static ZoomReport OneGame(League league, Game game)
		{
			ZoomReport report = new ZoomReport(game.LongTitle(),
			                                   "Rank,Name,Score,Tags +,Tags -,Tag Ratio,Score Ratio,TR\u00D7SR,Destroys,Denies,Denied,Yellow,Red",
			                                   "center,left,integer,integer,integer,float,float,float,integer,integer,integer,integer,integer,integer",
			                                   ",,,Tags,Tags,Ratio,Ratio,Ratio,,Base,Base,Base,Penalties,Penalties");
			report.MaxChartByColumn = true;
//			for (int i = 8; i < report.Columns.Count; i++)
//				report.Columns[i].Rotate = true;

			var gameTotal = new GamePlayer();

			foreach (GameTeam gameTeam in game.Teams)
			{
				var teamTotal = new GamePlayer();

				foreach (GamePlayer gamePlayer in gameTeam.Players)
				{
					// Add a row for each player on the team.
					ZRow row = new ZRow();

					Color color = (gamePlayer.Colour == Colour.None ? gameTeam.Colour : gamePlayer.Colour).ToColor();
					row.Add(new ZCell(game.Rank(gamePlayer.PlayerId), ChartType.None, "N0", color));  // Rank

					if (league.LeaguePlayer(gamePlayer) == null)
						row.Add(new ZCell("Player " + gamePlayer.PlayerId, color));
					else						
					{
						var leaguePlayer = league.LeaguePlayer(gamePlayer);
						row.Add(new ZCell(leaguePlayer.Name, color));
						row.Last().Hyper = "players.html#player" + leaguePlayer.Id;
					}

					FillDetails(row, gamePlayer, color, (double)game.TotalScore() / game.Players.Count);

					teamTotal.Add(gamePlayer);
					gameTotal.Add(gamePlayer);

					report.Rows.Add(row);
				}

				if (gameTeam.Adjustment != 0)
				{
					ZRow adjustmentRow = new ZRow();
					Color color = gameTeam.Colour.ToColor();

					adjustmentRow.Add(new ZCell(""));  // Rank.
					adjustmentRow.Add(new ZCell("Adjustment", color));

					var adjustment = new GamePlayer();
					adjustment.Score = gameTeam.Adjustment;
					FillDetails(adjustmentRow, adjustment, color, 0);

					report.Rows.Add(adjustmentRow);
				}

				// Add a row for the team.
				ZRow teamRow = new ZRow();
				Color teamColor = gameTeam.Colour.ToColor();
				teamColor = Color.FromArgb(teamColor.A, (int)(teamColor.R * 0.75), (int)(teamColor.G * 0.75), (int)(teamColor.B * 0.75));

				teamRow.Add(new ZCell(""));  // Rank.

				if (league.LeagueTeam(gameTeam) == null)
					teamRow.Add(new ZCell("Team " + (gameTeam.TeamId ?? -1).ToString(CultureInfo.InvariantCulture), teamColor));
				else
				{
					ZCell teamCell = new ZCell(league.LeagueTeam(gameTeam).Name, teamColor);
					teamCell.Hyper = "team" + (gameTeam.TeamId ?? -1).ToString("D2", CultureInfo.InvariantCulture) + ".html";
					teamRow.Add(teamCell);
				}

				teamTotal.Score = gameTeam.Score;
				FillDetails(teamRow, teamTotal, teamColor, (double)game.TotalScore() / game.Players.Count * gameTeam.Players.Count);

				report.Rows.Add(teamRow);
			}

			// Add an overall game average row.
			ZRow gameRow = new ZRow();
			gameRow.Add(new ZCell(""));  // Rank
			gameRow.Add(new ZCell("Teams Average"));  // Name
			gameTotal.DivideBy(game.Teams.Count);
			FillDetails(gameRow, gameTotal, Color.Empty, game.TotalScore() / game.Teams.Count);

			report.Rows.Add(gameRow);

			report.RemoveZeroColumns();
			return report;
		}  // OneGame

		public static ZoomReport GameHeatMap(League league, Game game)
		{
			ZoomReport report = new ZoomReport(game.LongTitle(), "Player", "left");

			foreach (var gameTeam in game.Teams)
			{
				var leagueTeam = league.LeagueTeam(gameTeam);

				foreach (var player1 in gameTeam.Players)
				{
					var leaguePlayer = league.LeaguePlayer(player1);
					string name = leaguePlayer == null ? player1.PlayerId : leaguePlayer.Name;
					Color color = (player1.Colour == Colour.None ? gameTeam.Colour : player1.Colour).ToColor();

					// Add a row and a column for each player on the team.
					var column = new ZColumn(name);
					column.GroupHeading = leagueTeam == null ? "Team ??" : leagueTeam.Name;
					column.Alignment = ZAlignment.Integer;
					report.Columns.Add(column);

					var row = new ZRow();
					row.Add(new ZCell(name, color));

					foreach (var gameTeam2 in game.Teams)
						foreach (var player2 in gameTeam2.Players)
						{
							ZCell cell;
							if (player1 == player2)
								cell = new ZCell("\u2572", Color.Gray);
							else if (player1 is ServerPlayer && player2 is ServerPlayer)
							{
								int count = game.ServerGame.Events.Count(x => x.PandCPlayerId == ((ServerPlayer)player1).PandCPlayerId && x.HitPlayer == ((ServerPlayer)player2).PandCPlayerId && x.Event_Type < 14);
								cell = BlankZero(count, ChartType.Bar, gameTeam == gameTeam2 ? color : Color.Empty);
								if (gameTeam != gameTeam2)
									cell.BarColor = ZReportColors.Darken(color);
							}
							else
								cell = new ZCell("");

							row.Add(cell);
						}

					var s = new StringBuilder();
					var startTime = game.ServerGame.Events.FirstOrDefault().Time;
					int minutes = 0;  // How many whole minutes into the game are we?
					if (player1 is ServerPlayer)
						foreach (var eevent in game.ServerGame.Events.Where(x => x.PandCPlayerId == ((ServerPlayer)player1).PandCPlayerId && ((x.Event_Type >= 28 && x.Event_Type <= 34) ||(x.Event_Type >= 37 && x.Event_Type <= 1404))))
						{
							int now = (int)Math.Truncate(eevent.Time.Subtract(startTime).TotalMinutes);
							s.Append('\u00B7', now - minutes);  // Add one dot for each whole minute of the game.
							minutes = now;
	
							switch (eevent.Event_Type)
							{
									case 28: s.Append('\u25af'); break;  // warning: open rectangle
									case 29: s.Append('\u25ae'); break;  // terminated: filled rectangle
									case 30: s.Append('\u25cb'); break;  // hit base: open circle
									case 31: s.Append('\u2b24'); break;  // destroyed base: filled circle
									case 32: s.Append("\U0001f480"); break;  // eliminated: skull
									case 33: s.Append('!'); break;  // hit by base
									case 34: s.Append('!'); break;  // hit by mine
									case 37: case 38: case 39: case 40: case 41: case 42: case 43: case 44: case 45: case 46: s.Append('!'); break;  // player tagged target
									case 1401: case 1402: s.Append('\u29bb', eevent.ShotsDenied / 2); if (eevent.ShotsDenied == 1) s.Append('\u2300'); break;  // score denial points: circle with cross, circle with slash
									case 1403: case 1404: s.Append(eevent.ShotsDenied == 1 ? "\U0001f61e" : "\U0001f620"); break;  // lose points for being denied: sad face, angry face
									default: s.Append('?'); break;
							}
						}
					s.Append('\u00B7', (int)Math.Truncate(game.ServerGame.Events.LastOrDefault().Time.Subtract(startTime).TotalMinutes) - minutes);  // Add one dot for each whole minute of the game.
					row.Add(new ZCell(s.ToString()));

					report.Rows.Add(row);
				}
			}
			
			report.Columns.Add(new ZColumn("Base hits etc.", ZAlignment.Left));

			return report;
		}

		// value, scaleMin and scaleMax are all in the before-scaling ordinate system. outputRange gives the range of the after-scaling ordinate system.
		static float Scale(double value, float outputRange, float scaleMin, float scaleMax)
		{
			return ((float)value - scaleMin) / (scaleMax - scaleMin) * outputRange;
		}

		static float Scale(DateTime value, double outputRange, DateTime scaleMin, DateTime scaleMax)
		{
			return (float)(value.Subtract(scaleMin).TotalMilliseconds / scaleMax.Subtract(scaleMin).TotalMilliseconds * outputRange);
		}

		// If i > 1000, show i / 1000 + "K". If i > 1E6, show i / 1E6 + "M".
		static string ToK(double i)
		{
			if (Math.Abs(i) < 1999)
				return i.ToString("F0");
			if (Math.Abs(i) < 9999)
				return (i / 1000.0).ToString("F1") + "K";
			if (Math.Abs(i) < 999999)
				return (i / 1000.0).ToString("F0") + "K";
			if (Math.Abs(i) < 9999999)
				return (i / 1000000.0).ToString("F1") + "M";
			return (i / 1000000.0).ToString("F0") + "M";
		}

		public static Bitmap GameWorm(League league, Game game, bool includeSecret)
		{
			var maxLeagueScore = league.Games(includeSecret).Max(g => g.Teams.Max(t => t.Score));
			var minScore = Math.Min(game.Teams.Min(x => x.Score), 0);  // Handle games where all teams scores are positive, some are negative, 
			var maxScore = Math.Max(game.Teams.Max(x => x.Score), 0);  // or all are negative (e.g. Lord of the Ring).
			double duration = game.ServerGame.EndTime.Subtract(game.Time).TotalSeconds;
			var height = (int)Math.Ceiling(duration * maxScore / maxLeagueScore);
			var skew = Scale(game.ServerGame.Events.Sum(e => e.Event_Type < 28 ? e.Score : 0) / game.Teams.Count, height, minScore, maxScore) / duration;  // In points per second, or points per pixel.

			var bitmap = new Bitmap((int)duration, height + (int)(skew * duration));
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

			var playerIds = game.ServerGame.Players.Select(p => p.PandCPlayerId).Distinct();
			var playerTeams = new Dictionary<int, GameTeam>();  // Dictionary to let us quickly find a player's team.
			foreach (var id in playerIds)
				playerTeams.Add(id, game.Teams.Find(t => t.Players.Exists(gp => gp.PlayerId == game.ServerGame.Players.Find(sp => sp.PandCPlayerId == id).PlayerId)));

			var currents = new Dictionary<GameTeam, KeyValuePair<DateTime, int>>();  // Dictionary of gameTeam -> <time, team score>.
			
			foreach (var gameTeam in game.Teams)
				currents.Add(gameTeam, new KeyValuePair<DateTime, int>(game.Time, 0));

			float yMin = height;
			float yMax = 0;

			foreach (var oneEvent in game.ServerGame.Events)
				if (oneEvent.Score != 0)
				{
					var serverPlayer = game.ServerGame.Players.Find(sp => sp.PandCPlayerId == oneEvent.PandCPlayerId);
					var gameTeam = game.Teams.Find(t => t.Players.Exists(gp => gp.PlayerId == serverPlayer.PlayerId));
					var pen = new Pen(gameTeam.Colour.ToSaturatedColor(), 3);
					var oldPoint = new PointF(Scale(currents[gameTeam].Key, duration, game.Time, game.ServerGame.EndTime), height - Scale(currents[gameTeam].Value, height, minScore, maxScore) + (float)(skew * currents[gameTeam].Key.Subtract(game.Time).TotalSeconds));
					currents[gameTeam] = new KeyValuePair<DateTime, int>(oneEvent.Time, currents[gameTeam].Value + oneEvent.Score);
					float y = height - Scale(currents[gameTeam].Value, height, minScore, maxScore) + (float)(skew * currents[gameTeam].Key.Subtract(game.Time).TotalSeconds);
					yMin = Math.Min(y, yMin);
					yMax = Math.Max(y, yMax);
					var newPoint = new PointF(Scale(currents[gameTeam].Key, duration, game.Time, game.ServerGame.EndTime), y);

					if (oneEvent.Event_Type == 30) // Base hit: show in the base's colour not the player's colour.
						pen.Color = ((Colour)(oneEvent.HitTeam + 1)).ToSaturatedColor();
					
					graphics.DrawLine(pen, oldPoint, newPoint);  // Show the hit.

					if (oneEvent.Event_Type == 31) // Base destroyed.
					{
						// Show the hit in a dashed line, half player colour, half base colour.
						var dashPen = new Pen(((Colour)(oneEvent.HitTeam + 1)).ToSaturatedColor(), 3);
						float baseHeight = Math.Abs(oldPoint.Y - newPoint.Y);
						dashPen.DashPattern = new float[] { baseHeight * 0.11F, baseHeight * 0.22F };
						dashPen.DashOffset = baseHeight * -0.11F;
						graphics.DrawLine(dashPen, oldPoint, newPoint);

						// Show the alias of the player who destroyed the base.
						var gamePlayer = game.Players.Find(gp => gp.PlayerId  == serverPlayer.PlayerId);
						var leaguePlayer = league.LeaguePlayer(gamePlayer);
						var brush = new SolidBrush(gameTeam.Colour.ToDarkColor());

						if (currents.Max(x => x.Value.Value) == currents[gameTeam].Value)
							graphics.DrawString(leaguePlayer.Name, font, brush, newPoint.X - graphics.MeasureString(leaguePlayer.Name, font).Width, newPoint.Y);
						else
							graphics.DrawString(leaguePlayer.Name, font, brush, newPoint.X, (oldPoint.Y + newPoint.Y) / 2);
					}
				}

			yMin = Math.Max(yMin - 1, 0);
			bitmap =  bitmap.Clone(new RectangleF(0, yMin, bitmap.Width, Math.Min(yMax + 1, bitmap.Height) - yMin), bitmap.PixelFormat);
			graphics = Graphics.FromImage(bitmap);
			graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

			// Write team names and scores in upper left corner.
			if (game.Teams.Count <= 10)
				for (int i = 0; i < game.Teams.Count; i++)
				{
					var leagueTeam = league.LeagueTeam(game.Teams[i]);
					var brush = new SolidBrush(game.Teams[i].Colour.ToDarkColor());
					graphics.DrawString(leagueTeam.Name + " " + game.Teams[i].Score.ToString("N0", CultureInfo.CurrentCulture), 
					                    font, brush, 30, graphics.MeasureString("0", font).Height * i);
				}

			return bitmap;
		}

		public static ZoomReport OnePlayer(League league, LeaguePlayer player, List<LeagueTeam> teams, GameHyper gameHyper)
		{
			ZoomReport report = new ZoomReport(string.IsNullOrEmpty(player.Name) ? "Player " + player.Id : player.Name,
			                                   "Time,Score,Tags +,Tags -,Tag Ratio,Score Ratio,TR\u00D7SR,Base Destroys,Denies,Denied,Yellow,Red",
			                                   "left,integer,integer,integer,float,float,float,integer,integer,integer,integer,integer,integer");

			report.Title += " \u2014 " + string.Join(", ", teams.Select(t => t.Name));
			if (teams.Count == 1)
				report.TitleHyper = "team" + teams[0].Id.ToString("D2", CultureInfo.InvariantCulture) + ".html";
			else
				report.Columns.Insert(1, new ZColumn("Team"));

			report.MaxChartByColumn = true;

			var totals = new GamePlayer();
			int totalScore = 0;
			int totalCount = 0;

			foreach (GamePlayer gamePlayer in player.Played)
			{
				// Add a row for each game the player played.
				ZRow row = new ZRow();
				Color color = (gamePlayer.Colour == Colour.None ? gamePlayer.Colour : gamePlayer.Colour).ToColor();

				Game game = league.Game(gamePlayer);
				if (game == null)
					row.Add(new ZCell("Game ??", color));
				else
				{
					var gameCell = new ZCell((game.Title + " " + game.Time.ToString()).Trim(), color);  // Game time
					gameCell.Hyper = gameHyper(game);
					row.Add(gameCell);
				}
				
				if (teams.Count > 1)
					row.Add(TeamCell(league.LeagueTeam(gamePlayer)));

				FillDetails(row, gamePlayer, color, game == null ? double.NaN : (double)game.TotalScore() / game.Players.Count);

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
				totalCount += game == null ? 1 : game.Players.Count;

				report.Rows.Add(row);
			}

			// Add an overall average row.
			ZRow totalRow = new ZRow();
			totalRow.Add(new ZCell("Average"));  // Game time
			if (teams.Count > 1)
				totalRow.Add(new ZCell(""));  // Team name

			if (player.Played.Count == 0)
				totals.Score = 0;
			else
				totals.Score /= player.Played.Count;

			FillDetails(totalRow, totals, default(Color), (double)totalScore / totalCount);

			report.Rows.Add(totalRow);

			report.RemoveZeroColumns();
			return report;
		}  // OnePlayer

		/// <summary> List a team and its players' performance in each game.  One player per column; one game per row.</summary>
		public static ZoomReport OneTeam(League league, bool includeSecret, LeagueTeam team, DateTime from, DateTime to, bool description, GameHyper gameHyper)
		{
			ZoomReport report = new ZoomReport(team.Name);

			report.Columns.Add(new ZColumn("Game", ZAlignment.Left));

			var averages = new Dictionary<LeaguePlayer, double>();
			foreach (LeaguePlayer leaguePlayer in team.Players)
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
				    throw(new Exception("Player " + leaguePlayer.Name + " already in averages."));
				averages.Add(leaguePlayer, count == 0 ? 0 : score / count);
			}
			
			List<KeyValuePair<LeaguePlayer, double>> averagesList = averages.ToList();
			averagesList.Sort(delegate(KeyValuePair<LeaguePlayer, double> x, KeyValuePair<LeaguePlayer, double> y)
			                 {
			                 	return Math.Sign(y.Value - x.Value);
			                 }
			                );
			List<LeaguePlayer> players = new List<LeaguePlayer>();
			foreach (var x in averagesList)
				players.Add(x.Key);
			
			// Add a column for each player on this team.
			foreach (LeaguePlayer leaguePlayer in players)
			{
				report.Columns.Add(new ZColumn(leaguePlayer.Name, ZAlignment.Integer, "Players"));
				report.Columns.Last().Hyper = "players.html#player" + leaguePlayer.Id;
			}
			report.Columns.Add(new ZColumn("Total", ZAlignment.Integer, "Score"));
			if (league.IsPoints())
				report.Columns.Add(new ZColumn("Pts", ZAlignment.Float, "Score"));

			report.AddColumns("Score again,Tags +,Tags -,Tag Ratio,Score Ratio,TR\u00D7SR,Destroys,Conceded,Ratio,Denies,Denied,Yellow,Red", 
			                  "integer,integer,integer,float,float,float,integer,integer,integer,integer,integer,integer",
			                  ",Tags,Tags,Ratios,Ratios,Ratios,Base,Base,Base,Base,Base,Penalties,Penalties");

			// TODO: add columns for total points for/against, total team tags for/against, total team denials for/against (from game.ServerGame.Events?)

			DateTime previousGameDate = DateTime.MinValue;
			// Add a row for each of this team's games. Fill in values for team score and player scores.
			foreach (GameTeam gameTeam in league.Played(team, includeSecret))
				if (from < league.Game(gameTeam).Time && league.Game(gameTeam).Time < to)
				{
					var game = league.Game(gameTeam); 
					if (game.Time.Date > previousGameDate)  // We've crossed a date boundary, so
					{
						ZRow dateRow = new ZRow();  // create a row
						report.Rows.Add(dateRow);
						dateRow.Add(new ZCell(game.Time.ToShortDateString()));  // to show the new date.
					}

					ZRow gameRow = new ZRow();
					report.Rows.Add(gameRow);
					var timeCell = new ZCell((game.Title + " " + game.Time.ToShortTimeString()).Trim());
					timeCell.Hyper = gameHyper(game);
					gameRow.Add(timeCell);
					
					// Add player scores to player columns.
					foreach (LeaguePlayer leaguePlayer in players)
					{
						GamePlayer gamePlayer = gameTeam.Players.Find(x => league.LeaguePlayer(x).Id == leaguePlayer.Id);
						if (gamePlayer == null)
							gameRow.Add(new ZCell(""));
						else
							gameRow.Add(new ZCell(gamePlayer.Score, ChartType.Bar, "N0", gamePlayer.Colour.ToColor()));
					}

					gameRow.Add(new ZCell(gameTeam.Score, ChartType.Bar, "N0", gameTeam.Colour.ToColor()));

					if (league.IsPoints())
					{
						if (game.IsPoints())
							gameRow.Add(new ZCell(gameTeam.Points, ChartType.None, "N0", gameTeam.Colour.ToColor()));
						else
							gameRow.Add(new ZCell(""));
					}

					var teamTotal = new GamePlayer();
					foreach (GamePlayer gamePlayer in gameTeam.Players)
						teamTotal.Add(gamePlayer);
					FillDetails(gameRow, teamTotal, Color.Empty, game.TotalScore() / game.Teams.Count);

					// Base Ratio: bases destroyed / bases conceded
					ZCell basesConceded;
					ZCell baseRatio;
					if (game.ServerGame.Events == null)
					{
						basesConceded = new ZCell("");
						baseRatio = new ZCell("");
					}
					else
					{
						var basesTotal = game.Teams.Sum(t => t.Players.Sum(p => p.BaseDestroys));
						var thisDestroyed = game.ServerGame.Events.Count(e => e.Event_Type == 31 && e.HitTeam == (int)gameTeam.Colour - 1);
						var basesThisTeam = gameTeam.Players.Sum(p => p.BaseDestroys);
						basesConceded = new ZCell(thisDestroyed, ChartType.Bar, "N0");
						if (thisDestroyed == 0)
							baseRatio = new ZCell("\u221E");  // infinity
						else
							baseRatio = new ZCell(1.0 * basesThisTeam / thisDestroyed, ChartType.Bar, "P0");
					}
					gameRow.Insert(gameRow.Count - 4, basesConceded);
					gameRow.Insert(gameRow.Count - 4, baseRatio);

					previousGameDate = game.Time.Date;
				}  // if from..to; for Played

			ZRow averageRow = new ZRow();
			averageRow.Add(new ZCell("Average"));
			FillAverages(report, averageRow);
			report.Rows.Add(averageRow);

			report.MaxChartByColumn = true;
//			TODO: replace the above line with report.OnCalcBar = KombiReportCalcBar; TeamLadderCalcBar;

			if (description)
				report.Description = "This report shows the team " + team.Name +" and its players.  Each row is one game.";

			report.RemoveColumn(report.Columns.IndexOf(report.Columns.Find(c => c.Text == "Score again")));
			report.RemoveZeroColumns();
			return report;
		}  // OneTeam

		public static ZoomReport FixtureList(Fixture fixture, League league, int? teamId = null)
		{
			FixtureTeam team = teamId == null || teamId == -1 ? null : fixture.Teams.Find(x => x.Id == teamId);
			string title = team == null ? "Fixtures for " + league.Title : "Fixtures for " + team.Name + " in " + league.Title;

			ZoomReport report = new ZoomReport(title, "Time", "left");

			int maxTeams = fixture.Games.Count == 0 ? 0 : fixture.Games.Max(x => x.Teams.Count());
			
			for (int i = 0; i < maxTeams; i++)
				report.Columns.Add(new ZColumn("Team", ZAlignment.Left));

			foreach (var fg in fixture.Games)
				if (team == null || fg.Teams.Keys.Contains(team))
				{
					ZRow row = new ZRow();

					row.Add(new ZCell(fg.Time.ToString("yyyy/MM/dd HH:mm")));

					for (var i = Colour.Red; i <= Colour.White; i++)
					{
						FixtureTeam ft = fg.Teams.FirstOrDefault(x => x.Value == i).Key;
						if (ft != null)
						{
							ZCell teamCell = new ZCell(ft.Name, i.ToColor());
			        		teamCell.Hyper = "fixture" + ft.Id.ToString("D2", CultureInfo.InvariantCulture) + ".html";
							row.Add(teamCell);
							
						}
					}

					report.Rows.Add(row);
				}

			if (team != null && team.LeagueTeam != null)
				report.Description = "This is a list of fixtures for " + team.Name + ". Their results are <a href=\"team" + team.LeagueTeam.Id.ToString("D2", CultureInfo.InvariantCulture) + ".html\">here</a>."; 

			return report;
		}

		public static ZoomReport FixtureGrid(Fixture fixture, League league, int? teamId = null)
		{
			FixtureTeam team = teamId == null || teamId == -1 ? null : fixture.Teams.Find(x => x.Id == teamId);
			string title = team == null ? "Fixtures for " + league.Title : "Fixtures for " + team.Name + " in " + league.Title;

			ZoomReport report = new ZoomReport(title, "Team", "left");

			foreach (var ft in fixture.Teams)
			{
				var row = new ZRow();
				var teamCell = new ZCell(ft.Name);
				teamCell.Hyper = "fixture" + ft.Id.ToString("D2", CultureInfo.InvariantCulture) + ".html";
				row.Add(teamCell);
				report.Rows.Add(row);
			}

			foreach (var fg in fixture.Games)
			{
				report.Columns.Add(new ZColumn(fg.Time.ToShortTimeString(), ZAlignment.Center));
				for (int i = 0; i < fixture.Teams.Count; i++)
				{
					var found = fg.Teams.ContainsKey(fixture.Teams[i]);
					if (found)
						report.Rows[i].Add(new ZCell(fg.Teams[fixture.Teams[i]].ToString()[0].ToString(), fg.Teams[fixture.Teams[i]].ToColor()));
					else if (teamId != null && teamId > -1 && (i == (int)teamId - 1 || fg.Teams.ContainsKey(fixture.Teams[(int)teamId - 1])))
						report.Rows[i].Add(new ZCell("", Color.Gainsboro));
					else
						report.Rows[i].Add(new ZCell(""));
				}
			}

			return report;
		}

		// false discovery rate. multivariate repeated measures. logit = p/(1-p). poisson model. 
		// do some histograms of: #hits by each pack, #hits on each pack, #hitsby minus #hitson, ratios of each pack, logits of each pack, and see what's normally distributed.
		// ch 7, ch 11.5 bootstrapping, p 1034 questions of interest, ch27
		// overlapping 95% confidence intervals. If top pack's confidence interval overlaps bottom pack's interval, there are no outliers.
		// ellipse containing 95% of values on a scatter plot. Draw 45 ellipses; see if they overlap.
		/// <summary>Show overall stats for each pack, including a t test comparing it to all other packs combined.</summary>
		public static ZoomReport PackReport(List<League> leagues, List<Game> soloGames, string title, DateTime? from, DateTime? to, ChartType chartType, bool description)
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
					if (player != null && player.Played.Count > 0)
					{
						hitsBy += player.Played.Sum(x => x.HitsBy);
						hitsOn += player.Played.Sum(x => x.HitsOn);
						scoreSum += player.Played.Sum(x => x.Score);
						scoreSum += player.Played.Average(x => x.Score);
						gameAverageSum += player.Played.Average(x => league.Game(x) == null ? 0 : league.Game(x).TotalScore() / league.Game(x).Players.Count);
					}
				}
				soloLadder.Add(solo, hitsBy == 0 && hitsOn == 0 ? 
				               (gameAverageSum == 0 ? 1000000 : scoreSum / gameAverageSum) :  // When there's no hit data, fall back to score data.
				               hitsOn == 0 ? 1000000 : (double)hitsBy / hitsOn);
			}

			// Build list of games to report on.
			var games = new List<Game>();
			foreach (var league in leagues)
				games.AddRange(league.AllGames.Where(g => g.Time > (from ?? DateTime.MinValue) && g.Time < (to ?? DateTime.MaxValue)));

			// Now build the pack report.
			var	report = new ZoomReport((string.IsNullOrEmpty(title) ? (leagues.Count == 1 ? leagues[0].Title + " " : "") + "Pack Report" : title) + FromTo(games, from, to),
				                        "Rank,Pack,Score Ratio,t,p,Count,Tag Ratio,t,p,Tag diff,t,p,Count,",
				                        "center,left,integer,float,float,integer,float,float,float,float,float,float,integer");
			report.MaxChartByColumn = true;

			var packs = games.SelectMany(game => game.Players.Select(player => player.Pack)).Distinct().ToList();

			foreach (string pack in packs)
			{
				var row = new ZRow();
				report.Rows.Add(row);
				
				row.Add(new ZCell(0, ChartType.None));  // put in a temporary Rank
				row.Add(new ZCell(pack));  // set up pack name

				// Sample 1 is "this pack".
				var scoreRatios = new List<double>();
				var tagRatios = new List<double>();
				var tagDifferences = new List<double>();

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

				foreach (var game in games.Where(g => g.Players.Exists(p => p.Pack == pack)))
				{
					double gameTotalScore = 0;
					double gameTotalScale = 0;
					foreach (var player in game.Players)
					{
						double scale = player.PlayerId != null && soloLadder.ContainsKey(player.PlayerId) ? soloLadder[player.PlayerId] : 1;
						gameTotalScore += 1.0 * player.Score / scale;
						if (player.HitsOn != 0)
							gameTotalScale += (double)player.HitsBy / player.HitsOn / scale;
					}
					
					foreach (var player in game.Players)
					{
						double scale = player.PlayerId != null && soloLadder.ContainsKey(player.PlayerId) ? soloLadder[player.PlayerId] : 1;
						double scoreRatio = 1.0 * player.Score / (gameTotalScore - player.Score / scale) * (game.Players.Count - 1) / scale;  // Scale this score ratio by this player's scale value, and by the average scaled scores of everyone else in the game.
						double? tagRatio = (player.HitsOn != 0 ? ((double)player.HitsBy / player.HitsOn / scale) : (double?)null);

						if (player.Pack == pack)
						{
							scoreRatios.Add(scoreRatio);
							if (tagRatio != null)
								tagRatios.Add((double)tagRatio);
							if (player.HitsBy != 0 || player.HitsOn != 0)
								tagDifferences.Add(player.HitsBy / scale - player.HitsOn);
						}
						else
						{
							n2++;
							scoreRatio2Sum += scoreRatio;
							scoreRatio2SquaredSum += scoreRatio * scoreRatio;
							if (tagRatio != null)
							{
								tagRatio2Sum += (double)tagRatio;
								tagRatio2SquaredSum += (double)(tagRatio * tagRatio);
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

				row.Add(new ZCell(scoreRatios.Average(), chartType, "P0"));  // Average score ratio.
				row.Last().Data.AddRange(scoreRatios);

				double t = tStatistic(scoreRatios.Count, scoreRatios.Sum(), scoreRatios.Sum(x => Math.Pow(x, 2)), n2, scoreRatio2Sum, scoreRatio2SquaredSum);
				double pValue = 1 - Erf(Math.Abs(t) / Math.Sqrt(2));

				if (scoreRatios.Count * 0.9 > tagRatios.Count) // Less than 90% of the results for this pack have tag ratios, so show score ratio statistics.
				{
					row.Add(new ZCell(t, ChartType.None, "F2"));  // t statistic
					row.Add(new ZCell(pValue, ChartType.None, "G2"));  // p value

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

				row.Add(new ZCell(scoreRatios.Count, ChartType.None, "F0"));  // Total games played.
				
				if (tagRatios.Count == 0)
				{
					row.Add(new ZCell("-"));  // Average tag ratio.
					row.Add(new ZCell("-"));  // t statistic
					row.Add(new ZCell("-"));  // p value

					row.Add(new ZCell("-"));  // Average tag difference.
					row.Add(new ZCell("-"));  // t statistic
					row.Add(new ZCell("-"));  // p value
				}
				else
				{
					row.Add(new ZCell(tagRatios.Average(), chartType, "P0"));  // Average tag ratio.
					row.Last().Data.AddRange(tagRatios);

					t = tStatistic(tagRatios.Count, tagRatios.Sum(), tagRatios.Sum(x => Math.Pow(x, 2)), tagRatio2Count, tagRatio2Sum, tagRatio2SquaredSum);
					pValue = 1 - Erf(Math.Abs(t) / Math.Sqrt(2));

					row.Add(new ZCell(t, ChartType.None, "F2"));  // t statistic
					row.Add(new ZCell(pValue, ChartType.None, "G2"));  // p value

					if (Math.Abs(pValue) < 0.05 / packs.Count)
						row.Last().Color = Color.FromArgb(0xFF, 0xC0, 0xC0);
					else if (Math.Abs(pValue) < 0.05)
						row.Last().Color = Color.FromArgb(0xFF, 0xF0, 0xF0);

					row.Add(new ZCell(tagDifferences.Average(), chartType, "F2"));  // Average tag difference.
					row.Last().Data.AddRange(tagDifferences);

					t = tStatistic(tagDifferences.Count, tagDifferences.Sum(), tagDifferences.Sum(x => Math.Pow(x, 2)), tagDifference2Count, tagDifference2Sum, tagDifference2SquaredSum);
					pValue = 1 - Erf(Math.Abs(t) / Math.Sqrt(2));

					row.Add(new ZCell(t, ChartType.None, "F2"));  // t statistic
					row.Add(new ZCell(pValue, ChartType.None, "G2"));  // p value

					if (Math.Abs(pValue) < 0.05 / packs.Count)
						row.Last().Color = Color.FromArgb(0xFF, 0xC0, 0xC0);
					else if (Math.Abs(pValue) < 0.05)
						row.Last().Color = Color.FromArgb(0xFF, 0xF0, 0xF0);
				}

				row.Add(new ZCell(tagRatios.Count, ChartType.None, "F0"));  // Total games used for tag ratio statistics.
			}  // foreach pack

			report.Rows = report.Rows.OrderByDescending(x => x[7].Number).ThenByDescending(x => x[3].Number).ToList();

			// Assign ranks.
			for (int i = 0; i < report.Rows.Count; i++)
				report.Rows[i][0].Number = i + 1;
			
			var averages = new ZRow();
			
			averages.Add(new ZCell("", Color.Gray));
			averages.Add(new ZCell("Averages", Color.Gray));
			if (report.Rows.Count > 0)
				for (int i = 2; i < report.Rows[0].Count; i++)
					if (i == 2 || i == 5 || i == 6 || i == 9 || i == 12)
						averages.Add(new ZCell(report.Rows.Average(row => row[i].Number), 
						                       ChartType.None, report.Rows[0][i].NumberFormat, Color.Gray));
					else
						averages.Add(new ZCell("", Color.Gray));

			report.Rows.Add(averages);

			if (report.Rows.Sum(row => row[12].Number) == 0)  // No tag ratios for any pack.
			{
				for (int i = 12; i >= 6; i--)
					report.RemoveColumn(i);
				foreach (var row in report.Rows)
					foreach (var cell in row)
						cell.ChartCell = report.Cell(row, "Score Ratio");
			}
			
			if (description)
			{
				report.Description = "This report shows the performance of each pack, each time it is used by a logged-on player. " +
				    "Each score the pack gains is scaled by the player's ratio, effectively 'handicapping'. " +
				    "You should ignore results from a pack where most of its games are played by a particular player. " +
				    "You should ignore results from a pack with less than 10 or so games. <br/>" +
					"'t' is the result of Student's t test -- the further the number is from 0, the more this pack's average deviates from the average of all the rest of the packs. " +
					"'p' is the likelihood of the t test result occurring by chance." +
					"You should pay attention to any pack with a p value smaller than " + (0.05 / packs.Count).ToString("G2", CultureInfo.CurrentCulture) +
					" -- these results are statistically significant, meaning that there is less than a 5% chance that such results occurred by chance. " +
				    "Once you know which packs are at the ends of the curve, you should remove any unusually good or bad packs. " +
				    "In a team event, you can try to balance the colours, by removing the best pack from this colour, the worst from that colour, etc.";
			
//				if (something)
//			        report.Description += " Note that on Nexus or Helios, in .Torn files created by Torn 4, only games committed with \"Calculate scores by Torn\" selected in Preferences will show tag ratios.";
			
				if (from != null || to != null)
			        report.Description += " The report has been limited to games" + FromTo(games, from, to) + ".";
			}

			return report;
		}  // PackReport

		/// <summary>Every player in every game.</summary>
		public static ZoomReport EverythingReport(League league, string title, DateTime? from, DateTime? to, bool description)
		{
			var	report = new ZoomReport(string.IsNullOrEmpty(title) ? league.Title + " " + "Everything Report" : title + FromTo(league.AllGames, from, to),
				                        "Player,Pack,Team,Rank,Score,Hits by,Hits On",
				                        "left,left,left,integer,integer,integer,integer");
			report.MaxChartByColumn = true;

			foreach (var game in league.AllGames.FindAll(x => x.Time.CompareTo(from ?? DateTime.MinValue) >= 0 &&
			                                             x.Time.CompareTo(to ?? DateTime.MaxValue) <= 0))
			{
				var gameRow = new ZRow();
				gameRow.Add(new ZCell(""));
				gameRow.Add(new ZCell(game.Title));
				gameRow.Add(new ZCell(game.Time.ToString()));
				gameRow.Add(new ZCell(""));
				gameRow.Add(new ZCell(game.TotalScore()));

				report.Rows.Add(gameRow);

				foreach (var player in game.Players)
				{
					var row = new ZRow();
				
					row.Add(new ZCell(league.LeaguePlayer(player) == null ? player.PlayerId : league.LeaguePlayer(player).Name));
					row.Add(new ZCell(player.Pack));
					row.Add(new ZCell(league.GameTeam(player) == null ? " " : league.LeagueTeam(league.GameTeam(player)).Name));
					row.Add(new ZCell(player.Rank));
					row.Add(new ZCell(player.Score));
					row.Add(new ZCell(player.HitsBy));
					row.Add(new ZCell(player.HitsOn));

					report.Rows.Add(row);
				}
			}

			return report;
		}  // EverythingReport

		/// <summary>If i is 0, return a cell which has "" as its text (but still 0 as its number). Otherwise return a cell with this number.</summary>
		static ZCell BlankZero(int i, ChartType chartType, Color color)
		{
			if (i == 0)
			{
				var cell = new ZCell("", color);
				cell.Number = 0;
				return cell;
			}
			else
				return new ZCell(i, chartType, null, color);
		}

		static ZCell DataCell(List<double> dataList, Drops drops, ChartType chartType, string numberFormat)
		{
			var dataCell = new ZCell(0, chartType, numberFormat);
			dataCell.Data.AddRange(dataList);
			DropScores(dataList, drops);
			dataCell.Number = dataList.Where(x => !double.IsNaN(x)).DefaultIfEmpty(0).Average();

			return dataCell;
		}

		static ZCell TeamCell(LeagueTeam leagueTeam, Color color = default(Color))
		{
			if (leagueTeam == null)
				return new ZCell("", color);

			ZCell teamcell = new ZCell(leagueTeam.Name, color);
			teamcell.Hyper = "team" + leagueTeam.Id.ToString("D2", CultureInfo.InvariantCulture) + ".html";
			return teamcell;
		}

		static double Erf(double x)
		{
			// Abramowitz, M. and Stegun, I. (1964). _Handbook of Mathematical Functions with Formulas, Graphs, and Mathematical Tables_. p.299.
			// http://people.math.sfu.ca/~cbm/aands/page_299.htm
			// erf(x) = 1 - 1 / (a1*x + a2*x^2 + a3*x^3 + a4*x^4 + a5*x^5 + a6*x^6) + epsilon(x)
			// a1 = 0.0705230784; a2 = 0.0422820123; a3 = 0.0092705272; a4 = 0.0001520143; a5 = 0.0002765672; a6 = 0.0000430638
			// epsilon(x) < 3e-7
			double a1 = 0.0705230784;
			double a2 = 0.0422820123;
			double a3 = 0.0092705272;
			double a4 = 0.0001520143;
			double a5 = 0.0002765672;
			double a6 = 0.0000430638;
			return 1 - Math.Pow(1 + a1 * x + a2 * Math.Pow(x, 2) + a3 * Math.Pow(x, 3) + a4 * Math.Pow(x, 4) + a5 * Math.Pow(x, 5) + a6 * Math.Pow(x, 6), -16);
		}

		// Used for t-test. https://en.wikipedia.org/wiki/T-statistic
		static double tStatistic(double n1, double sum1, double squaredSum1, double n2, double sum2, double squaredSum2)
		{
			double v1 = (squaredSum1 - (sum1 * sum1 / n1)) / (n1 - 1);  // Variance.
			double v2 = (squaredSum2 - (sum2 * sum2 / n2)) / (n2 - 1);
			double sp = Math.Sqrt(((n1 - 1) * v1 + (n2 - 1) * v2) / (n1 + n2 - 2));  //  sp is the pooled standard deviation.
			return (sum1 / n1 - sum2 / n2) / (sp * Math.Sqrt(1.0/n1 + 1.0/n2));  // Student's t test statistic.
		}

		static DateTime LastGameDate(List<Game> games, DateTime? to)
		{
			if (to == null)
				return games.Count == 0 ? DateTime.MinValue : games.Last().Time;

			for (int i = games.Count - 1; i >= 0; i--)
				if (games[i].Time <= to)
					return games[i].Time;

			return DateTime.MinValue;
		}

		static string FromTo(List<Game> games, DateTime? from, DateTime? to)
		{
			if (games.Count == 0)
				return " from " + (from == null ? "(none)" : ((DateTime)from).ToShortDateString()) +
				       " to " + (to == null ? "(none)" : ((DateTime)to).ToShortDateString());

			if (games.First().Time.Date == games.Last().Time.Date)  // this report is for games on a single day
				return " for " + games.First().Time.Date.ToShortDateString();

			return " from " + games.First().Time.ToShortDateString() + " to " + games.Last().Time.ToShortDateString();  // this report is for games over multiple days
		}

		static List<T> DropScores<T>(List<T> scores, Drops drops)
		{
			if (drops != null)
			{
				scores.Sort();

				int count = scores.Count();
				int dropBest = drops.DropBest(count);
				int dropWorst = drops.DropWorst(count);

				if (drops.CountAfterDrops(count) < 1 && count > 0)
				{
					T closest = scores[(int)Math.Round(1.0 * dropWorst / (dropBest + dropWorst) * (count - 1))];
					scores.Clear();
					scores.Add(closest);
					return scores;
				}

				if (dropBest > 0)
					scores.RemoveRange(count - dropBest, dropBest);

				if (dropWorst > 0)
					scores.RemoveRange(0, dropWorst);
			}
			return scores;
		}

		static void AddAverageAndDrops(League league, ZRow row, Drops drops, List<double> scoresList, List<double> pointsList)
		{
			int count = scoresList.Count;
			
			DropScores(scoresList, drops);
			DropScores(pointsList, drops);
			row.Add(new ZCell(scoresList.Average(), ChartType.Bar, "N0"));  // average game score
			if (league.IsPoints())
				row.Add(new ZCell(pointsList.Average(), ChartType.None, "N0"));  // average points

			if (scoresList.Count < count)
				row.Add(new ZCell(count - scoresList.Count, ChartType.None, "N0"));  // games dropped
			else
				row.Add(new ZCell(""));  // games dropped
		}
		
		static void SortGridReport(League league, ZoomReport report, ReportType reportType, List<Game> games, int averageCol, int pointsCol, bool reversed)
		{
			if (reportType == ReportType.GameGrid) {
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
			                 	return Math.Sign(result ?? 0);
			                 }
			                );
			}
			else if (reportType == ReportType.Ascension)
			{
				report.Columns.Add(new ZColumn("Last game index"));
				foreach (ZRow row in report.Rows)
				{
					for (int col = Math.Min(averageCol, row.Count()) - 1; col >= 0; col--)
						if (col >= 0 && row[col].Number != null)
						{
							row.Add(new ZCell(col, ChartType.None, "N0"));
							break;
						}
				}

				report.Rows.Sort(delegate(ZRow x, ZRow y)
			                 {
			                 	int result = 0;
		                 		result = Math.Sign((double)y[y.Count - 1].Number - (double)x[x.Count - 1].Number);
			                 	if (result == 0)
			                 		result = Math.Sign((double)y[(int)y[y.Count - 1].Number].Number - (double)x[(int)x[x.Count - 1].Number].Number);
			                 	return result;
			                 }
			                );

				report.RemoveColumn(report.Columns.Count - 1);  // Last game index
				report.RemoveColumn(report.Columns.Count - 1);  // Average
				if (league.IsPoints(games))
					report.RemoveColumn(report.Columns.Count - 1);  // Pts
			} 
			else if (reportType == ReportType.Pyramid)
			{
				PyramidComparer pc = new PyramidComparer();
				pc.Columns = report.Columns;
				pc.Reversed = reversed;
				report.Rows.Sort(pc);
				pc.DoColor(league, report);

				report.RemoveColumn(report.Columns.Count - 1);
				if (league.IsPoints(games))
					report.RemoveColumn(report.Columns.Count - 1);
			}
			
			for (int i = 0; i < report.Rows.Count; i++)
				report.Rows[i][0].Number = i + 1;

			report.Title = league.Title + (reportType == ReportType.Ascension ? " Ascension" : reportType == ReportType.Pyramid ? " Pyramid" : " Games");  // TODO: respect report title here.
		}

		static void FinishReport(League league, ZoomReport report, List<Game> games, ReportTemplate rt)
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
				else if (rt.From != null)
					report.Description += " The report has been limited to games after " + games.First().Time.ToShortDateString() + ".";
				else if (rt.To != null)
					report.Description += " The report has been limited to games before " + games.Last().Time.ToShortDateString() + ".";
			}
		}
	}

	class PyramidComparer: IComparer<ZRow>
	{
		public List<ZColumn> Columns { get; set; }
		/// <summary>Set this to true if the last column is rank -- larger ranks should be sorted lower.</summary>
		public bool Reversed { get; set; }

		int LastGame(ZRow row)
		{
			int col = row.Count - 1;

			while (col > 0 && (col >= Columns.Count || Columns[col].Text == "Average" || Columns[col].Text == "Pts"))
				col--;

			while (col > 0 && row[col].Number == null)
				col--;

			return col;
		}

		public int Compare(ZRow x, ZRow y)
		{
			int lastx = LastGame(x);
			int lasty = LastGame(y);

			if (Columns[lastx].GroupHeading == Columns[lasty].GroupHeading)  // These teams' last games are in the same round.
			{
				if (y[lasty].Number != x[lastx].Number)
					return (Reversed ? -1 : 1) * Math.Sign((double)(y[lasty].Number - x[lastx].Number));  // Compare their last scores.
				else if (y[lasty - 1].Number != null && x[lastx - 1].Number != null)
					return Math.Sign((double)(y[lasty - 1].Number - x[lastx - 1].Number));
				else
					return 0;
			}
			else
				return lasty - lastx;  // Whoever survived longest ranks highest.
		}

		// Mark alternate rounds in slightly different background colours.
		public void DoColor(League league, ZoomReport report)
		{
//			int step = league.IsPoints() ? 2 : 1;

			foreach (ZRow row in report.Rows)
			{
				int last = LastGame(row);

				while (Columns[last].GroupHeading == Columns[last + 1].GroupHeading && last < row.Count - 1)
					last++;  // Move last out to the end of this round (or repechage).

				bool even = false;
				int col = 2;

				while (col <= last)
				{
					if (even && row[col].Color == Color.Empty)
					{
						row[col].Color = Color.FromArgb(0xF0, 0xF0, 0xFF);
//						if (league.IsPoints())
//							row[col + 1].Color = Color.FromArgb(0xF0, 0xF0, 0xFF);
					}

					if (Columns[col].GroupHeading != Columns[col + 1].GroupHeading)
					{
						even = !even;
						col ++;//= step;  // Move col out to the end of this round (or repechage).
					}
					else
				    	col++;
				}
			}
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
