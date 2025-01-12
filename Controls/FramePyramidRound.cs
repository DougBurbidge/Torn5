using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Torn;
using Torn.Report;
using Torn.UI;

namespace Torn5.Controls
{
    public partial class FramePyramidRound : UserControl
    {
        const int ColTitle = 1;
        const int ColNumTeams = 2;
        const int ColTeamsToTake = 3;
        const int ColPriority = 4;
        const int ColSecret = 5;

        Holder holder;
        public Holder Holder
        {
            get { return holder; }

            set
            {
                holder = value;
                RefreshGames();
            }
        }

        League PreviousLeague;

        public FramePyramidRound()
        {
            InitializeComponent();
        }

        private void FramePyramidRoundEnter(object sender, EventArgs e)
        {
            RefreshGames();
        }

        /// <summary>If holder.League.AllGames has changed, rebuild listViewGames entries.</summary>
        private void RefreshGames()
        {
            if (holder == null)
                return;

            listViewGames.BeginUpdate();
            try
            {
                if (PreviousLeague != holder.League)
                    listViewGames.Items.Clear();
                PreviousLeague = holder.League;

                // Add or update items in the list view, one for each game.
                foreach (Game leagueGame in holder.League.AllGames)
                {
                    string key = leagueGame.Time.ToString("o");  // "o" is ISO 8601 yyyy-MM-ddTHH:mm:ss.fffffffK
                    ListViewItem[] matches = listViewGames.Items.Find(key, false);
                    ListViewItem item = matches.Any() ? matches[0] : listViewGames.Items.Add(new ListViewItem { Name = key });

                    item.Text = leagueGame.ServerGame?.InProgress ?? false ? "In Progress" : leagueGame.Time.FriendlyDateTime();

                    if (item.Tag == null)
                        item.Tag = new PyramidGame() { Game = leagueGame, Priority = Priority.Unmarked };
                    
                    while (item.SubItems.Count < listViewGames.Columns.Count || item.SubItems.Count <= ColSecret)
                        item.SubItems.Add("");

                    item.SubItems[ColTitle].Text = leagueGame.Title;  // Description
                    item.SubItems[ColNumTeams].Text = leagueGame.Teams.Count.ToString();  // # teams
                    item.SubItems[ColSecret].Text = leagueGame.Secret ? "Y" : "";  // Secret?
                }
            }
            finally
            {
                listViewGames.EndUpdate();
            }
        }

        readonly FormPyramidGame formPyramidGame = new FormPyramidGame();

        private void ButtonClearPyramidGames(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listViewGames.SelectedItems)
            {
                PyramidGame pg = (PyramidGame)item.Tag;
                pg.TeamsToTake = null;
                pg.Priority = Priority.Unmarked;

                while (item.SubItems.Count < listViewGames.Columns.Count)
                    item.SubItems.Add("");

                item.SubItems[ColTeamsToTake].Text = "";
                item.SubItems[ColPriority].Text = "";
            }
            RefreshPyramidDraw();
            CalculateSpins();
        }

        private void ButtonEditPyramidGamesClick(object sender, EventArgs e)
        {
            if (listViewGames.SelectedItems.Count == 0)
                return;

            var games = new List<PyramidGame>();
            foreach (ListViewItem item in listViewGames.SelectedItems)
                games.Add((PyramidGame)item.Tag);

            formPyramidGame.Games = games;
            if (formPyramidGame.ShowDialog() == DialogResult.OK)
            {
                foreach (ListViewItem item in listViewGames.SelectedItems)
                {
                    PyramidGame pg = (PyramidGame)item.Tag;
                    pg.TeamsToTake = formPyramidGame.TeamsToTake;
                    pg.Priority = formPyramidGame.Priority;

                    while (item.SubItems.Count < listViewGames.Columns.Count)
                        item.SubItems.Add("");

                    item.SubItems[ColTeamsToTake].Text = formPyramidGame.TeamsToTake.ToString();
                    item.SubItems[ColPriority].Text = formPyramidGame.Priority.ToString();
                }

                RefreshPyramidDraw();
                CalculateSpins();
            }
        }

        private void ButtonRepechageClick(object sender, EventArgs e)
        {
            textBoxTitle.Text = "Repêchage ";
            textBoxTitle.Focus();
            textBoxTitle.SelectionStart = 10;
        }

        private FormWithdraw formWithdraw;

        /// <summary>Mark team(s) as withdrawn and not eligible for next round.</summary>
        private void ButtonWithdrawClick(object sender, EventArgs e)
        {
            if (formWithdraw == null)
                formWithdraw = new FormWithdraw() { League = Holder.League, Icon = (Icon)((Form)Parent.Parent.Parent).Icon.Clone() };

            if (formWithdraw.ShowDialog() == DialogResult.OK)
				RefreshPyramidDraw();
        }

        /// <summary>Update number of teams from last round and repechage.</summary>
        private void CalculateSpins()
        {
            var takes = new int[Enum.GetValues(typeof(Priority)).Cast<int>().Max() + 1];

            foreach (ListViewItem item in listViewGames.Items)
            {
                PyramidGame pg = (PyramidGame)item.Tag;

                takes[(int)pg.Priority] += pg.TeamsToTake == null ? pg.Game.Teams.Count : (int)pg.TeamsToTake;
			}

			numericTeamsFromLastRound.Value = Math.Min(Math.Min(takes[(int)Priority.Round], Holder.League.Teams.Count), numericTeamsFromLastRound.Maximum);
            numericTeamsFromLastRepechage.Value = Math.Min(Math.Min(takes[(int)Priority.Repechage], Holder.League.Teams.Count), numericTeamsFromLastRepechage.Maximum);
			numericTeamsFromPlanB.Value = Math.Min(Math.Min(takes[(int)Priority.PlanB], Holder.League.Teams.Count), numericTeamsFromPlanB.Maximum);
		}

		private void ListViewGamesDoubleClick(object sender, EventArgs e)
        {
            ButtonEditPyramidGamesClick(sender, e);
        }

        private void PyramidSpinKeyUp(object sender, KeyEventArgs e)
        {
            var _ = ((NumericUpDown)sender).Value;  // This piece of black magic forces the control's ValueChanged to fire after the user edits the text in the control.
        }

        private void PyramidValueChanged(object sender, EventArgs e)
        {
            decimal numberOfTeams = numericTeamsFromLastRound.Value + numericTeamsFromLastRepechage.Value + numericTeamsFromPlanB.Value;
            labelNumberOfTeams.Text = numberOfTeams.ToString();
            labelTeamsPerGame.Text = (numberOfTeams / numericGames.Value).ToString("N2");
            RefreshPyramidDraw();
        }

        private void RefreshPyramidDraw()
        {
            var pyramidGames = new List<PyramidGame>();
            foreach (ListViewItem item in listViewGames.Items)
            {
                var pg = (PyramidGame)item.Tag;
                if (pg.Priority != Priority.Unmarked)
                    pyramidGames.Add((PyramidGame)item.Tag);
            }

            var pr = new PyramidDraw() { CompareRank = radioCompareRank.Checked, TakeTop = radioTakeTop.Checked, League = holder.League };

            (displayReportTaken.Report, displayReportDraw.Report) = pr.Reports(pyramidGames, (int)numericGames.Value,
                (int)numericTeamsFromLastRound.Value, (int)numericTeamsFromLastRepechage.Value, (int)numericTeamsFromPlanB.Value, textBoxTitle.Text, checkBoxColour.Checked);

			// Make appropriate items visible in the report key, and nicely space them.
			int x = labelKey.Left + labelKey.Width + 6;
			SetKeyLabelVisible(labelKeyRound, ref x);
			SetKeyLabelVisible(labelKeyRepechage, ref x);
			SetKeyLabelVisible(labelKeyPlanB, ref x);
			SetKeyLabelVisible(labelKeyWithdrawn, ref x);
		}

		private void SetKeyLabelVisible(Label label, ref int x)
        {
			label.Visible = displayReportTaken.Report.Rows.Any(r => r.Valid(1) && r[1].Color == label.BackColor);

			if (label.Visible)
			{
				label.Left = x;
				x += label.Width + 6;
			}
		}
	}
}
