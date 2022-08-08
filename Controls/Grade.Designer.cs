
namespace Torn5.Controls
{
	partial class GradeEditor
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
			this.checkBonus = new System.Windows.Forms.CheckBox();
			this.checkPenalty = new System.Windows.Forms.CheckBox();
			this.numericPoints = new System.Windows.Forms.NumericUpDown();
			this.textName = new System.Windows.Forms.TextBox();
			((System.ComponentModel.ISupportInitialize)(this.numericPoints)).BeginInit();
			this.SuspendLayout();
			// 
			// checkBonus
			// 
			this.checkBonus.AutoSize = true;
			this.checkBonus.Location = new System.Drawing.Point(173, 3);
			this.checkBonus.Name = "checkBonus";
			this.checkBonus.Size = new System.Drawing.Size(15, 14);
			this.checkBonus.TabIndex = 41;
			this.checkBonus.Tag = "";
			this.checkBonus.UseVisualStyleBackColor = true;
			this.checkBonus.CheckedChanged += new System.EventHandler(this.CheckBonusCheckedChanged);
			// 
			// checkPenalty
			// 
			this.checkPenalty.AutoSize = true;
			this.checkPenalty.Location = new System.Drawing.Point(112, 3);
			this.checkPenalty.Name = "checkPenalty";
			this.checkPenalty.Size = new System.Drawing.Size(15, 14);
			this.checkPenalty.TabIndex = 40;
			this.checkPenalty.Tag = "";
			this.checkPenalty.UseVisualStyleBackColor = true;
			this.checkPenalty.CheckedChanged += new System.EventHandler(this.CheckPenaltyCheckedChanged);
			// 
			// numericPoints
			// 
			this.numericPoints.Location = new System.Drawing.Point(60, 0);
			this.numericPoints.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
			this.numericPoints.Minimum = new decimal(new int[] {
            9,
            0,
            0,
            -2147483648});
			this.numericPoints.Name = "numericPoints";
			this.numericPoints.Size = new System.Drawing.Size(33, 20);
			this.numericPoints.TabIndex = 39;
			this.numericPoints.Tag = "";
			this.numericPoints.ValueChanged += new System.EventHandler(this.OnChanged);
			// 
			// textName
			// 
			this.textName.Location = new System.Drawing.Point(0, 0);
			this.textName.Name = "textName";
			this.textName.Size = new System.Drawing.Size(48, 20);
			this.textName.TabIndex = 38;
			this.textName.Tag = "";
			this.textName.TextChanged += new System.EventHandler(this.OnChanged);
			// 
			// GradeEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.checkBonus);
			this.Controls.Add(this.checkPenalty);
			this.Controls.Add(this.numericPoints);
			this.Controls.Add(this.textName);
			this.Name = "GradeEditor";
			this.Size = new System.Drawing.Size(200, 20);
			((System.ComponentModel.ISupportInitialize)(this.numericPoints)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox checkBonus;
		private System.Windows.Forms.CheckBox checkPenalty;
		private System.Windows.Forms.NumericUpDown numericPoints;
		private System.Windows.Forms.TextBox textName;
	}
}
