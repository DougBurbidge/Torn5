using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Torn;

namespace Torn.UI
{
	/// <summary>
	/// TeamBox encapsulates the ListBox used to display a team, linked to the data about that team in this game.
	/// </summary>
	public partial class TeamBox : BaseBox
	{
		int rank;
		public int Rank 
		{
			get { return rank; }
			set
			{
				rank = value;
				ListView.Columns[0].Text = Items.Count == 0 ? "Pack" : rank.ToString(CultureInfo.CurrentCulture);
			}
		}

		double score;
		public int Score { get { return (int)score; } }
		public LeagueTeam LeagueTeam { get; set; }
		public GameTeam GameTeam { get; set; }
		public League League { get; set; }
		/// <summary>Dialog used when the user ID's a player.</summary>
		public FormPlayer FormPlayer { get; set; }

		public TeamBox()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			ListView.ContextMenuStrip = contextMenuStrip1;
			GameTeam = new GameTeam();
		}

		public override void Clear()
		{
			foreach (var serverPlayer in Players())
				serverPlayer.Item = null;  // Break the link from the serverPlayer to its ListViewItem, since we're about to throw away the ListViewItem.
			Items.Clear();
			score = 0;
			Rank = 0;
			LeagueTeam = null;
			GameTeam = new GameTeam();
			ListView.Columns[2].Text = "Score";
		}

		protected override void Recalculate(bool guessTeam = true)
		{
			if (Items.Count == 0)
			{
				rank = 0;
				score = 0;
				ListView.Columns[0].Text = "Pack";
				ListView.Columns[1].Text = "Players";
				ListView.Columns[2].Text = "Score";
				return;
			}

			foreach (var listPlayer in Players())
			{
				var gamePlayer = GameTeam.Players.Find(x => x.PlayerId == listPlayer.PlayerId);
				if (gamePlayer == null)
				{
					gamePlayer = new GamePlayer();
					GameTeam.Players.Add(gamePlayer);
				}
				listPlayer.CopyTo(gamePlayer);
			}

			score = GameTeam.CalculateScore(League == null ? HandicapStyle.Percent : League.HandicapStyle);
			ListView.Columns[2].Text = Score.ToString(CultureInfo.InvariantCulture);

			var ids = new List<string>();
			foreach (var listPlayer in Players())
				ids.Add(listPlayer.PlayerId);

//			if (ListView.ListViewItemSorter == null)
//				ListView.ListViewItemSorter = new SortByScore();
//			ListView.Sort();

			if (guessTeam && League != null)
				LeagueTeam = League.GuessTeam(ids);

			ListView.Columns[1].Text = LeagueTeam == null ? "Players" : LeagueTeam.Name;
		}
		
		LeaguePlayer LeaguePlayer(ListViewItem item)
		{
			return item.Tag is ServerPlayer && ((ServerPlayer)item.Tag).LeaguePlayer != null ? ((ServerPlayer)item.Tag).LeaguePlayer : null;
		}

		ToolStripMenuItem FindMenu(string s)
		{
			if (!string.IsNullOrEmpty(s))
				foreach (ToolStripItem item in menuIdentifyTeam.DropDownItems)
					if (item.Text.Contains(s.ToUpper()[0]))
						return (ToolStripMenuItem)item;

			return (ToolStripMenuItem)menuIdentifyTeam.DropDownItems[menuIdentifyTeam.DropDownItems.Count - 1];  // "Other"
		}

		void ContextMenuStrip1Opening(object sender, CancelEventArgs e)
		{
			menuHandicapTeam.Enabled   = League != null;
			menuRememberTeam.Enabled   = League != null;
			menuUpdateTeam.Enabled     = LeagueTeam != null;
			menuNameTeam.Enabled       = LeagueTeam != null;
			menuIdentifyTeam.Enabled   = League != null;
			menuIdentifyPlayer.Enabled = ListView.SelectedItems.Count == 1;
			menuHandicapPlayer.Enabled = ListView.SelectedItems.Count == 1;
			menuMergePlayer.Enabled    = ListView.SelectedItems.Count == 2;
			//menuAdjustTeamScore.Enabled = always true.

			menuIdentifyTeam.DropDownItems.Clear();

			if (League == null)
				return;

			if (League.Teams.Count < 49)
				foreach (var team in League.Teams)
				{
					var item = new ToolStripMenuItem(team.Name);
					item.Tag = team;
					item.Click += MenuIdentifyClick;
					menuIdentifyTeam.DropDownItems.Add(item);
				}
			else  // There are so many teams that a flat list will be large and hard to visually scan. Create intermediate items.
			{
				if (League.Teams.Count < 73)  // 72 is geometric mean of 8 squared and 9 squared -- less than this and 8 or so bins make sense.
					foreach (string s in new string[9] { "ABC", "DEF", "GHI", "JKL", "MNO", "PQRS", "TUV", "WXYZ", "Other" })
						menuIdentifyTeam.DropDownItems.Add(new ToolStripMenuItem(s));
				else if (League.Teams.Count < 338)  // 338 is geometric mean of 13 squared and 26 squared -- more than this, and 26 bins makes sense. Less than this and 13 bins makes sense.
					foreach (string s in new string[14] { "AB", "CD", "EF", "GH", "IJ", "KL", "MN", "OP", "QR", "ST", "UV", "WX", "YZ", "Other" })
						menuIdentifyTeam.DropDownItems.Add(new ToolStripMenuItem(s));
				else
					foreach (string s in new string[27] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "Other" })
						menuIdentifyTeam.DropDownItems.Add(new ToolStripMenuItem(s));

				foreach (var team in League.Teams)
				{
					var item = new ToolStripMenuItem(team.Name);
					item.Tag = team;
					item.Click += MenuIdentifyClick;
					FindMenu(team.Name).DropDownItems.Add(item);
				}
			}
		}

		void MenuAdjustTeamScoreClick(object sender, EventArgs e)
		{
			GameTeam.Adjustment = InputDialog.GetInteger("Adjustment", "Set team score adjustment", GameTeam.Adjustment);
			Recalculate(false);
		}

		void MenuHandicapTeamClick(object sender, EventArgs e)
		{
			LeagueTeam.Handicap.Value = InputDialog.GetInteger("Handicap", "Set team handicap (" + League.HandicapStyle.ToString() + ")" , LeagueTeam.Handicap.Value);
			Recalculate(false);
		}

		private void MenuIdentifyClick(object sender, EventArgs e)
		{
			LeagueTeam = (LeagueTeam)((ToolStripMenuItem)sender).Tag;
			ListView.Columns[1].Text = LeagueTeam == null ? "Players" : LeagueTeam.Name;
		}

		void MenuNameTeamClick(object sender, EventArgs e)
		{
			LeagueTeam.Name = InputDialog.GetInput("Name: ", "Set a team name", LeagueTeam.Name);
			ListView.Columns[1].Text = LeagueTeam == null ? "Players" : LeagueTeam.Name;
		}

		void MenuRememberTeamClick(object sender, EventArgs e)
		{
			if (LeagueTeam == null)
			{
				LeagueTeam = new LeagueTeam();
				foreach (var serverPlayer in Players())
				{
					var leaguePlayer = serverPlayer.LeaguePlayer ?? League.Players.Find(p => p.Id == serverPlayer.PlayerId);
					if (leaguePlayer == null)
					{
						leaguePlayer = new LeaguePlayer();
						leaguePlayer.Name = serverPlayer.Alias;
						leaguePlayer.Id = serverPlayer.PlayerId;
						League.Players.Add(leaguePlayer);
					}
					LeagueTeam.Players.Add(leaguePlayer);
				}
				League.Teams.Add(LeagueTeam);
			}
			else
			{
				// offer to create a new team?
			}
		}

		void MenuSortTeamsClick(object sender, EventArgs e)
		{
			if (SortTeamsByRank != null)
				SortTeamsByRank();
		}

		void MenuUpdateTeamClick(object sender, EventArgs e)
		{
			foreach (var serverPlayer in Players())
			{
				var leaguePlayer = serverPlayer.LeaguePlayer ?? League.Players.Find(p => p.Id == serverPlayer.PlayerId);
				if (leaguePlayer == null)
				{
					leaguePlayer = new LeaguePlayer();
					leaguePlayer.Name = serverPlayer.Alias;
					leaguePlayer.Id = serverPlayer.PlayerId;
					League.Players.Add(leaguePlayer);
				}
				if (!LeagueTeam.Players.Contains(leaguePlayer))
					LeagueTeam.Players.Add(leaguePlayer);
			}
		}

		void MenuIdentifyPlayerClick(object sender, EventArgs e)
		{
			var player = (ServerPlayer)ListView.SelectedItems[0].Tag;
			FormPlayer.PlayerId = player.PlayerId;
			if (FormPlayer.ShowDialog() == DialogResult.OK)
			{
				player.PlayerId = FormPlayer.PlayerId;
				player.LeaguePlayer = null;
			}
		}

		void MenuHandicapPlayerClick(object sender, EventArgs e)
		{
			
		}
		
		void MenuMergePlayerClick(object sender, EventArgs e)
		{
			
		}
	}

	class SortByScore : IComparer
	{
		int IComparer.Compare(object x, object y)
		{
			return (x is ListViewItem && y is ListViewItem && ((ListViewItem)x).Tag is ServerPlayer && ((ListViewItem)y).Tag is ServerPlayer) ?
				((ServerPlayer)((ListViewItem)y).Tag).Score - ((ServerPlayer)((ListViewItem)x).Tag).Score :
				0;
		}
	}
}
