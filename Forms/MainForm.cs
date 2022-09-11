using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Torn.Report;
using Torn5;
using Zoom;

/*
Broad architecture:
* MainForm has more business logic in it than it probably should.
* timerGame fires once per second, but only does serious work once per minute. It polls the lasergame server for time remaining.
* There are a variety of classes that descend from LasergameServer, which implement connections to various proprietary systems (P&C Acacia, P&C O-Zone, Laserforce).
* WebOuput is an important module. It runs the internal web server, allowing others to query us for data in HTML or JSON, and also generates web pages to file for export or upload.
* Reports.cs contains the code that generates the data for each report type. ZoomReports renders those to various output formats: HTML with tables, HTML with SVG, .csv, etc.

IMPLEMENTED: load league, save league, delete game from league
read from P&C server, web server, report to HTML, upload to FTP,
transfer to team boxes via drag-drop, transfer to team boxes on game selection, set dimensions, persistence
show team name, scores and rank in team boxes
right-click remember team, identify team, identify player, handicap team
edit league -- team add/delete/rename, player add/delete/re-ID; implement OK/Cancel
better save format
read from demo "server"
read from laserforce server
sanity check report. Add new check: are there odd games out with no victory points?
tech report: hit totals for all sensors on all packs, plus games where a sensor takes 0 hits
output to printer
set up pyramid round
read from O-Zone server

TODO for BOTH:
on commit auto-update scoreboard
right-click handicap player, merge player

TODO for LEAGUE:
handicap, on commit auto-update team handicaps
set up fixtures

TODO for ZLTAC:
recalculate scores on Helios

OTHER:
league copy from
Space Marines match play
spark lines
check latest version via REST
reports and uploads in worker thread
option to zero eliminated players.
Move global settings to Program.cs

NEEDS TESTING:
adjust team score/victory points
remember all teams
upload to http, https, ftp
If we don't find a settings file in the user folder, check for one in the exe folder.
group players by LotR
on commit auto-update teams
send to scoreboard (web browser)
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

		string serverPort = "12123";


		int webPort;
		WebOutput webOutput;
		TornTcpListener tornTcpListener;
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
		bool hostRemoteTorn;
		string remoteTornPort;

		DateTime lastChecked = DateTime.MinValue;
		TimeSpan timeToNextCheck = TimeSpan.FromSeconds(5);
		TimeSpan timeElapsed = TimeSpan.FromMilliseconds(-1);
		bool gameInProgress = false;

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
			// Tries to export entire server could be 500 games and crashes everything
			ribbonButtonExportJson.Enabled = false;
			leagues = new Holders();
			webPort = 8080;
			systemType = SystemType.Demo;

			LoadSettings();

			webOutput = new WebOutput(webPort)
			{
				Leagues = leagues,
				Elapsed = Elapsed,
				ExportFolder = exportFolder
			};

			playersBox = new PlayersBox
			{
				Images = imageListPacks,
				GetMoveTarget = FindEmptyTeamBox
			};
			tableLayoutPanel1.Controls.Add(playersBox, 2, 0);
			tableLayoutPanel1.SetRowSpan(playersBox, tableLayoutPanel1.RowCount);
			playersBox.Dock = DockStyle.Fill;

			formPlayer = new FormPlayer
			{
				Icon = (Icon)Icon.Clone(),
			};

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
					case SystemType.Nexus: laserGameServer = new PAndCNexusWithIButton(serverAddress);  break;
					case SystemType.Zeon: laserGameServer = new PAndC(serverAddress);  break;
					case SystemType.OZone: laserGameServer = new OZone(serverAddress, serverPort);  break;
					case SystemType.Torn:
						laserGameServer = new TornTcpServer(serverAddress, serverPort);
						timeElapsed = laserGameServer.GameTimeElapsed();
					break;
					case SystemType.Demo: laserGameServer = new DemoServer();  break;
				}

				formPlayer.LaserGameServer = laserGameServer;
				webOutput.GetGames = laserGameServer.GetGames;
				webOutput.PopulateGame = laserGameServer.PopulateGame;
				webOutput.Players = laserGameServer.GetPlayers;
				tornTcpListener?.Close();
				if (hostRemoteTorn)
				{
					tornTcpListener = new TornTcpListener(laserGameServer, remoteTornPort);
					tornTcpListener.Connect();
				}
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
				tornTcpListener?.Close();
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
			item.SubItems.AddRange(new string[] { league.Title, league.AllGames.Count.ToString(CultureInfo.CurrentCulture), league.Teams.Count.ToString(CultureInfo.CurrentCulture), fileName });
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
				tableLayoutPanel1.SetRowSpan(splitContainer1, tableLayoutPanel1.RowCount + adjust);
				tableLayoutPanel1.SetRowSpan(panelGames, tableLayoutPanel1.RowCount + adjust);
				tableLayoutPanel1.SetRowSpan(playersBox, tableLayoutPanel1.RowCount + adjust);
			}
			catch
			{
			}		
		}

		void EnableRemoveRowColumnButtons()
		{
			ribbonButtonRemoveColumn.Enabled = tableLayoutPanel1.ColumnCount > 4;
			ribbonButtonRemoveRow.Enabled = tableLayoutPanel1.RowCount > 1;
		}

		void AddTeamBoxes()
		{
			while(tableLayoutPanel1.Controls.Count - 3 < tableLayoutPanel1.RowCount * (tableLayoutPanel1.ColumnCount - 3))
			{
				TeamBox teamBox = new TeamBox
				{
					League = activeHolder?.League,
					Images = imageListPacks,
					GetMoveTarget = FindEmptyTeamBox,
					RankTeams = RankTeamBoxes,
					SortTeamsByRank = ArrangeTeamsByRank,
					FormPlayer = formPlayer
				};
				tableLayoutPanel1.Controls.Add(teamBox);
				teamBox.Dock = DockStyle.Fill;
			}
			EnableRemoveRowColumnButtons();
		}

		void ButtonAboutClick(object sender, EventArgs e)
		{
			MessageBox.Show("A tournament scores editor by Doug Burbidge & AJ Horsman.\n\nhttp://www.dougburbidge.com/Apps/\n\nhttps://github.com/DougBurbidge/Torn5/\nhttps://github.com/MrMeeseeks200/Torn5/", "Torn 5");
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
			if (tableLayoutPanel1.RowCount > 1)
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
			List<ServerGame> serverGames = new List<ServerGame>();

			foreach (ListViewItem item in listViewGames.SelectedItems)
			{
				var teamDatas = new List<GameTeamData>();
				var teamBoxes = TeamBoxes();
				// Build a list, one TeamData per TeamBox, connecting each GameTeam to its ServerPlayers.
				foreach (TeamBox teamBox in teamBoxes)
					if (teamBox.Players().Any())
						teamDatas.Add(new GameTeamData
						{
							GameTeam = teamBox.GameTeam,
							Players = teamBox.Players()
						});

				if (teamDatas.Any())
				{
					ServerGame serverGame = item.Tag as ServerGame;

					laserGameServer.PopulateGame(serverGame);
					serverGames.Add(serverGame);

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
					if (autoUpdateScoreboard)
						UpdateScoreboard(serverGame);
				}
			}

			if (GetExportFolder())
			{
				Cursor.Current = Cursors.WaitCursor;
				progressBar1.Value = 0;
				try
				{
					webOutput.ExportGamesToJSON(exportFolder, serverGames, ProgressBar);
				}
				finally
				{
					FinishProgress();
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

			var form = new FormLeague
			{
				Icon = (Icon)Icon.Clone(),
				League = activeHolder.League.Clone(),
				FormPlayer = formPlayer
			};

			if (form.League.Teams.Count == 0)
				foreach (var ft in activeHolder.Fixture.Teams)
				{
					var lt = new LeagueTeam { Name = ft.Name };
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
			if (GetExportFolder())
			{
				Cursor.Current = Cursors.WaitCursor;
				progressBar1.Value  = 0;
				try {
					ExportPages.ExportReports(exportFolder, IncludeSecret(), SelectedLeagues(), ProgressBar);
				}
				finally
				{
					FinishProgress();
				}
			}
		}

		private void ButtonExportJsonClick(object sender, EventArgs e)
		{
			if (GetExportFolder())
			{
				Cursor.Current = Cursors.WaitCursor;
				progressBar1.Value = 0;
				try
				{
					webOutput.ExportJson(exportFolder, ProgressBar);
				}
				finally
				{
					FinishProgress();
				}
			}
		}

		private string ColorToTColor(Color color)
        {
			var r = color.R.ToString("X2");
			var g = color.G.ToString("X2");
			var b = color.B.ToString("X2");

			return "$02" + b + g + r;
		}

		private void UpdateScoreboard(ServerGame serverGame)
        {
			int TBOARD_SOCKET = 21570;

			UdpClient udp = new UdpClient();
			IPEndPoint groupEP = new IPEndPoint(IPAddress.Parse("255.255.255.255"), TBOARD_SOCKET);

			string fontColour = "$02000000"; //black

			League league = serverGame.League;
			Game game = league.AllGames.Find(g => g.Time == serverGame.Time);

			string message = "DISPLAYREPORTS";

			foreach (GameTeam team in game.Teams)
			{
				LeagueTeam leagueTeam = league.Teams.Find(t => t.TeamId == team.TeamId);

				string teamColour = ColorToTColor(team.Colour.ToColor());
				string teamColourLight = ColorToTColor(team.Colour.ToSaturatedColor());

				bool hasTR = (team.Players.Count > 0 && team.Players[0].HitsBy > 0) || (team.Players?.Count > 0 && team.Players[0].HitsOn > 0);

				string teamString = "," + fontColour + "," + teamColourLight + "," + teamColour + "," + teamColour + ",\"" + leagueTeam?.Name + " " + leagueTeam?.Handicap + " " + team.Score + "\",\"Player,Score," + (hasTR ? "TR," : "") + "Rank\",\"left,right," + (hasTR ? "right," : "") + "right\"";

				foreach (GamePlayer player in team.Players)
				{
					LeaguePlayer leaguePlayer = league.Players.Find(p => p.Id == player.PlayerId);
					string alias = leaguePlayer?.Name != null ? leaguePlayer.Name : player.Pack;
					decimal tagRatio = hasTR ? (Convert.ToDecimal(player.HitsBy) / Convert.ToDecimal(player.HitsOn)) : 0;
					teamString += ",\"" + alias + "\",clNone,1," + player.Score + ",clNone,1," + (hasTR ? tagRatio.ToString("0.00") + ",clNone,1," : "") + player.Rank + ",clNone,1,EOREOR";

				}

				teamString += ",EOTEOT";

				message += teamString;
			}

			message += ",EOSEOS";

			List<string> strs = message.Split(510).ToList();

			foreach (string str in strs)
			{
				string index = strs.IndexOf(str).ToString().PadLeft(2, '0');
				string chunk = index + str + "\x00";
				byte[] sendBytes = Encoding.ASCII.GetBytes(chunk);
				udp.Send(sendBytes, sendBytes.Length, groupEP);
			}

			string emptyIndex = strs.Count().ToString().PadLeft(2, '0');
			byte[] sendBytesEnd = Encoding.ASCII.GetBytes(emptyIndex + "\x00");
			udp.Send(sendBytesEnd, sendBytesEnd.Length, groupEP);
		}

		private void ButtonUpdateScoreboardClick(object sender, EventArgs e)
        {
			var item = listViewGames.SelectedItems[0];
			if (item.Tag is ServerGame serverGame && serverGame.Game != null)
			{
				UpdateScoreboard(serverGame);
			} else
            {
				MessageBox.Show("Please Commit Game First", "Cannot Display Scoreboard", MessageBoxButtons.OK);
            }
		}

		private void ButtonPrintReportsClick(object sender, EventArgs e)
		{
			if (listViewLeagues.SelectedItems.Count > 0)
			{
				var holder = (Holder)listViewLeagues.SelectedItems[0].Tag;
				PrintDocument pd;
				Cursor.Current = Cursors.WaitCursor;
				try
				{
					pd = ReportPages.OverviewReports(holder, true).ToPrint();
				}
				finally
				{
					Cursor.Current = Cursors.Default;
				}

				pd.DocumentName = holder.League.Title;
				if (printDialog.ShowDialog() == DialogResult.OK)
					pd.Print();
			}
		}

		ReportTemplate adhocReportTemplate;
		private void ButtonAdHocReportClick(object sender, EventArgs e)
		{
			Holder holder = SelectedLeagues().FirstOrDefault();
			if (holder != null)
			{
				if (adhocReportTemplate == null)
				{
					adhocReportTemplate = new ReportTemplate { ReportType = ReportType.TeamLadder };
					adhocReportTemplate.Settings.Add("Description");
				}

				if (new FormReport
				{
					Text = "Report on " + (SelectedLeagues().Count == 1 ? holder.League.Title : SelectedLeagues().Count.ToString() + " leagues"),
					From = (holder.League.AllGames.FirstOrDefault()?.Time ?? DateTime.Now).Date,
					To = (holder.League.AllGames.LastOrDefault()?.Time ?? DateTime.Now).Date,
					ReportTemplate = adhocReportTemplate,
					League = SelectedLeagues().FirstOrDefault()?.League,
					Icon = (Icon)this.Icon.Clone()

			}.ShowDialog() == DialogResult.OK)
				{
					Cursor.Current = Cursors.WaitCursor;
					try
					{
						new FormAdhoc 
						{
							Report = (ZoomReport)ReportPages.Report(SelectedLeagues().Select(h => h.League).ToList(), IncludeSecret(), adhocReportTemplate),
							Icon = (Icon)this.Icon.Clone()
						}.Show();
					}
					finally
					{
						Cursor.Current = Cursors.Default;
					}
				}
			}
		}

		void ButtonExportFixturesClick(object sender, EventArgs e)
		{
			if (GetExportFolder())
				ExportPages.ExportFixtures(exportFolder, SelectedLeagues());
		}

		FormFixture formFixture = new FormFixture();
		void ButtonFixtureClick(object sender, EventArgs e)
		{
			if (formFixture == null)
				formFixture = new FormFixture() { Icon = (Icon)Icon.Clone() };
			if (listViewLeagues.SelectedItems.Count == 1)
			{
				formFixture.Holder = (Holder)listViewLeagues.SelectedItems[0].Tag;
				formFixture.ExportFolder = exportFolder;
				formFixture.ShowDialog();
			}
		}

		void ButtonForgetClick(object sender, EventArgs e)
		{
			if (listViewGames.SelectedItems.Count == 0)
				return;

			var updatedLeagues = new List<League>();

			foreach (ListViewItem item in listViewGames.SelectedItems)
				if (item.Tag is ServerGame serverGame && serverGame.Game != null)
				{
					var holders = leagues.FindAll(h => h.League.AllGames.Any(g => g.Time == serverGame.Time));
					foreach (Holder holder in holders)
					{
						holder.League.AllGames.RemoveAll(g => g.Time == serverGame.Time);
						item.SubItems[1].Text = null;
						item.SubItems[2].Text = serverGame.Description;

						if (!updatedLeagues.Contains(holder.League))
							updatedLeagues.Add(holder.League);
					}
					serverGame.Game = null;
					serverGame.League = null;
				}

			foreach (var league in updatedLeagues)
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
			laserGameServer.GameTimeElapsed();
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

		void ButtonPreferencesClick(object sender, EventArgs e)
		{
			var form = new FormPreferences
			{
				Icon = (Icon)Icon.Clone(),
				GroupPlayersBy = groupPlayersBy,
				AutoUpdateScoreboard = autoUpdateScoreboard,
				AutoUpdateTeams = autoUpdateTeams,
				ReportsFolder = exportFolder,
				UploadMethod = uploadMethod,
				UploadSite = uploadSite,
				Username = username,
				Password = password,

				SystemType = systemType,
				ServerAddress = serverAddress,
				ServerPort = serverPort,
				WindowsAuth = windowsAuth,
				Sqluser = sqlUserId,
				SqlPassword = sqlPassword,
				WebPort = webPort,
				LogFolder = logFolder,
				HostRemoteTorn = hostRemoteTorn,
				RemoteTornPort = remoteTornPort
			};

			if (form.ShowDialog() == DialogResult.OK)
			{
				groupPlayersBy = form.GroupPlayersBy;
				autoUpdateScoreboard = form.AutoUpdateScoreboard;
				autoUpdateTeams = form.AutoUpdateTeams;
				exportFolder = form.ReportsFolder;
				uploadMethod = form.UploadMethod;
				uploadSite = form.UploadSite;
				username = form.Username;
				password = form.Password;

				systemType = form.SystemType;
				serverAddress = form.ServerAddress;
				serverPort = form.ServerPort;
				windowsAuth = form.WindowsAuth;
				sqlUserId = form.Sqluser;
				sqlPassword = form.SqlPassword;
				logFolder = form.LogFolder;
				remoteTornPort = form.RemoteTornPort;
				hostRemoteTorn = form.HostRemoteTorn;
				
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

			var firstItem = listViewGames.SelectedItems[0];
			while(firstItem.SubItems.Count <= 2) firstItem.SubItems.Add("");

			var id = new InputDialog("Description: ", "Set a description", firstItem.SubItems[2].Text);
			if (id.ShowDialog() == DialogResult.OK)
				foreach (ListViewItem item in listViewGames.SelectedItems)
					if (item.Tag is ServerGame serverGame && serverGame.Game != null)
					{
						UpdateGameDescription(serverGame.Time.ToString("yyyy/MM/dd HH:mm:ss"), id.Response, serverGame.League.FileName);
						while (item.SubItems.Count <= 2)
							item.SubItems.Add("");
						item.SubItems[2].Text = id.Response;
					}
		}

		void UpdateGameDescription(string gameTime, string description, string fileName) {
			Console.WriteLine(gameTime + " " + description + " " + fileName);
			var doc = new XmlDocument();
			doc.Load(fileName);
			var root = doc.DocumentElement;
			XmlNodeList gameNodes = root.SelectSingleNode("games").SelectNodes("game");
			foreach (XmlNode gameNode in gameNodes)
            {
				XmlNode timeNode = gameNode.SelectSingleNode("ansigametime");
				string time = timeNode.InnerText;
				Console.WriteLine("Time: " + time);
				if (timeNode.InnerText == gameTime)
                {
					Console.WriteLine("AAAA");
					if (gameNode.SelectSingleNode("title") == null)
                    {
						doc.AppendNode(gameNode, "title", description);
                    } else
                    {
						gameNode.SelectSingleNode("title").InnerText = description;

					}
                }
            }
			doc.Save(fileName);
		}

		void ButtonUploadClick(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(uploadMethod) || string.IsNullOrEmpty(uploadSite) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
			{
				MessageBox.Show("Please fill in details under Leagues > Preferences > Upload.", "Upload Details Required");
				return;
			}

			if (GetExportFolder())
			{
				progressBar1.Value = 0;
				try
				{
					ExportPages.UploadFiles(uploadMethod, uploadSite, username, password, exportFolder, SelectedLeagues(), ProgressBar);
				}
				finally {
					FinishProgress();
				}
			}
		}

		bool GetExportFolder()
		{
			if (!string.IsNullOrEmpty(exportFolder))
				return true;

			folderBrowserDialog1.Description = "Select a root folder for export of reports.";
			folderBrowserDialog1.SelectedPath = exportFolder;

			bool result = folderBrowserDialog1.ShowDialog() == DialogResult.OK;
			if (result)
			{
				exportFolder = folderBrowserDialog1.SelectedPath;
				webOutput.ExportFolder = exportFolder;
			}
			return result;
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

		void FinishProgress()
		{
			progressBar1.Visible = false;
			labelStatus.Text = "";
			Cursor.Current = Cursors.Default;
		}

		TeamBox FindEmptyTeamBox()
		{
			foreach (Control c in tableLayoutPanel1.Controls)
				if (c is TeamBox teamBox && teamBox.Items.Count == 0)
					return teamBox;

			return null;
		}

		List<TeamBox> TeamBoxes()
		{
			var teamBoxes = new List<TeamBox>();
			foreach (Control c in tableLayoutPanel1.Controls)
				if (c is TeamBox teamBox)
					teamBoxes.Add(teamBox);

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
				else // Alias or LotR
				{ 
					List<ServerPlayer> addedPlayers = new List<ServerPlayer>();
					foreach (var player in playersBox.Players())
					{
						bool isDuplicatePlayer = addedPlayers.Exists(p => p.PlayerId == player.PlayerId);
						if (!isDuplicatePlayer && box < teamBoxes.Count)
						{
							List<ServerPlayer> playersToAdd = new List<ServerPlayer>();
							playersToAdd.Add(player);
							teamBoxes[box++].Accept(playersToAdd);
							addedPlayers.Add(player);
						}
					}
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
						                                      (!string.IsNullOrEmpty(sp.ServerPlayerId) && gp is ServerPlayer player && sp.ServerPlayerId == player.ServerPlayerId) ||
						                                      (!string.IsNullOrEmpty(sp.Pack) && sp.Pack == gp.Pack));
						if (serverPlayer != null)
						{
							serverPlayers.Add(serverPlayer);

							serverPlayer.PlayerId = gp.PlayerId;

							int yCard = gp.YellowCards;
							int rCard = gp.RedCards;

							serverPlayer.Item.SubItems[1].Text = (rCard > 0 ? (rCard + "R ") : "") + (yCard > 0 ? (yCard + "Y ") : "") + league.Alias(gp);
						}
					}

					if (serverPlayers.Any() && box < teamBoxes.Count)
					{
						teamBoxes[box].LeagueTeam = league.LeagueTeam(gameTeam);
						teamBoxes[box].GameTeam = gameTeam;
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
			ribbonButtonSetDescription.Enabled = listViewGames.SelectedItems.Count > 0;
			updateScoreboard.Enabled = listViewGames.SelectedItems.Count == 1;
			ribbonButtonForget.Enabled = listViewGames.SelectedItems.Count > 0;
			ribbonButtonCommit.Enabled = EnableCommit();

			foreach (var tb in TeamBoxes())
				tb.Clear();

			if (listViewGames.SelectedItems.Count == 1)
			{
				ServerGame game = ((ServerGame)listViewGames.SelectedItems[0].Tag);

				if (laserGameServer != null)
					laserGameServer.PopulateGame(game);

				playersBox.LoadGame(activeHolder?.League, game);
				formPlayer.CurrentLeague = activeHolder?.League;

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

			ribbonButtonClose.Enabled = b;
			ribbonButtonSave.Enabled = b;
			ribbonButtonReport.Enabled = b;
			ribbonButtonUpload.Enabled = b;
			ribbonButtonConfigure.Enabled = listViewLeagues.SelectedItems.Count == 1;

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
				tb.League = activeHolder?.League;
		}

		void TimerGameTick(object sender, EventArgs e)
		{
			if (timeToNextCheck <= TimeSpan.Zero)
			{
				timeElapsed = laserGameServer == null ? TimeSpan.Zero : laserGameServer.GameTimeElapsed();  // This queries the lasergame server.
				timeElapsed = timeElapsed.TotalSeconds < 0 ? TimeSpan.Zero : timeElapsed;

				if (timeElapsed > TimeSpan.FromSeconds(1))
					timeToNextCheck = TimeSpan.FromSeconds(61 - timeElapsed.TotalSeconds % 60);  // Set the next query to be one second after an integer number of minutes of game time elapsed. This way, we will query one second after the game finishes.
				else
					timeToNextCheck = TimeSpan.FromMinutes(1);  // Only query the lasergame server for time elapsed once per minute. Outside that, dead-reckon.
			}
			else
			{
				if (timeElapsed > TimeSpan.Zero)
					timeElapsed = timeElapsed.Add(TimeSpan.FromMilliseconds(timerGame.Interval));
				timeToNextCheck = timeToNextCheck.Subtract(TimeSpan.FromMilliseconds(timerGame.Interval));
			}

			UpdateNow();

			if (timeToNextCheck <= TimeSpan.Zero)
				webOutput.MostRecentServerGame = MostRecent();  // This also queries the lasergame server.

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

			if (DateTime.Now.Subtract(lastChecked) >= new TimeSpan(0, 0, 20) && laserGameServer is JsonServer)  // This handles the case where a JsonServer is querying itself for debugging or demo purposes.
			{
				timeElapsed = laserGameServer.GameTimeElapsed();
				lastChecked = DateTime.Now;
			}

			if (timeElapsed == TimeSpan.Zero)
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
			else if (timeElapsed < new TimeSpan(0, 0, -1))
				labelTime.Text = timeElapsed.ToString();
			else if (timeElapsed.TotalHours < 1)
				labelTime.Text = "+" + timeElapsed.ToString("m\\:ss");
			else
				labelTime.Text = "+" + timeElapsed.ToString();

			if (!string.IsNullOrEmpty(laserGameServer?.Status))
				labelNow.Text = laserGameServer.Status;
			else
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
						if (PopulateGameIfIdle(league, serverGame, oldGames?.Find(x => x.Time == serverGame.Time)))
							ProgressBar(0.95 * i / serverGames.Count, "Populated " + Utility.FriendlyDateTime(serverGame.Time));
				}

				for (int i = 0; i < leagues.Count; i++)
				{
					FillEvents(leagues[i].League);
					ProgressBar(0.95 + 0.05 * i / leagues.Count, "Filled events for " + leagues[i].League.Title);
				}
			}
			finally
			{
				FinishProgress();
			}

			// Create fake "server" games to represent league games, where a server game doesn't already exist (because Acacia has forgotten it).
			foreach (Holder holder in leagues)
				foreach (Game leagueGame in holder.League.AllGames)
					if (serverGames.Find(x => x.Time == leagueGame.Time) == null)
						serverGames.Add(new ServerGame
						{
							League = holder.League,
							Game = leagueGame,
							Time = leagueGame.Time
						});

			serverGames.Sort();

			listViewGames.BeginUpdate();
			try
			{
				listViewGames.Items.Clear();

				// Create items in the list view, one for each server game.
				foreach (var serverGame in serverGames)
				{
					ListViewItem item = new ListViewItem
					{
						Text = (serverGame.InProgress ? "In Progress " : serverGame.Time.FriendlyDateTime()),
						Tag = serverGame
					};
					item.SubItems.AddRange(new string[] { serverGame.League == null ? "" : serverGame.League.Title,
					                       	serverGame.Game == null || string.IsNullOrEmpty(serverGame.Game.Title) ? serverGame.Description : serverGame.Game.Title });
					if (!serverGame.OnServer)
						item.BackColor = SystemColors.ControlLight;
					listViewGames.Items.Add(item);
				}
			}
			finally
			{
				listViewGames.EndUpdate();
			}

			if (focused != null && focused.Tag is ServerGame sg)
			{
				// Restore scroll position to where it was.
				foreach (ListViewItem item in listViewGames.Items)
					if (((ServerGame)item.Tag).Time == ((ServerGame)topItem.Tag).Time)
						listViewGames.TopItem = item;

				// Restore focus to the same item it was on before we started.
				foreach (ListViewItem item in listViewGames.Items)
					if (((ServerGame)item.Tag).Time == sg.Time)
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
			foreach (ListViewItem item in listViewGames.Items)
			{
				var serverGame = (ServerGame)item.Tag;
				if (serverGame.InProgress)
				{
					laserGameServer.PopulateGame(serverGame);
					item.SubItems.Clear();
					item.Text = serverGame.Time.FriendlyDateTime();
					item.SubItems.AddRange(new string[] { serverGame.League == null ? "" : serverGame.League.Title,
											   serverGame.Game == null || string.IsNullOrEmpty(serverGame.Game.Title) ? serverGame.Description : serverGame.Game.Title });
				}
			}
		}

		/// <summary>Pull Event data from ServerGames and put it into GamePlayers in league Games.</summary>
		bool FillEvents(League league)
		{
			bool any = false;
			var games = league.AllGames.ToList();  // Have to clone this by calling ToList. Without this, it sometimes complains that the collection has been modified partway through the foreach, and I don't know why.
			foreach (Game game in games)
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
			serverPort = root.GetString("GameServerPort", "12123");
			windowsAuth = root.GetString("Auth", "") == "windows";
			sqlUserId = root.GetString("SqlUserId");
			sqlPassword = root.GetString("SqlPassword");
			webPort = int.Parse(root.GetString("WebServerPort", "8080"));
			exportFolder = root.GetString("ExportFolder", "");
			logFolder = root.GetString("LogFolder", "");
			selectedNode = root.GetString("Selected", "");
			hostRemoteTorn = int.Parse(root.GetString("HostRemoteTorn", "0")) > 0 ;
			remoteTornPort = root.GetString("RemoteTornPort", "1300");

			XmlNodeList xleagues = root.SelectSingleNode("leagues").SelectNodes("holder");

			foreach (XmlNode xleague in xleagues)
			{
				Holder holder = AddLeague(xleague.GetString("filename"), xleague.GetString("key"));

				var xtemplates = xleague.SelectSingleNode("reporttemplates");
				if (xtemplates != null)
					holder.ReportTemplates.FromXml(xtemplates);

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
			doc.AppendNode(bodyNode, "GameServerPort", serverPort);
			doc.AppendNode(bodyNode, "WebServerPort", webPort.ToString());
			doc.AppendNode(bodyNode, "ExportFolder", exportFolder);
			doc.AppendNode(bodyNode, "LogFolder", logFolder);
			if (listViewLeagues.SelectedItems.Count > 0)
				doc.AppendNode(bodyNode, "Selected", listViewLeagues.SelectedItems[0].Text);
			doc.AppendNode(bodyNode, "UploadMethod", uploadMethod);
			doc.AppendNode(bodyNode, "UploadSite", uploadSite);
			doc.AppendNode(bodyNode, "Username", username);
			doc.AppendNode(bodyNode, "Password", password);
			doc.AppendNode(bodyNode, "HostRemoteTorn", hostRemoteTorn ? 1 : 0);
			doc.AppendNode(bodyNode, "RemoteTornPort", remoteTornPort);

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

		private void exportJSONToolStripMenuItem_Click(object sender, EventArgs e)
		{
			League league = activeHolder?.League;

			List<ServerGame> serverGames = new List<ServerGame>();

			foreach(ListViewItem item in listViewGames.SelectedItems)
            {
				if (item.Tag is ServerGame serverGame)
				{
					laserGameServer.PopulateGame(serverGame);
					serverGames.Add(serverGame);
				}
			}

			if (GetExportFolder())
			{
				Cursor.Current = Cursors.WaitCursor;
				progressBar1.Value = 0;
				try
				{
					webOutput.ExportGamesToJSON(exportFolder, serverGames, ProgressBar);
				}
				finally
				{
					FinishProgress();
				}
			}

		}
    }
	public static class Extensions
	{
		public static IEnumerable<string> Split(this string str, int n)
		{
			if (String.IsNullOrEmpty(str) || n < 1)
			{
				throw new ArgumentException();
			}

			for (int i = 0; i < str.Length; i += n)
			{
				if (str.Length - i > n)
				{
					yield return str.Substring(i, n);
				}
				else
				{
					yield return str.Substring(i, str.Length - i);
				}

			}
		}
	}
}
