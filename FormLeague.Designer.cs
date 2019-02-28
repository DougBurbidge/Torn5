
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
			this.treeView1 = new System.Windows.Forms.TreeView();
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
			this.victoryPoints = new System.Windows.Forms.RadioButton();
			this.totalScore = new System.Windows.Forms.RadioButton();
			this.label1 = new System.Windows.Forms.Label();
			this.scoresPage = new System.Windows.Forms.TabPage();
			this.listViewScores = new System.Windows.Forms.ListView();
			this.colGame = new System.Windows.Forms.ColumnHeader();
			this.colScore = new System.Windows.Forms.ColumnHeader();
			this.colRankorPoints = new System.Windows.Forms.ColumnHeader();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.panelRight.SuspendLayout();
			this.tabControl1.SuspendLayout();
			this.leaguePage.SuspendLayout();
			this.scoresPage.SuspendLayout();
			this.SuspendLayout();
			// 
			// treeView1
			// 
			this.treeView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.treeView1.HideSelection = false;
			this.treeView1.Location = new System.Drawing.Point(12, 12);
			this.treeView1.Name = "treeView1";
			this.treeView1.Size = new System.Drawing.Size(312, 463);
			this.treeView1.TabIndex = 0;
			this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TreeView1AfterSelect);
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
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.Location = new System.Drawing.Point(0, 0);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(273, 354);
			this.tabControl1.TabIndex = 1;
			// 
			// leaguePage
			// 
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
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.panelRight.ResumeLayout(false);
			this.tabControl1.ResumeLayout(false);
			this.leaguePage.ResumeLayout(false);
			this.leaguePage.PerformLayout();
			this.scoresPage.ResumeLayout(false);
			this.ResumeLayout(false);
		}
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
	}
}
