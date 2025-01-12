using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using Zoom;

namespace Torn
{
	public class Finals
	{
		public ZoomReport Report { get; set; }
		public Fixture Fixture { get; set; }
		public int NumTeams { get; set; }
		public int TeamsPerGame { get; set; }
		/// <summary>Number of losing teams from each game that get sent down to the next lower track, or from the bottom track get eliminated.</summary>
		public int TeamsSentDown { get; set; }
		/// <summary>Number of "tracks" -- parallel streams of games for top teams, middle teams, bottom teams, etc.</summary>
		public int Tracks { get; set; }
		/// <summary>Number of teams that go straight to grand finals without having to play any games.</summary>
		public int FreeRides { get; set; }

		/// <summary>Add a cell to a row with a simple arrow running from left to right in the cell.</summary>
		void AddCell(int row, ZColumn col)
		{
			if (col != null)
				Report.Rows[row].AddCell(new ZCell());
		}

		void AddCells(int row, ZColumn col1, ZColumn col2)
		{
			AddCell(row, col1);
			AddCell(row, col2);
		}

		/// <summary>
		/// Add column and cells representing this game, plus an arrow showing paths leading out of this game.
		/// </summary>
		/// <param name="gameName">Title for the column.</param>
		/// <param name="topSpace">Space above this game to fill with pale horizontal arrows representing teams that haven't played their first game yet.</param>
		/// <param name="stepDown">Space above this game to fill with _dark_ horizontal arrows, representing teams that have played their first game, but aren't in this game.</param>
		/// <param name="teamsInGame">Teams that are actually playing in this game.</param>
		/// <param name="bottomSpace">Space below this game to fill with dark horizontal arrows, representing teams that have played their first game, but aren't in this game.</param>
		/// <param name="narrow">If true, add just one column, and just one columns worth of cells, with the game cells going in the column just to the left of the one we're adding. If false, add two columns, with the second being for arrows.</param>
		/// <param name="prefillArrows">If true, place simple left-right expanding arrows in the cells above the game that would otherwise be blank in this column.</param>
		/// <returns>The column that contains the game cells.</returns>
		ZColumn FillColumn(string gameName, int topSpace, int stepDown, int teamsInGame, int bottomSpace, bool narrow = false, bool prefillArrows = false)
		{
			ZColumn gameCol;
			// Add columns for game (and the arrows between games).
			if (narrow)
			{
				gameCol = Report.Columns.Last();
				gameCol.Text = gameName;
			}
			else
			{
				gameCol = new ZColumn(gameName, ZAlignment.Center, "Games");
				Report.Columns.Add(gameCol);
			}

			var arrowCol = new ZColumn(null, ZAlignment.Center, "Games");
			Report.Columns.Add(arrowCol);
			var arrow = new Arrow();
			arrowCol.Arrows.Add(arrow);

			int row = 0;
			// Add cells for the space above this game (and the arrows between games).
			for (int i = 0; i < topSpace && row < Report.Rows.Count - teamsInGame; i++, row++)
			{
				AddCells(row, narrow ? null : gameCol, arrowCol);
				if (prefillArrows)
					gameCol.AddArrow(row, 5, Color.FromArgb(0xEE, 0xEE, 0xEE), true);
			}

			// Add cells for the space which is above this game but below the teams first game, because this is a lower track.
			for (int i = 0; i < stepDown && row < Report.Rows.Count - teamsInGame; i++, row++)
			{
				AddCells(row, narrow ? null : gameCol, arrowCol);
				if (prefillArrows)
					gameCol.AddArrow(row, 5, default, true);
			}

			// Add cells for the game (and the arrows between games).
			for (int i = 0; i < teamsInGame; i++, row++)
			{
				if (narrow)
				{
					Report.Rows[row].RemoveAt(Report.Rows[row].Count - 1);
					var dupArrow = gameCol.Arrows.Find(r => r.From[0].Row == row);
					if (dupArrow != null)
						gameCol.Arrows.Remove(dupArrow);
				}

				Report.Rows[row].Add(new ZCell(" ") { Border = Color.Black });
				Report.Rows[row].Add(new ZCell());

				arrow.From.Add(new ZArrowEnd(row, 5) { Expand = true } );
				arrow.To.Add(new ZArrowEnd(row, 5) { Expand = true } );
			}

			// Add cells for the space below this game (and the arrows between games), so that lower-track games are placed correctly.
			for (int i = 0; i < bottomSpace && row < Report.Rows.Count; i++, row++)
				AddCells(row, narrow ? null : gameCol, arrowCol);

			return gameCol;
		}

		static string GameName(int game)
		{
			if (game < 26)
				return ((char)((int)'A' + game)).ToString();  // "A", "B", etc.
			if (game < 52)
				return new String(((char)((int)'a' + game - 26)), 2);  // "aa", "bb", etc.
			return (game + 1).ToString();
		}

		/// <summary>Add grand finals games.</summary>
		void GrandFinals(List<ZColumn> games)
		{
			for (int i = 0; i < TeamsPerGame; i++)
			{
				var finalCol = new ZColumn((TeamsPerGame - i).ToString(), ZAlignment.Center, "Finals");
				Report.Columns.Add(finalCol);
				if (games != null)
					games.Add(finalCol);

				for (int j = 0; j < TeamsPerGame; j++)
					Report.Rows[i].Add(new ZCell(" ", ((Colour)((j + TeamsPerGame - i) % TeamsPerGame + 1)).ToColor()) { Border = Color.Black });
			}
		}

		public void TwoTrack()
		{
			// Add columns for games.
			int game = 0;
			var games = new List<ZColumn>();

			int topSpace = FreeRides;
			while (topSpace + 2 * TeamsPerGame <= NumTeams)
				topSpace += TeamsSentDown;

			// Ensure the bottom team(s) get a game.
			if (topSpace + 2 * TeamsPerGame < NumTeams + TeamsSentDown)
			{
				games.Add(FillColumn(GameName(game), NumTeams - TeamsPerGame, 0, TeamsPerGame, 0, false, true));
				game++;
			}

			// Add a "startup" games to get things rolling.
			games.Add(FillColumn(GameName(game), topSpace, 0, TeamsPerGame, TeamsPerGame - TeamsSentDown, false, !games.Any()));
			game++;
			topSpace -= TeamsSentDown;

			// Add all the "regular" games in the middle. Note narrow = true because these are back-to-back games: no team from these games plays in the immediately previous game.
			for (; topSpace >= FreeRides; topSpace -= TeamsSentDown, game += 2)
			{
				games.Add(FillColumn(GameName(game), topSpace, 0, TeamsPerGame, TeamsPerGame, game > 2));
				games.Add(FillColumn(GameName(game + 1), topSpace, TeamsPerGame, TeamsPerGame, 0, true));
			}

			// Add the "golden games" just before finals.
			for (int stepDown = TeamsPerGame - TeamsSentDown; stepDown > 0; stepDown -= TeamsSentDown, game++)
				games.Add(FillColumn(GameName(game), FreeRides, stepDown, TeamsPerGame, 0));

			for (int row = TeamsPerGame; row < NumTeams; row++)
				Report.Rows[row].AddCell(new ZCell(Utility.Ordinate(row + 1)));

			GrandFinals(games);
			Report.SameWidths.Add(games);
		}

		public void ThreeTrack()
		{
			// Add columns for games.
			int game = 0;
			var games = new List<ZColumn>();

			int teamsSentUp = TeamsPerGame - TeamsSentDown;
			int topSpace = FreeRides;
			while (topSpace + 2 * TeamsPerGame + teamsSentUp <= NumTeams)
				topSpace += TeamsSentDown;

			// Ensure the bottom team(s) get a game.
			bool prefix = topSpace + 2 * TeamsPerGame + teamsSentUp < NumTeams + TeamsSentDown;
			if (prefix)
			{
				games.Add(FillColumn(GameName(game), NumTeams - TeamsPerGame - teamsSentUp, 0, TeamsPerGame, TeamsSentDown, false, true));
				game++;

				games.Add(FillColumn(GameName(game), NumTeams - TeamsPerGame - teamsSentUp, teamsSentUp, TeamsPerGame, 0));
				game++;
			}

			// Add a "startup" games to get things rolling.
			games.Add(FillColumn(GameName(game), topSpace, 0, TeamsPerGame, NumTeams - topSpace - TeamsPerGame, prefix));
			game++;
			topSpace -= TeamsSentDown;
			bool first = true;
			// Add all the "regular" games in the middle. Note narrow = true because these are back-to-back games: no team from these games plays in the immediately previous game.
			for (; topSpace >= FreeRides; topSpace -= TeamsSentDown, game += 3)
			{
				games.Add(FillColumn(GameName(game), topSpace + TeamsSentDown, 1, TeamsPerGame, 1, !first));
				games.Add(FillColumn(GameName(game + 1), topSpace, 0, TeamsPerGame, TeamsPerGame + teamsSentUp, true));
				games.Add(FillColumn(GameName(game + 2), topSpace, TeamsPerGame + teamsSentUp, TeamsPerGame, 0, true));
				first = false;
			}

			// Add the "golden games" just before finals.
			games.Add(FillColumn(GameName(game), FreeRides, teamsSentUp, TeamsPerGame, teamsSentUp, true));
			games.Add(FillColumn(GameName(game + 1), FreeRides, 2 * teamsSentUp, TeamsPerGame, 0));

			for (int row = TeamsPerGame; row < NumTeams; row++)
				Report.Rows[row].AddCell(new ZCell(Utility.Ordinate(row + 1)));

			GrandFinals(games);
			Report.SameWidths.Add(games);
		}

		public void GeneralAscension()
		{
			// Add columns for games.
			int col = 0;
			int game = 0;
			var games = new List<ZColumn>();

			int topSpace;
			if (Tracks == 1)
				topSpace = NumTeams - TeamsPerGame;
			else
				topSpace = NumTeams - TeamsPerGame - (Tracks - 1) * (TeamsPerGame - TeamsSentDown);  // Start the first track, with the lowest teams we can while still allowing for lower tracks.

			topSpace += (topSpace + FreeRides) % TeamsSentDown;  // Ensure topSpace is a multiple of teamsToCut, so we will line up properly when we get to finals. If not, move upwards so that it is a whole multiple.

			bool bottom = false;  // True if the bottom-most team has played a game.
			while (topSpace >= FreeRides)
			{
				for (int track = 0; track < Tracks; track++)
					if (!bottom || topSpace + TeamsPerGame + track * TeamsSentDown <= NumTeams)
					{
						string gameName = Tracks == 1 ? (topSpace + TeamsPerGame).ToString() : GameName(game);
						games.Add(FillColumn(gameName, topSpace, track * (TeamsPerGame - TeamsSentDown), TeamsPerGame, (Tracks - track - 1) * TeamsSentDown, false, game == 0));
						game++;
						bottom |= topSpace + TeamsPerGame + track * TeamsSentDown >= NumTeams;
					}
				topSpace -= TeamsSentDown;
				col++;
			}

			for (int row = TeamsPerGame; row < NumTeams; row++)
				Report.Rows[row].AddCell(new ZCell(Utility.Ordinate(row + 1)));

            if (NumTeams > 0)
                GrandFinals(games);
			Report.SameWidths.Add(games);
			Report.Description = "You may wish to rearrange games to avoid back-to-backs where teams play twice in a row.";
        }

        public static ZoomReport Ascension(Fixture fixture, int teamsPerGame, int teamsToCut, int tracks, int freeRides)
		{
			var f = new Finals
			{
				Fixture = fixture,
				NumTeams = fixture.Teams.Count,
				TeamsPerGame = teamsPerGame,
				TeamsSentDown = teamsToCut,
				Tracks = tracks,
				FreeRides = freeRides
			};

			var report = new ZoomReport("Finals");
			f.Report = report;
			report.Colors.OddColor = report.Colors.BackgroundColor;

			report.Columns.Add(new ZColumn("Teams", ZAlignment.Left));

			// Add rows to the report.
			for (int row = 0; row < fixture.Teams.Count; row++)
			{
				report.Rows.Add(new ZRow());
				report.Rows[row].AddCell(new ZCell(fixture.Teams[row].Name));
			}

			if (tracks == 2 && fixture.Teams.Count > 4)
				f.TwoTrack();
			else if (tracks == 3 && fixture.Teams.Count > 5)
				f.ThreeTrack();
			else
				f.GeneralAscension();

			return report;
		}
	}
}
