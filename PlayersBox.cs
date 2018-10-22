
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
		
		public void LoadGame(League league, ServerGame game)
		{
			Items.Clear();

			if (game.Players.Count == 0 && game.Game != null)  // ServerGame is a fake, created from game; but ServerGame.players is not filled in yet, so fill it in.
				foreach (var player in game.Game.Players)
				{
					var serverPlayer = new ServerPlayer();
					LeaguePlayer leagueplayer = league.LeaguePlayer(player);
					if (leagueplayer != null)
						serverPlayer.Alias = leagueplayer.Name;
					serverPlayer.PlayerId = player.PlayerId;
					serverPlayer.PandCPlayerTeamId = (int)player.Colour - 1;
					serverPlayer.Score = player.Score;

					game.Players.Add(serverPlayer);
				}

			foreach (var player in game.Players)
			{
				ListViewItem item = new ListViewItem(player.PackName, player.PandCPlayerTeamId + 1);
				item.SubItems.Add(player.Alias);
				item.SubItems.Add(player.Score.ToString(CultureInfo.CurrentCulture));
				item.Tag = player;
				player.Item = item;
				Items.Add(item);
			}
		}
	}
}
