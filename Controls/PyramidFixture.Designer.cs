
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
			this.labelRound = new System.Windows.Forms.Label();
			this.checkBoxRepechage = new System.Windows.Forms.CheckBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.fixtureRepechage = new Torn5.Controls.PyramidHalfFixture();
			this.fixtureRound = new Torn5.Controls.PyramidHalfFixture();
			this.SuspendLayout();
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
			// panel1
			// 
			this.panel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel1.BackgroundImage")));
			this.panel1.Location = new System.Drawing.Point(75, 45);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(700, 20);
			this.panel1.TabIndex = 22;
			// 
			// fixtureRepechage
			// 
			this.fixtureRepechage.Advance = 0;
			this.fixtureRepechage.Games = 1;
			this.fixtureRepechage.GamesPerTeam = 1;
			this.fixtureRepechage.IsRound = false;
			this.fixtureRepechage.Location = new System.Drawing.Point(523, 3);
			this.fixtureRepechage.Name = "fixtureRepechage";
			this.fixtureRepechage.RoundNumber = 0;
			this.fixtureRepechage.Size = new System.Drawing.Size(364, 37);
			this.fixtureRepechage.TabIndex = 28;
			this.fixtureRepechage.TeamsIn = 1;
			this.fixtureRepechage.ValueChanged += new System.EventHandler(this.HalfFixtureChanged);
			// 
			// fixtureRound
			// 
			this.fixtureRound.Advance = 0;
			this.fixtureRound.Games = 1;
			this.fixtureRound.GamesPerTeam = 1;
			this.fixtureRound.IsRound = true;
			this.fixtureRound.Location = new System.Drawing.Point(57, 2);
			this.fixtureRound.Name = "fixtureRound";
			this.fixtureRound.RoundNumber = 0;
			this.fixtureRound.Size = new System.Drawing.Size(364, 37);
			this.fixtureRound.TabIndex = 27;
			this.fixtureRound.TeamsIn = 1;
			this.fixtureRound.ValueChanged += new System.EventHandler(this.HalfFixtureChanged);
			// 
			// PyramidFixture
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.fixtureRepechage);
			this.Controls.Add(this.fixtureRound);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.checkBoxRepechage);
			this.Controls.Add(this.labelRound);
			this.Name = "PyramidFixture";
			this.Size = new System.Drawing.Size(914, 70);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.Label labelRound;
		private System.Windows.Forms.CheckBox checkBoxRepechage;
		private System.Windows.Forms.Panel panel1;
		private PyramidHalfFixture fixtureRound;
		private PyramidHalfFixture fixtureRepechage;
	}
}
