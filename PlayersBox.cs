
using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Torn;

namespace Torn.UI
{
	/// <summary>
	/// Description of UserControl1.
	/// </summary>
	public partial class PlayersBox : BaseBox
	{
		public PlayersBox()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		
		public void LoadGame(League league, ServerGame serverGame)
		{
			Items.Clear();

			if (serverGame.Players != null && serverGame.Players.Count == 0 && serverGame.Game != null)  // ServerGame is a fake, created from game; but ServerGame.Players is not filled in yet, so fill it in.
				foreach (var player in serverGame.Game.Players)
				{
					var serverPlayer = new ServerPlayer();
					player.CopyTo(serverPlayer);

					LeaguePlayer leagueplayer = league.LeaguePlayer(player);
					if (leagueplayer != null)
						serverPlayer.Alias = leagueplayer.Name;

					serverGame.Players.Add(serverPlayer);
				}

			if (serverGame.Players != null)
				foreach (var player in serverGame.Players)
				{
				ListViewItem item = new ListViewItem(player.PackName, (int)player.Colour);
					item.SubItems.Add(player.Alias);
					item.SubItems.Add(player.Score.ToString(CultureInfo.CurrentCulture));
					item.Tag = player;
					player.Item = item;
					Items.Add(item);
				}
		}
	}
}
