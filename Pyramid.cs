﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Torn5.Controls;
using Zoom;

namespace Torn
{
	/// <summary>Creates a pyramid fixture with several rounds and repêchages. Outputs a report showing that fixture.</summary>
	class Pyramid
	{
		public List<PyramidFixture> Rounds { get; } = new List<PyramidFixture>();
		public ZoomReport Report(string title, int finalsGames, int finalsTeams)
		{
			var report = new ZoomReport(title + " Pyramid");
			report.Colors.BackgroundColor = Color.Empty;
			report.Colors.OddColor = Color.Empty;

			var gameColumns = new List<ZColumn>();
			report.SameWidths.Add(gameColumns);

			var col = 0;
			for (int i = 0; i < Rounds.Count; i++)
			{
				col = AddRound(report, "Round " + Rounds[i].Round, gameColumns, col, null, Rounds[i].FixtureRound, Rounds[i].FixtureRepechage);
				if (Rounds[i].HasRepechage)
					col = AddRound(report, "Rep " + Rounds[i].Round, gameColumns, col, Rounds[i].FixtureRound, Rounds[i].FixtureRepechage, i == Rounds.Count - 1 ? null : Rounds[i + 1].FixtureRound);
			}

			for (int i = 0; i < finalsGames; i++)
			{
				gameColumns.Add(report.AddColumn(new ZColumn(((char)((int)'A' + i)).ToString(), ZAlignment.Center, "Finals")));

				var cell = report.Rows.Force(0).Force(i + col);
				cell.TextColor = Color.LightGray;
				cell.Color = Color.White;
				cell.Border = Color.Black;
				cell.Text = finalsTeams.ToString();
			}

			var sb = new StringBuilder();

			foreach (var round in Rounds)
			{
				sb.Append(round.Description());
				sb.Append("\r\n\r\n\r\n");
			}
			report.Description = sb.ToString();

			return report;
		}

		int AddRound(ZoomReport report, string title, List<ZColumn> gameColumns, int col, PyramidHalfFixture previousRound, PyramidHalfFixture thisRound, PyramidHalfFixture nextRound)
		{
			if (thisRound.Games == 0)
				return col;

			gameColumns.Add(report.AddColumn(new ZColumn(thisRound.Games.ToString() + " games", ZAlignment.Center, title)));

			var arrowColumn = report.AddColumn(new ZColumn("", ZAlignment.Center, ""));
			var arrow = new Arrow();  // This arrow shows teams leaving this round, and skipping ahead, going to next round or repechage, or being eliminated.
			arrowColumn.Arrows.Add(arrow);

			if (!thisRound.IsRound)  // This is a repechage
				arrow.From.Add(new ZArrowEnd(0, Math.Min(previousRound.Advance, 5)));  // so add a From for teams traveling directly from previous round.

			int startRow = thisRound.IsRound ? 0 : 1;
			for (int i = 0; i < thisRound.Games; i++)
			{
				var row = report.Rows.Force(i + startRow);
				var cell = row.Force(col);
				cell.TextColor = Color.LightGray;
				cell.Color = Color.White;
				cell.Border = Color.Black;
				cell.Text = ((thisRound.TeamsIn * thisRound.GamesPerTeam + thisRound.Games - i - 1) / thisRound.Games).ToString();
				arrow.From.Add(new ZArrowEnd(i + startRow, Math.Min((double)thisRound.TeamsIn / thisRound.Games, 5)));
			}

			if (nextRound == null)
				arrow.To.Add(new ZArrowEnd(0, Math.Min(thisRound.Advance + (previousRound?.Advance ?? 0), 5)));
			else
			{
				if (!nextRound.IsRound)  // Next is a repechage
					arrow.To.Add(new ZArrowEnd(0, Math.Min(thisRound.Advance, 5)) { Expand = true });  // so add a To for teams traveling directly to next round.

				int startRow2 = nextRound.IsRound ? 0 : 1;
				double teamsOut = nextRound.IsRound ? (previousRound?.Advance ?? 0) + thisRound.Advance : thisRound.TeamsOut;
				for (int i = 0; i < nextRound.Games; i++)
					arrow.To.Add(new ZArrowEnd(i + startRow2, Math.Min(teamsOut / nextRound.Games, 5)));
			}

			if (nextRound == null || nextRound.IsRound)  // This is an elimination round
			{
				arrow.To.Add(new ZArrowEnd(thisRound.Games + 1, Math.Min(thisRound.TeamsIn - thisRound.Advance, 5)));  // so add an arrow for eliminated teams
				var row = report.Rows.Force(thisRound.Games + 1);
				var cell = row.Force(col + 2);  // and a cell representing that elimination.
				cell.Text = "X";
			}

			return col + 2;
		}

		/// <summary>
		/// Set all rounds and repêchages to a similar advance percentage; i.e. all games are approximately the same difficulty.
		/// Number of teams and games in each round and repêchage are adjusted accordingly.
		/// </summary>
		public double Idealise(int desiredTeamsPerGame, int teams)
		{
            double advanceRatePerRound = Math.Pow((double)desiredTeamsPerGame / teams, 1.0 / Rounds.Count);
            double advanceRatePerPartRound = 1 - Math.Sqrt(1 - advanceRatePerRound);

			for (int i = 0; i < Rounds.Count; i++)
			{
				Rounds[i].Idealise(desiredTeamsPerGame, advanceRatePerPartRound);

				if (i < Rounds.Count - 1)
					Rounds[i + 1].TeamsIn = Rounds[i].TeamsOut;
			}

			return advanceRatePerPartRound;
		}
	}

	class PyramidDraw
	{
		/// <summary>
		/// If true, compare teams on rank, then on victory points then score.
		/// Otherwise, compare on victory points then score then rank. 
		/// </summary>
		public bool CompareRank { get; set; }

		/// <summary>If true, take best teams from each game. If false, take worst teams from each game.</summary>
		public bool TakeTop { get; set; }

		public League League { get; set; }

		/// <summary>Build a list of at most (teamsFromRound) teams from the previous round followed by at most (teamsFromRepechage) teams from the previous repechage.</summary>
		ListTeamGames BuildList(List<PyramidGame> pyramidGames, int teamsFromRound, int teamsFromRepechage)
		{
			var roundList = new ListTeamGames();
			var repechageList = new ListTeamGames();

			BuildOneList(roundList, pyramidGames.Where(p => p.Priority == Priority.Round), teamsFromRound);
			BuildOneList(repechageList, pyramidGames.Where(p => p.Priority == Priority.Repechage), teamsFromRepechage);

			roundList.AddRange(repechageList);  // Add the repechage games into the main list.
			return roundList;
		}

		void BuildOneList(ListTeamGames list, IEnumerable<PyramidGame> pyramidGames, int length)
		{
			foreach (var pg in pyramidGames)
				if (pg.TeamsToTake is int take)
					list.AddN(League, pg.Game, TakeTop, pg.Priority, 0, take);

			GetRest(list, pyramidGames, length);
		}

		void GetRest(ListTeamGames list, IEnumerable<PyramidGame> pyramidGames, int length)
		{
			var comparer = new TeamGamesComparer() { CompareOnRank = CompareRank };
			list.Sort(comparer);

			// If we already have too many items in the list, remove some.
			int activeCount = list.Count(t => League.LeagueTeam(t.TeamId).Active);
			if (activeCount > length)
			{
				if (TakeTop)
					list.RemoveRange(length, activeCount - length);
				else
					list.RemoveRange(0, length);
			}

			// If we don't have enough items, add some from any games which don't specify how many teams to take from that game.
			else if (activeCount < length)
			{
				// Build the tail list: all the teams that didn't get brought in before.
				var tail = new ListTeamGames();
				foreach (var pg in pyramidGames)
					tail.AddN(League, pg.Game, TakeTop, pg.Priority, pg.TeamsToTake ?? 0, pg.Game.Teams.Count);

				tail.Sort(comparer);

				// Build the take list: enough active teams to get us up to length, plus any withdrawn teams we come across while we do that.
				var take = new ListTeamGames();

				int toAdd = length - list.Count(t => League.LeagueTeam(t.TeamId).Active);
				int i = 0;
				while (take.Count(t => League.LeagueTeam(t.TeamId).Active) < toAdd && i < tail.Count)
				{
					take.Add(tail[TakeTop ? i : tail.Count - i - 1]);
					i++;
				}
				take.Sort(comparer);

				// Then add in the take.
				list.AddRange(take);
			}
		}

		ListTeamGames TeamsNotTaken(ListTeamGames taken, IEnumerable<PyramidGame> pyramidGames)
		{
			var notTaken = new ListTeamGames();

			foreach (var pg in pyramidGames)
				foreach (var gt in pg.Game.Teams)
					if (!taken.Any(tg => tg.TeamId == gt.TeamId))
						notTaken.Add(League, pg.Game, gt, Priority.Unmarked);

			notTaken.Sort(new TeamGamesComparer() { CompareOnRank = CompareRank });

			return notTaken;
		}

		private static readonly Random random = new Random();

		/// <summary>
		/// Generate two reports. The past report highlights the teams which will advance. The draw report shows which teams are in which game for the next round or repechage.
		/// Each report has its data populated from teamsGames.
		/// </summary>
		// TODO: test withdrawing teams of various ladder positions to make sure it works in all cases, in both rounds and repechages.
		public (ZoomReport past, ZoomReport draw) Reports(List<PyramidGame> pyramidGames, int games, int teamsFromRound, int teamsFromRepechage, string title, bool colour)
		{
			var teamsGames = BuildList(pyramidGames, teamsFromRound, teamsFromRepechage);

			// Build the "past" report showing which teams made the cut: one row per team, one game per column.
			var pastReport = new Zoom.ZoomReport("From", "Rank,Team", "center,left");

			var times = pyramidGames.Select(pg => pg.Game.Time);
			bool oneDay = times.Any() && times.Min().Date == times.Max().Date;

			// Add columns for each game used from pyramidGames.
			foreach (var pg in pyramidGames.Where(pg => pg.Priority != Priority.Unmarked))
				pastReport.Columns.Add(new Zoom.ZColumn(pg.Game.Time.ToShortTimeString(), Zoom.ZAlignment.Left, pg.Game.Title + " " + (oneDay ? "" : Utility.FriendlyDate(pg.Game.Time))) { Tag = pg });

			pastReport.Columns.Add(new ZColumn("Average", ZAlignment.Right));

			// Put all the rows in the past report. If we're taking the top teams, list them first; otherwise, list them last.
			if (TakeTop)
				AddPastRows(pastReport, teamsGames);
			AddPastRows(pastReport, TeamsNotTaken(teamsGames, pyramidGames));
			if (!TakeTop)
				AddPastRows(pastReport, teamsGames);

			for (int r = 0; r < pastReport.Rows.Count; r++)
				pastReport.Rows[r][0].Number = r + 1;  // Rank.

			// Build the Draw report: one future game per column.
			var drawReport = new Zoom.ZoomReport(title);

			for (int col = 1; col <= games; col++)
				drawReport.Columns.Add(new Zoom.ZColumn("Game " + col.ToString(), Zoom.ZAlignment.Left));

			var activeTeams = teamsGames.Where(t => League.LeagueTeam(t.TeamId).Active).ToList();

			// Pull off teamGamesList into drawReport, either from the low end or the high end.
			int i = 0;
			while (i < activeTeams.Count)
			{
				var row = drawReport.AddRow(new Zoom.ZRow());

				if (i + games <= activeTeams.Count)  // This is a complete row.
					for (int j = 0; j < games; j++)
					{
						LeagueTeam team;
						int rowNum = drawReport.Rows.Count;
						if (rowNum % 2 == 1)
							team = League.LeagueTeam(activeTeams[i].TeamId);  // Zig.  Run this row forward: team1 in game 1, etc.
						else
							team = League.LeagueTeam(activeTeams[rowNum * games - j - 1].TeamId);  // Zag.  Run this row backwards: first team in last game, etc.

						if (team != null)
							row.Add(new Zoom.ZCell(team.Name));
						else
							row.Add(new Zoom.ZCell("---"));

						i++;
					}
				else  // This is the final, partial row.
					for (int j = 0; j < games; j++)
					{
						if (i < activeTeams.Count)
						{
							LeagueTeam team = League.LeagueTeam(activeTeams[i].TeamId);
							if (team != null)
								row.Add(new Zoom.ZCell(team.Name));
							else
								row.Add(new Zoom.ZCell("--"));
						}
						else
							row.Add(new Zoom.ZCell("-"));

						i++;
					}
			}

			if (colour)
			{
				List<Colour> colours = new List<Colour>() { Colour.Red, Colour.Blue, Colour.Green, Colour.Yellow, Colour.Cyan, Colour.Pink, Colour.Purple, Colour.Orange };  // This array has the hardest to distinguish colours last, so they get used the least.
				var coloursUsed = pyramidGames.SelectMany(pg => pg.Game.Teams.Select(t => t.Colour)).Distinct();

				colours.RemoveAll(c => !coloursUsed.Any(u => u == c));

				for (int col = 0; col < drawReport.Columns.Count; col++)  // For each column,
					for (int rowGroup = 0; rowGroup < drawReport.Rows.Count; rowGroup += colours.Count)  // For each group of rows,
					{
						int remaining = Math.Min(drawReport.Rows.Count - rowGroup, colours.Count);
						var shuffledColours = colours.Take(remaining).OrderBy(x => random.Next()).ToList();  // create shuffled colours,
						for (int row = 0; row < remaining; row++)
							drawReport.Rows[rowGroup + row][col].Color = shuffledColours[row].ToColor();  // and assign them to cells.
					}
			}

			return (pastReport, drawReport);
		}

		void AddPastRows(ZoomReport pastReport, ListTeamGames teamGamesList)
		{
			bool hasPoints = teamGamesList.Any(tg => tg.AveragePoints() > 0);

			foreach (var teamGames in teamGamesList)
			{
				var row = pastReport.AddRow(new Zoom.ZRow());
				row.Add(new Zoom.ZCell(0));  // Blank-for-now Rank.

				var team = League.LeagueTeam(teamGames.TeamId);
				var priorities = teamGames.Select(tg => tg.Priority).Distinct();

				var color = !team.Active ? Priority.Withdrawn.ToColor() :
							priorities.Count() == 1 ? priorities.First().ToColor() :
							Color.Red;

				row.Add(new Zoom.ZCell(team.Name, color));  // Team

				foreach (var pg in pastReport.Columns.Where(c => c.Tag is PyramidGame).Select(c => c.Tag as PyramidGame))
				{
					var teamGame = teamGames.Find(tg => tg.Game == pg.Game);
					if (teamGame == null)
						row.Add(new Zoom.ZCell());
					else
						row.Add(FillOnePastCell(teamGame, hasPoints, teamGame.Priority.ToColor()));
				}

				row.Add(FillOnePastCell(teamGames.AverageRank(), teamGames.AverageScore(), teamGames.AveragePoints(), hasPoints, default));
			}
		}

		ZCell FillOnePastCell(TeamGame teamGame, bool hasPoints, Color color)
		{
			var cell = new ZCell(teamGame.GameTeam.Score) { Color = color };

			if (CompareRank || hasPoints)
			{
				string format = CompareRank && hasPoints ? "{0}: {1}; {2}" :
					CompareRank ? "{0}: {1}" :
					hasPoints ? "{1}; {2}" : "";

				cell.Text = string.Format(format, Utility.Ordinate(teamGame.Game.Rank(teamGame.GameTeam)), teamGame.GameTeam.Score, teamGame.GameTeam.Points);
			}

			return cell;
		}

		ZCell FillOnePastCell(double rank, double score, double points, bool hasPoints, Color color)
		{
			var cell = new ZCell(score, ChartType.Bar, "n1") { Color = color };

			if (CompareRank || hasPoints)
			{
				string format = CompareRank && hasPoints ? "{0}: {1}; {2}" :
					CompareRank ? "{0}: {1}" :
					hasPoints ? "{1}; {2}" : "";

				cell.Text = string.Format(format, rank, score, points);
			}

			return cell;
		}
	}

	/// <summary>Used to mark from which games teams were taken for future rounds. Teams from games marked Round are ranked higher into 
	/// the next round than teams from games marked Repêchage, etc. Plan B is for future development and is not currently used.</summary>
	public enum Priority { Withdrawn = 0, Unmarked, PlanB, Repechage, Round };

	public static class PriorityExtensions
	{
		public static string ToString(Priority p)
		{
			string[] names = { "Withdrawn", "Unmarked", "Plan B", "Repêchage", "Round" };

			return names.Valid((int)p) ? names[(int)p] : "?";
		}

		public static Color ToColor(this Priority priority)
		{
			Color[] Colors = { Color.FromArgb(0xFF, 0xA0, 0xA0), Color.Empty, Color.FromArgb(0xFF, 0xD0, 0xA0), Color.FromArgb(0xFF, 0xFF, 0x90), Color.FromArgb(0xA0, 0xFF, 0xA0) };  // Red, None, Orange, Yellow, Green.

			return Colors.Valid((int)priority) ? Colors[(int)priority] : Color.Empty;
		}
	}

	/// <summary>A game from the pyramid round form's listViewGames.</summary>
	public class PyramidGame
	{
		public Game Game { get; set; }
		public int? TeamsToTake { get; set; }
		public Priority Priority { get; set; }
	}

	/// <summary>A single team from a single PyramidGame.</summary>
	internal class TeamGame
	{
		public Game Game { get; set; }
		public GameTeam GameTeam { get; set; }
		public Priority Priority { get; set; }
	}

	/// <summary>Games (and corresponding GameTeam's within each Game) for a single team.</summary>
	internal class TeamGames : List<TeamGame>
	{
		public int TeamId { get; set; }

		public double AverageScore() => this.Any() ? this.Average(tg => tg.GameTeam.Score) : double.MinValue;
		public double AveragePoints() => this.Any() ? this.Average(tg => tg.GameTeam.Points) : double.MinValue;
		public double AverageRank() => this.Any() ? this.Average(tg => tg.Game.Teams.FindIndex(gt => gt.TeamId == TeamId)) + 1 : double.MaxValue;
	}

	internal class ListTeamGames : List<TeamGames>
	{
		public void Add(League league, Game game, GameTeam gt, Priority priority)
		{
			var teamGame = Find(tg => tg.TeamId == gt.TeamId);
			if (teamGame == null && gt.TeamId != null)  // If this team doesn't have an entry yet,
				Add(teamGame = new TeamGames() { TeamId = (int)gt.TeamId });  // make one and add it.

			var team = league.LeagueTeam(teamGame.TeamId);
			if (!team.Active)
				priority = Priority.Withdrawn;

			teamGame.Add(new TeamGame() { Game = game, GameTeam = gt, Priority = priority });  // Add a game to this team.
		}

		/// <summary>Add _n_ teams from this game to our list, skipping over the first _skip_ teams.</summary>
		public void AddN(League league, Game game, bool topFirst, Priority priority, int skip, int n)
		{
				if (topFirst) // Take the top N.
				for (int i = skip; i < skip + n && i < game.Teams.Count; i++)
					Add(league, game, game.Teams[i], priority);
			else  // Take the bottom N.
				for (int i = game.Teams.Count - skip - 1; i >= game.Teams.Count - skip - n && i >= 0; i--)
					Add(league, game, game.Teams[i], priority);
		}
	}

	class TeamGamesComparer : IComparer<TeamGames>
	{
		/// <summary>Sorted list of league teams.</summary>
		public List<LeagueTeam> LeagueTeams { get; set; }
		public bool CompareOnRank { get; set; }

		/// If CompareOnRank then compare teams on rank, then on victory points then score.
		/// Otherwise, compare on victory points then score then rank.
		public int Compare(TeamGames x, TeamGames y)
		{
			double result = CompareOnRank ? x.AverageRank() - y.AverageRank() : 0;

			if (result == 0)
				result = y.AveragePoints() - x.AveragePoints();

			if (result == 0)
				result = y.AverageScore() - x.AverageScore();

			if (result == 0)
				result = x.AverageRank() - y.AverageRank();

			return Math.Sign(result);
		}
	}
}
