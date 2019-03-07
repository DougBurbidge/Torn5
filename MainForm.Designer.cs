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
			this.panelLeague = new System.Windows.Forms.Panel();
			this.labelNow = new System.Windows.Forms.Label();
			this.labelTime = new System.Windows.Forms.Label();
			this.labelLeagueDetails = new System.Windows.Forms.Label();
			this.progressBar1 = new System.Windows.Forms.ProgressBar();
			this.labelStatus = new System.Windows.Forms.Label();
			this.toolStripLeague = new System.Windows.Forms.ToolStrip();
			this.toolStripDropDownLeagues = new System.Windows.Forms.ToolStripDropDownButton();
			this.buttonPreferences = new System.Windows.Forms.ToolStripMenuItem();
			this.buttonSave = new System.Windows.Forms.ToolStripMenuItem();
			this.buttonPyramid = new System.Windows.Forms.ToolStripMenuItem();
			this.buttonFixtures = new System.Windows.Forms.ToolStripMenuItem();
			this.buttonMatch = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.buttonHelp = new System.Windows.Forms.ToolStripMenuItem();
			this.buttonAbout = new System.Windows.Forms.ToolStripMenuItem();
			this.buttonNew = new System.Windows.Forms.ToolStripButton();
			this.buttonOpen = new System.Windows.Forms.ToolStripButton();
			this.buttonClose = new System.Windows.Forms.ToolStripButton();
			this.buttonEdit = new System.Windows.Forms.ToolStripButton();
			this.toolStripGame = new System.Windows.Forms.ToolStrip();
			this.toolStripDropDownGames = new System.Windows.Forms.ToolStripDropDownButton();
			this.buttonCreateGame = new System.Windows.Forms.ToolStripMenuItem();
			this.buttonEditGame = new System.Windows.Forms.ToolStripMenuItem();
			this.buttonSetDescription = new System.Windows.Forms.ToolStripMenuItem();
			this.buttonForget = new System.Windows.Forms.ToolStripMenuItem();
			this.buttonLatestGame = new System.Windows.Forms.ToolStripButton();
			this.buttonCommit = new System.Windows.Forms.ToolStripButton();
			this.toolStripTeams = new System.Windows.Forms.ToolStrip();
			this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
			this.buttonRememberAllTeams = new System.Windows.Forms.ToolStripMenuItem();
			this.buttonAddRow = new System.Windows.Forms.ToolStripButton();
			this.buttonRemoveRow = new System.Windows.Forms.ToolStripButton();
			this.buttonAddColumn = new System.Windows.Forms.ToolStripButton();
			this.buttonRemoveColumn = new System.Windows.Forms.ToolStripButton();
			this.toolStripReports = new System.Windows.Forms.ToolStrip();
			this.toolStripDropDownReports = new System.Windows.Forms.ToolStripDropDownButton();
			this.buttonAdHocReport = new System.Windows.Forms.ToolStripMenuItem();
			this.buttonPackReport = new System.Windows.Forms.ToolStripMenuItem();
			this.buttonUpdateScoreboard = new System.Windows.Forms.ToolStripMenuItem();
			this.buttonExportFixtures = new System.Windows.Forms.ToolStripMenuItem();
			this.buttonTsvExport = new System.Windows.Forms.ToolStripMenuItem();
			this.buttonSetFolder = new System.Windows.Forms.ToolStripButton();
			this.buttonExportReports = new System.Windows.Forms.ToolStripButton();
			this.buttonUploadReports = new System.Windows.Forms.ToolStripButton();
			this.buttonConfigureReports = new System.Windows.Forms.ToolStripButton();
			this.imageListPacks = new System.Windows.Forms.ImageList(this.components);
			this.timerGame = new System.Windows.Forms.Timer(this.components);
			this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
			this.toolStripContainer1.ContentPanel.SuspendLayout();
			this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
			this.toolStripContainer1.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.panelGames.SuspendLayout();
			this.panelLeague.SuspendLayout();
			this.toolStripLeague.SuspendLayout();
			this.toolStripGame.SuspendLayout();
			this.toolStripTeams.SuspendLayout();
			this.toolStripReports.SuspendLayout();
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
			// 
			// toolStripContainer1.ContentPanel
			// 
			this.toolStripContainer1.ContentPanel.Controls.Add(this.tableLayoutPanel1);
			this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(1272, 592);
			this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
			this.toolStripContainer1.Name = "toolStripContainer1";
			this.toolStripContainer1.Size = new System.Drawing.Size(1272, 692);
			this.toolStripContainer1.TabIndex = 21;
			this.toolStripContainer1.Text = "toolStripContainer1";
			// 
			// toolStripContainer1.TopToolStripPanel
			// 
			this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStripLeague);
			this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStripGame);
			this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStripTeams);
			this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStripReports);
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
			this.tableLayoutPanel1.Size = new System.Drawing.Size(1272, 592);
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
			this.listViewLeagues.Size = new System.Drawing.Size(312, 290);
			this.listViewLeagues.SmallImageList = this.imageListLeagues;
			this.listViewLeagues.TabIndex = 11;
			this.listViewLeagues.UseCompatibleStateImageBehavior = false;
			this.listViewLeagues.View = System.Windows.Forms.View.Details;
			this.listViewLeagues.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.ListViewLeaguesAfterLabelEdit);
			this.listViewLeagues.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.ListViewLeaguesItemSelectionChanged);
			this.listViewLeagues.SelectedIndexChanged += new System.EventHandler(this.ListViewGamesSelectedIndexChanged);
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
			this.panelGames.Size = new System.Drawing.Size(318, 592);
			this.panelGames.TabIndex = 15;
			// 
			// buttonCommit2
			// 
			this.buttonCommit2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonCommit2.Location = new System.Drawing.Point(90, 552);
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
			this.buttonLatestGame2.Location = new System.Drawing.Point(3, 552);
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
			this.listViewGames.FullRowSelect = true;
			this.listViewGames.HideSelection = false;
			this.listViewGames.Location = new System.Drawing.Point(3, 3);
			this.listViewGames.Name = "listViewGames";
			this.listViewGames.Size = new System.Drawing.Size(315, 586);
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
			// panelLeague
			// 
			this.panelLeague.Controls.Add(this.labelNow);
			this.panelLeague.Controls.Add(this.labelTime);
			this.panelLeague.Controls.Add(this.labelLeagueDetails);
			this.panelLeague.Controls.Add(this.progressBar1);
			this.panelLeague.Controls.Add(this.labelStatus);
			this.panelLeague.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelLeague.Location = new System.Drawing.Point(3, 299);
			this.panelLeague.Name = "panelLeague";
			this.panelLeague.Size = new System.Drawing.Size(312, 290);
			this.panelLeague.TabIndex = 16;
			// 
			// labelNow
			// 
			this.labelNow.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.labelNow.Location = new System.Drawing.Point(12, 167);
			this.labelNow.Name = "labelNow";
			this.labelNow.Size = new System.Drawing.Size(297, 64);
			this.labelNow.TabIndex = 1;
			this.labelNow.Text = "Now Playing:";
			// 
			// labelTime
			// 
			this.labelTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.labelTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelTime.Location = new System.Drawing.Point(12, 136);
			this.labelTime.Name = "labelTime";
			this.labelTime.Size = new System.Drawing.Size(297, 23);
			this.labelTime.TabIndex = 0;
			this.labelTime.Text = "Time";
			// 
			// labelLeagueDetails
			// 
			this.labelLeagueDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.labelLeagueDetails.Location = new System.Drawing.Point(0, 0);
			this.labelLeagueDetails.Name = "labelLeagueDetails";
			this.labelLeagueDetails.Size = new System.Drawing.Size(312, 82);
			this.labelLeagueDetails.TabIndex = 13;
			this.labelLeagueDetails.Text = "Select a league and its details will appear here.";
			// 
			// progressBar1
			// 
			this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.progressBar1.Location = new System.Drawing.Point(3, 264);
			this.progressBar1.Maximum = 1000;
			this.progressBar1.Name = "progressBar1";
			this.progressBar1.Size = new System.Drawing.Size(286, 23);
			this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			this.progressBar1.TabIndex = 19;
			this.progressBar1.Visible = false;
			// 
			// labelStatus
			// 
			this.labelStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.labelStatus.Location = new System.Drawing.Point(3, 240);
			this.labelStatus.Name = "labelStatus";
			this.labelStatus.Size = new System.Drawing.Size(306, 21);
			this.labelStatus.TabIndex = 18;
			// 
			// toolStripLeague
			// 
			this.toolStripLeague.Dock = System.Windows.Forms.DockStyle.None;
			this.toolStripLeague.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.toolStripDropDownLeagues,
									this.buttonNew,
									this.buttonOpen,
									this.buttonClose,
									this.buttonEdit});
			this.toolStripLeague.Location = new System.Drawing.Point(3, 0);
			this.toolStripLeague.Name = "toolStripLeague";
			this.toolStripLeague.Size = new System.Drawing.Size(299, 25);
			this.toolStripLeague.TabIndex = 0;
			// 
			// toolStripDropDownLeagues
			// 
			this.toolStripDropDownLeagues.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripDropDownLeagues.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.buttonPreferences,
									this.buttonSave,
									this.buttonPyramid,
									this.buttonFixtures,
									this.buttonMatch,
									this.toolStripMenuItem1,
									this.buttonHelp,
									this.buttonAbout});
			this.toolStripDropDownLeagues.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripDropDownLeagues.Name = "toolStripDropDownLeagues";
			this.toolStripDropDownLeagues.Size = new System.Drawing.Size(68, 22);
			this.toolStripDropDownLeagues.Text = "Leagues";
			// 
			// buttonPreferences
			// 
			this.buttonPreferences.Name = "buttonPreferences";
			this.buttonPreferences.Size = new System.Drawing.Size(225, 22);
			this.buttonPreferences.Text = "Preferences...";
			this.buttonPreferences.Click += new System.EventHandler(this.ButtonPreferencesClick);
			// 
			// buttonSave
			// 
			this.buttonSave.Name = "buttonSave";
			this.buttonSave.Size = new System.Drawing.Size(225, 22);
			this.buttonSave.Text = "Save League(s)";
			this.buttonSave.Click += new System.EventHandler(this.ButtonSaveClick);
			// 
			// buttonPyramid
			// 
			this.buttonPyramid.Enabled = false;
			this.buttonPyramid.Name = "buttonPyramid";
			this.buttonPyramid.Size = new System.Drawing.Size(225, 22);
			this.buttonPyramid.Text = "Set up &Pyramid round...";
			// 
			// buttonFixtures
			// 
			this.buttonFixtures.Name = "buttonFixtures";
			this.buttonFixtures.Size = new System.Drawing.Size(225, 22);
			this.buttonFixtures.Text = "Set up &Fixtures...";
			this.buttonFixtures.Click += new System.EventHandler(this.ButtonFixtureClick);
			// 
			// buttonMatch
			// 
			this.buttonMatch.Enabled = false;
			this.buttonMatch.Name = "buttonMatch";
			this.buttonMatch.Size = new System.Drawing.Size(225, 22);
			this.buttonMatch.Text = "Match...";
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(222, 6);
			// 
			// buttonHelp
			// 
			this.buttonHelp.Name = "buttonHelp";
			this.buttonHelp.Size = new System.Drawing.Size(225, 22);
			this.buttonHelp.Text = "Help...";
			this.buttonHelp.Click += new System.EventHandler(this.ButtonHelpClick);
			// 
			// buttonAbout
			// 
			this.buttonAbout.Name = "buttonAbout";
			this.buttonAbout.Size = new System.Drawing.Size(225, 22);
			this.buttonAbout.Text = "About...";
			this.buttonAbout.Click += new System.EventHandler(this.ButtonAboutClick);
			// 
			// buttonNew
			// 
			this.buttonNew.Image = ((System.Drawing.Image)(resources.GetObject("buttonNew.Image")));
			this.buttonNew.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonNew.Name = "buttonNew";
			this.buttonNew.Size = new System.Drawing.Size(53, 22);
			this.buttonNew.Text = "&New";
			this.buttonNew.Click += new System.EventHandler(this.ButtonNewClick);
			// 
			// buttonOpen
			// 
			this.buttonOpen.Image = ((System.Drawing.Image)(resources.GetObject("buttonOpen.Image")));
			this.buttonOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonOpen.Name = "buttonOpen";
			this.buttonOpen.Size = new System.Drawing.Size(58, 22);
			this.buttonOpen.Text = "&Open";
			this.buttonOpen.Click += new System.EventHandler(this.ButtonLoadClick);
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
			this.buttonEdit.Text = "&Edit";
			this.buttonEdit.Click += new System.EventHandler(this.ButtonEditLeagueClick);
			// 
			// toolStripGame
			// 
			this.toolStripGame.Dock = System.Windows.Forms.DockStyle.None;
			this.toolStripGame.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.toolStripDropDownGames,
									this.buttonLatestGame,
									this.buttonCommit});
			this.toolStripGame.Location = new System.Drawing.Point(3, 25);
			this.toolStripGame.Name = "toolStripGame";
			this.toolStripGame.Size = new System.Drawing.Size(206, 25);
			this.toolStripGame.TabIndex = 2;
			// 
			// toolStripDropDownGames
			// 
			this.toolStripDropDownGames.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripDropDownGames.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.buttonCreateGame,
									this.buttonEditGame,
									this.buttonSetDescription,
									this.buttonForget});
			this.toolStripDropDownGames.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripDropDownGames.Name = "toolStripDropDownGames";
			this.toolStripDropDownGames.Size = new System.Drawing.Size(60, 22);
			this.toolStripDropDownGames.Text = "Games";
			// 
			// buttonCreateGame
			// 
			this.buttonCreateGame.Enabled = false;
			this.buttonCreateGame.Name = "buttonCreateGame";
			this.buttonCreateGame.Size = new System.Drawing.Size(175, 22);
			this.buttonCreateGame.Text = "Create";
			// 
			// buttonEditGame
			// 
			this.buttonEditGame.Enabled = false;
			this.buttonEditGame.Name = "buttonEditGame";
			this.buttonEditGame.Size = new System.Drawing.Size(175, 22);
			this.buttonEditGame.Text = "Edit";
			// 
			// buttonSetDescription
			// 
			this.buttonSetDescription.Name = "buttonSetDescription";
			this.buttonSetDescription.Size = new System.Drawing.Size(175, 22);
			this.buttonSetDescription.Text = "Set &Description";
			this.buttonSetDescription.Click += new System.EventHandler(this.ButtonSetDescriptionClick);
			// 
			// buttonForget
			// 
			this.buttonForget.Name = "buttonForget";
			this.buttonForget.Size = new System.Drawing.Size(175, 22);
			this.buttonForget.Text = "Forget";
			this.buttonForget.Click += new System.EventHandler(this.ButtonForgetClick);
			// 
			// buttonLatestGame
			// 
			this.buttonLatestGame.Image = ((System.Drawing.Image)(resources.GetObject("buttonLatestGame.Image")));
			this.buttonLatestGame.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonLatestGame.Name = "buttonLatestGame";
			this.buttonLatestGame.Size = new System.Drawing.Size(62, 22);
			this.buttonLatestGame.Text = "&Latest";
			this.buttonLatestGame.Click += new System.EventHandler(this.ButtonLatestGameClick);
			// 
			// buttonCommit
			// 
			this.buttonCommit.Image = ((System.Drawing.Image)(resources.GetObject("buttonCommit.Image")));
			this.buttonCommit.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonCommit.Name = "buttonCommit";
			this.buttonCommit.Size = new System.Drawing.Size(72, 22);
			this.buttonCommit.Text = "&Commit";
			this.buttonCommit.Click += new System.EventHandler(this.ButtonCommitClick);
			// 
			// toolStripTeams
			// 
			this.toolStripTeams.Dock = System.Windows.Forms.DockStyle.None;
			this.toolStripTeams.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.toolStripDropDownButton1,
									this.buttonAddRow,
									this.buttonRemoveRow,
									this.buttonAddColumn,
									this.buttonRemoveColumn});
			this.toolStripTeams.Location = new System.Drawing.Point(3, 50);
			this.toolStripTeams.Name = "toolStripTeams";
			this.toolStripTeams.Size = new System.Drawing.Size(164, 25);
			this.toolStripTeams.TabIndex = 3;
			// 
			// toolStripDropDownButton1
			// 
			this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.buttonRememberAllTeams});
			this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
			this.toolStripDropDownButton1.Size = new System.Drawing.Size(60, 22);
			this.toolStripDropDownButton1.Text = "Teams";
			// 
			// buttonRememberAllTeams
			// 
			this.buttonRememberAllTeams.Enabled = false;
			this.buttonRememberAllTeams.Name = "buttonRememberAllTeams";
			this.buttonRememberAllTeams.Size = new System.Drawing.Size(213, 22);
			this.buttonRememberAllTeams.Text = "Remember All Teams";
			this.buttonRememberAllTeams.Click += new System.EventHandler(this.ButtonRememberAllTeamsClick);
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
			// toolStripReports
			// 
			this.toolStripReports.Dock = System.Windows.Forms.DockStyle.None;
			this.toolStripReports.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.toolStripDropDownReports,
									this.buttonSetFolder,
									this.buttonExportReports,
									this.buttonUploadReports,
									this.buttonConfigureReports});
			this.toolStripReports.Location = new System.Drawing.Point(3, 75);
			this.toolStripReports.Name = "toolStripReports";
			this.toolStripReports.Size = new System.Drawing.Size(392, 25);
			this.toolStripReports.TabIndex = 4;
			// 
			// toolStripDropDownReports
			// 
			this.toolStripDropDownReports.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripDropDownReports.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.buttonAdHocReport,
									this.buttonPackReport,
									this.buttonUpdateScoreboard,
									this.buttonExportFixtures,
									this.buttonTsvExport});
			this.toolStripDropDownReports.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripDropDownReports.Name = "toolStripDropDownReports";
			this.toolStripDropDownReports.Size = new System.Drawing.Size(65, 22);
			this.toolStripDropDownReports.Text = "Reports";
			// 
			// buttonAdHocReport
			// 
			this.buttonAdHocReport.Enabled = false;
			this.buttonAdHocReport.Name = "buttonAdHocReport";
			this.buttonAdHocReport.Size = new System.Drawing.Size(199, 22);
			this.buttonAdHocReport.Text = "Ad Hoc Report...";
			// 
			// buttonPackReport
			// 
			this.buttonPackReport.Name = "buttonPackReport";
			this.buttonPackReport.Size = new System.Drawing.Size(199, 22);
			this.buttonPackReport.Text = "Pack Report";
			this.buttonPackReport.Click += new System.EventHandler(this.ButtonPackReportClick);
			// 
			// buttonUpdateScoreboard
			// 
			this.buttonUpdateScoreboard.Enabled = false;
			this.buttonUpdateScoreboard.Name = "buttonUpdateScoreboard";
			this.buttonUpdateScoreboard.Size = new System.Drawing.Size(199, 22);
			this.buttonUpdateScoreboard.Text = "Update Scoreboard";
			// 
			// buttonExportFixtures
			// 
			this.buttonExportFixtures.Name = "buttonExportFixtures";
			this.buttonExportFixtures.Size = new System.Drawing.Size(199, 22);
			this.buttonExportFixtures.Text = "Export Fixtures";
			this.buttonExportFixtures.Click += new System.EventHandler(this.ButtonExportFixturesClick);
			// 
			// buttonTsvExport
			// 
			this.buttonTsvExport.Name = "buttonTsvExport";
			this.buttonTsvExport.Size = new System.Drawing.Size(199, 22);
			this.buttonTsvExport.Text = "Export TSV";
			this.buttonTsvExport.Click += new System.EventHandler(this.ButtonTsvExportClick);
			// 
			// buttonSetFolder
			// 
			this.buttonSetFolder.Image = ((System.Drawing.Image)(resources.GetObject("buttonSetFolder.Image")));
			this.buttonSetFolder.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonSetFolder.Name = "buttonSetFolder";
			this.buttonSetFolder.Size = new System.Drawing.Size(87, 22);
			this.buttonSetFolder.Text = "Set &Folder";
			this.buttonSetFolder.Click += new System.EventHandler(this.ButtonSetExportFolderClick);
			// 
			// buttonExportReports
			// 
			this.buttonExportReports.Image = ((System.Drawing.Image)(resources.GetObject("buttonExportReports.Image")));
			this.buttonExportReports.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonExportReports.Name = "buttonExportReports";
			this.buttonExportReports.Size = new System.Drawing.Size(66, 22);
			this.buttonExportReports.Text = "&Report";
			this.buttonExportReports.Click += new System.EventHandler(this.ButtonExportClick);
			// 
			// buttonUploadReports
			// 
			this.buttonUploadReports.Image = ((System.Drawing.Image)(resources.GetObject("buttonUploadReports.Image")));
			this.buttonUploadReports.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonUploadReports.Name = "buttonUploadReports";
			this.buttonUploadReports.Size = new System.Drawing.Size(67, 22);
			this.buttonUploadReports.Text = "&Upload";
			this.buttonUploadReports.Click += new System.EventHandler(this.ButtonUploadClick);
			// 
			// buttonConfigureReports
			// 
			this.buttonConfigureReports.Image = ((System.Drawing.Image)(resources.GetObject("buttonConfigureReports.Image")));
			this.buttonConfigureReports.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonConfigureReports.Name = "buttonConfigureReports";
			this.buttonConfigureReports.Size = new System.Drawing.Size(95, 22);
			this.buttonConfigureReports.Text = "Configure...";
			this.buttonConfigureReports.Click += new System.EventHandler(this.ButtonEditReportsClick);
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
			this.ClientSize = new System.Drawing.Size(1272, 692);
			this.Controls.Add(this.toolStripContainer1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimumSize = new System.Drawing.Size(620, 380);
			this.Name = "MainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Torn 5";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainFormFormClosing);
			this.Shown += new System.EventHandler(this.MainFormShown);
			this.toolStripContainer1.ContentPanel.ResumeLayout(false);
			this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
			this.toolStripContainer1.TopToolStripPanel.PerformLayout();
			this.toolStripContainer1.ResumeLayout(false);
			this.toolStripContainer1.PerformLayout();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.panelGames.ResumeLayout(false);
			this.panelLeague.ResumeLayout(false);
			this.toolStripLeague.ResumeLayout(false);
			this.toolStripLeague.PerformLayout();
			this.toolStripGame.ResumeLayout(false);
			this.toolStripGame.PerformLayout();
			this.toolStripTeams.ResumeLayout(false);
			this.toolStripTeams.PerformLayout();
			this.toolStripReports.ResumeLayout(false);
			this.toolStripReports.PerformLayout();
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem buttonSave;
		private System.Windows.Forms.ToolStripMenuItem buttonHelp;
		private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownGames;
		private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
		private System.Windows.Forms.ToolStripMenuItem buttonTsvExport;
		private System.Windows.Forms.ToolStripMenuItem buttonExportFixtures;
		private System.Windows.Forms.ToolStripMenuItem buttonUpdateScoreboard;
		private System.Windows.Forms.ToolStripMenuItem buttonPackReport;
		private System.Windows.Forms.ToolStripMenuItem buttonAdHocReport;
		private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownReports;
		private System.Windows.Forms.ToolStripMenuItem buttonAbout;
		private System.Windows.Forms.ToolStripMenuItem buttonMatch;
		private System.Windows.Forms.ToolStripMenuItem buttonRememberAllTeams;
		private System.Windows.Forms.ToolStripMenuItem buttonFixtures;
		private System.Windows.Forms.ToolStripMenuItem buttonPyramid;
		private System.Windows.Forms.ToolStripMenuItem buttonPreferences;
		private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownLeagues;
		private System.Windows.Forms.ToolStripButton buttonConfigureReports;
		private System.Windows.Forms.ToolStripButton buttonSetFolder;
		private System.Windows.Forms.ToolStripButton buttonUploadReports;
		private System.Windows.Forms.ToolStripButton buttonExportReports;
		private System.Windows.Forms.ToolStrip toolStripReports;
		private System.Windows.Forms.ToolStripButton buttonEdit;
		private System.Windows.Forms.ToolStripMenuItem buttonEditGame;
		private System.Windows.Forms.ToolStripMenuItem buttonCreateGame;
		private System.Windows.Forms.ToolStrip toolStripTeams;
		private System.Windows.Forms.ToolStripMenuItem buttonForget;
		private System.Windows.Forms.ToolStripButton buttonCommit;
		private System.Windows.Forms.ToolStripMenuItem buttonSetDescription;
		private System.Windows.Forms.ToolStripButton buttonLatestGame;
		private System.Windows.Forms.ToolStrip toolStripGame;
		private System.Windows.Forms.ToolStripButton buttonClose;
		private System.Windows.Forms.SaveFileDialog saveFileDialog1;
		private System.Windows.Forms.ToolStripButton buttonNew;
		private System.Windows.Forms.Button buttonCommit2;
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
		private System.Windows.Forms.Label labelNow;
		private System.Windows.Forms.Timer timerGame;
		private System.Windows.Forms.Label labelTime;
		private System.Windows.Forms.ColumnHeader colTeams;
		private System.Windows.Forms.ColumnHeader colTag;
		private System.Windows.Forms.ImageList imageListLeagues;
		private System.Windows.Forms.ColumnHeader colGames;
		private System.Windows.Forms.ColumnHeader colFile;
		private System.Windows.Forms.ColumnHeader colTitle;
		private System.Windows.Forms.ListView listViewLeagues;
		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
	}
}
