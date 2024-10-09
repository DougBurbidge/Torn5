using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Torn;
using Torn.Report;

namespace Torn5.Controls
{
    public partial class FrameFinals : UserControl
    {
        Holder holder;
        public Holder Holder
        {
            get { return holder; }

            set
            {
                holder = value;
                if (holder != null)
                {
                    numericTeams.Value = holder.League.Teams.Count;

                    if (Holder.League.AllGames.Any())
                        numericTeamsPerGame.Value = (decimal)Math.Round(Holder.League.AllGames.Average(g => g.Teams.Count));

                    RefreshFinals(null, null);
                }
            }
        }

        public FrameFinals()
        {
            InitializeComponent();
        }

        private void RefreshFinals(object sender, EventArgs e)
        {
            labelTeamsToSendUp.Text = "Teams to send up from each game: " + (numericTeamsPerGame.Value - numericTeamsToCut.Value).ToString();

            if (holder != null)
                displayReportFinals.Report = Finals.Ascension(Holder.Fixture, (int)numericTeamsPerGame.Value, (int)numericTeamsToCut.Value, (int)numericTracks.Value, (int)numericFreeRides.Value);
        }

        private void NumericTeamsPerGameValueChanged(object sender, EventArgs e)
        {
            numericTeamsToCut.Maximum = numericTeamsPerGame.Value - 1;
            numericFreeRides.Maximum = numericTeamsPerGame.Value - 1;

            RefreshFinals(sender, e);
        }

        private void ButtonAscensionClick(object sender, EventArgs e)
        {
            numericTracks.Value = 1;
            numericTeamsToCut.Value = 1;
            numericFreeRides.Value = 1;
        }

        private void ButtonTwoTrackClick(object sender, EventArgs e)
        {
            numericTracks.Value = 2;
            numericTeamsToCut.Value = 2;
            numericFreeRides.Value = 0;
        }

        private void ButtonFormatDClick(object sender, EventArgs e)
        {
            numericTracks.Value = 3;
            numericTeamsToCut.Value = 2;
            numericFreeRides.Value = 0;
        }

        int selectedColumn = 0;
        private void ButtonLeftRightClick(object sender, EventArgs e)
        {
            var cols = displayReportFinals.Report.Columns;

            if (!cols.Any())
                return;

            cols[selectedColumn].Color = Color.Empty;

            if (sender == buttonLeft)
            {
                selectedColumn--;
                if (selectedColumn < 0)
                    selectedColumn = cols.Count - 1;
            }
            else
                selectedColumn++;

            HighlightSelectedColumn();
        }

        private void HighlightSelectedColumn()
        {
            var cols = displayReportFinals.Report.Columns;

            if (cols.Valid(selectedColumn))
            {
                cols[selectedColumn].Color = Color.Black;
                labelSelectedColumn.Text = cols[selectedColumn].Text;
            }
            else
            {
                selectedColumn = 0;
                labelSelectedColumn.Text = "";
            }

            displayReportFinals.Redraw();
        }

        private void ButtonUpClick(object sender, EventArgs e)
        {
            var r = displayReportFinals.Report;

            var topCell = r.Rows[0][selectedColumn];

            int i;
            int moved = 0;
            for (i = 0; i < r.Rows.Count - 1 && moved < numericToMove.Value; i++)
            {
                r.Rows[i][selectedColumn] = r.Rows[i + 1][selectedColumn];

                if (r.Rows[i][selectedColumn].Border != Color.Empty)
                    moved++;
            }
            r.Rows[i][selectedColumn] = topCell;

            displayReportFinals.Redraw();
        }

        private void ButtonDownClick(object sender, EventArgs e)
        {
            var r = displayReportFinals.Report;

            var bottomCell = r.Rows.Last()[selectedColumn];

            int i;
            int moved = 0;
            for (i = r.Rows.Count - 1; i > 0 && moved < numericToMove.Value; i--)
            {
                r.Rows[i][selectedColumn] = r.Rows[i - 1][selectedColumn];

                if (r.Rows[i][selectedColumn].Border != Color.Empty)
                    moved++;
            }
            r.Rows[i][selectedColumn] = bottomCell;

            displayReportFinals.Redraw();
        }

        private void ButtonMoveLeftRightClick(object sender, EventArgs e)
        {
            int otherColumn = sender == buttonMoveLeft ? selectedColumn - 1 : selectedColumn + 1;

            if (otherColumn == 0 || otherColumn == displayReportFinals.Report.Columns.Count)
                return;

            foreach (var row in displayReportFinals.Report.Rows)
                if (row.Valid(selectedColumn) && row.Valid(otherColumn))
                    (row[selectedColumn], row[otherColumn]) = (row[otherColumn], row[selectedColumn]);  // Swap the cells within the row, using tuples.

            displayReportFinals.Report.Columns[selectedColumn].Color = Color.Empty;
            selectedColumn = otherColumn;
            HighlightSelectedColumn();
        }

        private void ButtonDeleteGameClick(object sender, EventArgs e)
        {
            displayReportFinals.Report.RemoveColumn(selectedColumn);
            HighlightSelectedColumn();
        }
    }
}
