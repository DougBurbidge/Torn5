
namespace Torn.UI
{
	partial class FormFixture
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
			this.textBoxTeams = new System.Windows.Forms.TextBox();
			this.buttonImportTeams = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.buttonImportGrid = new System.Windows.Forms.Button();
			this.buttonImportGames = new System.Windows.Forms.Button();
			this.textBoxSeparator = new System.Windows.Forms.TextBox();
			this.radioButtonOther = new System.Windows.Forms.RadioButton();
			this.radioButtonTab = new System.Windows.Forms.RadioButton();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.textBoxGames = new System.Windows.Forms.TextBox();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabTeams = new System.Windows.Forms.TabPage();
			this.tabGamesList = new System.Windows.Forms.TabPage();
			this.buttonClearGames = new System.Windows.Forms.Button();
			this.tabGamesGrid = new System.Windows.Forms.TabPage();
			this.buttonClearGrid = new System.Windows.Forms.Button();
			this.numericMinutes = new System.Windows.Forms.NumericUpDown();
			this.timePicker = new System.Windows.Forms.DateTimePicker();
			this.datePicker = new System.Windows.Forms.DateTimePicker();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.textBoxGrid = new System.Windows.Forms.TextBox();
			this.tabGraphic = new System.Windows.Forms.TabPage();
			this.numericSize = new System.Windows.Forms.NumericUpDown();
			this.panelGraphic = new System.Windows.Forms.Panel();
			this.tabFinals = new System.Windows.Forms.TabPage();
			this.labelTeamsToSendUp = new System.Windows.Forms.Label();
			this.numericFreeRides = new System.Windows.Forms.NumericUpDown();
			this.label10 = new System.Windows.Forms.Label();
			this.buttonFormatD = new System.Windows.Forms.Button();
			this.buttonTwoTrack = new System.Windows.Forms.Button();
			this.numericTeamsPerGame = new System.Windows.Forms.NumericUpDown();
			this.label9 = new System.Windows.Forms.Label();
			this.numericTeamsToCut = new System.Windows.Forms.NumericUpDown();
			this.label8 = new System.Windows.Forms.Label();
			this.numericTracks = new System.Windows.Forms.NumericUpDown();
			this.label7 = new System.Windows.Forms.Label();
			this.panelFinals = new System.Windows.Forms.Panel();
			this.buttonAscension = new System.Windows.Forms.Button();
			this.timerRedraw = new System.Windows.Forms.Timer(this.components);
			this.tabControl1.SuspendLayout();
			this.tabTeams.SuspendLayout();
			this.tabGamesList.SuspendLayout();
			this.tabGamesGrid.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericMinutes)).BeginInit();
			this.tabGraphic.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericSize)).BeginInit();
			this.tabFinals.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericFreeRides)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericTeamsPerGame)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericTeamsToCut)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericTracks)).BeginInit();
			this.SuspendLayout();
			// 
			// textBoxTeams
			// 
			this.textBoxTeams.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxTeams.Location = new System.Drawing.Point(6, 26);
			this.textBoxTeams.Multiline = true;
			this.textBoxTeams.Name = "textBoxTeams";
			this.textBoxTeams.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBoxTeams.Size = new System.Drawing.Size(588, 571);
			this.textBoxTeams.TabIndex = 11;
			this.textBoxTeams.Text = "Team 1\r\nTeam 2\r\nTeam 3\r\netc.";
			this.textBoxTeams.WordWrap = false;
			this.textBoxTeams.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBoxKeyDown);
			// 
			// buttonImportTeams
			// 
			this.buttonImportTeams.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonImportTeams.Location = new System.Drawing.Point(519, 604);
			this.buttonImportTeams.Name = "buttonImportTeams";
			this.buttonImportTeams.Size = new System.Drawing.Size(75, 23);
			this.buttonImportTeams.TabIndex = 10;
			this.buttonImportTeams.Text = "Import";
			this.buttonImportTeams.UseVisualStyleBackColor = true;
			this.buttonImportTeams.Click += new System.EventHandler(this.ButtonImportTeamsClick);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(6, 6);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(256, 17);
			this.label1.TabIndex = 9;
			this.label1.Text = "Enter a list of teams, one team name per line.";
			// 
			// buttonImportGrid
			// 
			this.buttonImportGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonImportGrid.Location = new System.Drawing.Point(679, 604);
			this.buttonImportGrid.Name = "buttonImportGrid";
			this.buttonImportGrid.Size = new System.Drawing.Size(75, 23);
			this.buttonImportGrid.TabIndex = 25;
			this.buttonImportGrid.Text = "Import Grid";
			this.buttonImportGrid.UseVisualStyleBackColor = true;
			this.buttonImportGrid.Click += new System.EventHandler(this.ButtonImportGridClick);
			// 
			// buttonImportGames
			// 
			this.buttonImportGames.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonImportGames.Location = new System.Drawing.Point(519, 604);
			this.buttonImportGames.Name = "buttonImportGames";
			this.buttonImportGames.Size = new System.Drawing.Size(75, 23);
			this.buttonImportGames.TabIndex = 19;
			this.buttonImportGames.Text = "Import";
			this.buttonImportGames.UseVisualStyleBackColor = true;
			this.buttonImportGames.Click += new System.EventHandler(this.ButtonImportGamesClick);
			// 
			// textBoxSeparator
			// 
			this.textBoxSeparator.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textBoxSeparator.Location = new System.Drawing.Point(65, 603);
			this.textBoxSeparator.Name = "textBoxSeparator";
			this.textBoxSeparator.Size = new System.Drawing.Size(49, 20);
			this.textBoxSeparator.TabIndex = 24;
			this.textBoxSeparator.Text = " ";
			// 
			// radioButtonOther
			// 
			this.radioButtonOther.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.radioButtonOther.Location = new System.Drawing.Point(10, 600);
			this.radioButtonOther.Name = "radioButtonOther";
			this.radioButtonOther.Size = new System.Drawing.Size(104, 24);
			this.radioButtonOther.TabIndex = 23;
			this.radioButtonOther.Text = "Other";
			this.radioButtonOther.UseVisualStyleBackColor = true;
			// 
			// radioButtonTab
			// 
			this.radioButtonTab.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.radioButtonTab.Checked = true;
			this.radioButtonTab.Location = new System.Drawing.Point(10, 579);
			this.radioButtonTab.Name = "radioButtonTab";
			this.radioButtonTab.Size = new System.Drawing.Size(104, 24);
			this.radioButtonTab.TabIndex = 22;
			this.radioButtonTab.TabStop = true;
			this.radioButtonTab.Text = "Tab";
			this.radioButtonTab.UseVisualStyleBackColor = true;
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label3.Location = new System.Drawing.Point(6, 565);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(100, 23);
			this.label3.TabIndex = 21;
			this.label3.Text = "Separator:";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(6, 6);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(251, 27);
			this.label2.TabIndex = 20;
			this.label2.Text = "Enter a list of games, one game per line.\r\n(Date/time,teamnumber,teamnumber, ...)" +
    "";
			// 
			// textBoxGames
			// 
			this.textBoxGames.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxGames.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textBoxGames.Location = new System.Drawing.Point(6, 36);
			this.textBoxGames.Multiline = true;
			this.textBoxGames.Name = "textBoxGames";
			this.textBoxGames.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBoxGames.Size = new System.Drawing.Size(588, 526);
			this.textBoxGames.TabIndex = 18;
			this.textBoxGames.Text = "1/1/2000 7:00pm\t1\t2\t3\r\n1/1/2000 7:15pm\t2\t3\t4\r\n1/1/2000 7:30pm\t3\t4\t1\r\n1/1/2000 7:4" +
    "5pm\t2\t3\t4";
			this.textBoxGames.WordWrap = false;
			this.textBoxGames.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBoxKeyDown);
			// 
			// tabControl1
			// 
			this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl1.Controls.Add(this.tabTeams);
			this.tabControl1.Controls.Add(this.tabGamesList);
			this.tabControl1.Controls.Add(this.tabGamesGrid);
			this.tabControl1.Controls.Add(this.tabGraphic);
			this.tabControl1.Controls.Add(this.tabFinals);
			this.tabControl1.Location = new System.Drawing.Point(12, 12);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(768, 658);
			this.tabControl1.TabIndex = 19;
			// 
			// tabTeams
			// 
			this.tabTeams.Controls.Add(this.textBoxTeams);
			this.tabTeams.Controls.Add(this.label1);
			this.tabTeams.Controls.Add(this.buttonImportTeams);
			this.tabTeams.Location = new System.Drawing.Point(4, 22);
			this.tabTeams.Name = "tabTeams";
			this.tabTeams.Padding = new System.Windows.Forms.Padding(3);
			this.tabTeams.Size = new System.Drawing.Size(760, 632);
			this.tabTeams.TabIndex = 0;
			this.tabTeams.Text = "Teams";
			this.tabTeams.UseVisualStyleBackColor = true;
			// 
			// tabGamesList
			// 
			this.tabGamesList.Controls.Add(this.buttonClearGames);
			this.tabGamesList.Controls.Add(this.radioButtonTab);
			this.tabGamesList.Controls.Add(this.label2);
			this.tabGamesList.Controls.Add(this.buttonImportGames);
			this.tabGamesList.Controls.Add(this.textBoxGames);
			this.tabGamesList.Controls.Add(this.textBoxSeparator);
			this.tabGamesList.Controls.Add(this.label3);
			this.tabGamesList.Controls.Add(this.radioButtonOther);
			this.tabGamesList.Location = new System.Drawing.Point(4, 22);
			this.tabGamesList.Name = "tabGamesList";
			this.tabGamesList.Padding = new System.Windows.Forms.Padding(3);
			this.tabGamesList.Size = new System.Drawing.Size(760, 632);
			this.tabGamesList.TabIndex = 1;
			this.tabGamesList.Text = "Games as a list";
			this.tabGamesList.UseVisualStyleBackColor = true;
			// 
			// buttonClearGames
			// 
			this.buttonClearGames.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonClearGames.Location = new System.Drawing.Point(438, 603);
			this.buttonClearGames.Name = "buttonClearGames";
			this.buttonClearGames.Size = new System.Drawing.Size(75, 23);
			this.buttonClearGames.TabIndex = 25;
			this.buttonClearGames.Text = "Clear Fixture";
			this.buttonClearGames.UseVisualStyleBackColor = true;
			this.buttonClearGames.Click += new System.EventHandler(this.ButtonClearClick);
			// 
			// tabGamesGrid
			// 
			this.tabGamesGrid.Controls.Add(this.buttonClearGrid);
			this.tabGamesGrid.Controls.Add(this.numericMinutes);
			this.tabGamesGrid.Controls.Add(this.timePicker);
			this.tabGamesGrid.Controls.Add(this.datePicker);
			this.tabGamesGrid.Controls.Add(this.label6);
			this.tabGamesGrid.Controls.Add(this.label5);
			this.tabGamesGrid.Controls.Add(this.buttonImportGrid);
			this.tabGamesGrid.Controls.Add(this.label4);
			this.tabGamesGrid.Controls.Add(this.textBoxGrid);
			this.tabGamesGrid.Location = new System.Drawing.Point(4, 22);
			this.tabGamesGrid.Name = "tabGamesGrid";
			this.tabGamesGrid.Padding = new System.Windows.Forms.Padding(3);
			this.tabGamesGrid.Size = new System.Drawing.Size(760, 632);
			this.tabGamesGrid.TabIndex = 2;
			this.tabGamesGrid.Text = "Games as a grid";
			this.tabGamesGrid.UseVisualStyleBackColor = true;
			// 
			// buttonClearGrid
			// 
			this.buttonClearGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonClearGrid.Location = new System.Drawing.Point(601, 604);
			this.buttonClearGrid.Name = "buttonClearGrid";
			this.buttonClearGrid.Size = new System.Drawing.Size(75, 23);
			this.buttonClearGrid.TabIndex = 31;
			this.buttonClearGrid.Text = "Clear Fixture";
			this.buttonClearGrid.UseVisualStyleBackColor = true;
			this.buttonClearGrid.Click += new System.EventHandler(this.ButtonClearClick);
			// 
			// numericMinutes
			// 
			this.numericMinutes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.numericMinutes.Location = new System.Drawing.Point(426, 607);
			this.numericMinutes.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericMinutes.Name = "numericMinutes";
			this.numericMinutes.Size = new System.Drawing.Size(61, 20);
			this.numericMinutes.TabIndex = 30;
			this.numericMinutes.Value = new decimal(new int[] {
            15,
            0,
            0,
            0});
			// 
			// timePicker
			// 
			this.timePicker.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.timePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.timePicker.Location = new System.Drawing.Point(194, 605);
			this.timePicker.Name = "timePicker";
			this.timePicker.ShowUpDown = true;
			this.timePicker.Size = new System.Drawing.Size(98, 20);
			this.timePicker.TabIndex = 29;
			this.timePicker.Value = new System.DateTime(2000, 3, 1, 8, 0, 0, 0);
			// 
			// datePicker
			// 
			this.datePicker.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.datePicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			this.datePicker.Location = new System.Drawing.Point(90, 605);
			this.datePicker.Name = "datePicker";
			this.datePicker.Size = new System.Drawing.Size(98, 20);
			this.datePicker.TabIndex = 28;
			this.datePicker.Value = new System.DateTime(2019, 3, 1, 0, 0, 0, 0);
			// 
			// label6
			// 
			this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label6.Location = new System.Drawing.Point(298, 609);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(137, 23);
			this.label6.TabIndex = 27;
			this.label6.Text = "Minutes between games:";
			// 
			// label5
			// 
			this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label5.Location = new System.Drawing.Point(6, 609);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(99, 18);
			this.label5.TabIndex = 26;
			this.label5.Text = "First game time:";
			// 
			// label4
			// 
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label4.Location = new System.Drawing.Point(6, 6);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(748, 27);
			this.label4.TabIndex = 22;
			this.label4.Text = "Enter a grid of games, with each row being a team, each column being a game, and " +
    "each letter representing the colour of that team in that game: RGBYPMCOW";
			// 
			// textBoxGrid
			// 
			this.textBoxGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxGrid.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textBoxGrid.Location = new System.Drawing.Point(6, 36);
			this.textBoxGrid.Multiline = true;
			this.textBoxGrid.Name = "textBoxGrid";
			this.textBoxGrid.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBoxGrid.Size = new System.Drawing.Size(748, 561);
			this.textBoxGrid.TabIndex = 21;
			this.textBoxGrid.Text = "R...G.B\r\nBR...G.\r\n.BR...G\r\nG.BR...\r\n.G.BR..\r\n..G.BR.\r\n...G.BR\r\n";
			this.textBoxGrid.WordWrap = false;
			this.textBoxGrid.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBoxKeyDown);
			// 
			// tabGraphic
			// 
			this.tabGraphic.Controls.Add(this.numericSize);
			this.tabGraphic.Controls.Add(this.panelGraphic);
			this.tabGraphic.Location = new System.Drawing.Point(4, 22);
			this.tabGraphic.Name = "tabGraphic";
			this.tabGraphic.Padding = new System.Windows.Forms.Padding(3);
			this.tabGraphic.Size = new System.Drawing.Size(760, 632);
			this.tabGraphic.TabIndex = 3;
			this.tabGraphic.Text = "Graphic";
			this.tabGraphic.UseVisualStyleBackColor = true;
			// 
			// numericSize
			// 
			this.numericSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.numericSize.Location = new System.Drawing.Point(698, 606);
			this.numericSize.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
			this.numericSize.Name = "numericSize";
			this.numericSize.Size = new System.Drawing.Size(56, 20);
			this.numericSize.TabIndex = 1;
			this.numericSize.Value = new decimal(new int[] {
            15,
            0,
            0,
            0});
			this.numericSize.ValueChanged += new System.EventHandler(this.NumericSizeValueChanged);
			// 
			// panelGraphic
			// 
			this.panelGraphic.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panelGraphic.Location = new System.Drawing.Point(6, 6);
			this.panelGraphic.Name = "panelGraphic";
			this.panelGraphic.Size = new System.Drawing.Size(754, 626);
			this.panelGraphic.TabIndex = 0;
			this.panelGraphic.Paint += new System.Windows.Forms.PaintEventHandler(this.PanelGraphicPaint);
			this.panelGraphic.MouseClick += new System.Windows.Forms.MouseEventHandler(this.PanelGraphicMouseClick);
			// 
			// tabFinals
			// 
			this.tabFinals.Controls.Add(this.labelTeamsToSendUp);
			this.tabFinals.Controls.Add(this.numericFreeRides);
			this.tabFinals.Controls.Add(this.label10);
			this.tabFinals.Controls.Add(this.buttonFormatD);
			this.tabFinals.Controls.Add(this.buttonTwoTrack);
			this.tabFinals.Controls.Add(this.numericTeamsPerGame);
			this.tabFinals.Controls.Add(this.label9);
			this.tabFinals.Controls.Add(this.numericTeamsToCut);
			this.tabFinals.Controls.Add(this.label8);
			this.tabFinals.Controls.Add(this.numericTracks);
			this.tabFinals.Controls.Add(this.label7);
			this.tabFinals.Controls.Add(this.panelFinals);
			this.tabFinals.Controls.Add(this.buttonAscension);
			this.tabFinals.Location = new System.Drawing.Point(4, 22);
			this.tabFinals.Name = "tabFinals";
			this.tabFinals.Size = new System.Drawing.Size(760, 632);
			this.tabFinals.TabIndex = 4;
			this.tabFinals.Text = "Finals";
			this.tabFinals.UseVisualStyleBackColor = true;
			this.tabFinals.Enter += new System.EventHandler(this.RefreshFinals);
			// 
			// labelTeamsToSendUp
			// 
			this.labelTeamsToSendUp.AutoSize = true;
			this.labelTeamsToSendUp.Location = new System.Drawing.Point(528, 5);
			this.labelTeamsToSendUp.Name = "labelTeamsToSendUp";
			this.labelTeamsToSendUp.Size = new System.Drawing.Size(174, 13);
			this.labelTeamsToSendUp.TabIndex = 12;
			this.labelTeamsToSendUp.Text = "Teams to send up from each game:";
			// 
			// numericFreeRides
			// 
			this.numericFreeRides.Location = new System.Drawing.Point(472, 29);
			this.numericFreeRides.Maximum = new decimal(new int[] {
            2,
            0,
            0,
            0});
			this.numericFreeRides.Name = "numericFreeRides";
			this.numericFreeRides.Size = new System.Drawing.Size(50, 20);
			this.numericFreeRides.TabIndex = 11;
			this.numericFreeRides.ValueChanged += new System.EventHandler(this.RefreshFinals);
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(266, 31);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(200, 13);
			this.label10.TabIndex = 10;
			this.label10.Text = "Teams that get a free ride to grand finals:";
			// 
			// buttonFormatD
			// 
			this.buttonFormatD.Location = new System.Drawing.Point(175, 29);
			this.buttonFormatD.Name = "buttonFormatD";
			this.buttonFormatD.Size = new System.Drawing.Size(75, 23);
			this.buttonFormatD.TabIndex = 8;
			this.buttonFormatD.Text = "Format D";
			this.buttonFormatD.UseVisualStyleBackColor = true;
			this.buttonFormatD.Click += new System.EventHandler(this.ButtonFormatDClick);
			// 
			// buttonTwoTrack
			// 
			this.buttonTwoTrack.Location = new System.Drawing.Point(94, 29);
			this.buttonTwoTrack.Name = "buttonTwoTrack";
			this.buttonTwoTrack.Size = new System.Drawing.Size(75, 23);
			this.buttonTwoTrack.TabIndex = 7;
			this.buttonTwoTrack.Text = "Two Track";
			this.buttonTwoTrack.UseVisualStyleBackColor = true;
			this.buttonTwoTrack.Click += new System.EventHandler(this.ButtonTwoTrackClick);
			// 
			// numericTeamsPerGame
			// 
			this.numericTeamsPerGame.Location = new System.Drawing.Point(210, 3);
			this.numericTeamsPerGame.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
			this.numericTeamsPerGame.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericTeamsPerGame.Name = "numericTeamsPerGame";
			this.numericTeamsPerGame.Size = new System.Drawing.Size(50, 20);
			this.numericTeamsPerGame.TabIndex = 3;
			this.numericTeamsPerGame.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
			this.numericTeamsPerGame.ValueChanged += new System.EventHandler(this.NumericTeamsPerGameValueChanged);
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(115, 5);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(89, 13);
			this.label9.TabIndex = 2;
			this.label9.Text = "Teams per game:";
			// 
			// numericTeamsToCut
			// 
			this.numericTeamsToCut.Location = new System.Drawing.Point(472, 3);
			this.numericTeamsToCut.Maximum = new decimal(new int[] {
            2,
            0,
            0,
            0});
			this.numericTeamsToCut.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericTeamsToCut.Name = "numericTeamsToCut";
			this.numericTeamsToCut.Size = new System.Drawing.Size(50, 20);
			this.numericTeamsToCut.TabIndex = 5;
			this.numericTeamsToCut.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
			this.numericTeamsToCut.ValueChanged += new System.EventHandler(this.RefreshFinals);
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(266, 5);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(188, 13);
			this.label8.TabIndex = 4;
			this.label8.Text = "Teams to send down from each game:";
			// 
			// numericTracks
			// 
			this.numericTracks.Location = new System.Drawing.Point(59, 3);
			this.numericTracks.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
			this.numericTracks.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericTracks.Name = "numericTracks";
			this.numericTracks.Size = new System.Drawing.Size(50, 20);
			this.numericTracks.TabIndex = 1;
			this.numericTracks.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
			this.numericTracks.ValueChanged += new System.EventHandler(this.RefreshFinals);
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(10, 5);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(43, 13);
			this.label7.TabIndex = 0;
			this.label7.Text = "Tracks:";
			// 
			// panelFinals
			// 
			this.panelFinals.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panelFinals.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.panelFinals.Location = new System.Drawing.Point(3, 68);
			this.panelFinals.Name = "panelFinals";
			this.panelFinals.Size = new System.Drawing.Size(754, 561);
			this.panelFinals.TabIndex = 9;
			this.panelFinals.Resize += new System.EventHandler(this.PanelFinalsResize);
			// 
			// buttonAscension
			// 
			this.buttonAscension.Location = new System.Drawing.Point(13, 29);
			this.buttonAscension.Name = "buttonAscension";
			this.buttonAscension.Size = new System.Drawing.Size(75, 23);
			this.buttonAscension.TabIndex = 6;
			this.buttonAscension.Text = "Ascension";
			this.buttonAscension.UseVisualStyleBackColor = true;
			this.buttonAscension.Click += new System.EventHandler(this.ButtonAscensionClick);
			// 
			// timerRedraw
			// 
			this.timerRedraw.Interval = 1000;
			this.timerRedraw.Tick += new System.EventHandler(this.TimerRedrawTick);
			// 
			// FormFixture
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(792, 682);
			this.Controls.Add(this.tabControl1);
			this.Name = "FormFixture";
			this.Text = "Fixtures";
			this.Shown += new System.EventHandler(this.FormFixtureShown);
			this.ResizeBegin += new System.EventHandler(this.FormFixtureResizeBegin);
			this.ResizeEnd += new System.EventHandler(this.FormFixtureResizeEnd);
			this.tabControl1.ResumeLayout(false);
			this.tabTeams.ResumeLayout(false);
			this.tabTeams.PerformLayout();
			this.tabGamesList.ResumeLayout(false);
			this.tabGamesList.PerformLayout();
			this.tabGamesGrid.ResumeLayout(false);
			this.tabGamesGrid.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericMinutes)).EndInit();
			this.tabGraphic.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.numericSize)).EndInit();
			this.tabFinals.ResumeLayout(false);
			this.tabFinals.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericFreeRides)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericTeamsPerGame)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericTeamsToCut)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericTracks)).EndInit();
			this.ResumeLayout(false);

		}
		private System.Windows.Forms.Panel panelGraphic;
		private System.Windows.Forms.NumericUpDown numericSize;
		private System.Windows.Forms.TabPage tabGraphic;
		private System.Windows.Forms.Button buttonClearGrid;
		private System.Windows.Forms.Button buttonClearGames;
		private System.Windows.Forms.TextBox textBoxGrid;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.DateTimePicker datePicker;
		private System.Windows.Forms.DateTimePicker timePicker;
		private System.Windows.Forms.NumericUpDown numericMinutes;
		private System.Windows.Forms.TabPage tabGamesGrid;
		private System.Windows.Forms.TabPage tabGamesList;
		private System.Windows.Forms.TabPage tabTeams;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.Button buttonImportGrid;
		private System.Windows.Forms.TextBox textBoxSeparator;
		private System.Windows.Forms.RadioButton radioButtonOther;
		private System.Windows.Forms.RadioButton radioButtonTab;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button buttonImportGames;
		private System.Windows.Forms.Button buttonImportTeams;
		private System.Windows.Forms.TextBox textBoxGames;
		private System.Windows.Forms.TextBox textBoxTeams;
		private System.Windows.Forms.TabPage tabFinals;
		private System.Windows.Forms.Panel panelFinals;
		private System.Windows.Forms.Button buttonAscension;
		private System.Windows.Forms.Timer timerRedraw;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.NumericUpDown numericTeamsPerGame;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.NumericUpDown numericTeamsToCut;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.NumericUpDown numericTracks;
		private System.Windows.Forms.Button buttonFormatD;
		private System.Windows.Forms.Button buttonTwoTrack;
		private System.Windows.Forms.NumericUpDown numericFreeRides;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label labelTeamsToSendUp;
	}
}
