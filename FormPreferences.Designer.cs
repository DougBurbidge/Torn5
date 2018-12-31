
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
			this.tabPageUpload = new System.Windows.Forms.TabPage();
			this.textBoxSite = new System.Windows.Forms.TextBox();
			this.labelSite = new System.Windows.Forms.Label();
			this.textBoxPassword = new System.Windows.Forms.TextBox();
			this.textBoxUsername = new System.Windows.Forms.TextBox();
			this.labelPassword = new System.Windows.Forms.Label();
			this.labelUsername = new System.Windows.Forms.Label();
			this.panel3 = new System.Windows.Forms.Panel();
			this.radioHttps = new System.Windows.Forms.RadioButton();
			this.radioHttp = new System.Windows.Forms.RadioButton();
			this.label2 = new System.Windows.Forms.Label();
			this.radioSftp = new System.Windows.Forms.RadioButton();
			this.radioFtp = new System.Windows.Forms.RadioButton();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.labelPort = new System.Windows.Forms.Label();
			this.numericPort = new System.Windows.Forms.NumericUpDown();
			this.checkBoxWebServer = new System.Windows.Forms.CheckBox();
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
			this.tabPageUpload.SuspendLayout();
			this.panel3.SuspendLayout();
			this.tabPage1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericPort)).BeginInit();
			this.panelBottom.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPageSystem);
			this.tabControl1.Controls.Add(this.tabPageConfiguration);
			this.tabControl1.Controls.Add(this.tabPageUpload);
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.Location = new System.Drawing.Point(0, 0);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(364, 292);
			this.tabControl1.TabIndex = 0;
			// 
			// tabPageSystem
			// 
			this.tabPageSystem.Controls.Add(this.panel1);
			this.tabPageSystem.Controls.Add(this.panelSystemType);
			this.tabPageSystem.Location = new System.Drawing.Point(4, 22);
			this.tabPageSystem.Name = "tabPageSystem";
			this.tabPageSystem.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageSystem.Size = new System.Drawing.Size(356, 266);
			this.tabPageSystem.TabIndex = 0;
			this.tabPageSystem.Text = "System";
			this.tabPageSystem.UseVisualStyleBackColor = true;
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.Controls.Add(this.textBoxServerAddress);
			this.panel1.Controls.Add(this.label3);
			this.panel1.Location = new System.Drawing.Point(8, 127);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(340, 60);
			this.panel1.TabIndex = 1;
			// 
			// textBoxServerAddress
			// 
			this.textBoxServerAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxServerAddress.Location = new System.Drawing.Point(150, 8);
			this.textBoxServerAddress.Name = "textBoxServerAddress";
			this.textBoxServerAddress.Size = new System.Drawing.Size(187, 20);
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
			this.panelSystemType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
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
			this.tabPageConfiguration.Size = new System.Drawing.Size(356, 266);
			this.tabPageConfiguration.TabIndex = 1;
			this.tabPageConfiguration.Text = "Configuration";
			this.tabPageConfiguration.UseVisualStyleBackColor = true;
			// 
			// panel4
			// 
			this.panel4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
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
			this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
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
			// tabPageUpload
			// 
			this.tabPageUpload.Controls.Add(this.textBoxSite);
			this.tabPageUpload.Controls.Add(this.labelSite);
			this.tabPageUpload.Controls.Add(this.textBoxPassword);
			this.tabPageUpload.Controls.Add(this.textBoxUsername);
			this.tabPageUpload.Controls.Add(this.labelPassword);
			this.tabPageUpload.Controls.Add(this.labelUsername);
			this.tabPageUpload.Controls.Add(this.panel3);
			this.tabPageUpload.Location = new System.Drawing.Point(4, 22);
			this.tabPageUpload.Name = "tabPageUpload";
			this.tabPageUpload.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageUpload.Size = new System.Drawing.Size(356, 266);
			this.tabPageUpload.TabIndex = 2;
			this.tabPageUpload.Text = "Upload";
			this.tabPageUpload.UseVisualStyleBackColor = true;
			// 
			// textBoxSite
			// 
			this.textBoxSite.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxSite.Location = new System.Drawing.Point(80, 124);
			this.textBoxSite.Name = "textBoxSite";
			this.textBoxSite.Size = new System.Drawing.Size(268, 20);
			this.textBoxSite.TabIndex = 8;
			// 
			// labelSite
			// 
			this.labelSite.Location = new System.Drawing.Point(6, 127);
			this.labelSite.Name = "labelSite";
			this.labelSite.Size = new System.Drawing.Size(68, 23);
			this.labelSite.TabIndex = 7;
			this.labelSite.Text = "Site:";
			// 
			// textBoxPassword
			// 
			this.textBoxPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxPassword.Location = new System.Drawing.Point(80, 176);
			this.textBoxPassword.Name = "textBoxPassword";
			this.textBoxPassword.Size = new System.Drawing.Size(268, 20);
			this.textBoxPassword.TabIndex = 6;
			// 
			// textBoxUsername
			// 
			this.textBoxUsername.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxUsername.Location = new System.Drawing.Point(80, 150);
			this.textBoxUsername.Name = "textBoxUsername";
			this.textBoxUsername.Size = new System.Drawing.Size(268, 20);
			this.textBoxUsername.TabIndex = 5;
			// 
			// labelPassword
			// 
			this.labelPassword.Location = new System.Drawing.Point(6, 179);
			this.labelPassword.Name = "labelPassword";
			this.labelPassword.Size = new System.Drawing.Size(68, 23);
			this.labelPassword.TabIndex = 4;
			this.labelPassword.Text = "Password:";
			// 
			// labelUsername
			// 
			this.labelUsername.Location = new System.Drawing.Point(6, 153);
			this.labelUsername.Name = "labelUsername";
			this.labelUsername.Size = new System.Drawing.Size(68, 23);
			this.labelUsername.TabIndex = 3;
			this.labelUsername.Text = "Username:";
			// 
			// panel3
			// 
			this.panel3.Controls.Add(this.radioHttps);
			this.panel3.Controls.Add(this.radioHttp);
			this.panel3.Controls.Add(this.label2);
			this.panel3.Controls.Add(this.radioSftp);
			this.panel3.Controls.Add(this.radioFtp);
			this.panel3.Location = new System.Drawing.Point(6, 6);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(93, 112);
			this.panel3.TabIndex = 2;
			// 
			// radioHttps
			// 
			this.radioHttps.AutoSize = true;
			this.radioHttps.Location = new System.Drawing.Point(3, 85);
			this.radioHttps.Name = "radioHttps";
			this.radioHttps.Size = new System.Drawing.Size(48, 17);
			this.radioHttps.TabIndex = 4;
			this.radioHttps.TabStop = true;
			this.radioHttps.Tag = "https";
			this.radioHttps.Text = "https";
			this.radioHttps.UseVisualStyleBackColor = true;
			// 
			// radioHttp
			// 
			this.radioHttp.AutoSize = true;
			this.radioHttp.Location = new System.Drawing.Point(3, 62);
			this.radioHttp.Name = "radioHttp";
			this.radioHttp.Size = new System.Drawing.Size(43, 17);
			this.radioHttp.TabIndex = 3;
			this.radioHttp.TabStop = true;
			this.radioHttp.Tag = "http";
			this.radioHttp.Text = "http";
			this.radioHttp.UseVisualStyleBackColor = true;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(3, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(79, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Upload method";
			// 
			// radioSftp
			// 
			this.radioSftp.AutoSize = true;
			this.radioSftp.Enabled = false;
			this.radioSftp.Location = new System.Drawing.Point(3, 39);
			this.radioSftp.Name = "radioSftp";
			this.radioSftp.Size = new System.Drawing.Size(42, 17);
			this.radioSftp.TabIndex = 1;
			this.radioSftp.TabStop = true;
			this.radioSftp.Tag = "sftp";
			this.radioSftp.Text = "sftp";
			this.radioSftp.UseVisualStyleBackColor = true;
			// 
			// radioFtp
			// 
			this.radioFtp.AutoSize = true;
			this.radioFtp.Location = new System.Drawing.Point(3, 16);
			this.radioFtp.Name = "radioFtp";
			this.radioFtp.Size = new System.Drawing.Size(37, 17);
			this.radioFtp.TabIndex = 0;
			this.radioFtp.TabStop = true;
			this.radioFtp.Tag = "ftp";
			this.radioFtp.Text = "ftp";
			this.radioFtp.UseVisualStyleBackColor = true;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.labelPort);
			this.tabPage1.Controls.Add(this.numericPort);
			this.tabPage1.Controls.Add(this.checkBoxWebServer);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(356, 266);
			this.tabPage1.TabIndex = 3;
			this.tabPage1.Text = "Web Server";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// labelPort
			// 
			this.labelPort.Enabled = false;
			this.labelPort.Location = new System.Drawing.Point(8, 38);
			this.labelPort.Name = "labelPort";
			this.labelPort.Size = new System.Drawing.Size(100, 23);
			this.labelPort.TabIndex = 2;
			this.labelPort.Text = "Web server port:";
			// 
			// numericPort
			// 
			this.numericPort.Enabled = false;
			this.numericPort.Location = new System.Drawing.Point(114, 36);
			this.numericPort.Maximum = new decimal(new int[] {
									65536,
									0,
									0,
									0});
			this.numericPort.Name = "numericPort";
			this.numericPort.Size = new System.Drawing.Size(69, 20);
			this.numericPort.TabIndex = 1;
			this.numericPort.Value = new decimal(new int[] {
									8080,
									0,
									0,
									0});
			// 
			// checkBoxWebServer
			// 
			this.checkBoxWebServer.Location = new System.Drawing.Point(8, 6);
			this.checkBoxWebServer.Name = "checkBoxWebServer";
			this.checkBoxWebServer.Size = new System.Drawing.Size(185, 24);
			this.checkBoxWebServer.TabIndex = 0;
			this.checkBoxWebServer.Text = "Enable internal web server";
			this.checkBoxWebServer.UseVisualStyleBackColor = true;
			this.checkBoxWebServer.CheckedChanged += new System.EventHandler(this.CheckBoxWebServerCheckedChanged);
			// 
			// panelBottom
			// 
			this.panelBottom.Controls.Add(this.buttonCancel);
			this.panelBottom.Controls.Add(this.buttonOK);
			this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panelBottom.Location = new System.Drawing.Point(0, 254);
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
			this.ClientSize = new System.Drawing.Size(364, 292);
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
			this.tabPageUpload.ResumeLayout(false);
			this.tabPageUpload.PerformLayout();
			this.panel3.ResumeLayout(false);
			this.panel3.PerformLayout();
			this.tabPage1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.numericPort)).EndInit();
			this.panelBottom.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.CheckBox checkBoxWebServer;
		private System.Windows.Forms.NumericUpDown numericPort;
		private System.Windows.Forms.Label labelPort;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.Label labelSite;
		private System.Windows.Forms.TextBox textBoxSite;
		private System.Windows.Forms.RadioButton radioFtp;
		private System.Windows.Forms.RadioButton radioSftp;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.RadioButton radioHttp;
		private System.Windows.Forms.RadioButton radioHttps;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.Label labelUsername;
		private System.Windows.Forms.Label labelPassword;
		private System.Windows.Forms.TextBox textBoxUsername;
		private System.Windows.Forms.TextBox textBoxPassword;
		private System.Windows.Forms.TabPage tabPageUpload;
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
