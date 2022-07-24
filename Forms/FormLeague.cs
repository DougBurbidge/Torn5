using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Torn;

namespace Torn.UI
{
	/// <summary>
	/// Edit a League.
	/// </summary>
	public partial class FormLeague : Form
	{
		public League League { get; set; }
		public FormPlayer FormPlayer { get; set; }

		readonly List<NumericUpDown> victory;  // A list of victory points boxes.

		public FormLeague()
		{
			// The InitializeComponent() call is required for Windows Forms designer support.
			InitializeComponent();
			
			victory = new List<NumericUpDown>();
		}

		void FormLeagueShown(object sender, EventArgs e)
		{
			League.Load(League.FileName);
			Text = "Torn -- " + League.Title;
			treeView1.Nodes.Clear();
			
			foreach (var team in League.Teams)
			{
				var teamNode = new TreeNode(team.Name)
				{
					Tag = team
				};
				foreach (var player in team.Players)
				{
					var playerNode = new TreeNode(player.Name)
					{
						Tag = player
					};

					var gradeNode = playerNode.Nodes.Add(player.Grade);
					gradeNode.Tag = player.Grade;

					teamNode.Nodes.Add(playerNode);
				}
				treeView1.Nodes.Add(teamNode);
			}

			totalScore.Checked = League.VictoryPoints.Count == 0;
			victoryPoints.Checked = League.VictoryPoints.Any();

			for (int i = 0; i < League.VictoryPoints.Count; i++)
				SetVictoryBox(i, League.VictoryPoints[i]);

			radioButtonPercent.Checked = League.HandicapStyle == HandicapStyle.Percent;
			radioButtonPlus.Checked = League.HandicapStyle == HandicapStyle.Plus;
			radioButtonMinus.Checked = League.HandicapStyle == HandicapStyle.Minus;
			radioButtonNone.Checked = League.HandicapStyle == HandicapStyle.None;

			RankCheckedChanged(null, null);
		}

		private void setupGradeSelector(string alias, string grade)
        {
			playerGradeAlias.Visible = true;
			playerGradeAlias.Text = alias + " Grade";
			playerGradeBox.Items.Clear();
			playerGradeBox.Items.Add(AAAName.Text);
			playerGradeBox.Items.Add(AName.Text);
			playerGradeBox.Items.Add(BBName.Text);
			playerGradeBox.Items.Add(BName.Text);
			playerGradeBox.Items.Add(CName.Text);
			playerGradeBox.Items.Add(DName.Text);
			playerGradeBox.Items.Add(EName.Text);
			playerGradeBox.Items.Add(FName.Text);
			playerGradeBox.Items.Add(GName.Text);
			playerGradeBox.Items.Add(HName.Text);
			playerGradeBox.Items.Add(IName.Text);

			playerGradeBox.SelectedItem = grade;
		}

		void TreeView1AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (treeView1.SelectedNode.Tag is string grade && treeView1.SelectedNode.Parent.Tag is LeaguePlayer leaguePlayer)
			{
				tabControl1.SelectedTab = HandicapPage;
				setupGradeSelector(leaguePlayer.Name, grade);
			}
			else
			{
				tabControl1.SelectedTab = scoresPage;

				if (treeView1.SelectedNode.Tag is LeagueTeam team)
				{
					listViewScores.Items.Clear();
					foreach (var gameTeam in League.Played(team))
					{
						Game game = League.Game(gameTeam);
						var item = new ListViewItem(game == null ? "?" : game.Time.ToString());
						item.SubItems.Add(gameTeam.Score.ToString());
						item.SubItems.Add(League.IsPoints() ? gameTeam.Points.ToString() : game == null ? "?" : (game.Teams.IndexOf(gameTeam) + 1).ToString());
						item.BackColor = gameTeam.Colour.ToColor();
						listViewScores.Items.Add(item);
					}
					listViewScores.Columns[2].Text = League.IsPoints() ? "Points" : "Rank";
				}
				else if (treeView1.SelectedNode.Tag is LeaguePlayer player)
				{
					setupGradeSelector(player.Name, player.Grade);
					listViewScores.Items.Clear();
					foreach (var gamePlayer in League.Played(player))
					{
						var item = new ListViewItem(League.Game(gamePlayer).Time.ToString());
						item.SubItems.Add(gamePlayer.Score.ToString());
						item.SubItems.Add(gamePlayer.Rank.ToString());
						item.BackColor = gamePlayer.Colour.ToColor();
						listViewScores.Items.Add(item);
					}
					listViewScores.Columns[2].Text = "Rank";
				}
			}
			buttonDeletePlayer.Enabled = treeView1.SelectedNode.Tag is LeaguePlayer;
			buttonReIdPlayer.Enabled = treeView1.SelectedNode.Tag is LeaguePlayer;
			playerGradeAlias.Visible = treeView1.SelectedNode.Tag is string || treeView1.SelectedNode.Tag is LeaguePlayer;
			playerGradeBox.Visible = treeView1.SelectedNode.Tag is string || treeView1.SelectedNode.Tag is LeaguePlayer;
		}

		TreeNode hovered;

		private void TreeView1MouseMove(object sender, MouseEventArgs e)
		{
			hovered = treeView1.GetNodeAt(e.Location);
			if (hovered != null)
				Text = hovered.Text;
			else
				Text = "League";
		}

		private void TreeView1MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				if (hovered != null)
				{
					treeView1.SelectedNode = hovered;
					Text = hovered.Text;

					deletePlayerMenuItem.Visible = treeView1.SelectedNode.Tag is LeaguePlayer;
					reIDPlayerMenuItem.Visible = treeView1.SelectedNode.Tag is LeaguePlayer;
					deleteTeamMenuItem.Visible = treeView1.SelectedNode.Tag is LeagueTeam;
					renameTeamMenuItem.Visible = treeView1.SelectedNode.Tag is LeagueTeam;
				}
			}
		}

		void ButtonRenameLeagueClick(object sender, EventArgs e)
		{
			League.Title = InputDialog.GetInput("Rename League", "Choose a new name for the league", League.Title);
			Text = "Torn -- " + League.Title;
		}

		void ButtonCopyFromLeagueClick(object sender, EventArgs e)
		{
			
		}

		void ButtonAddTeamClick(object sender, EventArgs e)
		{
			string name = null;
			if (InputDialog.ConditionalInput("Add Team", "Choose a name for the new team", ref name))
			{
				var team = new LeagueTeam
				{
					Name = name
				};
				League.AddTeam(team);

				var node = new TreeNode(name)
				{
					Tag = team
				};
				treeView1.Nodes.Add(node);
				treeView1.SelectedNode = node;
			}
		}

		void ButtonDeleteTeamClick(object sender, EventArgs e)
		{
			if (treeView1.SelectedNode.Tag is LeagueTeam team && 
			    MessageBox.Show("Are you sure you want to delete team " + team.Name + "?",
			                    "Delete Team?", MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				League.Teams.Remove(team);
				treeView1.Nodes.Remove(treeView1.SelectedNode);
			}
		}

		void ButtonRenameTeamClick(object sender, EventArgs e)
		{
			if (treeView1.SelectedNode.Tag is LeagueTeam team)
			{
				string name = team.Name;
			    if (InputDialog.UpdateInput("Rename Team", "Choose a new name for the team", ref name))
			    {
			    	team.Name = name;
					treeView1.SelectedNode.Text = name;
			    }
			}
		}

		void ButtonAddPlayerClick(object sender, EventArgs e)
		{
			var teamNode = treeView1.SelectedNode;
			if (teamNode.Tag is LeaguePlayer)
				teamNode = teamNode.Parent;

			if (teamNode.Tag is LeagueTeam team && FormPlayer.ShowDialog() == DialogResult.OK)
			{
				var player = new LeaguePlayer
				{
					Id = FormPlayer.PlayerId,
					Name = FormPlayer.PlayerAlias
				};
				team.Players.Add(player);

				var playerNode = teamNode.Nodes.Add(player.Name);
				playerNode.Tag = player;
				
				treeView1.SelectedNode = playerNode;
			}
		}

		void ButtonDeletePlayerClick(object sender, EventArgs e)
		{
			if (treeView1.SelectedNode.Tag is LeaguePlayer player && 
			    MessageBox.Show("Are you sure you want to delete player " + player.Name + " from this team?",
			                    "Delete Player?", MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				((LeagueTeam)treeView1.SelectedNode.Parent.Tag).Players.Remove(player);
				treeView1.SelectedNode.Remove();
			}
		}

		void ButtonReIdPlayerClick(object sender, EventArgs e)
		{
			if (!(treeView1.SelectedNode.Tag is LeaguePlayer))
			    return;

			var player = (LeaguePlayer)treeView1.SelectedNode.Tag;
			FormPlayer.PlayerId = player.Id;
			FormPlayer.PlayerAlias = player.Name;
			if (FormPlayer.ShowDialog() == DialogResult.OK)
			{
				player.Id = FormPlayer.PlayerId;
				player.Name = FormPlayer.PlayerAlias;
				treeView1.SelectedNode.Text = FormPlayer.PlayerAlias;
			}

			// TODO: Change the player's ID in each of their committed games. This will require a deeper Clone() than currently used, in order to be cancellable.
		}

		void RankCheckedChanged(object sender, EventArgs e)
		{
			foreach (var c in leaguePage.Controls)
				if (c is Label label && label.Text.StartsWith("Points for "))
					label.Enabled = victoryPoints.Checked;

			foreach (var v in victory)
				v.Enabled = victoryPoints.Checked;
			
			if (totalScore.Checked)
				League.VictoryPoints.Clear();

			if (victoryPoints.Checked)
			{
				for (int i = 0; i < victory.Count; i++)
					Force(i, (double)victory[i].Value);
			
			if (victory.Count == 0)
				SetVictoryBox(0);
			}
		}

		/// <summary>Ensure that there is an i'th victory points box, and set its value.</summary>
		void SetVictoryBox(int i, double value = 0)
		{
			while (victory.Count <= i)
			{
				var label = new Label
				{
					Text = "Points for " + (victory.Count + 1).Ordinate(),
					Left = 32,
					Top = 72 + victory.Count * 26,
					Width = 79,
					Parent = leaguePage
				};

				var victoryBox = new NumericUpDown
				{
					Left = 112,
					Top = 70 + victory.Count * 26,
					Width = 60,
					Parent = leaguePage,
					Tag = i,
					Value = 0
				};
				victoryBox.ValueChanged += VictoryPointsChanged;
				victory.Add(victoryBox);
			}
			victory.Last().Value = (decimal)value;
		}

		void VictoryPointsChanged(object sender, EventArgs e)
		{
			NumericUpDown c = (NumericUpDown)sender;
			if (c.Tag is int @int)
				Force(@int, (double)c.Value);
			else
				Force(int.Parse((string)c.Tag), (double)c.Value);
			
			if (victory.Any() && victory.Last().Value > 0)
				SetVictoryBox(victory.Count);
		}

		void Force(int index, double value)
		{
			while (League.VictoryPoints.Count <= index)
				League.VictoryPoints.Add(0);

			League.VictoryPoints[index] = value;

			while (League.VictoryPoints.Any() && League.VictoryPoints.Last() == 0)
				League.VictoryPoints.RemoveAt(League.VictoryPoints.Count - 1);
		}
		
		void RadioButtonHandicapCheckedChanged(object sender, EventArgs e)
		{
			League.HandicapStyle = HandicapExtensions.ToHandicapStyle(((Control)sender).Text);
		}

        private void automaticHandicapEnabled_CheckedChanged(object sender, EventArgs e)
        {
			teamSize.Enabled = automaticHandicapEnabled.Checked;
			missingPlayerPenalty.Enabled = automaticHandicapEnabled.Checked;
			extraAPenalty.Enabled = automaticHandicapEnabled.Checked;
			extraGBonus.Enabled = automaticHandicapEnabled.Checked;

			AAABonus.Enabled = automaticHandicapEnabled.Checked;
			BBBonus.Enabled = automaticHandicapEnabled.Checked;
			ABonus.Enabled = automaticHandicapEnabled.Checked;
			BBonus.Enabled = automaticHandicapEnabled.Checked;
			CBonus.Enabled = automaticHandicapEnabled.Checked;
			DBonus.Enabled = automaticHandicapEnabled.Checked;
			EBonus.Enabled = automaticHandicapEnabled.Checked;
			FBonus.Enabled = automaticHandicapEnabled.Checked;
			GBonus.Enabled = automaticHandicapEnabled.Checked;
			HBonus.Enabled = automaticHandicapEnabled.Checked;
			IBonus.Enabled = automaticHandicapEnabled.Checked;

			AAAPenalty.Enabled = automaticHandicapEnabled.Checked;
			BBPenalty.Enabled = automaticHandicapEnabled.Checked;
			APenalty.Enabled = automaticHandicapEnabled.Checked;
			BPenalty.Enabled = automaticHandicapEnabled.Checked;
			CPenalty.Enabled = automaticHandicapEnabled.Checked;
			DPenalty.Enabled = automaticHandicapEnabled.Checked;
			EPenalty.Enabled = automaticHandicapEnabled.Checked;
			FPenalty.Enabled = automaticHandicapEnabled.Checked;
			GPenalty.Enabled = automaticHandicapEnabled.Checked;
			HPenalty.Enabled = automaticHandicapEnabled.Checked;
			IPenalty.Enabled = automaticHandicapEnabled.Checked;

			AAAPoints.Enabled = automaticHandicapEnabled.Checked;
			BBPoints.Enabled = automaticHandicapEnabled.Checked;
			APoints.Enabled = automaticHandicapEnabled.Checked;
			BPoints.Enabled = automaticHandicapEnabled.Checked;
			CPoints.Enabled = automaticHandicapEnabled.Checked;
			DPoints.Enabled = automaticHandicapEnabled.Checked;
			EPoints.Enabled = automaticHandicapEnabled.Checked;
			FPoints.Enabled = automaticHandicapEnabled.Checked;
			GPoints.Enabled = automaticHandicapEnabled.Checked;
			HPoints.Enabled = automaticHandicapEnabled.Checked;
			IPoints.Enabled = automaticHandicapEnabled.Checked;


		}

		private void UpdatePlayerGrade(LeaguePlayer leaguePlayer)
        {
			Console.WriteLine(leaguePlayer.Name + " " + leaguePlayer.Grade);
			League.Load(League.FileName);
			foreach (LeagueTeam team in League.Teams.ToList())
            {
				int teamIndex = League.Teams.IndexOf(team);
				foreach (LeaguePlayer player in team.Players.ToList())
                {
					int playerIndex = team.Players.IndexOf(player);
					if (player.Id == leaguePlayer.Id)
                    {
						League.Teams[teamIndex].Players[playerIndex] = leaguePlayer;
                    }
                }
            }
			League.Save();
        }

        private void playerGradeBox_SelectedIndexChanged(object sender, EventArgs e)
        {
			string grade = playerGradeBox.SelectedItem.ToString();
			if (treeView1.SelectedNode.Tag is string && treeView1.SelectedNode.Parent.Tag is LeaguePlayer leaguePlayer)
			{
				leaguePlayer.Grade = grade;
				treeView1.SelectedNode.Text = grade;
				UpdatePlayerGrade(leaguePlayer);
			}
			if (treeView1.SelectedNode.Tag is LeaguePlayer player)
			{
				player.Grade = grade;
				treeView1.SelectedNode.Nodes[0].Text = grade;
				UpdatePlayerGrade(player);
			}
        }
    }
}
