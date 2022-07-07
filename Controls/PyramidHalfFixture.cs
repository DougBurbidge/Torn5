using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

		public int TeamsIn { get => (int)numericTeams.Value; set { numericTeams.Value = value; ValueChangedInternal(); } }
		public int TeamsOut { get => (int)numericTeams.Value - (int)numericAdvance.Value; }
		public int Games { get => (int)numericGames.Value; set { if (value > 0) numericGames.Value = value; ValueChangedInternal(); } }
		public int Advance { get => (int)numericAdvance.Value; set { numericAdvance.Value = value; ValueChangedInternal(); } }

		int gamesPerTeam = 1;
		public int GamesPerTeam
		{
			get => gamesPerTeam; set
			{
				gamesPerTeam = value;
				numericTeamsPerGame.Value = numericTeams.Value * GamesPerTeam / numericGames.Value;
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
			numericTeamsPerGame.Value = numericTeams.Value * GamesPerTeam / numericGames.Value;

			if (numericTeams.Value > 0)
				numericAdvancePercent.Value = numericAdvance.Value / numericTeams.Value * 100;
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
