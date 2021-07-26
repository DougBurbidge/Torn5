using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Svg;
using Torn;
using Zoom;

namespace Torn.UI
{
	/// <summary>
	/// Allow users to import tournament fixtures.
	/// </summary>
	public partial class FormFixture : Form
	{
		public Fixture Fixture { get; private set; }
		public League League { get; private set; }
		
		Colour leftButton, middleButton, rightButton, xButton1, xButton2;
		Point point;  // This is the point in the grid last clicked on. It's counted in grid squares, not in pixels: 9,9 is ninth column, ninth row.
		bool resizing;
		SvgDocument document;
		double aspectRatio = 1.0;

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

		public FormFixture(Fixture fixture, League league): this()
		{
			Fixture = fixture;
			League = league;
		}

		void ButtonClearClick(object sender, EventArgs e)
		{
			Fixture.Games.Clear();
		}

		void ButtonImportTeamsClick(object sender, EventArgs e)
		{
			Fixture.Teams.Clear();
			Fixture.Teams.Parse(textBoxTeams.Text, League);
			textBoxTeams.Text = Fixture.Teams.ToString();
		}

		void ButtonImportGamesClick(object sender, EventArgs e)
		{
			bool fromLeague = (ModifierKeys.HasFlag(Keys.Control) && ModifierKeys.HasFlag(Keys.Shift)) ||
				(ModifierKeys.HasFlag(Keys.Control) && ModifierKeys.HasFlag(Keys.Alt)) ||
				(ModifierKeys.HasFlag(Keys.Shift) && ModifierKeys.HasFlag(Keys.Alt));

			if (fromLeague)
			{
				Fixture.Games.Parse(League, Fixture.Teams);
			}
			else
			{
				if (radioButtonTab.Checked)
					Fixture.Games.Parse(textBoxGames.Text, Fixture.Teams, '\t');
				else if (textBoxSeparator.Text.Length > 0)
					Fixture.Games.Parse(textBoxGames.Text, Fixture.Teams, textBoxSeparator.Text[0]);
			}

			textBoxGames.Text = Fixture.Games.ToString();
			textBoxGrid.Lines = Fixture.Games.ToGrid(Fixture.Teams);
		}

		void ButtonImportGridClick(object sender, EventArgs e)
		{
			textBoxGrid.Lines = Fixture.Games.Parse(textBoxGrid.Lines, Fixture.Teams, 
			                                        datePicker.Value.Date + timePicker.Value.TimeOfDay, 
			                                        TimeSpan.FromMinutes((double)numericMinutes.Value));
			textBoxGames.Text = Fixture.Games.ToString();
		}

		void FormFixtureShown(object sender, EventArgs e)
		{
			if (Fixture != null)
			{
				if (Fixture.Teams.Count == 0)
					foreach (var lt in League.Teams)
					{
						var ft = new FixtureTeam();
						ft.LeagueTeam = lt;
						ft.Name = lt.Name;
						Fixture.Teams.Add(ft);
					}

				textBoxTeams.Text = Fixture.Teams.ToString();

				if (Fixture.Games.Any())
				{
					textBoxGames.Text = Fixture.Games.ToString();
					textBoxGrid.Lines = Fixture.Games.ToGrid(Fixture.Teams);
				}
			}
		}

		void TextBoxKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Control && e.KeyCode == Keys.A &&sender != null)
	        	((TextBox)sender).SelectAll();
		}

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

			foreach (var fg in Fixture.Games)
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

			g.FillRectangle(new SolidBrush(Color.White), Fixture.Games.Count * size + 1, 0, Fixture.Games.Count * size, rows * size);  // Erase right-hand side text etc.

			// Draw difficulty text and chart.
			for (int row = 0; row < rows; row++)
				if (!float.IsNaN(averages[row]))
				{
					g.DrawString(counts[row].ToString() + "  " + averages[row].ToString("N2"), font, Brushes.Black, Fixture.Games.Count * size, row * size - 2);
					float x = Fixture.Games.Count * size + 50 + averages[row] / max * 100;
					g.DrawLine(pen, x, row * size, x, row * size + size - 1);
				}

			g.FillRectangle(new SolidBrush(Color.White), 0, rows * size + 1, Fixture.Games.Count * size, size);  // Erase bottom text.

			for (int col = 0; col < Fixture.Games.Count; col++)
				g.DrawString(Fixture.Games[col].Teams.Count.ToString(), font, Brushes.Black, col * size - 1, rows * size);
		}

		void PaintWhoPlaysWho(int size, int rows)
		{
			int left = Fixture.Games.Count * size + 150;

			var g = panelGraphic.CreateGraphics();
			var font = new Font("Arial", size - 2);
			g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
			Pen pen = new Pen(Color.Black);

			// Paint diagonal.
			for (int x = 0; x < Fixture.Teams.Count; x++)
				g.FillRectangle(new SolidBrush(Color.FromArgb(0xF8, 0xF8, 0xF8)), left + x * size, x * size, size, size);

				// Colour squares for selected team and game.
				if (point.X > -1 && point.X < Fixture.Games.Count)
				{
				var game = Fixture.Games[point.X];
				var team = Fixture.Teams[point.Y];

				// Highlight cells for selected team.
				for (int x = 0; x < Fixture.Teams.Count; x++)
					if (x != point.Y && Fixture.Games.Count(fg => fg.Teams.ContainsKey(Fixture.Teams[x]) && fg.Teams.ContainsKey(team)) != 0)
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
			for (int x = 0; x < Fixture.Teams.Count; x++)
				for (int y = 0; y < Fixture.Teams.Count; y++)
					if (x != y)
					{
						int sum = Fixture.Games.Count(fg => fg.Teams.ContainsKey(Fixture.Teams[x]) && fg.Teams.ContainsKey(Fixture.Teams[y]));
						if (sum != 0)
							g.DrawString(sum.ToString(), font, Brushes.Black, left + x * size, y * size - 2);
					}
		}

		void PaintBorder(int size, int rows)
		{
			var g = panelGraphic.CreateGraphics();
			Pen pen = new Pen(Color.Gray);

			g.DrawLine(pen, 0, 0, Math.Min(Fixture.Games.Count * size, panelGraphic.DisplayRectangle.Right), 0);
			g.DrawLine(pen, 0, rows * size, Math.Min(Fixture.Games.Count * size, panelGraphic.DisplayRectangle.Right), rows * size);

			g.DrawLine(pen, 0, 0, 0, Math.Min(rows * size, panelGraphic.DisplayRectangle.Bottom));
			g.DrawLine(pen, Fixture.Games.Count * size, 0, Fixture.Games.Count * size, Math.Min(rows * size, panelGraphic.DisplayRectangle.Bottom));
		}

		void PaintGrid(int size, int rows)
		{
			var g = panelGraphic.CreateGraphics();
			Pen pen = new Pen(Color.Gray);

			for (int row = 0; row <= rows; row++)
				g.DrawLine(pen, 0, row * size, Math.Min(Fixture.Games.Count * size, panelGraphic.DisplayRectangle.Right), row * size);

			for (int col = 0; col <= Fixture.Games.Count; col++)
				g.DrawLine(pen, col * size, 0, col * size, Math.Min(rows * size, panelGraphic.DisplayRectangle.Bottom));

//			// Paint a light gray bar on the row and column of the clicked team. Not in use because we'd have to rebuild grid every PanelGraphicMouseClick.
//			for (int x = 0; x < Fixture.Games.Count; x++)
//				for (int y = 0; y < Fixture.Teams.Count; y++)
//					if (point != null && (point.Y == x || point.Y == y))
//						FillCell(x, y, size, Color.LightGray);
		}

		// Paint coloured cells onto grid to show teams in games.
		void PaintCells(int size)
		{
			var g = panelGraphic.CreateGraphics();

			for (int col = 0; col < Fixture.Games.Count; col++)
			{
				var fg = Fixture.Games[col];
				foreach (var x in fg.Teams)
				{
					var row = Fixture.Teams.IndexOf(x.Key);
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
			int rows = Fixture.Teams.Count;

			if (resizing)
			{
				PaintBorder(size, rows);
				return;
			}

			PaintGrid(size, rows);
			PaintCells(size);
			PaintNumbers(size, rows);
			PaintWhoPlaysWho(size, rows);

			// Paint palette.
			for (Colour i = Colour.None; i < Colour.White; i++)
				FillCell(rows + 2, (int)i, size, i.ToSaturatedColor());
		}

		void FillColumn(ZoomReport report, int topSpace, int teamsInGame, int teamsCut)
		{
			int row = 0;
			// Add cells for the space above this team's first game (and the ribbons between games).
			for (int i = 0; i < topSpace; i++, row++)
			{
				report.Rows[row].AddCell(new ZCell());
				report.Rows[row].AddCell(new ZCell());
			}

			// Add cells for the game (and the ribbons between games).
			for (int i = 0; i < teamsInGame; i++, row++)
			{
				report.Rows[row].AddCell(new ZCell(" ... ", Colour.Referee.ToColor()));
				report.Rows[row].AddCell(new ZCell());
			}

			for (int i = 0; i < teamsCut; i++, row++)
				report.Rows[row].AddCell(new ZCell(Utility.Ordinate(row + 1)));
		}

		private void RefreshFinals(object sender, EventArgs e)
		{
			var report = new ZoomReport("Finals");

			report.Columns.Add(new ZColumn("Teams", ZAlignment.Left));

			int numteams = Fixture.Teams.Count;
			int teamsPerGame = (int)numericTeamsPerGame.Value;
			int teamsToCut = (int)numericTeamsToCut.Value;

			// Add columns for games.
			int col;
			for (col = numteams; col >= teamsPerGame + 1; col -= teamsToCut)
			{
				report.Columns.Add(new ZColumn(col.ToString(), ZAlignment.Center, "Games"));

				var ribbon = new ZRibbonColumn("Games");
				for (int j = 0; j < teamsPerGame; j++)
				{
					ribbon.From.Add(new ZRibbonEnd(col - j - 1, 5));
					ribbon.To.Add(new ZRibbonEnd(col - j - 1, 5));
				}
				report.Columns.Add(ribbon);
			}

			// Add rows to the report.
			for (int row = 0; row < numteams; row++)
			{
				report.Rows.Add(new ZRow());
				report.Rows[row].AddCell(new ZCell(Fixture.Teams[numteams - row - 1].Name));
			}

			// Add cells to each column.
			col = 0;
			int topSpace = numteams - teamsPerGame;
			while (topSpace > 0)
			{
				FillColumn(report, topSpace, teamsPerGame, col == 0 ? 0 : teamsToCut);
				topSpace -= teamsToCut;
				col++;
			}

			for (int row = teamsPerGame; row < topSpace + teamsPerGame + teamsToCut; row++)
				report.Rows[row].AddCell(new ZCell(Utility.Ordinate(row + 1)));

			// Add columns for grand finals.
			for (int i = 0; i < teamsPerGame; i++)
				report.Columns.Add(new ZColumn((teamsPerGame - i).ToString(), ZAlignment.Center, "Finals"));

			// Add grand finals games.
			for (int i = 0; i < teamsPerGame; i++)
				for (int j = 0; j < teamsPerGame; j++)
					report.Rows[i].AddCell(new ZCell(" ... ", ((Colour)((j + teamsPerGame - i) % teamsPerGame + 1)).ToColor()));

			using (StringWriter sw = new StringWriter())
			{
				sw.Write(report.ToSvg(true));
				document = SvgDocument.FromSvg<SvgDocument>(sw.ToString());
				aspectRatio = document.ViewBox.Width / document.ViewBox.Height;
				timerRedraw_Tick(null, null);
			}
		}

		private void PanelFinalsResize(object sender, EventArgs e)
		{
			timerRedraw.Enabled = true;
		}

		private void timerRedraw_Tick(object sender, EventArgs e)
		{
			if (document != null)
			{
				document.Width = new SvgUnit(SvgUnitType.Pixel, panelFinals.Width);
				document.Height = new SvgUnit(SvgUnitType.Pixel, (int)(panelFinals.Width / aspectRatio));
				panelFinals.BackgroundImage = document.Draw();
			}
			timerRedraw.Enabled = false;
		}

		private void ButtonAscensionClick(object sender, EventArgs e)
		{
			numericTracks.Value = 1;
			numericTeamsToCut.Value = 1;
		}

		private void ButtonTwoTrackClick(object sender, EventArgs e)
		{
			numericTracks.Value = 2;
			numericTeamsToCut.Value = 2;
		}

		private void ButtonFormatDClick(object sender, EventArgs e)
		{
			numericTracks.Value = 3;
			numericTeamsToCut.Value = 2;
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
			int rows = Fixture.Teams.Count;

			if (point.X < Fixture.Games.Count && point.Y < rows)
			{
				if (Fixture.Games[point.X].Teams.ContainsKey(Fixture.Teams[point.Y]))
				{
					Fixture.Games[point.X].Teams.Remove(Fixture.Teams[point.Y]);
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
					Fixture.Games[point.X].Teams.Add(Fixture.Teams[point.Y], c);
					FillCell(point.Y, point.X, size, c.ToSaturatedColor());
				}

				PaintNumbers(size, rows);
				PaintWhoPlaysWho(size, rows);
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
		
		void FormFixtureResizeBegin(object sender, EventArgs e)
		{
			resizing = true;
		}

		void FormFixtureResizeEnd(object sender, EventArgs e)
		{
			resizing = false;
			panelGraphic.Invalidate();
		}
	}
}
