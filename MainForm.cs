using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Torn;
using Torn.Report;
using Torn.UI;
using Zoom;

/*
Implemented: load league, save league, delete game from league
read from P&C server, web server, report to HTML, upload to FTP,
transfer to team boxes via drag-drop, transfer to team boxes on game selection, set dimensions, persistence
show team name, scores and rank in team boxes
right-click remember team, identify team, identify player, handicap team
edit league -- team add/delete/rename, player add/delete/re-ID; implement OK/Cancel
better save format
read from demo "server"
read from laserforce server

TODO for BOTH:
output to screen and printer
send to scoreboard (web browser)
on commit auto-update scoreboard
right-click handicap player, merge player

TODO for LEAGUE:
handicap, on commit auto-update team handicaps
set up fixtures

TODO for ZLTAC:
set up pyramid round
recalculate scores on Helios

OTHER:
league copy from
Space Marines match play
read from Ozone server
spark lines
check latest version via REST
reports and uploads in worker thread
sanity check report. Add new check: are there odd games out with no victory points?
tech report: hit totals for all sensors on all packs, plus games where a sensor takes 0 hits
option to zero eliminated players.
Move global settings to Program.cs

NEEDS TESTING:
adjust team score/victory points
remember all teams
upload to http, https, ftp
If we don't find a settings file in the user folder, check for one in the exe folder.
group players by LotR
on commit auto-update teams
*/

namespace Torn.UI
{
	/// <summary>Torn 5 Main Form.</summary>
	public partial class MainForm : Form
	{
		GroupPlayersBy groupPlayersBy = GroupPlayersBy.Colour;
		//SortTeamsBy sortTeamsBy;
		bool autoUpdateScoreboard = true;
		bool autoUpdateTeams = true;

		SystemType systemType;
		bool windowsAuth;
		string sqlUserId;
		string sqlPassword;

		string serverAddress = "localhost";

		int webPort;
		WebOutput webOutput;
		LaserGameServer laserGameServer;
		List<ServerGame> serverGames;
		static Holders leagues;
		Holder activeHolder;  // This is the league selected in the listView.
		string selectedNode; // Only used in MainFormShown() and LoadSettings().
		string exportFolder;

		string uploadMethod;
		string uploadSite;
		string username;
		string password;

		string logFolder;

		PlayersBox playersBox;
		
		FormPlayer formPlayer;

		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
		}

		void MainFormShown(object sender, EventArgs e)
		{
			leagues = new Holders();
			webPort = 8080;
			systemType = SystemType.Demo;

			LoadSettings();

			toolStripReports.Location = new Point(0, 1);
			toolStripTeams.Location = new Point(0, 1);
			toolStripGame.Location = new Point(0, 1);
			toolStripLeague.Location = new Point(0, 1);

			webOutput = new WebOutput(webPort);
			webOutput.Leagues = leagues;
			webOutput.Elapsed = Elapsed;

			playersBox = new PlayersBox();
			playersBox.Images = imageListPacks;
			playersBox.GetMoveTarget = FindEmptyTeamBox;
			tableLayoutPanel1.Controls.Add(playersBox, 2, 0);
			tableLayoutPanel1.SetRowSpan(playersBox, tableLayoutPanel1.RowCount);
			playersBox.Dock = DockStyle.Fill;
			
			formPlayer = new FormPlayer();
			formPlayer.Icon = (Icon)Icon.Clone();
			ConnectLaserGameServer();

			AddTeamBoxes();

			listViewLeagues.Focus();

			foreach (ListViewItem item in listViewLeagues.Items)
				if (item.Text == selectedNode)
					item.Selected = true;

			if (listViewLeagues.Items.Count == 0)
				ListViewLeaguesItemSelectionChanged(null, null);
			else if (listViewLeagues.SelectedIndices.Count == 0)
				listViewLeagues.SelectedIndices.Add(0);
		}

		void ConnectLaserGameServer()
		{
			if (laserGameServer != null)
				laserGameServer.Dispose();

			try
			{
				switch (systemType) {
					case SystemType.Laserforce:
						laserGameServer = new Laserforce();
						if (windowsAuth)
							((Laserforce)laserGameServer).Connect(serverAddress);
						else
							((Laserforce)laserGameServer).Connect(serverAddress, sqlUserId, sqlPassword);
						((Laserforce)laserGameServer).LogFolder = logFolder;
					break;
					case SystemType.Nexus: laserGameServer = new PAndCNexusWithIButton(serverAddress); break;
					case SystemType.Zeon: laserGameServer = new PAndC(serverAddress);  break;
					case SystemType.Demo: laserGameServer = new DemoServer();  break;
				}

				formPlayer.LaserGameServer = laserGameServer;
				webOutput.Games = laserGameServer.GetGames;
				webOutput.PopulateGame = laserGameServer.PopulateGame;
				webOutput.Players = laserGameServer.GetPlayers;
			}
			catch (Exception ex)
			{
				string s = ex.ToString();
				MessageBox.Show("Error while connecting to lasergame database server. Please check your settings.\n\n" + s.Substring(0, s.IndexOf('\n')));
			}

			timeToNextCheck = TimeSpan.FromSeconds(0);
			TimerGameTick(null, null);
			RefreshGamesList();
		}

		void MainFormFormClosing(object sender, FormClosingEventArgs e)
		{
			try
			{
				webOutput.Dispose();
				if (laserGameServer != null)
					laserGameServer.Dispose();
	
				SaveSettings();
			}
			catch (Exception ex)
			{
				string s = ex.ToString();
				MessageBox.Show("Error while exiting application.\n\n" + s.Substring(0, s.IndexOf('\n')));
			}
		}

		Holder AddLeague(string fileName, string key = "", bool neww = false)
		{
			League league = new League(fileName);
			ListViewItem item = new ListViewItem();
			if (!neww)
				item.ImageKey = "tick";
			try
			{
				if (!neww)
					league.Load(fileName);
				item.Text = key;
			}
			catch (Exception ex)
			{
				item.ImageKey = "cross";
				item.Text = ex.Message + " -- " + ex.StackTrace;
			}

			if (string.IsNullOrEmpty(key))
				key = "league" + leagues.Count.ToString("D2", CultureInfo.InvariantCulture);

			Holder holder = new Holder(key, fileName, league);
			leagues.Add(holder);

			item.Tag = holder;
			item.SubItems.AddRange(new string[] { league.Title, fileName, league.AllGames.Count.ToString(CultureInfo.CurrentCulture), league.Teams.Count.ToString(CultureInfo.CurrentCulture)});
			listViewLeagues.Items.Add(item);
			listViewLeagues.FocusedItem = item;

			if (neww)
				league.Save();

			return holder;
		}

		void SetRowSpans(int adjust)
		{
			try
			{
				tableLayoutPanel1.SetCellPosition(panelLeague, new TableLayoutPanelCellPosition(0, (tableLayoutPanel1.RowCount + adjust) / 2));
				tableLayoutPanel1.SetRowSpan(listViewLeagues, (tableLayoutPanel1.RowCount + adjust) / 2);
				tableLayoutPanel1.SetRowSpan(panelLeague, (tableLayoutPanel1.RowCount + adjust + 1) / 2);
				tableLayoutPanel1.SetRowSpan(panelGames, tableLayoutPanel1.RowCount + adjust);
				tableLayoutPanel1.SetRowSpan(playersBox, tableLayoutPanel1.RowCount + adjust);
			}
			catch
			{
			}		
		}

		void EnableRemoveRowColumnButtons()
		{
			buttonRemoveColumn.Enabled = tableLayoutPanel1.ColumnCount > 4;
			buttonRemoveRow.Enabled = tableLayoutPanel1.RowCount > 2;
		}

		void AddTeamBoxes()
		{
			while(tableLayoutPanel1.Controls.Count - 4 < tableLayoutPanel1.RowCount * (tableLayoutPanel1.ColumnCount - 3))
			{
				TeamBox teamBox = new TeamBox();
				teamBox.League = activeHolder == null ? null : activeHolder.League;
				teamBox.Images = imageListPacks;
				teamBox.GetMoveTarget = FindEmptyTeamBox;
				teamBox.RankTeams = RankTeamBoxes;
				teamBox.SortTeamsByRank = ArrangeTeamsByRank;
				teamBox.FormPlayer = formPlayer; 
				tableLayoutPanel1.Controls.Add(teamBox);
				teamBox.Dock = DockStyle.Fill;
			}
			EnableRemoveRowColumnButtons();
		}

		void ButtonAboutClick(object sender, EventArgs e)
		{
			MessageBox.Show("A tournament scores editor by Doug Burbidge.\n\nhttp://www.dougburbidge.com/Apps/\n\nhttps://github.com/DougBurbidge/Torn5/", "Torn 5");
		}

		void ButtonAddRowClick(object sender, EventArgs e)
		{
			tableLayoutPanel1.RowCount++;
			
			SetRowSpans(0);
 
			if (tableLayoutPanel1.RowStyles.Count < tableLayoutPanel1.RowCount)
				tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent));
			foreach (RowStyle rowStyle in tableLayoutPanel1.RowStyles)
				rowStyle.Height = 100 / tableLayoutPanel1.RowCount;

			AddTeamBoxes();
		}

		void ButtonRemoveRowClick(object sender, EventArgs e)
		{
			if (tableLayoutPanel1.RowCount > 2)
			{
				for (int i = 0; i < tableLayoutPanel1.ColumnCount - 3; i++)
					tableLayoutPanel1.Controls.RemoveAt(tableLayoutPanel1.Controls.Count - 1);

				SetRowSpans(-1);
				tableLayoutPanel1.RowCount--;
				EnableRemoveRowColumnButtons();
			}
		}

		void ButtonAddColumnClick(object sender, EventArgs e)
		{
			if (tableLayoutPanel1.ColumnStyles.Count < ++tableLayoutPanel1.ColumnCount)
				tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent));
			foreach (ColumnStyle columnStyle in tableLayoutPanel1.ColumnStyles)
				columnStyle.Width = 100 / tableLayoutPanel1.ColumnCount;

			AddTeamBoxes();
		}

		void ButtonRemoveColumnClick(object sender, EventArgs e)
		{
			if (tableLayoutPanel1.ColumnCount > 4)
			{
				for (int i = 0; i < tableLayoutPanel1.RowCount; i++)
					tableLayoutPanel1.Controls.RemoveAt(tableLayoutPanel1.Controls.Count - 1);

				tableLayoutPanel1.ColumnCount--;
				EnableRemoveRowColumnButtons();
			}
		}

		void ButtonCloseClick(object sender, EventArgs e)
		{
			foreach (ListViewItem item in listViewLeagues.SelectedItems)
			{
				leagues.Remove((Holder)(item.Tag));
				listViewLeagues.Items.Remove(item);
			}
		}

		void ButtonCommitClick(object sender, EventArgs e)
		{
			foreach (ListViewItem item in listViewGames.SelectedItems)
			{
				ServerGame serverGame = item.Tag is ServerGame ? ((ServerGame)item.Tag) : null;

				var teamDatas = new List<GameTeamData>();
				var teamBoxes = TeamBoxes();
				// Build a list, one TeamData per TeamBox, connecting each GameTeam to its ServerPlayers.
				foreach (TeamBox teamBox in teamBoxes)
					if (teamBox.Players().Any())
					{
						var teamData = new GameTeamData();
						teamData.GameTeam = teamBox.GameTeam;
						teamData.Players = teamBox.Players();
						teamDatas.Add(teamData);
					}

				if (teamDatas.Any())
				{
					activeHolder.League.CommitGame(serverGame, teamDatas, groupPlayersBy);

					foreach (TeamBox teamBox in teamBoxes)
					{
						var teamData = teamDatas.Find(x => x.GameTeam != null && x.GameTeam.TeamId == teamBox.GameTeam.TeamId);
						if (teamData != null)
							teamBox.GameTeam = teamData.GameTeam;

						if (autoUpdateTeams)
						{
							var lt = activeHolder.League.LeagueTeam(teamBox.GameTeam);

							if (lt != null && teamBox.Players().Any())
								lt.Handicap = teamBox.Handicap;
						}
					}

					RefreshGamesList();
					RankTeams();
					listViewGames.Focus();
//					if (autoUpdateScoreboard)
//						UpdateScoreboard();
				}
			}
		}

		void ButtonEditLeagueClick(object sender, EventArgs e)
		{
			if (activeHolder == null)
			{
				MessageBox.Show("Please select a league.");
				return;
			}

			var form = new FormLeague();
			form.Icon = (Icon)Icon.Clone();
			form.League = activeHolder.League.Clone();
			form.FormPlayer = formPlayer;

			if (form.League.Teams.Count == 0)
				foreach (var ft in activeHolder.Fixture.Teams)
				{
					var lt = new LeagueTeam();
					lt.Name = ft.Name;
					ft.LeagueTeam = lt;
					form.League.AddTeam(lt);
				}

			if (form.ShowDialog() == DialogResult.OK)
			{
				activeHolder.League = form.League;
				activeHolder.League.Save();
				foreach (TeamBox teamBox in TeamBoxes())
					teamBox.League = activeHolder.League;
			}
		}

		void ButtonEditReportsClick(object sender, EventArgs e)
		{
			if (listViewLeagues.SelectedItems.Count > 0)
			{
				var item = listViewLeagues.SelectedItems[0];
				using (var form = new FormReports((Holder)item.Tag))
				{
					form.Icon = (Icon)Icon.Clone();
					form.ShowDialog();
				}
			}
		}

		void ButtonExportClick(object sender, EventArgs e)
		{
			if (GetExportFolder() != null)
			{
				progressBar1.Value  = 0;
				try {
					ExportPages.ExportReports(exportFolder, IncludeSecret(), SelectedLeagues(), ProgressBar);
				}
				finally { progressBar1.Visible = false; labelStatus.Text = ""; }
			}
		}

		void ButtonExportFixturesClick(object sender, EventArgs e)
		{
			if (GetExportFolder() != null)
				ExportPages.ExportFixtures(exportFolder, SelectedLeagues());
		}

		void ButtonFixtureClick(object sender, EventArgs e)
		{
			if (listViewLeagues.SelectedItems.Count == 1)
			{
				var item = listViewLeagues.SelectedItems[0];
				using (var form = new FormFixture(((Holder)item.Tag).Fixture, ((Holder)item.Tag).League))
				{
					form.Icon = (Icon)Icon.Clone();
					form.ShowDialog();
				}
			}
		}

		void ButtonForceClick(object sender, EventArgs e) {}

		void ButtonForgetClick(object sender, EventArgs e)
		{
			if (listViewGames.SelectedItems.Count == 0)
				return;

			var changed = new List<League>();

			foreach (ListViewItem item in listViewGames.SelectedItems)
				if (item.Tag is ServerGame && ((ServerGame)item.Tag).Game != null)
				{
					Game game = ((ServerGame)item.Tag).Game;
					var holder = leagues.Find(h => h.League.AllGames.Contains(game));
					if (holder != null)
					{
						holder.League.AllGames.Remove(game);
						item.SubItems[1].Text = null;
						if (!changed.Contains(holder.League))
						    changed.Add(holder.League);
					}
					((ServerGame)item.Tag).Game = null;
				}

			foreach (var league in changed)
				league.Save();

			playersBox.Clear();
		}

		void ButtonHelpClick(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start("http://dougburbidge.com/Apps/help.html");
		}

		void ButtonLatestGameClick(object sender, EventArgs e)
		{
			RefreshGamesList();
			listViewGames.SelectedItems.Clear();
			if (listViewGames.Items.Count > 0)
			{
				var index = listViewGames.Items.Count - 1;
				while (index > 0 && !((ServerGame)listViewGames.Items[index].Tag).OnServer)
					index--;
				listViewGames.SelectedIndices.Add(index);
				listViewGames.FocusedItem = listViewGames.Items[index];
				listViewGames.TopItem = listViewGames.Items[index];
			}
		}

		void ButtonLoadClick(object sender, EventArgs e)
		{
			if (openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				foreach (string fileName in openFileDialog1.FileNames)
					AddLeague(fileName);

				RefreshGamesList();
			}
		}

		FormReport packReport; 
		void ButtonPackReportClick(object sender, EventArgs e)
		{
			if (GetExportFolder() != null)
			{
				var leagues = new List<League>();

				foreach (ListViewItem item in listViewLeagues.SelectedItems)
					leagues.Add(((Holder)item.Tag).League);

				if (packReport == null)
				{
					packReport = new FormReport();
					packReport.ReportTemplate = new ReportTemplate();
					packReport.ReportTemplate.ReportType = ReportType.Packs;
					packReport.ReportTemplate.Title = "Pack report for " + string.Join(", ", leagues);
					packReport.ReportTemplate.Settings.Add("ChartType=kernel density estimate with rug");
					packReport.ReportTemplate.Settings.Add("Description");
					packReport.ReportTemplate.Settings.Add("Longitudinal");
				}

				if (packReport.ShowDialog() == DialogResult.OK)
				{
					Cursor.Current = Cursors.WaitCursor;
					try
					{
						ExportPages.PackReport(exportFolder, leagues, packReport.ReportTemplate, ((Holder)listViewLeagues.SelectedItems[0].Tag).ReportTemplates.OutputFormat);
					}
					finally
					{
						Cursor.Current = Cursors.Default;
					}
				}
			}
		}

		void ButtonPreferencesClick(object sender, EventArgs e)
		{
			var form = new FormPreferences();
			form.Icon = (Icon)Icon.Clone();
			form.GroupPlayersBy = groupPlayersBy;
			form.AutoUpdateScoreboard = autoUpdateScoreboard;
			form.AutoUpdateTeams = autoUpdateTeams;
			form.UploadMethod = uploadMethod;
			form.UploadSite = uploadSite;
			form.Username = username;
			form.Password = password;

			form.SystemType = systemType;
			form.ServerAddress = serverAddress;
			form.WindowsAuth = windowsAuth;
			form.Sqluser = sqlUserId;
			form.Password = sqlPassword;
			form.WebPort = webPort;
			form.LogFolder = logFolder;

			if (form.ShowDialog() == DialogResult.OK)
			{
				groupPlayersBy = form.GroupPlayersBy;
				autoUpdateScoreboard = form.AutoUpdateScoreboard;
				autoUpdateTeams = form.AutoUpdateTeams;
				uploadMethod = form.UploadMethod;
				uploadSite = form.UploadSite;
				username = form.Username;
				password = form.Password;

				systemType = form.SystemType;
				serverAddress = form.ServerAddress;
				windowsAuth = form.WindowsAuth;
				sqlUserId = form.Sqluser;
				sqlPassword = form.SqlPassword;
				logFolder = form.LogFolder;
				
				ConnectLaserGameServer();
				ListViewLeaguesItemSelectionChanged(null, null);
				timeToNextCheck = TimeSpan.FromSeconds(1);
				ButtonLatestGameClick(null,null);

				webPort = form.WebPort;
				if (webOutput != null)
					webOutput.Restart(webPort);
			}
		}

		void ButtonNewClick(object sender, EventArgs e)
		{
			if (saveFileDialog1.ShowDialog() == DialogResult.OK)
			{
				foreach (string fileName in saveFileDialog1.FileNames)
					AddLeague(fileName, "", true);

				RefreshGamesList();
			}			
		}

		void ButtonRememberAllTeamsClick(object sender, EventArgs e)
		{
			foreach (var teamBox in TeamBoxes())
				if (teamBox.LeagueTeam == null)
					teamBox.RememberTeam();
		}

		void ButtonSaveClick(object sender, EventArgs e)
		{
			foreach (ListViewItem item in listViewLeagues.SelectedItems)
				((Holder)item.Tag).League.Save();
		}

		void ButtonSetDescriptionClick(object sender, EventArgs e)
		{
			if (listViewGames.SelectedItems.Count == 0)
				return;

			var changed = new List<League>();

			var firstItem = listViewGames.SelectedItems[0];
			while(firstItem.SubItems.Count <= 2) firstItem.SubItems.Add("");

			var id = new InputDialog("Description: ", "Set a description", firstItem.SubItems[2].Text);
			if (id.ShowDialog() == DialogResult.OK)
				foreach (ListViewItem item in listViewGames.SelectedItems)
					if (item.Tag is ServerGame && ((ServerGame)item.Tag).Game != null)
					{
						var serverGame = (ServerGame)item.Tag;
						serverGame.Game.Title = id.Response;
						serverGame.Game.Reported = false;
						while (item.SubItems.Count <= 2)
							item.SubItems.Add("");
						item.SubItems[2].Text = id.Response;
						if (!changed.Contains(serverGame.League))
						    changed.Add(serverGame.League);
					}

			foreach (var league in changed)
				league.Save();
		}

		void ButtonSetExportFolderClick(object sender, EventArgs e)
		{
			GetExportFolder("Select a root folder for bulk export of league reports.", true);
		}

		void ButtonUploadClick(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(uploadMethod) || string.IsNullOrEmpty(uploadSite) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
			{
				MessageBox.Show("Please fill in details under Leagues > Preferences > Upload.", "Upload Details Required");
				return;
			}

			if (GetExportFolder() != null)
			{
				progressBar1.Value = 0;
				try {
					ExportPages.UploadFiles(uploadMethod, uploadSite, username, password, exportFolder, IncludeSecret(), SelectedLeagues(), ProgressBar);
				}
				finally { progressBar1.Visible = false; labelStatus.Text = ""; }
			}
		}

		string GetExportFolder(string message = "", bool showDialog = false)
		{
			if (!showDialog && !string.IsNullOrEmpty(exportFolder))
				return exportFolder;

			if (!string.IsNullOrEmpty(message))
				folderBrowserDialog1.Description = message;
			else if (listViewLeagues.SelectedItems.Count == 1)
				folderBrowserDialog1.Description = "Select a root folder for bulk export of " + listViewLeagues.SelectedItems[0].SubItems[1].Text;
			else
				folderBrowserDialog1.Description = "Select a root folder for bulk export of the " + listViewLeagues.SelectedItems.Count.ToString(CultureInfo.CurrentCulture) + " selected leagues.";

			folderBrowserDialog1.SelectedPath = exportFolder;

			if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
				return exportFolder = folderBrowserDialog1.SelectedPath;
			else
				return null;
		}

		void SetRowColumnCount(int rows, int columns)
		{
			for (int i = rows * columns + 3; i < tableLayoutPanel1.Controls.Count; )
				try
				{
					tableLayoutPanel1.Controls.RemoveAt(tableLayoutPanel1.Controls.Count - 1);
				}
				catch
				{
				}

			if (rows < tableLayoutPanel1.RowCount)
				SetRowSpans(rows - tableLayoutPanel1.RowCount);

			try {
				tableLayoutPanel1.RowCount = Math.Max(rows, 2);
				tableLayoutPanel1.ColumnCount = Math.Max(columns + 3, 4);
				
				SetRowSpans(0);
	 
				while (tableLayoutPanel1.RowStyles.Count < rows)
					tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent));
				foreach (RowStyle rowStyle in tableLayoutPanel1.RowStyles)
					rowStyle.Height = 100 / tableLayoutPanel1.RowCount;

				while (tableLayoutPanel1.ColumnStyles.Count < columns + 3)
					tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent));
				foreach (ColumnStyle columnStyle in tableLayoutPanel1.ColumnStyles)
					columnStyle.Width = 100 / tableLayoutPanel1.ColumnCount;

				AddTeamBoxes();
				EnableRemoveRowColumnButtons();
			} catch {}
		}

		bool IncludeSecret()
		{
			return (ModifierKeys.HasFlag(Keys.Control) && ModifierKeys.HasFlag(Keys.Shift)) ||
				(ModifierKeys.HasFlag(Keys.Control) && ModifierKeys.HasFlag(Keys.Alt)) ||
				(ModifierKeys.HasFlag(Keys.Shift) && ModifierKeys.HasFlag(Keys.Alt));
		}

		List<Holder> SelectedLeagues()
		{
			List<Holder> selected = new List<Holder>();

			foreach (ListViewItem item in listViewLeagues.SelectedItems)
				selected.Add((Holder)item.Tag);

			return selected;
		}

		void ProgressBar(double progress, string status = "")
		{
			progressBar1.Visible = true;
			progressBar1.Value = (int)(progress * 1000);
			labelStatus.Text = status;
			Application.DoEvents();
		}

		TeamBox FindEmptyTeamBox()
		{
			foreach (Control c in tableLayoutPanel1.Controls)
				if (c is TeamBox && ((TeamBox)c).Items.Count == 0)
					return (TeamBox)c;

			return null;
		}

		List<TeamBox> TeamBoxes()
		{
			var teamBoxes = new List<TeamBox>();
			foreach (Control c in tableLayoutPanel1.Controls)
				if (c is TeamBox)
					teamBoxes.Add((TeamBox)c);

			return teamBoxes;
		}

		void TransferPlayers(ServerGame serverGame)
		{
			League league = serverGame.League ?? (listViewLeagues.SelectedItems.Count == 1 ? ((Holder)listViewLeagues.SelectedItems[0].Tag).League : null);
			if (league == null)
				return;

			var teamBoxes = TeamBoxes();

			int box = 0;

			if (serverGame.Game == null)  // This game is not yet committed. Match players to league teams.
			{
				if (groupPlayersBy == GroupPlayersBy.Colour)
					foreach (var colour in (Colour[])Enum.GetValues(typeof(Colour)))
					{
						var serverPlayers = playersBox.Players().FindAll(p => p.Colour == colour).ToList();
						if (serverPlayers.Any() && box < teamBoxes.Count)
							teamBoxes[box++].Accept(serverPlayers);
					}
				else  // Alias or LotR
					foreach (var team in league.Teams)
					{
						var serverPlayers = playersBox.Players().FindAll(p => team.Players.Exists(p2 => p.PlayerId == p2.Id)).ToList();
						if (serverPlayers.Any() && box < teamBoxes.Count)
							teamBoxes[box++].Accept(serverPlayers);
					}
			}
			else  // This game is previously committed. Match game players to game teams.
			{
				var leagueGame = serverGame.Game;
				foreach (var gameTeam in leagueGame.Teams)
				{
					var serverPlayers = new List<ServerPlayer>();
					foreach (var gp in gameTeam.Players)
					{
						var players = playersBox.Players();
						var serverPlayer = players.Find(sp => (!string.IsNullOrEmpty(sp.PlayerId) && sp.PlayerId == gp.PlayerId) ||
						                                      (!string.IsNullOrEmpty(sp.ServerPlayerId) && gp is ServerPlayer && sp.ServerPlayerId == ((ServerPlayer)gp).ServerPlayerId) ||
						                                      (!string.IsNullOrEmpty(sp.Pack) && sp.Pack == gp.Pack));
						if (serverPlayer != null)
						{
							serverPlayers.Add(serverPlayer);

							serverPlayer.PlayerId = gp.PlayerId;
							serverPlayer.Item.SubItems[1].Text = league.Alias(gp);
						}
					}

					if (serverPlayers.Any() && box < teamBoxes.Count)
					{
						teamBoxes[box].LeagueTeam = league.LeagueTeam(gameTeam);
						teamBoxes[box].Accept(serverPlayers);
						box++;
					}
				}
			}

			RankTeamBoxes();
			ArrangeTeamsByRank();
		}

		bool EnableCommit()
		{
			return listViewLeagues.SelectedItems.Count == 1 &&
				listViewGames.SelectedItems.Count == 1 &&
				!((ServerGame)listViewGames.SelectedItems[0].Tag).InProgress;
		}

		void ListViewGamesSelectedIndexChanged(object sender, EventArgs e)
		{
			buttonSetDescription.Enabled = listViewGames.SelectedItems.Count > 0;
			buttonForget.Enabled = listViewGames.SelectedItems.Count > 0;
			buttonCommit.Enabled = EnableCommit();
			
			foreach (var tb in TeamBoxes())
				tb.Clear();

			if (listViewGames.SelectedItems.Count == 1)
			{
				ServerGame game = ((ServerGame)listViewGames.SelectedItems[0].Tag);

				if (laserGameServer != null)
					laserGameServer.PopulateGame(game);

				if (activeHolder != null && activeHolder.League != null)
					playersBox.LoadGame(activeHolder.League, game);
				else
					playersBox.LoadGame(null, game);

				TransferPlayers(game);
			}
			else
				playersBox.Clear();
		}

		void ListViewLeaguesAfterLabelEdit(object sender, LabelEditEventArgs e)
		{
			Holder holder = (Holder)listViewLeagues.Items[e.Item].Tag;
			string newKey = e.Label;

			if (holder != null && !string.IsNullOrEmpty(newKey))
		    {
				//listViewLeagues.Items[e.Item].Tag = newKey;
				holder.Key = newKey;
		    }
		}

		void ListViewLeaguesItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			Boolean b = listViewLeagues.SelectedItems.Count > 0;
			string s = listViewLeagues.SelectedItems.Count > 1 ? "s" : "";

			buttonClose.Enabled = b;
			buttonSave.Enabled = b;
			buttonExportReports.Enabled = b;
			buttonUploadReports.Enabled = b;
			buttonPackReport.Enabled = b;
			buttonConfigureReports.Enabled = listViewLeagues.SelectedItems.Count == 1;

			if (listViewLeagues.Items.Count == 0)
			{
				labelLeagueDetails.Text = systemType == SystemType.Demo ? "\nClick Leagues » Preferences to choose lasergame server type.\n" : "";
				labelLeagueDetails.Text += "\nClick New to create a new league file,\nor Open to open an existing one."; 
			}
			else if (listViewLeagues.SelectedItems.Count == 1 && e != null)
			{
				activeHolder = (Holder)e.Item.Tag;
				labelLeagueDetails.Text = "Title: " + activeHolder.League.Title + "\nKey: " + activeHolder.Key + 
					"\nFile name: " + activeHolder.FileName +
					"\nGames: " + activeHolder.League.AllGames.Count.ToString(CultureInfo.CurrentCulture) + 
					"\nTeams: " + activeHolder.League.Teams.Count.ToString(CultureInfo.CurrentCulture);
				SetRowColumnCount(activeHolder.League.GridHigh, activeHolder.League.GridWide);
			}
			else
			{
				activeHolder = null;
				var sb = new StringBuilder();
				sb.Append(listViewLeagues.SelectedItems.Count.ToString(CultureInfo.CurrentCulture));
				          sb.Append(" leagues selected:\n");
				foreach (ListViewItem item in listViewLeagues.SelectedItems)
					sb.Append(item.SubItems[1].Text + ",\n");
				sb.Length -= 2;
				labelLeagueDetails.Text = sb.ToString();
			}

			foreach (var tb in TeamBoxes())
				tb.League = activeHolder == null ? null : activeHolder.League;
		}

		TimeSpan timeToNextCheck = TimeSpan.FromSeconds(5);
		TimeSpan timeElapsed = TimeSpan.FromMilliseconds(-1);
		bool gameInProgress = false;

		void TimerGameTick(object sender, EventArgs e)
		{
			if (timeToNextCheck <= TimeSpan.Zero)
			{
				timeElapsed = laserGameServer == null ? TimeSpan.Zero : laserGameServer.GameTimeElapsed();  // This queries the database server.

				if (timeElapsed > TimeSpan.FromSeconds(1))
					timeToNextCheck = TimeSpan.FromSeconds(61 - timeElapsed.TotalSeconds % 60);  // Set the next query to be one second after an integer number of minutes elapsed. This way, we will query one second after the game finishes.
				else
					timeToNextCheck = TimeSpan.FromMinutes(1);  // Only query the database for time elapsed once per minute. Outside that, dead-reckon.
			}
			else
			{
				if (timeElapsed > TimeSpan.Zero)
					timeElapsed = timeElapsed.Add(TimeSpan.FromMilliseconds(timerGame.Interval));
				timeToNextCheck = timeToNextCheck.Subtract(TimeSpan.FromMilliseconds(timerGame.Interval));
			}

			UpdateNow();

			if (timeToNextCheck <= TimeSpan.Zero)
				webOutput.MostRecentServerGame = MostRecent();  // This also queries the database server.

			if (timeElapsed > TimeSpan.Zero && !gameInProgress)
			{
				webOutput.Update();
				gameInProgress = true;
			}
			if (timeElapsed == TimeSpan.Zero && gameInProgress)
			{
				webOutput.Update();
				RefreshInProgressGame();
				gameInProgress = false;
			}
			labelNow.Text = webOutput.NowText();
		}

		int Elapsed()
		{
			if (laserGameServer == null)
				return -3;
			else if (!laserGameServer.Connected)
				return -2;
			else if (timeElapsed == TimeSpan.Zero)
				return -1;
			else
				return (int)timeElapsed.TotalSeconds;
		}

		void UpdateNow()
		{
			webOutput.MostRecentHolder = leagues.MostRecent();

			if (webOutput.MostRecentHolder != null)
				webOutput.MostRecentGame = webOutput.MostRecentHolder.League.AllGames.LastOrDefault();

			if (laserGameServer == null)
				labelTime.Text = "No lasergame server";
			else if (!laserGameServer.Connected)
				labelTime.Text = "Not connected";
			else if (timeElapsed == TimeSpan.Zero)
				labelTime.Text = "Idle";
			else if (timeElapsed.TotalHours < 1)
				labelTime.Text = "+" + timeElapsed.ToString("m\\:ss");
			else
				labelTime.Text = "+" + timeElapsed.ToString();

			labelNow.Text = webOutput.NowText();
		}

		void NumericPortValueChanged(object sender, EventArgs e)
		{
			if (webOutput != null)
				webOutput.Restart(webPort);
		}

		void RankTeamBoxes()
		{
			RankTeams();
		}

		List<TeamBox> RankTeams()
		{
			var teams = TeamBoxes();

			teams.Sort((x, y) => (y.Score - x.Score) * 100 + (x.GameTeam == null || y.GameTeam == null ? 0 : (int)(x.GameTeam.Colour - y.GameTeam.Colour)));

			for (int i = 0; i < teams.Count; i++)
				teams[i].Rank = i + 1;

			return teams;
		}

		void ArrangeTeamsByRank()
		{
			var teams = RankTeams();
			try 
			{
				foreach(var team in teams)
					tableLayoutPanel1.Controls.Remove(team);

				foreach(var team in teams)
					tableLayoutPanel1.Controls.Add(team);
			}
			catch (Exception)
			{
				// Squish. Sometimes .NET throws while adding/removing panels. It shouldn't, because the number of panels after should be the same as the number of panels before.
			}
		}

		ServerGame MostRecent()
		{
			if (timeElapsed == TimeSpan.Zero || laserGameServer == null)
				return null;

			var serverGames = laserGameServer.GetGames();

			if (serverGames.Any())
			{
				laserGameServer.PopulateGame(serverGames.Last());
				serverGames.Last().InProgress = timeElapsed > TimeSpan.Zero;
				return serverGames.Last();
			}

			return null;
		}

		void RefreshGamesList()
		{
			var topItem = listViewGames.TopItem;
			var focused = listViewGames.FocusedItem ?? (listViewGames.SelectedItems.Count > 0 ? listViewGames.SelectedItems[0] : null);
			var oldGames = serverGames;
			serverGames = laserGameServer == null ? new List<ServerGame>() : laserGameServer.GetGames();

			if (serverGames.Any())
				serverGames.Last().InProgress = timeElapsed > TimeSpan.Zero;

			Cursor.Current = Cursors.WaitCursor;
			progressBar1.Value  = 0;
			try
			{
				// Link server games to league games, where a matching league game exists.
				for (int i = 0; i < serverGames.Count; i++)
				{
					var serverGame = serverGames[i];
					var leaguelist = leagues.Select(h => h.League);  // Grab the leagues out, because the below loop takes a long time, during which the user may add/remove leagues, modifying the "leagues" list.
					foreach (var league in leaguelist)
						if (PopulateGameIfIdle(league, serverGame, oldGames == null ? null : oldGames.Find(x => x.Time == serverGame.Time)))
							ProgressBar(0.95 * i / serverGames.Count, "Populated " + Utility.FriendlyDate(serverGame.Time) + serverGame.Time.ToString(" HH:mm"));
				}

				for (int i = 0; i < leagues.Count; i++)
				{
					FillEvents(leagues[i].League);
					ProgressBar(0.95 + 0.05 * i / leagues.Count, "Filled events for " + leagues[i].League.Title);
				}
			}
			finally
			{
				progressBar1.Visible = false;
				labelStatus.Text = "";
				Cursor.Current = Cursors.Default;
			}

			// Create fake "server" games to represent league games, where a server game doesn't already exist (because Acacia has forgotten it).
			foreach (Holder holder in leagues)
				foreach (Game leagueGame in holder.League.AllGames)
					if (serverGames.Find(x => x.Time == leagueGame.Time) == null)
					{
						ServerGame game = new ServerGame();
						game.League = holder.League;
						game.Game = leagueGame;
						game.Time = leagueGame.Time;
						serverGames.Add(game);
					}

			serverGames.Sort();

			listViewGames.BeginUpdate();
			try
			{
				listViewGames.Items.Clear();

				// Create items in the list view, one for each server game.
				foreach (var serverGame in serverGames)
				{
					ListViewItem item = new ListViewItem();
					item.Text = (serverGame.InProgress ? "In Progress " : serverGame.Time.FriendlyDate()) + 
						serverGame.Time.ToString(" HH:mm");

					item.SubItems.AddRange(new string[] { serverGame.League == null ? "" : serverGame.League.Title,
					                       	serverGame.Game == null || string.IsNullOrEmpty(serverGame.Game.Title) ? serverGame.Description : serverGame.Game.Title });
					item.Tag = serverGame;
					if (!serverGame.OnServer)
						item.BackColor = SystemColors.ControlLight;
					listViewGames.Items.Add(item);
				}
			}
			finally
			{
				listViewGames.EndUpdate();
			}

			if (focused != null && focused.Tag is ServerGame)
			{
				// Restore scroll position to where it was.
				foreach (ListViewItem item in listViewGames.Items)
					if (((ServerGame)item.Tag).Time == ((ServerGame)topItem.Tag).Time)
						listViewGames.TopItem = item;

				// Restore focus to the same item it was on before we started.
				foreach (ListViewItem item in listViewGames.Items)
					if (((ServerGame)item.Tag).Time == ((ServerGame)focused.Tag).Time)
					{
						listViewGames.FocusedItem = item;
						listViewGames.SelectedIndices.Add(item.Index);
						if (!listViewGames.Items[item.Index].Bounds.IntersectsWith(listViewGames.ClientRectangle))  // if item is not visible
							listViewGames.EnsureVisible(item.Index);
					}
			}
		}

		void PopulateTeamPlayersFromServerGame(League league, ServerGame serverGame, List<GamePlayer> players)
		{
			for (int p = 0; p < players.Count; p++)
				if (!(players[p] is ServerPlayer))
				{
					var playerId = players[p].PlayerId;
					var serverPlayer = serverGame.Players.Find(sp => sp.PlayerId == playerId);
					if (serverPlayer != null)
					{
						var oldPlayer = players[p];
						var leaguePlayer = league.LeaguePlayer(oldPlayer);

						oldPlayer.CopyTo(serverPlayer);
						players[p] = serverPlayer;
					}
				}
		}

		bool PopulateGameIfIdle(League league, ServerGame serverGame, ServerGame oldGame)
		{
			Game leagueGame = league.AllGames.Find(x => x.Time == serverGame.Time);
			if (leagueGame == null)
				return false;

			serverGame.League = league;
			serverGame.Game = leagueGame;
			leagueGame.ServerGame = serverGame;
			if (oldGame != null && !oldGame.InProgress && oldGame.Events.Any()) {
				serverGame.Events = oldGame.Events;
				serverGame.Players.AddRange(oldGame.Players.Where(p => !serverGame.Players.Exists(p2 => p2.ServerPlayerId == p.ServerPlayerId)));
			}
			else if (timeElapsed == TimeSpan.Zero && serverGame.Events.Count == 0 && laserGameServer != null) // timeElapsed == 0 means system is idle -- if a game is in progress, we don't want to query/populate 99 games when the user starts the app or opens a league file.
			{
				laserGameServer.PopulateGame(serverGame);

				// Attempt to replace each GamePlayer with a ServerPlayer. This gives us under-the-hood data used by e.g. GameHeatMap.
				foreach (var gameTeam in leagueGame.Teams)
					PopulateTeamPlayersFromServerGame(league, serverGame, gameTeam.Players);

				PopulateTeamPlayersFromServerGame(league, serverGame, leagueGame.UnallocatedPlayers);
			}
			return true;
		}
		
		void RefreshInProgressGame()
		{
			foreach(ListViewItem item in listViewGames.Items)
				if (((ServerGame)item.Tag).InProgress)
				{
					var serverGame = (ServerGame)item.Tag;
					laserGameServer.PopulateGame(serverGame);
					item.SubItems.Clear();
					item.Text = serverGame.Time.FriendlyDate() + serverGame.Time.ToString(" HH:mm");
					item.SubItems.AddRange(new string[] { serverGame.League == null ? "" : serverGame.League.Title,
					                       	serverGame.Game == null || string.IsNullOrEmpty(serverGame.Game.Title) ? serverGame.Description : serverGame.Game.Title });
				}
		}

		/// <summary>Pull Event data from ServerGames and put it into GamePlayers in league Games.</summary>
		bool FillEvents(League league)
		{
			bool any = false;
			foreach (Game game in league.AllGames)
				any |= game.PopulateEvents();
			return any;
		}

		void ListViewGamesDrawItem(object sender, DrawListViewItemEventArgs e)
		{
			SolidBrush brush = new SolidBrush(SystemColors.Window);
			if ((e.State & ListViewItemStates.Selected) != 0)  // Selected item.
				brush.Color = SystemColors.ActiveCaption;
			else if (!((ServerGame)e.Item.Tag).OnServer)
				brush.Color = SystemColors.ControlLight;
	
            e.Graphics.FillRectangle(brush, e.Bounds);
            e.DrawFocusRectangle();
            e.DrawText();
		}

		void ListViewGamesDrawSubItem(object sender, DrawListViewSubItemEventArgs e)
		{
			TextFormatFlags flags = TextFormatFlags.Left;

			// Store the column text alignment, letting it default to Left if it has not been set to Center or Right.
			switch (e.Header.TextAlign)
			{
			    case HorizontalAlignment.Center:
			        flags = TextFormatFlags.HorizontalCenter;
			        break;
			    case HorizontalAlignment.Right:
			        flags = TextFormatFlags.Right;
			        break;
			}
			
//			if ((e.ItemState & ListViewItemStates.Selected) == 0)
//			    e.DrawBackground();
			// Draw normal text.
			e.DrawText(flags);
		}

		public void LoadSettings()
		{
			var doc = new XmlDocument();
			string filename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Torn", "Torn5.Settings");
			if (!File.Exists(filename))
			{
				filename = Path.Combine(Environment.CurrentDirectory, "Torn5.Settings");
				if (!File.Exists(filename))
					return;
			}
			doc.Load(filename);

			var root = doc.DocumentElement;

			Enum.TryParse(root.GetString("SystemType", "Acacia"), out systemType);
			Enum.TryParse(root.GetString("GroupPlayersBy", "Colour"), out groupPlayersBy);
			serverAddress = root.GetString("GameServerAddress", "localhost");
			windowsAuth = root.GetString("Auth", "") == "windows";
			sqlUserId = root.GetString("SqlUserId");
			sqlPassword = root.GetString("SqlPassword");
			webPort = int.Parse(root.GetString("WebServerPort", "8080"));
			exportFolder = root.GetString("ExportFolder", "");
			selectedNode = root.GetString("Selected", "");

			XmlNodeList xleagues = root.SelectSingleNode("leagues").SelectNodes("holder");

			foreach (XmlNode xleague in xleagues)
			{
				Holder holder = AddLeague(xleague.GetString("filename"), xleague.GetString("key"));

				var xtemplates = xleague.SelectSingleNode("reporttemplates");
				if (xtemplates != null)
					holder.ReportTemplates.FromXml(doc, xtemplates);

				holder.Fixture.Teams.Parse(holder.League);
				var xfixtures = xleague.SelectSingleNode("fixtures");
				if (xfixtures != null)
					holder.Fixture.Games.Parse(xfixtures.InnerText, holder.Fixture.Teams, '\t');  // TODO: change to .FromXml(doc, xfixtures);
			}
		}

		public void SaveSettings()
		{
			XmlDocument doc = new XmlDocument();
			XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
			doc.AppendChild(docNode);

			XmlNode bodyNode = doc.CreateElement("body");
			doc.AppendChild(bodyNode);

			doc.AppendNode(bodyNode, "SystemType", systemType.ToString());
			if (systemType == SystemType.Laserforce)
			{
				doc.AppendNode(bodyNode, "Auth", windowsAuth ? "windows" : "sql");
				doc.AppendNode(bodyNode, "SqlUserId", sqlUserId);
				doc.AppendNode(bodyNode, "SqlPassword", sqlPassword);
			}
			doc.AppendNode(bodyNode, "GroupPlayersBy", groupPlayersBy.ToString());
			doc.AppendNode(bodyNode, "GameServerAddress", serverAddress);
			doc.AppendNode(bodyNode, "WebServerPort", webPort.ToString());
			doc.AppendNode(bodyNode, "ExportFolder", exportFolder);
			if (listViewLeagues.SelectedItems.Count > 0)
				doc.AppendNode(bodyNode, "Selected", listViewLeagues.SelectedItems[0].Text);
			doc.AppendNode(bodyNode, "UploadMethod", uploadMethod);
			doc.AppendNode(bodyNode, "UploadSite", uploadSite);
			doc.AppendNode(bodyNode, "Username", username);
			doc.AppendNode(bodyNode, "Password", password);

			XmlNode leaguesNode = doc.CreateElement("leagues");
			bodyNode.AppendChild(leaguesNode);

			foreach (var holder in leagues)
			{
				XmlNode holderNode = doc.CreateElement("holder");
				leaguesNode.AppendChild(holderNode);

				doc.AppendNode(holderNode, "key", holder.Key);
				doc.AppendNode(holderNode, "filename", holder.FileName);

				if (holder.ReportTemplates.Any() || holder.Fixture.Games.Count() > 0)
					holder.ReportTemplates.ToXml(doc, holderNode);

				if (holder.Fixture.Games.Count() > 0)
					doc.AppendNode(holderNode, "fixtures", holder.Fixture.Games.ToString());  // TODO: change to .ToXml(doc, holderNode);
			}

			string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Torn");
			Directory.CreateDirectory(path);
			doc.Save(Path.Combine(path, "Torn5.Settings"));
		}
	}
}
