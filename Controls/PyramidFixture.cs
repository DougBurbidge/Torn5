using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Torn5.Controls
{
	public partial class PyramidFixture : UserControl
	{
		int round;
		public int Round { get => round;
			set
			{
				round = value;
				labelRound.Text = "Round " + round.ToString();
				checkBoxRepechage.Text = "Repêchage " + round.ToString();
				fixtureRound.RoundNumber = value;
				fixtureRepechage.RoundNumber = value;
			}
		}

		public int TeamsIn { get => fixtureRound.TeamsIn; set { fixtureRound.TeamsIn = value; ValueChangedInternal(); } }
		public int TeamsOut { get => fixtureRound.Advance + (checkBoxRepechage.Checked ? fixtureRepechage.Advance : 0); }
		public int RepechageTeams { get => checkBoxRepechage.Checked ? fixtureRepechage.TeamsIn : 0; }
		public int RoundGames { get => fixtureRound.Games; set => fixtureRound.Games = value; }
		public int RepechageGames { get => checkBoxRepechage.Checked ? fixtureRepechage.Games : 0; set => fixtureRepechage.Games = value; }
		public int RoundAdvance { get => fixtureRound.Advance; set { fixtureRound.Advance = value; ValueChangedInternal(); } }
		public int RepechageAdvance { get => checkBoxRepechage.Checked ? fixtureRepechage.Advance : 0; set => fixtureRepechage.Advance = value; }
		public bool HasRepechage { get => checkBoxRepechage.Checked; set => checkBoxRepechage.Checked = value; }
		public int RoundGamesPerTeam { get => fixtureRound.GamesPerTeam; set => fixtureRound.GamesPerTeam = value; }

		public PyramidHalfFixture FixtureRound { get => fixtureRound; }
		public PyramidHalfFixture FixtureRepechage { get => fixtureRepechage; }

		[Browsable(true)] [Category("Action")]
		public event EventHandler ValueChanged;

		public PyramidFixture()
		{
			InitializeComponent();
		}

		public void Idealise(int desiredTeamsPerGame, double advanceRatePerPartRound)
		{
			fixtureRound.Idealise(desiredTeamsPerGame, advanceRatePerPartRound);
			fixtureRepechage.Idealise(desiredTeamsPerGame, advanceRatePerPartRound);
		}

		public string Description()
		{
			string s = "Round " + Round + ": ";
			if (RoundGamesPerTeam != 1)
				s += "You play " + RoundGamesPerTeam + " games. ";
			s += "Top " + RoundAdvance + " teams advance to Round " + (Round + 1) + ". Remaining " + RepechageTeams + " to Repêchage " + Round + ".\r\n";
			s += "Repêchage " + Round + ": ";
			if (RoundGamesPerTeam != 1)
				s += "You play 1 game. ";
			s += "Top " + RepechageAdvance + " teams advance to Round " + (Round + 1) + ". Remaining " + (TeamsIn - RoundAdvance - RepechageAdvance) + " eliminated.\r\n";

			return s;
		}

		private void HalfFixtureChanged(object sender, System.EventArgs e)
		{
			ValueChangedInternal();

			ValueChanged?.Invoke(this, e);
		}

		private void ValueChangedInternal()
		{
			if (fixtureRound.TeamsIn > fixtureRound.Advance)
				fixtureRepechage.TeamsIn = fixtureRound.TeamsIn - fixtureRound.Advance;
		}

		private void CheckBoxRepechageCheckedChanged(object sender, System.EventArgs e)
		{
			fixtureRepechage.Visible = checkBoxRepechage.Checked;
			HalfFixtureChanged(sender, e);
        }
	}
}
