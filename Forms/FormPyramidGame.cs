using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Torn;

namespace Torn.UI
{
	public partial class FormPyramidGame : Form
	{
		List<PyramidGame> games;
		public List<PyramidGame> Games {
			get { return games; }
			set
			{
				games = value;
				if (games.Any())
				{
					var times = games.Select(pg => pg.Game.Time);
					if (games.Count <= 4)
					{
						if (times.Min().Date == times.Max().Date)
							labelGameTime.Text = string.Join(", ", games.Select(g => g.Game.Time.ToString(" HH:mm")));
						else
							labelGameTime.Text = string.Join(", ", games.Select(g => g.Game.Time.FriendlyDateTime()));
					}
					else
					{
						if (times.Min().Date == times.Max().Date)
							labelGameTime.Text = games.Count.ToString() + " games from " + times.Min().ToString("HH:mm") + " to " + times.Max().ToString("HH:mm");
						else
							labelGameTime.Text = games.Count.ToString() + " games from " + times.Min().FriendlyDateTime() + " to " + times.Max().FriendlyDateTime();
					}

					textBoxDescription.Text = games.First().Game.Title;
					labelTeamsInGame.Text = games.First().Game.Teams.Count.ToString();
					numericTeamsToTake.Maximum = games.Max(pg => pg.Game.Teams.Count);

					var takes = games.Select(pg => pg.TeamsToTake);
					if (!takes.Any(t => !t.HasValue) && takes.Min() == takes.Max())
						numericTeamsToTake.Value = (int)takes.First();
					else
					{
						numericTeamsToTake.Value = 0;
						numericTeamsToTake.Text = "";
					}

					var priorities = games.Select(pg => pg.Priority);
					radioRound.Checked = priorities.All(p => p == Priority.Round);
					radioRepechage.Checked = priorities.All(p => p == Priority.Repechage);

					checkBoxSecret.Checked = games.First().Game.Secret;
				}
				else
				{
					labelGameTime.Text = "";
					textBoxDescription.Text = "";
					labelTeamsInGame.Text = "";
					numericTeamsToTake.Value = 0;
					numericTeamsToTake.Maximum = 0;
					numericTeamsToTake.Text = "";
					radioRound.Checked = false;
					radioRepechage.Checked = false;
					checkBoxSecret.Checked = false;
				}
				buttonOK.Enabled = radioRound.Checked || radioRepechage.Checked;
			}
		}

		public int? TeamsToTake
		{
			get { return string.IsNullOrWhiteSpace(numericTeamsToTake.Text) ? null : (int?)numericTeamsToTake.Value; }
			set { if (value.HasValue) numericTeamsToTake.Value = (int)value; }
		}

		public Priority Priority
		{
			get { return radioRound.Checked ? Priority.Round : radioRepechage.Checked ? Priority.Repechage : Priority.Unmarked; }
			set
			{
				radioRound.Checked = value == Priority.Round;
				radioRepechage.Checked = value == Priority.Repechage;
				buttonOK.Enabled = radioRound.Checked || radioRepechage.Checked;
			}
		}

		public FormPyramidGame()
		{
			InitializeComponent();
		}

		private void ButtonOKClick(object sender, EventArgs e)
		{
			foreach (var game in games)
				game.Game.Title = textBoxDescription.Text;

			DialogResult = DialogResult.OK;
		}

		private void RadioCheckedChanged(object sender, EventArgs e)
		{
			buttonOK.Enabled = radioRound.Checked || radioRepechage.Checked;
		}

		private void ButtonClearClick(object sender, EventArgs e)
		{
			numericTeamsToTake.ResetText();
		}
	}
}
