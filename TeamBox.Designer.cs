
namespace Torn.UI
{
	partial class TeamBox
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the control.
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
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.menuSortTeams = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.menuHandicapTeam = new System.Windows.Forms.ToolStripMenuItem();
			this.menuRememberTeam = new System.Windows.Forms.ToolStripMenuItem();
			this.menuUpdateTeam = new System.Windows.Forms.ToolStripMenuItem();
			this.menuNameTeam = new System.Windows.Forms.ToolStripMenuItem();
			this.menuIdentifyTeam = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.menuIdentifyPlayer = new System.Windows.Forms.ToolStripMenuItem();
			this.menuHandicapPlayer = new System.Windows.Forms.ToolStripMenuItem();
			this.menuMergePlayer = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.menuAdjustTeamScore = new System.Windows.Forms.ToolStripMenuItem();
			this.contextMenuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.menuSortTeams,
									this.toolStripSeparator1,
									this.menuHandicapTeam,
									this.menuRememberTeam,
									this.menuUpdateTeam,
									this.menuNameTeam,
									this.menuIdentifyTeam,
									this.toolStripSeparator2,
									this.menuIdentifyPlayer,
									this.menuHandicapPlayer,
									this.menuMergePlayer,
									this.toolStripSeparator3,
									this.menuAdjustTeamScore});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(210, 264);
			this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.ContextMenuStrip1Opening);
			// 
			// menuSortTeams
			// 
			this.menuSortTeams.Name = "menuSortTeams";
			this.menuSortTeams.Size = new System.Drawing.Size(209, 22);
			this.menuSortTeams.Text = "&Sort teams by rank";
			this.menuSortTeams.Click += new System.EventHandler(this.MenuSortTeamsClick);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(206, 6);
			// 
			// menuHandicapTeam
			// 
			this.menuHandicapTeam.Name = "menuHandicapTeam";
			this.menuHandicapTeam.Size = new System.Drawing.Size(209, 22);
			this.menuHandicapTeam.Text = "&Handicap this team";
			this.menuHandicapTeam.Click += new System.EventHandler(this.MenuHandicapTeamClick);
			// 
			// menuRememberTeam
			// 
			this.menuRememberTeam.Name = "menuRememberTeam";
			this.menuRememberTeam.Size = new System.Drawing.Size(209, 22);
			this.menuRememberTeam.Text = "&Remember this team";
			this.menuRememberTeam.Click += new System.EventHandler(this.MenuRememberTeamClick);
			// 
			// menuUpdateTeam
			// 
			this.menuUpdateTeam.Name = "menuUpdateTeam";
			this.menuUpdateTeam.Size = new System.Drawing.Size(209, 22);
			this.menuUpdateTeam.Text = "&Update this team";
			this.menuUpdateTeam.Click += new System.EventHandler(this.MenuUpdateTeamClick);
			// 
			// menuNameTeam
			// 
			this.menuNameTeam.Name = "menuNameTeam";
			this.menuNameTeam.Size = new System.Drawing.Size(209, 22);
			this.menuNameTeam.Text = "&Name this team...";
			this.menuNameTeam.Click += new System.EventHandler(this.MenuNameTeamClick);
			// 
			// menuIdentifyTeam
			// 
			this.menuIdentifyTeam.Name = "menuIdentifyTeam";
			this.menuIdentifyTeam.Size = new System.Drawing.Size(209, 22);
			this.menuIdentifyTeam.Text = "&Identify this team";
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(206, 6);
			// 
			// menuIdentifyPlayer
			// 
			this.menuIdentifyPlayer.Name = "menuIdentifyPlayer";
			this.menuIdentifyPlayer.Size = new System.Drawing.Size(209, 22);
			this.menuIdentifyPlayer.Text = "I&dentify ";
			this.menuIdentifyPlayer.Click += new System.EventHandler(this.MenuIdentifyPlayerClick);
			// 
			// menuHandicapPlayer
			// 
			this.menuHandicapPlayer.Name = "menuHandicapPlayer";
			this.menuHandicapPlayer.Size = new System.Drawing.Size(209, 22);
			this.menuHandicapPlayer.Text = "Handi&cap ";
			this.menuHandicapPlayer.Click += new System.EventHandler(this.MenuHandicapPlayerClick);
			// 
			// menuMergePlayer
			// 
			this.menuMergePlayer.Name = "menuMergePlayer";
			this.menuMergePlayer.Size = new System.Drawing.Size(209, 22);
			this.menuMergePlayer.Text = "&Merge ";
			this.menuMergePlayer.Click += new System.EventHandler(this.MenuMergePlayerClick);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(206, 6);
			// 
			// menuAdjustTeamScore
			// 
			this.menuAdjustTeamScore.Name = "menuAdjustTeamScore";
			this.menuAdjustTeamScore.Size = new System.Drawing.Size(209, 22);
			this.menuAdjustTeamScore.Text = "&Adjust team score...";
			this.menuAdjustTeamScore.Click += new System.EventHandler(this.MenuAdjustTeamScoreClick);
			// 
			// TeamBox
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Name = "TeamBox";
			this.contextMenuStrip1.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.ToolStripMenuItem menuAdjustTeamScore;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripMenuItem menuMergePlayer;
		private System.Windows.Forms.ToolStripMenuItem menuHandicapPlayer;
		private System.Windows.Forms.ToolStripMenuItem menuIdentifyPlayer;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem menuIdentifyTeam;
		private System.Windows.Forms.ToolStripMenuItem menuNameTeam;
		private System.Windows.Forms.ToolStripMenuItem menuUpdateTeam;
		private System.Windows.Forms.ToolStripMenuItem menuRememberTeam;
		private System.Windows.Forms.ToolStripMenuItem menuHandicapTeam;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem menuSortTeams;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
	}
}
