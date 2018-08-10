using System;
using System.Drawing;
using System.Windows.Forms;

namespace Torn.UI
{
	public enum SystemType { Acacia, Zeon, Laserforce, Demo };
	public enum GroupPlayersBy { Alias, Colour, Lotr };
	public enum SortTeamsBy { Rank, Colour };

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
				if (radioAcacia.Checked) return SystemType.Acacia;
				if (radioZeon.Checked) return SystemType.Zeon;
				if (radioLaserforce.Checked) return SystemType.Laserforce;
				return SystemType.Demo;
			}

			set {
				switch (value) {
					case SystemType.Acacia:      radioAcacia.Checked     = true;  break;
					case SystemType.Zeon:        radioZeon.Checked       = true;  break;
					case SystemType.Laserforce:  radioLaserforce.Checked = true;  break;
					default:                     radioDemo.Checked       = true;  break;
				}
			}
		}

		public string ServerAddress { get { return textBoxServerAddress.Text; }  set { textBoxServerAddress.Text = value; } }

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

//		public SortTeamsBy SortTeamsBy {
//			get { return radioSortColour.Checked ? SortTeamsBy.Colour : SortTeamsBy.Rank; }
//
//			set {
//				switch (value) {
//					case SortTeamsBy.Colour:  radioSortColour.Checked = true;  break;
//					default:                    radioSortRank.Checked = true;  break;
//				}
//			}
//		}

		public bool AutoUpdateScoreboard { get { return checkBoxAutoUpdateScoreboard.Checked; }
			                               set { checkBoxAutoUpdateScoreboard.Checked = value; } }

		public bool AutoUpdateTeams { get { return checkBoxAutoUpdateTeams.Checked; }
		                              set { checkBoxAutoUpdateTeams.Checked = value; } }
	}
}
