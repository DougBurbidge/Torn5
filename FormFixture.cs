using System;
using System.Drawing;
using System.Linq;
using System.Globalization;
using System.Windows.Forms;
using Torn;

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

		public FormFixture()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			timePicker.CustomFormat = CultureInfo.CurrentUICulture.DateTimeFormat.ShortTimePattern;
			leftButton = Colour.Red;
			middleButton = Colour.Blue;
			rightButton = Colour.Green;
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
						ft.Id = lt.Id;
						Fixture.Teams.Add(ft);
					}

				textBoxTeams.Text = Fixture.Teams.ToString();

				if (Fixture.Games.Count > 0)
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

		void PaintNumbers()
		{
	        int size = (int)numericSize.Value;
			int rows = Fixture.Teams.Count;//Math.Max(Fixture.Teams.Count, (int)Fixture.Games.Max(fg => fg.Teams.Max(ft => (int?)ft.Key.Id)));
			var difficulties = new float[rows];
			var counts = new int[rows];
			var averages = new float[rows];

			foreach (var fg in Fixture.Games)
				foreach (var ft in fg.Teams)
				{
					difficulties[ft.Key.Id - 1] += (fg.Teams.Sum(x => x.Key.Id) - ft.Key.Id) / (fg.Teams.Count - 1F);
					counts[ft.Key.Id - 1]++;
				}
			
			for (int row = 0; row < rows; row++)
				if (counts[row] > 0)
					averages[row] = difficulties[row] / counts[row];
				else
					averages[row] = float.NaN;
			
			float max = averages.Max();

			var g = panelGraphic.CreateGraphics();
			var font = new Font("Arial", size - 2);
			g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
	        Pen pen = new Pen(Color.Black);

	        g.FillRectangle(new SolidBrush(Color.White), Fixture.Games.Count * size + 1, 0, Fixture.Games.Count * size, rows * size);

	        for (int row = 0; row < rows; row++)
	        	if (!float.IsNaN(averages[row]))
				{
					g.DrawString(counts[row].ToString() + "  " + averages[row].ToString("N2"), font, Brushes.Black, Fixture.Games.Count * size, row * size - 2);
					float x = Fixture.Games.Count * size + 50 + averages[row] / max * 100;
					g.DrawLine(pen, x, row * size, x, row * size + size - 1);
				}

	        g.FillRectangle(new SolidBrush(Color.White), 0, rows * size + 1, Fixture.Games.Count * size, size);

	        for (int col = 0; col < Fixture.Games.Count; col++)
				g.DrawString(Fixture.Games[col].Teams.Count.ToString(), font, Brushes.Black, col * size - 1, rows * size);
		}

		void PanelGraphicPaint(object sender, PaintEventArgs e)
		{
	        int size = (int)numericSize.Value;
			int rows = Fixture.Teams.Count;//Math.Max(Fixture.Teams.Count, (int)Fixture.Games.Max(fg => fg.Teams.Max(ft => (int?)ft.Key.Id)));

			var panel = sender as Panel;
	        var g = e.Graphics;

	        g.FillRectangle(new SolidBrush(Color.White), panel.DisplayRectangle);
	        Pen pen = new Pen(Color.Gray);

			for (int col = 0; col < Fixture.Games.Count; col++)
				for (int row = 0; row <= rows; row++)
					g.DrawLine(pen, 0, row * size, Math.Min(Fixture.Games.Count * size, panel.DisplayRectangle.Right), row * size);

			for (int row = 0; row <= rows; row++)
				for (int col = 0; col <= Fixture.Games.Count; col++)
					g.DrawLine(pen, col * size, 0, col * size, Math.Min(rows * size, panel.DisplayRectangle.Bottom));
			
			for (int col = 0; col < Fixture.Games.Count; col++)
			{
				var fg = Fixture.Games[col];
				foreach (var x in fg.Teams)
				{
					var row = Fixture.Teams.IndexOf(x.Key);
					if (row != -1)
						FillCell(row, col, size, x.Value.ToSaturatedColor());
				}
			}

			PaintNumbers();
			
			for (Colour i = Colour.None; i < Colour.White; i++)
				FillCell(rows + 2, (int)i, size, i.ToSaturatedColor());
		}

		void NumericSizeValueChanged(object sender, EventArgs e)
		{
			panelGraphic.Invalidate();
		}

		void PanelGraphicMouseClick(object sender, MouseEventArgs e)
		{
			Point point = panelGraphic.PointToClient(Cursor.Position);
	        int size = (int)numericSize.Value;
			int rows = Fixture.Teams.Count;//Math.Max(Fixture.Teams.Count, (int)Fixture.Games.Max(fg => fg.Teams.Max(ft => (int?)ft.Key.Id)));

			if (point.X / size < Fixture.Games.Count && point.Y / size < rows)
			{
				if (Fixture.Games[point.X / size].Teams.ContainsKey(Fixture.Teams[point.Y / size]))
				{
					Fixture.Games[point.X / size].Teams.Remove(Fixture.Teams[point.Y / size]);
					FillCell(point.Y / size, point.X / size, size, Color.White);
				}
				else
				{
					Fixture.Games[point.X / size].Teams.Add(Fixture.Teams[point.Y / size], leftButton);
					Colour c = Colour.None;
					switch (e.Button) {
						case MouseButtons.Left: c = leftButton;	    break;
						case MouseButtons.Right: c = rightButton;	break;
						case MouseButtons.Middle: c = middleButton;	break;
						case MouseButtons.XButton1: c = xButton1;	break;
						case MouseButtons.XButton2: c = xButton2;	break;
					}
					FillCell(point.Y / size, point.X / size, size, c.ToSaturatedColor());
				}

				PaintNumbers();
			}

			else if (point.Y / size == rows + 2 && point.X / size > 0 && point.X / size < 9)
			{
				var c = (Colour)(point.X / size);
				switch (e.Button) {
					case MouseButtons.Left: leftButton = c;	    break;
					case MouseButtons.Right: rightButton = c;	break;
					case MouseButtons.Middle: middleButton = c;	break;
					case MouseButtons.XButton1: xButton1 = c;	break;
					case MouseButtons.XButton2: xButton2 = c;	break;
				}
			}
		}
	}
}
