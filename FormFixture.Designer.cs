
namespace Torn.UI
{
	partial class FormFixture
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.textBoxTeams = new System.Windows.Forms.TextBox();
			this.buttonImportTeams = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.buttonImportGames = new System.Windows.Forms.Button();
			this.textBoxSeparator = new System.Windows.Forms.TextBox();
			this.radioButtonOther = new System.Windows.Forms.RadioButton();
			this.radioButtonTab = new System.Windows.Forms.RadioButton();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.textBoxGames = new System.Windows.Forms.TextBox();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.textBoxTeams);
			this.splitContainer1.Panel1.Controls.Add(this.buttonImportTeams);
			this.splitContainer1.Panel1.Controls.Add(this.label1);
			this.splitContainer1.Panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.Panel1Paint);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.buttonImportGames);
			this.splitContainer1.Panel2.Controls.Add(this.textBoxSeparator);
			this.splitContainer1.Panel2.Controls.Add(this.radioButtonOther);
			this.splitContainer1.Panel2.Controls.Add(this.radioButtonTab);
			this.splitContainer1.Panel2.Controls.Add(this.label3);
			this.splitContainer1.Panel2.Controls.Add(this.label2);
			this.splitContainer1.Panel2.Controls.Add(this.textBoxGames);
			this.splitContainer1.Size = new System.Drawing.Size(632, 682);
			this.splitContainer1.SplitterDistance = 226;
			this.splitContainer1.SplitterWidth = 8;
			this.splitContainer1.TabIndex = 18;
			// 
			// textBoxTeams
			// 
			this.textBoxTeams.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxTeams.Location = new System.Drawing.Point(12, 41);
			this.textBoxTeams.Multiline = true;
			this.textBoxTeams.Name = "textBoxTeams";
			this.textBoxTeams.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBoxTeams.Size = new System.Drawing.Size(200, 570);
			this.textBoxTeams.TabIndex = 11;
			this.textBoxTeams.Text = "Team 1\r\nTeam 2\r\nTeam 3\r\netc.";
			this.textBoxTeams.WordWrap = false;
			// 
			// buttonImportTeams
			// 
			this.buttonImportTeams.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonImportTeams.Location = new System.Drawing.Point(137, 12);
			this.buttonImportTeams.Name = "buttonImportTeams";
			this.buttonImportTeams.Size = new System.Drawing.Size(75, 23);
			this.buttonImportTeams.TabIndex = 10;
			this.buttonImportTeams.Text = "Import";
			this.buttonImportTeams.UseVisualStyleBackColor = true;
			this.buttonImportTeams.Click += new System.EventHandler(this.ButtonImportTeamsClick);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 17);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(59, 16);
			this.label1.TabIndex = 9;
			this.label1.Text = "Teams";
			// 
			// buttonImportGames
			// 
			this.buttonImportGames.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonImportGames.Location = new System.Drawing.Point(307, 12);
			this.buttonImportGames.Name = "buttonImportGames";
			this.buttonImportGames.Size = new System.Drawing.Size(75, 23);
			this.buttonImportGames.TabIndex = 19;
			this.buttonImportGames.Text = "Import";
			this.buttonImportGames.UseVisualStyleBackColor = true;
			this.buttonImportGames.Click += new System.EventHandler(this.ButtonImportGamesClick);
			// 
			// textBoxSeparator
			// 
			this.textBoxSeparator.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textBoxSeparator.Location = new System.Drawing.Point(86, 652);
			this.textBoxSeparator.Name = "textBoxSeparator";
			this.textBoxSeparator.Size = new System.Drawing.Size(49, 20);
			this.textBoxSeparator.TabIndex = 24;
			this.textBoxSeparator.Text = " ";
			// 
			// radioButtonOther
			// 
			this.radioButtonOther.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.radioButtonOther.Location = new System.Drawing.Point(31, 649);
			this.radioButtonOther.Name = "radioButtonOther";
			this.radioButtonOther.Size = new System.Drawing.Size(104, 24);
			this.radioButtonOther.TabIndex = 23;
			this.radioButtonOther.Text = "Other";
			this.radioButtonOther.UseVisualStyleBackColor = true;
			// 
			// radioButtonTab
			// 
			this.radioButtonTab.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.radioButtonTab.Checked = true;
			this.radioButtonTab.Location = new System.Drawing.Point(31, 628);
			this.radioButtonTab.Name = "radioButtonTab";
			this.radioButtonTab.Size = new System.Drawing.Size(104, 24);
			this.radioButtonTab.TabIndex = 22;
			this.radioButtonTab.TabStop = true;
			this.radioButtonTab.Text = "Tab";
			this.radioButtonTab.UseVisualStyleBackColor = true;
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label3.Location = new System.Drawing.Point(27, 614);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(100, 23);
			this.label3.TabIndex = 21;
			this.label3.Text = "Separator:";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(13, 11);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(251, 27);
			this.label2.TabIndex = 20;
			this.label2.Text = "Games\r\n(Date/time,teamnumber,teamnumber, ...)";
			// 
			// textBoxGames
			// 
			this.textBoxGames.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxGames.Location = new System.Drawing.Point(13, 41);
			this.textBoxGames.Multiline = true;
			this.textBoxGames.Name = "textBoxGames";
			this.textBoxGames.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBoxGames.Size = new System.Drawing.Size(369, 570);
			this.textBoxGames.TabIndex = 18;
			this.textBoxGames.Text = "1/1/2000 7:00pm\t1\t2\t3\r\n1/1/2000 7:15pm\t2\t3\t4\r\n1/1/2000 7:30pm\t3\t4\t1\r\n1/1/2000 7:4" +
			"5pm\t2\t3\t4";
			this.textBoxGames.WordWrap = false;
			// 
			// FormFixture
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(632, 682);
			this.Controls.Add(this.splitContainer1);
			this.Name = "FormFixture";
			this.Text = "Fixtures";
			this.Shown += new System.EventHandler(this.FormFixtureShown);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel1.PerformLayout();
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.TextBox textBoxSeparator;
		private System.Windows.Forms.RadioButton radioButtonOther;
		private System.Windows.Forms.RadioButton radioButtonTab;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button buttonImportGames;
		private System.Windows.Forms.Button buttonImportTeams;
		private System.Windows.Forms.TextBox textBoxGames;
		private System.Windows.Forms.TextBox textBoxTeams;
	}
}
