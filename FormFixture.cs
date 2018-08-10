
using System;
using System.Drawing;
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

		public FormFixture()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		
		public FormFixture(Fixture fixture, League league): this()
		{
			Fixture = fixture;
			League = league;
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

			Fixture.Games.Clear();

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
		}
		
		void FormFixtureShown(object sender, EventArgs e)
		{
			if (Fixture != null)
			{
				textBoxTeams.Text = Fixture.Teams.ToString();

				if (Fixture.Games.Count > 0)
					textBoxGames.Text = Fixture.Games.ToString();
			}
		}
		
		void TextBoxKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Control && e.KeyCode == Keys.A &&sender != null)
	        	((TextBox)sender).SelectAll();
		}
		
		void Panel1Paint(object sender, PaintEventArgs e)
		{
			
		}
	}
}
