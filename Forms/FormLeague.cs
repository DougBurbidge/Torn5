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
		
		List<NumericUpDown> victory;  // A list of victory points boxes.

		public FormLeague()
		{
			// The InitializeComponent() call is required for Windows Forms designer support.
			InitializeComponent();
			
			victory = new List<NumericUpDown>();
		}

		void FormLeagueShown(object sender, EventArgs e)
		{
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
					var playerNode = teamNode.Nodes.Add(player.Name);
					playerNode.Tag = player;
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

		void TreeView1AfterSelect(object sender, TreeViewEventArgs e)
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
			
			buttonDeletePlayer.Enabled = treeView1.SelectedNode.Tag is LeaguePlayer;
			buttonReIdPlayer.Enabled = treeView1.SelectedNode.Tag is LeaguePlayer;
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
	}
}
