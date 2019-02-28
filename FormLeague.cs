using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Torn;

namespace Torn.UI
{
	/// <summary>
	/// Description of FormLeague.
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
				var teamNode = new TreeNode(team.Name);
				teamNode.Tag = team;
				foreach (var player in team.Players)
				{
					var playerNode = teamNode.Nodes.Add(player.Name);
					playerNode.Tag = player;
				}
				treeView1.Nodes.Add(teamNode);
			}

			totalScore.Checked = League.VictoryPoints.Count == 0;
			victoryPoints.Checked = League.VictoryPoints.Count > 0;

			for (int i = 0; i < League.VictoryPoints.Count; i++)
				SetVictoryBox(i, League.VictoryPoints[i]);

			RankCheckedChanged(null, null);
		}

		void TreeView1AfterSelect(object sender, TreeViewEventArgs e)
		{
			tabControl1.SelectedTab = scoresPage;

			if (treeView1.SelectedNode.Tag is LeagueTeam)
			{
				listViewScores.Items.Clear();
				foreach (var gameTeam in ((LeagueTeam)treeView1.SelectedNode.Tag).AllPlayed)
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
			else if (treeView1.SelectedNode.Tag is LeaguePlayer)
			{
				listViewScores.Items.Clear();
				foreach (var gamePlayer in ((LeaguePlayer)treeView1.SelectedNode.Tag).Played)
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
				var team = new LeagueTeam();
				team.Name = name;
				League.Teams.Add(team);

				var node = new TreeNode(name);
				node.Tag = team;
				treeView1.Nodes.Add(node);
				treeView1.SelectedNode = node;
			}
		}

		void ButtonDeleteTeamClick(object sender, EventArgs e)
		{
			if (treeView1.SelectedNode.Tag is LeagueTeam && 
			    MessageBox.Show("Are you sure you want to delete team " + ((LeagueTeam)treeView1.SelectedNode.Tag).Name + "?",
			                    "Delete Team?", MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				League.Teams.Remove(((LeagueTeam)treeView1.SelectedNode.Tag));
				treeView1.Nodes.Remove(treeView1.SelectedNode);
			}
		}

		void ButtonRenameTeamClick(object sender, EventArgs e)
		{
			if (treeView1.SelectedNode.Tag is LeagueTeam)
			{
				string name = ((LeagueTeam)treeView1.SelectedNode.Tag).Name;
			    if (InputDialog.UpdateInput("Rename Team", "Choose a new name for the team", ref name))
			    {
			    	((LeagueTeam)treeView1.SelectedNode.Tag).Name = name;
					treeView1.SelectedNode.Text = name;
			    }
			}
		}

		void ButtonAddPlayerClick(object sender, EventArgs e)
		{
			var teamNode = treeView1.SelectedNode;
			if (teamNode.Tag is LeaguePlayer)
				teamNode = teamNode.Parent;

			if (teamNode.Tag is LeagueTeam && FormPlayer.ShowDialog() == DialogResult.OK)
			{
				var player = new LeaguePlayer();
				player.Id = FormPlayer.PlayerId;
				player.Name = FormPlayer.PlayerAlias;
				((LeagueTeam)teamNode.Tag).Players.Add(player);

				var playerNode = teamNode.Nodes.Add(player.Name);
				playerNode.Tag = player;
				
				treeView1.SelectedNode = playerNode;
			}
		}

		void ButtonDeletePlayerClick(object sender, EventArgs e)
		{
			if (treeView1.SelectedNode.Tag is LeaguePlayer && 
			    MessageBox.Show("Are you sure you want to delete player " + ((LeaguePlayer)treeView1.SelectedNode.Tag).Name + " from this team?",
			                    "Delete Player?", MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				((LeagueTeam)treeView1.SelectedNode.Parent.Tag).Players.Remove((LeaguePlayer)treeView1.SelectedNode.Tag);
				treeView1.SelectedNode.Remove();
			}
		}

		void ButtonReIdPlayerClick(object sender, EventArgs e)
		{
			if (!(treeView1.SelectedNode.Tag is LeaguePlayer))
			    return;

			var player = (LeaguePlayer)treeView1.SelectedNode.Tag;
			FormPlayer.PlayerId = player.Id;
			if (FormPlayer.ShowDialog() == DialogResult.OK)
			{
				player.Id = FormPlayer.PlayerId;
				player.Name = FormPlayer.PlayerAlias;
				treeView1.SelectedNode.Text = FormPlayer.PlayerAlias;
			}

		}

		void RankCheckedChanged(object sender, EventArgs e)
		{
			foreach (var c in leaguePage.Controls)
				if (c is Label && ((Label)c).Text.StartsWith("Points for "))
					((Control)c).Enabled = victoryPoints.Checked;

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
				var label = new Label();
				label.Text = "Points for " + (victory.Count + 1).Ordinate();
				label.Left = 32;
				label.Top = 72 + victory.Count * 26;
				label.Width = 79;
				label.Parent = leaguePage;

				var victoryBox = new NumericUpDown();
				victoryBox.Left = 112;
				victoryBox.Top = 70 + victory.Count * 26;
				victoryBox.Width = 60;
				victoryBox.Parent = leaguePage;
				victoryBox.Tag = i;
				victoryBox.Value = 0;
				victoryBox.ValueChanged += victoryPointsChanged;
				victory.Add(victoryBox);
			}
			victory.Last().Value = (decimal)value;
		}

		void victoryPointsChanged(object sender, EventArgs e)
		{
			NumericUpDown c = (NumericUpDown)sender;
			if (c.Tag is int)
				Force((int)c.Tag, (double)c.Value);
			else
				Force(int.Parse((string)c.Tag), (double)c.Value);
			
			if (victory.Count > 0 && victory.Last().Value > 0)
				SetVictoryBox(victory.Count);
		}

		void Force(int index, double value)
		{
			while (League.VictoryPoints.Count <= index)
				League.VictoryPoints.Add(0);

			League.VictoryPoints[index] = value;

			while (League.VictoryPoints.Count > 0 && League.VictoryPoints.Last() == 0)
				League.VictoryPoints.RemoveAt(League.VictoryPoints.Count - 1);
		}
	}
}
