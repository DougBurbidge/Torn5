namespace Torn.UI
{
	partial class FormAdhoc
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.displayReport = new Torn5.Controls.DisplayReport();
			this.panelRight = new System.Windows.Forms.Panel();
			this.printReport = new Torn5.Controls.PrintReport();
			this.panelRerender = new System.Windows.Forms.Panel();
			this.buttonRerender = new System.Windows.Forms.Button();
			this.panelRight.SuspendLayout();
			this.panelRerender.SuspendLayout();
			this.SuspendLayout();
			// 
			// displayReport
			// 
			this.displayReport.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.displayReport.Dock = System.Windows.Forms.DockStyle.Fill;
			this.displayReport.Location = new System.Drawing.Point(0, 0);
			this.displayReport.Name = "displayReport";
			this.displayReport.Report = null;
			this.displayReport.Size = new System.Drawing.Size(720, 689);
			this.displayReport.TabIndex = 0;
			// 
			// panelRight
			// 
			this.panelRight.Controls.Add(this.printReport);
			this.panelRight.Controls.Add(this.panelRerender);
			this.panelRight.Dock = System.Windows.Forms.DockStyle.Right;
			this.panelRight.Location = new System.Drawing.Point(720, 0);
			this.panelRight.Name = "panelRight";
			this.panelRight.Size = new System.Drawing.Size(64, 689);
			this.panelRight.TabIndex = 11;
			// 
			// printReport
			// 
			this.printReport.DisplayReport = this.displayReport;
			this.printReport.Dock = System.Windows.Forms.DockStyle.Top;
			this.printReport.Location = new System.Drawing.Point(0, 64);
			this.printReport.Name = "printReport";
			this.printReport.Size = new System.Drawing.Size(64, 480);
			this.printReport.TabIndex = 2;
			// 
			// panelRerender
			// 
			this.panelRerender.Controls.Add(this.buttonRerender);
			this.panelRerender.Dock = System.Windows.Forms.DockStyle.Top;
			this.panelRerender.Location = new System.Drawing.Point(0, 0);
			this.panelRerender.Name = "panelRerender";
			this.panelRerender.Size = new System.Drawing.Size(64, 64);
			this.panelRerender.TabIndex = 0;
			// 
			// buttonRerender
			// 
			this.buttonRerender.Location = new System.Drawing.Point(6, 12);
			this.buttonRerender.Name = "buttonRerender";
			this.buttonRerender.Size = new System.Drawing.Size(52, 37);
			this.buttonRerender.TabIndex = 0;
			this.buttonRerender.Text = "Re- render";
			this.buttonRerender.UseVisualStyleBackColor = true;
			this.buttonRerender.Click += new System.EventHandler(this.ButtonRerenderClick);
			// 
			// FormAdhoc
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.ClientSize = new System.Drawing.Size(784, 689);
			this.Controls.Add(this.displayReport);
			this.Controls.Add(this.panelRight);
			this.Name = "FormAdhoc";
			this.Text = "FormAdhoc";
			this.panelRight.ResumeLayout(false);
			this.panelRerender.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.Panel panelRight;
		private System.Windows.Forms.Panel panelRerender;
		private System.Windows.Forms.Button buttonRerender;
		private Torn5.Controls.PrintReport printReport;
		private Torn5.Controls.DisplayReport displayReport;
	}
}