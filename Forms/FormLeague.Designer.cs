
namespace Torn.UI
{
	partial class FormLeague
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
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteTeamMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameTeamMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deletePlayerMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reIDPlayerMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonCopyFromLeague = new System.Windows.Forms.Button();
            this.buttonRenameLeague = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.buttonRenameTeam = new System.Windows.Forms.Button();
            this.buttonDeleteTeam = new System.Windows.Forms.Button();
            this.buttonAddTeam = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.buttonReIdPlayer = new System.Windows.Forms.Button();
            this.buttonDeletePlayer = new System.Windows.Forms.Button();
            this.buttonAddPlayer = new System.Windows.Forms.Button();
            this.panelRight = new System.Windows.Forms.Panel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.leaguePage = new System.Windows.Forms.TabPage();
            this.groupBoxHandicapStyle = new System.Windows.Forms.GroupBox();
            this.radioButtonMinus = new System.Windows.Forms.RadioButton();
            this.radioButtonPlus = new System.Windows.Forms.RadioButton();
            this.radioButtonNone = new System.Windows.Forms.RadioButton();
            this.radioButtonPercent = new System.Windows.Forms.RadioButton();
            this.victoryPoints = new System.Windows.Forms.RadioButton();
            this.totalScore = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.scoresPage = new System.Windows.Forms.TabPage();
            this.listViewScores = new System.Windows.Forms.ListView();
            this.colGame = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colScore = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colRankorPoints = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.GradesPage = new System.Windows.Forms.TabPage();
            this.IBonus = new System.Windows.Forms.CheckBox();
            this.HBonus = new System.Windows.Forms.CheckBox();
            this.GBonus = new System.Windows.Forms.CheckBox();
            this.FBonus = new System.Windows.Forms.CheckBox();
            this.EBonus = new System.Windows.Forms.CheckBox();
            this.DBonus = new System.Windows.Forms.CheckBox();
            this.CBonus = new System.Windows.Forms.CheckBox();
            this.BBonus = new System.Windows.Forms.CheckBox();
            this.BBBonus = new System.Windows.Forms.CheckBox();
            this.ABonus = new System.Windows.Forms.CheckBox();
            this.AAABonus = new System.Windows.Forms.CheckBox();
            this.IPenalty = new System.Windows.Forms.CheckBox();
            this.HPenalty = new System.Windows.Forms.CheckBox();
            this.GPenalty = new System.Windows.Forms.CheckBox();
            this.FPenalty = new System.Windows.Forms.CheckBox();
            this.EPenalty = new System.Windows.Forms.CheckBox();
            this.DPenalty = new System.Windows.Forms.CheckBox();
            this.CPenalty = new System.Windows.Forms.CheckBox();
            this.BPenalty = new System.Windows.Forms.CheckBox();
            this.BBPenalty = new System.Windows.Forms.CheckBox();
            this.APenalty = new System.Windows.Forms.CheckBox();
            this.AAAPenalty = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.IPoints = new System.Windows.Forms.NumericUpDown();
            this.IName = new System.Windows.Forms.TextBox();
            this.HPoints = new System.Windows.Forms.NumericUpDown();
            this.HName = new System.Windows.Forms.TextBox();
            this.GPoints = new System.Windows.Forms.NumericUpDown();
            this.GName = new System.Windows.Forms.TextBox();
            this.FPoints = new System.Windows.Forms.NumericUpDown();
            this.FName = new System.Windows.Forms.TextBox();
            this.EPoints = new System.Windows.Forms.NumericUpDown();
            this.EName = new System.Windows.Forms.TextBox();
            this.DPoints = new System.Windows.Forms.NumericUpDown();
            this.DName = new System.Windows.Forms.TextBox();
            this.CPoints = new System.Windows.Forms.NumericUpDown();
            this.CName = new System.Windows.Forms.TextBox();
            this.BPoints = new System.Windows.Forms.NumericUpDown();
            this.BName = new System.Windows.Forms.TextBox();
            this.BBPoints = new System.Windows.Forms.NumericUpDown();
            this.BBName = new System.Windows.Forms.TextBox();
            this.APoints = new System.Windows.Forms.NumericUpDown();
            this.AName = new System.Windows.Forms.TextBox();
            this.AAAPoints = new System.Windows.Forms.NumericUpDown();
            this.AAAName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.HandicapPage = new System.Windows.Forms.TabPage();
            this.label6 = new System.Windows.Forms.Label();
            this.teamSize = new System.Windows.Forms.NumericUpDown();
            this.automaticHandicapEnabled = new System.Windows.Forms.CheckBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.missingPlayerPenalty = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.extraAPenalty = new System.Windows.Forms.NumericUpDown();
            this.extraGBonusLabel = new System.Windows.Forms.Label();
            this.extraGBonus = new System.Windows.Forms.NumericUpDown();
            this.contextMenuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.panelRight.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.leaguePage.SuspendLayout();
            this.groupBoxHandicapStyle.SuspendLayout();
            this.scoresPage.SuspendLayout();
            this.GradesPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.IPoints)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.HPoints)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.GPoints)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.FPoints)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.EPoints)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DPoints)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CPoints)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BPoints)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BBPoints)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.APoints)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.AAAPoints)).BeginInit();
            this.HandicapPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.teamSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.missingPlayerPenalty)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.extraAPenalty)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.extraGBonus)).BeginInit();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView1.ContextMenuStrip = this.contextMenuStrip1;
            this.treeView1.HideSelection = false;
            this.treeView1.Location = new System.Drawing.Point(12, 12);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(312, 463);
            this.treeView1.TabIndex = 0;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TreeView1AfterSelect);
            this.treeView1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TreeView1MouseClick);
            this.treeView1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.TreeView1MouseMove);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteTeamMenuItem,
            this.renameTeamMenuItem,
            this.deletePlayerMenuItem,
            this.reIDPlayerMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(118, 92);
            // 
            // deleteTeamMenuItem
            // 
            this.deleteTeamMenuItem.Name = "deleteTeamMenuItem";
            this.deleteTeamMenuItem.Size = new System.Drawing.Size(117, 22);
            this.deleteTeamMenuItem.Text = "Delete";
            this.deleteTeamMenuItem.Click += new System.EventHandler(this.ButtonDeleteTeamClick);
            // 
            // renameTeamMenuItem
            // 
            this.renameTeamMenuItem.Name = "renameTeamMenuItem";
            this.renameTeamMenuItem.Size = new System.Drawing.Size(117, 22);
            this.renameTeamMenuItem.Text = "Rename";
            this.renameTeamMenuItem.Click += new System.EventHandler(this.ButtonRenameTeamClick);
            // 
            // deletePlayerMenuItem
            // 
            this.deletePlayerMenuItem.Name = "deletePlayerMenuItem";
            this.deletePlayerMenuItem.Size = new System.Drawing.Size(117, 22);
            this.deletePlayerMenuItem.Text = "Delete";
            this.deletePlayerMenuItem.Click += new System.EventHandler(this.ButtonDeletePlayerClick);
            // 
            // reIDPlayerMenuItem
            // 
            this.reIDPlayerMenuItem.Name = "reIDPlayerMenuItem";
            this.reIDPlayerMenuItem.Size = new System.Drawing.Size(117, 22);
            this.reIDPlayerMenuItem.Text = "Re-ID";
            this.reIDPlayerMenuItem.Click += new System.EventHandler(this.ButtonReIdPlayerClick);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.buttonCopyFromLeague);
            this.groupBox1.Controls.Add(this.buttonRenameLeague);
            this.groupBox1.Location = new System.Drawing.Point(330, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(87, 106);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "League";
            // 
            // buttonCopyFromLeague
            // 
            this.buttonCopyFromLeague.Enabled = false;
            this.buttonCopyFromLeague.Location = new System.Drawing.Point(6, 48);
            this.buttonCopyFromLeague.Name = "buttonCopyFromLeague";
            this.buttonCopyFromLeague.Size = new System.Drawing.Size(75, 23);
            this.buttonCopyFromLeague.TabIndex = 1;
            this.buttonCopyFromLeague.Text = "Copy from";
            this.buttonCopyFromLeague.UseVisualStyleBackColor = true;
            this.buttonCopyFromLeague.Click += new System.EventHandler(this.ButtonCopyFromLeagueClick);
            // 
            // buttonRenameLeague
            // 
            this.buttonRenameLeague.Location = new System.Drawing.Point(6, 19);
            this.buttonRenameLeague.Name = "buttonRenameLeague";
            this.buttonRenameLeague.Size = new System.Drawing.Size(75, 23);
            this.buttonRenameLeague.TabIndex = 0;
            this.buttonRenameLeague.Text = "Rename";
            this.buttonRenameLeague.UseVisualStyleBackColor = true;
            this.buttonRenameLeague.Click += new System.EventHandler(this.ButtonRenameLeagueClick);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.buttonRenameTeam);
            this.groupBox2.Controls.Add(this.buttonDeleteTeam);
            this.groupBox2.Controls.Add(this.buttonAddTeam);
            this.groupBox2.Location = new System.Drawing.Point(423, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(87, 106);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Team";
            // 
            // buttonRenameTeam
            // 
            this.buttonRenameTeam.Location = new System.Drawing.Point(6, 77);
            this.buttonRenameTeam.Name = "buttonRenameTeam";
            this.buttonRenameTeam.Size = new System.Drawing.Size(75, 23);
            this.buttonRenameTeam.TabIndex = 2;
            this.buttonRenameTeam.Text = "Rename";
            this.buttonRenameTeam.UseVisualStyleBackColor = true;
            this.buttonRenameTeam.Click += new System.EventHandler(this.ButtonRenameTeamClick);
            // 
            // buttonDeleteTeam
            // 
            this.buttonDeleteTeam.Location = new System.Drawing.Point(6, 48);
            this.buttonDeleteTeam.Name = "buttonDeleteTeam";
            this.buttonDeleteTeam.Size = new System.Drawing.Size(75, 23);
            this.buttonDeleteTeam.TabIndex = 1;
            this.buttonDeleteTeam.Text = "Delete";
            this.buttonDeleteTeam.UseVisualStyleBackColor = true;
            this.buttonDeleteTeam.Click += new System.EventHandler(this.ButtonDeleteTeamClick);
            // 
            // buttonAddTeam
            // 
            this.buttonAddTeam.Location = new System.Drawing.Point(6, 19);
            this.buttonAddTeam.Name = "buttonAddTeam";
            this.buttonAddTeam.Size = new System.Drawing.Size(75, 23);
            this.buttonAddTeam.TabIndex = 0;
            this.buttonAddTeam.Text = "Add";
            this.buttonAddTeam.UseVisualStyleBackColor = true;
            this.buttonAddTeam.Click += new System.EventHandler(this.ButtonAddTeamClick);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.buttonReIdPlayer);
            this.groupBox3.Controls.Add(this.buttonDeletePlayer);
            this.groupBox3.Controls.Add(this.buttonAddPlayer);
            this.groupBox3.Location = new System.Drawing.Point(516, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(87, 106);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Player";
            // 
            // buttonReIdPlayer
            // 
            this.buttonReIdPlayer.Location = new System.Drawing.Point(6, 77);
            this.buttonReIdPlayer.Name = "buttonReIdPlayer";
            this.buttonReIdPlayer.Size = new System.Drawing.Size(75, 23);
            this.buttonReIdPlayer.TabIndex = 2;
            this.buttonReIdPlayer.Text = "Re-ID";
            this.buttonReIdPlayer.UseVisualStyleBackColor = true;
            this.buttonReIdPlayer.Click += new System.EventHandler(this.ButtonReIdPlayerClick);
            // 
            // buttonDeletePlayer
            // 
            this.buttonDeletePlayer.Location = new System.Drawing.Point(6, 48);
            this.buttonDeletePlayer.Name = "buttonDeletePlayer";
            this.buttonDeletePlayer.Size = new System.Drawing.Size(75, 23);
            this.buttonDeletePlayer.TabIndex = 1;
            this.buttonDeletePlayer.Text = "Delete";
            this.buttonDeletePlayer.UseVisualStyleBackColor = true;
            this.buttonDeletePlayer.Click += new System.EventHandler(this.ButtonDeletePlayerClick);
            // 
            // buttonAddPlayer
            // 
            this.buttonAddPlayer.Location = new System.Drawing.Point(6, 19);
            this.buttonAddPlayer.Name = "buttonAddPlayer";
            this.buttonAddPlayer.Size = new System.Drawing.Size(75, 23);
            this.buttonAddPlayer.TabIndex = 0;
            this.buttonAddPlayer.Text = "Add...";
            this.buttonAddPlayer.UseVisualStyleBackColor = true;
            this.buttonAddPlayer.Click += new System.EventHandler(this.ButtonAddPlayerClick);
            // 
            // panelRight
            // 
            this.panelRight.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelRight.Controls.Add(this.tabControl1);
            this.panelRight.Location = new System.Drawing.Point(330, 124);
            this.panelRight.Name = "panelRight";
            this.panelRight.Size = new System.Drawing.Size(273, 354);
            this.panelRight.TabIndex = 4;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.leaguePage);
            this.tabControl1.Controls.Add(this.scoresPage);
            this.tabControl1.Controls.Add(this.GradesPage);
            this.tabControl1.Controls.Add(this.HandicapPage);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(273, 354);
            this.tabControl1.TabIndex = 1;
            // 
            // leaguePage
            // 
            this.leaguePage.Controls.Add(this.groupBoxHandicapStyle);
            this.leaguePage.Controls.Add(this.victoryPoints);
            this.leaguePage.Controls.Add(this.totalScore);
            this.leaguePage.Controls.Add(this.label1);
            this.leaguePage.Location = new System.Drawing.Point(4, 22);
            this.leaguePage.Name = "leaguePage";
            this.leaguePage.Padding = new System.Windows.Forms.Padding(3);
            this.leaguePage.Size = new System.Drawing.Size(265, 328);
            this.leaguePage.TabIndex = 0;
            this.leaguePage.Text = "League";
            this.leaguePage.UseVisualStyleBackColor = true;
            // 
            // groupBoxHandicapStyle
            // 
            this.groupBoxHandicapStyle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxHandicapStyle.Controls.Add(this.radioButtonMinus);
            this.groupBoxHandicapStyle.Controls.Add(this.radioButtonPlus);
            this.groupBoxHandicapStyle.Controls.Add(this.radioButtonNone);
            this.groupBoxHandicapStyle.Controls.Add(this.radioButtonPercent);
            this.groupBoxHandicapStyle.Location = new System.Drawing.Point(6, 272);
            this.groupBoxHandicapStyle.Name = "groupBoxHandicapStyle";
            this.groupBoxHandicapStyle.Size = new System.Drawing.Size(253, 50);
            this.groupBoxHandicapStyle.TabIndex = 3;
            this.groupBoxHandicapStyle.TabStop = false;
            this.groupBoxHandicapStyle.Text = "Handicap Style";
            // 
            // radioButtonMinus
            // 
            this.radioButtonMinus.Location = new System.Drawing.Point(98, 19);
            this.radioButtonMinus.Name = "radioButtonMinus";
            this.radioButtonMinus.Size = new System.Drawing.Size(40, 24);
            this.radioButtonMinus.TabIndex = 3;
            this.radioButtonMinus.TabStop = true;
            this.radioButtonMinus.Text = "-";
            this.radioButtonMinus.UseVisualStyleBackColor = true;
            this.radioButtonMinus.CheckedChanged += new System.EventHandler(this.RadioButtonHandicapCheckedChanged);
            // 
            // radioButtonPlus
            // 
            this.radioButtonPlus.Location = new System.Drawing.Point(52, 19);
            this.radioButtonPlus.Name = "radioButtonPlus";
            this.radioButtonPlus.Size = new System.Drawing.Size(40, 24);
            this.radioButtonPlus.TabIndex = 2;
            this.radioButtonPlus.TabStop = true;
            this.radioButtonPlus.Text = "+";
            this.radioButtonPlus.UseVisualStyleBackColor = true;
            this.radioButtonPlus.CheckedChanged += new System.EventHandler(this.RadioButtonHandicapCheckedChanged);
            // 
            // radioButtonNone
            // 
            this.radioButtonNone.Location = new System.Drawing.Point(144, 19);
            this.radioButtonNone.Name = "radioButtonNone";
            this.radioButtonNone.Size = new System.Drawing.Size(60, 24);
            this.radioButtonNone.TabIndex = 1;
            this.radioButtonNone.TabStop = true;
            this.radioButtonNone.Text = "None";
            this.radioButtonNone.UseVisualStyleBackColor = true;
            this.radioButtonNone.CheckedChanged += new System.EventHandler(this.RadioButtonHandicapCheckedChanged);
            // 
            // radioButtonPercent
            // 
            this.radioButtonPercent.Location = new System.Drawing.Point(6, 19);
            this.radioButtonPercent.Name = "radioButtonPercent";
            this.radioButtonPercent.Size = new System.Drawing.Size(40, 24);
            this.radioButtonPercent.TabIndex = 0;
            this.radioButtonPercent.TabStop = true;
            this.radioButtonPercent.Text = "%";
            this.radioButtonPercent.UseVisualStyleBackColor = true;
            this.radioButtonPercent.CheckedChanged += new System.EventHandler(this.RadioButtonHandicapCheckedChanged);
            // 
            // victoryPoints
            // 
            this.victoryPoints.Location = new System.Drawing.Point(16, 44);
            this.victoryPoints.Name = "victoryPoints";
            this.victoryPoints.Size = new System.Drawing.Size(104, 24);
            this.victoryPoints.TabIndex = 2;
            this.victoryPoints.Text = "victory points";
            this.victoryPoints.UseVisualStyleBackColor = true;
            this.victoryPoints.CheckedChanged += new System.EventHandler(this.RankCheckedChanged);
            // 
            // totalScore
            // 
            this.totalScore.Checked = true;
            this.totalScore.Location = new System.Drawing.Point(16, 24);
            this.totalScore.Name = "totalScore";
            this.totalScore.Size = new System.Drawing.Size(104, 24);
            this.totalScore.TabIndex = 1;
            this.totalScore.TabStop = true;
            this.totalScore.Text = "total score";
            this.totalScore.UseVisualStyleBackColor = true;
            this.totalScore.CheckedChanged += new System.EventHandler(this.RankCheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(114, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Rank teams based on:";
            // 
            // scoresPage
            // 
            this.scoresPage.Controls.Add(this.listViewScores);
            this.scoresPage.Location = new System.Drawing.Point(4, 22);
            this.scoresPage.Name = "scoresPage";
            this.scoresPage.Padding = new System.Windows.Forms.Padding(3);
            this.scoresPage.Size = new System.Drawing.Size(265, 328);
            this.scoresPage.TabIndex = 1;
            this.scoresPage.Text = "Scores";
            this.scoresPage.UseVisualStyleBackColor = true;
            // 
            // listViewScores
            // 
            this.listViewScores.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colGame,
            this.colScore,
            this.colRankorPoints});
            this.listViewScores.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewScores.FullRowSelect = true;
            this.listViewScores.HideSelection = false;
            this.listViewScores.Location = new System.Drawing.Point(3, 3);
            this.listViewScores.Name = "listViewScores";
            this.listViewScores.Size = new System.Drawing.Size(259, 322);
            this.listViewScores.TabIndex = 0;
            this.listViewScores.UseCompatibleStateImageBehavior = false;
            this.listViewScores.View = System.Windows.Forms.View.Details;
            // 
            // colGame
            // 
            this.colGame.Text = "Game";
            this.colGame.Width = 130;
            // 
            // colScore
            // 
            this.colScore.Text = "Score";
            this.colScore.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // colRankorPoints
            // 
            this.colRankorPoints.Text = "Points";
            this.colRankorPoints.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.colRankorPoints.Width = 44;
            // 
            // GradesPage
            // 
            this.GradesPage.Controls.Add(this.IBonus);
            this.GradesPage.Controls.Add(this.HBonus);
            this.GradesPage.Controls.Add(this.GBonus);
            this.GradesPage.Controls.Add(this.FBonus);
            this.GradesPage.Controls.Add(this.EBonus);
            this.GradesPage.Controls.Add(this.DBonus);
            this.GradesPage.Controls.Add(this.CBonus);
            this.GradesPage.Controls.Add(this.BBonus);
            this.GradesPage.Controls.Add(this.BBBonus);
            this.GradesPage.Controls.Add(this.ABonus);
            this.GradesPage.Controls.Add(this.AAABonus);
            this.GradesPage.Controls.Add(this.IPenalty);
            this.GradesPage.Controls.Add(this.HPenalty);
            this.GradesPage.Controls.Add(this.GPenalty);
            this.GradesPage.Controls.Add(this.FPenalty);
            this.GradesPage.Controls.Add(this.EPenalty);
            this.GradesPage.Controls.Add(this.DPenalty);
            this.GradesPage.Controls.Add(this.CPenalty);
            this.GradesPage.Controls.Add(this.BPenalty);
            this.GradesPage.Controls.Add(this.BBPenalty);
            this.GradesPage.Controls.Add(this.APenalty);
            this.GradesPage.Controls.Add(this.AAAPenalty);
            this.GradesPage.Controls.Add(this.label5);
            this.GradesPage.Controls.Add(this.label4);
            this.GradesPage.Controls.Add(this.IPoints);
            this.GradesPage.Controls.Add(this.IName);
            this.GradesPage.Controls.Add(this.HPoints);
            this.GradesPage.Controls.Add(this.HName);
            this.GradesPage.Controls.Add(this.GPoints);
            this.GradesPage.Controls.Add(this.GName);
            this.GradesPage.Controls.Add(this.FPoints);
            this.GradesPage.Controls.Add(this.FName);
            this.GradesPage.Controls.Add(this.EPoints);
            this.GradesPage.Controls.Add(this.EName);
            this.GradesPage.Controls.Add(this.DPoints);
            this.GradesPage.Controls.Add(this.DName);
            this.GradesPage.Controls.Add(this.CPoints);
            this.GradesPage.Controls.Add(this.CName);
            this.GradesPage.Controls.Add(this.BPoints);
            this.GradesPage.Controls.Add(this.BName);
            this.GradesPage.Controls.Add(this.BBPoints);
            this.GradesPage.Controls.Add(this.BBName);
            this.GradesPage.Controls.Add(this.APoints);
            this.GradesPage.Controls.Add(this.AName);
            this.GradesPage.Controls.Add(this.AAAPoints);
            this.GradesPage.Controls.Add(this.AAAName);
            this.GradesPage.Controls.Add(this.label3);
            this.GradesPage.Controls.Add(this.label2);
            this.GradesPage.Location = new System.Drawing.Point(4, 22);
            this.GradesPage.Name = "GradesPage";
            this.GradesPage.Padding = new System.Windows.Forms.Padding(3);
            this.GradesPage.Size = new System.Drawing.Size(265, 328);
            this.GradesPage.TabIndex = 2;
            this.GradesPage.Text = "Grades";
            this.GradesPage.UseVisualStyleBackColor = true;
            // 
            // IBonus
            // 
            this.IBonus.AutoSize = true;
            this.IBonus.Checked = true;
            this.IBonus.CheckState = System.Windows.Forms.CheckState.Checked;
            this.IBonus.Enabled = false;
            this.IBonus.Location = new System.Drawing.Point(182, 298);
            this.IBonus.Name = "IBonus";
            this.IBonus.Size = new System.Drawing.Size(15, 14);
            this.IBonus.TabIndex = 47;
            this.IBonus.UseVisualStyleBackColor = true;
            // 
            // HBonus
            // 
            this.HBonus.AutoSize = true;
            this.HBonus.Checked = true;
            this.HBonus.CheckState = System.Windows.Forms.CheckState.Checked;
            this.HBonus.Enabled = false;
            this.HBonus.Location = new System.Drawing.Point(182, 272);
            this.HBonus.Name = "HBonus";
            this.HBonus.Size = new System.Drawing.Size(15, 14);
            this.HBonus.TabIndex = 46;
            this.HBonus.UseVisualStyleBackColor = true;
            // 
            // GBonus
            // 
            this.GBonus.AutoSize = true;
            this.GBonus.Checked = true;
            this.GBonus.CheckState = System.Windows.Forms.CheckState.Checked;
            this.GBonus.Enabled = false;
            this.GBonus.Location = new System.Drawing.Point(182, 246);
            this.GBonus.Name = "GBonus";
            this.GBonus.Size = new System.Drawing.Size(15, 14);
            this.GBonus.TabIndex = 45;
            this.GBonus.UseVisualStyleBackColor = true;
            // 
            // FBonus
            // 
            this.FBonus.AutoSize = true;
            this.FBonus.Enabled = false;
            this.FBonus.Location = new System.Drawing.Point(182, 220);
            this.FBonus.Name = "FBonus";
            this.FBonus.Size = new System.Drawing.Size(15, 14);
            this.FBonus.TabIndex = 44;
            this.FBonus.UseVisualStyleBackColor = true;
            // 
            // EBonus
            // 
            this.EBonus.AutoSize = true;
            this.EBonus.Enabled = false;
            this.EBonus.Location = new System.Drawing.Point(182, 194);
            this.EBonus.Name = "EBonus";
            this.EBonus.Size = new System.Drawing.Size(15, 14);
            this.EBonus.TabIndex = 43;
            this.EBonus.UseVisualStyleBackColor = true;
            // 
            // DBonus
            // 
            this.DBonus.AutoSize = true;
            this.DBonus.Enabled = false;
            this.DBonus.Location = new System.Drawing.Point(182, 168);
            this.DBonus.Name = "DBonus";
            this.DBonus.Size = new System.Drawing.Size(15, 14);
            this.DBonus.TabIndex = 42;
            this.DBonus.UseVisualStyleBackColor = true;
            // 
            // CBonus
            // 
            this.CBonus.AutoSize = true;
            this.CBonus.Enabled = false;
            this.CBonus.Location = new System.Drawing.Point(182, 142);
            this.CBonus.Name = "CBonus";
            this.CBonus.Size = new System.Drawing.Size(15, 14);
            this.CBonus.TabIndex = 41;
            this.CBonus.UseVisualStyleBackColor = true;
            // 
            // BBonus
            // 
            this.BBonus.AutoSize = true;
            this.BBonus.Enabled = false;
            this.BBonus.Location = new System.Drawing.Point(182, 116);
            this.BBonus.Name = "BBonus";
            this.BBonus.Size = new System.Drawing.Size(15, 14);
            this.BBonus.TabIndex = 40;
            this.BBonus.UseVisualStyleBackColor = true;
            // 
            // BBBonus
            // 
            this.BBBonus.AutoSize = true;
            this.BBBonus.Enabled = false;
            this.BBBonus.Location = new System.Drawing.Point(182, 90);
            this.BBBonus.Name = "BBBonus";
            this.BBBonus.Size = new System.Drawing.Size(15, 14);
            this.BBBonus.TabIndex = 39;
            this.BBBonus.UseVisualStyleBackColor = true;
            // 
            // ABonus
            // 
            this.ABonus.AutoSize = true;
            this.ABonus.Enabled = false;
            this.ABonus.Location = new System.Drawing.Point(182, 64);
            this.ABonus.Name = "ABonus";
            this.ABonus.Size = new System.Drawing.Size(15, 14);
            this.ABonus.TabIndex = 38;
            this.ABonus.UseVisualStyleBackColor = true;
            // 
            // AAABonus
            // 
            this.AAABonus.AutoSize = true;
            this.AAABonus.Enabled = false;
            this.AAABonus.Location = new System.Drawing.Point(182, 38);
            this.AAABonus.Name = "AAABonus";
            this.AAABonus.Size = new System.Drawing.Size(15, 14);
            this.AAABonus.TabIndex = 37;
            this.AAABonus.UseVisualStyleBackColor = true;
            // 
            // IPenalty
            // 
            this.IPenalty.AutoSize = true;
            this.IPenalty.Enabled = false;
            this.IPenalty.Location = new System.Drawing.Point(121, 298);
            this.IPenalty.Name = "IPenalty";
            this.IPenalty.Size = new System.Drawing.Size(15, 14);
            this.IPenalty.TabIndex = 36;
            this.IPenalty.UseVisualStyleBackColor = true;
            // 
            // HPenalty
            // 
            this.HPenalty.AutoSize = true;
            this.HPenalty.Enabled = false;
            this.HPenalty.Location = new System.Drawing.Point(121, 272);
            this.HPenalty.Name = "HPenalty";
            this.HPenalty.Size = new System.Drawing.Size(15, 14);
            this.HPenalty.TabIndex = 35;
            this.HPenalty.UseVisualStyleBackColor = true;
            // 
            // GPenalty
            // 
            this.GPenalty.AutoSize = true;
            this.GPenalty.Enabled = false;
            this.GPenalty.Location = new System.Drawing.Point(121, 246);
            this.GPenalty.Name = "GPenalty";
            this.GPenalty.Size = new System.Drawing.Size(15, 14);
            this.GPenalty.TabIndex = 34;
            this.GPenalty.UseVisualStyleBackColor = true;
            // 
            // FPenalty
            // 
            this.FPenalty.AutoSize = true;
            this.FPenalty.Enabled = false;
            this.FPenalty.Location = new System.Drawing.Point(121, 220);
            this.FPenalty.Name = "FPenalty";
            this.FPenalty.Size = new System.Drawing.Size(15, 14);
            this.FPenalty.TabIndex = 33;
            this.FPenalty.UseVisualStyleBackColor = true;
            // 
            // EPenalty
            // 
            this.EPenalty.AutoSize = true;
            this.EPenalty.Enabled = false;
            this.EPenalty.Location = new System.Drawing.Point(121, 194);
            this.EPenalty.Name = "EPenalty";
            this.EPenalty.Size = new System.Drawing.Size(15, 14);
            this.EPenalty.TabIndex = 32;
            this.EPenalty.UseVisualStyleBackColor = true;
            // 
            // DPenalty
            // 
            this.DPenalty.AutoSize = true;
            this.DPenalty.Enabled = false;
            this.DPenalty.Location = new System.Drawing.Point(121, 168);
            this.DPenalty.Name = "DPenalty";
            this.DPenalty.Size = new System.Drawing.Size(15, 14);
            this.DPenalty.TabIndex = 31;
            this.DPenalty.UseVisualStyleBackColor = true;
            // 
            // CPenalty
            // 
            this.CPenalty.AutoSize = true;
            this.CPenalty.Enabled = false;
            this.CPenalty.Location = new System.Drawing.Point(121, 142);
            this.CPenalty.Name = "CPenalty";
            this.CPenalty.Size = new System.Drawing.Size(15, 14);
            this.CPenalty.TabIndex = 30;
            this.CPenalty.UseVisualStyleBackColor = true;
            // 
            // BPenalty
            // 
            this.BPenalty.AutoSize = true;
            this.BPenalty.Enabled = false;
            this.BPenalty.Location = new System.Drawing.Point(121, 116);
            this.BPenalty.Name = "BPenalty";
            this.BPenalty.Size = new System.Drawing.Size(15, 14);
            this.BPenalty.TabIndex = 29;
            this.BPenalty.UseVisualStyleBackColor = true;
            // 
            // BBPenalty
            // 
            this.BBPenalty.AutoSize = true;
            this.BBPenalty.Enabled = false;
            this.BBPenalty.Location = new System.Drawing.Point(121, 90);
            this.BBPenalty.Name = "BBPenalty";
            this.BBPenalty.Size = new System.Drawing.Size(15, 14);
            this.BBPenalty.TabIndex = 28;
            this.BBPenalty.UseVisualStyleBackColor = true;
            // 
            // APenalty
            // 
            this.APenalty.AutoSize = true;
            this.APenalty.Checked = true;
            this.APenalty.CheckState = System.Windows.Forms.CheckState.Checked;
            this.APenalty.Enabled = false;
            this.APenalty.Location = new System.Drawing.Point(121, 64);
            this.APenalty.Name = "APenalty";
            this.APenalty.Size = new System.Drawing.Size(15, 14);
            this.APenalty.TabIndex = 27;
            this.APenalty.UseVisualStyleBackColor = true;
            // 
            // AAAPenalty
            // 
            this.AAAPenalty.AutoSize = true;
            this.AAAPenalty.Checked = true;
            this.AAAPenalty.CheckState = System.Windows.Forms.CheckState.Checked;
            this.AAAPenalty.Enabled = false;
            this.AAAPenalty.Location = new System.Drawing.Point(121, 38);
            this.AAAPenalty.Name = "AAAPenalty";
            this.AAAPenalty.Size = new System.Drawing.Size(15, 14);
            this.AAAPenalty.TabIndex = 26;
            this.AAAPenalty.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(179, 13);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(37, 13);
            this.label5.TabIndex = 25;
            this.label5.Text = "Bonus";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(118, 13);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(42, 13);
            this.label4.TabIndex = 24;
            this.label4.Text = "Penalty";
            // 
            // IPoints
            // 
            this.IPoints.Enabled = false;
            this.IPoints.Location = new System.Drawing.Point(69, 299);
            this.IPoints.Name = "IPoints";
            this.IPoints.Size = new System.Drawing.Size(33, 20);
            this.IPoints.TabIndex = 23;
            // 
            // IName
            // 
            this.IName.Location = new System.Drawing.Point(9, 298);
            this.IName.Name = "IName";
            this.IName.Size = new System.Drawing.Size(48, 20);
            this.IName.TabIndex = 22;
            this.IName.Text = "I";
            // 
            // HPoints
            // 
            this.HPoints.Enabled = false;
            this.HPoints.Location = new System.Drawing.Point(69, 273);
            this.HPoints.Name = "HPoints";
            this.HPoints.Size = new System.Drawing.Size(33, 20);
            this.HPoints.TabIndex = 21;
            // 
            // HName
            // 
            this.HName.Location = new System.Drawing.Point(9, 272);
            this.HName.Name = "HName";
            this.HName.Size = new System.Drawing.Size(48, 20);
            this.HName.TabIndex = 20;
            this.HName.Text = "H";
            // 
            // GPoints
            // 
            this.GPoints.Enabled = false;
            this.GPoints.Location = new System.Drawing.Point(69, 247);
            this.GPoints.Name = "GPoints";
            this.GPoints.Size = new System.Drawing.Size(33, 20);
            this.GPoints.TabIndex = 19;
            // 
            // GName
            // 
            this.GName.Location = new System.Drawing.Point(9, 246);
            this.GName.Name = "GName";
            this.GName.Size = new System.Drawing.Size(48, 20);
            this.GName.TabIndex = 18;
            this.GName.Text = "G";
            // 
            // FPoints
            // 
            this.FPoints.Enabled = false;
            this.FPoints.Location = new System.Drawing.Point(69, 221);
            this.FPoints.Name = "FPoints";
            this.FPoints.Size = new System.Drawing.Size(33, 20);
            this.FPoints.TabIndex = 17;
            // 
            // FName
            // 
            this.FName.Location = new System.Drawing.Point(9, 220);
            this.FName.Name = "FName";
            this.FName.Size = new System.Drawing.Size(48, 20);
            this.FName.TabIndex = 16;
            this.FName.Text = "F";
            // 
            // EPoints
            // 
            this.EPoints.Enabled = false;
            this.EPoints.Location = new System.Drawing.Point(69, 195);
            this.EPoints.Name = "EPoints";
            this.EPoints.Size = new System.Drawing.Size(33, 20);
            this.EPoints.TabIndex = 15;
            this.EPoints.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // EName
            // 
            this.EName.Location = new System.Drawing.Point(9, 194);
            this.EName.Name = "EName";
            this.EName.Size = new System.Drawing.Size(48, 20);
            this.EName.TabIndex = 14;
            this.EName.Text = "E";
            // 
            // DPoints
            // 
            this.DPoints.Enabled = false;
            this.DPoints.Location = new System.Drawing.Point(69, 169);
            this.DPoints.Name = "DPoints";
            this.DPoints.Size = new System.Drawing.Size(33, 20);
            this.DPoints.TabIndex = 13;
            this.DPoints.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // DName
            // 
            this.DName.Location = new System.Drawing.Point(9, 168);
            this.DName.Name = "DName";
            this.DName.Size = new System.Drawing.Size(48, 20);
            this.DName.TabIndex = 12;
            this.DName.Text = "D";
            // 
            // CPoints
            // 
            this.CPoints.Enabled = false;
            this.CPoints.Location = new System.Drawing.Point(69, 143);
            this.CPoints.Name = "CPoints";
            this.CPoints.Size = new System.Drawing.Size(33, 20);
            this.CPoints.TabIndex = 11;
            this.CPoints.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // CName
            // 
            this.CName.Location = new System.Drawing.Point(9, 142);
            this.CName.Name = "CName";
            this.CName.Size = new System.Drawing.Size(48, 20);
            this.CName.TabIndex = 10;
            this.CName.Text = "C";
            // 
            // BPoints
            // 
            this.BPoints.Enabled = false;
            this.BPoints.Location = new System.Drawing.Point(69, 117);
            this.BPoints.Name = "BPoints";
            this.BPoints.Size = new System.Drawing.Size(33, 20);
            this.BPoints.TabIndex = 9;
            this.BPoints.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            // 
            // BName
            // 
            this.BName.Location = new System.Drawing.Point(9, 116);
            this.BName.Name = "BName";
            this.BName.Size = new System.Drawing.Size(48, 20);
            this.BName.TabIndex = 8;
            this.BName.Text = "B";
            // 
            // BBPoints
            // 
            this.BBPoints.Enabled = false;
            this.BBPoints.Location = new System.Drawing.Point(69, 91);
            this.BBPoints.Name = "BBPoints";
            this.BBPoints.Size = new System.Drawing.Size(33, 20);
            this.BBPoints.TabIndex = 7;
            this.BBPoints.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // BBName
            // 
            this.BBName.Location = new System.Drawing.Point(9, 90);
            this.BBName.Name = "BBName";
            this.BBName.Size = new System.Drawing.Size(48, 20);
            this.BBName.TabIndex = 6;
            this.BBName.Text = "BB";
            // 
            // APoints
            // 
            this.APoints.Enabled = false;
            this.APoints.Location = new System.Drawing.Point(69, 65);
            this.APoints.Name = "APoints";
            this.APoints.Size = new System.Drawing.Size(33, 20);
            this.APoints.TabIndex = 5;
            this.APoints.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // AName
            // 
            this.AName.Location = new System.Drawing.Point(9, 64);
            this.AName.Name = "AName";
            this.AName.Size = new System.Drawing.Size(48, 20);
            this.AName.TabIndex = 4;
            this.AName.Text = "A";
            // 
            // AAAPoints
            // 
            this.AAAPoints.Enabled = false;
            this.AAAPoints.Location = new System.Drawing.Point(69, 39);
            this.AAAPoints.Name = "AAAPoints";
            this.AAAPoints.Size = new System.Drawing.Size(33, 20);
            this.AAAPoints.TabIndex = 3;
            this.AAAPoints.Value = new decimal(new int[] {
            6,
            0,
            0,
            0});
            // 
            // AAAName
            // 
            this.AAAName.Location = new System.Drawing.Point(9, 38);
            this.AAAName.Name = "AAAName";
            this.AAAName.Size = new System.Drawing.Size(48, 20);
            this.AAAName.TabIndex = 2;
            this.AAAName.Text = "AAA";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(66, 13);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(36, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Points";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Grade";
            // 
            // HandicapPage
            // 
            this.HandicapPage.Controls.Add(this.extraGBonus);
            this.HandicapPage.Controls.Add(this.extraGBonusLabel);
            this.HandicapPage.Controls.Add(this.extraAPenalty);
            this.HandicapPage.Controls.Add(this.label8);
            this.HandicapPage.Controls.Add(this.missingPlayerPenalty);
            this.HandicapPage.Controls.Add(this.label7);
            this.HandicapPage.Controls.Add(this.label6);
            this.HandicapPage.Controls.Add(this.teamSize);
            this.HandicapPage.Controls.Add(this.automaticHandicapEnabled);
            this.HandicapPage.Location = new System.Drawing.Point(4, 22);
            this.HandicapPage.Name = "HandicapPage";
            this.HandicapPage.Padding = new System.Windows.Forms.Padding(3);
            this.HandicapPage.Size = new System.Drawing.Size(265, 328);
            this.HandicapPage.TabIndex = 3;
            this.HandicapPage.Text = "Handicap";
            this.HandicapPage.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(39, 47);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(57, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "Team Size";
            // 
            // teamSize
            // 
            this.teamSize.Enabled = false;
            this.teamSize.Location = new System.Drawing.Point(169, 40);
            this.teamSize.Name = "teamSize";
            this.teamSize.Size = new System.Drawing.Size(37, 20);
            this.teamSize.TabIndex = 1;
            this.teamSize.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // automaticHandicapEnabled
            // 
            this.automaticHandicapEnabled.AutoSize = true;
            this.automaticHandicapEnabled.Location = new System.Drawing.Point(42, 17);
            this.automaticHandicapEnabled.Name = "automaticHandicapEnabled";
            this.automaticHandicapEnabled.Size = new System.Drawing.Size(164, 17);
            this.automaticHandicapEnabled.TabIndex = 0;
            this.automaticHandicapEnabled.Text = "Automatic Handicap Enabled";
            this.automaticHandicapEnabled.UseVisualStyleBackColor = true;
            this.automaticHandicapEnabled.CheckedChanged += new System.EventHandler(this.automaticHandicapEnabled_CheckedChanged);
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(444, 484);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 5;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(525, 484);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 6;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(39, 73);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(112, 13);
            this.label7.TabIndex = 3;
            this.label7.Text = "Missing Player Penalty";
            // 
            // missingPlayerPenalty
            // 
            this.missingPlayerPenalty.Enabled = false;
            this.missingPlayerPenalty.Location = new System.Drawing.Point(169, 66);
            this.missingPlayerPenalty.Name = "missingPlayerPenalty";
            this.missingPlayerPenalty.Size = new System.Drawing.Size(37, 20);
            this.missingPlayerPenalty.TabIndex = 4;
            this.missingPlayerPenalty.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(39, 99);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(79, 13);
            this.label8.TabIndex = 5;
            this.label8.Text = "Extra A Penalty";
            // 
            // extraAPenalty
            // 
            this.extraAPenalty.Enabled = false;
            this.extraAPenalty.Location = new System.Drawing.Point(169, 92);
            this.extraAPenalty.Name = "extraAPenalty";
            this.extraAPenalty.Size = new System.Drawing.Size(37, 20);
            this.extraAPenalty.TabIndex = 6;
            this.extraAPenalty.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // extraGBonusLabel
            // 
            this.extraGBonusLabel.AutoSize = true;
            this.extraGBonusLabel.Location = new System.Drawing.Point(39, 125);
            this.extraGBonusLabel.Name = "extraGBonusLabel";
            this.extraGBonusLabel.Size = new System.Drawing.Size(75, 13);
            this.extraGBonusLabel.TabIndex = 7;
            this.extraGBonusLabel.Text = "Extra G Bonus";
            // 
            // extraGBonus
            // 
            this.extraGBonus.Enabled = false;
            this.extraGBonus.Location = new System.Drawing.Point(169, 118);
            this.extraGBonus.Maximum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.extraGBonus.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.extraGBonus.Name = "extraGBonus";
            this.extraGBonus.Size = new System.Drawing.Size(37, 20);
            this.extraGBonus.TabIndex = 8;
            this.extraGBonus.Value = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            // 
            // FormLeague
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(615, 519);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.panelRight);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.treeView1);
            this.MinimumSize = new System.Drawing.Size(400, 400);
            this.Name = "FormLeague";
            this.Text = "League";
            this.Shown += new System.EventHandler(this.FormLeagueShown);
            this.contextMenuStrip1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.panelRight.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.leaguePage.ResumeLayout(false);
            this.leaguePage.PerformLayout();
            this.groupBoxHandicapStyle.ResumeLayout(false);
            this.scoresPage.ResumeLayout(false);
            this.GradesPage.ResumeLayout(false);
            this.GradesPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.IPoints)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.HPoints)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.GPoints)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.FPoints)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.EPoints)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DPoints)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CPoints)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BPoints)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BBPoints)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.APoints)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.AAAPoints)).EndInit();
            this.HandicapPage.ResumeLayout(false);
            this.HandicapPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.teamSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.missingPlayerPenalty)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.extraAPenalty)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.extraGBonus)).EndInit();
            this.ResumeLayout(false);

		}
		private System.Windows.Forms.RadioButton radioButtonNone;
		private System.Windows.Forms.RadioButton radioButtonPlus;
		private System.Windows.Forms.RadioButton radioButtonMinus;
		private System.Windows.Forms.RadioButton radioButtonPercent;
		private System.Windows.Forms.GroupBox groupBoxHandicapStyle;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.RadioButton totalScore;
		private System.Windows.Forms.RadioButton victoryPoints;
		private System.Windows.Forms.TabPage scoresPage;
		private System.Windows.Forms.TabPage leaguePage;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.ColumnHeader colRankorPoints;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.ColumnHeader colScore;
		private System.Windows.Forms.ColumnHeader colGame;
		private System.Windows.Forms.ListView listViewScores;
		private System.Windows.Forms.Panel panelRight;
		private System.Windows.Forms.Button buttonAddPlayer;
		private System.Windows.Forms.Button buttonDeletePlayer;
		private System.Windows.Forms.Button buttonReIdPlayer;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Button buttonAddTeam;
		private System.Windows.Forms.Button buttonDeleteTeam;
		private System.Windows.Forms.Button buttonRenameTeam;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Button buttonRenameLeague;
		private System.Windows.Forms.Button buttonCopyFromLeague;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TreeView treeView1;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripMenuItem deleteTeamMenuItem;
		private System.Windows.Forms.ToolStripMenuItem renameTeamMenuItem;
		private System.Windows.Forms.ToolStripMenuItem deletePlayerMenuItem;
		private System.Windows.Forms.ToolStripMenuItem reIDPlayerMenuItem;
        private System.Windows.Forms.TabPage GradesPage;
        private System.Windows.Forms.NumericUpDown IPoints;
        private System.Windows.Forms.TextBox IName;
        private System.Windows.Forms.NumericUpDown HPoints;
        private System.Windows.Forms.TextBox HName;
        private System.Windows.Forms.NumericUpDown GPoints;
        private System.Windows.Forms.TextBox GName;
        private System.Windows.Forms.NumericUpDown FPoints;
        private System.Windows.Forms.TextBox FName;
        private System.Windows.Forms.NumericUpDown EPoints;
        private System.Windows.Forms.TextBox EName;
        private System.Windows.Forms.NumericUpDown DPoints;
        private System.Windows.Forms.TextBox DName;
        private System.Windows.Forms.NumericUpDown CPoints;
        private System.Windows.Forms.TextBox CName;
        private System.Windows.Forms.NumericUpDown BPoints;
        private System.Windows.Forms.TextBox BName;
        private System.Windows.Forms.NumericUpDown BBPoints;
        private System.Windows.Forms.TextBox BBName;
        private System.Windows.Forms.NumericUpDown APoints;
        private System.Windows.Forms.TextBox AName;
        private System.Windows.Forms.NumericUpDown AAAPoints;
        private System.Windows.Forms.TextBox AAAName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox IBonus;
        private System.Windows.Forms.CheckBox HBonus;
        private System.Windows.Forms.CheckBox GBonus;
        private System.Windows.Forms.CheckBox FBonus;
        private System.Windows.Forms.CheckBox EBonus;
        private System.Windows.Forms.CheckBox DBonus;
        private System.Windows.Forms.CheckBox CBonus;
        private System.Windows.Forms.CheckBox BBonus;
        private System.Windows.Forms.CheckBox BBBonus;
        private System.Windows.Forms.CheckBox ABonus;
        private System.Windows.Forms.CheckBox AAABonus;
        private System.Windows.Forms.CheckBox IPenalty;
        private System.Windows.Forms.CheckBox HPenalty;
        private System.Windows.Forms.CheckBox GPenalty;
        private System.Windows.Forms.CheckBox FPenalty;
        private System.Windows.Forms.CheckBox EPenalty;
        private System.Windows.Forms.CheckBox DPenalty;
        private System.Windows.Forms.CheckBox CPenalty;
        private System.Windows.Forms.CheckBox BPenalty;
        private System.Windows.Forms.CheckBox BBPenalty;
        private System.Windows.Forms.CheckBox APenalty;
        private System.Windows.Forms.CheckBox AAAPenalty;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TabPage HandicapPage;
        private System.Windows.Forms.CheckBox automaticHandicapEnabled;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown teamSize;
        private System.Windows.Forms.NumericUpDown extraGBonus;
        private System.Windows.Forms.Label extraGBonusLabel;
        private System.Windows.Forms.NumericUpDown extraAPenalty;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown missingPlayerPenalty;
        private System.Windows.Forms.Label label7;
    }
}
