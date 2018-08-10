
namespace Torn.UI
{
	partial class FormPreferences
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
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPageSystem = new System.Windows.Forms.TabPage();
			this.panel1 = new System.Windows.Forms.Panel();
			this.textBoxServerAddress = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.panelSystemType = new System.Windows.Forms.Panel();
			this.radioDemo = new System.Windows.Forms.RadioButton();
			this.radioLaserforce = new System.Windows.Forms.RadioButton();
			this.label1 = new System.Windows.Forms.Label();
			this.radioZeon = new System.Windows.Forms.RadioButton();
			this.radioAcacia = new System.Windows.Forms.RadioButton();
			this.tabPageConfiguration = new System.Windows.Forms.TabPage();
			this.panel4 = new System.Windows.Forms.Panel();
			this.checkBoxAutoUpdateTeams = new System.Windows.Forms.CheckBox();
			this.checkBoxAutoUpdateScoreboard = new System.Windows.Forms.CheckBox();
			this.label6 = new System.Windows.Forms.Label();
			this.panel2 = new System.Windows.Forms.Panel();
			this.radioLotr = new System.Windows.Forms.RadioButton();
			this.label4 = new System.Windows.Forms.Label();
			this.radioColour = new System.Windows.Forms.RadioButton();
			this.radioAlias = new System.Windows.Forms.RadioButton();
			this.panelBottom = new System.Windows.Forms.Panel();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			this.tabControl1.SuspendLayout();
			this.tabPageSystem.SuspendLayout();
			this.panel1.SuspendLayout();
			this.panelSystemType.SuspendLayout();
			this.tabPageConfiguration.SuspendLayout();
			this.panel4.SuspendLayout();
			this.panel2.SuspendLayout();
			this.panelBottom.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPageSystem);
			this.tabControl1.Controls.Add(this.tabPageConfiguration);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.Location = new System.Drawing.Point(0, 0);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(364, 250);
			this.tabControl1.TabIndex = 0;
			// 
			// tabPageSystem
			// 
			this.tabPageSystem.Controls.Add(this.panel1);
			this.tabPageSystem.Controls.Add(this.panelSystemType);
			this.tabPageSystem.Location = new System.Drawing.Point(4, 22);
			this.tabPageSystem.Name = "tabPageSystem";
			this.tabPageSystem.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageSystem.Size = new System.Drawing.Size(356, 224);
			this.tabPageSystem.TabIndex = 0;
			this.tabPageSystem.Text = "System";
			this.tabPageSystem.UseVisualStyleBackColor = true;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.textBoxServerAddress);
			this.panel1.Controls.Add(this.label3);
			this.panel1.Location = new System.Drawing.Point(8, 127);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(340, 60);
			this.panel1.TabIndex = 1;
			// 
			// textBoxServerAddress
			// 
			this.textBoxServerAddress.Location = new System.Drawing.Point(150, 8);
			this.textBoxServerAddress.Name = "textBoxServerAddress";
			this.textBoxServerAddress.Size = new System.Drawing.Size(100, 20);
			this.textBoxServerAddress.TabIndex = 3;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(3, 11);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(141, 13);
			this.label3.TabIndex = 2;
			this.label3.Text = "Laser Game Server address:";
			// 
			// panelSystemType
			// 
			this.panelSystemType.Controls.Add(this.radioDemo);
			this.panelSystemType.Controls.Add(this.radioLaserforce);
			this.panelSystemType.Controls.Add(this.label1);
			this.panelSystemType.Controls.Add(this.radioZeon);
			this.panelSystemType.Controls.Add(this.radioAcacia);
			this.panelSystemType.Location = new System.Drawing.Point(8, 6);
			this.panelSystemType.Name = "panelSystemType";
			this.panelSystemType.Size = new System.Drawing.Size(340, 115);
			this.panelSystemType.TabIndex = 0;
			// 
			// radioDemo
			// 
			this.radioDemo.AutoSize = true;
			this.radioDemo.Location = new System.Drawing.Point(3, 85);
			this.radioDemo.Name = "radioDemo";
			this.radioDemo.Size = new System.Drawing.Size(53, 17);
			this.radioDemo.TabIndex = 4;
			this.radioDemo.TabStop = true;
			this.radioDemo.Text = "Demo";
			this.radioDemo.UseVisualStyleBackColor = true;
			// 
			// radioLaserforce
			// 
			this.radioLaserforce.AutoSize = true;
			this.radioLaserforce.Location = new System.Drawing.Point(3, 62);
			this.radioLaserforce.Name = "radioLaserforce";
			this.radioLaserforce.Size = new System.Drawing.Size(75, 17);
			this.radioLaserforce.TabIndex = 3;
			this.radioLaserforce.TabStop = true;
			this.radioLaserforce.Text = "Laserforce";
			this.radioLaserforce.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(64, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "System type";
			// 
			// radioZeon
			// 
			this.radioZeon.AutoSize = true;
			this.radioZeon.Location = new System.Drawing.Point(3, 39);
			this.radioZeon.Name = "radioZeon";
			this.radioZeon.Size = new System.Drawing.Size(110, 17);
			this.radioZeon.TabIndex = 1;
			this.radioZeon.TabStop = true;
			this.radioZeon.Text = "Nexus with ZEON";
			this.radioZeon.UseVisualStyleBackColor = true;
			// 
			// radioAcacia
			// 
			this.radioAcacia.AutoSize = true;
			this.radioAcacia.Location = new System.Drawing.Point(3, 16);
			this.radioAcacia.Name = "radioAcacia";
			this.radioAcacia.Size = new System.Drawing.Size(106, 17);
			this.radioAcacia.TabIndex = 0;
			this.radioAcacia.TabStop = true;
			this.radioAcacia.Text = "Acacia / Infusion";
			this.radioAcacia.UseVisualStyleBackColor = true;
			// 
			// tabPageConfiguration
			// 
			this.tabPageConfiguration.Controls.Add(this.panel4);
			this.tabPageConfiguration.Controls.Add(this.panel2);
			this.tabPageConfiguration.Location = new System.Drawing.Point(4, 22);
			this.tabPageConfiguration.Name = "tabPageConfiguration";
			this.tabPageConfiguration.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageConfiguration.Size = new System.Drawing.Size(356, 224);
			this.tabPageConfiguration.TabIndex = 1;
			this.tabPageConfiguration.Text = "Configuration";
			this.tabPageConfiguration.UseVisualStyleBackColor = true;
			// 
			// panel4
			// 
			this.panel4.Controls.Add(this.checkBoxAutoUpdateTeams);
			this.panel4.Controls.Add(this.checkBoxAutoUpdateScoreboard);
			this.panel4.Controls.Add(this.label6);
			this.panel4.Location = new System.Drawing.Point(8, 102);
			this.panel4.Name = "panel4";
			this.panel4.Size = new System.Drawing.Size(338, 65);
			this.panel4.TabIndex = 3;
			// 
			// checkBoxAutoUpdateTeams
			// 
			this.checkBoxAutoUpdateTeams.AutoSize = true;
			this.checkBoxAutoUpdateTeams.Location = new System.Drawing.Point(3, 39);
			this.checkBoxAutoUpdateTeams.Name = "checkBoxAutoUpdateTeams";
			this.checkBoxAutoUpdateTeams.Size = new System.Drawing.Size(330, 17);
			this.checkBoxAutoUpdateTeams.TabIndex = 4;
			this.checkBoxAutoUpdateTeams.Text = "On Commit, automatically update team player lists and handicaps";
			this.checkBoxAutoUpdateTeams.UseVisualStyleBackColor = true;
			// 
			// checkBoxAutoUpdateScoreboard
			// 
			this.checkBoxAutoUpdateScoreboard.AutoSize = true;
			this.checkBoxAutoUpdateScoreboard.Location = new System.Drawing.Point(3, 16);
			this.checkBoxAutoUpdateScoreboard.Name = "checkBoxAutoUpdateScoreboard";
			this.checkBoxAutoUpdateScoreboard.Size = new System.Drawing.Size(236, 17);
			this.checkBoxAutoUpdateScoreboard.TabIndex = 3;
			this.checkBoxAutoUpdateScoreboard.Text = "On Commit, automatically update scoreboard";
			this.checkBoxAutoUpdateScoreboard.UseVisualStyleBackColor = true;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(3, 0);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(58, 13);
			this.label6.TabIndex = 2;
			this.label6.Text = "On Commit";
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.radioLotr);
			this.panel2.Controls.Add(this.label4);
			this.panel2.Controls.Add(this.radioColour);
			this.panel2.Controls.Add(this.radioAlias);
			this.panel2.Location = new System.Drawing.Point(8, 6);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(338, 90);
			this.panel2.TabIndex = 1;
			// 
			// radioLotr
			// 
			this.radioLotr.AutoSize = true;
			this.radioLotr.Location = new System.Drawing.Point(3, 62);
			this.radioLotr.Name = "radioLotr";
			this.radioLotr.Size = new System.Drawing.Size(101, 17);
			this.radioLotr.TabIndex = 3;
			this.radioLotr.TabStop = true;
			this.radioLotr.Text = "Lord of the Ring";
			this.radioLotr.UseVisualStyleBackColor = true;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(3, 0);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(72, 13);
			this.label4.TabIndex = 2;
			this.label4.Text = "Group players";
			// 
			// radioColour
			// 
			this.radioColour.AutoSize = true;
			this.radioColour.Location = new System.Drawing.Point(3, 39);
			this.radioColour.Name = "radioColour";
			this.radioColour.Size = new System.Drawing.Size(68, 17);
			this.radioColour.TabIndex = 1;
			this.radioColour.TabStop = true;
			this.radioColour.Text = "by colour";
			this.radioColour.UseVisualStyleBackColor = true;
			// 
			// radioAlias
			// 
			this.radioAlias.AutoSize = true;
			this.radioAlias.Location = new System.Drawing.Point(3, 16);
			this.radioAlias.Name = "radioAlias";
			this.radioAlias.Size = new System.Drawing.Size(60, 17);
			this.radioAlias.TabIndex = 0;
			this.radioAlias.TabStop = true;
			this.radioAlias.Text = "by alias";
			this.radioAlias.UseVisualStyleBackColor = true;
			// 
			// panelBottom
			// 
			this.panelBottom.Controls.Add(this.buttonCancel);
			this.panelBottom.Controls.Add(this.buttonOK);
			this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panelBottom.Location = new System.Drawing.Point(0, 212);
			this.panelBottom.Name = "panelBottom";
			this.panelBottom.Size = new System.Drawing.Size(364, 38);
			this.panelBottom.TabIndex = 1;
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(277, 3);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 1;
			this.buttonCancel.Text = "&Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point(196, 3);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 0;
			this.buttonOK.Text = "&OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			// 
			// FormPreferences
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(364, 250);
			this.Controls.Add(this.panelBottom);
			this.Controls.Add(this.tabControl1);
			this.Name = "FormPreferences";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Preferences";
			this.tabControl1.ResumeLayout(false);
			this.tabPageSystem.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.panelSystemType.ResumeLayout(false);
			this.panelSystemType.PerformLayout();
			this.tabPageConfiguration.ResumeLayout(false);
			this.panel4.ResumeLayout(false);
			this.panel4.PerformLayout();
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this.panelBottom.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.CheckBox checkBoxAutoUpdateTeams;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Panel panelBottom;
		private System.Windows.Forms.RadioButton radioAlias;
		private System.Windows.Forms.RadioButton radioColour;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.RadioButton radioLotr;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.CheckBox checkBoxAutoUpdateScoreboard;
		private System.Windows.Forms.Panel panel4;
		private System.Windows.Forms.TabPage tabPageConfiguration;
		private System.Windows.Forms.RadioButton radioAcacia;
		private System.Windows.Forms.RadioButton radioZeon;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.RadioButton radioLaserforce;
		private System.Windows.Forms.RadioButton radioDemo;
		private System.Windows.Forms.Panel panelSystemType;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textBoxServerAddress;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.TabPage tabPageSystem;
		private System.Windows.Forms.TabControl tabControl1;
	}
}
