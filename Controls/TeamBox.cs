using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

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

		LeagueTeam leagueTeam;

		public LeagueTeam LeagueTeam 
		{ 
			get { return leagueTeam; }

			set
			{
				if (leagueTeam == null && value != null && value.Handicap == null)
					value.Handicap = handicap;
				leagueTeam = value;
				GameTeam.TeamId = leagueTeam == null ? (int?)null : leagueTeam.TeamId;
				ListView.Columns[1].Text = leagueTeam == null ? "Players" : leagueTeam.Name;
			}
		}

		Handicap handicap;
		public Handicap Handicap 
		{
			get 
			{
				return LeagueTeam == null ? handicap : LeagueTeam.Handicap;
			}

			private set { }
		}

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
			SetSort(2, SortOrder.Descending);  // Default to sorting by score.
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
			handicap = new Handicap();
			ListView.Columns[1].Text = "Players";
			ListView.Columns[2].Text = "Score";
			ListView.Columns[3].Text = "Grade";
		}

		LeagueTeam GetLeagueTeamFromFile()
        {
			League.Load(League.FileName);
			List<string> playerIds = new List<string>();
			foreach (ServerPlayer player in Players())
			{
				playerIds.Add(player.PlayerId);

			}
			LeagueTeam leagueTeam = League.GuessTeam(playerIds);
			return leagueTeam;
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
				ListView.Columns[3].Text = "Grade";
				return;
			}

			var tempTeam = GameTeam.Clone();

			tempTeam.Players.Clear();
			tempTeam.Players.AddRange(Players());

			if (guessTeam && League != null)
			{
				var ids = new List<string>();
				foreach (var listPlayer in Players())
					ids.Add(listPlayer.PlayerId);

				LeagueTeam = League.GuessTeam(ids);
				if (LeagueTeam != null)
					tempTeam.TeamId = LeagueTeam.TeamId;
			}

			if(League != null)
            {
				if (League.isAutoHandicap)
				{
					ListView.Columns[3].Text = League.CalulateTeamCap(GameTeam).ToString() + "%";
				} else
                {
					LeagueTeam leagueTeam = GetLeagueTeamFromFile();
					if (leagueTeam.Handicap != null)
                    {
						ListView.Columns[3].Text = leagueTeam.Handicap.ToString();
					}
				}
				score = League.CalculateScore(GameTeam);
			} else
            {
				score = 0;
            }

			ListView.Columns[2].Text = Score.ToString(CultureInfo.InvariantCulture) +
				(GameTeam.Adjustment == 0 ? "" : "*");
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
			menuHandicapTeam.Enabled   = League != null && !League.isAutoHandicap;
			menuRememberTeam.Enabled   = League != null;
			menuUpdateTeam.Enabled     = LeagueTeam != null;
			menuNameTeam.Enabled       = LeagueTeam != null;
			menuIdentifyTeam.Enabled   = League != null;
			menuIdentifyPlayer.Enabled = ListView.SelectedItems.Count == 1;
			menuHandicapPlayer.Enabled = false;// ListView.SelectedItems.Count == 1;
			menuAdjustPlayerScore.Enabled = ListView.SelectedItems.Count == 1;
			menuMergePlayer.Enabled    = ListView.SelectedItems.Count == 2;
			menuGradePlayer.Enabled = ListView.SelectedItems.Count == 1 && League != null && League.isAutoHandicap && LeagueTeam != null;
			//menuAdjustTeamScore.Enabled = always true.

			menuIdentifyTeam.DropDownItems.Clear();
			menuGradePlayer.DropDownItems.Clear();

			if (League == null)
				return;

			foreach (var grade in League.Grades)
			{
				var item = new ToolStripMenuItem(grade.Name)
				{
					Tag = grade
				};
				item.Click += MenuGradePlayerClick;
				menuGradePlayer.DropDownItems.Add(item);
			}

			if (League.Teams.Count < 49)
				foreach (var team in League.Teams)
				{
					var item = new ToolStripMenuItem(team.Name)
					{
						Tag = team
					};
					item.Click += MenuIdentifyTeamClick;
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
					var item = new ToolStripMenuItem(team.Name)
					{
						Tag = team
					};
					item.Click += MenuIdentifyTeamClick;
					FindMenu(team.Name).DropDownItems.Add(item);
				}
			}

			if (ListView.SelectedItems.Count == 1)
			{
				var item = ListView.SelectedItems[0];
				var subIndex = item.SubItems.Count == 0 || string.IsNullOrEmpty(item.SubItems[1].Text) ? 0 : 1;
				menuIdentifyPlayer.Text = "Identify " + item.SubItems[subIndex].Text.Trim() + "...";
				menuHandicapPlayer.Text = "Handicap " + item.SubItems[subIndex].Text.Trim() + "...";
			}
			else
			{
				menuIdentifyPlayer.Text = "Identify player";
				menuHandicapPlayer.Text = "Handicap player";
			}
		}

		void MenuAdjustTeamScoreClick(object sender, EventArgs e)
		{
			double a = GameTeam.Adjustment == 0 ? -1000 : GameTeam.Adjustment;
			if (InputDialog.GetDouble("Adjustment", "Set team score adjustment", ref a))
				GameTeam.Adjustment = a;
			Recalculate(false);
		}

		void MenuAdjustVictoryPointsClick(object sender, EventArgs e)
		{
			GameTeam.PointsAdjustment = (double)InputDialog.GetDouble("Victory Points Adjustment", "Set team victory points adjustment", GameTeam.PointsAdjustment);
			Recalculate(false);
		}

		void MenuHandicapTeamClick(object sender, EventArgs e)
		{
			LeagueTeam leagueTeam = GetLeagueTeamFromFile();

			if (leagueTeam != null)
            {
				Handicap.Value = InputDialog.GetDouble("Handicap", "Set team handicap (" + League.HandicapStyle.ToString() + ")" , Handicap.Value ?? 100);

				int index = League.Teams.IndexOf(leagueTeam);

				League.Teams[index].Handicap = new Handicap(Handicap.Value, League.HandicapStyle);

				League.Save();
				Recalculate(false);
			} else
            {
				MessageBoxButtons buttons = MessageBoxButtons.OK;
				MessageBox.Show("Please Identify Team before adding Handicap", "Cannot Apply Handicap", buttons);
            }
		}

		private void MenuIdentifyTeamClick(object sender, EventArgs e)
		{
			LeagueTeam = (LeagueTeam)((ToolStripMenuItem)sender).Tag;
			ListView.Columns[1].Text = LeagueTeam == null ? "Players" : LeagueTeam.Name;
		}

		private void MenuGradePlayerClick(object sender, EventArgs e)
        {
			Grade grade = (Grade)((ToolStripMenuItem)sender).Tag;
			ListView.SelectedItems[0].SubItems[3].Text = grade.Name;

			ServerPlayer player = (ServerPlayer)ListView.SelectedItems[0].Tag;

			if(GameTeam.Players.Count() == 0)
            {
				GameTeam.Players.AddRange(Players());
			}

			int index = GameTeam.Players.FindIndex(p => p.PlayerId == player.PlayerId);

			GameTeam.Players[index].Grade = grade.Name;

			Recalculate(false);

		}

		void MenuNameTeamClick(object sender, EventArgs e)
		{
			LeagueTeam.Name = InputDialog.GetInput("Name: ", "Set a team name", LeagueTeam.Name);
			ListView.Columns[1].Text = LeagueTeam == null ? "Players" : LeagueTeam.Name;
			League.Save();
			League.Load(League.FileName);
		}

		void MenuRememberTeamClick(object sender, EventArgs e)
		{
			if (LeagueTeam == null || MessageBox.Show("This box already has a team (" + LeagueTeam.Name + "). Create a new team anyway?",
													  "Create new team", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == DialogResult.Yes)
				RememberTeam();
		}

		public void RememberTeam()
		{
			LeagueTeam = new LeagueTeam();
			foreach (var serverPlayer in Players())
			{
				var leaguePlayer = League.LeaguePlayer(serverPlayer) ?? League.Players.Find(p => p.Id == serverPlayer.PlayerId);
				if (leaguePlayer == null)
				{
					leaguePlayer = new LeaguePlayer
					{
						Name = serverPlayer.Alias,
						Id = serverPlayer.PlayerId
					};
					League.Players.Add(leaguePlayer);
				}
				LeagueTeam.Players.Add(leaguePlayer);
			}
			League.AddTeam(LeagueTeam);
		}

		void MenuSortTeamsClick(object sender, EventArgs e)
		{
			SortTeamsByRank?.Invoke();
		}

		void MenuUpdateTeamClick(object sender, EventArgs e)
		{
			foreach (var serverPlayer in Players())
			{
				var leaguePlayer = League.LeaguePlayer(serverPlayer) ?? League.Players.Find(p => p.Id == serverPlayer.PlayerId);
				if (leaguePlayer == null)
				{
					leaguePlayer = new LeaguePlayer
					{
						Name = serverPlayer.Alias,
						Id = serverPlayer.PlayerId
					};
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
			FormPlayer.PlayerAlias = player.Alias;
			if (FormPlayer.ShowDialog() == DialogResult.OK)
			{
				player.PlayerId = FormPlayer.PlayerId;
				player.Alias = FormPlayer.PlayerAlias;
				ListView.SelectedItems[0].SubItems[1].Text = player.Alias;
			}
		}

		void MenuHandicapPlayerClick(object sender, EventArgs e)
		{
			// TODO: Implement.
		}
		
		void MenuMergePlayerClick(object sender, EventArgs e)
		{
			var player1 = (ServerPlayer)ListView.SelectedItems[0].Tag;
			var player2 = (ServerPlayer)ListView.SelectedItems[1].Tag;

			player1.Score = player1.Score + player2.Score;

			ListView.SelectedItems[0].SubItems[2].Text = player1.Score.ToString();

			ListView.SelectedItems[1].Remove();

			Recalculate(false);
		}

		private void menuAdjustPlayerScoreClick(object sender, EventArgs e)
		{
			double penalty = -1000;
			InputDialog.GetDouble("Adjustment", "Set player score adjustment", ref penalty);
			var player1 = (ServerPlayer)ListView.SelectedItems[0].Tag;

			player1.Score = player1.Score + penalty;

			ListView.SelectedItems[0].SubItems[2].Text = player1.Score.ToString();

			Recalculate(false);
		}
	}

	class SortByScore : IComparer
	{
		int IComparer.Compare(object x, object y)
		{
			return (x is ListViewItem itemX && y is ListViewItem itemY && itemX.Tag is ServerPlayer playerX && itemY.Tag is ServerPlayer playerY) ?
				playerY.Score.CompareTo(playerX.Score) :
				0;
		}
	}
}
