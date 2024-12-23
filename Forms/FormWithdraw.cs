using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Torn.UI
{
	public partial class FormWithdraw : Form
	{
		private League league;
		public League League { get; set; }

		private void BuildControls()
		{
			panelMiddle.Controls.Clear();

			var teams = League.Teams.OrderBy(t => t.Name).ToList();
			int longestName = teams.Max(t => TextRenderer.MeasureText(t.Name, Font).Width);

			int maxHeight = (int)(Screen.PrimaryScreen.WorkingArea.Height * 0.8 / 23);  // Conservative maximum number of checkboxes we can fit vertically.
			int columns = teams.Count / maxHeight + 1;
			int boxesPerColumn = (int)Math.Ceiling(teams.Count * 1.0 / columns);

			ClientSize = new Size(Math.Max(Math.Min(columns * (longestName + 40) + 20, (int)(Screen.PrimaryScreen.WorkingArea.Width * 0.9)), 280),
									boxesPerColumn * 23 + 12 + panelTop.Height + panelBottom.Height);

			for (int team = 0; team < teams.Count; team++)
			{
				int column = team / boxesPerColumn;
				var x = new CheckBox()
				{
					Top = (team % boxesPerColumn) * 23 + 6,
					Left = column * (longestName + 40) + 16,
					Width = longestName + 30,
					Text = teams[team].Name,
					Tag = teams[team].TeamId,
					Checked = teams[team].Active,
					Parent = panelMiddle
				};
			}
		}

		public FormWithdraw()
		{
			InitializeComponent();
		}

		private void ButtonOKClick(object sender, System.EventArgs e)
		{
			foreach (var control in panelMiddle.Controls)
			{
				if (control is CheckBox checkBox && checkBox.Tag is int tag)
				{
					var team = League.Teams.Find(t => t.TeamId == tag);
					if (team != null)
						team.Active = checkBox.Checked;
				}
			}
		}

		private void FormWithdrawShown(object sender, EventArgs e)
		{
			if (League != null)
			BuildControls();
		}
	}
}
