using System;
using System.Linq;
using System.Windows.Forms;
using Torn;
using Torn.Report;

namespace Torn5.Controls
{
    public partial class FramePyramid : UserControl
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
                    if (numericPyramidTeams.Value == 1 && numericPyramidDesiredTeamsPerGame.Value == 1)
                    {
                        string s = holder.League.Title;
                        if (holder.League.Teams.Count > 3)
                        {
                            numericPyramidTeams.Value = holder.League.Teams.Count;
                            if (holder.League.Games(true).Count > 0)
                                numericPyramidDesiredTeamsPerGame.Value = (int)Math.Round(holder.League.Games(true).Average(g => g.Teams.Count));
                            else
                                numericPyramidDesiredTeamsPerGame.Value = (int)Math.Round(Math.Sqrt(holder.League.Teams.Count));
                        }
                        else if (s.IndexOf("Solo", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            numericPyramidTeams.Value = 160;
                            numericPyramidDesiredTeamsPerGame.Value = 20;
                        }
                        else if (s.IndexOf("Double", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            numericPyramidTeams.Value = 56;
                            numericPyramidDesiredTeamsPerGame.Value = 8;
                        }
                        else
                        {
                            numericPyramidTeams.Value = 42;
                            numericPyramidDesiredTeamsPerGame.Value = 6;
                        }

                        NumericPyramidRoundsValueChanged(null, null);
                        ButtonIdealiseClick(null, null);
                    }
                }
            }
        }

        Pyramid Pyramid = new Pyramid();

        public FramePyramid()
        {
            InitializeComponent();
        }

        private void NumericPyramidFinalsGamesValueChanged(object sender, EventArgs e)
        {
            RefreshPyramidFixture();
        }

        void RefreshPyramidFixture()
        {
            if (Pyramid.Rounds.Count > 0)
            {
                displayReportPyramid.Report = Pyramid.Report(Holder.League.Title, (int)numericPyramidFinalsGames.Value, Pyramid.Rounds.Last().TeamsOut);
                textDescription.Text = displayReportPyramid.Report.Description;
            }
        }

        private void ButtonIdealiseClick(object sender, EventArgs e)
        {
            var x = Pyramid.Idealise((int)numericPyramidDesiredTeamsPerGame.Value, (int)numericPyramidTeams.Value);
            labelAdvancePercent.Text = String.Format("{0:0.00%}", x);
        }

        private void NumericPyramidGamesPerTeamValueChanged(object sender, EventArgs e)
        {
            pyramidRound1.RoundGamesPerTeam = (int)numericPyramidGamesPerTeam.Value;
            RefreshPyramidFixture();
        }

        private void NumericPyramidTeamsValueChanged(object sender, EventArgs e)
        {
            pyramidRound1.TeamsIn = (int)numericPyramidTeams.Value;
            RefreshPyramidFixture();
        }

        private void NumericPyramidRoundsValueChanged(object sender, EventArgs e)
        {
            Pyramid.Rounds.Clear();

            pyramidRound1.Visible = numericPyramidRounds.Value >= 1;
            pyramidRound2.Visible = numericPyramidRounds.Value >= 2;
            pyramidRound3.Visible = numericPyramidRounds.Value >= 3;

            if (numericPyramidRounds.Value >= 1)
                Pyramid.Rounds.Add(pyramidRound1);

            if (numericPyramidRounds.Value >= 2)
                Pyramid.Rounds.Add(pyramidRound2);

            if (numericPyramidRounds.Value >= 3)
                Pyramid.Rounds.Add(pyramidRound3);

            RefreshPyramidFixture();
        }

        private void PyramidRoundValueChanged(object sender, EventArgs e)
        {
            int i;
            for (i = 0; i < Pyramid.Rounds.Count - 1; i++)
                Pyramid.Rounds[i + 1].TeamsIn = Pyramid.Rounds[i].TeamsOut;

            labelPyramidFinalsTeams.Text = Pyramid.Rounds[i].TeamsOut.ToString();
            RefreshPyramidFixture();

        }
    }
}
