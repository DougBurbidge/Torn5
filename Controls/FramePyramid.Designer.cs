namespace Torn5.Controls
{
    partial class FramePyramid
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.labelPyramidFinalsTeams = new System.Windows.Forms.Label();
            this.textDescription = new System.Windows.Forms.TextBox();
            this.numericPyramidFinalsGames = new System.Windows.Forms.NumericUpDown();
            this.label19 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label24 = new System.Windows.Forms.Label();
            this.labelAdvancePercent = new System.Windows.Forms.Label();
            this.buttonIdealise = new System.Windows.Forms.Button();
            this.numericPyramidDesiredTeamsPerGame = new System.Windows.Forms.NumericUpDown();
            this.label21 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.numericPyramidGamesPerTeam = new System.Windows.Forms.NumericUpDown();
            this.numericPyramidTeams = new System.Windows.Forms.NumericUpDown();
            this.label17 = new System.Windows.Forms.Label();
            this.numericPyramidRounds = new System.Windows.Forms.NumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            this.printReportPyramid = new Torn5.Controls.PrintReport();
            this.displayReportPyramid = new Torn5.Controls.DisplayReport();
            this.pyramidRound3 = new Torn5.Controls.PyramidFixture();
            this.pyramidRound2 = new Torn5.Controls.PyramidFixture();
            this.pyramidRound1 = new Torn5.Controls.PyramidFixture();
            ((System.ComponentModel.ISupportInitialize)(this.numericPyramidFinalsGames)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericPyramidDesiredTeamsPerGame)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericPyramidGamesPerTeam)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericPyramidTeams)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericPyramidRounds)).BeginInit();
            this.SuspendLayout();
            // 
            // labelPyramidFinalsTeams
            // 
            this.labelPyramidFinalsTeams.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelPyramidFinalsTeams.Location = new System.Drawing.Point(73, 318);
            this.labelPyramidFinalsTeams.Name = "labelPyramidFinalsTeams";
            this.labelPyramidFinalsTeams.Size = new System.Drawing.Size(40, 19);
            this.labelPyramidFinalsTeams.TabIndex = 52;
            this.labelPyramidFinalsTeams.Text = "8";
            this.labelPyramidFinalsTeams.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textDescription
            // 
            this.textDescription.BackColor = System.Drawing.SystemColors.Window;
            this.textDescription.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textDescription.Location = new System.Drawing.Point(910, 121);
            this.textDescription.Multiline = true;
            this.textDescription.Name = "textDescription";
            this.textDescription.ReadOnly = true;
            this.textDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textDescription.Size = new System.Drawing.Size(600, 200);
            this.textDescription.TabIndex = 49;
            this.textDescription.Text = "textDescription";
            // 
            // numericPyramidFinalsGames
            // 
            this.numericPyramidFinalsGames.Location = new System.Drawing.Point(148, 318);
            this.numericPyramidFinalsGames.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.numericPyramidFinalsGames.Name = "numericPyramidFinalsGames";
            this.numericPyramidFinalsGames.Size = new System.Drawing.Size(40, 20);
            this.numericPyramidFinalsGames.TabIndex = 48;
            this.numericPyramidFinalsGames.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.numericPyramidFinalsGames.ValueChanged += new System.EventHandler(this.NumericPyramidFinalsGamesValueChanged);
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(135, 302);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(78, 13);
            this.label19.TabIndex = 47;
            this.label19.Text = "Games in finals";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(47, 302);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(77, 13);
            this.label18.TabIndex = 46;
            this.label18.Text = "Teams in finals";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label24);
            this.panel1.Controls.Add(this.labelAdvancePercent);
            this.panel1.Controls.Add(this.buttonIdealise);
            this.panel1.Controls.Add(this.numericPyramidDesiredTeamsPerGame);
            this.panel1.Controls.Add(this.label21);
            this.panel1.Controls.Add(this.label20);
            this.panel1.Controls.Add(this.numericPyramidGamesPerTeam);
            this.panel1.Controls.Add(this.numericPyramidTeams);
            this.panel1.Controls.Add(this.label17);
            this.panel1.Controls.Add(this.numericPyramidRounds);
            this.panel1.Controls.Add(this.label11);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1224, 100);
            this.panel1.TabIndex = 42;
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(801, 33);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(86, 13);
            this.label24.TabIndex = 9;
            this.label24.Text = "Ideal advance %";
            // 
            // labelAdvancePercent
            // 
            this.labelAdvancePercent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelAdvancePercent.Location = new System.Drawing.Point(833, 49);
            this.labelAdvancePercent.Name = "labelAdvancePercent";
            this.labelAdvancePercent.Size = new System.Drawing.Size(50, 19);
            this.labelAdvancePercent.TabIndex = 10;
            this.labelAdvancePercent.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonIdealise
            // 
            this.buttonIdealise.Location = new System.Drawing.Point(437, 46);
            this.buttonIdealise.Name = "buttonIdealise";
            this.buttonIdealise.Size = new System.Drawing.Size(75, 23);
            this.buttonIdealise.TabIndex = 8;
            this.buttonIdealise.Text = "Idealise";
            this.buttonIdealise.UseVisualStyleBackColor = true;
            this.buttonIdealise.Click += new System.EventHandler(this.ButtonIdealiseClick);
            // 
            // numericPyramidDesiredTeamsPerGame
            // 
            this.numericPyramidDesiredTeamsPerGame.Location = new System.Drawing.Point(223, 49);
            this.numericPyramidDesiredTeamsPerGame.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericPyramidDesiredTeamsPerGame.Name = "numericPyramidDesiredTeamsPerGame";
            this.numericPyramidDesiredTeamsPerGame.Size = new System.Drawing.Size(40, 20);
            this.numericPyramidDesiredTeamsPerGame.TabIndex = 5;
            this.numericPyramidDesiredTeamsPerGame.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(189, 33);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(105, 13);
            this.label21.TabIndex = 4;
            this.label21.Text = "Desired teams/game";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(295, 33);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(123, 13);
            this.label20.TabIndex = 6;
            this.label20.Text = "Games/team in Round 1";
            // 
            // numericPyramidGamesPerTeam
            // 
            this.numericPyramidGamesPerTeam.Location = new System.Drawing.Point(298, 49);
            this.numericPyramidGamesPerTeam.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericPyramidGamesPerTeam.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericPyramidGamesPerTeam.Name = "numericPyramidGamesPerTeam";
            this.numericPyramidGamesPerTeam.Size = new System.Drawing.Size(40, 20);
            this.numericPyramidGamesPerTeam.TabIndex = 7;
            this.numericPyramidGamesPerTeam.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numericPyramidGamesPerTeam.ValueChanged += new System.EventHandler(this.NumericPyramidGamesPerTeamValueChanged);
            // 
            // numericPyramidTeams
            // 
            this.numericPyramidTeams.Location = new System.Drawing.Point(73, 49);
            this.numericPyramidTeams.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericPyramidTeams.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericPyramidTeams.Name = "numericPyramidTeams";
            this.numericPyramidTeams.Size = new System.Drawing.Size(40, 20);
            this.numericPyramidTeams.TabIndex = 1;
            this.numericPyramidTeams.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericPyramidTeams.ValueChanged += new System.EventHandler(this.NumericPyramidTeamsValueChanged);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(60, 33);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(74, 13);
            this.label17.TabIndex = 0;
            this.label17.Text = "Starting teams";
            // 
            // numericPyramidRounds
            // 
            this.numericPyramidRounds.Location = new System.Drawing.Point(148, 49);
            this.numericPyramidRounds.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.numericPyramidRounds.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericPyramidRounds.Name = "numericPyramidRounds";
            this.numericPyramidRounds.Size = new System.Drawing.Size(40, 20);
            this.numericPyramidRounds.TabIndex = 3;
            this.numericPyramidRounds.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.numericPyramidRounds.ValueChanged += new System.EventHandler(this.NumericPyramidRoundsValueChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(146, 33);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(44, 13);
            this.label11.TabIndex = 2;
            this.label11.Text = "Rounds";
            // 
            // printReportPyramid
            // 
            this.printReportPyramid.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.printReportPyramid.DisplayReport = this.displayReportPyramid;
            this.printReportPyramid.Location = new System.Drawing.Point(1156, 327);
            this.printReportPyramid.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.printReportPyramid.Name = "printReportPyramid";
            this.printReportPyramid.Size = new System.Drawing.Size(64, 473);
            this.printReportPyramid.TabIndex = 51;
            // 
            // displayReportPyramid
            // 
            this.displayReportPyramid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.displayReportPyramid.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.displayReportPyramid.Location = new System.Drawing.Point(4, 356);
            this.displayReportPyramid.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.displayReportPyramid.Name = "displayReportPyramid";
            this.displayReportPyramid.Report = null;
            this.displayReportPyramid.Size = new System.Drawing.Size(1151, 345);
            this.displayReportPyramid.TabIndex = 50;
            // 
            // pyramidRound3
            // 
            this.pyramidRound3.HasRepechage = true;
            this.pyramidRound3.Location = new System.Drawing.Point(3, 234);
            this.pyramidRound3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pyramidRound3.Name = "pyramidRound3";
            this.pyramidRound3.RepechageAdvance = 4;
            this.pyramidRound3.RepechageGames = 2;
            this.pyramidRound3.Round = 3;
            this.pyramidRound3.RoundAdvance = 4;
            this.pyramidRound3.RoundGames = 2;
            this.pyramidRound3.RoundGamesPerTeam = 1;
            this.pyramidRound3.Size = new System.Drawing.Size(914, 66);
            this.pyramidRound3.TabIndex = 45;
            this.pyramidRound3.TeamsIn = 16;
            this.pyramidRound3.ValueChanged += new System.EventHandler(this.PyramidRoundValueChanged);
            // 
            // pyramidRound2
            // 
            this.pyramidRound2.HasRepechage = true;
            this.pyramidRound2.Location = new System.Drawing.Point(3, 168);
            this.pyramidRound2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pyramidRound2.Name = "pyramidRound2";
            this.pyramidRound2.RepechageAdvance = 8;
            this.pyramidRound2.RepechageGames = 3;
            this.pyramidRound2.Round = 2;
            this.pyramidRound2.RoundAdvance = 8;
            this.pyramidRound2.RoundGames = 4;
            this.pyramidRound2.RoundGamesPerTeam = 1;
            this.pyramidRound2.Size = new System.Drawing.Size(914, 66);
            this.pyramidRound2.TabIndex = 44;
            this.pyramidRound2.TeamsIn = 32;
            this.pyramidRound2.ValueChanged += new System.EventHandler(this.PyramidRoundValueChanged);
            // 
            // pyramidRound1
            // 
            this.pyramidRound1.HasRepechage = true;
            this.pyramidRound1.Location = new System.Drawing.Point(3, 102);
            this.pyramidRound1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pyramidRound1.Name = "pyramidRound1";
            this.pyramidRound1.RepechageAdvance = 8;
            this.pyramidRound1.RepechageGames = 4;
            this.pyramidRound1.Round = 1;
            this.pyramidRound1.RoundAdvance = 24;
            this.pyramidRound1.RoundGames = 14;
            this.pyramidRound1.RoundGamesPerTeam = 2;
            this.pyramidRound1.Size = new System.Drawing.Size(914, 66);
            this.pyramidRound1.TabIndex = 43;
            this.pyramidRound1.TeamsIn = 56;
            this.pyramidRound1.ValueChanged += new System.EventHandler(this.PyramidRoundValueChanged);
            // 
            // FramePyramid
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.labelPyramidFinalsTeams);
            this.Controls.Add(this.textDescription);
            this.Controls.Add(this.numericPyramidFinalsGames);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.printReportPyramid);
            this.Controls.Add(this.displayReportPyramid);
            this.Controls.Add(this.pyramidRound3);
            this.Controls.Add(this.pyramidRound2);
            this.Controls.Add(this.pyramidRound1);
            this.Name = "FramePyramid";
            this.Size = new System.Drawing.Size(1224, 701);
            ((System.ComponentModel.ISupportInitialize)(this.numericPyramidFinalsGames)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericPyramidDesiredTeamsPerGame)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericPyramidGamesPerTeam)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericPyramidTeams)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericPyramidRounds)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelPyramidFinalsTeams;
        private System.Windows.Forms.TextBox textDescription;
        private System.Windows.Forms.NumericUpDown numericPyramidFinalsGames;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label labelAdvancePercent;
        private System.Windows.Forms.Button buttonIdealise;
        private System.Windows.Forms.NumericUpDown numericPyramidDesiredTeamsPerGame;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.NumericUpDown numericPyramidGamesPerTeam;
        private System.Windows.Forms.NumericUpDown numericPyramidTeams;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.NumericUpDown numericPyramidRounds;
        private System.Windows.Forms.Label label11;
        private PrintReport printReportPyramid;
        private DisplayReport displayReportPyramid;
        private PyramidFixture pyramidRound3;
        private PyramidFixture pyramidRound2;
        private PyramidFixture pyramidRound1;
    }
}
