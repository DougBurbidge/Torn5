
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
			this.labelEquals = new System.Windows.Forms.Label();
			this.labelDivide = new System.Windows.Forms.Label();
			this.numericAdvancePercent = new System.Windows.Forms.NumericUpDown();
			this.numericAdvance = new System.Windows.Forms.NumericUpDown();
			this.numericTeamsPerGame = new System.Windows.Forms.NumericUpDown();
			this.numericGames = new System.Windows.Forms.NumericUpDown();
			this.numericTeams = new System.Windows.Forms.NumericUpDown();
			this.labelAdvancePercent = new System.Windows.Forms.Label();
			this.labelAdvance = new System.Windows.Forms.Label();
			this.labelTeamsPerGame = new System.Windows.Forms.Label();
			this.labelGames = new System.Windows.Forms.Label();
			this.labelTeams = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.numericAdvancePercent)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericAdvance)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericTeamsPerGame)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericGames)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericTeams)).BeginInit();
			this.SuspendLayout();
			// 
			// labelEquals
			// 
			this.labelEquals.AutoSize = true;
			this.labelEquals.Location = new System.Drawing.Point(139, 18);
			this.labelEquals.Name = "labelEquals";
			this.labelEquals.Size = new System.Drawing.Size(13, 13);
			this.labelEquals.TabIndex = 36;
			this.labelEquals.Text = "=";
			// 
			// labelDivide
			// 
			this.labelDivide.AutoSize = true;
			this.labelDivide.Location = new System.Drawing.Point(65, 18);
			this.labelDivide.Name = "labelDivide";
			this.labelDivide.Size = new System.Drawing.Size(13, 13);
			this.labelDivide.TabIndex = 35;
			this.labelDivide.Text = "÷";
			// 
			// numericAdvancePercent
			// 
			this.numericAdvancePercent.DecimalPlaces = 1;
			this.numericAdvancePercent.Enabled = false;
			this.numericAdvancePercent.Location = new System.Drawing.Point(303, 16);
			this.numericAdvancePercent.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
			this.numericAdvancePercent.Name = "numericAdvancePercent";
			this.numericAdvancePercent.Size = new System.Drawing.Size(60, 20);
			this.numericAdvancePercent.TabIndex = 34;
			// 
			// numericAdvance
			// 
			this.numericAdvance.Location = new System.Drawing.Point(228, 16);
			this.numericAdvance.Name = "numericAdvance";
			this.numericAdvance.Size = new System.Drawing.Size(60, 20);
			this.numericAdvance.TabIndex = 33;
			this.numericAdvance.ValueChanged += new System.EventHandler(this.NumericChanged);
			this.numericAdvance.KeyUp += new System.Windows.Forms.KeyEventHandler(this.NumericKeyUp);
			// 
			// numericTeamsPerGame
			// 
			this.numericTeamsPerGame.DecimalPlaces = 2;
			this.numericTeamsPerGame.Enabled = false;
			this.numericTeamsPerGame.Location = new System.Drawing.Point(153, 16);
			this.numericTeamsPerGame.Name = "numericTeamsPerGame";
			this.numericTeamsPerGame.Size = new System.Drawing.Size(60, 20);
			this.numericTeamsPerGame.TabIndex = 32;
			// 
			// numericGames
			// 
			this.numericGames.Location = new System.Drawing.Point(78, 16);
			this.numericGames.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericGames.Name = "numericGames";
			this.numericGames.Size = new System.Drawing.Size(60, 20);
			this.numericGames.TabIndex = 31;
			this.numericGames.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericGames.ValueChanged += new System.EventHandler(this.NumericChanged);
			this.numericGames.KeyUp += new System.Windows.Forms.KeyEventHandler(this.NumericKeyUp);
			// 
			// numericTeams
			// 
			this.numericTeams.Enabled = false;
			this.numericTeams.Location = new System.Drawing.Point(3, 16);
			this.numericTeams.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this.numericTeams.Name = "numericTeams";
			this.numericTeams.Size = new System.Drawing.Size(60, 20);
			this.numericTeams.TabIndex = 30;
			this.numericTeams.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// labelAdvancePercent
			// 
			this.labelAdvancePercent.AutoSize = true;
			this.labelAdvancePercent.Location = new System.Drawing.Point(300, 0);
			this.labelAdvancePercent.Name = "labelAdvancePercent";
			this.labelAdvancePercent.Size = new System.Drawing.Size(61, 13);
			this.labelAdvancePercent.TabIndex = 29;
			this.labelAdvancePercent.Text = "Advance %";
			// 
			// labelAdvance
			// 
			this.labelAdvance.AutoSize = true;
			this.labelAdvance.Location = new System.Drawing.Point(212, 0);
			this.labelAdvance.Name = "labelAdvance";
			this.labelAdvance.Size = new System.Drawing.Size(81, 13);
			this.labelAdvance.TabIndex = 28;
			this.labelAdvance.Text = "Advance teams";
			// 
			// labelTeamsPerGame
			// 
			this.labelTeamsPerGame.AutoSize = true;
			this.labelTeamsPerGame.Location = new System.Drawing.Point(143, 0);
			this.labelTeamsPerGame.Name = "labelTeamsPerGame";
			this.labelTeamsPerGame.Size = new System.Drawing.Size(70, 13);
			this.labelTeamsPerGame.TabIndex = 27;
			this.labelTeamsPerGame.Text = "Teams/game";
			// 
			// labelGames
			// 
			this.labelGames.AutoSize = true;
			this.labelGames.Location = new System.Drawing.Point(86, 0);
			this.labelGames.Name = "labelGames";
			this.labelGames.Size = new System.Drawing.Size(40, 13);
			this.labelGames.TabIndex = 26;
			this.labelGames.Text = "Games";
			// 
			// labelTeams
			// 
			this.labelTeams.AutoSize = true;
			this.labelTeams.Location = new System.Drawing.Point(0, 0);
			this.labelTeams.Name = "labelTeams";
			this.labelTeams.Size = new System.Drawing.Size(74, 13);
			this.labelTeams.TabIndex = 25;
			this.labelTeams.Text = "Starting teams";
			// 
			// PyramidHalfFixture
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.labelEquals);
			this.Controls.Add(this.labelDivide);
			this.Controls.Add(this.numericAdvancePercent);
			this.Controls.Add(this.numericAdvance);
			this.Controls.Add(this.numericTeamsPerGame);
			this.Controls.Add(this.numericGames);
			this.Controls.Add(this.numericTeams);
			this.Controls.Add(this.labelAdvancePercent);
			this.Controls.Add(this.labelAdvance);
			this.Controls.Add(this.labelTeamsPerGame);
			this.Controls.Add(this.labelGames);
			this.Controls.Add(this.labelTeams);
			this.Name = "PyramidHalfFixture";
			this.Size = new System.Drawing.Size(364, 37);
			((System.ComponentModel.ISupportInitialize)(this.numericAdvancePercent)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericAdvance)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericTeamsPerGame)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericGames)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericTeams)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label labelEquals;
		private System.Windows.Forms.Label labelDivide;
		private System.Windows.Forms.NumericUpDown numericAdvancePercent;
		private System.Windows.Forms.NumericUpDown numericAdvance;
		private System.Windows.Forms.NumericUpDown numericTeamsPerGame;
		private System.Windows.Forms.NumericUpDown numericGames;
		private System.Windows.Forms.NumericUpDown numericTeams;
		private System.Windows.Forms.Label labelAdvancePercent;
		private System.Windows.Forms.Label labelAdvance;
		private System.Windows.Forms.Label labelTeamsPerGame;
		private System.Windows.Forms.Label labelGames;
		private System.Windows.Forms.Label labelTeams;
	}
}
