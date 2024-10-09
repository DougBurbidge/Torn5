namespace Torn5.Controls
{
    partial class FrameFinals
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
            this.label27 = new System.Windows.Forms.Label();
            this.buttonMoveRight = new System.Windows.Forms.Button();
            this.buttonMoveLeft = new System.Windows.Forms.Button();
            this.label25 = new System.Windows.Forms.Label();
            this.buttonDeleteGame = new System.Windows.Forms.Button();
            this.label26 = new System.Windows.Forms.Label();
            this.numericToMove = new System.Windows.Forms.NumericUpDown();
            this.buttonDown = new System.Windows.Forms.Button();
            this.buttonUp = new System.Windows.Forms.Button();
            this.buttonRight = new System.Windows.Forms.Button();
            this.labelSelectedColumn = new System.Windows.Forms.Label();
            this.buttonLeft = new System.Windows.Forms.Button();
            this.printReportFinals = new Torn5.Controls.PrintReport();
            this.displayReportFinals = new Torn5.Controls.DisplayReport();
            this.labelTeamsToSendUp = new System.Windows.Forms.Label();
            this.numericFreeRides = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.buttonFormatD = new System.Windows.Forms.Button();
            this.buttonTwoTrack = new System.Windows.Forms.Button();
            this.numericTeamsPerGame = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.numericTeamsToCut = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.numericTracks = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.buttonAscension = new System.Windows.Forms.Button();
            this.numericTeams = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericToMove)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericFreeRides)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericTeamsPerGame)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericTeamsToCut)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericTracks)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericTeams)).BeginInit();
            this.SuspendLayout();
            // 
            // label27
            // 
            this.label27.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(360, 653);
            this.label27.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(71, 13);
            this.label27.TabIndex = 54;
            this.label27.Text = "Move column";
            // 
            // buttonMoveRight
            // 
            this.buttonMoveRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonMoveRight.Location = new System.Drawing.Point(400, 669);
            this.buttonMoveRight.Name = "buttonMoveRight";
            this.buttonMoveRight.Size = new System.Drawing.Size(23, 23);
            this.buttonMoveRight.TabIndex = 53;
            this.buttonMoveRight.Tag = "1";
            this.buttonMoveRight.Text = "▶\t";
            this.buttonMoveRight.UseVisualStyleBackColor = true;
            this.buttonMoveRight.Click += new System.EventHandler(this.ButtonMoveLeftRightClick);
            // 
            // buttonMoveLeft
            // 
            this.buttonMoveLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonMoveLeft.Location = new System.Drawing.Point(367, 669);
            this.buttonMoveLeft.Name = "buttonMoveLeft";
            this.buttonMoveLeft.Size = new System.Drawing.Size(23, 23);
            this.buttonMoveLeft.TabIndex = 52;
            this.buttonMoveLeft.Tag = "-1";
            this.buttonMoveLeft.Text = "◀\t";
            this.buttonMoveLeft.UseVisualStyleBackColor = true;
            this.buttonMoveLeft.Click += new System.EventHandler(this.ButtonMoveLeftRightClick);
            // 
            // label25
            // 
            this.label25.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(21, 653);
            this.label25.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(74, 13);
            this.label25.TabIndex = 51;
            this.label25.Text = "Select column";
            // 
            // buttonDeleteGame
            // 
            this.buttonDeleteGame.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonDeleteGame.Location = new System.Drawing.Point(496, 669);
            this.buttonDeleteGame.Name = "buttonDeleteGame";
            this.buttonDeleteGame.Size = new System.Drawing.Size(77, 23);
            this.buttonDeleteGame.TabIndex = 50;
            this.buttonDeleteGame.Text = "Delete Game";
            this.buttonDeleteGame.UseVisualStyleBackColor = true;
            this.buttonDeleteGame.Click += new System.EventHandler(this.ButtonDeleteGameClick);
            // 
            // label26
            // 
            this.label26.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(149, 653);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(174, 13);
            this.label26.TabIndex = 49;
            this.label26.Text = "Number of teams to move up/down";
            // 
            // numericToMove
            // 
            this.numericToMove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.numericToMove.Location = new System.Drawing.Point(152, 673);
            this.numericToMove.Name = "numericToMove";
            this.numericToMove.Size = new System.Drawing.Size(50, 20);
            this.numericToMove.TabIndex = 48;
            this.numericToMove.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // buttonDown
            // 
            this.buttonDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonDown.Location = new System.Drawing.Point(120, 673);
            this.buttonDown.Name = "buttonDown";
            this.buttonDown.Size = new System.Drawing.Size(23, 23);
            this.buttonDown.TabIndex = 47;
            this.buttonDown.Text = "▼";
            this.buttonDown.UseVisualStyleBackColor = true;
            this.buttonDown.Click += new System.EventHandler(this.ButtonDownClick);
            // 
            // buttonUp
            // 
            this.buttonUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonUp.Location = new System.Drawing.Point(120, 648);
            this.buttonUp.Name = "buttonUp";
            this.buttonUp.Size = new System.Drawing.Size(23, 23);
            this.buttonUp.TabIndex = 46;
            this.buttonUp.Text = "▲";
            this.buttonUp.UseVisualStyleBackColor = true;
            this.buttonUp.Click += new System.EventHandler(this.ButtonUpClick);
            // 
            // buttonRight
            // 
            this.buttonRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonRight.Location = new System.Drawing.Point(73, 669);
            this.buttonRight.Name = "buttonRight";
            this.buttonRight.Size = new System.Drawing.Size(23, 23);
            this.buttonRight.TabIndex = 45;
            this.buttonRight.Tag = "1";
            this.buttonRight.Text = "▶\t";
            this.buttonRight.UseVisualStyleBackColor = true;
            this.buttonRight.Click += new System.EventHandler(this.ButtonLeftRightClick);
            // 
            // labelSelectedColumn
            // 
            this.labelSelectedColumn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelSelectedColumn.AutoSize = true;
            this.labelSelectedColumn.Location = new System.Drawing.Point(51, 676);
            this.labelSelectedColumn.Name = "labelSelectedColumn";
            this.labelSelectedColumn.Size = new System.Drawing.Size(16, 13);
            this.labelSelectedColumn.TabIndex = 44;
            this.labelSelectedColumn.Text = "...";
            // 
            // buttonLeft
            // 
            this.buttonLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonLeft.Location = new System.Drawing.Point(20, 669);
            this.buttonLeft.Name = "buttonLeft";
            this.buttonLeft.Size = new System.Drawing.Size(23, 23);
            this.buttonLeft.TabIndex = 43;
            this.buttonLeft.Tag = "-1";
            this.buttonLeft.Text = "◀\t";
            this.buttonLeft.UseVisualStyleBackColor = true;
            this.buttonLeft.Click += new System.EventHandler(this.ButtonLeftRightClick);
            // 
            // printReportFinals
            // 
            this.printReportFinals.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.printReportFinals.DisplayReport = this.displayReportFinals;
            this.printReportFinals.Location = new System.Drawing.Point(1156, 6);
            this.printReportFinals.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.printReportFinals.Name = "printReportFinals";
            this.printReportFinals.Size = new System.Drawing.Size(64, 475);
            this.printReportFinals.TabIndex = 42;
            // 
            // displayReportFinals
            // 
            this.displayReportFinals.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.displayReportFinals.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.displayReportFinals.Location = new System.Drawing.Point(5, 71);
            this.displayReportFinals.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.displayReportFinals.Name = "displayReportFinals";
            this.displayReportFinals.Report = null;
            this.displayReportFinals.Size = new System.Drawing.Size(1145, 559);
            this.displayReportFinals.TabIndex = 41;
            // 
            // labelTeamsToSendUp
            // 
            this.labelTeamsToSendUp.AutoSize = true;
            this.labelTeamsToSendUp.Location = new System.Drawing.Point(627, 6);
            this.labelTeamsToSendUp.Name = "labelTeamsToSendUp";
            this.labelTeamsToSendUp.Size = new System.Drawing.Size(174, 13);
            this.labelTeamsToSendUp.TabIndex = 40;
            this.labelTeamsToSendUp.Text = "Teams to send up from each game:";
            // 
            // numericFreeRides
            // 
            this.numericFreeRides.Location = new System.Drawing.Point(571, 30);
            this.numericFreeRides.Maximum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numericFreeRides.Name = "numericFreeRides";
            this.numericFreeRides.Size = new System.Drawing.Size(50, 20);
            this.numericFreeRides.TabIndex = 39;
            this.numericFreeRides.ValueChanged += new System.EventHandler(this.RefreshFinals);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(365, 32);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(200, 13);
            this.label10.TabIndex = 38;
            this.label10.Text = "Teams that get a free ride to grand finals:";
            // 
            // buttonFormatD
            // 
            this.buttonFormatD.Location = new System.Drawing.Point(274, 30);
            this.buttonFormatD.Name = "buttonFormatD";
            this.buttonFormatD.Size = new System.Drawing.Size(75, 23);
            this.buttonFormatD.TabIndex = 37;
            this.buttonFormatD.Text = "Format D";
            this.buttonFormatD.UseVisualStyleBackColor = true;
            this.buttonFormatD.Click += new System.EventHandler(this.ButtonFormatDClick);
            // 
            // buttonTwoTrack
            // 
            this.buttonTwoTrack.Location = new System.Drawing.Point(193, 30);
            this.buttonTwoTrack.Name = "buttonTwoTrack";
            this.buttonTwoTrack.Size = new System.Drawing.Size(75, 23);
            this.buttonTwoTrack.TabIndex = 36;
            this.buttonTwoTrack.Text = "Two Track";
            this.buttonTwoTrack.UseVisualStyleBackColor = true;
            this.buttonTwoTrack.Click += new System.EventHandler(this.ButtonTwoTrackClick);
            // 
            // numericTeamsPerGame
            // 
            this.numericTeamsPerGame.Location = new System.Drawing.Point(309, 4);
            this.numericTeamsPerGame.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.numericTeamsPerGame.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericTeamsPerGame.Name = "numericTeamsPerGame";
            this.numericTeamsPerGame.Size = new System.Drawing.Size(50, 20);
            this.numericTeamsPerGame.TabIndex = 32;
            this.numericTeamsPerGame.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.numericTeamsPerGame.ValueChanged += new System.EventHandler(this.NumericTeamsPerGameValueChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(214, 6);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(89, 13);
            this.label9.TabIndex = 31;
            this.label9.Text = "Teams per game:";
            // 
            // numericTeamsToCut
            // 
            this.numericTeamsToCut.Location = new System.Drawing.Point(571, 4);
            this.numericTeamsToCut.Maximum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numericTeamsToCut.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericTeamsToCut.Name = "numericTeamsToCut";
            this.numericTeamsToCut.Size = new System.Drawing.Size(50, 20);
            this.numericTeamsToCut.TabIndex = 34;
            this.numericTeamsToCut.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numericTeamsToCut.ValueChanged += new System.EventHandler(this.RefreshFinals);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(365, 6);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(188, 13);
            this.label8.TabIndex = 33;
            this.label8.Text = "Teams to send down from each game:";
            // 
            // numericTracks
            // 
            this.numericTracks.Location = new System.Drawing.Point(158, 4);
            this.numericTracks.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.numericTracks.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericTracks.Name = "numericTracks";
            this.numericTracks.Size = new System.Drawing.Size(50, 20);
            this.numericTracks.TabIndex = 30;
            this.numericTracks.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numericTracks.ValueChanged += new System.EventHandler(this.RefreshFinals);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(109, 6);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(43, 13);
            this.label7.TabIndex = 29;
            this.label7.Text = "Tracks:";
            // 
            // buttonAscension
            // 
            this.buttonAscension.Location = new System.Drawing.Point(112, 30);
            this.buttonAscension.Name = "buttonAscension";
            this.buttonAscension.Size = new System.Drawing.Size(75, 23);
            this.buttonAscension.TabIndex = 35;
            this.buttonAscension.Text = "Ascension";
            this.buttonAscension.UseVisualStyleBackColor = true;
            this.buttonAscension.Click += new System.EventHandler(this.ButtonAscensionClick);
            // 
            // numericTeams
            // 
            this.numericTeams.Location = new System.Drawing.Point(53, 4);
            this.numericTeams.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.numericTeams.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericTeams.Name = "numericTeams";
            this.numericTeams.Size = new System.Drawing.Size(50, 20);
            this.numericTeams.TabIndex = 57;
            this.numericTeams.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 13);
            this.label1.TabIndex = 56;
            this.label1.Text = "Teams:";
            // 
            // FrameFinals
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.numericTeams);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label27);
            this.Controls.Add(this.buttonMoveRight);
            this.Controls.Add(this.buttonMoveLeft);
            this.Controls.Add(this.label25);
            this.Controls.Add(this.buttonDeleteGame);
            this.Controls.Add(this.label26);
            this.Controls.Add(this.numericToMove);
            this.Controls.Add(this.buttonDown);
            this.Controls.Add(this.buttonUp);
            this.Controls.Add(this.buttonRight);
            this.Controls.Add(this.labelSelectedColumn);
            this.Controls.Add(this.buttonLeft);
            this.Controls.Add(this.printReportFinals);
            this.Controls.Add(this.displayReportFinals);
            this.Controls.Add(this.labelTeamsToSendUp);
            this.Controls.Add(this.numericFreeRides);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.buttonFormatD);
            this.Controls.Add(this.buttonTwoTrack);
            this.Controls.Add(this.numericTeamsPerGame);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.numericTeamsToCut);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.numericTracks);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.buttonAscension);
            this.Name = "FrameFinals";
            this.Size = new System.Drawing.Size(1224, 701);
            this.Enter += new System.EventHandler(this.RefreshFinals);
            ((System.ComponentModel.ISupportInitialize)(this.numericToMove)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericFreeRides)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericTeamsPerGame)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericTeamsToCut)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericTracks)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericTeams)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Button buttonMoveRight;
        private System.Windows.Forms.Button buttonMoveLeft;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Button buttonDeleteGame;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.NumericUpDown numericToMove;
        private System.Windows.Forms.Button buttonDown;
        private System.Windows.Forms.Button buttonUp;
        private System.Windows.Forms.Button buttonRight;
        private System.Windows.Forms.Label labelSelectedColumn;
        private System.Windows.Forms.Button buttonLeft;
        private PrintReport printReportFinals;
        private DisplayReport displayReportFinals;
        private System.Windows.Forms.Label labelTeamsToSendUp;
        private System.Windows.Forms.NumericUpDown numericFreeRides;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button buttonFormatD;
        private System.Windows.Forms.Button buttonTwoTrack;
        private System.Windows.Forms.NumericUpDown numericTeamsPerGame;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown numericTeamsToCut;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown numericTracks;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button buttonAscension;
        private System.Windows.Forms.NumericUpDown numericTeams;
        private System.Windows.Forms.Label label1;
    }
}
