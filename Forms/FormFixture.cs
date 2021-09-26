using System;
using System.Collections.Generic;
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

		// TODO: turn panelGraphic into a custom control that takes a Fixture as a property, so it can manage its own painting, clicks, etc.
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

		/// Add a cell to a row with a simple ribbon running from left to right in the cell.
		void AddCell(ZoomReport report, int row, ZColumn col, Color color)
		{
			if (col != null)
			{
				report.Rows[row].AddCell(new ZCell());
				col.AddRibbon(row, 5, color);
			}
		}

		void AddCells(ZoomReport report, int row, ZColumn col1, ZColumn col2, Color color = default)
		{
			AddCell(report, row, col1, color);
			AddCell(report, row, col2, color);
		}

		/// <summary>
		/// Add column and cells representing this game, plus a ribbon showing arrows leading out of this game.
		/// </summary>
		/// <param name="report">The report template we're adding a column to.</param>
		/// <param name="gameName">Title for the column.</param>
		/// <param name="topSpace">Space above this game to fill with pale horizontal ribbons representing teams that haven't played their first game yet.</param>
		/// <param name="stepDown">Space above this game to fill with _dark_ horizontal ribbons, representing teams that have played their first game, but aren't in this game.</param>
		/// <param name="teamsInGame">Teams that are actually playing in this game.</param>
		/// <param name="bottomSpace">Space below this game to fill with dark horizontal ribbons, representing teams that have played their first game, but aren't in this game.</param>
		/// <param name="narrow">If true, add just one column, and just one columns worth of cells, with the game cells going in the column just to the left of the one we're adding. If false, add two columns, with the second being for ribbons.</param>
		/// <returns>The column that contains the game cells.</returns>
		ZColumn FillColumn(ZoomReport report, string gameName, int topSpace, int stepDown, int teamsInGame, int bottomSpace, bool narrow = false)
		{
			ZColumn gameCol;
			// Add columns for game (and the ribbons between games).
			if (narrow)
			{
				gameCol = report.Columns.Last();
				gameCol.Text = gameName;
			}
			else
			{
				gameCol = new ZColumn(gameName, ZAlignment.Center, "Games");
				report.Columns.Add(gameCol);
			}

			var arrowCol = new ZColumn(null, ZAlignment.Center, "Games");
			report.Columns.Add(arrowCol);
			var ribbon = new ZRibbon();
			arrowCol.Ribbons.Add(ribbon);

			int row = 0;
			// Add cells for the space above this game (and the ribbons between games).
			for (int i = 0; i < topSpace && row < report.Rows.Count - teamsInGame; i++, row++)
				AddCells(report, row, narrow ? null : gameCol, arrowCol, Color.FromArgb(0xEE, 0xEE, 0xEE));

			// Add cells for the space which is above this game but below the teams first game, because this is a lower track.
			for (int i = 0; i < stepDown && row < report.Rows.Count - teamsInGame; i++, row++)
				AddCells(report, row, narrow ? null : gameCol, arrowCol);

			// Add cells for the game (and the ribbons between games).
			for (int i = 0; i < teamsInGame; i++, row++)
			{
				if (narrow)
				{
					report.Rows[row].RemoveAt(report.Rows[row].Count - 1);
					var dupRibbon = gameCol.Ribbons.Find(r => r.From[0].Row == row);
					if (dupRibbon != null)
						gameCol.Ribbons.Remove(dupRibbon);
				}

				report.Rows[row].AddCell(new ZCell(" ... ", Colour.Referee.ToColor()));
				report.Rows[row].AddCell(new ZCell());

				ribbon.From.Add(new ZRibbonEnd(row, 5));
				ribbon.To.Add(new ZRibbonEnd(row, 5));
			}

			// Add cells for the space below this game (and the ribbons between games), so that lower-track games are placed correctly.
			for (int i = 0; i < bottomSpace && row < report.Rows.Count; i++, row++)
				AddCells(report, row, narrow ? null : gameCol, arrowCol);

			return gameCol;
		}

		string GameName(int game)
		{
			if (game < 26)
				return ((char)((int)'A' + game)).ToString();  // "A", "B", etc.
			if (game < 52)
				return new String(((char)((int)'a' + game - 26)), 2);  // "aa", "bb", etc.
			return (game + 1).ToString();
		}

		void TwoTrack(ZoomReport report, int numTeams, int teamsPerGame, int teamsToCut, int freeRides)
		{
			// Add columns for games.
			int game = 0;
			int topSpace;
			var games = new List<ZColumn>();

			// Add one or two "startup" games to get things rolling.
			if ((numTeams - teamsPerGame - freeRides) % teamsToCut == 0)
			{
				topSpace = numTeams - teamsPerGame;  // Start the first track at the bottom.
				games.Add(FillColumn(report, GameName(game), topSpace, 0, teamsPerGame, 0));
				game++;
				topSpace -= teamsToCut;
				games.Add(FillColumn(report, GameName(game), topSpace, 0, teamsPerGame, teamsPerGame - 2));
				game++;
				topSpace -= teamsToCut;
			}
			else
			{
				topSpace = numTeams - 2 * teamsPerGame + teamsToCut;  // Start the first track, with the lowest teams we can while still allowing for lower tracks.
				games.Add(FillColumn(report, GameName(game), topSpace, 0, teamsPerGame, teamsPerGame - 2));
				game++;
				topSpace = numTeams - 2 * teamsPerGame;
			}

			// Add all the "regular" games in the middle. Note narrow = true because these are back-to-back games: no team from these games plays in the immediately previous game.
			for ( ; topSpace >= freeRides; game += 2)
			{
				games.Add(FillColumn(report, GameName(game), topSpace, 0, teamsPerGame, teamsPerGame, game > 2));
				games.Add(FillColumn(report, GameName(game + 1), topSpace, teamsPerGame, teamsPerGame, 0, true));
				topSpace -= teamsToCut;
			}

			// Add the "golden game" just before finals.
			games.Add(FillColumn(report, GameName(game), freeRides, teamsPerGame - teamsToCut, teamsPerGame, 0));

			for (int row = teamsPerGame; row < numTeams; row++)
				report.Rows[row].AddCell(new ZCell(Utility.Ordinate(row + 1)));

			// Add grand finals games.
			for (int i = 0; i < teamsPerGame; i++)
			{
				var finalCol = new ZColumn((teamsPerGame - i).ToString(), ZAlignment.Center, "Finals");
				report.Columns.Add(finalCol);
				games.Add(finalCol);
				for (int j = 0; j < teamsPerGame; j++)
					report.Rows[i].AddCell(new ZCell(" ... ", ((Colour)((j + teamsPerGame - i) % teamsPerGame + 1)).ToColor()));
			}

			report.SameWidths.Add(games);
		}
		void GeneralAscension(ZoomReport report, int numTeams, int teamsPerGame, int teamsToCut, int tracks, int freeRides)
		{
			// Add columns for games.
			int col = 0;
			int game = 0;
			int topSpace;
			if (tracks == 1)
			{
				topSpace = numTeams - teamsPerGame;
				topSpace += (topSpace + freeRides) % teamsToCut;
			}
			else
			{
				topSpace = numTeams - teamsPerGame - (tracks - 1) * (teamsPerGame - teamsToCut);  // Start the first track, with the lowest teams we can while still allowing for lower tracks.
				topSpace += (topSpace + freeRides) % teamsToCut;  // Ensure topSpace is a multiple of teamsToCut, so we will line up properly when we get to finals. If not, move upwards so that it is a whole multiple.
			}

			bool bottom = false;  // True if the bottom-most team has played a game.
			while (topSpace >= freeRides)
			{
				for (int track = 0; track < tracks; track++)
					if (!bottom || topSpace + teamsPerGame + track * teamsToCut <= numTeams)
					{
						string gameName = tracks == 1 ? (topSpace + teamsPerGame).ToString() : GameName(game);
						FillColumn(report, gameName, topSpace, track * (teamsPerGame - teamsToCut), teamsPerGame, (tracks - track - 1) * teamsToCut);
						game++;
						bottom |= topSpace + teamsPerGame + track * teamsToCut >= numTeams;
					}
				topSpace -= teamsToCut;
				col++;
			}

			for (int row = teamsPerGame; row < numTeams; row++)
				report.Rows[row].AddCell(new ZCell(Utility.Ordinate(row + 1)));

			// Add grand finals games.
			for (int i = 0; i < teamsPerGame; i++)
			{
				report.Columns.Add(new ZColumn((teamsPerGame - i).ToString(), ZAlignment.Center, "Finals"));
				for (int j = 0; j < teamsPerGame; j++)
					report.Rows[i].AddCell(new ZCell(" ... ", ((Colour)((j + teamsPerGame - i) % teamsPerGame + 1)).ToColor()));
			}
		}

		private void RefreshFinals(object sender, EventArgs e)
		{
			labelTeamsToSendUp.Text = "Teams to send up from each game: " + (numericTeamsPerGame.Value - numericTeamsToCut.Value).ToString();

			var report = new ZoomReport("Finals");
			report.Colors.OddColor = report.Colors.BackgroundColor;

			report.Columns.Add(new ZColumn("Teams", ZAlignment.Left));

			// Add rows to the report.
			for (int row = 0; row < Fixture.Teams.Count; row++)
			{
				report.Rows.Add(new ZRow());
				report.Rows[row].AddCell(new ZCell(Fixture.Teams[row].Name));
			}

			if (numericTracks.Value == 2 && Fixture.Teams.Count > 4)
				TwoTrack(report, Fixture.Teams.Count, (int)numericTeamsPerGame.Value, (int)numericTeamsToCut.Value, (int)numericFreeRides.Value);
			else
				GeneralAscension(report, Fixture.Teams.Count, (int)numericTeamsPerGame.Value, (int)numericTeamsToCut.Value, (int)numericTracks.Value, (int)numericFreeRides.Value);

			using (StringWriter sw = new StringWriter())
			{
				sw.Write(report.ToSvg(true));
				document = SvgDocument.FromSvg<SvgDocument>(sw.ToString());
				aspectRatio = document.ViewBox.Width / document.ViewBox.Height;
				timerRedraw_Tick(null, null);
			}
		}

		private void numericTeamsPerGame_ValueChanged(object sender, EventArgs e)
		{
			numericTeamsToCut.Maximum = numericTeamsPerGame.Value - 1;
			numericFreeRides.Maximum = numericTeamsPerGame.Value - 1;

			RefreshFinals(sender, e);
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
