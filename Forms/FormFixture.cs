using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Torn;
using Torn.Report;

namespace Torn.UI
{
	/// <summary>
	/// Allow users to create or import tournament fixtures.
	/// </summary>
	public partial class FormFixture : Form
	{
		Holder holder;
		public Holder Holder { 
			get { return holder; }

			set { 
				holder = value;
				if (frameFinals1 != null)
					frameFinals1.Holder = holder; }
		}
		public string ExportFolder { get; set; }

		private List<List<int>> previousGrid;
		private double previousBestScore;
		private double previousGamesPerTeam;
		private bool previousHasRef;
		private List<List<int>> previousExistingPlays;

		Colour leftButton, middleButton, rightButton, xButton1, xButton2;
		Point point;  // This is the point in the grid last clicked on. It's counted in grid squares, not in pixels: 9,9 is ninth column, ninth row.
		bool resizing;

		public FormFixture()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();

			datePicker.Value = DateTime.Now.Date;
			timePicker.CustomFormat = CultureInfo.CurrentUICulture.DateTimeFormat.ShortTimePattern;
			leftButton = Colour.Red;
			middleButton = Colour.Blue;
			rightButton = Colour.Green;
			xButton1 = Colour.Yellow;
			xButton2 = Colour.Purple;
			point = new Point(-1, -1);
			resizing = false;
		}

		void ButtonClearClick(object sender, EventArgs e)
		{
			Holder.Fixture.Games.Clear();
			displayReportGames.Report = null;
			displayReportGrid.Report = null;
		}

		void LogGrid<T>(List<List<T>> grid)
		{
			string str = "**********\n";
			foreach (List<T> list in grid)
			{
				foreach (T item in list)
				{
					str += item + ",\t";
				}
				str += "\n";
			}
			str += "**********";
			Console.WriteLine(str);
		}

		List<T> FlattenGrid<T>(List<List<T>> grid)
		{
			List<T> flat = new List<T>();
			foreach (List<T> list in grid)
			{
				flat.AddRange(list);
			}
			return flat;
		}

		List<List<T>> TransposeGrid<T>(List<List<T>> grid)
        {
			return grid.SelectMany(inner => inner.Select((item, index) => new { item, index }))
				.GroupBy(i => i.index, i => i.item)
				.Select(g => g.ToList())
				.ToList();
		}

		List<List<int>> GetGrid(double numberOfTeams, double teamsPerGame, double gamesPerTeam, bool hasRef, List<List<int>> existingPlays, int maxMillis)
		{
			List<List<int>> bestGrid = SetupGrid(numberOfTeams, teamsPerGame, gamesPerTeam);
			double bestScore = CalcScore(bestGrid, gamesPerTeam, hasRef, existingPlays);

			return ContinueMixing(bestGrid, gamesPerTeam, hasRef, existingPlays, maxMillis, bestScore);

		}

		List<List<int>> ContinueMixing(List<List<int>> grid, double gamesPerTeam, bool hasRef, List<List<int>> existingPlays, int maxMillis, double startingScore)
        {
			List<List<int>> bestGrid = grid;
			double bestScore = startingScore;
			bool badFixture = bestScore > 10000;

			Console.WriteLine("initScore: {0}", bestScore);
			LogGrid(bestGrid);

			Stopwatch sw = new Stopwatch();
			sw.Start();
			scoreLabel.Visible = false;
			scoreLabel.Text = "";

			for (int i = 0; i < 20000; i++)
			{
				List<List<int>> mixedGrid = MixGrid(bestGrid);
				double score = CalcScore(mixedGrid, gamesPerTeam, hasRef, existingPlays);
				if (score <= bestScore)
				{
					bestScore = score;
					bestGrid = mixedGrid;
					if (bestScore > 10000)
					{
						badFixture = true;
					}
					else
					{
						badFixture = false;
					}
				} 
			}

			while (badFixture)
			{
				if (sw.ElapsedMilliseconds > maxMillis)
					break;
				List<List<int>> mixedGrid = MixGrid(bestGrid);
				double score = CalcScore(mixedGrid, gamesPerTeam, hasRef, existingPlays);
				if (score <= bestScore)
				{
					bestScore = score;
					bestGrid = mixedGrid;
					if (bestScore > 10000)
					{
						badFixture = true;
					}
					else
					{
						badFixture = false;
					}
				}
			}
			Console.WriteLine("Time Elapsed (ms): {0}", sw.ElapsedMilliseconds);

			Console.WriteLine("bestScore: {0}", bestScore);
			LogGrid(bestGrid);
			LogGrid(CalcPlays(bestGrid, hasRef, existingPlays));
			scoreLabel.Text = bestScore >= 10000 ? "Extend max time and regenerate: " + Math.Round(bestScore).ToString() : "Score: " + Math.Round(bestScore).ToString() + " (Lower is better)";
			scoreLabel.BackColor = bestScore >= 10000 ? Color.FromName("red") : Color.Transparent;
			scoreLabel.Visible = true;

			previousGrid = bestGrid;
			previousBestScore = bestScore;
			previousGamesPerTeam = gamesPerTeam;
			previousHasRef = hasRef;
			previousExistingPlays = existingPlays;
			continueGenerating.Enabled = true;


			return bestGrid;
		}

		List<List<int>> MixGrid(List<List<int>> grid)
        {
			List<List<int>> newGrid = grid.ConvertAll(row => row.ConvertAll(cell => cell));
			double teamsPerGame = grid[0].Count;
			double totalTeams = FlattenGrid(grid).Uniq().Count;
			Random rnd = new Random();

			int game1 = rnd.Next(grid.Count);
			int game2 = rnd.Next(grid.Count);

			int player1 = rnd.Next((int)teamsPerGame);
			int player2 = rnd.Next((int)teamsPerGame);

			while ((totalTeams / teamsPerGame) > 1 && game1 == game2)
			{
				game2 = rnd.Next(grid.Count - 1);
			}

			newGrid[game1][player1] = grid[game2][player2];
			newGrid[game2][player2] = grid[game1][player1];

			return newGrid;
        }

		List<int> SumRows(List<List<int>> grid )
        {
			List<int> totals = new List<int>();
			foreach(List<int> row in grid)
            {
				int total = 0;
				foreach(int value in row)
                {
					total += value;
                }
				totals.Add(total);
            }
			return totals;
        }

		int AverageRow(List<int> row)
		{
			int total = 0;
			foreach (int value in row)
			{
				total += value;
			}

			return total / row.Count;
				
		}

		double CalcScore(List<List<int>> grid, double gamesPerTeam, bool hasRef, List<List<int>> existingPlays)
		{
			int BACK_TO_BACK_PENALTY = 10;
			int totalTeams = FlattenGrid(grid).Uniq().Count;
			int teamsPerGame = grid[0].Count;
			double previousAveragePlays = AverageRow(SumRows(existingPlays)) / totalTeams;
			List<List<int>> plays = CalcPlays(grid, hasRef, existingPlays);
			double averagePlays = (((gamesPerTeam - (hasRef ? 1 : 0)) * (teamsPerGame  - (hasRef ? 1 : 0)) * (totalTeams - 1)) / (totalTeams * totalTeams)) + previousAveragePlays;

			double score = 0;

			for (int player1 = 0; player1 < plays.Count; player1++)
            {
				score += plays[player1][player1] * 10000; // penalty for playing themselves
				for(int player2 = player1 + 1; player2 < plays[player1].Count; player2++)
                {
					score += Math.Pow(plays[player1][player2] - averagePlays, 4);
                }
            }

			 List<List<int>> transposedGrid = TransposeGrid(grid);

			//penalty for same colour
			foreach (List<int> colour in transposedGrid)
            {
				int numberOfGames = colour.Count;
				int numberOfColours = transposedGrid.Count;
				int uniqueTeams = colour.Uniq().Count;
				double gamesOnEachColour = gamesPerTeam / numberOfColours;
				int penalties = colour.FindAll(team => colour.FindAll(t => t == team).Count > gamesOnEachColour).Count;

				double penalty = penalties * 10000;
				score += penalty;
            }

			foreach(List<int> row in grid)
            {
				int uniquePlayers = row.Uniq().Count;
				score += Math.Abs(uniquePlayers - teamsPerGame) * 100000; // penalty for playing themselves
			}

			//back to back
			for (int game = 0; game < grid.Count - 1; game++)
			{
				for (int player1 = 0; player1 < teamsPerGame; player1++)
				{
					for (int player2 = 0; player2 < teamsPerGame; player2++)
					{
						foreach(int player in grid[game])
                        {
							List<int> backToBacks = grid[game + 1].FindAll(p => p == player);
							score += BACK_TO_BACK_PENALTY * backToBacks.Count;
                        }
					}
				}
			}

			return score;
		}

		List<List<int>> AddGrids(List<List<int>> grid1, List<List<int>> grid2)
        {
			
			if (grid1.Count > 0 && grid1.Count == grid2.Count && grid1[0].Count == grid2[0].Count)
			{
				List<List<int>> grid3 = new List<List<int>>();
				for (int i = 0; i < grid1.Count; i++)
				{
					List<int> row = new List<int>();
					for (int j = 0; j < grid1[i].Count; j++)
					{
						if(grid2.Count > i && grid2[i].Count > j)
                        {
							row.Add(grid1[i][j] + grid2[i][j]);
						} else
                        {
							row.Add(grid1[i][j]);
                        }
						
					}
					grid3.Add(row);
				}
				return grid3;
			} else
            {
				return grid1;
            }
        }

		List<List<int>> CalcPlays(List<List<int>> grid, bool hasRef, List<List<int>> existingPlays)
        {
			List<List<int>> newGrid = grid.ConvertAll(row => row.ConvertAll(cell => cell));
			// ignore ref games
			if (hasRef)
            {
				List<List<int>> transposedGrid = TransposeGrid(newGrid);
				transposedGrid.RemoveAt(transposedGrid.Count - 1);
				newGrid = TransposeGrid(transposedGrid);
            }

			int totalTeams = FlattenGrid(newGrid).Uniq().Count;
			List<List<int>> plays = new List<List<int>>();
			for(int team1 = 0; team1 < totalTeams; team1++)
            {
				List<int> row = new List<int>();
				for (int team2 = 0; team2 < totalTeams; team2++)
				{
					if (team1 == team2)
					{
						List<List<int>> games = newGrid.FindAll(g => g.FindAll(t => t == team1).Count > 1);
						row.Add(games.Count);
					}
					else
					{
						List<List<int>> games = newGrid.FindAll(g => g.Contains(team1) && g.Contains(team2));
						row.Add(games.Count);
					}
				}
				plays.Add(row);
			}

			return AddGrids(plays, existingPlays);
		}


		List<List<int>> SetupGrid (double numberOfTeams, double teamsPerGame, double gamesPerTeam)
        {
			double numGames = (numberOfTeams / teamsPerGame) * gamesPerTeam;

			// set up grid
			List<int> arr = Enumerable.Repeat(-1, (int) teamsPerGame * (int)Math.Ceiling(numGames)).ToList();
			int index = 0;
			List<int> updatedArr = new List<int>();
			foreach (int el in arr)
			{
				updatedArr.Add((int)(index % numberOfTeams));
				index++;
			}
			List<List<int>> grid = updatedArr.ChunkBy((int)teamsPerGame);

			return grid;

		}

		public List<List<int>> GetLeagueGrid(League league)
		{
			List<List<int>> grid = new List<List<int>>();
			foreach(Game game in league.AllGames)
            {
				List<int> row = new List<int>();
				foreach(GameTeam team in game.Teams)
                {
					row.Add(team.TeamId - 1 ?? -1);
                }
				grid.Add(row);
            }

			return grid;
		}

		public List<List<int>> PadPlaysToTeamNumber(int numberOfTeams, List<List<int>> plays)
        {
			Console.WriteLine(numberOfTeams);
			List<List<int>> newGrid = plays.ConvertAll(row => row.ConvertAll(cell => cell));
			int teamPlays = newGrid.Count;
			if (numberOfTeams > teamPlays)
            {
				for(int i = 0; i < teamPlays; i++)
                {
					for(int j = teamPlays; j < numberOfTeams; j++)
                    {
						newGrid[i].Add(0);
                    }
				}
				for(int i = teamPlays; i < numberOfTeams; i++)
                {
					List<int> arr = Enumerable.Repeat(0, numberOfTeams).ToList();
					newGrid.Add(arr);
				}

				return newGrid;
			} else
            {
				return plays;
            }
        }

		double TeamsPerGame()
        {
			double teamsPerGame = 0;
			teamsPerGame += red.Checked ? 1 : 0;
			teamsPerGame += yellow.Checked ? 1 : 0;
			teamsPerGame += green.Checked ? 1 : 0;
			teamsPerGame += blue.Checked ? 1 : 0;
			teamsPerGame += referee.Checked ? 1 : 0;
			return teamsPerGame;
		}

		string TeamColours()
		{
			string colours = "";
			colours += red.Checked ? "1," : "";
			colours += blue.Checked ? "2," : "";
			colours += yellow.Checked ? "4," : "";
			colours += green.Checked ? "3," : "";
			colours += referee.Checked ? "17," : "";
			return colours;
		}

		void ButtonImportTeamsClick(object sender, EventArgs e)
		{
			buttonImportTeams.Text = "Generating...";
			buttonImportTeams.Enabled = false;
			continueGenerating.Enabled = false;
			Holder.Fixture.Teams.Clear();
			Holder.Fixture.Teams.Parse(textBoxTeams.Text, Holder.League);
			Holder.Fixture.Games.Clear();
			double numberOfTeams = Holder.Fixture.Teams.Count;
			bool hasRef = referee.Checked;
			double teamsPerGame = TeamsPerGame();
			double gamesPerTeam = (double) gamesPerTeamInput.Value + (hasRef ? (double) gamesPerTeamInput.Value / (teamsPerGame - 1) : 0);
			int maxMillis = (int)maxTime.Value * 1000;

			List<List<int>> existingGrid = GetLeagueGrid(Holder.League);
			List<List<int>> existingPlays = CalcPlays(existingGrid, false, new List<List<int>>());
			List<List<int>> existingPlaysPadded = PadPlaysToTeamNumber((int)numberOfTeams, existingPlays);

			LogGrid(existingPlaysPadded);


			List<List<int>> grid = GetGrid(numberOfTeams, teamsPerGame, gamesPerTeam, hasRef, existingPlaysPadded, maxMillis);


			Holder.Fixture.Games.Parse(grid, Holder.Fixture.Teams, timePicker.Value, TimeSpan.FromMinutes((double)numericMinutes.Value), TeamColours());


			textBoxTeams.Text = Holder.Fixture.Teams.ToString();
			textBoxGames.Text = Holder.Fixture.Games.ToString();
			textBoxGrid.Lines = Holder.Fixture.Games.ToGrid(Holder.Fixture.Teams);
			if(outputGrid.Checked && outputList.Checked)
            {
				reportTeamsList.Report = Reports.FixtureCombined(Holder.Fixture, Holder.League);
			} else if(outputList.Checked)
            {
				reportTeamsList.Report = Reports.FixtureList(Holder.Fixture, Holder.League);
			}
			else if (outputGrid.Checked)
			{
				reportTeamsList.Report = Reports.FixtureGrid(Holder.Fixture, Holder.League);
			}
			buttonImportTeams.Text = "Generate";
			buttonImportTeams.Enabled = true;
		}

		private void ContinueGenerateClick(object sender, EventArgs e)
		{
			Holder.Fixture.Teams.Clear();
			Holder.Fixture.Teams.Parse(textBoxTeams.Text, Holder.League);
			Holder.Fixture.Games.Clear();
			buttonImportTeams.Text = "Generating...";
			buttonImportTeams.Enabled = false;
			continueGenerating.Enabled = false;
			int maxMillis = (int)maxTime.Value * 1000;
			List<List<int>> grid = ContinueMixing(previousGrid, previousGamesPerTeam, previousHasRef, previousExistingPlays, maxMillis, previousBestScore);

			Holder.Fixture.Games.Parse(grid, Holder.Fixture.Teams, timePicker.Value, TimeSpan.FromMinutes((double)numericMinutes.Value), TeamColours());

			textBoxTeams.Text = Holder.Fixture.Teams.ToString();
			textBoxGames.Text = Holder.Fixture.Games.ToString();
			textBoxGrid.Lines = Holder.Fixture.Games.ToGrid(Holder.Fixture.Teams);
			if (outputGrid.Checked && outputList.Checked)
			{
				reportTeamsList.Report = Reports.FixtureCombined(Holder.Fixture, Holder.League);
			}
			else if (outputList.Checked)
			{
				reportTeamsList.Report = Reports.FixtureList(Holder.Fixture, Holder.League);
			}
			else if (outputGrid.Checked)
			{
				reportTeamsList.Report = Reports.FixtureGrid(Holder.Fixture, Holder.League);
			}
			buttonImportTeams.Text = "Generate";
			buttonImportTeams.Enabled = true;
		}

		void ButtonImportGamesClick(object sender, EventArgs e)
		{
			Holder.Fixture.Games.Clear();

			bool fromLeague = (ModifierKeys.HasFlag(Keys.Control) && ModifierKeys.HasFlag(Keys.Shift)) ||
				(ModifierKeys.HasFlag(Keys.Control) && ModifierKeys.HasFlag(Keys.Alt)) ||
				(ModifierKeys.HasFlag(Keys.Shift) && ModifierKeys.HasFlag(Keys.Alt));

			if (fromLeague)
				Holder.Fixture.Games.Parse(Holder.League, Holder.Fixture.Teams);
			else
			{
				if (radioButtonTab.Checked)
					Holder.Fixture.Games.Parse(textBoxGames.Text, Holder.Fixture.Teams, '\t');
				else if (textBoxSeparator.Text.Length > 0)
					Holder.Fixture.Games.Parse(textBoxGames.Text, Holder.Fixture.Teams, textBoxSeparator.Text[0]);
			}

			textBoxGames.Text = Holder.Fixture.Games.ToString();
			textBoxGrid.Lines = Holder.Fixture.Games.ToGrid(Holder.Fixture.Teams);
			displayReportGames.Report = Reports.FixtureList(Holder.Fixture, Holder.League);
			displayReportGrid.Report = Reports.FixtureGrid(Holder.Fixture, Holder.League);
		}

		void ButtonImportGridClick(object sender, EventArgs e)
		{
			textBoxGrid.Lines = Holder.Fixture.Games.Parse(textBoxGrid.Lines, Holder.Fixture.Teams, 
			                                        datePicker.Value.Date + timePicker.Value.TimeOfDay, 
			                                        TimeSpan.FromMinutes((double)numericMinutes.Value));
			textBoxGames.Text = Holder.Fixture.Games.ToString();
			displayReportGames.Report = Reports.FixtureList(Holder.Fixture, Holder.League);
			displayReportGrid.Report = Reports.FixtureGrid(Holder.Fixture, Holder.League);
		}

		private void ButtonExportClick(object sender, EventArgs e)
		{
			ExportPages.ExportFixture(ExportFolder, Holder);
			MessageBox.Show("Fixture exported to " + System.IO.Path.Combine(ExportFolder, Holder.Key), "Fixture Exported");
		}

		List<LeagueTeam> Ladder()
		{
			// Find the most appropriate report template showing the date range, drop games, etc.
			var rt = Holder.ReportTemplates.Find(r => r.ReportType == ReportType.TeamLadder) ?? 
				Holder.ReportTemplates.Find(r => r.ReportType == ReportType.MultiLadder) ??
				Holder.ReportTemplates.Find(r => r.ReportType == ReportType.GameByGame) ?? 
				Holder.ReportTemplates.Find(r => r.ReportType == ReportType.GameGrid || r.ReportType == ReportType.GameGridCondensed) ??
				Holder.ReportTemplates.Find(r => r.ReportType == ReportType.Pyramid || r.ReportType == ReportType.PyramidCondensed) ??
				Holder.ReportTemplates.FirstOrDefault() ??
				new ReportTemplate() { ReportType = ReportType.TeamLadder };

			List<Game> games = Holder.League.Games(true).Where(g => g.Time > (rt.From ?? DateTime.MinValue) && g.Time < (rt.To ?? DateTime.MaxValue)).ToList();
			return games != null && games.Any() ? Reports.Ladder(Holder.League, games, rt).Select(tle => tle.Team).ToList() : null;
		}

		void FormFixtureShown(object sender, EventArgs e)
		{
			if (Holder.Fixture != null)
			{
				var ladder = Ladder();
				var teams = ladder != null && ladder.Any() ? ladder : Holder.League.Teams;

				if (Holder.Fixture.Teams.Count == 0)
				{
					foreach (var lt in teams)
					{
						Holder.Fixture.Teams.Add(new FixtureTeam
						{
							LeagueTeam = lt,
							Name = lt.Name
						}
						);
					}
				}
				else
				{
					var comparer = new TeamComparer() { LeagueTeams = ladder };
					Holder.Fixture.Teams.Sort(comparer);
				}

				textBoxTeams.Text = Holder.Fixture.Teams.ToString();
				if (Holder.Fixture.Games.Any())
				{
					displayReportGames.Report = Reports.FixtureList(Holder.Fixture, Holder.League);
					displayReportGrid.Report = Reports.FixtureGrid(Holder.Fixture, Holder.League);
				}
				else
				{
					displayReportGames.Report = null;
					displayReportGrid.Report = null;
				}

				if (Holder.Fixture.Games.Any())
				{
					textBoxGames.Text = Holder.Fixture.Games.ToString();
					textBoxGrid.Lines = Holder.Fixture.Games.ToGrid(Holder.Fixture.Teams);
				}
			}
		}

		void TextBoxKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Control && e.KeyCode == Keys.A &&sender != null)
	        	((TextBox)sender).SelectAll();
		}

		// TODO: turn panelGraphic into a custom control that takes a Holder.Fixture as a property, so it can manage its own painting, clicks, etc.
		void FillCell(int row, int col, int size, Color color)
		{
			var g = panelGraphic.CreateGraphics();
			g.FillRectangle(new SolidBrush(color), new Rectangle(col * size + 1, row * size + 1, size - 1, size - 1));
		}

		void PaintNumbers(int size, int rows)
		{
			var difficulties = new float[rows];
			var counts = new int[rows];
			var averages = new float[rows];
			var games = Holder.Fixture.Games;

			foreach (var fg in games)
				foreach (var ft in fg.Teams)
					if (0 < ft.Key.Id() && ft.Key.Id() < difficulties.Length)
					{
						difficulties[ft.Key.Id() - 1] += (fg.Teams.Sum(x => x.Key.Id()) - ft.Key.Id()) / (fg.Teams.Count - 1F);
						counts[ft.Key.Id() - 1]++;
					}
			
			for (int row = 0; row < rows; row++)
				if (counts[row] > 0)
					averages[row] = difficulties[row] / counts[row];
				else
					averages[row] = float.NaN;
			
			float max = averages.Count() == 0 ? 1 : averages.Max();

			var g = panelGraphic.CreateGraphics();
			var font = new Font("Arial", size - 2);
			g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
			Pen pen = new Pen(Color.Black);

			g.FillRectangle(new SolidBrush(Color.White), games.Count * size + 1, 0, games.Count * size, rows * size);  // Erase right-hand side text etc.

			// Draw difficulty text and chart.
			for (int row = 0; row < rows; row++)
				if (!float.IsNaN(averages[row]))
				{
					g.DrawString(counts[row].ToString() + "  " + averages[row].ToString("N2"), font, Brushes.Black, games.Count * size, row * size - 2);
					float x = games.Count * size + 50 + averages[row] / max * 100;
					g.DrawLine(pen, x, row * size, x, row * size + size - 1);
				}

			g.FillRectangle(new SolidBrush(Color.White), 0, rows * size + 1, games.Count * size, size);  // Erase bottom text.

			for (int col = 0; col < games.Count; col++)
				g.DrawString(games[col].Teams.Count.ToString(), font, Brushes.Black, col * size - 1, rows * size);
		}

		void PaintWhoPlaysWho(int size)
		{
			int left = Holder.Fixture.Games.Count * size + 150;

			var g = panelGraphic.CreateGraphics();
			var font = new Font("Arial", size - 2);
			g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
			Pen pen = new Pen(Color.Black);

			// Paint diagonal.
			for (int x = 0; x < Holder.Fixture.Teams.Count; x++)
				g.FillRectangle(new SolidBrush(Color.FromArgb(0xF8, 0xF8, 0xF8)), left + x * size, x * size, size, size);

				// Colour squares for selected team and game.
				if (Holder.Fixture.Games.Valid(point.X))
				{
				var game = Holder.Fixture.Games[point.X];
				var team = Holder.Fixture.Teams[point.Y];

				// Highlight cells for selected team.
				for (int x = 0; x < Holder.Fixture.Teams.Count; x++)
					if (x != point.Y && Holder.Fixture.Games.Count(fg => fg.Teams.ContainsKey(Holder.Fixture.Teams[x]) && fg.Teams.ContainsKey(team)) != 0)
					{
						g.FillRectangle(new SolidBrush(Color.FromArgb(0xE0, 0xE0, 0xE0)), left + x * size, point.Y * size, size, size);
						g.FillRectangle(new SolidBrush(Color.FromArgb(0xE0, 0xE0, 0xE0)), left + point.Y * size, x * size, size, size);
					}

				// Colour cells that represent teams that the selected team plays against in this game.
				foreach (var kv in game.Teams)
				{
					g.FillRectangle(new SolidBrush(kv.Value.ToColor()), left + (kv.Key.Id() - 1) * size, (team.Id() - 1) * size, size, size);
					g.FillRectangle(new SolidBrush(kv.Value.ToColor()), left + (team.Id() - 1) * size, (kv.Key.Id() - 1) * size, size, size);
				}
			}

			// Put numbers in squares.
			for (int x = 0; x < Holder.Fixture.Teams.Count; x++)
				for (int y = 0; y < Holder.Fixture.Teams.Count; y++)
					if (x != y)
					{
						int sum = Holder.Fixture.Games.Count(fg => fg.Teams.ContainsKey(Holder.Fixture.Teams[x]) && fg.Teams.ContainsKey(Holder.Fixture.Teams[y]));
						if (sum != 0)
							g.DrawString(sum.ToString(), font, Brushes.Black, left + x * size, y * size - 2);
					}
		}

		void PaintBorder(int size, int rows)
		{
			var g = panelGraphic.CreateGraphics();
			Pen pen = new Pen(Color.Gray);

			g.DrawLine(pen, 0, 0, Math.Min(Holder.Fixture.Games.Count * size, panelGraphic.DisplayRectangle.Right), 0);
			g.DrawLine(pen, 0, rows * size, Math.Min(Holder.Fixture.Games.Count * size, panelGraphic.DisplayRectangle.Right), rows * size);

			g.DrawLine(pen, 0, 0, 0, Math.Min(rows * size, panelGraphic.DisplayRectangle.Bottom));
			g.DrawLine(pen, Holder.Fixture.Games.Count * size, 0, Holder.Fixture.Games.Count * size, Math.Min(rows * size, panelGraphic.DisplayRectangle.Bottom));
		}

		void PaintGrid(int size, int rows)
		{
			var g = panelGraphic.CreateGraphics();
			Pen pen = new Pen(Color.Gray);

			for (int row = 0; row <= rows; row++)
				g.DrawLine(pen, 0, row * size, Math.Min(Holder.Fixture.Games.Count * size, panelGraphic.DisplayRectangle.Right), row * size);

			for (int col = 0; col <= Holder.Fixture.Games.Count; col++)
				g.DrawLine(pen, col * size, 0, col * size, Math.Min(rows * size, panelGraphic.DisplayRectangle.Bottom));

//			// Paint a light gray bar on the row and column of the clicked team. Not in use because we'd have to rebuild grid every PanelGraphicMouseClick.
//			for (int x = 0; x < Holder.Fixture.Games.Count; x++)
//				for (int y = 0; y < Holder.Fixture.Teams.Count; y++)
//					if (point != null && (point.Y == x || point.Y == y))
//						FillCell(x, y, size, Color.LightGray);
		}

		// Paint coloured cells onto grid to show teams in games.
		void PaintCells(int size)
		{
			for (int col = 0; col < Holder.Fixture.Games.Count; col++)
			{
				var fg = Holder.Fixture.Games[col];
				foreach (var x in fg.Teams)
				{
					var row = Holder.Fixture.Teams.IndexOf(x.Key);
					if (row != -1)
					{
						if (x.Value == Colour.None)
							FillCell(row, col, size, Color.Black);
						else
							FillCell(row, col, size, x.Value.ToSaturatedColor());
					}
				}
			}
		}

		void PanelGraphicPaint(object sender, PaintEventArgs e)
		{
			int size = (int)numericSize.Value;
			int rows = Holder.Fixture.Teams.Count;

			if (resizing)
			{
				PaintBorder(size, rows);
				return;
			}

			PaintGrid(size, rows);
			PaintCells(size);
			PaintNumbers(size, rows);
			PaintWhoPlaysWho(size);

			// Paint palette.
			for (Colour i = Colour.None; i < Colour.White; i++)
				FillCell(rows + 2, (int)i, size, i.ToSaturatedColor());
		}

        void NumericSizeValueChanged(object sender, EventArgs e)
		{
			panelGraphic.Invalidate();
		}

		void PanelGraphicMouseClick(object sender, MouseEventArgs e)
		{
			int size = (int)numericSize.Value;
			point = panelGraphic.PointToClient(Cursor.Position);
			point = new Point(point.X / size, point.Y / size);
			int rows = Holder.Fixture.Teams.Count;

			if (point.X < Holder.Fixture.Games.Count && point.Y < rows)
			{
				if (Holder.Fixture.Games[point.X].Teams.ContainsKey(Holder.Fixture.Teams[point.Y]))
				{
					Holder.Fixture.Games[point.X].Teams.Remove(Holder.Fixture.Teams[point.Y]);
					FillCell(point.Y, point.X, size, Color.White);
				}
				else
				{
					Colour c = Colour.None;
					switch (e.Button) {
						case MouseButtons.Left: c = leftButton;	    break;
						case MouseButtons.Right: c = rightButton;	break;
						case MouseButtons.Middle: c = middleButton;	break;
						case MouseButtons.XButton1: c = xButton1;	break;
						case MouseButtons.XButton2: c = xButton2;	break;
					}
					Holder.Fixture.Games[point.X].Teams.Add(Holder.Fixture.Teams[point.Y], c);
					FillCell(point.Y, point.X, size, c.ToSaturatedColor());
				}

				PaintNumbers(size, rows);
				PaintWhoPlaysWho(size);
			}

			else
			{
				if (point.Y == rows + 2 && point.X > 0 && point.X < 9)
				{
					var c = (Colour)(point.X);
					switch (e.Button) {
						case MouseButtons.Left: leftButton = c;	    break;
						case MouseButtons.Right: rightButton = c;	break;
						case MouseButtons.Middle: middleButton = c;	break;
						case MouseButtons.XButton1: xButton1 = c;	break;
						case MouseButtons.XButton2: xButton2 = c;	break;
					}
				}

				point = new Point(-1, -1);
			}
		}

		private void TabControl1Selected(object sender, TabControlEventArgs e)
		{
			if (e.Action == TabControlAction.Selected && e.TabPage == tabPyramid)
				TabPyramidSelected();
			if (e.Action == TabControlAction.Selected && e.TabPage == tabPyramidRound)
				TabPyramidRoundSelected();
		}

		void FormFixtureResizeBegin(object sender, EventArgs e)
		{
			resizing = true;
		}

		void FormFixtureResizeEnd(object sender, EventArgs e)
		{
			resizing = false;
			panelGraphic.Invalidate();
		}

        Pyramid Pyramid = new Pyramid();
		private void TabPyramidSelected()
		{
			if (numericPyramidTeams.Value == 1 && numericPyramidDesiredTeamsPerGame.Value == 1)
			{
				string s = Holder.League.Title;
				if (Holder.League.Teams.Count > 3)
				{
					numericPyramidTeams.Value = Holder.League.Teams.Count;
					if (Holder.League.Games(true).Count > 0)
                        numericPyramidDesiredTeamsPerGame.Value = (int)Math.Round(Holder.League.Games(true).Average(g => g.Teams.Count));
					else
						numericPyramidDesiredTeamsPerGame.Value = (int)Math.Round(Math.Sqrt(Holder.League.Teams.Count));
                }
				else if (s.IndexOf("Solo", StringComparison.OrdinalIgnoreCase) >= 0)
				{
					numericPyramidTeams.Value = 160;
					numericPyramidDesiredTeamsPerGame.Value = 20;
				}
				else if (s.IndexOf("Double", StringComparison.OrdinalIgnoreCase) >= 0)
				{
					numericPyramidTeams.Value = 56;
					numericPyramidDesiredTeamsPerGame.Value = 8;
				}
				else
                {
                    numericPyramidTeams.Value = 42;
                    numericPyramidDesiredTeamsPerGame.Value = 6;
                }
            }
            NumericPyramidRoundsValueChanged(null, null);
            ButtonIdealiseClick(null, null);
        }

        private void NumericPyramidRoundsValueChanged(object sender, EventArgs e)
        {
			Pyramid.Rounds.Clear();

            pyramidRound1.Visible = numericPyramidRounds.Value >= 1;
            pyramidRound2.Visible = numericPyramidRounds.Value >= 2;
            pyramidRound3.Visible = numericPyramidRounds.Value >= 3;

            if (numericPyramidRounds.Value >= 1)
                Pyramid.Rounds.Add(pyramidRound1);

            if (numericPyramidRounds.Value >= 2)
                Pyramid.Rounds.Add(pyramidRound2);

            if (numericPyramidRounds.Value >= 3)
                Pyramid.Rounds.Add(pyramidRound3);

            RefreshPyramidFixture();
        }

        private void NumericPyramidGamesPerTeamValueChanged(object sender, EventArgs e)
		{
			pyramidRound1.RoundGamesPerTeam = (int)numericPyramidGamesPerTeam.Value;
			RefreshPyramidFixture();
		}

		private void NumericPyramidTeamsValueChanged(object sender, EventArgs e)
		{
			pyramidRound1.TeamsIn = (int)numericPyramidTeams.Value;
            RefreshPyramidFixture();
		}

		private void PyramidRoundValueChanged(object sender, EventArgs e)
		{
			int i;
            for (i = 0; i < Pyramid.Rounds.Count - 1; i++)
                Pyramid.Rounds[i + 1].TeamsIn = Pyramid.Rounds[i].TeamsOut;

            labelPyramidFinalsTeams.Text = Pyramid.Rounds[i].TeamsOut.ToString();
            RefreshPyramidFixture();
		}

		private void NumericPyramidFinalsGamesValueChanged(object sender, EventArgs e)
		{
			RefreshPyramidFixture();
		}

		void RefreshPyramidFixture()
		{
			if (Pyramid.Rounds.Count > 0)
			{
				displayReportPyramid.Report = Pyramid.Report(Holder.League.Title, (int)numericPyramidFinalsGames.Value, Pyramid.Rounds.Last().TeamsOut);
				textDescription.Text = displayReportPyramid.Report.Description;
			}
		}

		private void ButtonIdealiseClick(object sender, EventArgs e)
		{
			var x = Pyramid.Idealise((int)numericPyramidDesiredTeamsPerGame.Value, (int)numericPyramidTeams.Value);
			labelAdvancePercent.Text = String.Format("{0:0.00%}", x);
				
		}

		const int ColTitle = 1;
		const int ColNumTeams = 2;
		const int ColTeamsToTake = 3;
		const int ColPriority = 4;
		const int ColSecret = 5;

		League PreviousLeague;
		private void TabPyramidRoundSelected()
		{
			listViewGames.BeginUpdate();
			try
			{
				if (PreviousLeague != Holder.League)
					listViewGames.Items.Clear();
				PreviousLeague = Holder.League;

				// Add or update items in the list view, one for each game.
				foreach (Game leagueGame in Holder.League.AllGames)
				{
					string key = leagueGame.Time.ToString("o");
					ListViewItem[] matches = listViewGames.Items.Find(key, false);
					ListViewItem item = matches.Any() ? matches[0] : listViewGames.Items.Add(new ListViewItem { Name = key });

					item.Text = leagueGame.ServerGame?.InProgress ?? false ? "In Progress" : leagueGame.Time.FriendlyDateTime();
					item.Tag = new PyramidGame() { Game = leagueGame };
					while (item.SubItems.Count < listViewGames.Columns.Count)
						item.SubItems.Add("");

					item.SubItems[ColTitle].Text = leagueGame.Title;  // Description
					item.SubItems[ColNumTeams].Text = leagueGame.Teams.Count.ToString();  // # teams
					item.SubItems[ColSecret].Text = leagueGame.Secret ? "Y" : "";  // Secret?
				}
			}
			finally
			{
				listViewGames.EndUpdate();
			}
		}

		private void PyramidValueChanged(object sender, EventArgs e)
		{
			decimal numberOfTeams = numericTeamsFromLastRound.Value + numericTeamsFromLastRepechage.Value;
			labelNumberOfTeams.Text = numberOfTeams.ToString();
			labelTeamsPerGame.Text = (numberOfTeams / numericGames.Value).ToString("N2");
			RefreshPyramidDraw();
		}

		private void ListViewGamesDoubleClick(object sender, EventArgs e)
		{
			ButtonEditPyramidGamesClick(sender, e);
		}

		readonly FormPyramidGame formPyramidGame = new FormPyramidGame();

		private void ButtonRepechageClick(object sender, EventArgs e)
		{
			textBoxTitle.Text = "Repêchage ";
			textBoxTitle.Focus();
			textBoxTitle.SelectionStart = 10;
		}

		private void PyramidSpinKeyUp(object sender, KeyEventArgs e)
		{
			var _ = ((NumericUpDown)sender).Value;  // This piece of black magic forces the control's ValueChanged to fire after the user edits the text in the control.
		}

        private void ButtonEditPyramidGamesClick(object sender, EventArgs e)
		{
			if (listViewGames.SelectedItems.Count == 0)
				return;

			var games = new List<PyramidGame>();
			foreach (ListViewItem item in listViewGames.SelectedItems)
				games.Add((PyramidGame)item.Tag);

			formPyramidGame.Games = games;
			if (formPyramidGame.ShowDialog() == DialogResult.OK)
			{
				foreach (ListViewItem item in listViewGames.SelectedItems)
				{
					PyramidGame pg = (PyramidGame)item.Tag;
					pg.TeamsToTake = formPyramidGame.TeamsToTake;
					pg.Priority = formPyramidGame.Priority;

					while (item.SubItems.Count < listViewGames.Columns.Count)
						item.SubItems.Add("");

					item.SubItems[ColTeamsToTake].Text = formPyramidGame.TeamsToTake.ToString();
					item.SubItems[ColPriority].Text = formPyramidGame.Priority.ToString();
				}

				RefreshPyramidDraw();
				CalculateSpins();
			}
		}

		private void ButtonClearPyramidGames(object sender, EventArgs e)
		{
			foreach (ListViewItem item in listViewGames.SelectedItems)
			{
				PyramidGame pg = (PyramidGame)item.Tag;
				pg.TeamsToTake = null;
				pg.Priority = Priority.Unmarked;

				while (item.SubItems.Count < listViewGames.Columns.Count)
					item.SubItems.Add("");

				item.SubItems[ColTeamsToTake].Text = "";
				item.SubItems[ColPriority].Text = "";
			}
			RefreshPyramidDraw();
			CalculateSpins();
		}

		private void RefreshPyramidDraw()
		{
			var pyramidGames = new List<PyramidGame>();
			foreach (ListViewItem item in listViewGames.Items)
			{
				var pg = (PyramidGame)item.Tag;
				if (pg.Priority != Priority.Unmarked)
					pyramidGames.Add((PyramidGame)item.Tag);
			}

			var pr = new PyramidDraw() { CompareRank = radioCompareRank.Checked, TakeTop = radioTakeTop.Checked };

			(displayReportTaken.Report, displayReportDraw.Report) = pr.Reports(pyramidGames, Holder.League, (int)numericGames.Value, 
				(int)numericTeamsFromLastRound.Value, (int)numericTeamsFromLastRepechage.Value, textBoxTitle.Text, checkBoxColour.Checked);
		}

		private void CalculateSpins()
		{
			int roundTeams = 0;
			int repechageTeams = 0;
			foreach (ListViewItem item in listViewGames.Items)
			{
				PyramidGame pg = (PyramidGame)item.Tag;
				int take = pg.TeamsToTake == null ? pg.Game.Teams.Count : (int)pg.TeamsToTake;

				if (pg.Priority == Priority.Round)
					roundTeams += take;
				else if (pg.Priority == Priority.Repechage)
					repechageTeams += take;
			}

			numericTeamsFromLastRound.Value = roundTeams;
			numericTeamsFromLastRepechage.Value = repechageTeams;
		}
	}
}

class TeamComparer : IComparer<FixtureTeam>
{
	/// <summary>Sorted list of league teams.</summary>
	public List<LeagueTeam> LeagueTeams { get; set; }

	public int Compare(FixtureTeam x, FixtureTeam y)
	{
		if (LeagueTeams == null)
			return 0;

		int ix = LeagueTeams.FindIndex(lt => lt == x.LeagueTeam);
		int iy = LeagueTeams.FindIndex(lt => lt == y.LeagueTeam);
		if (ix == -1) ix = 999999;
		if (iy == -1) iy = 999999;
		return ix - iy;
	}
}

public static class ListExtensions
{
	public static List<List<T>> ChunkBy<T>(this List<T> source, int chunkSize)
	{
		return source
			.Select((x, i) => new { Index = i, Value = x })
			.GroupBy(x => x.Index / chunkSize)
			.Select(x => x.Select(v => v.Value).ToList())
			.ToList();
	}
	public static List<T> Uniq<T>(this List<T> source)
    {
		List<T> result = new List<T>();

		foreach(T el in source)
        {
			bool exists = result.Contains(el);
			if (!exists)
				result.Add(el);
        }
		return result;

    }
}