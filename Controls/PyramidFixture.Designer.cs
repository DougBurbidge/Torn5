
namespace Torn5.Controls
{
	partial class PyramidFixture
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PyramidFixture));
			this.labelRoundTeams = new System.Windows.Forms.Label();
			this.labelRoundGames = new System.Windows.Forms.Label();
			this.labelRoundTeamsPerGame = new System.Windows.Forms.Label();
			this.labelAdvance = new System.Windows.Forms.Label();
			this.labelAdvancePercent = new System.Windows.Forms.Label();
			this.numericRoundTeams = new System.Windows.Forms.NumericUpDown();
			this.numericRoundGames = new System.Windows.Forms.NumericUpDown();
			this.numericRoundTeamsPerGame = new System.Windows.Forms.NumericUpDown();
			this.numericRoundAdvance = new System.Windows.Forms.NumericUpDown();
			this.numericRoundAdvancePercent = new System.Windows.Forms.NumericUpDown();
			this.labelRound = new System.Windows.Forms.Label();
			this.checkBoxRepechage = new System.Windows.Forms.CheckBox();
			this.numericRepAdvancePercent = new System.Windows.Forms.NumericUpDown();
			this.numericRepAdvance = new System.Windows.Forms.NumericUpDown();
			this.numericRepTeamsPerGame = new System.Windows.Forms.NumericUpDown();
			this.numericRepGames = new System.Windows.Forms.NumericUpDown();
			this.numericRepTeams = new System.Windows.Forms.NumericUpDown();
			this.labelRepAdvancePercent = new System.Windows.Forms.Label();
			this.labelRepAdvance = new System.Windows.Forms.Label();
			this.labelRepTeamsPerGame = new System.Windows.Forms.Label();
			this.labelRepGames = new System.Windows.Forms.Label();
			this.labelRepTeams = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.labelRoundDivide = new System.Windows.Forms.Label();
			this.labelRoundEquals = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.numericRoundTeams)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericRoundGames)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericRoundTeamsPerGame)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericRoundAdvance)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericRoundAdvancePercent)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericRepAdvancePercent)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericRepAdvance)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericRepTeamsPerGame)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericRepGames)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericRepTeams)).BeginInit();
			this.SuspendLayout();
			// 
			// labelRoundTeams
			// 
			this.labelRoundTeams.AutoSize = true;
			this.labelRoundTeams.Location = new System.Drawing.Point(57, 3);
			this.labelRoundTeams.Name = "labelRoundTeams";
			this.labelRoundTeams.Size = new System.Drawing.Size(74, 13);
			this.labelRoundTeams.TabIndex = 0;
			this.labelRoundTeams.Text = "Starting teams";
			// 
			// labelRoundGames
			// 
			this.labelRoundGames.AutoSize = true;
			this.labelRoundGames.Location = new System.Drawing.Point(143, 3);
			this.labelRoundGames.Name = "labelRoundGames";
			this.labelRoundGames.Size = new System.Drawing.Size(40, 13);
			this.labelRoundGames.TabIndex = 1;
			this.labelRoundGames.Text = "Games";
			// 
			// labelRoundTeamsPerGame
			// 
			this.labelRoundTeamsPerGame.AutoSize = true;
			this.labelRoundTeamsPerGame.Location = new System.Drawing.Point(200, 3);
			this.labelRoundTeamsPerGame.Name = "labelRoundTeamsPerGame";
			this.labelRoundTeamsPerGame.Size = new System.Drawing.Size(70, 13);
			this.labelRoundTeamsPerGame.TabIndex = 2;
			this.labelRoundTeamsPerGame.Text = "Teams/game";
			// 
			// labelAdvance
			// 
			this.labelAdvance.AutoSize = true;
			this.labelAdvance.Location = new System.Drawing.Point(269, 3);
			this.labelAdvance.Name = "labelAdvance";
			this.labelAdvance.Size = new System.Drawing.Size(81, 13);
			this.labelAdvance.TabIndex = 3;
			this.labelAdvance.Text = "Advance teams";
			// 
			// labelAdvancePercent
			// 
			this.labelAdvancePercent.AutoSize = true;
			this.labelAdvancePercent.Location = new System.Drawing.Point(357, 3);
			this.labelAdvancePercent.Name = "labelAdvancePercent";
			this.labelAdvancePercent.Size = new System.Drawing.Size(61, 13);
			this.labelAdvancePercent.TabIndex = 4;
			this.labelAdvancePercent.Text = "Advance %";
			// 
			// numericRoundTeams
			// 
			this.numericRoundTeams.Enabled = false;
			this.numericRoundTeams.Location = new System.Drawing.Point(60, 19);
			this.numericRoundTeams.Name = "numericRoundTeams";
			this.numericRoundTeams.Size = new System.Drawing.Size(60, 20);
			this.numericRoundTeams.TabIndex = 5;
			this.numericRoundTeams.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericRoundTeams.ValueChanged += new System.EventHandler(this.NumericChanged);
			this.numericRoundTeams.KeyUp += new System.Windows.Forms.KeyEventHandler(this.NumericKeyUp);
			// 
			// numericRoundGames
			// 
			this.numericRoundGames.Location = new System.Drawing.Point(135, 19);
			this.numericRoundGames.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericRoundGames.Name = "numericRoundGames";
			this.numericRoundGames.Size = new System.Drawing.Size(60, 20);
			this.numericRoundGames.TabIndex = 6;
			this.numericRoundGames.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericRoundGames.ValueChanged += new System.EventHandler(this.NumericChanged);
			// 
			// numericRoundTeamsPerGame
			// 
			this.numericRoundTeamsPerGame.DecimalPlaces = 2;
			this.numericRoundTeamsPerGame.Enabled = false;
			this.numericRoundTeamsPerGame.Location = new System.Drawing.Point(210, 19);
			this.numericRoundTeamsPerGame.Name = "numericRoundTeamsPerGame";
			this.numericRoundTeamsPerGame.Size = new System.Drawing.Size(60, 20);
			this.numericRoundTeamsPerGame.TabIndex = 7;
			// 
			// numericRoundAdvance
			// 
			this.numericRoundAdvance.Location = new System.Drawing.Point(285, 19);
			this.numericRoundAdvance.Name = "numericRoundAdvance";
			this.numericRoundAdvance.Size = new System.Drawing.Size(60, 20);
			this.numericRoundAdvance.TabIndex = 8;
			this.numericRoundAdvance.ValueChanged += new System.EventHandler(this.NumericChanged);
			this.numericRoundAdvance.KeyUp += new System.Windows.Forms.KeyEventHandler(this.NumericKeyUp);
			// 
			// numericRoundAdvancePercent
			// 
			this.numericRoundAdvancePercent.DecimalPlaces = 1;
			this.numericRoundAdvancePercent.Enabled = false;
			this.numericRoundAdvancePercent.Location = new System.Drawing.Point(360, 19);
			this.numericRoundAdvancePercent.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
			this.numericRoundAdvancePercent.Name = "numericRoundAdvancePercent";
			this.numericRoundAdvancePercent.Size = new System.Drawing.Size(60, 20);
			this.numericRoundAdvancePercent.TabIndex = 9;
			// 
			// labelRound
			// 
			this.labelRound.AutoSize = true;
			this.labelRound.Location = new System.Drawing.Point(3, 21);
			this.labelRound.Name = "labelRound";
			this.labelRound.Size = new System.Drawing.Size(48, 13);
			this.labelRound.TabIndex = 10;
			this.labelRound.Text = "Round n";
			// 
			// checkBoxRepechage
			// 
			this.checkBoxRepechage.AutoSize = true;
			this.checkBoxRepechage.Checked = true;
			this.checkBoxRepechage.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxRepechage.Location = new System.Drawing.Point(426, 20);
			this.checkBoxRepechage.Name = "checkBoxRepechage";
			this.checkBoxRepechage.Size = new System.Drawing.Size(91, 17);
			this.checkBoxRepechage.TabIndex = 11;
			this.checkBoxRepechage.Text = "Repechage n";
			this.checkBoxRepechage.UseVisualStyleBackColor = true;
			this.checkBoxRepechage.CheckedChanged += new System.EventHandler(this.CheckBoxRepechageCheckedChanged);
			// 
			// numericRepAdvancePercent
			// 
			this.numericRepAdvancePercent.DecimalPlaces = 1;
			this.numericRepAdvancePercent.Enabled = false;
			this.numericRepAdvancePercent.Location = new System.Drawing.Point(826, 19);
			this.numericRepAdvancePercent.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
			this.numericRepAdvancePercent.Name = "numericRepAdvancePercent";
			this.numericRepAdvancePercent.Size = new System.Drawing.Size(60, 20);
			this.numericRepAdvancePercent.TabIndex = 21;
			// 
			// numericRepAdvance
			// 
			this.numericRepAdvance.Location = new System.Drawing.Point(751, 19);
			this.numericRepAdvance.Name = "numericRepAdvance";
			this.numericRepAdvance.Size = new System.Drawing.Size(60, 20);
			this.numericRepAdvance.TabIndex = 20;
			this.numericRepAdvance.ValueChanged += new System.EventHandler(this.NumericChanged);
			this.numericRepAdvance.KeyUp += new System.Windows.Forms.KeyEventHandler(this.NumericKeyUp);
			// 
			// numericRepTeamsPerGame
			// 
			this.numericRepTeamsPerGame.DecimalPlaces = 2;
			this.numericRepTeamsPerGame.Enabled = false;
			this.numericRepTeamsPerGame.Location = new System.Drawing.Point(676, 19);
			this.numericRepTeamsPerGame.Name = "numericRepTeamsPerGame";
			this.numericRepTeamsPerGame.Size = new System.Drawing.Size(60, 20);
			this.numericRepTeamsPerGame.TabIndex = 19;
			// 
			// numericRepGames
			// 
			this.numericRepGames.Location = new System.Drawing.Point(601, 19);
			this.numericRepGames.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericRepGames.Name = "numericRepGames";
			this.numericRepGames.Size = new System.Drawing.Size(60, 20);
			this.numericRepGames.TabIndex = 18;
			this.numericRepGames.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericRepGames.ValueChanged += new System.EventHandler(this.NumericChanged);
			this.numericRepGames.KeyUp += new System.Windows.Forms.KeyEventHandler(this.NumericKeyUp);
			// 
			// numericRepTeams
			// 
			this.numericRepTeams.Enabled = false;
			this.numericRepTeams.Location = new System.Drawing.Point(526, 19);
			this.numericRepTeams.Name = "numericRepTeams";
			this.numericRepTeams.Size = new System.Drawing.Size(60, 20);
			this.numericRepTeams.TabIndex = 17;
			// 
			// labelRepAdvancePercent
			// 
			this.labelRepAdvancePercent.AutoSize = true;
			this.labelRepAdvancePercent.Location = new System.Drawing.Point(823, 3);
			this.labelRepAdvancePercent.Name = "labelRepAdvancePercent";
			this.labelRepAdvancePercent.Size = new System.Drawing.Size(61, 13);
			this.labelRepAdvancePercent.TabIndex = 16;
			this.labelRepAdvancePercent.Text = "Advance %";
			// 
			// labelRepAdvance
			// 
			this.labelRepAdvance.AutoSize = true;
			this.labelRepAdvance.Location = new System.Drawing.Point(735, 3);
			this.labelRepAdvance.Name = "labelRepAdvance";
			this.labelRepAdvance.Size = new System.Drawing.Size(81, 13);
			this.labelRepAdvance.TabIndex = 15;
			this.labelRepAdvance.Text = "Advance teams";
			// 
			// labelRepTeamsPerGame
			// 
			this.labelRepTeamsPerGame.AutoSize = true;
			this.labelRepTeamsPerGame.Location = new System.Drawing.Point(666, 3);
			this.labelRepTeamsPerGame.Name = "labelRepTeamsPerGame";
			this.labelRepTeamsPerGame.Size = new System.Drawing.Size(70, 13);
			this.labelRepTeamsPerGame.TabIndex = 14;
			this.labelRepTeamsPerGame.Text = "Teams/game";
			// 
			// labelRepGames
			// 
			this.labelRepGames.AutoSize = true;
			this.labelRepGames.Location = new System.Drawing.Point(609, 3);
			this.labelRepGames.Name = "labelRepGames";
			this.labelRepGames.Size = new System.Drawing.Size(40, 13);
			this.labelRepGames.TabIndex = 13;
			this.labelRepGames.Text = "Games";
			// 
			// labelRepTeams
			// 
			this.labelRepTeams.AutoSize = true;
			this.labelRepTeams.Location = new System.Drawing.Point(523, 3);
			this.labelRepTeams.Name = "labelRepTeams";
			this.labelRepTeams.Size = new System.Drawing.Size(60, 13);
			this.labelRepTeams.TabIndex = 12;
			this.labelRepTeams.Text = "From round";
			// 
			// panel1
			// 
			this.panel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel1.BackgroundImage")));
			this.panel1.Location = new System.Drawing.Point(75, 45);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(700, 20);
			this.panel1.TabIndex = 22;
			// 
			// labelRoundDivide
			// 
			this.labelRoundDivide.AutoSize = true;
			this.labelRoundDivide.Location = new System.Drawing.Point(122, 21);
			this.labelRoundDivide.Name = "labelRoundDivide";
			this.labelRoundDivide.Size = new System.Drawing.Size(13, 13);
			this.labelRoundDivide.TabIndex = 23;
			this.labelRoundDivide.Text = "÷";
			// 
			// labelRoundEquals
			// 
			this.labelRoundEquals.AutoSize = true;
			this.labelRoundEquals.Location = new System.Drawing.Point(196, 21);
			this.labelRoundEquals.Name = "labelRoundEquals";
			this.labelRoundEquals.Size = new System.Drawing.Size(13, 13);
			this.labelRoundEquals.TabIndex = 24;
			this.labelRoundEquals.Text = "=";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(588, 21);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(13, 13);
			this.label1.TabIndex = 25;
			this.label1.Text = "÷";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(663, 21);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(13, 13);
			this.label2.TabIndex = 26;
			this.label2.Text = "=";
			// 
			// PyramidFixture
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.labelRoundEquals);
			this.Controls.Add(this.labelRoundDivide);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.numericRepAdvancePercent);
			this.Controls.Add(this.numericRepAdvance);
			this.Controls.Add(this.numericRepTeamsPerGame);
			this.Controls.Add(this.numericRepGames);
			this.Controls.Add(this.numericRepTeams);
			this.Controls.Add(this.labelRepAdvancePercent);
			this.Controls.Add(this.labelRepAdvance);
			this.Controls.Add(this.labelRepTeamsPerGame);
			this.Controls.Add(this.labelRepGames);
			this.Controls.Add(this.labelRepTeams);
			this.Controls.Add(this.checkBoxRepechage);
			this.Controls.Add(this.labelRound);
			this.Controls.Add(this.numericRoundAdvancePercent);
			this.Controls.Add(this.numericRoundAdvance);
			this.Controls.Add(this.numericRoundTeamsPerGame);
			this.Controls.Add(this.numericRoundGames);
			this.Controls.Add(this.numericRoundTeams);
			this.Controls.Add(this.labelAdvancePercent);
			this.Controls.Add(this.labelAdvance);
			this.Controls.Add(this.labelRoundTeamsPerGame);
			this.Controls.Add(this.labelRoundGames);
			this.Controls.Add(this.labelRoundTeams);
			this.Name = "PyramidFixture";
			this.Size = new System.Drawing.Size(914, 70);
			((System.ComponentModel.ISupportInitialize)(this.numericRoundTeams)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericRoundGames)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericRoundTeamsPerGame)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericRoundAdvance)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericRoundAdvancePercent)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericRepAdvancePercent)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericRepAdvance)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericRepTeamsPerGame)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericRepGames)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericRepTeams)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label labelRoundTeams;
		private System.Windows.Forms.Label labelRoundGames;
		private System.Windows.Forms.Label labelRoundTeamsPerGame;
		private System.Windows.Forms.Label labelAdvance;
		private System.Windows.Forms.Label labelAdvancePercent;
		private System.Windows.Forms.NumericUpDown numericRoundTeams;
		private System.Windows.Forms.NumericUpDown numericRoundGames;
		private System.Windows.Forms.NumericUpDown numericRoundTeamsPerGame;
		private System.Windows.Forms.NumericUpDown numericRoundAdvance;
		private System.Windows.Forms.NumericUpDown numericRoundAdvancePercent;
		private System.Windows.Forms.Label labelRound;
		private System.Windows.Forms.CheckBox checkBoxRepechage;
		private System.Windows.Forms.NumericUpDown numericRepAdvancePercent;
		private System.Windows.Forms.NumericUpDown numericRepAdvance;
		private System.Windows.Forms.NumericUpDown numericRepTeamsPerGame;
		private System.Windows.Forms.NumericUpDown numericRepGames;
		private System.Windows.Forms.NumericUpDown numericRepTeams;
		private System.Windows.Forms.Label labelRepAdvancePercent;
		private System.Windows.Forms.Label labelRepAdvance;
		private System.Windows.Forms.Label labelRepTeamsPerGame;
		private System.Windows.Forms.Label labelRepGames;
		private System.Windows.Forms.Label labelRepTeams;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label labelRoundDivide;
		private System.Windows.Forms.Label labelRoundEquals;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
	}
}
