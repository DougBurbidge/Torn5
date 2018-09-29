using System;
using System.Collections.Generic;
using System.Globalization;
using System.Drawing;
using System.Linq;
using Zoom;

namespace Torn.Report
{
//	public enum TeamOrderBy { VictoryThenScore, VictoryThenRatio };
	public enum GridType { GameGrid, Ascension, Pyramid };

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

		public static ZoomReport TeamLadder(League league, bool includeSecret, DateTime? from, DateTime? to, Drops drops,
		                                    bool scaleGames, bool ratio, bool showColours, bool description)
		{
			ZoomReport report = new ZoomReport("", "Rank,Team", "center,left");
			report.MaxChartByColumn = true;
			report.Columns[0].ColumnType = ZColumnType.Integer;

	 		var colourTotals = new Dictionary<Colour, List<int>>();
			if (showColours)
		 		for (Colour c = Colour.Red; c <= Colour.Orange; c++)
				{
					report.Columns.Add(new ZColumn("1st", ZAlignment.Right, ZColumnType.Integer));
					report.Columns.Add(new ZColumn("2nd", ZAlignment.Right, ZColumnType.Integer));
					report.Columns.Add(new ZColumn("3rd", ZAlignment.Right, ZColumnType.Integer));
					report.Columns[report.Columns.Count - 3].GroupHeading = c.ToString();
					report.Columns[report.Columns.Count - 2].GroupHeading = c.ToString();
					report.Columns[report.Columns.Count - 1].GroupHeading = c.ToString();

					colourTotals.Add(c, new List<int>());
				}

			if (league.IsPoints())
				report.Columns.Add(new ZColumn("Points", ZAlignment.Right, ZColumnType.Float));

			report.Columns.Add(new ZColumn(ratio ? "Score Ratio" : "Average score", 
			                               ZAlignment.Right, ZColumnType.Float));
			report.Columns.Add(new ZColumn("Games", ZAlignment.Right, ZColumnType.Integer));

			if (drops != null && drops.HasDrops())
				report.Columns.Add(new ZColumn("Dropped", ZAlignment.Right, ZColumnType.Integer));

			List<Game> games = league.Games(includeSecret);
			report.Title = league.Title + " Team Ladder" + FromTo(games, from, to);

			int barcol = 0;
			double scoreTotal = 0;
			double pointsTotal = 0.0;

			List<int> countList = new List<int>();  // Number of games each team has played, for scaling.

			foreach (LeagueTeam team in league.Teams)  // Create a row for each League team.
			{
				ZRow row = new ZRow();

		        row.Add(new ZCell(""));  // put in a temporary Rank

		        ZCell teamcell = new ZCell(team.Name);
		        teamcell.Hyper = "team" + team.Id.ToString("D2", CultureInfo.InvariantCulture) + ".html";
		        row.Add(teamcell);

		 		double points = 0;

		 		var colourCounts = new Dictionary<Colour, List<int>>();
		 		for (Colour c = Colour.Red; c <= Colour.Orange; c++)
		 			colourCounts.Add(c, new List<int>());

		 		List<double> scoreList = new List<double>();  // Holds either scores or score ratios.

				ZCell scoreCell;
		 		foreach (GameTeam gameTeam in team.GameTeams(includeSecret))  // Roll through this team's games.
		 		{
		 			if ((from == null || gameTeam.Game.Time >= from) && (to == null || gameTeam.Game.Time <= to))
					{
		 				if (ratio)
		 				{
							if (gameTeam.Game.TotalScore() != 0)
								scoreList.Add(gameTeam.Score / gameTeam.Game.TotalScore() * gameTeam.Game.Teams.Count);
		 				}
		 				else
							scoreList.Add(gameTeam.Score);

						points += gameTeam.Points;

						if (showColours)
						{
							// Add the team's rank in this game to the colourCounts.
							int rank = gameTeam.Game.Teams.IndexOf(gameTeam);

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
					row.Add(new ZCell(points));  // league points scored
				pointsTotal += points;

				int count = scoreList.Count;  // Number of games _before_ any are dropped.

				if (scoreList.Count == 0)
					scoreCell = new ZCell("-");     // average game scores
				else
				{
					scoreCell = new ZCell(0, ChartType.Bar | ChartType.Rug);
					DropScores(scoreList, drops);

					if (ratio)
					{
						scoreCell.Number = (scoreList.Average() * 100);
						scoreCell.NumberFormat = "P1";  // average game score ratio
						scoreCell.Data.AddRange(scoreList.Select(x => x / scoreList.Average() * 100));
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
				barcol = row.Count - (league.IsPoints() ? 2 : 1);
				scoreCell.ChartCell = barcol; 

				row.Add(new ZCell(count, ChartType.None, "N0"));  // games played

		        if (scoreList.Count < count)
		            row.Add(new ZCell(count - scoreList.Count, ChartType.None, "N0"));  // games dropped

		        countList.Add(scoreList.Count);

		        if (showColours)
		        {
		            for (int i = 2; i < barcol; i++)
		            	row[i].ChartCell = i;
		            for (int i = barcol; i < row.Count; i++)
		            	row[i].ChartCell = barcol;
		        }
		        else
					foreach (ZCell cell in  row)
						cell.ChartCell = barcol;

				if (count > 0)
					report.Rows.Add(row);
			}  // foreach team in league.Teams

			bool more = false;
			bool less = false;
			int mode = 0;

			if (scaleGames && countList.Count > 0 && countList[0] != countList[countList.Count - 1])  // Find the mode, and add scaled columns.
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
	        		int count = gamesColumn == -1 ? 1 : int.Parse(row[gamesColumn].Text.Trim(), CultureInfo.InvariantCulture);
//					if (drops != null && drops.HasDrops())
//						    count = StrToIntDef(Trim(report.Rows[i][coloffset + 4].Text), 0) - StrToIntDef(Trim(report.Rows[i][coloffset + 5].Text), 0);

		            if (league.IsPoints())
		            {
		                if (mode != count && count > 0) 
		                {
		                	row.Add(new ZCell((row[barcol].Number * mode / count), ChartType.None, "F0"));  // scaled points
		                    if (mode < count)
							    more = true;
							else
							    less = true;
		                }
		                else
		                	row.Add(new ZCell(""));  // scaled points
		            }
	        	}

//		        report.OnCalcBar = TeamLadderScaledCalcBar;
			}  // if scalegames

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
				row.Add(new ZCell("Totals", Color.Gray));  // Team name.

		 		for (Colour c = Colour.Red; c <= Colour.Orange; c++)
		 			for (int rank = 0; rank < 3; rank++)
		 			{
		 			Color dark = Color.FromArgb((c.ToColor().R + Color.Gray.R) / 2, (c.ToColor().G + Color.Gray.G) / 2, (c.ToColor().B + Color.Gray.B) / 2);
		 				if (colourTotals[c].Count > rank)
							row.Add(new ZCell(BlankZero(colourTotals[c][rank]), dark));
		 				else
		 					row.Add(new ZCell("", dark));
		 			}

				if (league.IsPoints())
					row.Add(new ZCell(pointsTotal / report.Rows.Count(), ChartType.None, "N0"));  // average league points scored

				if (ratio)
					row.Add(new ZCell(scoreTotal * 100.0 / report.Rows.Count(), ChartType.Bar, "P1"));     // average game score ratio
				else
					row.Add(new ZCell(scoreTotal / report.Rows.Count(), ChartType.Bar, "N0"));     // average game score

				row.Add(new ZCell(countList.Average(), ChartType.None, "N0"));  // average games played

				if (drops != null && drops.HasDrops())
		            row.Add(new ZCell(""));  // games dropped

//		    if IsPoints(Prefs) then
//		        row[report.Columns.Count - 3].Text = '';  // points
//		    row[report.Columns.Count - 2].Text = FloatToStr(row[report.Columns.Count - 2].AsInteger / report.Rows.Count);  // average score
//		//            row[report.Columns.Count - 1].Text = FloatToStr(row[report.Columns.Count - 1].AsInteger / report.Rows.Count);  // games
//		
//		    // Clear groups of three columns that contain only '0's.
//		    colours = 8;
//		    for j = 0 to 7 do begin
//		        zeroes = True;
//		        for k = 0 to 2 do
//		            for i = 0 to report.Rows.Count - 1 do
//		                zeroes = zeroes and (report.Rows[i][j * 3 + k + 2].Text = '0');
//		
//		        if zeroes then begin
//		            for k = 0 to 2 do
//		                for i = 0 to report.Rows.Count - 1 do
//		                    report.Rows[i][j * 3 + k + 2].Text = '';
//		            Dec(colours);
//		        end;
//		    end;
//		
//		    row[1].Text = Format('Totals (average = %d)', [GameList.Count]);
//		
//		    report.OnCalcBar = TeamLadderColourCalcBar;
//		end; // if ShowColours
			}
			//report.Rows.PurgeBlankRows;

			if (description)
			{
			    report.Description = "This report ranks teams. ";

//		    if (DropWorst > 0) and (DropBest > 0) then
//		        if PercentGames then
//		            report.Description += "The top " + DropBest.ToString() + "% and bottom " + DropWorst.ToString() + "% of scores for each team have been dropped. "
//		        else
//		            report.Description += "The top " + DropBest.ToString() + " and bottom " + DropWorst.ToString() + " scores for each team have been dropped. "
//		    else if DropWorst > 0 then
//		        if PercentGames then
//		            report.Description += "The bottom " + DropWorst.ToString() + "% of scores for each team have been dropped. "
//		        else
//		            report.Description += "The bottom " + DropWorst.ToString() + " scores for each team have been dropped. "
//		    else if DropWorst > 0 then
//		        if PercentGames then
//		            report.Description += "The top " + (ropBest.ToString() + "% of scores for each team have been dropped. "
//		        else
//		            report.Description += "The top " + DropBest.ToString() + " scores for each team have been dropped. ";
		
			    if (more && less)
					report.Description += "Teams with more or less than " + mode.ToString(CultureInfo.CurrentCulture) + " games have been scaled down or up. This is shown in the Scaled column. ";
	    		else if (more)
					report.Description += "Teams with more than " + mode.ToString(CultureInfo.CurrentCulture) + " games have been scaled down. This is shown in the Scaled column. ";
				else if (less)
					report.Description += "Teams with less than " + mode.ToString(CultureInfo.CurrentCulture) + " games have been scaled up. This is shown in the Scaled column. ";
	
//				if (datelimited)
//					report.Description += "The report has been limited to games " + FromTo(games, from, to) + ". ";

//				if (ShowColours)
//					report.Description += "For each team, the report shows the number of times they place first, second and third on each colour. The Totals row shows the total number of firsts, seconds and thirds that were scored by each colour.";

				if (report.Description == "This report ranks teams. ")  // No interesting boxes are checked,
			  			report.Description = "";  // so this description is too boring to show.

			}
			return report;
		}  // BuildTeamLadder()

		/// <summary>Build a square tale showing how many times each team has played (and beaten) each other team.</summary>
		public static ZoomReport TeamsVsTeams(League league, bool includeSecret, DateTime? from, DateTime? to, bool description, GameHyper gameHyper)
		{
			ZoomReport report = new ZoomReport(league.Title + " Teams vs Teams", "Teams", "left");
			
			List<LeagueTeam> teams = new List<LeagueTeam>();
			teams.AddRange(league.Teams);
			teams.Sort(delegate(LeagueTeam x, LeagueTeam y)
			           {
			           	double result = y.AveragePoints(includeSecret) - x.AveragePoints(includeSecret);
			           	return Math.Sign(result == 0 ? y.AverageScore(includeSecret) - x.AverageScore(includeSecret) : result);
			           });

			foreach (var team1 in teams)
			{
				var column = new ZColumn(team1.Name, ZAlignment.Center);
				column.Hyper = "team" + team1.Id.ToString("D2", CultureInfo.InvariantCulture) + ".html";
				column.Rotate = true;
				report.Columns.Add(column);

				var row = new ZRow();
				report.Rows.Add(row);

				var teamCell = new ZCell(team1.Name);
				teamCell.Hyper = "team" + team1.Id.ToString("D2", CultureInfo.InvariantCulture) + ".html";
				row.Add(teamCell);

				foreach (var team2 in teams)
				{
					var games = league.Games(includeSecret).FindAll(x => x.Teams.Any(y => y.LeagueTeam == team1) && 
					                                                     x.Teams.Any(z => z.LeagueTeam == team2));  // Get games that include these two teams.
					ZCell cell;
					if (team1 == team2)
						cell = new ZCell("\\", Color.Gray);
					else if (games.Count == 0)
						cell = new ZCell((string)null);
					else
					{
						var wins = games.Count(x => x.Teams.FindIndex(y => y.LeagueTeam == team1) < x.Teams.FindIndex(z => z.LeagueTeam == team2));
						cell = new ZCell(wins.ToString(CultureInfo.CurrentCulture) +
						                     "/" + games.Count().ToString(CultureInfo.CurrentCulture));
						cell.Number = (double)wins / games.Count();
						cell.ChartType = ChartType.Bar;
						if (games.Count == 1)
							cell.Hyper = gameHyper(games[0]);
					}
					row.Add(cell);
				}
			}

			return report;
		}  // TeamsVsTeams

		/// <summary>Build a list of games over a specified date/time range. One game per row.</summary>
		public static ZoomReport GamesList(League league, bool includeSecret, DateTime? from, DateTime? to, bool description, GameHyper gameHyper)
		{
			ZoomReport report = new ZoomReport("", "Game", "right");

//			report.MultiColumnOK = true;
			
			DateTime lastgametime = DateTime.MinValue;
			DateTime thisgametime = DateTime.MinValue;
			int thisgame = 0;
			List<Game> games = league.Games(includeSecret);
			games.Sort();

			DateTime newFrom = from ?? DateTime.MinValue;
			
			// Find the game immediately after 'from'.
			while (thisgame < games.Count && games[thisgame].Time < newFrom)
				thisgame++;

	 		if (thisgame < games.Count)
				newFrom = games[thisgame].Time;

	 		int mostTeams = 0;
	 		foreach (Game game in games)
	 			if (mostTeams < game.Teams.Count)
	 				mostTeams = game.Teams.Count;

	 		while (thisgame < games.Count && (to == null || games[thisgame].Time < to))  // loop through each game, and create a row for it
			{
				Game game = games[thisgame];
			    thisgametime = game.Time;
			    if (lastgametime.Date < thisgametime.Date)  // We've crossed a date boundary, so
			    {
			    	ZRow dateRow = new ZRow();  // create a row
			    	report.Rows.Add(dateRow);
			    	dateRow.Add(new ZCell(thisgametime.ToShortDateString()));  // to show the new date.
			    }

				lastgametime = thisgametime;
				ZRow row = new ZRow();;
			    report.Rows.Add(row);

			    ZCell dateCell = new ZCell((game.Title + " " + thisgametime.ToShortTimeString()).Trim());
			    dateCell.Hyper = gameHyper(game);
			    row.Add(dateCell);

				foreach (GameTeam gameTeam in game.Teams)
				{
			        ZCell teamCell = new ZCell(gameTeam.LeagueTeam == null ? "Team ?" : gameTeam.LeagueTeam.Name, 
					                           gameTeam.Colour.ToColor());  // team name
			        teamCell.Hyper = "team" + gameTeam.TeamId.ToString("D2", CultureInfo.InvariantCulture) + ".html";
			        row.Add(teamCell);
					ZCell scoreCell = new ZCell(gameTeam.Score, ChartType.Bar, "N0", gameTeam.Colour.ToColor());
					row.Add(scoreCell);
					teamCell.ChartCell = row.Count - 1;
					scoreCell.ChartCell = row.Count - 1;
				    if (league.IsPoints())  // there are victory points for this league
				    {
				    	row.Add(new ZCell(gameTeam.Points, ChartType.None, "", gameTeam.Colour.ToColor()));
				    	row[row.Count - 1].ChartCell = row.Count - 2;
				    }
				    else
				    	row.Add(new ZCell(""));
				}  // foreach gameTeam

				if (league.VictoryPointsHighScore != 0 && game.Players.Count > 0 && game.Players[0].LeaguePlayer != null)  // there is a highscore entry at the end of each row
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
						
					row.Add(new ZCell(game.Players[0].LeaguePlayer.Name, highScoreColor));
					row.Add(new ZCell(game.Players[0].Score, ChartType.Bar, "N0", highScoreColor));
					row[row.Count - 2].ChartCell = row.Count - 1;
					row[row.Count - 1].ChartCell = row.Count - 1;
				}

			    thisgame++;
			}  // while date/time <= Too

			if (newFrom.Date == thisgametime.Date && report.Rows.Count > 0)
			    report.Rows.RemoveAt(0);  // the first entry in rows is a date line; since there"s only one date for the whole report, we can delete it.
			report.Title = league.Title + " Games " + FromTo(games, newFrom, thisgametime);
			
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
		
				if (description)
					report.Description = "This is a list of games.  Each row in the table is one game.  Across each row, you see the teams that were in that game (with the team that placed first listed first, and so on), and the score for each team.";
		
				if (league.VictoryPointsHighScore != 0)
					report.Description += "  At the end of each row, you see the high-scoring player for that game, and their score.";
		
		//            if WithinDateRange then
		//                report.Description = report.Description + "  The report has been limited to games " FromTo(games, newFrom, thisgametime) + ".";
			return report;
		}  // GamesList

		/// <summary> Build a list of games over a specified date/time range. One team per row; one game per column.
		public static ZoomReport GamesGrid(League league, bool includeSecret, DateTime? from, DateTime? to, Drops drops, bool description, GridType gridType, GameHyper gameHyper)
		{
			ZoomReport report = new ZoomReport("", "Rank,Team", "center,left");

			DateTime thisgametime = DateTime.MinValue;
			int thisgame = 0;
			List<Game> games = league.Games(includeSecret);
			games.Sort();

			DateTime newFrom = from ?? DateTime.MinValue;
			DateTime newTo = to ?? DateTime.MaxValue;
			
			// Find the game immediately after 'from'.
			while (thisgame < games.Count && games[thisgame].Time < newFrom)
				thisgame++;

	 		if (thisgame < games.Count)
				newFrom = games[thisgame].Time;

	 		// Create columns.
	 		foreach (Game game in games)
				if (game.Time >= newFrom && game.Time <= newTo)
				{
					thisgametime = game.Time;
					ZColumn column = new ZColumn(
						game.Time.Date < thisgametime.Date ? 
						game.Time.ToShortDateString() + " " + game.Time.ToShortTimeString() :
						game.Time.ToShortTimeString(),
						ZAlignment.Right, ZColumnType.Integer);
					column.GroupHeading = game.Title;
					column.Hyper = gameHyper(game);
					report.Columns.Add(column);

					if (league.IsPoints())
					{
						column = new ZColumn("Pts");
						column.GroupHeading = game.Title;
						report.Columns.Add(column);
					}
		 		}

	 		report.Columns.Add(new ZColumn("Average"));
			int averageCol = report.Columns.Count() - 1;
			int pointsCol = averageCol;
			if (league.IsPoints())
			{
				report.Columns.Add(new ZColumn("Pts"));
				pointsCol = report.Columns.Count() - 1;
			}

			if (drops != null && (drops.DropWorst(100) > 0 || drops.DropBest(100) > 0))
				report.Columns.Add(new ZColumn("Dropped", ZAlignment.Right, ZColumnType.Integer));

			foreach (LeagueTeam leagueTeam in league.Teams)
			{
				List<double> scoresList = new List<double>();
				List<double> pointsList = new List<double>();

				ZRow row = new ZRow();;

				row.Add(new ZCell(0, ChartType.None, "N0")); // Temporary blank rank.

				ZCell teamcell = new ZCell(leagueTeam.Name);
				teamcell.Hyper = "team" + leagueTeam.Id.ToString("D2", CultureInfo.InvariantCulture) + ".html";
				row.Add(teamcell);
				
				// Add a cell for each game for this team. If the team is not in this game, add a blank.
		 		foreach (Game game in games)
					if (game.Time >= newFrom && game.Time <= newTo)
		 			{
		 				GameTeam gameTeam = game.Teams.Find(x => x.LeagueTeam == leagueTeam);
		 				if (gameTeam == null)
		 				{
		 					row.Add(new ZCell(""));
							if (league.IsPoints())
								row.Add(new ZCell(""));
		 				}
		 				else
		 				{
		 					row.Add(new ZCell(gameTeam.Score, ChartType.Bar, "N0", gameTeam.Colour.ToColor()));
							if (league.IsPoints())
								row.Add(new ZCell(gameTeam.Points, ChartType.None, "", gameTeam.Colour.ToColor()));
							scoresList.Add(gameTeam.Score);
							pointsList.Add(gameTeam.Points);
		 				}
				}  // foreach gameTeam

				if (scoresList.Count > 0)
				{
					AddAverageAndDrops(league, row, drops, scoresList, pointsList);
					report.Rows.Add(row);
				}
			}  // foreach leagueTeam

			SortGridReport(league, report, gridType, games, averageCol, pointsCol, false);
			FinishGridReport(league, report, games, from, to, description);

			return report;
		}  // GamesGrid

		/// <summary> Build a list of games over a specified date/time range. One team per row.</summary>
		public static ZoomReport GamesGridCondensed(League league, bool includeSecret, DateTime? from, DateTime? to, Drops drops, bool description, GridType gridType, GameHyper gameHyper)
		{
			ZoomReport report = new ZoomReport("", "Rank,Team", "center,left");

			List<Game> games = league.Games(includeSecret);
			games.Sort();

			DateTime newFrom = from ?? DateTime.MinValue;
			DateTime newTo = to ?? DateTime.MaxValue;
			
			Game after = games.Exists(x => x.Time > newTo) ? games.Find(x => x.Time > newTo) : null;
			games = games.FindAll(x => x.Time.CompareTo(newFrom) >= 0 && x.Time.CompareTo(newTo) <= 0);
			List<string> titles = games.Select(x => x.Title).Distinct().ToList();
			var titleCount = new Dictionary<string, int>();

			// Find the number of columns we need for each title -- this is the maximum number of games any single team has played with that title. 
			foreach (string title in titles)
			{
				// select max(count(team_id)) from (select game_id, team_id from games join teams where game.title = title)
				int maxCount = 0;
				foreach (var team in league.Teams)
					maxCount = Math.Max(maxCount, games.FindAll(x => x.Title == title && x.Teams.Exists(y => y.LeagueTeam == team)).Count);

				titleCount.Add(title, maxCount);
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

			if (drops != null && (drops.DropWorst(100) > 0 || drops.DropBest(100) > 0))
				report.Columns.Add(new ZColumn("Dropped", ZAlignment.Right, ZColumnType.Integer));

			foreach (LeagueTeam leagueTeam in league.Teams)
			{
				var scoresList = new List<double>();
				var pointsList = new List<double>();

				ZRow row = new ZRow();;

				row.Add(new ZCell(0, ChartType.None, "N0")); // Temporary blank rank.

				ZCell teamcell = new ZCell(leagueTeam.Name);
				teamcell.Hyper = "team" + leagueTeam.Id.ToString("D2", CultureInfo.InvariantCulture) + ".html";
				row.Add(teamcell);
				
				int col = 2;
				while (col < averageCol)
				{
					string title = report.Columns[col].GroupHeading;
					// Add a cell for each game for this team.
					foreach (Game game in games.Where(x => x.Title == title))
		 			{
		 				GameTeam gameTeam = game.Teams.Find(x => x.LeagueTeam == leagueTeam);
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
					while (report.Columns[col].GroupHeading == title)
					{
	 					row.Add(new ZCell(""));
	 					row.Add(new ZCell(""));
	 					row.Add(new ZCell(""));
	 					col += 3;
					}
				}

				if (scoresList.Count > 0)
				{
					AddAverageAndDrops(league, row, drops, scoresList, pointsList);
					report.Rows.Add(row);
				}
			}  // foreach leagueTeam

			SortGridReport(league, report, gridType, games, averageCol, pointsCol, !league.IsPoints());
			FinishGridReport(league, report, games, from, to, description);

			return report;
		}  // GamesGridCondensed

		public static ZoomReport SoloLadder(League league, bool includeSecret, bool showComment, DateTime? from, DateTime? to, Drops drops, bool description)
		{
			ZoomReport report = new ZoomReport(league.Title + " Solo Ladder",
			                                   "Rank,Player,Team,Average Score,Average Rank,Score Ratio,Games",
			                                   "center,left,left,right,right,right,right");
			report.MaxChartByColumn = true;

			if (drops != null && drops.HasDrops())
				report.Columns.Add(new ZColumn("Dropped", ZAlignment.Right, ZColumnType.Integer));

			foreach (LeaguePlayer player in league.Players)
			{
//				DateTime newFrom = from == null ? DateTime.MinValue : (DateTime)from;
				var games = league.Games(includeSecret).Where(x => 
				                                              (from ?? DateTime.MinValue) < x.Time &&
				                                              x.Time < (to ?? DateTime.MaxValue) &&
				                                              x.Players.Exists(y => y.PlayerId == player.Id));

				if (games.Count() > 0)
				{
					ZRow row = new ZRow();
					report.Rows.Add(row);
					row.Add(new ZCell(0, ChartType.None, "N0"));  // Temporary rank
					row.Add(new ZCell(player.Name));

					var teams = league.Teams.Where(x => x.Players.Any(y => y == player));
					if (teams.Count() == 1)
					{
						ZCell teamCell = new ZCell(teams.First().Name);
						teamCell.Hyper = "team" + teams.First().Id.ToString("D2", CultureInfo.InvariantCulture) + ".html";
						row.Add(teamCell);
					}
					else
						row.Add(new ZCell(string.Join(", ", teams.Select(x => x.Name))));  // Team(s) played for

					row.Add(DataCell(player.Played.Select(x => (double)x.Score).ToList(), drops, ChartType.Bar | ChartType.Rug, "N0"));  // Av score
					row.Add(DataCell(player.Played.Select(x => (double)x.Rank).ToList(), drops, ChartType.Bar | ChartType.Rug, "N2"));  // Av rank
					
					List<double> ratiosList = new List<double>();
					foreach (var played in player.Played)
//						if (played.GameTeam != null && played.GameTeam.Game != null)
							ratiosList.Add(((double)played.Score) / played.GameTeam.Game.TotalScore() * played.GameTeam.Game.Players.Count);
					row.Add(DataCell(ratiosList, drops, ChartType.Bar | ChartType.Rug, "P1"));  // Score ratio

					row.Add(new ZCell(games.Count(), ChartType.None, "N0"));  // Games

					if (drops == null)
						row.Add(new ZCell(""));  // games dropped
					else
					{
						int countAfterDrops = drops.CountAfterDrops(player.Played.Count);
						if (countAfterDrops < player.Played.Count)
				            row.Add(new ZCell(player.Played.Count - countAfterDrops, ChartType.None, "N0"));  // games dropped
			    	    else
							row.Add(new ZCell(""));  // games dropped
					}

 					row[0].ChartCell = 3;
 					row[1].ChartCell = 3;
 					row[2].ChartCell = 3;
 					row[3].ChartCell = 3;
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

			return report;
		}  // SoloLadder

		public static ZoomReport OneGame(Game game)
		{
			string title = string.IsNullOrEmpty(game.Title) ? "Game " : game.Title + " Game ";

			ZoomReport report = new ZoomReport(title + game.Time.ToString(),
			                                   "Rank,Name,Score,Tags +,Tags -,Tag Ratio,Score Ratio,TR x SR,Base Hits,Base Destroys",
			                                   "center,left,right,right,right,right,right,right,right,right");

			//report.MaxBarByColumn = true;
			
			int totalhitsby = 0;
			int totalhitson = 0;
			int totalbasehits = 0;
			int totalbasedestroys = 0;

			foreach (GameTeam gameTeam in game.Teams)
			{
			    int teamhitsby = 0;
			    int teamhitson = 0;
			    int teambasehits = 0;
			    int teambasedestroys = 0;

				foreach (GamePlayer gamePlayer in gameTeam.Players)
				{
					// Add a row for each player on the team.
					ZRow row = new ZRow();

					Color color = gameTeam.Colour.ToColor();
					row.Add(new ZCell(gamePlayer.Rank, ChartType.None, "N0", color));

					if (gamePlayer.LeaguePlayer == null)
						row.Add(new ZCell("Player " + gamePlayer.PlayerId, color));
					else						
						row.Add(new ZCell(gamePlayer.LeaguePlayer.Name, color));

					row.Add(new ZCell(gamePlayer.Score, ChartType.Bar, "N0", color));
					row.Add(new ZCell(gamePlayer.HitsBy, ChartType.Bar, "N0", color));
					row.Add(new ZCell(gamePlayer.HitsOn, ChartType.Bar, "N0", color));

					if (gamePlayer.HitsOn == 0)  // Tag ratio
						row.Add(new ZCell("infinite", color));
					else
						row.Add(new ZCell((double)gamePlayer.HitsBy / gamePlayer.HitsOn, ChartType.Bar, "P0", color));

					row.Add(new ZCell((double)gamePlayer.Score / game.TotalScore() * game.Players.Count, ChartType.Bar, "P0", color));  // Score ratio

					if (gamePlayer.HitsOn == 0)  // TR x SR
						row.Add(new ZCell("infinite", color));
					else
					    row.Add(new ZCell(((double)gamePlayer.HitsBy / gamePlayer.HitsOn *
						                   gamePlayer.Score / game.TotalScore() * game.Players.Count), ChartType.Bar, "P0", color));

					row.Add(new ZCell(BlankZero(gamePlayer.BaseHits), color));
					row.Add(new ZCell(BlankZero(gamePlayer.BaseDestroys), color));

					teamhitsby += gamePlayer.HitsBy;
					teamhitson += gamePlayer.HitsOn;
					teambasehits += gamePlayer.BaseHits;
					teambasedestroys += gamePlayer.BaseDestroys;

					totalhitsby += gamePlayer.HitsBy;
					totalhitson += gamePlayer.HitsOn;
					totalbasehits += gamePlayer.BaseHits;
					totalbasedestroys += gamePlayer.BaseDestroys;

					report.Rows.Add(row);
				}

				// Add a row for the team.
				ZRow teamRow = new ZRow();
				Color teamColor = gameTeam.Colour.ToColor();
				teamColor = Color.FromArgb(teamColor.A, (int)(teamColor.R * 0.75), (int)(teamColor.G * 0.75), (int)(teamColor.B * 0.75));

				teamRow.Add(new ZCell(""));  // Rank.

				if (gameTeam.LeagueTeam == null)
					teamRow.Add(new ZCell("Team " + gameTeam.TeamId.ToString(CultureInfo.InvariantCulture), teamColor));
				else
				{
					ZCell teamCell = new ZCell(gameTeam.LeagueTeam.Name, teamColor);
			        teamCell.Hyper = "team" + gameTeam.TeamId.ToString("D2", CultureInfo.InvariantCulture) + ".html";
					teamRow.Add(teamCell);
				}

				teamRow.Add(new ZCell(gameTeam.Score, ChartType.Bar, "N0", teamColor));
				teamRow.Add(new ZCell(teamhitsby, ChartType.Bar, "N0", teamColor));
				teamRow.Add(new ZCell(teamhitson, ChartType.Bar, "N0", teamColor));

				if (teamhitson == 0)  // Tag ratio
					teamRow.Add(new ZCell("infinite", teamColor));
				else
					teamRow.Add(new ZCell(((double)teamhitsby / teamhitson), ChartType.Bar, "P0", teamColor));

				teamRow.Add(new ZCell(((double)gameTeam.Score / game.TotalScore() * game.Teams.Count), ChartType.Bar, "P0", teamColor));  // Score ratio

				if (teamhitson == 0)  // TR x SR
					teamRow.Add(new ZCell("infinite", teamColor));
				else
					teamRow.Add(new ZCell(((double)teamhitsby / teamhitson * gameTeam.Score / game.TotalScore() * game.Teams.Count), 
					                      ChartType.Bar, "P0", teamColor));

				teamRow.Add(new ZCell(BlankZero(teambasehits), teamColor));
				teamRow.Add(new ZCell(BlankZero(teambasedestroys), teamColor));

				report.Rows.Add(teamRow);
			}

			// Add an overall game average row.
			ZRow gameRow = new ZRow();
			gameRow.Add(new ZCell(""));  // Rank.
			gameRow.Add(new ZCell("Team Average"));  // Name.
			gameRow.Add(new ZCell((double)game.TotalScore() / game.Teams.Count, ChartType.Bar, "N0"));  // Average score
			gameRow.Add(new ZCell(totalhitsby / game.Teams.Count, ChartType.Bar, "P0"));
			gameRow.Add(new ZCell(totalhitson / game.Teams.Count, ChartType.Bar, "P0"));

			gameRow.Add(new ZCell("1.00"));  // Tag ratio
			gameRow.Add(new ZCell("1.00"));  // Score ratio
			gameRow.Add(new ZCell("1.00"));  // TR x SR

			gameRow.Add(new ZCell(((double)totalbasehits / game.Teams.Count), ChartType.None, "P0"));
			gameRow.Add(new ZCell(((double)totalbasedestroys / game.Teams.Count), ChartType.None, "P0"));

			if (totalhitsby == 0 && totalhitson == 0 && totalbasehits == 0 && totalbasedestroys == 0)  // There is no hit data, probably because we are Calculate By Nexus/Helios.
			{
				report.RemoveColumn(9);  // base destroys
				report.RemoveColumn(8);  // base hits
				report.RemoveColumn(7);  // tag ratio x score ratio
				report.RemoveColumn(5);  // tag ratio
				report.RemoveColumn(4);  // hits on
				report.RemoveColumn(3);  // hits by
			}

			report.Rows.Add(gameRow);
			
			return report;
		}  // OneGame

		/// <summary> List a team and its players' performance in each game.  One player per column; one game per row.</summary>
		public static ZoomReport OneTeam(League league, bool includeSecret, LeagueTeam team, DateTime from, DateTime to, bool description, GameHyper gameHyper)
		{
			ZoomReport report = new ZoomReport(team.Name);

			report.Columns.Add(new ZColumn("Game Time", ZAlignment.Left, ZColumnType.String));

			var averages = new Dictionary<LeaguePlayer, double>();
			foreach (LeaguePlayer leaguePlayer in team.Players)
			{
				double score = 0;
				int count = 0;

				foreach (GameTeam gameTeam in team.GameTeams(includeSecret))
					if (gameTeam.Game.Time > from && gameTeam.Game.Time < to)
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
				report.Columns.Add(new ZColumn(leaguePlayer.Name, ZAlignment.Right, ZColumnType.Integer));

			report.Columns.Add(new ZColumn("Total", ZAlignment.Right, ZColumnType.Integer));
			if (league.IsPoints())
				report.Columns.Add(new ZColumn("Pts", ZAlignment.Right, ZColumnType.Float));

			int teamTotalScore = 0;
			int teamTotalPoints = 0;
			int teamGames = 0;
			DateTime previousGameDate = DateTime.MinValue;
			// Add a row for each of this team"s games. Fill in values for team score and player scores.
			foreach (GameTeam gameTeam in team.GameTeams(includeSecret))
				if (from < gameTeam.Game.Time && gameTeam.Game.Time < to)
				{
				    if (gameTeam.Game.Time.Date > previousGameDate)  // We've crossed a date boundary, so
				    {
				    	ZRow dateRow = new ZRow();  // create a row
				    	report.Rows.Add(dateRow);
				    	dateRow.Add(new ZCell(gameTeam.Game.Time.ToShortDateString()));  // to show the new date.
				    }

					ZRow gameRow = new ZRow();
					report.Rows.Add(gameRow);
					var timeCell = new ZCell(gameTeam.Game.Time.ToShortTimeString());
					timeCell.Hyper = gameHyper(gameTeam.Game);
					gameRow.Add(timeCell);
					
					// Add player scores to player columns.
					foreach (LeaguePlayer leaguePlayer in players)
					{
						GamePlayer gamePlayer = gameTeam.Players.Find(x => x.LeaguePlayer == leaguePlayer);
						if (gamePlayer == null)
							gameRow.Add(new ZCell(""));
						else
							gameRow.Add(new ZCell(gamePlayer.Score, ChartType.Bar, "N0", gamePlayer.Colour.ToColor()));
					}

					gameRow.Add(new ZCell(gameTeam.Score, ChartType.Bar, "N0", gameTeam.Colour.ToColor()));
	
					teamTotalScore += gameTeam.Score;
					teamGames++;
					if (league.IsPoints())
					{
						gameRow.Add(new ZCell(gameTeam.Points, ChartType.None, "N0", gameTeam.Colour.ToColor()));

						teamTotalPoints += gameTeam.Points;
					}
					previousGameDate = gameTeam.Game.Time.Date;
				}  // if from..to; for GameList
			
			ZRow averageRow = new ZRow();
			report.Rows.Add(averageRow);
			averageRow.Add(new ZCell("Average"));
			foreach (var x in averagesList)
				averageRow.Add(new ZCell(x.Value, ChartType.Bar, "N0"));
			
			if (teamGames > 0)
			{
				averageRow.Add(new ZCell(teamTotalScore / teamGames, ChartType.Bar, "N0"));
				if (league.IsPoints())
					averageRow.Add(new ZCell(1.0 * teamTotalPoints / teamGames, ChartType.None, "N0"));
			}
			report.MaxChartByColumn = true;
//			TODO: replace the above line with report.OnCalcBar = KombiReportCalcBar; TeamLadderCalcBar;

			if (description)
				report.Description = "This report shows the team " + team.Name +" and its players.  Each row is one game.";
			
			return report;
		}  // OneTeam

		public static ZoomReport FixtureList(Fixture fixture, League league, int? teamId = null)
		{
			FixtureTeam team = teamId == null || teamId == -1 ? null : fixture.Teams.Find(x => x.Id == teamId);
			string title = team == null ? "Fixtures for " + league.Title : "Fixtures for " + team.Name + " in " + league.Title;

			ZoomReport report = new ZoomReport(title, "Time", "left");

			int maxTeams = fixture.Games.Max(x => x.Teams.Count());
			
			for (int i = 0; i < maxTeams; i++)
				report.Columns.Add(new ZColumn("Team", ZAlignment.Left, ZColumnType.String));

			foreach (FixtureGame fg in fixture.Games)
				if (team == null || fg.Teams.Keys.Contains(team))
				{
					ZRow row = new ZRow();

					row.Add(new ZCell(fg.Time.ToString("yyyy/MM/dd HH:mm")));

					foreach (var ft in fg.Teams)
					{
						ZCell teamCell = new ZCell(ft.Key.Name, ft.Value.ToColor());
		        		teamCell.Hyper = "fixture" + ft.Key.Id.ToString("D2", CultureInfo.InvariantCulture) + ".html";
						row.Add(teamCell);
						
					}

					report.Rows.Add(row);
				}

			if (team != null && team.LeagueTeam != null)
				report.Description = "This is a list of fixtures for " + team.Name + ". Their results are <a href=\"team" + team.LeagueTeam.Id.ToString("D2", CultureInfo.InvariantCulture) + ".html\">here</a>."; 

			return report;
		}

		public static ZoomReport FixtureGrid(Fixture fixture, League league)
		{
			return null;
		}

		// false discovery rate. multivariate repeated measures. logit = p/(1-p). poisson model. 
		// do some histograms of: #hits by each pack, #hits on each pack, #hitsby minus #hitson, ratios of each pack, logits of each pack, and see what's normally distributed.
		// ch 7, ch 11.5 bootstrapping, p 1034 questions of interest, ch27
		// overlapping 95% confidence intervals. If top pack's confidence interval overlaps bottom pack's interval, there are no outliers.
		// ellipse containing 95% of values on a scatter plot. Draw 45 ellipses; see if they overlap.
		/// <summary>Show overall stats for each pack, including a t test comparing it to all other packs combined.</summary>
		public static ZoomReport PackReport(List<League> leagues, List<Game> soloGames, DateTime? from, DateTime? to, bool description)
		{
			ChartType chartType = ChartType.KernelDensityEstimate;
			// First build a solo ladder, showing tag ratios for each player ID, so we can calibrate the pack report.
			var solos = new List<string>();

			foreach (var league in leagues)
				foreach (var player in league.Players)
					if (!solos.Contains(player.Id))
						solos.Add(player.Id);

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
						gameAverageSum += player.Played.Average(x => x.Game.TotalScore() / x.Game.Players.Count);
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
			var	report = new ZoomReport((leagues.Count == 1 ? leagues[0].Title + " " : "") + "Pack Report" + FromTo(games, from, to),
				                        "Rank,Pack,Rank diff,Score Ratio,t,p,Count,Tag Ratio,t,p,Tag diff,t,p,Count,",
				                        "center,left,right,right,right,right,right,right,right,right,right,right,right,right");
			report.MaxChartByColumn = true;
			report.Columns[2].ColumnType = ZColumnType.Integer;

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
				var rankDifferences = new List<double>();
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
						double scale = player.LeaguePlayer!= null && soloLadder.ContainsKey(player.LeaguePlayer.Id) ? soloLadder[player.LeaguePlayer.Id] : 1;
						gameTotalScore += 1.0 * player.Score / scale;
						if (player.HitsOn != 0)
							gameTotalScale += (double)player.HitsBy / player.HitsOn / scale;
					}
					
					foreach (var player in game.Players)
					{
						double scale = player.LeaguePlayer!= null && soloLadder.ContainsKey(player.LeaguePlayer.Id) ? soloLadder[player.LeaguePlayer.Id] : 1;
						double scoreRatio = 1.0 * player.Score / (gameTotalScore - player.Score / scale) * (game.Players.Count - 1) / scale;  // Scale this score ratio by this player's scale value, and by the average scaled scores of everyone else in the game.
						double? tagRatio = (player.HitsOn != 0 ? ((double)player.HitsBy / player.HitsOn / scale) : (double?)null);

						if (player.Pack == pack)
						{
							scoreRatios.Add(scoreRatio);
							if (tagRatio != null)
								tagRatios.Add((double)tagRatio);
							if (player.HitsBy != 0 || player.HitsOn != 0)
								tagDifferences.Add(player.HitsBy / scale - player.HitsOn);
							rankDifferences.Add((game.Players.Count - 1) / 2 - game.Players.IndexOf(player));
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

				row.Add(new ZCell(rankDifferences.Average(), chartType, "F1"));  // Average rank difference.
				row.Last().Data.AddRange(rankDifferences);
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

			report.Rows = report.Rows.OrderByDescending(x => x[8].Number).ThenByDescending(x => x[7].Number).ThenByDescending(x => x[3].Number).ToList();

			// Assign ranks.
			for (int i = 0; i < report.Rows.Count; i++)
				report.Rows[i][0].Number = i + 1;
			
			var averages = new ZRow();
			
			averages.Add(new ZCell("", Color.Gray));
			averages.Add(new ZCell("Averages", Color.Gray));
			if (report.Rows.Count > 0)
				for (int i = 2; i < report.Rows[0].Count; i++)
					if (i == 2 || i == 3 || i == 6 || i == 7 || i == 10 || i == 13)
						averages.Add(new ZCell(report.Rows.Average(row => row[i].Number), 
						                       ChartType.None, report.Rows[0][i].NumberFormat, Color.Gray));
					else
						averages.Add(new ZCell("", Color.Gray));

			report.Rows.Add(averages);
			
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
				    "In a team event, you can try to balance the teams, by removing the best pack from this team, the worst from that team, etc.";
			
//				if (something)
//			        report.Description += " Note that on Nexus or Helios, only games committed with \"Calculate scores by Torn\" selected in Preferences will show tag ratios.";
			
				if (from != null || to != null)
			        report.Description += " The report has been limited to games" + FromTo(games, from, to) + ".";
			}

			return report;
		}  // PackReport

		/// <summary>Every player in every game.</summary>
		public static ZoomReport EverythingReport(League league, DateTime? from, DateTime? to, bool description)
		{
			var	report = new ZoomReport(league.Title + " " + "Everything Report" + FromTo(league.AllGames, from, to),
				                        "Player,Pack,Team,Rank,Score,Hits by,Hits On",
				                        "left,left,left,right,right,right,right");
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
				
					row.Add(new ZCell(player.LeaguePlayer == null ? player.PlayerId : player.LeaguePlayer.Name));
					row.Add(new ZCell(player.Pack));
					row.Add(new ZCell(player.GameTeam == null || player.GameTeam.LeagueTeam == null ? " " : player.GameTeam.LeagueTeam.Name));
					row.Add(new ZCell(player.Rank));
					row.Add(new ZCell(player.Score));
					row.Add(new ZCell(player.HitsBy));
					row.Add(new ZCell(player.HitsOn));

					report.Rows.Add(row);
				}
			}

			return report;
		}  // EverythingReport
		
		static string BlankZero(int i)
		{
			if (i == 0)
				return "";
			else
				return i.ToString(CultureInfo.CurrentCulture);
		}

		static ZCell DataCell(List<double> dataList, Drops drops, ChartType chartType, string numberFormat)
		{
			var dataCell = new ZCell(0, chartType, numberFormat);
			dataCell.Data.AddRange(dataList);
			DropScores(dataList, drops);
			dataCell.Number = dataList.DefaultIfEmpty(0).Average();

			return dataCell;
		}

		static double Erf(double x)
		{
			// Abramowitz, M. and Stegun, I. (1964). _Handbook of Mathematical Functions with Formulas, Graphs, and Mathematical Tables_. p.299.
			// http://people.math.sfu.ca/~cbm/aands/page_299.htm
			// erf(x) = 1 - 1 / (a1*x +a2*x^2 + a6*x^6) + epsilon(x)
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
			{
				string s1 = " from " + (from == null ? "(none)" : ((DateTime)from).ToShortDateString());
				return s1 + " to " + (to == null ? "(none)" : ((DateTime)to).ToShortDateString());
			}
			if (games.First().Time.Date == games.Last().Time.Date)  // this report is for games on a single day
				return " for " + games.First().Time.Date.ToShortDateString();
			// this report is for games over multiple days
			return " from " + games.First().Time.ToShortDateString() + " to " + games.Last().Time.ToShortDateString();
		}

		static List<T> DropScores<T>(List<T> scores, Drops drops)
		{
			if (drops != null)
			{
				scores.Sort();

				int count = scores.Count();

				if (drops.CountAfterDrops(count) > 1 && count > 0)
				{
					T median = scores[count % 2];
					scores.Clear();
					scores.Add(median);
					return scores;
				}

				int dropBest = drops.DropBest(count);
				if (dropBest > 0)
					scores.RemoveRange(count - dropBest, dropBest);

				int dropWorst = drops.DropWorst(count);
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
		
		static void SortGridReport(League league, ZoomReport report, GridType gridType, List<Game> games, int averageCol, int pointsCol, bool reversed)
		{
			if (gridType == GridType.GameGrid) {
				report.Rows.Sort(delegate(ZRow x, ZRow y)
			                 {
			                 	double? result = 0;
			                 	if (league.IsPoints())
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
			else if (gridType == GridType.Ascension)
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
				if (league.IsPoints())
					report.RemoveColumn(report.Columns.Count - 1);  // Pts
			} 
			else if (gridType == GridType.Pyramid)
			{
				PyramidComparer pc = new PyramidComparer();
				pc.Columns = report.Columns;
				pc.Reversed = reversed;
				report.Rows.Sort(pc);
				pc.DoColor(league, report);

				report.RemoveColumn(report.Columns.Count - 1);
				if (league.IsPoints())
					report.RemoveColumn(report.Columns.Count - 1);
			}
			
			for (int i = 0; i < report.Rows.Count; i++)
				report.Rows[i][0].Number = i + 1;

			report.Title = league.Title + (gridType == GridType.Ascension ? " Ascension" : gridType == GridType.Pyramid ? " Pyramid" : " Games");
		}

		static void FinishGridReport(League league, ZoomReport report, List<Game> games, DateTime? from, DateTime? to, bool description)
		{
			report.Title += FromTo(games, from, to);

			if (description)
			{
				report.Description = "This is a grid of games. Each row in the table is one team.";

				if (games.Count == 0)
					report.Description += " No games were found in the specified date/time range.";
				else if (from != null && to != null)
					report.Description += " The report has been limited to games " + FromTo(games, from, to) + ".";
				else if (from != null)
					report.Description += " The report has been limited to games after " + games.First().Time.ToShortDateString() + ".";
				else if (to != null)
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
