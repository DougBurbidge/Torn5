using System;
using System.Drawing;
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

		public FormLeague()
		{
			// The InitializeComponent() call is required for Windows Forms designer support.
			InitializeComponent();
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
		}

		void TreeView1AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (treeView1.SelectedNode.Tag is LeagueTeam)
			{
				listViewScores.Items.Clear();
				foreach (var gameTeam in ((LeagueTeam)treeView1.SelectedNode.Tag).AllGameTeams)
				{
					var item = new ListViewItem(gameTeam.Game.Time.ToString());
					item.SubItems.Add(gameTeam.Score.ToString());
					item.BackColor = gameTeam.Colour.ToColor();
					listViewScores.Items.Add(item);
				}
			}
			else if (treeView1.SelectedNode.Tag is LeaguePlayer)
			{
				listViewScores.Items.Clear();
				foreach (var gamePlayer in ((LeaguePlayer)treeView1.SelectedNode.Tag).Played)
				{
					var item = new ListViewItem(gamePlayer.GameTeam.Game.Time.ToString());
					item.SubItems.Add(gamePlayer.Score.ToString());
					item.BackColor = gamePlayer.Colour.ToColor();
					listViewScores.Items.Add(item);
				}
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
	}
}
