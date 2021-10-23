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
			bool ratio = rt.Setting("OrderBy").Contains("score ratio");
			bool scaled = rt.Setting("OrderBy").StartsWith("scaled");
		    bool showColours = rt.Settings.Contains("ShowColours");

			ZoomReport report = new ZoomReport(string.IsNullOrEmpty(rt.Title) ? league.Title + " Team Ladder" : rt.Title, "Rank,Team", "center,left");
			report.MaxChartByColumn = true;

			List<Game> games = league.Games(includeSecret);

			var coloursUsed = games.SelectMany(g => g.Teams.Select(t => t.Colour)).Distinct();
			var colourTotals = new Dictionary<Colour, List<int>>();

			if (showColours)
				foreach (Colour c in coloursUsed)
				{
					report.AddColumn(new ZColumn("1st", ZAlignment.Integer, c.ToString()));
					report.AddColumn(new ZColumn("2nd", ZAlignment.Integer, c.ToString()));
					report.AddColumn(new ZColumn("3rd", ZAlignment.Integer, c.ToString()));

					colourTotals.Add(c, new List<int>());
				}

			if (league.IsPoints())
				report.AddColumn(new ZColumn("Points", ZAlignment.Float));

			report.AddColumn(new ZColumn(ratio ? "Score Ratio" : "Average score", ZAlignment.Float));
			report.AddColumn(new ZColumn("Games", ZAlignment.Integer));

			if (rt.Drops != null && rt.Drops.HasDrops())
				report.AddColumn(new ZColumn("Dropped", ZAlignment.Integer));

			ZCell barCell = null;
			double scoreTotal = 0;
			double pointsTotal = 0.0;

			List<int> countList = new List<int>();  // Number of games each team has played, for scaling.

			foreach (LeagueTeam team in league.Teams)  // Create a row for each League team.
			{
				ZRow row = new ZRow();

				row.Add(new ZCell(0));  // put in a temporary Rank

				row.Add(TeamCell(team));

				double points = 0;

				var colourCounts = new Dictionary<Colour, List<int>>();
				foreach (Colour c in coloursUsed)
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
								scoreList.Add(gameTeam.Score / league.Game(gameTeam).TotalScore() * league.Game(gameTeam).Teams.Count);
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
					foreach (Colour c in coloursUsed)
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
					for (int i = 2; i < coloursUsed.Count() * 3 + 1; i++)
						row[i].ChartCell = row[i];
					for (int i = coloursUsed.Count() * 3 + 2; i < row.Count; i++)
						row[i].ChartCell = barCell;
				}
				else
					foreach (ZCell cell in  row)
						cell.ChartCell = barCell;

				if (count > 0)
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
							row.Add(new ZCell(""));  // scaled points
					}
				}

//				report.OnCalcBar = TeamLadderScaledCalcBar;
			}  // if ScaleGames

			int scoreColumn = report.Columns.FindIndex(x => x.Text.Contains("core"));
			int pointsColumn = report.Columns.FindIndex(x => x.Text == "Points");
			int scaledColumn = report.Columns.FindIndex(x => x.Text.Contains("caled"));

			report.Rows.Sort(delegate(ZRow x, ZRow y)
			                 {
								double? result = 0;
								if (pointsColumn != -1)
								{
									double? xPoints = scaledColumn == -1 || x[scaledColumn].Number == null ? x[pointsColumn].Number : x[scaledColumn].Number;
									double? yPoints = scaledColumn == -1 || y[scaledColumn].Number == null ? y[pointsColumn].Number : y[scaledColumn].Number;

									if (xPoints == null)
										return 1;
									if (yPoints == null)
										return -1;

									result = yPoints - xPoints;
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

				foreach (Colour c in coloursUsed)
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
			}

			for (int i = 0; i < report.Rows.Count; i++)
				report.Rows[i][0].Number = i + 1;

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
		}  // TeamLadder

		/// <summary>Build a square table showing how many times each team has played (and beaten) each other team.</summary>
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
				column.Hyper = "team" + team1.TeamId.ToString("D2", CultureInfo.InvariantCulture) + ".html";
				column.Rotate = true;
				report.AddColumn(column);

				var row = new ZRow();
				report.Rows.Add(row);

				row.Add(TeamCell(team1));

				foreach (var team2 in teams)
				{
					var cellGames = games.FindAll(x => x.Teams.Any(y => league.LeagueTeam(y) == team1) &&
					                                   x.Teams.Any(z => league.LeagueTeam(z) == team2));  // Get games that include these two teams.
					ZCell cell;
					if (team1 == team2)
					{
						cell = new ZCell("\u2572", Color.Gray);
						cell.Number = 1;  // Ensure that ZoomReport.Widths will set sensible widths for all columns.
						cell.NumberFormat = "F1";  // Ensure that ZoomReport.Widths will set sensible widths for all columns.
					}
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
				report.Description = "This report shows how may times each team has played and beaten each other team, " + FromTo(games, rt.From, rt.To) + "."; 
			return report;
		}  // TeamsVsTeams

		public static ZoomReport ColourReport(League league, bool includeSecret, ReportTemplate rt)
		{
			ChartType chartType = ChartTypeExtensions.ToChartType(rt.Setting("ChartType"));

			ZoomReport report = new ZoomReport(string.IsNullOrEmpty(rt.Title) ? league.Title + " Colour Performance" : rt.Title, "Rank", "center");

			List<Game> games = league.Games(includeSecret);

			var coloursUsed = games.SelectMany(g => g.Teams.Select(t => t.Colour)).Distinct();
			var colourTotals = new Dictionary<Colour, List<int>>();

			foreach (Colour c in coloursUsed)
			{
				report.AddColumn(new ZColumn(c.ToString(), ZAlignment.Integer));

				colourTotals.Add(c, new List<int>());
			}

			foreach (Game game in games)
				foreach (GameTeam gameTeam in game.Teams)  // Roll through this team's games.
					if ((rt.From == null || game.Time >= rt.From) && (rt.To == null || game.Time <= rt.To))
					{
						// Add the team's rank in this game to the colourTotals.
						int rank = league.Game(gameTeam).Teams.IndexOf(gameTeam);

						while (colourTotals[gameTeam.Colour].Count <= rank)
							colourTotals[gameTeam.Colour].Add(0);
						colourTotals[gameTeam.Colour][rank]++;
					}
			
			int maxRank = games.Max(g => g.Teams.Count);

 			for (int rank = 0; rank < maxRank; rank++)
 			{
				ZRow row = new ZRow();
				report.Rows.Add(row);
				switch (rank) 
				{
					case 0:  row.Add(new ZCell("1st"));  break;
					case 1:  row.Add(new ZCell("2nd"));  break;
					case 2:  row.Add(new ZCell("3rd"));  break;
					default: row.Add(new ZCell((rank + 1).ToString() + "th"));  break;
				}

				foreach (Colour c in coloursUsed)
					if (colourTotals[c].Count > rank)
						row.Add(BlankZero(colourTotals[c][rank], ChartType.Bar, c.ToColor()));
					else
						row.Add(new ZCell("", c.ToColor()));
			}

			if (rt.Settings.Contains("Description"))
				report.Description = "This report shows the total number of firsts, seconds and thirds that were scored by each colour.";

			FinishReport(league, report, games, rt);

			return report;
		}

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

				if (league.VictoryPointsHighScore != 0 && game.Players().Any() && league.LeaguePlayer(game.Players().First()) != null)  // there is a highscore entry at the end of each row
				{
					for (int i = game.Teams.Count; i < mostTeams; i++)
					{
						row.Add(new ZCell());
						row.Add(new ZCell());
						row.Add(new ZCell());
					}

					var highPlayer = game.Players().First();

					row.Add(new ZCell(""));
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
	 		report.Title = (string.IsNullOrEmpty(rt.Title) ? league.Title + " Games " : rt.Title) + FromTo(games, rt.From, rt.To);
			
			for (int i = 0; i < mostTeams; i++)  // set up the Headings text, to cater for however many columns the report has turned out to be
			{
				report.AddColumn(new ZColumn("Team", ZAlignment.Left));
				report.AddColumn(new ZColumn("Score", ZAlignment.Right));
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

				if (rt.From != null || rt.To != null)
					report.Description += "  The report has been limited to games " + FromTo(games, rt.From, rt.To) + ".";
			}

			return report;
		}  // GamesList

		/// <summary>Build a list of games over a specified date/time range. One row per day; one game per cell.</summary>
		public static ZoomReport GamesToc(League league, bool includeSecret, ReportTemplate rt, GameHyper gameHyper)
		{
			ZoomReport report = new ZoomReport("Table of Contents");
			report.Columns.Add(new ZColumn("", ZAlignment.Right));

			List<Game> games = league.Games(includeSecret).Where(g => g.Time > (rt.From ?? DateTime.MinValue) && g.Time < (rt.To ?? DateTime.MaxValue)).ToList();
			games.Sort();

			var dates = games.Select(g => g.Time.Date).Distinct().ToList();
			foreach (var date in dates)
			{
				var dayGames = league.AllGames.Where(g => g.Time.Date == date);
				if (dayGames.Any())
				{
					ZRow dateRow = new ZRow();  // create a row
					report.Rows.Add(dateRow);
					if (dates.Count == 1)
						dateRow.Add(new ZCell(dayGames.First().Title));  // to show the title.
					else
						dateRow.Add(new ZCell(date.ToShortDateString()));  // to show the new date.
					int gamesThisRow = 0;
					var lastGame = dayGames.First();
					
					foreach (var game in dayGames)
					{
						if (gamesThisRow > 15 || lastGame.Time.AddHours(0.5) < game.Time || lastGame.Title != game.Title)  // If this row is too long, or if there's a 3 hour break between games, or the title has changed,
						{
							dateRow = new ZRow();  // Start a new row.
							report.Rows.Add(dateRow);
							dateRow.Add(new ZCell(game.Title));
							gamesThisRow = 0;
						}
						ZCell dateCell = new ZCell((game.Time.ToShortTimeString()).Trim());
						dateCell.Hyper = gameHyper(game);
						dateRow.Add(dateCell);
						gamesThisRow++;
						lastGame = game;
					}
				}
			}
			
			int columns = report.Rows.Max(r => r.Count());
			for (; report.Columns.Count < columns; )
				report.AddColumn(new ZColumn("Game"));

			if (rt.Settings.Contains("Description"))
			{
				report.Description = "This is a list of games. Each cell in the table is one game. Click the cell for details.";

				if (rt.From != null || rt.To != null)
					report.Description += "  The report has been limited to games " + FromTo(games, rt.From, rt.To) + ".";
			}
			return report;
		}

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
				report.AddColumn(column);

				if (league.IsPoints(games))
				{
					column = new ZColumn("Pts");
					column.GroupHeading = game.Title;
					report.AddColumn(column);
				}
	 		}

			int averageCol = report.Columns.Count();
			int pointsCol = averageCol + 1;
	 		if (rt.ReportType == ReportType.GameGrid)
	 		{
		 		report.AddColumn(new ZColumn("Average"));
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

				if (scoresList.Any())
				{
					if (rt.ReportType == ReportType.GameGrid || rt.ReportType == ReportType.GameGridCondensed)
						AddAverageAndDrops(league, row, rt.Drops, scoresList, pointsList);

					report.Rows.Add(row);
				}
			}  // foreach leagueTeam

			SortGridReport(league, report, rt, games, averageCol, pointsCol, false);

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

				if (gameTitle != null)
					titleCount.Add(gameTitle, maxCount);
				else if (titles.Count == 1)
					titleCount.Add("", maxCount);
			}

			// Create columns.
			foreach (var tc in titleCount)
				for (int i = 0; i < tc.Value; i++)
				{
					ZColumn column = new ZColumn("Time");
					column.GroupHeading = tc.Key;
					report.AddColumn(column);
					column = new ZColumn("Score", ZAlignment.Right);
					column.GroupHeading = tc.Key;
					report.AddColumn(column);
					column = new ZColumn(league.IsPoints() ? "Pts" : "Rank", ZAlignment.Right);
					column.GroupHeading = tc.Key;
					report.AddColumn(column);
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

				ZRow row = new ZRow();;

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
					while (col < report.Columns.Count && report.Columns[col].GroupHeading == gameTitle)
					{
	 					row.Add(new ZCell(""));
	 					row.Add(new ZCell(""));
	 					row.Add(new ZCell(""));
	 					col += 3;
					}
				}

				if (scoresList.Any())
				{
					if (rt.ReportType == ReportType.GameGridCondensed)
						AddAverageAndDrops(league, row, rt.Drops, scoresList, pointsList);

					report.Rows.Add(row);
				}
			}  // foreach leagueTeam

			SortGridReport(league, report, rt, games, averageCol, pointsCol, !league.IsPoints());
			if (rt.Settings.Contains("Description"))
				report.Description = "This is a grid of games. Each row in the table is one team.";
			FinishReport(league, report, games, rt);

			return report;
		}  // GamesGridCondensed

		public static ZoomReport SoloLadder(League league, bool includeSecret, ReportTemplate rt)
		{
			ChartType chartType = ChartTypeExtensions.ToChartType(rt.Setting("ChartType"));

			ZoomReport report = new ZoomReport(string.IsNullOrEmpty(rt.Title) ? league.Title + " Solo Ladder" : rt.Title,
			                                   "Rank,Player,Team,Average Score,Average Rank,Tags +,Tags-,Tag Ratio,Score Ratio,TR\u00D7SR,Destroys,Denies,Denied,Yellow,Red,Games,Dropped,Longitudinal",
			                                   "center,left,left,integer,integer,integer,integer,float,float,float,integer,integer,integer,integer,integer,integer,integer,float",
			                                   ",,,,,Tags,Tags,Ratios,Ratios,Ratios,Base,Base,Base,Penalties,Penalties,,,");
			report.MaxChartByColumn = true;

			if (rt.Drops != null && rt.Drops.HasDrops())
				report.AddColumn(new ZColumn("Dropped", ZAlignment.Integer));

			double bestScoreRatio = 0;
			string bestScoreRatioText = "";
			double bestTagRatio = 0;
			string bestTagRatioText = "";

			var playerTeams = league.BuildPlayerTeamList();
			foreach (var pt in playerTeams)
			{
				var player = pt.Key;
				var games = league.Games(includeSecret).Where(x => 
				                                              (rt.From ?? DateTime.MinValue) < x.Time &&
				                                              x.Time < (rt.To ?? DateTime.MaxValue) &&
				                                              x.Players().Exists(y => y.PlayerId == player.Id));

				if (games.Count() > 0)
				{
					ZRow row = new ZRow();
					report.Rows.Add(row);
					row.Add(new ZCell(0, ChartType.None, "N0"));  // Temporary rank
					row.AddCell(new ZCell(player.Name)).Hyper = "players.html#player" + player.Id;  // Player alias

					if (pt.Value.Count() == 1)
						row.Add(TeamCell(pt.Value.First()));
					else
						row.Add(new ZCell(string.Join(", ", pt.Value.Select(x => x.Name))));  // Team(s) played for

					var played = League.Played(games, player, includeSecret);

					row.Add(DataCell(played.Select(x => (double)x.Score).ToList(), rt.Drops, chartType, "N0"));  // Av score
					row.Add(DataCell(played.Select(x => (double)x.Rank).ToList(), rt.Drops, chartType, "N2"));  // Av rank
					row.Add(DataCell(played.Select(x => (double)x.HitsBy).ToList(), rt.Drops, chartType, "N0"));  // Tags +
					row.Add(DataCell(played.Select(x => (double)x.HitsOn).ToList(), rt.Drops, chartType, "N0"));  // Tags -
					row.Add(DataCell(played.Select(x => (double)x.HitsBy / x.HitsOn).ToList(), rt.Drops, chartType, "P1"));  // Tag ratio

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

					row.Add(DataCell(scoreRatios, rt.Drops, chartType, "P1"));  // Score ratio
					row.Add(DataCell(srxTrs, rt.Drops, chartType, "P1"));  // SR x TR

					row.Add(DataCell(played.Select(x => (double)x.BaseDestroys).ToList(), rt.Drops, ChartType.Bar, "N1"));
					row.Add(DataCell(played.Select(x => (double)x.BaseDenies).ToList(), rt.Drops, ChartType.Bar, "N1"));
					row.Add(DataCell(played.Select(x => (double)x.BaseDenied).ToList(), rt.Drops, ChartType.Bar, "N1"));
					row.Add(DataCell(played.Select(x => (double)x.YellowCards).ToList(), rt.Drops, ChartType.Bar, "N1"));
					row.Add(DataCell(played.Select(x => (double)x.RedCards).ToList(), rt.Drops, ChartType.Bar, "N1"));

					row.Add(new ZCell(played.Count(), ChartType.None, "N0"));  // Games

					if (rt.Drops == null)
						row.Add(new ZCell(""));  // games dropped
					else
					{
						int countAfterDrops = rt.Drops.CountAfterDrops(played.Count);
						if (countAfterDrops < played.Count)
				            row.Add(new ZCell(played.Count - countAfterDrops, ChartType.None, "N0"));  // games dropped
			    	    else
							row.Add(new ZCell(""));  // games dropped
					}

					if (rt.Settings.Contains("Longitudinal"))
						row.AddCell(new ZCell(null, ChartType.XYScatter, "P0")).Tag = pointRatios;  // Longitudinal scatter of score ratios and tag ratios.

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

			if (bestTagRatio > 0)
				report.Description += string.Format("Best tag ratio was {0:P0} by {1}. ", bestTagRatio, bestTagRatioText);
			if (bestScoreRatio > 0)
				report.Description += string.Format("Best score ratio was {0:P0} by {1}. ", bestScoreRatio, bestScoreRatioText);

			if (rt.Settings.Contains("Longitudinal"))
			{
				report.Rows[0].Last().Text = "1";
				report.RemoveZeroColumns();
				report.Rows[0].Last().Text = null;
				report.Description += " The Longitudinal column shows each game for each player. Scarlet is score ratio; blue is tag ratio.";
			}
			else
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
			                                   ",,,Tags,Tags,Ratio,Ratio,Ratio,Base,Base,Base,Penalties,Penalties");
			report.MaxChartByColumn = true;
//			for (int i = 8; i < report.Columns.Count; i++)
//				report.Columns[i].Rotate = true;

			bool solo = 1.0 * game.Players().Count / game.Teams.Count < 1.5;  // True if most "teams" have one player.

			var gameTotal = new GamePlayer();

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
						playerRow.AddCell(new ZCell(leaguePlayer.Name, color)).Hyper = "players.html#player" + leaguePlayer.Id;
					}

					FillDetails(playerRow, gamePlayer, color, (double)game.TotalScore() / game.Players().Count);

					teamTotal.Add(gamePlayer);
					gameTotal.Add(gamePlayer);

					report.Rows.Add(playerRow);
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
				if (!solo || gameTeam.Players.Count > 1 || gameTeam.Adjustment != 0 || (gameTeam.Players.Any() && gameTeam.Players[0].Score != gameTeam.Score))
				{
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
			bool solo = 1.0 * game.Players().Count / game.Teams.Count < 1.1;  // True if nearly all "teams" have one player.

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
					if (!solo || gameTeam.Players.Count > 1)
						column.GroupHeading = leagueTeam == null ? "Team ??" : leagueTeam.Name;
					column.Alignment = ZAlignment.Integer;
					report.AddColumn(column);

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
								int count = game.ServerGame.Events.Count(x => x.ServerPlayerId == ((ServerPlayer)player1).ServerPlayerId && x.OtherPlayer == ((ServerPlayer)player2).ServerPlayerId && x.Event_Type < 14);
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
						foreach (var eevent in game.ServerGame.Events.Where(x => x.ServerPlayerId == ((ServerPlayer)player1).ServerPlayerId && ((x.Event_Type >= 28 && x.Event_Type <= 34) ||(x.Event_Type >= 37 && x.Event_Type <= 1404))))
						{
							int now = (int)Math.Truncate(eevent.Time.Subtract(startTime).TotalMinutes);
							s.Append('\u00B7', now - minutes);  // Add one dot for each whole minute of the game.
							minutes = now;
	
							switch (eevent.Event_Type)
							{
									case 28: s.Append('\u25af'); break;  // warning: open rectangle
									case 29: s.Append('\u25ae'); break;  // terminated: filled rectangle
									case 30: s.Append('\u25cb'); break;  // hit base: open circle
									case 31: s.Append('\u2b24'); break;  // destroyed base: filled circle. For future: red U+1F534, blue U+1F535, green U+1F7E2, yellow U+1F7E1, purple U+1F7E3, orange U+1F7E0, brown U+1F7E4.
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

			report.AddColumn(new ZColumn("Base hits etc.", ZAlignment.Left));

			return report;
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

			var currents = new Dictionary<GameTeam, KeyValuePair<DateTime, int>>();  // Dictionary of gameTeam -> <time, team score>.
			
			foreach (var gameTeam in game.Teams)
				currents.Add(gameTeam, new KeyValuePair<DateTime, int>(game.Time, 0));

			float yMin = height;
			float yMax = 0;

			foreach (var oneEvent in game.ServerGame.Events)
				if (oneEvent.Score != 0)
				{
					var serverPlayer = game.ServerGame.Players.Find(sp => sp.ServerPlayerId == oneEvent.ServerPlayerId);
					if (serverPlayer != null)
					{
						var gameTeam = game.Teams.Find(t => t.Players.Exists(gp => gp.PlayerId == serverPlayer.PlayerId));
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
							var gamePlayer = game.Players().Find(gp => gp.PlayerId  == serverPlayer.PlayerId);
							var leaguePlayer = league.LeaguePlayer(gamePlayer);
							if (leaguePlayer != null)
							{
								var brush = new SolidBrush(gameTeam.Colour.ToDarkColor());
	
								if (currents.Max(x => x.Value.Value) == currents[gameTeam].Value)
									graphics.DrawString(leaguePlayer.Name, font, brush, newPoint.X - graphics.MeasureString(leaguePlayer.Name, font).Width, newPoint.Y);
								else
									graphics.DrawString(leaguePlayer.Name, font, brush, newPoint.X, (oldPoint.Y + newPoint.Y) / 2);
							}
						}
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

		public static ZoomReport OnePlayer(League league, LeaguePlayer player, List<LeagueTeam> teams, GameHyper gameHyper)
		{
			ZoomReport report = new ZoomReport(string.IsNullOrEmpty(player.Name) ? "Player " + player.Id : player.Name,
			                                   "Time,Score,Tags +,Tags -,Tag Ratio,Score Ratio,TR\u00D7SR,Destroys,Denies,Denied,Yellow,Red",
			                                   "left,integer,integer,integer,float,float,float,integer,integer,integer,integer,integer,integer",
			                                   ",,Tags,Tags,Ratios,Ratios,Ratios,Base,Base,Base,Penalties,Penalties,");

			report.Title += " \u2014 " + string.Join(", ", teams.Select(t => t.Name));
			if (teams.Count == 1)
				report.TitleHyper = "team" + teams[0].TeamId.ToString("D2", CultureInfo.InvariantCulture) + ".html";
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
					var gameCell = new ZCell((game.Title + " " + game.Time.ShortDateTime()).Trim(), color);  // Game time
					gameCell.Hyper = gameHyper(game);
					row.Add(gameCell);
				}
				
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
			ZRow totalRow = new ZRow();
			totalRow.Add(new ZCell("Average"));  // Game time
			if (teams.Count > 1)
				totalRow.Add(new ZCell(""));  // Team name

			var played = league.Played(player);
			if (played.Any())
				totals.Score /= played.Count;
			else
				totals.Score = 0;

			FillDetails(totalRow, totals, default(Color), (double)totalScore / totalCount);

			report.Rows.Add(totalRow);

			report.RemoveZeroColumns();
			return report;
		}  // OnePlayer

		/// <summary> List a team and its players' performance in each game.  One player per column; one game per row.</summary>
		public static ZoomReport OneTeam(League league, bool includeSecret, LeagueTeam team, DateTime from, DateTime to, bool description, GameHyper gameHyper)
		{
			ZoomReport report = new ZoomReport(team.Name);

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
				report.AddColumn(new ZColumn(leaguePlayer.Name, ZAlignment.Integer, "Players")).Hyper = "players.html#player" + leaguePlayer.Id;
			}
			report.AddColumn(new ZColumn("Total", ZAlignment.Integer, "Score"));
			if (league.IsPoints())
				report.AddColumn(new ZColumn("Pts", ZAlignment.Float, "Score"));

			report.AddColumns("Score again,Tags +,Tags -,Tag Ratio,Score Ratio,TR\u00D7SR,Destroys,Conceded,Ratio,Denies,Denied,Yellow,Red", 
			                  "integer,integer,integer,float,float,float,integer,integer,integer,integer,integer,integer",
			                  ",Tags,Tags,Ratios,Ratios,Ratios,Base,Base,Base,Base,Base,Penalties,Penalties");

			// TODO: add columns for total points for/against, total team tags for/against, total team denials for/against (from game.ServerGame.Events?)

			DateTime previousGameDate = DateTime.MinValue;
			// Add a row for each of this team's games. Fill in values for team score and player scores.
			foreach (GameTeam gameTeam in league.Played(team, includeSecret).OrderBy(gt => league.Game(gt).Time))
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
						GamePlayer gamePlayer = gameTeam.Players.Find(x => league.LeaguePlayer(x) != null && league.LeaguePlayer(x).Id == leaguePlayer.Id);
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
					if (game.ServerGame == null || game.ServerGame.Events == null)
					{
						basesConceded = new ZCell("");
						baseRatio = new ZCell("");
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

		/// <summary>Each row is a game.</summary>
		public static ZoomReport FixtureList(Fixture fixture, League league, GameHyper gameHyper)
		{
			ZoomReport report = new ZoomReport("Fixtures for " + league.Title, "Time", "left");
			report.CssClass = "fixturelist";

			var match = new Dictionary<FixtureGame, Game>();
			foreach (Game game in league.Games(false))
			{
				double score;
				var fg = fixture.BestMatch(game, out score);
				if (score > 0.5)
					match.Add(fg, game);
			}

			int maxTeams = fixture.Games.Count == 0 ? 0 : fixture.Games.Max(x => x.Teams.Count());
			
			for (int i = 0; i < maxTeams; i++)
				report.AddColumn(new ZColumn("Team", ZAlignment.Left));

			foreach (var fg in fixture.Games)
			{
				ZRow row = new ZRow();

				ZCell timeCell = new ZCell(fg.Time.ToString("yyyy/MM/dd HH:mm"));
				timeCell.CssClass = "time";
				if (match.ContainsKey(fg))
					timeCell.Hyper = gameHyper(match[fg]);
				row.Add(timeCell);

				foreach (var kv in fg.Teams.OrderBy(t => t.Value).ThenBy(t => t.Key.Name))
				{
					ZCell teamCell = new ZCell(kv.Key.Name, kv.Value.ToColor());
					teamCell.Hyper = "fixture.html?team=" + kv.Key.Id().ToString(CultureInfo.InvariantCulture);
					row.Add(teamCell);
				}

				row.CssClass = String.Concat(fg.Teams.Keys.Select(k => " t" + k.Id().ToString(CultureInfo.InvariantCulture)));

				report.Rows.Add(row);
			}

			return report;
		}

		/// <summary>Each row is a team. Each column is a game.</summary>
		public static ZoomReport FixtureGrid(Fixture fixture, League league)
		{
			ZoomReport report = new ZoomReport("Fixtures for " + league.Title, "Team", "left");
			report.CssClass = "fixturegrid";

			// Create a row for each team, with team name at left. 
			foreach (var ft in fixture.Teams)
			{
				var row = new ZRow();
				var teamCell = new ZCell(ft.Name);
				teamCell.Hyper = "fixture.html?team=" + ft.Id().ToString(CultureInfo.InvariantCulture);
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
						cell = new ZCell("");

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
				var teamCell = new ZCell(ft.Name);
				teamCell.Hyper = "fixture.html?team=" + ft.Id().ToString(CultureInfo.InvariantCulture);
				report.Rows[i].Add(teamCell);
			}

			return report;
		}

		// https://stackoverflow.com/questions/470690/how-to-automatically-generate-n-distinct-colors
		static List<Color> BoyntonColors = new List<Color>
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

		/// <summary>Show overall stats for each pack, including a t test comparing it to all other packs combined.</summary>
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
				leagueColors.Add(leagues[i], BoyntonColors[i % 9]);

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
			var	report = new ZoomReport((string.IsNullOrEmpty(title) ? (leagues.Count == 1 ? leagues[0].Title + " " : "") + "Pack Report" : title) + FromTo(games, from, to),
				                        "Rank,Pack,Score Ratio,t,p,Count,Tag Ratio,t,p,Count",
				                        "center,left,integer,float,float,integer,float,float,float,integer");
			
			if (longitudinal)
				report.Columns.Add(new ZColumn("Longitudinal", ZAlignment.Float));

			report.MaxChartByColumn = true;

			var packs = games.SelectMany(game => game.Players().Select(player => player.Pack)).Distinct().ToList();
			bool missingTags = false;  // True if any pack is missing some tag ratios.

			foreach (string pack in packs)
			{
				var row = new ZRow();
				report.Rows.Add(row);
				
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
			var averages = new ZRow();
			
			averages.Add(new ZCell("", Color.Gray));
			averages.Add(new ZCell("Averages", Color.Gray));
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
		}  // PackReport

		/// <summary>Every player in every game.</summary>
		public static ZoomReport EverythingReport(League league, string title, DateTime? from, DateTime? to, bool description)
		{
			var report = new ZoomReport(string.IsNullOrEmpty(title) ? league.Title + " Everything Report" : title + FromTo(league.AllGames, from, to),
			                                   "Player,Pack,Team,Rank,Score,Tags +,Tags -,Tag Ratio,Score Ratio,TR\u00D7SR,Destroys,Denies,Denied,Yellow,Red",
			                                   "left,left,left,integer,integer,integer,integer,float,float,float,integer,integer,integer,integer,integer,integer",
			                                   ",,,,,Tags,Tags,Ratio,Ratio,Ratio,Base,Base,Base,Penalties,Penalties");
			report.MaxChartByColumn = true;

			foreach (var game in league.AllGames.FindAll(x => x.Time.CompareTo(from ?? DateTime.MinValue) >= 0 &&
			                                             x.Time.CompareTo(to ?? DateTime.MaxValue) <= 0))
			{
				var gameTotal = new GamePlayer();

				foreach (var player in game.Players().OrderByDescending(p => p.Score))
				{
					var playerRow = new ZRow();

					Color color = player.Colour.ToColor();
					playerRow.AddCell(new ZCell(league.LeaguePlayer(player) == null ? player.PlayerId : league.LeaguePlayer(player).Name))
						.Hyper = "players.html#player" + player.PlayerId;;
					playerRow.Add(new ZCell(player.Pack));
					playerRow.Add(new ZCell(league.GameTeamFromPlayer(player) == null ? " " : league.LeagueTeam(league.GameTeamFromPlayer(player)).Name));
					playerRow.Add(new ZCell(player.Rank));

					FillDetails(playerRow, player, color, (double)game.TotalScore() / game.Players().Count);

					gameTotal.Add(player);
				
					report.Rows.Add(playerRow);
				}

				var gameRow = new ZRow();
				gameRow.Add(new ZCell("", Color.LightGray));
				gameRow.Add(new ZCell("", Color.LightGray));
				gameRow.Add(new ZCell(game.LongTitle(), Color.LightGray));
				gameRow.Add(new ZCell(""));

				FillDetails(gameRow, gameTotal, Color.Empty, game.TotalScore());

				report.Rows.Add(gameRow);
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
			teamcell.Hyper = "team" + leagueTeam.TeamId.ToString("D2", CultureInfo.InvariantCulture) + ".html";
			return teamcell;
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

			DateTime first = from == null ? DateTime.MinValue : (DateTime)from;
			first = games.First().Time > first ? games.First().Time : first;
			DateTime last = to == null ? DateTime.MaxValue : (DateTime)to;
			last = games.Last().Time < last ? games.Last().Time : last;

			if (first.Date == last.Date)  // This report is for games on a single day.
				return " for " + first.Date.ToShortDateString();

			return " from " + first.ToShortDateString() + " to " + last.ToShortDateString();  // This report is for games over multiple days.
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
				row.Add(new ZCell(pointsList.Sum(), ChartType.None, "N0"));  // total points

			if (scoresList.Count < count)
				row.Add(new ZCell(count - scoresList.Count, ChartType.None, "N0"));  // games dropped
			else
				row.Add(new ZCell(""));  // games dropped
		}
		
		static double? TopScore(ZoomReport report, int col)
		{
			return report.Rows.Max(r => r.Count <= col ? null : r[col].Number);
		}

		static void SortGridReport(League league, ZoomReport report, ReportTemplate rt, List<Game> games, int averageCol, int pointsCol, bool reversed)
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
										int lastx = (int)x.Last().Number;
										int lasty = (int)y.Last().Number;

										int result = 0;

										if (x[lastx].Number == TopScore(report, lastx) && y[lasty].Number == TopScore(report, lasty))
											result = Math.Sign(lastx - lasty);
										else if (x[lastx].Number == TopScore(report, lastx))  // x won its last game.
											result = -1;
										else if (y[lasty].Number == TopScore(report, lasty))  // y won its last game.
											result = +1;

										int last = Math.Max(lastx, lasty);
										if (x[last].Number.HasValue && y[last].Number.HasValue)  // x and y play each other in the last game for both of them.
											result = Math.Sign((double)y[last].Number - (double)x[last].Number);

										int notlast = Math.Min(lastx, lasty);
										if (result == 0 && x[notlast].Number.HasValue && y[notlast].Number.HasValue)  // x and y play each other in the last game for one of them.
											result = Math.Sign((double)y[notlast].Number - (double)x[notlast].Number);

										return result == 0 ? Math.Sign(lasty - lastx) : result;  // Just use who played last.
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
					report.RemoveColumn(report.Columns.Count - 1);  // Average
					if (league.IsPoints(games))
						report.RemoveColumn(report.Columns.Count - 1);  // Pts
					break;
				
				case ReportType.Pyramid: case ReportType.PyramidCondensed:
					PyramidComparer pc = new PyramidComparer();
					pc.Columns = report.Columns;
					pc.Reversed = reversed;
					pc.IsPoints = league.IsPoints(games) || rt.ReportType == ReportType.PyramidCondensed;
					pc.Setup(report);
					try 
					{
						report.Rows.Sort(pc);
					}
					catch
					{
						report.Title += "...";
					}

					pc.DoColor(league, report);
					pc.Cleanup();

					break;
			}

			for (int i = 0; i < report.Rows.Count; i++)
				report.Rows[i][0].Number = i + 1;

			report.Title = string.IsNullOrEmpty(rt.Title) ?
				league.Title + (rt.ReportType == ReportType.Ascension ? " Ascension" : 
			                    rt.ReportType == ReportType.Pyramid   ? " Pyramid" : 
			                                                            " Games") : rt.Title;
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
				else if (rt.From != null && games.Any())
					report.Description += " The report has been limited to games after " + games.First().Time.ToShortDateString() + ".";
				else if (rt.To != null && games.Any())
					report.Description += " The report has been limited to games before " + games.Last().Time.ToShortDateString() + ".";
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

				double averageScore;
				double averagePoints;

				CalcRoundScores(row, lastCol, lastGroup, out averageScore, out averagePoints);
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
			int lastx = (int)x[lastGameIndex].Number;
			int lasty = (int)y[lastGameIndex].Number;

			if ((int)x[lastGroupIndex].Number == (int)y[lastGroupIndex].Number)  // These teams' last games are in the same round.
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
				if (x[lastGroupIndex].Number == y[lastGroupIndex].Number + 1 && groups[(int)x[lastGroupIndex].Number].Contains("chage"))  // x's last game is a repechage.
				{
					if (IsPoints && y[averagePointsIndex].Number != x[previousPointsIndex].Number)
						return (Reversed ? -1 : 1) * Math.Sign((double)(y[averagePointsIndex].Number - x[previousPointsIndex].Number));  // Compare their previous round average points.
					else if (y[averageScoreIndex].Number != null && x[previousScoreIndex].Number != null)
						return Math.Sign((double)(y[averageScoreIndex].Number - x[previousScoreIndex].Number));  // Points were an exact match? Try scores.
				}

				if (x[lastGroupIndex].Number + 1 == y[lastGroupIndex].Number && groups[(int)y[lastGroupIndex].Number].Contains("chage"))  // y's last game is a repechage.
				{
					if (IsPoints && y[previousPointsIndex].Number != x[averagePointsIndex].Number)
						return (Reversed ? -1 : 1) * Math.Sign((double)(y[previousPointsIndex].Number - x[averagePointsIndex].Number));  // Compare their previous round average points.
					else if (y[previousScoreIndex].Number != null && x[averageScoreIndex].Number != null)
						return Math.Sign((double)(y[previousScoreIndex].Number - x[averageScoreIndex].Number));  // Points were an exact match? Try scores.
				}

				return (int)y[lastGroupIndex].Number - (int)x[lastGroupIndex].Number;  // Whoever survived longest ranks highest.
			}
		}

		// Mark alternate rounds in slightly different background colours.
		public void DoColor(League league, ZoomReport report)
		{
			foreach (ZRow row in report.Rows)
				for (int i = 0; i < row.Count && groups.IndexOf(Columns[i].GroupHeading) <= (int)row[lastGroupIndex].Number; i++)
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
