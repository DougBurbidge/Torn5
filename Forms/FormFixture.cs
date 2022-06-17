using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Torn;
using Torn.Report;

namespace Torn.UI
{
	/// <summary>
	/// Allow users to import tournament fixtures.
	/// </summary>
	public partial class FormFixture : Form
	{
		public Holder Holder { get; set; }
		
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

		void ButtonImportTeamsClick(object sender, EventArgs e)
		{
			Holder.Fixture.Teams.Clear();
			Holder.Fixture.Teams.Parse(textBoxTeams.Text, Holder.League);
			textBoxTeams.Text = Holder.Fixture.Teams.ToString();
		}

		void ButtonImportGamesClick(object sender, EventArgs e)
		{
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

				if (Holder.League.AllGames.Any())
					numericTeamsPerGame.Value = (decimal)Math.Round(Holder.League.AllGames.Average(g => g.Teams.Count));
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

		private void RefreshFinals(object sender, EventArgs e)
		{
			labelTeamsToSendUp.Text = "Teams to send up from each game: " + (numericTeamsPerGame.Value - numericTeamsToCut.Value).ToString();

			displayReportFinals.Report = Finals.Ascension(Holder.Fixture, (int)numericTeamsPerGame.Value, (int)numericTeamsToCut.Value, (int)numericTracks.Value, (int)numericFreeRides.Value);
			printReportFinals.Report = displayReportFinals.Report;
			printReportFinals.Image = displayReportFinals.BackgroundImage;
		}

		private void NumericTeamsPerGameValueChanged(object sender, EventArgs e)
		{
			numericTeamsToCut.Maximum = numericTeamsPerGame.Value - 1;
			numericFreeRides.Maximum = numericTeamsPerGame.Value - 1;

			RefreshFinals(sender, e);
		}

		private void ButtonAscensionClick(object sender, EventArgs e)
		{
			numericTracks.Value = 1;
			numericTeamsToCut.Value = 1;
			numericFreeRides.Value = 1;
		}

		private void ButtonTwoTrackClick(object sender, EventArgs e)
		{
			numericTracks.Value = 2;
			numericTeamsToCut.Value = 2;
			numericFreeRides.Value = 0;
		}

		private void ButtonFormatDClick(object sender, EventArgs e)
		{
			numericTracks.Value = 3;
			numericTeamsToCut.Value = 2;
			numericFreeRides.Value = 0;
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

		private void PyramidSpinChanged(object sender, EventArgs e)
		{
			decimal numberOfTeams = numericTeamsFromLastRound.Value + numericTeamsFromLastRepechage.Value;
			labelNumberOfTeams.Text = numberOfTeams.ToString();
			if (numericGames.Value > 0)
				labelTeamsPerGame.Text = (numberOfTeams / numericGames.Value).ToString("N2");
			RefreshPyramid();
		}

		private void ListViewGamesDoubleClick(object sender, EventArgs e)
		{
			ButtonEditPyramidGamesClick(sender, e);
		}

		readonly FormPyramidGame formPyramidGame = new FormPyramidGame();

		private void LabelRoundTitleDoubleClick(object sender, EventArgs e)
		{
			textBoxTitle.Text = "Repêchage ";
		}

		private void PyramidSpinKeyUp(object sender, KeyEventArgs e)
		{
			var v = ((NumericUpDown)sender).Value;  // This piece of black magic forces the control's ValueChanged to fire after the user edits the text in the control.
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

				RefreshPyramid();
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
			RefreshPyramid();
			CalculateSpins();
		}

		private void RefreshPyramid()
		{
			// Build a list of all the teams from the previous round followed by all the teams from the previous repechage.
			// Then pull off that list into the report, either from the low end or the high end.

			var roundList = new ListTeamGames();
			var repechageList = new ListTeamGames();

			var pyramidGames = new List<PyramidGame>();
			foreach (ListViewItem item in listViewGames.Items)
				pyramidGames.Add((PyramidGame)item.Tag);

			var pr = new PyramidRound() { CompareRank = radioCompareRank.Checked, TakeTop = radioTakeTop.Checked };

			pr.BuildOneList(roundList, pyramidGames.Where(p => p.Priority == Priority.Round), (int)numericTeamsFromLastRound.Value);
			pr.BuildOneList(repechageList, pyramidGames.Where(p => p.Priority == Priority.Repechage), (int)numericTeamsFromLastRepechage.Value);

			roundList.AddRange(repechageList);  // Add the repechage games into the main list.

			displayReportTaken.Report = pr.PastReport(roundList, pyramidGames, Holder.League);
			displayReportDraw.Report = pr.DrawReport(roundList, pyramidGames, Holder.League, (int)numericGames.Value, textBoxTitle.Text);
		}

		private void CalculateSpins()
		{
			int roundTeams = 0;
			int repechageTeams = 0;
			foreach (ListViewItem item in listViewGames.Items)
			{
				PyramidGame pg = (PyramidGame)item.Tag;
				if (pg.TeamsToTake is int take)
				{
					if (pg.Priority == Priority.Round)
						roundTeams += take;
					else if (pg.Priority == Priority.Repechage)
						repechageTeams += take;
				}
				else
				{
					if (pg.Priority == Priority.Round)
						roundTeams += pg.Game.Teams.Count;
					else if (pg.Priority == Priority.Repechage)
						repechageTeams += pg.Game.Teams.Count;
				}
			}
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