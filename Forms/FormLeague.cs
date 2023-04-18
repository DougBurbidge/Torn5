using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Torn5.Controls;

namespace Torn.UI
{
	/// <summary>
	/// Edit a League.
	/// League property is set by caller, and is passed back to caller for saving if the user clicked OK, or discarding if the user clicked Cancel.
	/// </summary>
	public partial class FormLeague : Form
	{
		public League League { get; set; }
		public FormPlayer FormPlayer { get; set; }

		readonly List<NumericUpDown> victory = new List<NumericUpDown>();  // A list of victory points boxes.
		readonly List<GradeEditor> Grades = new List<GradeEditor>();
		List<PointPercentEditor> PointPercents = new List<PointPercentEditor>();

		public FormLeague()
		{
			// The InitializeComponent() call is required for Windows Forms designer support.
			InitializeComponent();
		}

		bool loading = false;
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

					teamNode.Nodes.Add(playerNode);
				}
				treeView1.Nodes.Add(teamNode);
			}

			totalScore.Checked = League.VictoryPoints.Count == 0;
			victoryPoints.Checked = League.VictoryPoints.Any();
			hitsTieBreak.Checked = League.HitsTieBreak;
			hitsTieBreak.Enabled = victoryPoints.Checked;
			zeroVps.Checked = League.ZeroVps;
			zeroVps.Enabled = victoryPoints.Checked;
			halfVps.Checked = League.HalfVps;
			halfVps.Enabled = victoryPoints.Checked;
			zeroedTieBreak.Enabled = victoryPoints.Checked;
			zeroedTieBreak.Checked = League.ZeroedTieBreak;
			zeroElimed.Checked = League.ZeroElimed;

			for (int i = 0; i < League.VictoryPoints.Count; i++)
				SetVictoryBox(i, League.VictoryPoints[i]);

			labelHighScore.Enabled = victoryPoints.Checked;
			numericHighScore.Enabled = victoryPoints.Checked;
			numericHighScore.Value = (decimal)League.VictoryPointsHighScore;

			sweepBonus.Enabled = victoryPoints.Checked;
			sweepBonus.Value = (decimal)League.SweepBonus;

			radioButtonPercent.Checked = League.HandicapStyle == HandicapStyle.Percent;
			radioButtonPlus.Checked = League.HandicapStyle == HandicapStyle.Plus;
			radioButtonMinus.Checked = League.HandicapStyle == HandicapStyle.Minus;
			radioButtonNone.Checked = League.HandicapStyle == HandicapStyle.None;

			RankCheckedChanged(null, null);

			
			loading = true;
			// Load Grades.
			SetGradeBox(League.Grades.Count);
			for (int grade = 0; grade < League.Grades.Count; grade++)
				Grades[grade].Grade = League.Grades[grade];
			// Load Point Percent
			SetPointPercentBox(League.PointPercents.Count);
			for (int i = 0; i < League.PointPercents.Count; i++)
				PointPercents[i].pointPercent = League.PointPercents[i];
			loading = false;

			teamSize.Value = League.ExpectedTeamSize;
			missingPlayerPenalty.Value = League.MissingPlayerPenalty;
			extraAPenalty.Value = League.ExtraAPenalty;
			extraGBonus.Value = League.ExtraGBonus;
			automaticHandicapEnabled.Checked = League.IsAutoHandicap;
			verbalTermValue.Value = League.VerbalTermValue;
			yellowTermValue.Value = League.YellowTermValue;
			redTermValue.Value = League.RedTermValue;
		}

		private void SetupGradeSelector(string alias, string grade)
        {
			playerGradeAlias.Visible = true;
			playerGradeAlias.Text = alias + " Grade";
			playerGradeBox.Items.Clear();
			foreach (var gradeEditor in Grades)
				playerGradeBox.Items.Add(gradeEditor.GradeName);

			playerGradeBox.Text = grade;
		}

		void TreeView1AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (treeView1.SelectedNode.Tag is string grade && treeView1.SelectedNode.Parent.Tag is LeaguePlayer leaguePlayer)
			{
				temTab.SelectedTab = HandicapPage;
				SetupGradeSelector(leaguePlayer.Name, grade);
			}
			else
			{
				if (treeView1.SelectedNode.Tag is LeagueTeam team)
				{
					temTab.SelectedTab = scoresPage;
					manualTeamCap.Value = Convert.ToDecimal(team?.Handicap?.Value ?? 100);
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
					SetupGradeSelector(player.Name, player.Grade);
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
			manualTeamCap.Visible = treeView1.SelectedNode.Tag is LeagueTeam;
			manualTeamCapLabel.Visible = treeView1.SelectedNode.Tag is LeagueTeam;
		}

		/// <summary>Draw player grade (if any).</summary>
		private void TreeView1DrawNode(object sender, DrawTreeNodeEventArgs e)
		{
			e.DrawDefault = true;
			if (e.Node.Tag is LeaguePlayer lp && !string.IsNullOrEmpty(lp.Grade))
				e.Graphics.DrawString(lp.Grade, e.Node.NodeFont ?? ((TreeView)sender).Font, Brushes.Gray, Math.Max(e.Node.Bounds.Right + 10, 120), e.Node.Bounds.Top);
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

			labelHighScore.Enabled = victoryPoints.Checked;
			numericHighScore.Enabled = victoryPoints.Checked;

			if (totalScore.Checked)
				League.VictoryPoints.Clear();

			if (victoryPoints.Checked)
			{
				for (int i = 0; i < victory.Count; i++)
					Force(i, (double)victory[i].Value);
			
			if (victory.Count == 0)
				SetVictoryBox(0);
			}
			hitsTieBreak.Enabled = victoryPoints.Checked;
			zeroVps.Enabled = victoryPoints.Checked;
			halfVps.Enabled = victoryPoints.Checked;
			zeroedTieBreak.Enabled = victoryPoints.Checked;
			sweepBonus.Enabled = victoryPoints.Checked;
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

		private void NumericHighScore_ValueChanged(object sender, EventArgs e)
		{
			League.VictoryPointsHighScore = (double)numericHighScore.Value;
		}

		void RadioButtonHandicapCheckedChanged(object sender, EventArgs e)
		{
			League.HandicapStyle = HandicapExtensions.ToHandicapStyle(((Control)sender).Text);
		}

        private void automaticHandicapEnabled_CheckedChanged(object sender, EventArgs e)
        {
			League.IsAutoHandicap = automaticHandicapEnabled.Checked;
			teamSize.Enabled = automaticHandicapEnabled.Checked;
			missingPlayerPenalty.Enabled = automaticHandicapEnabled.Checked;
			extraAPenalty.Enabled = automaticHandicapEnabled.Checked;
			extraGBonus.Enabled = automaticHandicapEnabled.Checked;

			foreach (var gradeEditor in Grades)
				gradeEditor.Enabled = automaticHandicapEnabled.Checked;

			if(automaticHandicapEnabled.Checked)
            {
				League.HandicapStyle = HandicapExtensions.ToHandicapStyle("Percent");
				radioButtonPercent.Checked = true;
			}
			radioButtonMinus.Enabled = !automaticHandicapEnabled.Checked;
			radioButtonPlus.Enabled = !automaticHandicapEnabled.Checked;
			radioButtonPercent.Enabled = !automaticHandicapEnabled.Checked;
			radioButtonNone.Enabled = !automaticHandicapEnabled.Checked;
			manualTeamCap.Enabled = !automaticHandicapEnabled.Checked;
			manualTeamCapLabel.Enabled = !automaticHandicapEnabled.Checked;
		}

		private void UpdatePlayerGrade(LeaguePlayer leaguePlayer)
        {
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
        }

		private void manualTeamCap_ValueChanged(object sender, EventArgs e)
		{
			if (treeView1.SelectedNode.Tag is LeagueTeam leagueTeam)
			{
				int index = League.Teams.FindIndex(t => t.TeamId == leagueTeam.TeamId);
				League.Teams[index].Handicap = new Handicap(Decimal.ToDouble(manualTeamCap.Value), League.HandicapStyle);
			}
		}

		private void playerGradeBox_SelectedIndexChanged(object sender, EventArgs e)
        {
			string grade = playerGradeBox.SelectedItem.ToString();
			if (treeView1.SelectedNode.Tag is LeaguePlayer player)
			{
				player.Grade = grade;
				UpdatePlayerGrade(player);
				treeView1.Invalidate();
			}
		}

		/// <summary>Ensure that there is an i'th grade editor.</summary>
		void SetGradeBox(int i)
		{
			while (Grades.Count <= i)
			{
				var gradeBox = new GradeEditor
				{
					Left = 9,
					Top = 38 + Grades.Count * 26,
					Parent = GradesPage
				};

				gradeBox.ValueChanged += GradeValueChanged;
				Grades.Add(gradeBox);
			}
		}

		/// <summary>Reload League.Grades from form controls.</summary>
		private void GradeValueChanged(object sender, EventArgs e)
		{
			if (loading)
				return;

			League.Grades.Clear();
			foreach (var grade in Grades)
				if (grade.HasValue())
					League.Grades.Add(grade.Grade);

			if (Grades.Any() && Grades.Last().HasValue())
				SetGradeBox(Grades.Count);
		}

		private void teamSize_ValueChanged(object sender, EventArgs e)
		{
			League.ExpectedTeamSize = (int)teamSize.Value;
		}

		private void missingPlayerPenalty_ValueChanged(object sender, EventArgs e)
		{
			League.MissingPlayerPenalty = (int)missingPlayerPenalty.Value;
		}

		private void extraAPenalty_ValueChanged(object sender, EventArgs e)
		{
			League.ExtraAPenalty = (int)extraAPenalty.Value;
		}

		private void extraGBonus_ValueChanged(object sender, EventArgs e)
		{
			League.ExtraGBonus = (int)extraGBonus.Value;
		}

        private void hitsTieBreak_CheckedChanged(object sender, EventArgs e)
        {
			League.HitsTieBreak = hitsTieBreak.Checked;
			if(hitsTieBreak.Checked)
            {
				zeroedTieBreak.Checked = false;
            }
        }

		void SetPointPercentBox(int i)
		{
			while (PointPercents.Count <= i)
			{
				var pointPercent = new PointPercentEditor
				{
					Left = 20,
					Top = 60 + PointPercents.Count * 26,
					Parent = pointPercentBox
				};

				pointPercent.ValueChanged += PointPercentChanged;
				PointPercents.Add(pointPercent);
			}
		}

		private void PointPercentChanged(object sender, EventArgs e)
		{
			if(loading)
				return;

			League.PointPercents.Clear();
			foreach (var cap in PointPercents)
				if (cap.HasValue())
					League.PointPercents.Add(cap.pointPercent);

			if (PointPercents.Any() && PointPercents.Last().HasValue())
				SetPointPercentBox(PointPercents.Count);
		}

		private void loadPresetPoints(List<PointPercent> presetPoints)
        {
			SetPointPercentBox(presetPoints.Count);
			for (int i = 0; i < presetPoints.Count; i++)
				PointPercents[i].pointPercent = presetPoints[i];
			for (int i = presetPoints.Count; i < PointPercents.Count; i++)
				PointPercents[i].pointPercent = new PointPercent(0, 0);
		}

        private void waLeaguePoints_Click(object sender, EventArgs e)
		{
			loadPresetPoints(League.WA_LEAGUE_POINTS);
		}

        private void waDoubles_Click(object sender, EventArgs e)
        {
			loadPresetPoints(League.WA_DOUBLES_POINTS);

		}

        private void waTriples_Click(object sender, EventArgs e)
        {
			loadPresetPoints(League.WA_TRIPLES_POINTS);
		}

        private void verbalTermValue_ValueChanged(object sender, EventArgs e)
        {
			League.VerbalTermValue = verbalTermValue.Value;
        }

        private void yellowTermValue_ValueChanged(object sender, EventArgs e)
        {
			League.YellowTermValue = yellowTermValue.Value;

		}

		private void redTermValue_ValueChanged(object sender, EventArgs e)
        {
			League.RedTermValue = redTermValue.Value;
        }

        private void zeroElimed_CheckedChanged(object sender, EventArgs e)
        {
			League.ZeroElimed = zeroElimed.Checked;
		}

        private void zeroVps_CheckedChanged(object sender, EventArgs e)
        {
			League.ZeroVps = zeroVps.Checked;
			if(zeroVps.Checked)
				halfVps.Checked = false;
		}

        private void sweepBonus_ValueChanged(object sender, EventArgs e)
        {
			League.SweepBonus = (int)sweepBonus.Value;
		}

        private void zeroedTieBreak_CheckedChanged(object sender, EventArgs e)
        {
			League.ZeroedTieBreak = zeroedTieBreak.Checked;
			if (zeroedTieBreak.Checked)
			{
				hitsTieBreak.Checked = false;
			}
		}

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void halfElimed_CheckedChanged(object sender, EventArgs e)
        {
			League.HalfVps = halfVps.Checked;
			if(halfVps.Checked)
				zeroVps.Checked = false;
		}
    }
}
