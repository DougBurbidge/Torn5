/*
 * Created by SharpDevelop.
 * User: Doug
 * Date: 1/09/2017
 * Time: 9:01 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace Torn.UI
{
	partial class MainForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
			this.imageListLeagues = new System.Windows.Forms.ImageList(this.components);
			this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.listViewLeagues = new System.Windows.Forms.ListView();
			this.colTag = new System.Windows.Forms.ColumnHeader();
			this.colTitle = new System.Windows.Forms.ColumnHeader();
			this.colFile = new System.Windows.Forms.ColumnHeader();
			this.colGames = new System.Windows.Forms.ColumnHeader();
			this.colTeams = new System.Windows.Forms.ColumnHeader();
			this.panelGames = new System.Windows.Forms.Panel();
			this.buttonCommit2 = new System.Windows.Forms.Button();
			this.buttonLatestGame2 = new System.Windows.Forms.Button();
			this.listViewGames = new System.Windows.Forms.ListView();
			this.colGame = new System.Windows.Forms.ColumnHeader();
			this.colLeague = new System.Windows.Forms.ColumnHeader();
			this.colDescription = new System.Windows.Forms.ColumnHeader();
			this.contextMenuStripGames = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.latestGameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.commitGameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.setDescriptionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.forgetGameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.panelLeague = new System.Windows.Forms.Panel();
			this.labelNow = new System.Windows.Forms.Label();
			this.buttonTsvExport = new System.Windows.Forms.Button();
			this.labelTime = new System.Windows.Forms.Label();
			this.numericPort = new System.Windows.Forms.NumericUpDown();
			this.labelLeagueDetails = new System.Windows.Forms.Label();
			this.progressBar1 = new System.Windows.Forms.ProgressBar();
			this.buttonExportFixtures = new System.Windows.Forms.Button();
			this.labelStatus = new System.Windows.Forms.Label();
			this.menuStripMain = new System.Windows.Forms.MenuStrip();
			this.menuLeague = new System.Windows.Forms.ToolStripMenuItem();
			this.menuNewLeague = new System.Windows.Forms.ToolStripMenuItem();
			this.menuOpenLeague = new System.Windows.Forms.ToolStripMenuItem();
			this.menuSaveLeague = new System.Windows.Forms.ToolStripMenuItem();
			this.menuCloseLeague = new System.Windows.Forms.ToolStripMenuItem();
			this.menuEditLeague = new System.Windows.Forms.ToolStripMenuItem();
			this.menuSplitter1 = new System.Windows.Forms.ToolStripSeparator();
			this.menuPyramid = new System.Windows.Forms.ToolStripMenuItem();
			this.menuFixtures = new System.Windows.Forms.ToolStripMenuItem();
			this.menuRememberAllTeams = new System.Windows.Forms.ToolStripMenuItem();
			this.menuMatch = new System.Windows.Forms.ToolStripMenuItem();
			this.menuSplitter2 = new System.Windows.Forms.ToolStripSeparator();
			this.menuPreferences = new System.Windows.Forms.ToolStripMenuItem();
			this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.menuSplitter3 = new System.Windows.Forms.ToolStripSeparator();
			this.menuReport = new System.Windows.Forms.ToolStripMenuItem();
			this.menuAdHocReport = new System.Windows.Forms.ToolStripMenuItem();
			this.menuExportReports = new System.Windows.Forms.ToolStripMenuItem();
			this.menuBulkUploadReports = new System.Windows.Forms.ToolStripMenuItem();
			this.menuConfigureReports = new System.Windows.Forms.ToolStripMenuItem();
			this.menuPackReport = new System.Windows.Forms.ToolStripMenuItem();
			this.updateScoreboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripLeague = new System.Windows.Forms.ToolStrip();
			this.toolStripLabelLeagues = new System.Windows.Forms.ToolStripLabel();
			this.buttonNew = new System.Windows.Forms.ToolStripButton();
			this.buttonOpen = new System.Windows.Forms.ToolStripButton();
			this.buttonSave = new System.Windows.Forms.ToolStripButton();
			this.buttonClose = new System.Windows.Forms.ToolStripButton();
			this.buttonEdit = new System.Windows.Forms.ToolStripButton();
			this.toolStripTeams = new System.Windows.Forms.ToolStrip();
			this.toolStripLabelTeams = new System.Windows.Forms.ToolStripLabel();
			this.buttonAddRow = new System.Windows.Forms.ToolStripButton();
			this.buttonRemoveRow = new System.Windows.Forms.ToolStripButton();
			this.buttonAddColumn = new System.Windows.Forms.ToolStripButton();
			this.buttonRemoveColumn = new System.Windows.Forms.ToolStripButton();
			this.toolStripGame = new System.Windows.Forms.ToolStrip();
			this.toolStripLabelGames = new System.Windows.Forms.ToolStripLabel();
			this.buttonLatestGame = new System.Windows.Forms.ToolStripButton();
			this.buttonCommit = new System.Windows.Forms.ToolStripButton();
			this.buttonCreateGame = new System.Windows.Forms.ToolStripButton();
			this.buttonEditGame = new System.Windows.Forms.ToolStripButton();
			this.buttonSetDescription = new System.Windows.Forms.ToolStripButton();
			this.buttonForget = new System.Windows.Forms.ToolStripButton();
			this.imageListPacks = new System.Windows.Forms.ImageList(this.components);
			this.timerGame = new System.Windows.Forms.Timer(this.components);
			this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
			this.toolStripContainer1.ContentPanel.SuspendLayout();
			this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
			this.toolStripContainer1.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.panelGames.SuspendLayout();
			this.contextMenuStripGames.SuspendLayout();
			this.panelLeague.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericPort)).BeginInit();
			this.menuStripMain.SuspendLayout();
			this.toolStripLeague.SuspendLayout();
			this.toolStripTeams.SuspendLayout();
			this.toolStripGame.SuspendLayout();
			this.SuspendLayout();
			// 
			// openFileDialog1
			// 
			this.openFileDialog1.FileName = "openFileDialog1";
			this.openFileDialog1.Filter = "Torn files|*.Torn|All files|*.*";
			this.openFileDialog1.Multiselect = true;
			// 
			// imageListLeagues
			// 
			this.imageListLeagues.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListLeagues.ImageStream")));
			this.imageListLeagues.TransparentColor = System.Drawing.Color.Transparent;
			this.imageListLeagues.Images.SetKeyName(0, "tick");
			this.imageListLeagues.Images.SetKeyName(1, "cross");
			// 
			// toolStripContainer1
			// 
			this.toolStripContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			// 
			// toolStripContainer1.ContentPanel
			// 
			this.toolStripContainer1.ContentPanel.Controls.Add(this.tableLayoutPanel1);
			this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(1272, 617);
			this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
			this.toolStripContainer1.Name = "toolStripContainer1";
			this.toolStripContainer1.Size = new System.Drawing.Size(1272, 716);
			this.toolStripContainer1.TabIndex = 21;
			this.toolStripContainer1.Text = "toolStripContainer1";
			// 
			// toolStripContainer1.TopToolStripPanel
			// 
			this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.menuStripMain);
			this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStripLeague);
			this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStripTeams);
			this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStripGame);
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 4;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.tableLayoutPanel1.Controls.Add(this.listViewLeagues, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.panelGames, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.panelLeague, 0, 1);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(1272, 617);
			this.tableLayoutPanel1.TabIndex = 20;
			// 
			// listViewLeagues
			// 
			this.listViewLeagues.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
									this.colTag,
									this.colTitle,
									this.colFile,
									this.colGames,
									this.colTeams});
			this.listViewLeagues.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listViewLeagues.HideSelection = false;
			this.listViewLeagues.LabelEdit = true;
			this.listViewLeagues.Location = new System.Drawing.Point(3, 3);
			this.listViewLeagues.Name = "listViewLeagues";
			this.listViewLeagues.Size = new System.Drawing.Size(312, 302);
			this.listViewLeagues.SmallImageList = this.imageListLeagues;
			this.listViewLeagues.TabIndex = 11;
			this.listViewLeagues.UseCompatibleStateImageBehavior = false;
			this.listViewLeagues.View = System.Windows.Forms.View.Details;
			this.listViewLeagues.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.ListViewLeaguesAfterLabelEdit);
			this.listViewLeagues.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.ListViewLeaguesItemSelectionChanged);
			// 
			// colTag
			// 
			this.colTag.Text = "Tag";
			this.colTag.Width = 100;
			// 
			// colTitle
			// 
			this.colTitle.Text = "League";
			this.colTitle.Width = 200;
			// 
			// colFile
			// 
			this.colFile.Text = "File";
			this.colFile.Width = 350;
			// 
			// colGames
			// 
			this.colGames.Text = "Games";
			this.colGames.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.colGames.Width = 50;
			// 
			// colTeams
			// 
			this.colTeams.Text = "Teams";
			this.colTeams.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.colTeams.Width = 50;
			// 
			// panelGames
			// 
			this.panelGames.Controls.Add(this.buttonCommit2);
			this.panelGames.Controls.Add(this.buttonLatestGame2);
			this.panelGames.Controls.Add(this.listViewGames);
			this.panelGames.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelGames.Location = new System.Drawing.Point(318, 0);
			this.panelGames.Margin = new System.Windows.Forms.Padding(0);
			this.panelGames.Name = "panelGames";
			this.tableLayoutPanel1.SetRowSpan(this.panelGames, 2);
			this.panelGames.Size = new System.Drawing.Size(318, 617);
			this.panelGames.TabIndex = 15;
			// 
			// buttonCommit2
			// 
			this.buttonCommit2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonCommit2.Location = new System.Drawing.Point(90, 577);
			this.buttonCommit2.Name = "buttonCommit2";
			this.buttonCommit2.Size = new System.Drawing.Size(81, 34);
			this.buttonCommit2.TabIndex = 16;
			this.buttonCommit2.Text = "Commit Game";
			this.buttonCommit2.UseVisualStyleBackColor = true;
			this.buttonCommit2.Visible = false;
			this.buttonCommit2.Click += new System.EventHandler(this.ButtonCommitClick);
			// 
			// buttonLatestGame2
			// 
			this.buttonLatestGame2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonLatestGame2.Location = new System.Drawing.Point(3, 577);
			this.buttonLatestGame2.Name = "buttonLatestGame2";
			this.buttonLatestGame2.Size = new System.Drawing.Size(81, 34);
			this.buttonLatestGame2.TabIndex = 13;
			this.buttonLatestGame2.Text = "Latest Game";
			this.buttonLatestGame2.UseVisualStyleBackColor = true;
			this.buttonLatestGame2.Visible = false;
			this.buttonLatestGame2.Click += new System.EventHandler(this.ButtonLatestGameClick);
			// 
			// listViewGames
			// 
			this.listViewGames.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.listViewGames.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
									this.colGame,
									this.colLeague,
									this.colDescription});
			this.listViewGames.ContextMenuStrip = this.contextMenuStripGames;
			this.listViewGames.FullRowSelect = true;
			this.listViewGames.HideSelection = false;
			this.listViewGames.Location = new System.Drawing.Point(3, 3);
			this.listViewGames.Name = "listViewGames";
			this.listViewGames.Size = new System.Drawing.Size(315, 611);
			this.listViewGames.TabIndex = 12;
			this.listViewGames.UseCompatibleStateImageBehavior = false;
			this.listViewGames.View = System.Windows.Forms.View.Details;
			this.listViewGames.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.ListViewGamesDrawItem);
			this.listViewGames.DrawSubItem += new System.Windows.Forms.DrawListViewSubItemEventHandler(this.ListViewGamesDrawSubItem);
			this.listViewGames.SelectedIndexChanged += new System.EventHandler(this.ListViewGamesSelectedIndexChanged);
			// 
			// colGame
			// 
			this.colGame.Text = "Game Time";
			this.colGame.Width = 100;
			// 
			// colLeague
			// 
			this.colLeague.Text = "League";
			// 
			// colDescription
			// 
			this.colDescription.Text = "Description";
			this.colDescription.Width = 70;
			// 
			// contextMenuStripGames
			// 
			this.contextMenuStripGames.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.latestGameToolStripMenuItem,
									this.commitGameToolStripMenuItem,
									this.setDescriptionToolStripMenuItem,
									this.forgetGameToolStripMenuItem});
			this.contextMenuStripGames.Name = "contextMenuStripGames";
			this.contextMenuStripGames.Size = new System.Drawing.Size(176, 92);
			// 
			// latestGameToolStripMenuItem
			// 
			this.latestGameToolStripMenuItem.Name = "latestGameToolStripMenuItem";
			this.latestGameToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
			this.latestGameToolStripMenuItem.Text = "&Latest Game";
			this.latestGameToolStripMenuItem.Click += new System.EventHandler(this.ButtonLatestGameClick);
			// 
			// commitGameToolStripMenuItem
			// 
			this.commitGameToolStripMenuItem.Name = "commitGameToolStripMenuItem";
			this.commitGameToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
			this.commitGameToolStripMenuItem.Text = "&Commit Game";
			this.commitGameToolStripMenuItem.Click += new System.EventHandler(this.ButtonCommitClick);
			// 
			// setDescriptionToolStripMenuItem
			// 
			this.setDescriptionToolStripMenuItem.Name = "setDescriptionToolStripMenuItem";
			this.setDescriptionToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
			this.setDescriptionToolStripMenuItem.Text = "Set Description";
			this.setDescriptionToolStripMenuItem.Click += new System.EventHandler(this.ButtonSetDescriptionClick);
			// 
			// forgetGameToolStripMenuItem
			// 
			this.forgetGameToolStripMenuItem.Name = "forgetGameToolStripMenuItem";
			this.forgetGameToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
			this.forgetGameToolStripMenuItem.Text = "Forget Game";
			this.forgetGameToolStripMenuItem.Click += new System.EventHandler(this.ButtonForgetClick);
			// 
			// panelLeague
			// 
			this.panelLeague.Controls.Add(this.labelNow);
			this.panelLeague.Controls.Add(this.buttonTsvExport);
			this.panelLeague.Controls.Add(this.labelTime);
			this.panelLeague.Controls.Add(this.numericPort);
			this.panelLeague.Controls.Add(this.labelLeagueDetails);
			this.panelLeague.Controls.Add(this.progressBar1);
			this.panelLeague.Controls.Add(this.buttonExportFixtures);
			this.panelLeague.Controls.Add(this.labelStatus);
			this.panelLeague.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelLeague.Location = new System.Drawing.Point(3, 311);
			this.panelLeague.Name = "panelLeague";
			this.panelLeague.Size = new System.Drawing.Size(312, 303);
			this.panelLeague.TabIndex = 16;
			// 
			// labelNow
			// 
			this.labelNow.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.labelNow.Location = new System.Drawing.Point(12, 180);
			this.labelNow.Name = "labelNow";
			this.labelNow.Size = new System.Drawing.Size(297, 64);
			this.labelNow.TabIndex = 1;
			this.labelNow.Text = "Now Playing:";
			// 
			// buttonTsvExport
			// 
			this.buttonTsvExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonTsvExport.Location = new System.Drawing.Point(118, 107);
			this.buttonTsvExport.Name = "buttonTsvExport";
			this.buttonTsvExport.Size = new System.Drawing.Size(100, 23);
			this.buttonTsvExport.TabIndex = 23;
			this.buttonTsvExport.Text = "TSV Export";
			this.buttonTsvExport.UseVisualStyleBackColor = true;
			this.buttonTsvExport.Click += new System.EventHandler(this.ButtonTsvExportClick);
			// 
			// labelTime
			// 
			this.labelTime.Location = new System.Drawing.Point(12, 157);
			this.labelTime.Name = "labelTime";
			this.labelTime.Size = new System.Drawing.Size(100, 23);
			this.labelTime.TabIndex = 0;
			this.labelTime.Text = "Time";
			// 
			// numericPort
			// 
			this.numericPort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.numericPort.Location = new System.Drawing.Point(118, 136);
			this.numericPort.Maximum = new decimal(new int[] {
									65535,
									0,
									0,
									0});
			this.numericPort.Minimum = new decimal(new int[] {
									1,
									0,
									0,
									0});
			this.numericPort.Name = "numericPort";
			this.numericPort.Size = new System.Drawing.Size(60, 20);
			this.numericPort.TabIndex = 10;
			this.numericPort.Value = new decimal(new int[] {
									8080,
									0,
									0,
									0});
			this.numericPort.ValueChanged += new System.EventHandler(this.NumericPortValueChanged);
			// 
			// labelLeagueDetails
			// 
			this.labelLeagueDetails.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.labelLeagueDetails.Location = new System.Drawing.Point(0, 0);
			this.labelLeagueDetails.Name = "labelLeagueDetails";
			this.labelLeagueDetails.Size = new System.Drawing.Size(312, 95);
			this.labelLeagueDetails.TabIndex = 13;
			this.labelLeagueDetails.Text = "Select a league and its details will appear here.";
			// 
			// progressBar1
			// 
			this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.progressBar1.Location = new System.Drawing.Point(3, 277);
			this.progressBar1.Maximum = 1000;
			this.progressBar1.Name = "progressBar1";
			this.progressBar1.Size = new System.Drawing.Size(286, 23);
			this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			this.progressBar1.TabIndex = 19;
			this.progressBar1.Visible = false;
			// 
			// buttonExportFixtures
			// 
			this.buttonExportFixtures.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonExportFixtures.Location = new System.Drawing.Point(12, 107);
			this.buttonExportFixtures.Name = "buttonExportFixtures";
			this.buttonExportFixtures.Size = new System.Drawing.Size(100, 23);
			this.buttonExportFixtures.TabIndex = 16;
			this.buttonExportFixtures.Text = "Export Fixtures";
			this.buttonExportFixtures.UseVisualStyleBackColor = true;
			this.buttonExportFixtures.Click += new System.EventHandler(this.ButtonExportFixturesClick);
			// 
			// labelStatus
			// 
			this.labelStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.labelStatus.Location = new System.Drawing.Point(3, 253);
			this.labelStatus.Name = "labelStatus";
			this.labelStatus.Size = new System.Drawing.Size(306, 21);
			this.labelStatus.TabIndex = 18;
			// 
			// menuStripMain
			// 
			this.menuStripMain.Dock = System.Windows.Forms.DockStyle.None;
			this.menuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.menuLeague,
									this.menuReport});
			this.menuStripMain.Location = new System.Drawing.Point(0, 0);
			this.menuStripMain.Name = "menuStripMain";
			this.menuStripMain.Size = new System.Drawing.Size(1272, 24);
			this.menuStripMain.TabIndex = 1;
			this.menuStripMain.Text = "menuStrip1";
			// 
			// menuLeague
			// 
			this.menuLeague.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.menuNewLeague,
									this.menuOpenLeague,
									this.menuSaveLeague,
									this.menuCloseLeague,
									this.menuEditLeague,
									this.menuSplitter1,
									this.menuPyramid,
									this.menuFixtures,
									this.menuRememberAllTeams,
									this.menuMatch,
									this.menuSplitter2,
									this.menuPreferences,
									this.aboutToolStripMenuItem,
									this.menuSplitter3});
			this.menuLeague.Name = "menuLeague";
			this.menuLeague.Size = new System.Drawing.Size(61, 20);
			this.menuLeague.Text = "&League";
			// 
			// menuNewLeague
			// 
			this.menuNewLeague.Name = "menuNewLeague";
			this.menuNewLeague.Size = new System.Drawing.Size(225, 22);
			this.menuNewLeague.Text = "&New League";
			this.menuNewLeague.Click += new System.EventHandler(this.ButtonNewClick);
			// 
			// menuOpenLeague
			// 
			this.menuOpenLeague.Name = "menuOpenLeague";
			this.menuOpenLeague.Size = new System.Drawing.Size(225, 22);
			this.menuOpenLeague.Text = "&Open League";
			this.menuOpenLeague.Click += new System.EventHandler(this.ButtonLoadClick);
			// 
			// menuSaveLeague
			// 
			this.menuSaveLeague.Name = "menuSaveLeague";
			this.menuSaveLeague.Size = new System.Drawing.Size(225, 22);
			this.menuSaveLeague.Text = "&Save League";
			this.menuSaveLeague.Click += new System.EventHandler(this.ButtonSaveClick);
			// 
			// menuCloseLeague
			// 
			this.menuCloseLeague.Name = "menuCloseLeague";
			this.menuCloseLeague.Size = new System.Drawing.Size(225, 22);
			this.menuCloseLeague.Text = "&Close League";
			// 
			// menuEditLeague
			// 
			this.menuEditLeague.Name = "menuEditLeague";
			this.menuEditLeague.Size = new System.Drawing.Size(225, 22);
			this.menuEditLeague.Text = "&Edit League";
			this.menuEditLeague.Click += new System.EventHandler(this.MenuEditLeagueClick);
			// 
			// menuSplitter1
			// 
			this.menuSplitter1.Name = "menuSplitter1";
			this.menuSplitter1.Size = new System.Drawing.Size(222, 6);
			// 
			// menuPyramid
			// 
			this.menuPyramid.Name = "menuPyramid";
			this.menuPyramid.Size = new System.Drawing.Size(225, 22);
			this.menuPyramid.Text = "Set up &Pyramid round...";
			// 
			// menuFixtures
			// 
			this.menuFixtures.Name = "menuFixtures";
			this.menuFixtures.Size = new System.Drawing.Size(225, 22);
			this.menuFixtures.Text = "Set up &Fixtures...";
			this.menuFixtures.Click += new System.EventHandler(this.ButtonFixtureClick);
			// 
			// menuRememberAllTeams
			// 
			this.menuRememberAllTeams.Name = "menuRememberAllTeams";
			this.menuRememberAllTeams.Size = new System.Drawing.Size(225, 22);
			this.menuRememberAllTeams.Text = "Remember all teams";
			// 
			// menuMatch
			// 
			this.menuMatch.Name = "menuMatch";
			this.menuMatch.Size = new System.Drawing.Size(225, 22);
			this.menuMatch.Text = "Match...";
			// 
			// menuSplitter2
			// 
			this.menuSplitter2.Name = "menuSplitter2";
			this.menuSplitter2.Size = new System.Drawing.Size(222, 6);
			// 
			// menuPreferences
			// 
			this.menuPreferences.Name = "menuPreferences";
			this.menuPreferences.Size = new System.Drawing.Size(225, 22);
			this.menuPreferences.Text = "Preferences...";
			this.menuPreferences.Click += new System.EventHandler(this.ButtonPreferencesClick);
			// 
			// aboutToolStripMenuItem
			// 
			this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
			this.aboutToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
			this.aboutToolStripMenuItem.Text = "About...";
			// 
			// menuSplitter3
			// 
			this.menuSplitter3.Name = "menuSplitter3";
			this.menuSplitter3.Size = new System.Drawing.Size(222, 6);
			// 
			// menuReport
			// 
			this.menuReport.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.menuAdHocReport,
									this.menuExportReports,
									this.menuBulkUploadReports,
									this.menuConfigureReports,
									this.menuPackReport,
									this.updateScoreboardToolStripMenuItem});
			this.menuReport.Name = "menuReport";
			this.menuReport.Size = new System.Drawing.Size(58, 20);
			this.menuReport.Text = "&Report";
			// 
			// menuAdHocReport
			// 
			this.menuAdHocReport.Name = "menuAdHocReport";
			this.menuAdHocReport.Size = new System.Drawing.Size(257, 22);
			this.menuAdHocReport.Text = "Ad Hoc &Report";
			// 
			// menuExportReports
			// 
			this.menuExportReports.Name = "menuExportReports";
			this.menuExportReports.Size = new System.Drawing.Size(257, 22);
			this.menuExportReports.Text = "Bulk Export Reports";
			this.menuExportReports.Click += new System.EventHandler(this.ButtonExportClick);
			// 
			// menuBulkUploadReports
			// 
			this.menuBulkUploadReports.Name = "menuBulkUploadReports";
			this.menuBulkUploadReports.Size = new System.Drawing.Size(257, 22);
			this.menuBulkUploadReports.Text = "Bulk Upload Reports";
			this.menuBulkUploadReports.Click += new System.EventHandler(this.ButtonUploadClick);
			// 
			// menuConfigureReports
			// 
			this.menuConfigureReports.Name = "menuConfigureReports";
			this.menuConfigureReports.Size = new System.Drawing.Size(257, 22);
			this.menuConfigureReports.Text = "Configure Reports for League";
			this.menuConfigureReports.Click += new System.EventHandler(this.ButtonEditReportsClick);
			// 
			// menuPackReport
			// 
			this.menuPackReport.Name = "menuPackReport";
			this.menuPackReport.Size = new System.Drawing.Size(257, 22);
			this.menuPackReport.Text = "Pack Report";
			this.menuPackReport.Click += new System.EventHandler(this.ButtonPackReportClick);
			// 
			// updateScoreboardToolStripMenuItem
			// 
			this.updateScoreboardToolStripMenuItem.Name = "updateScoreboardToolStripMenuItem";
			this.updateScoreboardToolStripMenuItem.Size = new System.Drawing.Size(257, 22);
			this.updateScoreboardToolStripMenuItem.Text = "&Update Scoreboard";
			// 
			// toolStripLeague
			// 
			this.toolStripLeague.Dock = System.Windows.Forms.DockStyle.None;
			this.toolStripLeague.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.toolStripLabelLeagues,
									this.buttonNew,
									this.buttonOpen,
									this.buttonSave,
									this.buttonClose,
									this.buttonEdit});
			this.toolStripLeague.Location = new System.Drawing.Point(3, 24);
			this.toolStripLeague.Name = "toolStripLeague";
			this.toolStripLeague.Size = new System.Drawing.Size(347, 25);
			this.toolStripLeague.TabIndex = 0;
			// 
			// toolStripLabelLeagues
			// 
			this.toolStripLabelLeagues.Name = "toolStripLabelLeagues";
			this.toolStripLabelLeagues.Size = new System.Drawing.Size(60, 22);
			this.toolStripLabelLeagues.Text = "Leagues:";
			// 
			// buttonNew
			// 
			this.buttonNew.Image = ((System.Drawing.Image)(resources.GetObject("buttonNew.Image")));
			this.buttonNew.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonNew.Name = "buttonNew";
			this.buttonNew.Size = new System.Drawing.Size(53, 22);
			this.buttonNew.Text = "New";
			this.buttonNew.Click += new System.EventHandler(this.ButtonNewClick);
			// 
			// buttonOpen
			// 
			this.buttonOpen.Image = ((System.Drawing.Image)(resources.GetObject("buttonOpen.Image")));
			this.buttonOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonOpen.Name = "buttonOpen";
			this.buttonOpen.Size = new System.Drawing.Size(58, 22);
			this.buttonOpen.Text = "Open";
			this.buttonOpen.Click += new System.EventHandler(this.ButtonLoadClick);
			// 
			// buttonSave
			// 
			this.buttonSave.Image = ((System.Drawing.Image)(resources.GetObject("buttonSave.Image")));
			this.buttonSave.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonSave.Name = "buttonSave";
			this.buttonSave.Size = new System.Drawing.Size(56, 22);
			this.buttonSave.Text = "Save";
			this.buttonSave.Click += new System.EventHandler(this.ButtonSaveClick);
			// 
			// buttonClose
			// 
			this.buttonClose.Image = ((System.Drawing.Image)(resources.GetObject("buttonClose.Image")));
			this.buttonClose.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonClose.Name = "buttonClose";
			this.buttonClose.Size = new System.Drawing.Size(59, 22);
			this.buttonClose.Text = "Close";
			this.buttonClose.Click += new System.EventHandler(this.ButtonCloseClick);
			// 
			// buttonEdit
			// 
			this.buttonEdit.Image = ((System.Drawing.Image)(resources.GetObject("buttonEdit.Image")));
			this.buttonEdit.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonEdit.Name = "buttonEdit";
			this.buttonEdit.Size = new System.Drawing.Size(49, 22);
			this.buttonEdit.Text = "Edit";
			this.buttonEdit.Click += new System.EventHandler(this.MenuEditLeagueClick);
			// 
			// toolStripTeams
			// 
			this.toolStripTeams.Dock = System.Windows.Forms.DockStyle.None;
			this.toolStripTeams.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.toolStripLabelTeams,
									this.buttonAddRow,
									this.buttonRemoveRow,
									this.buttonAddColumn,
									this.buttonRemoveColumn});
			this.toolStripTeams.Location = new System.Drawing.Point(3, 49);
			this.toolStripTeams.Name = "toolStripTeams";
			this.toolStripTeams.Size = new System.Drawing.Size(156, 25);
			this.toolStripTeams.TabIndex = 3;
			// 
			// toolStripLabelTeams
			// 
			this.toolStripLabelTeams.Name = "toolStripLabelTeams";
			this.toolStripLabelTeams.Size = new System.Drawing.Size(52, 22);
			this.toolStripLabelTeams.Text = "Teams:";
			// 
			// buttonAddRow
			// 
			this.buttonAddRow.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonAddRow.Image = ((System.Drawing.Image)(resources.GetObject("buttonAddRow.Image")));
			this.buttonAddRow.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.buttonAddRow.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonAddRow.Name = "buttonAddRow";
			this.buttonAddRow.Size = new System.Drawing.Size(23, 22);
			this.buttonAddRow.Text = "Add Row";
			this.buttonAddRow.Click += new System.EventHandler(this.ButtonAddRowClick);
			// 
			// buttonRemoveRow
			// 
			this.buttonRemoveRow.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonRemoveRow.Enabled = false;
			this.buttonRemoveRow.Image = ((System.Drawing.Image)(resources.GetObject("buttonRemoveRow.Image")));
			this.buttonRemoveRow.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.buttonRemoveRow.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonRemoveRow.Name = "buttonRemoveRow";
			this.buttonRemoveRow.Size = new System.Drawing.Size(23, 22);
			this.buttonRemoveRow.Text = "Remove Row";
			this.buttonRemoveRow.Click += new System.EventHandler(this.ButtonRemoveRowClick);
			// 
			// buttonAddColumn
			// 
			this.buttonAddColumn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonAddColumn.Image = ((System.Drawing.Image)(resources.GetObject("buttonAddColumn.Image")));
			this.buttonAddColumn.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.buttonAddColumn.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonAddColumn.Name = "buttonAddColumn";
			this.buttonAddColumn.Size = new System.Drawing.Size(23, 22);
			this.buttonAddColumn.Text = "Add Column";
			this.buttonAddColumn.Click += new System.EventHandler(this.ButtonAddColumnClick);
			// 
			// buttonRemoveColumn
			// 
			this.buttonRemoveColumn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonRemoveColumn.Enabled = false;
			this.buttonRemoveColumn.Image = ((System.Drawing.Image)(resources.GetObject("buttonRemoveColumn.Image")));
			this.buttonRemoveColumn.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.buttonRemoveColumn.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonRemoveColumn.Name = "buttonRemoveColumn";
			this.buttonRemoveColumn.Size = new System.Drawing.Size(23, 22);
			this.buttonRemoveColumn.Text = "Remove Column";
			this.buttonRemoveColumn.Click += new System.EventHandler(this.ButtonRemoveColumnClick);
			// 
			// toolStripGame
			// 
			this.toolStripGame.Dock = System.Windows.Forms.DockStyle.None;
			this.toolStripGame.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.toolStripLabelGames,
									this.buttonLatestGame,
									this.buttonCommit,
									this.buttonCreateGame,
									this.buttonEditGame,
									this.buttonSetDescription,
									this.buttonForget});
			this.toolStripGame.Location = new System.Drawing.Point(3, 74);
			this.toolStripGame.Name = "toolStripGame";
			this.toolStripGame.Size = new System.Drawing.Size(433, 25);
			this.toolStripGame.TabIndex = 2;
			// 
			// toolStripLabelGames
			// 
			this.toolStripLabelGames.Name = "toolStripLabelGames";
			this.toolStripLabelGames.Size = new System.Drawing.Size(52, 22);
			this.toolStripLabelGames.Text = "Games:";
			// 
			// buttonLatestGame
			// 
			this.buttonLatestGame.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.buttonLatestGame.Image = ((System.Drawing.Image)(resources.GetObject("buttonLatestGame.Image")));
			this.buttonLatestGame.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonLatestGame.Name = "buttonLatestGame";
			this.buttonLatestGame.Size = new System.Drawing.Size(46, 22);
			this.buttonLatestGame.Text = "Latest";
			this.buttonLatestGame.Click += new System.EventHandler(this.ButtonLatestGameClick);
			// 
			// buttonCommit
			// 
			this.buttonCommit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.buttonCommit.Image = ((System.Drawing.Image)(resources.GetObject("buttonCommit.Image")));
			this.buttonCommit.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonCommit.Name = "buttonCommit";
			this.buttonCommit.Size = new System.Drawing.Size(56, 22);
			this.buttonCommit.Text = "Commit";
			this.buttonCommit.Click += new System.EventHandler(this.ButtonCommitClick);
			// 
			// buttonCreateGame
			// 
			this.buttonCreateGame.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.buttonCreateGame.Enabled = false;
			this.buttonCreateGame.Image = ((System.Drawing.Image)(resources.GetObject("buttonCreateGame.Image")));
			this.buttonCreateGame.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonCreateGame.Name = "buttonCreateGame";
			this.buttonCreateGame.Size = new System.Drawing.Size(50, 22);
			this.buttonCreateGame.Text = "Create";
			// 
			// buttonEditGame
			// 
			this.buttonEditGame.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.buttonEditGame.Enabled = false;
			this.buttonEditGame.Image = ((System.Drawing.Image)(resources.GetObject("buttonEditGame.Image")));
			this.buttonEditGame.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonEditGame.Name = "buttonEditGame";
			this.buttonEditGame.Size = new System.Drawing.Size(70, 22);
			this.buttonEditGame.Text = "Edit Game";
			// 
			// buttonSetDescription
			// 
			this.buttonSetDescription.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.buttonSetDescription.Image = ((System.Drawing.Image)(resources.GetObject("buttonSetDescription.Image")));
			this.buttonSetDescription.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonSetDescription.Name = "buttonSetDescription";
			this.buttonSetDescription.Size = new System.Drawing.Size(98, 22);
			this.buttonSetDescription.Text = "Set Description";
			this.buttonSetDescription.Click += new System.EventHandler(this.ButtonSetDescriptionClick);
			// 
			// buttonForget
			// 
			this.buttonForget.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.buttonForget.Image = ((System.Drawing.Image)(resources.GetObject("buttonForget.Image")));
			this.buttonForget.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonForget.Name = "buttonForget";
			this.buttonForget.Size = new System.Drawing.Size(49, 22);
			this.buttonForget.Text = "Forget";
			this.buttonForget.Click += new System.EventHandler(this.ButtonForgetClick);
			// 
			// imageListPacks
			// 
			this.imageListPacks.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListPacks.ImageStream")));
			this.imageListPacks.TransparentColor = System.Drawing.Color.Transparent;
			this.imageListPacks.Images.SetKeyName(0, "unknown.png");
			this.imageListPacks.Images.SetKeyName(1, "red.png");
			this.imageListPacks.Images.SetKeyName(2, "blue.png");
			this.imageListPacks.Images.SetKeyName(3, "green.png");
			this.imageListPacks.Images.SetKeyName(4, "yellow.png");
			this.imageListPacks.Images.SetKeyName(5, "purple.png");
			this.imageListPacks.Images.SetKeyName(6, "pink.png");
			this.imageListPacks.Images.SetKeyName(7, "cyan.png");
			this.imageListPacks.Images.SetKeyName(8, "orange.png");
			// 
			// timerGame
			// 
			this.timerGame.Enabled = true;
			this.timerGame.Interval = 1000;
			this.timerGame.Tick += new System.EventHandler(this.TimerGameTick);
			// 
			// saveFileDialog1
			// 
			this.saveFileDialog1.Filter = "Torn files|*.Torn|All files|*.*";
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.ClientSize = new System.Drawing.Size(1272, 748);
			this.Controls.Add(this.toolStripContainer1);
			this.MainMenuStrip = this.menuStripMain;
			this.MinimumSize = new System.Drawing.Size(620, 380);
			this.Name = "MainForm";
			this.Text = "Torn5";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainFormFormClosing);
			this.Shown += new System.EventHandler(this.MainFormShown);
			this.toolStripContainer1.ContentPanel.ResumeLayout(false);
			this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
			this.toolStripContainer1.TopToolStripPanel.PerformLayout();
			this.toolStripContainer1.ResumeLayout(false);
			this.toolStripContainer1.PerformLayout();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.panelGames.ResumeLayout(false);
			this.contextMenuStripGames.ResumeLayout(false);
			this.panelLeague.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.numericPort)).EndInit();
			this.menuStripMain.ResumeLayout(false);
			this.menuStripMain.PerformLayout();
			this.toolStripLeague.ResumeLayout(false);
			this.toolStripLeague.PerformLayout();
			this.toolStripTeams.ResumeLayout(false);
			this.toolStripTeams.PerformLayout();
			this.toolStripGame.ResumeLayout(false);
			this.toolStripGame.PerformLayout();
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.ToolStripButton buttonEdit;
		private System.Windows.Forms.ToolStripLabel toolStripLabelGames;
		private System.Windows.Forms.ToolStripLabel toolStripLabelTeams;
		private System.Windows.Forms.ToolStripLabel toolStripLabelLeagues;
		private System.Windows.Forms.ToolStripButton buttonEditGame;
		private System.Windows.Forms.ToolStripButton buttonCreateGame;
		private System.Windows.Forms.ToolStripMenuItem menuExportReports;
		private System.Windows.Forms.ToolStripMenuItem forgetGameToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem commitGameToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem setDescriptionToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem latestGameToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip contextMenuStripGames;
		private System.Windows.Forms.ToolStripMenuItem menuReport;
		private System.Windows.Forms.ToolStripMenuItem menuPackReport;
		private System.Windows.Forms.ToolStripMenuItem menuConfigureReports;
		private System.Windows.Forms.ToolStripMenuItem menuBulkUploadReports;
		private System.Windows.Forms.ToolStrip toolStripTeams;
		private System.Windows.Forms.ToolStripButton buttonForget;
		private System.Windows.Forms.ToolStripButton buttonCommit;
		private System.Windows.Forms.ToolStripButton buttonSetDescription;
		private System.Windows.Forms.ToolStripButton buttonLatestGame;
		private System.Windows.Forms.ToolStrip toolStripGame;
		private System.Windows.Forms.ToolStripButton buttonClose;
		private System.Windows.Forms.ToolStripButton buttonSave;
		private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem updateScoreboardToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem menuPreferences;
		private System.Windows.Forms.ToolStripSeparator menuSplitter2;
		private System.Windows.Forms.ToolStripMenuItem menuMatch;
		private System.Windows.Forms.ToolStripMenuItem menuRememberAllTeams;
		private System.Windows.Forms.ToolStripMenuItem menuFixtures;
		private System.Windows.Forms.ToolStripMenuItem menuPyramid;
		private System.Windows.Forms.ToolStripMenuItem menuAdHocReport;
		private System.Windows.Forms.ToolStripSeparator menuSplitter3;
		private System.Windows.Forms.ToolStripSeparator menuSplitter1;
		private System.Windows.Forms.ToolStripMenuItem menuEditLeague;
		private System.Windows.Forms.ToolStripMenuItem menuCloseLeague;
		private System.Windows.Forms.ToolStripMenuItem menuSaveLeague;
		private System.Windows.Forms.ToolStripMenuItem menuOpenLeague;
		private System.Windows.Forms.ToolStripMenuItem menuNewLeague;
		private System.Windows.Forms.ToolStripMenuItem menuLeague;
		private System.Windows.Forms.MenuStrip menuStripMain;
		private System.Windows.Forms.SaveFileDialog saveFileDialog1;
		private System.Windows.Forms.ToolStripButton buttonNew;
		private System.Windows.Forms.Button buttonCommit2;
		private System.Windows.Forms.Button buttonTsvExport;
		private System.Windows.Forms.Panel panelLeague;
		private System.Windows.Forms.Button buttonLatestGame2;
		private System.Windows.Forms.Panel panelGames;
		private System.Windows.Forms.ImageList imageListPacks;
		private System.Windows.Forms.ToolStripButton buttonRemoveColumn;
		private System.Windows.Forms.ToolStripButton buttonAddColumn;
		private System.Windows.Forms.ToolStripButton buttonRemoveRow;
		private System.Windows.Forms.ToolStripButton buttonAddRow;
		private System.Windows.Forms.ToolStripButton buttonOpen;
		private System.Windows.Forms.ToolStrip toolStripLeague;
		private System.Windows.Forms.ToolStripContainer toolStripContainer1;
		private System.Windows.Forms.ColumnHeader colDescription;
		private System.Windows.Forms.ColumnHeader colLeague;
		private System.Windows.Forms.ColumnHeader colGame;
		private System.Windows.Forms.Label labelLeagueDetails;
		private System.Windows.Forms.ListView listViewGames;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.ProgressBar progressBar1;
		private System.Windows.Forms.Label labelStatus;
		private System.Windows.Forms.Button buttonExportFixtures;
		private System.Windows.Forms.Label labelNow;
		private System.Windows.Forms.Timer timerGame;
		private System.Windows.Forms.Label labelTime;
		private System.Windows.Forms.ColumnHeader colTeams;
		private System.Windows.Forms.ColumnHeader colTag;
		private System.Windows.Forms.NumericUpDown numericPort;
		private System.Windows.Forms.ImageList imageListLeagues;
		private System.Windows.Forms.ColumnHeader colGames;
		private System.Windows.Forms.ColumnHeader colFile;
		private System.Windows.Forms.ColumnHeader colTitle;
		private System.Windows.Forms.ListView listViewLeagues;
		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
	}
}
