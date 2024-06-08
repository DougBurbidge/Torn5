﻿
namespace Torn5.Controls
{
	partial class PyramidHalfFixture
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
            this.components = new System.ComponentModel.Container();
            this.labelEquals = new System.Windows.Forms.Label();
            this.labelDivide = new System.Windows.Forms.Label();
            this.numericAdvance = new System.Windows.Forms.NumericUpDown();
            this.numericGames = new System.Windows.Forms.NumericUpDown();
            this.labelAdvancePercent1 = new System.Windows.Forms.Label();
            this.labelAdvance = new System.Windows.Forms.Label();
            this.labelTeamsPerGame1 = new System.Windows.Forms.Label();
            this.labelGames = new System.Windows.Forms.Label();
            this.labelTeams = new System.Windows.Forms.Label();
            this.labelTeamsIn = new System.Windows.Forms.Label();
            this.labelTeamsPerGame = new System.Windows.Forms.Label();
            this.labelAdvancePercent = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.numericAdvance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericGames)).BeginInit();
            this.SuspendLayout();
            // 
            // labelEquals
            // 
            this.labelEquals.AutoSize = true;
            this.labelEquals.Location = new System.Drawing.Point(139, 18);
            this.labelEquals.Name = "labelEquals";
            this.labelEquals.Size = new System.Drawing.Size(13, 13);
            this.labelEquals.TabIndex = 6;
            this.labelEquals.Text = "=";
            // 
            // labelDivide
            // 
            this.labelDivide.AutoSize = true;
            this.labelDivide.Location = new System.Drawing.Point(64, 18);
            this.labelDivide.Name = "labelDivide";
            this.labelDivide.Size = new System.Drawing.Size(13, 13);
            this.labelDivide.TabIndex = 2;
            this.labelDivide.Text = "÷";
            // 
            // numericAdvance
            // 
            this.numericAdvance.Location = new System.Drawing.Point(238, 16);
            this.numericAdvance.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericAdvance.Name = "numericAdvance";
            this.numericAdvance.Size = new System.Drawing.Size(40, 20);
            this.numericAdvance.TabIndex = 9;
            this.numericAdvance.ValueChanged += new System.EventHandler(this.NumericChanged);
            this.numericAdvance.KeyUp += new System.Windows.Forms.KeyEventHandler(this.NumericKeyUp);
            // 
            // numericGames
            // 
            this.numericGames.Location = new System.Drawing.Point(88, 16);
            this.numericGames.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericGames.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericGames.Name = "numericGames";
            this.numericGames.Size = new System.Drawing.Size(40, 20);
            this.numericGames.TabIndex = 4;
            this.numericGames.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericGames.ValueChanged += new System.EventHandler(this.NumericChanged);
            this.numericGames.KeyUp += new System.Windows.Forms.KeyEventHandler(this.NumericKeyUp);
            // 
            // labelAdvancePercent1
            // 
            this.labelAdvancePercent1.AutoSize = true;
            this.labelAdvancePercent1.Location = new System.Drawing.Point(300, 0);
            this.labelAdvancePercent1.Name = "labelAdvancePercent1";
            this.labelAdvancePercent1.Size = new System.Drawing.Size(61, 13);
            this.labelAdvancePercent1.TabIndex = 10;
            this.labelAdvancePercent1.Text = "Advance %";
            // 
            // labelAdvance
            // 
            this.labelAdvance.AutoSize = true;
            this.labelAdvance.Location = new System.Drawing.Point(214, 0);
            this.labelAdvance.Name = "labelAdvance";
            this.labelAdvance.Size = new System.Drawing.Size(81, 13);
            this.labelAdvance.TabIndex = 8;
            this.labelAdvance.Text = "Advance teams";
            // 
            // labelTeamsPerGame1
            // 
            this.labelTeamsPerGame1.AutoSize = true;
            this.labelTeamsPerGame1.Location = new System.Drawing.Point(143, 0);
            this.labelTeamsPerGame1.Name = "labelTeamsPerGame1";
            this.labelTeamsPerGame1.Size = new System.Drawing.Size(70, 13);
            this.labelTeamsPerGame1.TabIndex = 5;
            this.labelTeamsPerGame1.Text = "Teams/game";
            // 
            // labelGames
            // 
            this.labelGames.AutoSize = true;
            this.labelGames.Location = new System.Drawing.Point(86, 0);
            this.labelGames.Name = "labelGames";
            this.labelGames.Size = new System.Drawing.Size(40, 13);
            this.labelGames.TabIndex = 3;
            this.labelGames.Text = "Games";
            // 
            // labelTeams
            // 
            this.labelTeams.AutoSize = true;
            this.labelTeams.Location = new System.Drawing.Point(0, 0);
            this.labelTeams.Name = "labelTeams";
            this.labelTeams.Size = new System.Drawing.Size(74, 13);
            this.labelTeams.TabIndex = 0;
            this.labelTeams.Text = "Starting teams";
            // 
            // labelTeamsIn
            // 
            this.labelTeamsIn.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelTeamsIn.Location = new System.Drawing.Point(13, 16);
            this.labelTeamsIn.Name = "labelTeamsIn";
            this.labelTeamsIn.Size = new System.Drawing.Size(40, 19);
            this.labelTeamsIn.TabIndex = 1;
            this.labelTeamsIn.Text = "99";
            this.labelTeamsIn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelTeamsPerGame
            // 
            this.labelTeamsPerGame.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelTeamsPerGame.Location = new System.Drawing.Point(163, 16);
            this.labelTeamsPerGame.Name = "labelTeamsPerGame";
            this.labelTeamsPerGame.Size = new System.Drawing.Size(40, 19);
            this.labelTeamsPerGame.TabIndex = 7;
            this.labelTeamsPerGame.Text = "12.34";
            this.labelTeamsPerGame.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelAdvancePercent
            // 
            this.labelAdvancePercent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelAdvancePercent.Location = new System.Drawing.Point(307, 16);
            this.labelAdvancePercent.Name = "labelAdvancePercent";
            this.labelAdvancePercent.Size = new System.Drawing.Size(50, 19);
            this.labelAdvancePercent.TabIndex = 11;
            this.labelAdvancePercent.Text = "12.34%";
            this.labelAdvancePercent.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // PyramidHalfFixture
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.labelAdvancePercent);
            this.Controls.Add(this.labelTeamsPerGame);
            this.Controls.Add(this.labelTeamsIn);
            this.Controls.Add(this.labelEquals);
            this.Controls.Add(this.labelDivide);
            this.Controls.Add(this.numericAdvance);
            this.Controls.Add(this.numericGames);
            this.Controls.Add(this.labelAdvancePercent1);
            this.Controls.Add(this.labelAdvance);
            this.Controls.Add(this.labelTeamsPerGame1);
            this.Controls.Add(this.labelGames);
            this.Controls.Add(this.labelTeams);
            this.Name = "PyramidHalfFixture";
            this.Size = new System.Drawing.Size(364, 37);
            ((System.ComponentModel.ISupportInitialize)(this.numericAdvance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericGames)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label labelEquals;
		private System.Windows.Forms.Label labelDivide;
		private System.Windows.Forms.NumericUpDown numericAdvance;
		private System.Windows.Forms.NumericUpDown numericGames;
		private System.Windows.Forms.Label labelAdvancePercent1;
		private System.Windows.Forms.Label labelAdvance;
		private System.Windows.Forms.Label labelTeamsPerGame1;
		private System.Windows.Forms.Label labelGames;
		private System.Windows.Forms.Label labelTeams;
        private System.Windows.Forms.Label labelTeamsIn;
        private System.Windows.Forms.Label labelTeamsPerGame;
        private System.Windows.Forms.Label labelAdvancePercent;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}
