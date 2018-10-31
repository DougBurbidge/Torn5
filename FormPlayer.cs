using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Torn;

namespace Torn.UI
{
	/// <summary>
	/// Let the user choose a player. Used to identify a pack, or re-ID a player on a team.
	/// </summary>
	public partial class FormPlayer : Form
	{
		public LaserGameServer LaserGameServer { get; set; }
		public string PlayerId { get { return textId.Text; }  set { textId.Text = value; } }
		public string PlayerAlias { get { return textSearch.Text; }  private set { textSearch.Text = value; } }
		
		string search;

		public FormPlayer()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
		}

		void FormPlayerShown(object sender, EventArgs e)
		{
			buttonOK.Enabled = listViewPlayers.SelectedItems.Count == 1;
			search = null;
		}

		void ListViewPlayersDoubleClick(object sender, EventArgs e)
		{
			if (buttonOK.Enabled)
				buttonOK.PerformClick();
		}

		void TextSearchKeyUp(object sender, KeyEventArgs e)
		{
			search = textSearch.Text;
			if (search.Length >= 1)
			{
                try
                {
                    var players = LaserGameServer.GetPlayers(search);

					listViewPlayers.Items.Clear();
                    foreach (var player in players)
                    {
                        var item = new ListViewItem(player.Alias);
                        item.SubItems.Add(player.Name);
                        item.Tag =player.UserId;
                        item.Selected = player.Alias.StartsWith(search, true, CultureInfo.CurrentCulture);
                        listViewPlayers.Items.Add(item);
                    }
				}
				finally
				{
				
				}
			}

			if (!LaserGameServer.HasNames())
				listViewPlayers.Columns[1].Width = 0;
		}

		void ListViewPlayersSelectedIndexChanged(object sender, EventArgs e)
		{
			if (listViewPlayers.SelectedItems.Count == 1)
			{
				textSearch.Text = listViewPlayers.SelectedItems[0].Text;

				if (listViewPlayers.SelectedItems[0].Text.StartsWith(search, true, CultureInfo.CurrentCulture))
				{
					textSearch.SelectionStart = search.Length;
					textSearch.SelectionLength = textSearch.Text.Length - textSearch.SelectionStart;
				}

				textId.Text = (string)listViewPlayers.SelectedItems[0].Tag;
			}
			buttonOK.Enabled = !string.IsNullOrEmpty(textId.Text);
		}
	}
}
