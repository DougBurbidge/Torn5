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

TODO for BOTH:
output to screen and printer
send to scoreboard (web browser)
on commit auto-update scoreboard, teams and handicaps
right-click handicap player, merge player
adjust team score/victory points

TODO for LEAGUE:
read from laserforce server
handicap
set up fixtures

TODO for ZLTAC:
group players by alias/colour/LotR
set up pyramid round
recalculate scores on Helios

OTHER:
better save format
read from demo "server"
sort teams by rank/colour
league copy from
upload to http, https, sftp
Space Marines match play
read from Ozone server
spark lines
check latest version via REST
reports and uploads in worker thread

*/

namespace Torn.UI
{
	/// <summary>Torn 5 Main Form.</summary>
	public partial class MainForm : Form
	{
		GroupPlayersBy groupPlayersBy = GroupPlayersBy.Colour;
		//SortTeamsBy sortTeamsBy;
		SystemType systemType;
		string serverAddress = "localhost";

		WebOutput webOutput;
		LaserGameServer laserGameServer;
		static Holders leagues;
		Holder activeHolder;  // This is the league selected in the listView.

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
			//leagues = new Dictionary<string, League>();
			leagues = new Holders();

			if (Properties.Torn5.Default.UpgradeRequired)
			{
				Properties.Torn5.Default.Upgrade();
				Properties.Torn5.Default.UpgradeRequired = false;
				Properties.Torn5.Default.Save();
			}

			toolStripTeams.Location = new Point(3, 24);
			toolStripGame.Location = new Point(2, 24);
			toolStripLeague.Location = new Point(1, 24);

			//Properties.TornWeb.Default.Reload();
			numericPort.Value = Properties.Torn5.Default.Port;

			var serializedDictionary = Properties.Torn5.Default.Leagues.Split('|');
			foreach (string entry in serializedDictionary)
			{
				Holder holder;
				var keyValue = entry.Split('?');
				if (keyValue.Length > 1)
				{
					holder = AddLeague(keyValue[1], keyValue[0]);

					if (keyValue.Length > 2)
						holder.ReportTemplates.Parse(keyValue[2]);
					holder.Fixture.Teams.Parse(holder.League);
					if (keyValue.Length > 3)
						holder.Fixture.Games.Parse(keyValue[3], holder.Fixture.Teams, '\t');
				}
			}

			webOutput = new WebOutput((int)numericPort.Value);
			webOutput.Leagues = leagues;
	        laserGameServer = new PAndC(serverAddress);

	        playersBox = new PlayersBox();
			playersBox.Images = imageListPacks;
			playersBox.GetMoveTarget = FindEmptyTeamBox;
			tableLayoutPanel1.Controls.Add(playersBox, 2, 0);
			tableLayoutPanel1.SetRowSpan(playersBox, tableLayoutPanel1.RowCount);
			playersBox.Dock = DockStyle.Fill;
			
			formPlayer = new FormPlayer();
			formPlayer.LaserGameServer = laserGameServer;

	        RefreshGamesList();
			AddTeamBoxes();

			listViewLeagues.Focus();
			if (listViewLeagues.Items.Count > 0)
				listViewLeagues.SelectedIndices.Add(0);
		}

		void MainFormFormClosing(object sender, FormClosingEventArgs e)
		{
			webOutput.Dispose();
			laserGameServer.Dispose();

			Properties.Torn5.Default.Port = (int)numericPort.Value;

			StringBuilder sb = new StringBuilder();
			foreach (Holder holder in leagues)
			{
				sb.Append(holder.Key);
				sb.Append('?');
				sb.Append(holder.FileName);
				if (holder.ReportTemplates.Count > 0 || holder.Fixture.Games.Count() > 0)
				{
					sb.Append('?');
					sb.Append(holder.ReportTemplates.ToString());
				}
				if (holder.Fixture.Games.Count() > 0)
				{
					sb.Append('?');
					sb.Append(holder.Fixture.Games.ToString());
				}
				sb.Append('|');
			}
			Properties.Torn5.Default.Leagues = sb.ToString();
			Properties.Torn5.Default.Save();
		}

		Holder AddLeague(string fileName, string key = "")
		{
			League league = new League();
			ListViewItem item = new ListViewItem();
			item.ImageKey = "tick";
			try
			{
				league.Load(fileName);
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
			item.Text = key;
			item.SubItems.AddRange(new string[] { league.Title, fileName, league.AllGames.Count.ToString(CultureInfo.CurrentCulture), league.Teams.Count.ToString(CultureInfo.CurrentCulture)});
			listViewLeagues.Items.Add(item);
		
			return holder;
		}

		void SetRowSpans(int adjust)
		{
			tableLayoutPanel1.SetCellPosition(panelLeague, new TableLayoutPanelCellPosition(0, (tableLayoutPanel1.RowCount + adjust) / 2));
			tableLayoutPanel1.SetRowSpan(listViewLeagues, (tableLayoutPanel1.RowCount + adjust) / 2);
			tableLayoutPanel1.SetRowSpan(panelLeague, (tableLayoutPanel1.RowCount + adjust + 1) / 2);
			tableLayoutPanel1.SetRowSpan(panelGames, tableLayoutPanel1.RowCount + adjust);
			tableLayoutPanel1.SetRowSpan(playersBox, tableLayoutPanel1.RowCount + adjust);
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

		void SetRowColumnCount(int rows, int columns)
		{
			for (int i = rows * columns + 3; i < tableLayoutPanel1.Controls.Count; )
				tableLayoutPanel1.Controls.RemoveAt(tableLayoutPanel1.Controls.Count - 1);

			if (rows < tableLayoutPanel1.RowCount)
				SetRowSpans(rows - tableLayoutPanel1.RowCount);

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
		}

		void ButtonEditReportsClick(object sender, EventArgs e)
		{
			if (listViewLeagues.SelectedItems.Count > 0)
			{
				var item = listViewLeagues.SelectedItems[0];
				using (var r = new FormReports((Holder)item.Tag))
					r.ShowDialog();
			}
		}

		void ButtonForceClick(object sender, EventArgs e) {}

		void ButtonLoadClick(object sender, EventArgs e)
		{
			if (openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				foreach (string fileName in openFileDialog1.FileNames)
					AddLeague(fileName);

				RefreshGamesList();
			}
		}

		void ButtonSaveClick(object sender, EventArgs e)
		{
			foreach (ListViewItem item in listViewLeagues.SelectedItems)
				((Holder)item.Tag).League.Save();
		}

		void ButtonNewClick(object sender, EventArgs e)
		{
			if (saveFileDialog1.ShowDialog() == DialogResult.OK)
			{
				foreach (string fileName in saveFileDialog1.FileNames)
					AddLeague(fileName);

				RefreshGamesList();
			}			
		}

		string GetExportFolder()
		{
			if (listViewLeagues.SelectedItems.Count == 1)
				folderBrowserDialog1.Description = "Select a root folder for bulk export of " + listViewLeagues.SelectedItems[0].SubItems[1].Text;
			else
				folderBrowserDialog1.Description = "Select a root folder for bulk export of the " + listViewLeagues.SelectedItems.Count.ToString(CultureInfo.CurrentCulture) + " selected leagues.";

			return (folderBrowserDialog1.ShowDialog() == DialogResult.OK) ? folderBrowserDialog1.SelectedPath : null;
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

		void ButtonExportClick(object sender, EventArgs e)
		{
			bool includeSecret = IncludeSecret(); // This call has to be before the GetExportFolder() call -- it queries the keyboard state.

			string path = GetExportFolder();
			if (path != null)
				webOutput.ExportReports(path, includeSecret, SelectedLeagues());
		}

		void ButtonTsvExportClick(object sender, EventArgs e)
		{
			bool includeSecret = IncludeSecret();

			string path = GetExportFolder();
			if (path != null)
				webOutput.ExportReportsAsTsv(path, includeSecret, SelectedLeagues());
		}

		void ButtonFixtureClick(object sender, EventArgs e)
		{
			if (listViewLeagues.SelectedItems.Count == 1)
			{
				var item = listViewLeagues.SelectedItems[0];
				using (var f = new FormFixture(((Holder)item.Tag).Fixture, ((Holder)item.Tag).League))
					f.ShowDialog();
			}
		}

		void ButtonExportFixturesClick(object sender, EventArgs e)
		{
			string path = GetExportFolder();
			if (path != null)
				webOutput.ExportFixtures(path, SelectedLeagues());
		}

		void ButtonUploadClick(object sender, EventArgs e)
		{
			webOutput.UploadFiles(folderBrowserDialog1.SelectedPath, IncludeSecret(), SelectedLeagues());
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
			if (listViewGames.SelectedItems.Count == 1)
			{
				ServerGame game = ((ServerGame)listViewGames.SelectedItems[0].Tag);
				if (game.Game == null)  // Game does not exist; create one.
				{
					game.Game = new Game();
					game.Game.Time = game.Time;
					foreach (Control c in tableLayoutPanel1.Controls)
						if (c is TeamBox && ((TeamBox)c).Items.Count > 0)
						{
							var gameTeam = ((TeamBox)c).GameTeam; //.CopyTo(new GameTeam());
							foreach (var player in ((TeamBox)c).Players())
							{
								var gamePlayer = player; //.CopyTo(new GamePlayer());
								gamePlayer.GameTeam = gameTeam;
								if (gamePlayer.LeaguePlayer == null)
									gamePlayer.LeaguePlayer = activeHolder.League.Players.Find(p => p.Id == gamePlayer.PlayerId);
								game.Game.Players.Add(gamePlayer);
								gamePlayer.LeaguePlayer.Played.Add(gamePlayer);
							}
							gameTeam.Players.Sort();
							game.Game.Teams.Add(gameTeam);
						}
					game.Game.Teams.Sort();
					activeHolder.League.AllGames.Add(game.Game);
				}
				else  // Game already exists; revise the existing one.
				{
					game.Game.Time = game.Time;
					foreach (Control c in tableLayoutPanel1.Controls)
						if (c is TeamBox && ((TeamBox)c).Items.Count > 0)
						{
							var gameTeam = game.Game.Teams.Find(gt => gt.LeagueTeam == ((TeamBox)c).LeagueTeam);
							if (gameTeam == null)
							{
								new GameTeam();
								game.Game.Teams.Add(gameTeam);
							}

							foreach (var player in ((TeamBox)c).Players())
							{
								var gamePlayer = game.Game.Players.Find(gp => gp.PlayerId == player.PlayerId) ?? player.CopyTo(new GamePlayer());
								gamePlayer.GameTeam = gameTeam;
								if (gamePlayer.LeaguePlayer == null)
									gamePlayer.LeaguePlayer = activeHolder.League.Players.Find(p => p.Id == gamePlayer.PlayerId);
								gamePlayer.LeaguePlayer.Played.Add(gamePlayer);
							}
							gameTeam.Players.Sort();
						}
				}
				RefreshGamesList();
				RankTeams();
			}
		}

		void ButtonForgetClick(object sender, EventArgs e)
		{
			if (listViewGames.SelectedItems.Count == 0)
				return;

			foreach (ListViewItem item in listViewGames.SelectedItems)
				if (item.Tag is ServerGame && ((ServerGame)item.Tag).Game != null)
				{
					Game game = ((ServerGame)item.Tag).Game;
					var holder = leagues.Find(h => h.League.AllGames.Contains(game));
					if (holder != null)
					{
						holder.League.AllGames.Remove(game);
						item.SubItems[1].Text = null;
					}
					((ServerGame)item.Tag).Game = null;
				}

			playersBox.Clear();
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
			}
		}

		void ButtonPackReportClick(object sender, EventArgs e)
		{
			string path = GetExportFolder();
			if (path != null)
			{
				var leagues = new List<League>();

				foreach (ListViewItem item in listViewLeagues.SelectedItems)
					leagues.Add(((Holder)item.Tag).League);

				webOutput.PackReport(path, leagues);
			}
		}

		void ButtonSetDescriptionClick(object sender, EventArgs e)
		{
			if (listViewGames.SelectedItems.Count == 0)
				return;

			var id = new InputDialog("Description: ", "Set a description", listViewGames.SelectedItems[0].SubItems[2].Text);
			if (id.ShowDialog() == DialogResult.OK)
				foreach (ListViewItem item in listViewGames.SelectedItems)
					if (item.Tag is ServerGame && ((ServerGame)item.Tag).Game != null)
					{
						((ServerGame)item.Tag).Game.Title = id.Response;
						item.SubItems[2].Text = id.Response;
					}
		}

		void ButtonPreferencesClick(object sender, EventArgs e)
		{
			var form = new FormPreferences();
			form.GroupPlayersBy = groupPlayersBy;
			form.SystemType = systemType;
			form.ServerAddress = serverAddress;
			if (form.ShowDialog() == DialogResult.OK)
			{
				groupPlayersBy = form.GroupPlayersBy;
				systemType = form.SystemType;
				serverAddress = form.ServerAddress;
				laserGameServer = new PAndC(serverAddress);
			}
		}

		TeamBox FindEmptyTeamBox()
		{
			foreach (Control c in tableLayoutPanel1.Controls)
				if (c is TeamBox && ((TeamBox)c).Items.Count == 0)
					return (TeamBox)c;

			return null;
		}

		void TransferPlayers(ServerGame game)
		{
			League league = game.League ?? (listViewLeagues.SelectedItems.Count == 1 ? ((Holder)listViewLeagues.SelectedItems[0].Tag).League : null);
			if (league == null)
				return;

			var teamBoxes = new List<TeamBox>();
			foreach (Control c in tableLayoutPanel1.Controls)
				if (c is TeamBox)
					teamBoxes.Add((TeamBox)c);

			int box = 0;

			if (game.Game == null)  // This game is not yet committed. Match players to league teams.
			{
				if (groupPlayersBy == GroupPlayersBy.Colour)
					foreach (var colour in (Colour[])Enum.GetValues(typeof(Colour)))
					{
						var serverPlayers = playersBox.Players().FindAll(p => p.Colour == colour).ToList();
						if (serverPlayers.Count > 0 && box < teamBoxes.Count)
							teamBoxes[box++].Accept(serverPlayers);
					}
				else  // Alias or LotR
					foreach (var team in league.Teams)
					{
						var serverPlayers = playersBox.Players().FindAll(p => team.Players.Exists(p2 => p.PlayerId == p2.Id)).ToList();
						if (serverPlayers.Count > 0 && box < teamBoxes.Count)
							teamBoxes[box++].Accept(serverPlayers);
					}
			}
			else  // This game is previously committed. Match game players to game teams.
			{
				var leagueGame = game.Game;
				foreach (var gameTeam in leagueGame.Teams)
				{
					var teamPlayers = leagueGame.Players.Where(p => p.GameTeam == gameTeam).ToList();
					var serverPlayers = playersBox.Players().FindAll(sp => teamPlayers.Exists(gp => sp.PlayerId == gp.PlayerId)).ToList();
					if (serverPlayers.Count > 0 && box < teamBoxes.Count)
						teamBoxes[box++].Accept(serverPlayers);
				}
			}

			RankTeamBoxes();
		}

		void ListViewGamesSelectedIndexChanged(object sender, EventArgs e)
		{
			buttonSetDescription.Enabled = listViewGames.SelectedItems.Count > 0;
			buttonForget.Enabled = listViewGames.SelectedItems.Count > 0;
			buttonCommit.Enabled = listViewGames.SelectedItems.Count == 1;
			buttonCommit2.Enabled = listViewGames.SelectedItems.Count == 1;
			
			buttonForget.Text = "Forget Game" + (listViewGames.SelectedItems.Count > 1 ? "s" : "");

			if (listViewGames.SelectedItems.Count == 1)
			{
				ServerGame game = ((ServerGame)listViewGames.SelectedItems[0].Tag);

				laserGameServer.PopulateGame(game);
				playersBox.LoadGame(game);

				foreach (Control c in tableLayoutPanel1.Controls)
					if (c is TeamBox)
						((TeamBox)c).Clear();
	
				TransferPlayers(game);
			}
			else
			{
				playersBox.Clear();

				foreach (Control c in tableLayoutPanel1.Controls)
					if (c is TeamBox)
						((TeamBox)c).Clear();
			}
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

			menuCloseLeague.Text = "&Close League" + s;
			menuSaveLeague.Text = "&Save League" + s;
			
			buttonClose.Enabled = b;
			menuCloseLeague.Enabled = b;
			buttonSave.Enabled = b;
			menuSaveLeague.Enabled = b;
			menuExportReports.Enabled = b;
			buttonTsvExport.Enabled = b;
			menuBulkUploadReports.Enabled = b;
			menuPackReport.Enabled = b;
			menuConfigureReports.Enabled = listViewLeagues.SelectedItems.Count == 1;

			if (listViewLeagues.SelectedItems.Count == 1)
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
					sb.Append(item.SubItems[1].Text + ", ");
				sb.Remove(sb.Length - 2, 2);
				labelLeagueDetails.Text = sb.ToString();
			}

			foreach (var c in tableLayoutPanel1.Controls)
				if (c is TeamBox)
					((TeamBox)c).League = activeHolder == null ? null : activeHolder.League;
		}

		TimeSpan lastDBCheck = TimeSpan.Zero;
		TimeSpan timeElapsed = TimeSpan.Zero;

		void TimerGameTick(object sender, EventArgs e)
		{
			if (lastDBCheck <= TimeSpan.Zero)
			{
				timeElapsed = laserGameServer.GameTimeElapsed();
				lastDBCheck = new TimeSpan(0, 1, 0);  // Only query the database for time elapsed once per minute. Outside that, dead-reckon.
			}
			else
			{
				timeElapsed.Subtract(new TimeSpan(timerGame.Interval * 10000));  // The *10000 converts from timer ticks (1 millisecond) to TimeSpan ticks (100 nanoseconds).
				lastDBCheck.Subtract(new TimeSpan(timerGame.Interval * 10000));
			}

			if (timeElapsed == TimeSpan.Zero)
				labelTime.Text = "Idle";
			else if (timeElapsed.TotalHours < 1)
				labelTime.Text = timeElapsed.ToString("m\\:ss");
			else
				labelTime.Text = timeElapsed.ToString();

			webOutput.Tick(timeElapsed);
			labelNow.Text = webOutput.NowText();
		}

		void NumericPortValueChanged(object sender, EventArgs e)
		{
			webOutput.Restart((int)numericPort.Value);
		}

		void RankTeamBoxes()
		{
			RankTeams();
		}

		List<TeamBox> RankTeams()
		{
			var teams = new List<TeamBox>();
			foreach (var c in tableLayoutPanel1.Controls)
				if (c is TeamBox)
					teams.Add((TeamBox)c);

			teams.Sort((x, y) => y.Score - x.Score);

			for (int i = 0; i < teams.Count; i++)
				teams[i].Rank = i + 1;

			return teams;
		}

		void ArrangeTeamsByRank()
		{
			var teams = RankTeams();

			foreach(var team in teams)
				tableLayoutPanel1.Controls.Remove(team);

			foreach(var team in teams)
				tableLayoutPanel1.Controls.Add(team);
		}

		string FriendlyDate(DateTime date)
		{
			int daysAgo = (int)DateTime.Now.Date.Subtract(date.Date).TotalDays;

			if (daysAgo == 0) return "today";
			if (daysAgo == 1) return "yesterday";
			if (daysAgo > 0 && daysAgo < 7) return date.DayOfWeek.ToString();
			return date.ToShortDateString();
		}

		void RefreshGamesList()
		{
			var focused = listViewGames.FocusedItem;
			List<ServerGame> games = laserGameServer.GetGames();

			if (games.Count > 0)
				games.LastOrDefault(x => x.InProgress == false).InProgress = timeElapsed > TimeSpan.Zero;

			// Link server games to league games, where a matching league game exists.
			foreach (var serverGame in games)
				foreach (Holder holder in leagues)
				{
					Game leagueGame = holder.League.AllGames.Find(x => x.Time == serverGame.Time);
					if (leagueGame != null)
					{
						serverGame.League = holder.League;
						serverGame.Game = leagueGame;
					}
				}

			// Create fake "server" games to represent league games, where a server game doesn't already exist (because Acacia has forgotten it).
			foreach (Holder holder in leagues)
				foreach (Game leagueGame in holder.League.AllGames)
					if (games.Find(x => x.Time == leagueGame.Time) == null)
					{
						ServerGame game = new ServerGame();
						game.League = holder.League;
						game.Game = leagueGame;
						game.Time = leagueGame.Time;
						games.Add(game);
					}

			games.Sort();

			listViewGames.BeginUpdate();
			try
			{
				listViewGames.Items.Clear();

				// Create items in the list view, one for each server game.
				foreach (var serverGame in games)
				{
					ListViewItem item = new ListViewItem();
					item.Text = FriendlyDate(serverGame.Time) + serverGame.Time.ToString(" HH:mm");

					item.SubItems.AddRange(new string[] { serverGame.League == null ? "" : serverGame.League.Title,
						                       	serverGame.Game == null ? serverGame.Description : serverGame.Game.Title });
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
			
			// Restore focus to the same item it was on before we started.
			if (focused != null && focused.Tag is ServerGame)
			{
				var old = ((ServerGame)focused.Tag).Time;
				foreach (ListViewItem item in listViewGames.Items)
					if (((ServerGame)item.Tag).Time == old)
					{
						listViewGames.FocusedItem = item;
						listViewGames.SelectedIndices.Add(item.Index);
					}
			}
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

		void MenuEditLeagueClick(object sender, EventArgs e)
		{
			var form = new FormLeague();
			form.League = activeHolder.League.Clone();
			form.FormPlayer = formPlayer;
			if (form.ShowDialog() == DialogResult.OK)
			{
				activeHolder.League = form.League;
				activeHolder.League.Save();
			}
		}
	}
}
