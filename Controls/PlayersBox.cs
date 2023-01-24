using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace Torn.UI
{
	/// <summary>
	/// Description of UserControl1.
	/// </summary>
	public partial class PlayersBox : BaseBox
	{
		public PlayersBox()
		{
			InitializeComponent();
			SetSort(0, SortOrder.Ascending);  // Default to sorting by colour then pack.
		}

		public void LoadGame(League league, ServerGame serverGame)
		{
			Items.Clear();
		if (serverGame.Players.Count == 0 && serverGame.Game != null)  // ServerGame is a fake, created from game; but ServerGame.Players is not filled in yet, so fill it in.
				foreach (var player in serverGame.Game.AllPlayers())
				{
					var serverPlayer = new ServerPlayer();
					player.CopyTo(serverPlayer);

					if (league != null)
					{
						LeaguePlayer leagueplayer = league.LeaguePlayer(player);
						if (leagueplayer != null)
							serverPlayer.Alias = leagueplayer.Name;
					}

					serverGame.Players.Add(serverPlayer);
				}

			if (serverGame.Players != null)
				foreach (var player in serverGame.Players)
				{
					league?.Load(league?.FileName);
					LeaguePlayer leaguePlayer = league?.Players?.Find(p => p.Id == player.PlayerId);
					GamePlayer gamePlayer = league?.Games(false)?.Find(g => g.Time == serverGame.Time)?.Players()?.Find(p => p.PlayerId == player.PlayerId);
					if(gamePlayer != null && gamePlayer.Grade != null)
                    {
						player.Grade = gamePlayer.Grade;
                    } else if (leaguePlayer != null && leaguePlayer.Grade != null)
					{
						player.Grade = leaguePlayer.Grade;
					}

					bool isRichoCard = player.QRCode != null && player.QRCode.StartsWith("00005");

					int yCard = player.YellowCards;
					int rCard = player.RedCards;

					string alias = isRichoCard ? "**** " + player.GetFormattedAlias() + " ****" : player.GetFormattedAlias();

					bool isNewPlayer = league != null && leaguePlayer == null;

					bool isChangedAlias = league != null && !isNewPlayer && league.Players.Find(p => p.Name == player.Alias) == null;

					string tooltip = "";

					ListViewItem item = new ListViewItem(player.Pack, (int)player.Colour);
					if ((player.Grade == null && league != null && league.IsAutoHandicap) )
                    {
						item.BackColor = Color.FromName("yellow");
						tooltip += "No grade found on player.";
					}
					if (isNewPlayer)
					{
						item.BackColor = Color.FromName("lightgreen");
						tooltip += "Player does not exist in league yet.\n";
					}
					if(isChangedAlias && gamePlayer == null)
                    {
						item.BackColor = Color.FromName("orange");
						tooltip += "Player Alias does not match saved alias for player.\n" + "Server: " + player.Alias + " League: " + leaguePlayer.Name;
                    }

					item.ToolTipText = tooltip;

					var score = player.Score > 0 && player.IsEliminated && league.ZeroElimed ? "0" : player.Score.ToString(CultureInfo.CurrentCulture);

					item.SubItems.Add(alias);
					item.SubItems.Add(score);
					item.SubItems.Add(player.Grade);
					item.Tag = player;
						player.Item = item;
						Items.Add(item);
				}

			ListView.Sort();
		}
	}
}
