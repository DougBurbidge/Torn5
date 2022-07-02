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

		public int TeamsIn { get => fixtureRound.TeamsIn; set => fixtureRound.TeamsIn = value; }
		public int TeamsOut { get => fixtureRound.Advance + fixtureRepechage.Advance; }
		public int RepechageTeams {  get => checkBoxRepechage.Checked ? fixtureRepechage.TeamsIn : 0; }
		public int RoundGames { get => fixtureRound.Games; set => fixtureRound.Games = value; }
		public int RepechageGames { get => checkBoxRepechage.Checked ? fixtureRepechage.Games : 0; set => fixtureRepechage.Games = value; }
		public int RoundAdvance { get => fixtureRound.Advance; set => fixtureRound.Advance = value; }
		public int RepechageAdvance { get => checkBoxRepechage.Checked ? fixtureRepechage.Advance : 0; set => fixtureRepechage.Advance = value; }
		public bool HasRepechage { get => checkBoxRepechage.Checked; set => checkBoxRepechage.Checked = value; }
		public int RoundGamesPerTeam { get => fixtureRound.GamesPerTeam; set => fixtureRound.GamesPerTeam = value; }

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

		private void HalfFixtureChanged(object sender, System.EventArgs e)
		{
			if (fixtureRound.TeamsIn > fixtureRound.Advance)
				fixtureRepechage.TeamsIn = fixtureRound.TeamsIn - fixtureRound.Advance;

			ValueChanged?.Invoke(this, e);
		}

		private void CheckBoxRepechageCheckedChanged(object sender, System.EventArgs e)
		{
			fixtureRepechage.Visible = checkBoxRepechage.Checked;
		}
	}
}
