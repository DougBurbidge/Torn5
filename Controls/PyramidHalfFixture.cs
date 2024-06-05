using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;

namespace Torn5.Controls
{
	public partial class PyramidHalfFixture : UserControl
	{
		bool isRound;
		[Browsable(true)]
		public bool IsRound { get => isRound;
			set
			{
				isRound = value;
				if (isRound)
					labelTeams.Text = roundNumber == 1 ? "Starting teams" : "From Round " + (roundNumber - 1).ToString();
				else
					labelTeams.Text = "From Round " + (roundNumber).ToString();
			}
		}

		int roundNumber;
		[Browsable(true)]
		public int RoundNumber
		{
			get => roundNumber;
			set
			{
				roundNumber = value;
				if (isRound)
					labelTeams.Text = roundNumber == 1 ? "Starting teams" : "From Round " + (roundNumber - 1).ToString();
				else
					labelTeams.Text = "From Round " + (roundNumber).ToString();
			}
		}

		int teamsIn;
		public int TeamsIn { get => teamsIn; set { teamsIn = value; ValueChangedInternal(); } }
		public int TeamsOut { get => teamsIn - (int)numericAdvance.Value; }
		public int Games { get => (int)numericGames.Value; set { if (value > 0) numericGames.Value = value; ValueChangedInternal(); } }
		public int Advance { get => (int)numericAdvance.Value; set { numericAdvance.Value = value; ValueChangedInternal(); } }

		int gamesPerTeam = 1;
		public int GamesPerTeam
		{
			get => gamesPerTeam; set
			{
				gamesPerTeam = value;
				labelTeamsPerGame.Text = (teamsIn * GamesPerTeam / numericGames.Value).ToString();
			}
		}

		[Browsable(true)]
		[Category("Action")]
		public event EventHandler ValueChanged;

		public PyramidHalfFixture()
		{
			InitializeComponent();
        }

		public void Idealise(int desiredTeamsPerGame, double advanceRatePerPartRound)
		{
			Games = (int)Math.Round(1.0 * TeamsIn * GamesPerTeam / desiredTeamsPerGame);
			Advance = (int)Math.Round(1.0 * TeamsIn * advanceRatePerPartRound);
		}

		private void NumericChanged(object sender, System.EventArgs e)
		{
			ValueChangedInternal();

			ValueChanged?.Invoke(this, e);
		}

		private void ValueChangedInternal()
		{
			labelTeamsIn.Text = teamsIn.ToString();

            decimal tpg = teamsIn * GamesPerTeam / numericGames.Value;
            if (tpg == (int)tpg)  // If teams per game is a whole number, print it as a whole number, plus invisible characters the width of ".00"
                labelTeamsPerGame.Text = tpg.ToString("F0", CultureInfo.CurrentCulture) + '\u2008' + '\u2002' + '\u2002';  // punctutation space (width of a .), en space (nut), en space (nut).
			else  //  else print it with its two actual decimal places showing.
                labelTeamsPerGame.Text = tpg.ToString("F2", CultureInfo.CurrentCulture);

            if (teamsIn > 0)
                labelAdvancePercent.Text = String.Format("{0:0.00%}", numericAdvance.Value / teamsIn);
		}

		private void NumericKeyUp(object sender, KeyEventArgs e)
		{
			var _ = ((NumericUpDown)sender).Value;  // This black magic forces the control's ValueChanged to fire after the user edits the text in the control.
		}

		public override string ToString()
		{
			return (IsRound ? "Round " : "Repêchage ") + roundNumber;
		}
	}
}
