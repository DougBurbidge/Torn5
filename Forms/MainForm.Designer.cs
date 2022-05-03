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
			this.ribbon1 = new System.Windows.Forms.Ribbon();
			this.ribbonTab1 = new System.Windows.Forms.RibbonTab();
			this.ribbonPanelLeagues = new System.Windows.Forms.RibbonPanel();
			this.ribbonButtonNew = new System.Windows.Forms.RibbonButton();
			this.ribbonButtonOpen = new System.Windows.Forms.RibbonButton();
			this.ribbonButtonSave = new System.Windows.Forms.RibbonButton();
			this.ribbonButtonClose = new System.Windows.Forms.RibbonButton();
			this.ribbonButtonEdit = new System.Windows.Forms.RibbonButton();
			this.ribbonButtonPreferences = new System.Windows.Forms.RibbonButton();
			this.ribbonButtonFixtures = new System.Windows.Forms.RibbonButton();
			this.ribbonPanelGames = new System.Windows.Forms.RibbonPanel();
			this.ribbonButtonLatest = new System.Windows.Forms.RibbonButton();
			this.ribbonButtonCommit = new System.Windows.Forms.RibbonButton();
			this.ribbonButtonSetDescription = new System.Windows.Forms.RibbonButton();
			this.ribbonButtonForget = new System.Windows.Forms.RibbonButton();
			this.ribbonPanelTeams = new System.Windows.Forms.RibbonPanel();
			this.ribbonButtonAddRow = new System.Windows.Forms.RibbonButton();
			this.ribbonButtonRemoveRow = new System.Windows.Forms.RibbonButton();
			this.ribbonButtonAddColumn = new System.Windows.Forms.RibbonButton();
			this.ribbonButtonRemoveColumn = new System.Windows.Forms.RibbonButton();
			this.ribbonButtonRememberAllTeams = new System.Windows.Forms.RibbonButton();
			this.ribbonPanelReports = new System.Windows.Forms.RibbonPanel();
			this.ribbonButtonReport = new System.Windows.Forms.RibbonButton();
			this.ribbonButton1 = new System.Windows.Forms.RibbonButton();
			this.ribbonButton2 = new System.Windows.Forms.RibbonButton();
			this.ribbonButtonUpload = new System.Windows.Forms.RibbonButton();
			this.ribbonButtonPrint = new System.Windows.Forms.RibbonButton();
			this.ribbonButtonConfigure = new System.Windows.Forms.RibbonButton();
			this.ribbonButtonAdHoc = new System.Windows.Forms.RibbonButton();
			this.ribbonPanelHelp = new System.Windows.Forms.RibbonPanel();
			this.ribbonButtonHelp = new System.Windows.Forms.RibbonButton();
			this.ribbonButtonAbout = new System.Windows.Forms.RibbonButton();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.panelGames = new System.Windows.Forms.Panel();
			this.listViewGames = new System.Windows.Forms.ListView();
			this.colGame = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.colLeague = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.colDescription = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.listViewLeagues = new System.Windows.Forms.ListView();
			this.colTag = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.colTitle = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.colFile = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.colGames = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.colTeams = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.labelStatus = new System.Windows.Forms.Label();
			this.progressBar1 = new System.Windows.Forms.ProgressBar();
			this.labelNow = new System.Windows.Forms.Label();
			this.labelTime = new System.Windows.Forms.Label();
			this.labelLeagueDetails = new System.Windows.Forms.Label();
			this.imageListPacks = new System.Windows.Forms.ImageList(this.components);
			this.timerGame = new System.Windows.Forms.Timer(this.components);
			this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
			this.printDialog = new System.Windows.Forms.PrintDialog();
			this.contextMenuStripGames = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.commitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.setDescriptionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.forgetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.tableLayoutPanel1.SuspendLayout();
			this.panelGames.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.contextMenuStripGames.SuspendLayout();
			this.SuspendLayout();
			// 
			// openFileDialog1
			// 
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
			// ribbon1
			// 
			this.ribbon1.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.ribbon1.Location = new System.Drawing.Point(0, 0);
			this.ribbon1.Minimized = false;
			this.ribbon1.Name = "ribbon1";
			// 
			// 
			// 
			this.ribbon1.OrbDropDown.BorderRoundness = 8;
			this.ribbon1.OrbDropDown.Location = new System.Drawing.Point(0, 0);
			this.ribbon1.OrbDropDown.Name = "";
			this.ribbon1.OrbDropDown.Size = new System.Drawing.Size(527, 72);
			this.ribbon1.OrbDropDown.TabIndex = 0;
			this.ribbon1.OrbStyle = System.Windows.Forms.RibbonOrbStyle.Office_2013;
			this.ribbon1.OrbText = "";
			this.ribbon1.OrbVisible = false;
			this.ribbon1.RibbonTabFont = new System.Drawing.Font("Trebuchet MS", 9F);
			this.ribbon1.Size = new System.Drawing.Size(1272, 118);
			this.ribbon1.TabIndex = 21;
			this.ribbon1.Tabs.Add(this.ribbonTab1);
			this.ribbon1.TabSpacing = 4;
			// 
			// ribbonTab1
			// 
			this.ribbonTab1.Name = "ribbonTab1";
			this.ribbonTab1.Panels.Add(this.ribbonPanelLeagues);
			this.ribbonTab1.Panels.Add(this.ribbonPanelGames);
			this.ribbonTab1.Panels.Add(this.ribbonPanelTeams);
			this.ribbonTab1.Panels.Add(this.ribbonPanelReports);
			this.ribbonTab1.Panels.Add(this.ribbonPanelHelp);
			this.ribbonTab1.Text = "";
			// 
			// ribbonPanelLeagues
			// 
			this.ribbonPanelLeagues.ButtonMoreVisible = false;
			this.ribbonPanelLeagues.Items.Add(this.ribbonButtonNew);
			this.ribbonPanelLeagues.Items.Add(this.ribbonButtonOpen);
			this.ribbonPanelLeagues.Items.Add(this.ribbonButtonSave);
			this.ribbonPanelLeagues.Items.Add(this.ribbonButtonClose);
			this.ribbonPanelLeagues.Items.Add(this.ribbonButtonEdit);
			this.ribbonPanelLeagues.Items.Add(this.ribbonButtonPreferences);
			this.ribbonPanelLeagues.Items.Add(this.ribbonButtonFixtures);
			this.ribbonPanelLeagues.Name = "ribbonPanelLeagues";
			this.ribbonPanelLeagues.Text = "Leagues";
			// 
			// ribbonButtonNew
			// 
			this.ribbonButtonNew.Image = ((System.Drawing.Image)(resources.GetObject("ribbonButtonNew.Image")));
			this.ribbonButtonNew.LargeImage = ((System.Drawing.Image)(resources.GetObject("ribbonButtonNew.LargeImage")));
			this.ribbonButtonNew.MinSizeMode = System.Windows.Forms.RibbonElementSizeMode.Compact;
			this.ribbonButtonNew.Name = "ribbonButtonNew";
			this.ribbonButtonNew.SmallImage = ((System.Drawing.Image)(resources.GetObject("ribbonButtonNew.SmallImage")));
			this.ribbonButtonNew.Text = "New";
			this.ribbonButtonNew.ToolTip = "New";
			this.ribbonButtonNew.Click += new System.EventHandler(this.ButtonNewClick);
			// 
			// ribbonButtonOpen
			// 
			this.ribbonButtonOpen.Image = ((System.Drawing.Image)(resources.GetObject("ribbonButtonOpen.Image")));
			this.ribbonButtonOpen.LargeImage = ((System.Drawing.Image)(resources.GetObject("ribbonButtonOpen.LargeImage")));
			this.ribbonButtonOpen.Name = "ribbonButtonOpen";
			this.ribbonButtonOpen.SmallImage = ((System.Drawing.Image)(resources.GetObject("ribbonButtonOpen.SmallImage")));
			this.ribbonButtonOpen.Text = "Open";
			this.ribbonButtonOpen.ToolTipTitle = "Open";
			this.ribbonButtonOpen.Click += new System.EventHandler(this.ButtonLoadClick);
			// 
			// ribbonButtonSave
			// 
			this.ribbonButtonSave.Image = ((System.Drawing.Image)(resources.GetObject("ribbonButtonSave.Image")));
			this.ribbonButtonSave.LargeImage = ((System.Drawing.Image)(resources.GetObject("ribbonButtonSave.LargeImage")));
			this.ribbonButtonSave.Name = "ribbonButtonSave";
			this.ribbonButtonSave.SmallImage = ((System.Drawing.Image)(resources.GetObject("ribbonButtonSave.SmallImage")));
			this.ribbonButtonSave.Text = "Save";
			this.ribbonButtonSave.Click += new System.EventHandler(this.ButtonSaveClick);
			// 
			// ribbonButtonClose
			// 
			this.ribbonButtonClose.Image = ((System.Drawing.Image)(resources.GetObject("ribbonButtonClose.Image")));
			this.ribbonButtonClose.LargeImage = ((System.Drawing.Image)(resources.GetObject("ribbonButtonClose.LargeImage")));
			this.ribbonButtonClose.Name = "ribbonButtonClose";
			this.ribbonButtonClose.SmallImage = ((System.Drawing.Image)(resources.GetObject("ribbonButtonClose.SmallImage")));
			this.ribbonButtonClose.Text = "Close";
			this.ribbonButtonClose.Click += new System.EventHandler(this.ButtonCloseClick);
			// 
			// ribbonButtonEdit
			// 
			this.ribbonButtonEdit.Image = ((System.Drawing.Image)(resources.GetObject("ribbonButtonEdit.Image")));
			this.ribbonButtonEdit.LargeImage = ((System.Drawing.Image)(resources.GetObject("ribbonButtonEdit.LargeImage")));
			this.ribbonButtonEdit.Name = "ribbonButtonEdit";
			this.ribbonButtonEdit.SmallImage = ((System.Drawing.Image)(resources.GetObject("ribbonButtonEdit.SmallImage")));
			this.ribbonButtonEdit.Text = "Edit";
			this.ribbonButtonEdit.Click += new System.EventHandler(this.ButtonEditLeagueClick);
			// 
			// ribbonButtonPreferences
			// 
			this.ribbonButtonPreferences.Image = ((System.Drawing.Image)(resources.GetObject("ribbonButtonPreferences.Image")));
			this.ribbonButtonPreferences.LargeImage = ((System.Drawing.Image)(resources.GetObject("ribbonButtonPreferences.LargeImage")));
			this.ribbonButtonPreferences.Name = "ribbonButtonPreferences";
			this.ribbonButtonPreferences.SmallImage = ((System.Drawing.Image)(resources.GetObject("ribbonButtonPreferences.SmallImage")));
			this.ribbonButtonPreferences.Text = "Preferences";
			this.ribbonButtonPreferences.Click += new System.EventHandler(this.ButtonPreferencesClick);
			// 
			// ribbonButtonFixtures
			// 
			this.ribbonButtonFixtures.Image = ((System.Drawing.Image)(resources.GetObject("ribbonButtonFixtures.Image")));
			this.ribbonButtonFixtures.LargeImage = ((System.Drawing.Image)(resources.GetObject("ribbonButtonFixtures.LargeImage")));
			this.ribbonButtonFixtures.Name = "ribbonButtonFixtures";
			this.ribbonButtonFixtures.SmallImage = ((System.Drawing.Image)(resources.GetObject("ribbonButtonFixtures.SmallImage")));
			this.ribbonButtonFixtures.Text = "Fixtures";
			this.ribbonButtonFixtures.Click += new System.EventHandler(this.ButtonFixtureClick);
			// 
			// ribbonPanelGames
			// 
			this.ribbonPanelGames.ButtonMoreVisible = false;
			this.ribbonPanelGames.Items.Add(this.ribbonButtonLatest);
			this.ribbonPanelGames.Items.Add(this.ribbonButtonCommit);
			this.ribbonPanelGames.Items.Add(this.ribbonButtonSetDescription);
			this.ribbonPanelGames.Items.Add(this.ribbonButtonForget);
			this.ribbonPanelGames.Name = "ribbonPanelGames";
			this.ribbonPanelGames.Text = "Games";
			// 
			// ribbonButtonLatest
			// 
			this.ribbonButtonLatest.AltKey = "L";
			this.ribbonButtonLatest.Image = ((System.Drawing.Image)(resources.GetObject("ribbonButtonLatest.Image")));
			this.ribbonButtonLatest.LargeImage = ((System.Drawing.Image)(resources.GetObject("ribbonButtonLatest.LargeImage")));
			this.ribbonButtonLatest.MinSizeMode = System.Windows.Forms.RibbonElementSizeMode.Large;
			this.ribbonButtonLatest.Name = "ribbonButtonLatest";
			this.ribbonButtonLatest.SmallImage = ((System.Drawing.Image)(resources.GetObject("ribbonButtonLatest.SmallImage")));
			this.ribbonButtonLatest.Text = "Latest";
			this.ribbonButtonLatest.Click += new System.EventHandler(this.ButtonLatestGameClick);
			// 
			// ribbonButtonCommit
			// 
			this.ribbonButtonCommit.AltKey = "C";
			this.ribbonButtonCommit.Image = ((System.Drawing.Image)(resources.GetObject("ribbonButtonCommit.Image")));
			this.ribbonButtonCommit.LargeImage = ((System.Drawing.Image)(resources.GetObject("ribbonButtonCommit.LargeImage")));
			this.ribbonButtonCommit.MinSizeMode = System.Windows.Forms.RibbonElementSizeMode.Large;
			this.ribbonButtonCommit.Name = "ribbonButtonCommit";
			this.ribbonButtonCommit.SmallImage = ((System.Drawing.Image)(resources.GetObject("ribbonButtonCommit.SmallImage")));
			this.ribbonButtonCommit.Text = "Commit";
			this.ribbonButtonCommit.Click += new System.EventHandler(this.ButtonCommitClick);
			// 
			// ribbonButtonSetDescription
			// 
			this.ribbonButtonSetDescription.Image = ((System.Drawing.Image)(resources.GetObject("ribbonButtonSetDescription.Image")));
			this.ribbonButtonSetDescription.LargeImage = ((System.Drawing.Image)(resources.GetObject("ribbonButtonSetDescription.LargeImage")));
			this.ribbonButtonSetDescription.Name = "ribbonButtonSetDescription";
			this.ribbonButtonSetDescription.SmallImage = ((System.Drawing.Image)(resources.GetObject("ribbonButtonSetDescription.SmallImage")));
			this.ribbonButtonSetDescription.Text = "Set Description";
			this.ribbonButtonSetDescription.Click += new System.EventHandler(this.ButtonSetDescriptionClick);
			// 
			// ribbonButtonForget
			// 
			this.ribbonButtonForget.Image = ((System.Drawing.Image)(resources.GetObject("ribbonButtonForget.Image")));
			this.ribbonButtonForget.LargeImage = ((System.Drawing.Image)(resources.GetObject("ribbonButtonForget.LargeImage")));
			this.ribbonButtonForget.Name = "ribbonButtonForget";
			this.ribbonButtonForget.SmallImage = ((System.Drawing.Image)(resources.GetObject("ribbonButtonForget.SmallImage")));
			this.ribbonButtonForget.Text = "Forget";
			this.ribbonButtonForget.Click += new System.EventHandler(this.ButtonForgetClick);
			// 
			// ribbonPanelTeams
			// 
			this.ribbonPanelTeams.ButtonMoreVisible = false;
			this.ribbonPanelTeams.Items.Add(this.ribbonButtonAddRow);
			this.ribbonPanelTeams.Items.Add(this.ribbonButtonRemoveRow);
			this.ribbonPanelTeams.Items.Add(this.ribbonButtonAddColumn);
			this.ribbonPanelTeams.Items.Add(this.ribbonButtonRemoveColumn);
			this.ribbonPanelTeams.Items.Add(this.ribbonButtonRememberAllTeams);
			this.ribbonPanelTeams.Name = "ribbonPanelTeams";
			this.ribbonPanelTeams.Text = "Teams";
			// 
			// ribbonButtonAddRow
			// 
			this.ribbonButtonAddRow.Image = ((System.Drawing.Image)(resources.GetObject("ribbonButtonAddRow.Image")));
			this.ribbonButtonAddRow.LargeImage = ((System.Drawing.Image)(resources.GetObject("ribbonButtonAddRow.LargeImage")));
			this.ribbonButtonAddRow.Name = "ribbonButtonAddRow";
			this.ribbonButtonAddRow.SmallImage = ((System.Drawing.Image)(resources.GetObject("ribbonButtonAddRow.SmallImage")));
			this.ribbonButtonAddRow.Text = "Add Row";
			this.ribbonButtonAddRow.Click += new System.EventHandler(this.ButtonAddRowClick);
			// 
			// ribbonButtonRemoveRow
			// 
			this.ribbonButtonRemoveRow.Enabled = false;
			this.ribbonButtonRemoveRow.Image = ((System.Drawing.Image)(resources.GetObject("ribbonButtonRemoveRow.Image")));
			this.ribbonButtonRemoveRow.LargeImage = ((System.Drawing.Image)(resources.GetObject("ribbonButtonRemoveRow.LargeImage")));
			this.ribbonButtonRemoveRow.Name = "ribbonButtonRemoveRow";
			this.ribbonButtonRemoveRow.SmallImage = ((System.Drawing.Image)(resources.GetObject("ribbonButtonRemoveRow.SmallImage")));
			this.ribbonButtonRemoveRow.Text = "Remove Row";
			this.ribbonButtonRemoveRow.Click += new System.EventHandler(this.ButtonRemoveRowClick);
			// 
			// ribbonButtonAddColumn
			// 
			this.ribbonButtonAddColumn.Image = ((System.Drawing.Image)(resources.GetObject("ribbonButtonAddColumn.Image")));
			this.ribbonButtonAddColumn.LargeImage = ((System.Drawing.Image)(resources.GetObject("ribbonButtonAddColumn.LargeImage")));
			this.ribbonButtonAddColumn.Name = "ribbonButtonAddColumn";
			this.ribbonButtonAddColumn.SmallImage = ((System.Drawing.Image)(resources.GetObject("ribbonButtonAddColumn.SmallImage")));
			this.ribbonButtonAddColumn.Text = "Add Column";
			this.ribbonButtonAddColumn.Click += new System.EventHandler(this.ButtonAddColumnClick);
			// 
			// ribbonButtonRemoveColumn
			// 
			this.ribbonButtonRemoveColumn.Enabled = false;
			this.ribbonButtonRemoveColumn.Image = ((System.Drawing.Image)(resources.GetObject("ribbonButtonRemoveColumn.Image")));
			this.ribbonButtonRemoveColumn.LargeImage = ((System.Drawing.Image)(resources.GetObject("ribbonButtonRemoveColumn.LargeImage")));
			this.ribbonButtonRemoveColumn.Name = "ribbonButtonRemoveColumn";
			this.ribbonButtonRemoveColumn.SmallImage = ((System.Drawing.Image)(resources.GetObject("ribbonButtonRemoveColumn.SmallImage")));
			this.ribbonButtonRemoveColumn.Text = "Remove Column";
			this.ribbonButtonRemoveColumn.Click += new System.EventHandler(this.ButtonRemoveColumnClick);
			// 
			// ribbonButtonRememberAllTeams
			// 
			this.ribbonButtonRememberAllTeams.Image = ((System.Drawing.Image)(resources.GetObject("ribbonButtonRememberAllTeams.Image")));
			this.ribbonButtonRememberAllTeams.LargeImage = ((System.Drawing.Image)(resources.GetObject("ribbonButtonRememberAllTeams.LargeImage")));
			this.ribbonButtonRememberAllTeams.Name = "ribbonButtonRememberAllTeams";
			this.ribbonButtonRememberAllTeams.SmallImage = ((System.Drawing.Image)(resources.GetObject("ribbonButtonRememberAllTeams.SmallImage")));
			this.ribbonButtonRememberAllTeams.Text = "Remember Teams";
			this.ribbonButtonRememberAllTeams.Click += new System.EventHandler(this.ButtonRememberAllTeamsClick);
			// 
			// ribbonPanelReports
			// 
			this.ribbonPanelReports.ButtonMoreVisible = false;
			this.ribbonPanelReports.Items.Add(this.ribbonButtonReport);
			this.ribbonPanelReports.Items.Add(this.ribbonButtonUpload);
			this.ribbonPanelReports.Items.Add(this.ribbonButtonPrint);
			this.ribbonPanelReports.Items.Add(this.ribbonButtonConfigure);
			this.ribbonPanelReports.Items.Add(this.ribbonButtonAdHoc);
			this.ribbonPanelReports.Name = "ribbonPanelReports";
			this.ribbonPanelReports.Text = "Reports";
			// 
			// ribbonButtonReport
			// 
			this.ribbonButtonReport.DropDownItems.Add(this.ribbonButton1);
			this.ribbonButtonReport.DropDownItems.Add(this.ribbonButton2);
			this.ribbonButtonReport.Image = ((System.Drawing.Image)(resources.GetObject("ribbonButtonReport.Image")));
			this.ribbonButtonReport.LargeImage = ((System.Drawing.Image)(resources.GetObject("ribbonButtonReport.LargeImage")));
			this.ribbonButtonReport.Name = "ribbonButtonReport";
			this.ribbonButtonReport.SmallImage = ((System.Drawing.Image)(resources.GetObject("ribbonButtonReport.SmallImage")));
			this.ribbonButtonReport.Text = "Report";
			this.ribbonButtonReport.Click += new System.EventHandler(this.ButtonExportClick);
			// 
			// ribbonButton1
			// 
			this.ribbonButton1.Image = ((System.Drawing.Image)(resources.GetObject("ribbonButton1.Image")));
			this.ribbonButton1.LargeImage = ((System.Drawing.Image)(resources.GetObject("ribbonButton1.LargeImage")));
			this.ribbonButton1.Name = "ribbonButton1";
			this.ribbonButton1.SmallImage = ((System.Drawing.Image)(resources.GetObject("ribbonButton1.SmallImage")));
			this.ribbonButton1.Text = "ribbonButton1";
			// 
			// ribbonButton2
			// 
			this.ribbonButton2.Image = ((System.Drawing.Image)(resources.GetObject("ribbonButton2.Image")));
			this.ribbonButton2.LargeImage = ((System.Drawing.Image)(resources.GetObject("ribbonButton2.LargeImage")));
			this.ribbonButton2.Name = "ribbonButton2";
			this.ribbonButton2.SmallImage = ((System.Drawing.Image)(resources.GetObject("ribbonButton2.SmallImage")));
			this.ribbonButton2.Text = "ribbonButton2";
			// 
			// ribbonButtonUpload
			// 
			this.ribbonButtonUpload.Image = ((System.Drawing.Image)(resources.GetObject("ribbonButtonUpload.Image")));
			this.ribbonButtonUpload.LargeImage = ((System.Drawing.Image)(resources.GetObject("ribbonButtonUpload.LargeImage")));
			this.ribbonButtonUpload.Name = "ribbonButtonUpload";
			this.ribbonButtonUpload.SmallImage = ((System.Drawing.Image)(resources.GetObject("ribbonButtonUpload.SmallImage")));
			this.ribbonButtonUpload.Text = "Upload";
			this.ribbonButtonUpload.Click += new System.EventHandler(this.ButtonUploadClick);
			// 
			// ribbonButtonPrint
			// 
			this.ribbonButtonPrint.AltKey = "P";
			this.ribbonButtonPrint.Image = ((System.Drawing.Image)(resources.GetObject("ribbonButtonPrint.Image")));
			this.ribbonButtonPrint.LargeImage = ((System.Drawing.Image)(resources.GetObject("ribbonButtonPrint.LargeImage")));
			this.ribbonButtonPrint.Name = "ribbonButtonPrint";
			this.ribbonButtonPrint.SmallImage = ((System.Drawing.Image)(resources.GetObject("ribbonButtonPrint.SmallImage")));
			this.ribbonButtonPrint.Text = "Print";
			this.ribbonButtonPrint.Click += new System.EventHandler(this.ButtonPrintReportsClick);
			// 
			// ribbonButtonConfigure
			// 
			this.ribbonButtonConfigure.Image = ((System.Drawing.Image)(resources.GetObject("ribbonButtonConfigure.Image")));
			this.ribbonButtonConfigure.LargeImage = ((System.Drawing.Image)(resources.GetObject("ribbonButtonConfigure.LargeImage")));
			this.ribbonButtonConfigure.Name = "ribbonButtonConfigure";
			this.ribbonButtonConfigure.SmallImage = ((System.Drawing.Image)(resources.GetObject("ribbonButtonConfigure.SmallImage")));
			this.ribbonButtonConfigure.Text = "Configure";
			this.ribbonButtonConfigure.Click += new System.EventHandler(this.ButtonEditReportsClick);
			// 
			// ribbonButtonAdHoc
			// 
			this.ribbonButtonAdHoc.Image = ((System.Drawing.Image)(resources.GetObject("ribbonButtonAdHoc.Image")));
			this.ribbonButtonAdHoc.LargeImage = ((System.Drawing.Image)(resources.GetObject("ribbonButtonAdHoc.LargeImage")));
			this.ribbonButtonAdHoc.Name = "ribbonButtonAdHoc";
			this.ribbonButtonAdHoc.SmallImage = ((System.Drawing.Image)(resources.GetObject("ribbonButtonAdHoc.SmallImage")));
			this.ribbonButtonAdHoc.Text = "Ad Hoc";
			this.ribbonButtonAdHoc.Click += new System.EventHandler(this.ButtonAdHocReportClick);
			// 
			// ribbonPanelHelp
			// 
			this.ribbonPanelHelp.ButtonMoreVisible = false;
			this.ribbonPanelHelp.Items.Add(this.ribbonButtonHelp);
			this.ribbonPanelHelp.Items.Add(this.ribbonButtonAbout);
			this.ribbonPanelHelp.Name = "ribbonPanelHelp";
			this.ribbonPanelHelp.Text = "";
			// 
			// ribbonButtonHelp
			// 
			this.ribbonButtonHelp.Image = ((System.Drawing.Image)(resources.GetObject("ribbonButtonHelp.Image")));
			this.ribbonButtonHelp.LargeImage = ((System.Drawing.Image)(resources.GetObject("ribbonButtonHelp.LargeImage")));
			this.ribbonButtonHelp.Name = "ribbonButtonHelp";
			this.ribbonButtonHelp.SmallImage = ((System.Drawing.Image)(resources.GetObject("ribbonButtonHelp.SmallImage")));
			this.ribbonButtonHelp.Text = "Help";
			this.ribbonButtonHelp.Click += new System.EventHandler(this.ButtonHelpClick);
			// 
			// ribbonButtonAbout
			// 
			this.ribbonButtonAbout.Image = ((System.Drawing.Image)(resources.GetObject("ribbonButtonAbout.Image")));
			this.ribbonButtonAbout.LargeImage = ((System.Drawing.Image)(resources.GetObject("ribbonButtonAbout.LargeImage")));
			this.ribbonButtonAbout.Name = "ribbonButtonAbout";
			this.ribbonButtonAbout.SmallImage = ((System.Drawing.Image)(resources.GetObject("ribbonButtonAbout.SmallImage")));
			this.ribbonButtonAbout.Text = "About";
			this.ribbonButtonAbout.Click += new System.EventHandler(this.ButtonAboutClick);
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel1.ColumnCount = 4;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.tableLayoutPanel1.Controls.Add(this.panelGames, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.splitContainer1, 0, 0);
			this.tableLayoutPanel1.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 118);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(1272, 574);
			this.tableLayoutPanel1.TabIndex = 20;
			// 
			// panelGames
			// 
			this.panelGames.Controls.Add(this.listViewGames);
			this.panelGames.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelGames.Location = new System.Drawing.Point(318, 0);
			this.panelGames.Margin = new System.Windows.Forms.Padding(0);
			this.panelGames.Name = "panelGames";
			this.tableLayoutPanel1.SetRowSpan(this.panelGames, 2);
			this.panelGames.Size = new System.Drawing.Size(318, 574);
			this.panelGames.TabIndex = 15;
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
			this.listViewGames.Size = new System.Drawing.Size(315, 568);
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
			this.colDescription.Width = 75;
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(3, 3);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.listViewLeagues);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.labelStatus);
			this.splitContainer1.Panel2.Controls.Add(this.progressBar1);
			this.splitContainer1.Panel2.Controls.Add(this.labelNow);
			this.splitContainer1.Panel2.Controls.Add(this.labelTime);
			this.splitContainer1.Panel2.Controls.Add(this.labelLeagueDetails);
			this.tableLayoutPanel1.SetRowSpan(this.splitContainer1, 2);
			this.splitContainer1.Size = new System.Drawing.Size(312, 568);
			this.splitContainer1.SplitterDistance = 347;
			this.splitContainer1.TabIndex = 17;
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
			this.listViewLeagues.Location = new System.Drawing.Point(0, 0);
			this.listViewLeagues.Name = "listViewLeagues";
			this.listViewLeagues.Size = new System.Drawing.Size(312, 347);
			this.listViewLeagues.SmallImageList = this.imageListLeagues;
			this.listViewLeagues.TabIndex = 0;
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
			// labelStatus
			// 
			this.labelStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.labelStatus.Location = new System.Drawing.Point(3, 165);
			this.labelStatus.Name = "labelStatus";
			this.labelStatus.Size = new System.Drawing.Size(306, 21);
			this.labelStatus.TabIndex = 3;
			// 
			// progressBar1
			// 
			this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.progressBar1.Location = new System.Drawing.Point(3, 189);
			this.progressBar1.Maximum = 1000;
			this.progressBar1.Name = "progressBar1";
			this.progressBar1.Size = new System.Drawing.Size(306, 23);
			this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			this.progressBar1.TabIndex = 4;
			this.progressBar1.Visible = false;
			// 
			// labelNow
			// 
			this.labelNow.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.labelNow.Location = new System.Drawing.Point(12, 92);
			this.labelNow.Name = "labelNow";
			this.labelNow.Size = new System.Drawing.Size(297, 64);
			this.labelNow.TabIndex = 2;
			this.labelNow.Text = "Now Playing:";
			// 
			// labelTime
			// 
			this.labelTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.labelTime.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelTime.Location = new System.Drawing.Point(12, 61);
			this.labelTime.Name = "labelTime";
			this.labelTime.Size = new System.Drawing.Size(297, 23);
			this.labelTime.TabIndex = 1;
			this.labelTime.Text = "Time";
			// 
			// labelLeagueDetails
			// 
			this.labelLeagueDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.labelLeagueDetails.Location = new System.Drawing.Point(0, 0);
			this.labelLeagueDetails.Name = "labelLeagueDetails";
			this.labelLeagueDetails.Size = new System.Drawing.Size(312, 61);
			this.labelLeagueDetails.TabIndex = 0;
			this.labelLeagueDetails.Text = "\nSelect a league and its details will appear here.";
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
			this.imageListPacks.Images.SetKeyName(9, "white.png");
			this.imageListPacks.Images.SetKeyName(10, "black.png");
			this.imageListPacks.Images.SetKeyName(11, "fire.png");
			this.imageListPacks.Images.SetKeyName(12, "ice.png");
			this.imageListPacks.Images.SetKeyName(13, "earth.png");
			this.imageListPacks.Images.SetKeyName(14, "crystal.png");
			this.imageListPacks.Images.SetKeyName(15, "rainbow.png");
			this.imageListPacks.Images.SetKeyName(16, "cops.png");
			this.imageListPacks.Images.SetKeyName(17, "cops.png");
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
			// printDialog
			// 
			this.printDialog.AllowSomePages = true;
			// 
			// contextMenuStripGames
			// 
			this.contextMenuStripGames.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.commitToolStripMenuItem,
            this.setDescriptionToolStripMenuItem,
            this.forgetToolStripMenuItem});
			this.contextMenuStripGames.Name = "contextMenuStripGames";
			this.contextMenuStripGames.Size = new System.Drawing.Size(154, 70);
			// 
			// commitToolStripMenuItem
			// 
			this.commitToolStripMenuItem.Name = "commitToolStripMenuItem";
			this.commitToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
			this.commitToolStripMenuItem.Text = "Commit";
			this.commitToolStripMenuItem.Click += new System.EventHandler(this.ButtonCommitClick);
			// 
			// setDescriptionToolStripMenuItem
			// 
			this.setDescriptionToolStripMenuItem.Name = "setDescriptionToolStripMenuItem";
			this.setDescriptionToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
			this.setDescriptionToolStripMenuItem.Text = "Set Description";
			this.setDescriptionToolStripMenuItem.Click += new System.EventHandler(this.ButtonSetDescriptionClick);
			// 
			// forgetToolStripMenuItem
			// 
			this.forgetToolStripMenuItem.Name = "forgetToolStripMenuItem";
			this.forgetToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
			this.forgetToolStripMenuItem.Text = "Forget";
			this.forgetToolStripMenuItem.Click += new System.EventHandler(this.ButtonForgetClick);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.ClientSize = new System.Drawing.Size(1272, 692);
			this.Controls.Add(this.ribbon1);
			this.Controls.Add(this.tableLayoutPanel1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.MinimumSize = new System.Drawing.Size(620, 380);
			this.Name = "MainForm";
			this.Text = "Torn 5";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainFormFormClosing);
			this.Shown += new System.EventHandler(this.MainFormShown);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.panelGames.ResumeLayout(false);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.contextMenuStripGames.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		private System.Windows.Forms.SaveFileDialog saveFileDialog1;
		private System.Windows.Forms.Panel panelGames;
		private System.Windows.Forms.ImageList imageListPacks;
		private System.Windows.Forms.ColumnHeader colDescription;
		private System.Windows.Forms.ColumnHeader colLeague;
		private System.Windows.Forms.ColumnHeader colGame;
		private System.Windows.Forms.ListView listViewGames;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Timer timerGame;
		private System.Windows.Forms.ImageList imageListLeagues;
		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.ListView listViewLeagues;
		private System.Windows.Forms.ColumnHeader colTag;
		private System.Windows.Forms.ColumnHeader colTitle;
		private System.Windows.Forms.ColumnHeader colFile;
		private System.Windows.Forms.ColumnHeader colGames;
		private System.Windows.Forms.ColumnHeader colTeams;
		private System.Windows.Forms.Label labelStatus;
		private System.Windows.Forms.ProgressBar progressBar1;
		private System.Windows.Forms.Label labelNow;
		private System.Windows.Forms.Label labelTime;
		private System.Windows.Forms.Label labelLeagueDetails;
		private System.Windows.Forms.PrintDialog printDialog;
		private System.Windows.Forms.Ribbon ribbon1;
		private System.Windows.Forms.RibbonTab ribbonTab1;
		private System.Windows.Forms.RibbonPanel ribbonPanelLeagues;
		private System.Windows.Forms.RibbonButton ribbonButtonNew;
		private System.Windows.Forms.RibbonButton ribbonButtonOpen;
		private System.Windows.Forms.RibbonButton ribbonButtonClose;
		private System.Windows.Forms.RibbonButton ribbonButtonEdit;
		private System.Windows.Forms.RibbonPanel ribbonPanelGames;
		private System.Windows.Forms.RibbonButton ribbonButtonSetDescription;
		private System.Windows.Forms.RibbonButton ribbonButtonForget;
		private System.Windows.Forms.RibbonButton ribbonButtonLatest;
		private System.Windows.Forms.RibbonButton ribbonButtonCommit;
		private System.Windows.Forms.RibbonPanel ribbonPanelTeams;
		private System.Windows.Forms.RibbonButton ribbonButtonRememberAllTeams;
		private System.Windows.Forms.RibbonButton ribbonButtonAddRow;
		private System.Windows.Forms.RibbonButton ribbonButtonRemoveRow;
		private System.Windows.Forms.RibbonButton ribbonButtonAddColumn;
		private System.Windows.Forms.RibbonButton ribbonButtonRemoveColumn;
		private System.Windows.Forms.RibbonPanel ribbonPanelReports;
		private System.Windows.Forms.RibbonButton ribbonButtonAdHoc;
		private System.Windows.Forms.RibbonButton ribbonButtonReport;
		private System.Windows.Forms.RibbonButton ribbonButton1;
		private System.Windows.Forms.RibbonButton ribbonButton2;
		private System.Windows.Forms.RibbonButton ribbonButtonUpload;
		private System.Windows.Forms.RibbonButton ribbonButtonPrint;
		private System.Windows.Forms.RibbonButton ribbonButtonConfigure;
		private System.Windows.Forms.RibbonButton ribbonButtonPreferences;
		private System.Windows.Forms.RibbonPanel ribbonPanelHelp;
		private System.Windows.Forms.RibbonButton ribbonButtonHelp;
		private System.Windows.Forms.RibbonButton ribbonButtonAbout;
		private System.Windows.Forms.RibbonButton ribbonButtonFixtures;
		private System.Windows.Forms.RibbonButton ribbonButtonSave;
		private System.Windows.Forms.ContextMenuStrip contextMenuStripGames;
		private System.Windows.Forms.ToolStripMenuItem commitToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem setDescriptionToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem forgetToolStripMenuItem;
	}
}
