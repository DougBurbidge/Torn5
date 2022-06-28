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
				labelRoundTeams.Text = round == 1 ? "Starting teams" : "From round " + (round - 1).ToString();
				labelRepTeams.Text = "From round " + round.ToString();
			}
		}

		public int TeamsIn { get => (int)numericRoundTeams.Value; set => numericRoundTeams.Value = value; }
		public int TeamsOut { get => (int)(numericRoundAdvance.Value + numericRepAdvance.Value); }
		public int RoundGames { get => (int)numericRoundGames.Value; set => numericRoundGames.Value = value; }
		public int RepechageGames { get => (int)numericRepGames.Value; set => numericRepGames.Value = value; }
		public int RoundAdvance { get => (int)numericRoundAdvance.Value; set => numericRoundAdvance.Value = value; }
		public int RepechageAdvance { get => (int)numericRepAdvance.Value; set => numericRepAdvance.Value = value; }
		public bool HasRepechage { get => checkBoxRepechage.Checked; set => checkBoxRepechage.Checked = value; }

		int roundGamesPerTeam = 1;
		public int RoundGamesPerTeam { get => roundGamesPerTeam; set
			{
				roundGamesPerTeam = value;
				NumericChanged(null, null);
			}
		}

		[Browsable(true)] [Category("Action")]
		public event EventHandler ValueChanged;

		public PyramidFixture()
		{
			InitializeComponent();
		}

		private void NumericChanged(object sender, System.EventArgs e)
		{
			numericRoundTeamsPerGame.Value = numericRoundTeams.Value * RoundGamesPerTeam / numericRoundGames.Value;
			numericRoundAdvancePercent.Value = numericRoundAdvance.Value / numericRoundTeams.Value * 100;

			if (numericRoundTeams.Value > numericRoundAdvance.Value)
				numericRepTeams.Value = numericRoundTeams.Value - numericRoundAdvance.Value;

			numericRepTeamsPerGame.Value = numericRepTeams.Value / numericRepGames.Value;
			numericRepAdvancePercent.Value = numericRepAdvance.Value / numericRepTeams.Value * 100;
			
			ValueChanged?.Invoke(this, e);
		}

		private void NumericKeyUp(object sender, KeyEventArgs e)
		{
			var _ = ((NumericUpDown)sender).Value;  // This black magic forces the control's ValueChanged to fire after the user edits the text in the control.
		}

		private void CheckBoxRepechageCheckedChanged(object sender, System.EventArgs e)
		{
			bool b = checkBoxRepechage.Checked;
			labelRepAdvance.Visible = b;
			labelRepAdvancePercent.Visible = b;
			labelRepGames.Visible = b;
			labelRepTeams.Visible = b;
			labelRepTeamsPerGame.Visible = b;
			numericRepAdvance.Visible = b;
			numericRepAdvancePercent.Visible = b;
			numericRepGames.Visible = b;
			numericRepTeams.Visible = b;
			numericRepTeamsPerGame.Visible = b;
		}
	}
}
