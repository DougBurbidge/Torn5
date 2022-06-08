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
		public League CurrentLeague { get; set; }
		public string PlayerId { get { return textId.Text; }  set { textId.Text = value; } }
		public string PlayerAlias { get { return textSearch.Text; }  set { textSearch.Text = value; } }
		
		string search;
		int caretPos;

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
			listViewPlayers.Items.Clear();
			TextSearchKeyUp(null, null);
			textSearch.Focus();
		}

		void ListViewPlayersDoubleClick(object sender, EventArgs e)
		{
			if (buttonOK.Enabled)
				buttonOK.PerformClick();
		}

		void TextSearchKeyDown(object sender, KeyEventArgs e)
		{
			caretPos = textSearch.SelectionStart;
		}

		void TextSearchKeyUp(object sender, KeyEventArgs e)
		{
			search = textSearch.Text;

			if (e != null && (e.KeyCode == Keys.Back || e.KeyCode == Keys.Left))
			{
				if (caretPos > 0)
					textSearch.Select(caretPos - 1, textSearch.Text.Length - caretPos + 1);
			}
			else if (e != null && e.KeyCode == Keys.Right)
			{
				if (caretPos < textSearch.Text.Length)
					textSearch.Select(caretPos + 1, textSearch.Text.Length - caretPos - 1);
			}
			else
			{
				var players = LaserGameServer.GetPlayers(search, CurrentLeague?.Players);

				listViewPlayers.Items.Clear();
				foreach (var player in players)
				{
					var item = new ListViewItem(player.Alias);
					item.SubItems.Add(player.Name);
					item.Tag = player.Id;
					listViewPlayers.Items.Add(item);
				}

				for (int i = 0; i < listViewPlayers.Items.Count; i++)
					if (listViewPlayers.Items[i].Text.StartsWith(search, true, CultureInfo.CurrentCulture))
					{
						listViewPlayers.Items[i].Selected = true;
						break;
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

				if (search != null && listViewPlayers.SelectedItems[0].Text.StartsWith(search, true, CultureInfo.CurrentCulture))
				{
					textSearch.SelectionStart = search.Length;
					textSearch.SelectionLength = textSearch.Text.Length - textSearch.SelectionStart;
				}

				textId.Text = (string)listViewPlayers.SelectedItems[0].Tag;
			}
			buttonOK.Enabled = !string.IsNullOrEmpty(textId.Text);
		}

        private void textId_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
