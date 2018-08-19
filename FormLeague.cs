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
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
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
		}

		void ButtonRenameLeagueClick(object sender, EventArgs e)
		{
			League.Title = InputDialog.GetInput("Rename League", "Choose a new name for the league", League.Title);
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
			}
		}

		void ButtonDeleteTeamClick(object sender, EventArgs e)
		{
			if (treeView1.SelectedNode.Tag is LeagueTeam && 
			    MessageBox.Show("Delete Team?", "Are yous sure you want to delete team " + ((LeagueTeam)treeView1.SelectedNode.Tag).Name + "?",
			                    MessageBoxButtons.YesNo) == DialogResult.Yes)
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
			if (treeView1.SelectedNode.Tag is LeaguePlayer)
				treeView1.SelectedNode = treeView1.SelectedNode.Parent;

			if (treeView1.SelectedNode.Tag is LeagueTeam)
			{

			}
		}

		void ButtonDeletePlayerClick(object sender, EventArgs e)
		{
			if (treeView1.SelectedNode.Tag is LeaguePlayer && 
			    MessageBox.Show("Delete Player?", "Are yous sure you want to delete player " + ((LeaguePlayer)treeView1.SelectedNode.Tag).Name + " from this team?",
			                    MessageBoxButtons.YesNo) == DialogResult.Yes)
			{

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
