﻿using System;
using System.Drawing;
using System.Windows.Forms;

namespace Torn.UI
{
	public enum SystemType { Nexus, Zeon, OZone, Laserforce, Torn, Demo };

	/// <summary>Prefs Form.</summary>
	public partial class FormPreferences : Form
	{
		public FormPreferences()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}

		public SystemType SystemType {
			get {
				if (radioNexus.Checked)      return SystemType.Nexus;
				if (radioZeon.Checked)       return SystemType.Zeon;
				if (radioOZone.Checked)      return SystemType.OZone;
				if (radioLaserforce.Checked) return SystemType.Laserforce;
				if (radioTorn.Checked)       return SystemType.Torn;
				return SystemType.Demo;
			}

			set {
				switch (value) {
					case SystemType.Nexus:       radioNexus.Checked      = true;  break;
					case SystemType.Zeon:        radioZeon.Checked       = true;  break;
					case SystemType.OZone:		 radioOZone.Checked      = true;  break;
					case SystemType.Laserforce:  radioLaserforce.Checked = true;  break;
					case SystemType.Torn:        radioTorn.Checked       = true;  break;
					default:                     radioDemo.Checked       = true;  break;
				}
			}
		}

		public string ServerAddress { get { return textBoxServerAddress.Text; }  set { textBoxServerAddress.Text = value; } }
		public string ServerPort { get { return serverPort.Text; } set { serverPort.Text = value; } }


		public GroupPlayersBy GroupPlayersBy {
			get {
				if (radioAlias.Checked) return GroupPlayersBy.Alias;
				if (radioLotr.Checked)  return GroupPlayersBy.Lotr;
				return GroupPlayersBy.Colour;
			}

			set {
				switch (value) {
					case GroupPlayersBy.Alias:  radioAlias.Checked  = true;  break;
					case GroupPlayersBy.Lotr:   radioLotr.Checked   = true;  break;
					default:                    radioColour.Checked = true;  break;
				}
			}
		}

		public bool AutoUpdateScoreboard { get { return checkBoxAutoUpdateScoreboard.Checked; }
			                               set { checkBoxAutoUpdateScoreboard.Checked = value; } }

		public bool AutoUpdateTeams { get { return checkBoxAutoUpdateTeams.Checked; }
		                              set { checkBoxAutoUpdateTeams.Checked = value; } }

		public string ReportsFolder
		{
			get { return textBoxReportsFolder.Text; }
			set { textBoxReportsFolder.Text = value; }
		}

		public string UploadMethod {
			get {
				if (radioFtp.Checked) return (string)radioFtp.Tag;
				if (radioSftp.Checked) return (string)radioSftp.Tag;
				if (radioHttp.Checked) return (string)radioHttp.Tag;
				if (radioHttps.Checked) return (string)radioHttps.Tag;
				return null;
			}

			set {
				switch (value) {
					case "ftp":   radioFtp.Checked   = true;  break;
					case "sftp":  radioSftp.Checked  = true;  break;
					case "http":  radioHttp.Checked  = true;  break;
					case "https": radioHttps.Checked = true;  break;
					default:                                  break;
				}
			}
		}

		public string UploadSite { get { return textBoxSite.Text; }
		                           set { textBoxSite.Text = value; } }

		public string Username { get { return textBoxUsername.Text; }
		                         set { textBoxUsername.Text = value; } }

		public string Password { get { return textBoxPassword.Text; }
		                         set { textBoxPassword.Text = value; } }

		public int WebPort {
			get { return checkBoxWebServer.Checked ? (int)numericPort.Value : 0; }
			set {
				checkBoxWebServer.Checked = value != 0;
				if (value != 0)
					numericPort.Value = value;
			}
		}

		public bool WindowsAuth { get { return radioWindowsAuth.Checked; }
		                          set { radioWindowsAuth.Checked = value; radioSqlAuth.Checked = !value; } }
		
		public string Sqluser { get { return textBoxSqlUser.Text; }
		                        set { textBoxSqlUser.Text = value; } }

		public string SqlPassword { get { return textBoxSqlPassword.Text; }
		                            set { textBoxSqlPassword.Text = value; } }
		
		public string LogFolder { get { return textBoxLogFolder.Text; }
			                      set { textBoxLogFolder.Text = value; } }

		public bool HostRemoteTorn { get { return hostRemoteTorn.Checked; }
										set { hostRemoteTorn.Checked = value; } }
		public string RemoteTornPort { get { return remoteTornPort.Text; } 
										set { remoteTornPort.Text = value; } }

		void CheckBoxWebServerCheckedChanged(object sender, EventArgs e)
		{
			labelPort.Enabled = checkBoxWebServer.Checked;
			numericPort.Enabled = checkBoxWebServer.Checked;
		}

		void RadioLaserforceCheckedChanged(object sender, EventArgs e)
		{
			if (radioLaserforce.Checked)
				textBoxServerAddress.Text = "lf-main\\lf6";

			radioWindowsAuth.Enabled = radioLaserforce.Checked;
			radioSqlAuth.Enabled = radioLaserforce.Checked;

			RadioSqlAuthCheckedChanged(sender, e);			
		}

		void RadioSqlAuthCheckedChanged(object sender, EventArgs e)
		{
			labelSqlUser.Enabled = radioLaserforce.Checked && radioSqlAuth.Checked;
			labelSqlPassword.Enabled = radioLaserforce.Checked && radioSqlAuth.Checked;
			textBoxSqlUser.Enabled = radioLaserforce.Checked && radioSqlAuth.Checked;
			textBoxSqlPassword.Enabled = radioLaserforce.Checked && radioSqlAuth.Checked;	

			if (textBoxSqlUser.Enabled && string.IsNullOrEmpty(textBoxSqlUser.Text))
				textBoxSqlUser.Text = "sa";
		}

		void ButtonFolderClick(object sender, EventArgs e)
		{
			TextBox textBox = sender == buttonLogFolder ? textBoxLogFolder : textBoxReportsFolder;

			if (!string.IsNullOrEmpty(textBox.Text))
			    folderBrowserDialog1.SelectedPath = textBox.Text;

			if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
				textBox.Text = folderBrowserDialog1.SelectedPath;
		}

        private void hostRemoteTorn_CheckedChanged(object sender, EventArgs e)
        {
			remoteTornPort.Enabled = hostRemoteTorn.Checked;
        }
    }
}
