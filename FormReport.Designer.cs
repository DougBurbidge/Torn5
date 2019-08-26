
namespace Torn.UI
{
	partial class FormReport
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
			this.labelReport = new System.Windows.Forms.Label();
			this.scaleGames = new System.Windows.Forms.CheckBox();
			this.dropGames = new System.Windows.Forms.CheckBox();
			this.showColours = new System.Windows.Forms.CheckBox();
			this.showPoints = new System.Windows.Forms.CheckBox();
			this.showComments = new System.Windows.Forms.CheckBox();
			this.showTopN = new System.Windows.Forms.CheckBox();
			this.atLeastN = new System.Windows.Forms.CheckBox();
			this.labelOrderBy = new System.Windows.Forms.Label();
			this.labelTopWhat = new System.Windows.Forms.Label();
			this.labelAtLeastGames = new System.Windows.Forms.Label();
			this.numericUpDownTopN = new System.Windows.Forms.NumericUpDown();
			this.numericUpDownAtLeastN = new System.Windows.Forms.NumericUpDown();
			this.orderBy = new System.Windows.Forms.ComboBox();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.groupBoxDateRange = new System.Windows.Forms.GroupBox();
			this.datePickerTo = new System.Windows.Forms.DateTimePicker();
			this.datePickerFrom = new System.Windows.Forms.DateTimePicker();
			this.dateTo = new System.Windows.Forms.CheckBox();
			this.dateFrom = new System.Windows.Forms.CheckBox();
			this.timePickerTo = new System.Windows.Forms.DateTimePicker();
			this.timePickerFrom = new System.Windows.Forms.DateTimePicker();
			this.groupBoxDrops = new System.Windows.Forms.GroupBox();
			this.radioButtonPercent = new System.Windows.Forms.RadioButton();
			this.radioButtonGames = new System.Windows.Forms.RadioButton();
			this.numericUpDownWorst = new System.Windows.Forms.NumericUpDown();
			this.numericUpDownBest = new System.Windows.Forms.NumericUpDown();
			this.labelDropWorst = new System.Windows.Forms.Label();
			this.labelDropBest = new System.Windows.Forms.Label();
			this.listBoxReportType = new System.Windows.Forms.ListBox();
			this.description = new System.Windows.Forms.CheckBox();
			this.labelChartType = new System.Windows.Forms.Label();
			this.chartType = new System.Windows.Forms.ComboBox();
			this.labelTitle = new System.Windows.Forms.Label();
			this.title = new System.Windows.Forms.TextBox();
			this.longitudinal = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownTopN)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownAtLeastN)).BeginInit();
			this.groupBoxDateRange.SuspendLayout();
			this.groupBoxDrops.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownWorst)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownBest)).BeginInit();
			this.SuspendLayout();
			// 
			// labelReport
			// 
			this.labelReport.Location = new System.Drawing.Point(12, 15);
			this.labelReport.Name = "labelReport";
			this.labelReport.Size = new System.Drawing.Size(44, 23);
			this.labelReport.TabIndex = 0;
			this.labelReport.Text = "Report:";
			// 
			// scaleGames
			// 
			this.scaleGames.Location = new System.Drawing.Point(12, 196);
			this.scaleGames.Name = "scaleGames";
			this.scaleGames.Size = new System.Drawing.Size(250, 24);
			this.scaleGames.TabIndex = 2;
			this.scaleGames.Tag = "ScaleGames";
			this.scaleGames.Text = "scale up teams with less games";
			this.scaleGames.UseVisualStyleBackColor = true;
			// 
			// dropGames
			// 
			this.dropGames.Location = new System.Drawing.Point(12, 226);
			this.dropGames.Name = "dropGames";
			this.dropGames.Size = new System.Drawing.Size(250, 24);
			this.dropGames.TabIndex = 3;
			this.dropGames.Text = "drop best/worst games";
			this.dropGames.UseVisualStyleBackColor = true;
			this.dropGames.CheckedChanged += new System.EventHandler(this.DropGamesCheckedChanged);
			// 
			// showColours
			// 
			this.showColours.Location = new System.Drawing.Point(12, 412);
			this.showColours.Name = "showColours";
			this.showColours.Size = new System.Drawing.Size(250, 24);
			this.showColours.TabIndex = 7;
			this.showColours.Tag = "ShowColours";
			this.showColours.Text = "show colours";
			this.showColours.UseVisualStyleBackColor = true;
			// 
			// showPoints
			// 
			this.showPoints.Location = new System.Drawing.Point(12, 442);
			this.showPoints.Name = "showPoints";
			this.showPoints.Size = new System.Drawing.Size(250, 24);
			this.showPoints.TabIndex = 8;
			this.showPoints.Tag = "ShowPoints";
			this.showPoints.Text = "show average victory points";
			this.showPoints.UseVisualStyleBackColor = true;
			// 
			// showComments
			// 
			this.showComments.Location = new System.Drawing.Point(12, 472);
			this.showComments.Name = "showComments";
			this.showComments.Size = new System.Drawing.Size(250, 24);
			this.showComments.TabIndex = 9;
			this.showComments.Tag = "ShowComments";
			this.showComments.Text = "show comments";
			this.showComments.UseVisualStyleBackColor = true;
			// 
			// showTopN
			// 
			this.showTopN.Location = new System.Drawing.Point(12, 532);
			this.showTopN.Name = "showTopN";
			this.showTopN.Size = new System.Drawing.Size(97, 24);
			this.showTopN.TabIndex = 12;
			this.showTopN.Text = "show only top";
			this.showTopN.UseVisualStyleBackColor = true;
			// 
			// atLeastN
			// 
			this.atLeastN.Location = new System.Drawing.Point(12, 562);
			this.atLeastN.Name = "atLeastN";
			this.atLeastN.Size = new System.Drawing.Size(186, 24);
			this.atLeastN.TabIndex = 15;
			this.atLeastN.Text = "show only players with at least";
			this.atLeastN.UseVisualStyleBackColor = true;
			// 
			// labelOrderBy
			// 
			this.labelOrderBy.Location = new System.Drawing.Point(12, 595);
			this.labelOrderBy.Name = "labelOrderBy";
			this.labelOrderBy.Size = new System.Drawing.Size(59, 23);
			this.labelOrderBy.TabIndex = 18;
			this.labelOrderBy.Text = "order by";
			// 
			// labelTopWhat
			// 
			this.labelTopWhat.Location = new System.Drawing.Point(185, 535);
			this.labelTopWhat.Name = "labelTopWhat";
			this.labelTopWhat.Size = new System.Drawing.Size(51, 23);
			this.labelTopWhat.TabIndex = 14;
			this.labelTopWhat.Text = "players";
			// 
			// labelAtLeastGames
			// 
			this.labelAtLeastGames.Location = new System.Drawing.Point(242, 567);
			this.labelAtLeastGames.Name = "labelAtLeastGames";
			this.labelAtLeastGames.Size = new System.Drawing.Size(41, 23);
			this.labelAtLeastGames.TabIndex = 17;
			this.labelAtLeastGames.Text = "games";
			// 
			// numericUpDownTopN
			// 
			this.numericUpDownTopN.Increment = new decimal(new int[] {
									10,
									0,
									0,
									0});
			this.numericUpDownTopN.Location = new System.Drawing.Point(129, 535);
			this.numericUpDownTopN.Maximum = new decimal(new int[] {
									1000,
									0,
									0,
									0});
			this.numericUpDownTopN.Name = "numericUpDownTopN";
			this.numericUpDownTopN.Size = new System.Drawing.Size(50, 20);
			this.numericUpDownTopN.TabIndex = 13;
			this.numericUpDownTopN.Value = new decimal(new int[] {
									40,
									0,
									0,
									0});
			// 
			// numericUpDownAtLeastN
			// 
			this.numericUpDownAtLeastN.Location = new System.Drawing.Point(185, 565);
			this.numericUpDownAtLeastN.Name = "numericUpDownAtLeastN";
			this.numericUpDownAtLeastN.Size = new System.Drawing.Size(50, 20);
			this.numericUpDownAtLeastN.TabIndex = 16;
			this.numericUpDownAtLeastN.Value = new decimal(new int[] {
									7,
									0,
									0,
									0});
			// 
			// orderBy
			// 
			this.orderBy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.orderBy.FormattingEnabled = true;
			this.orderBy.Items.AddRange(new object[] {
									"score",
									"score ratio"});
			this.orderBy.Location = new System.Drawing.Point(73, 592);
			this.orderBy.Name = "orderBy";
			this.orderBy.Size = new System.Drawing.Size(189, 21);
			this.orderBy.TabIndex = 19;
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point(129, 659);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 21;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(211, 659);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 22;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// groupBoxDateRange
			// 
			this.groupBoxDateRange.Controls.Add(this.datePickerTo);
			this.groupBoxDateRange.Controls.Add(this.datePickerFrom);
			this.groupBoxDateRange.Controls.Add(this.dateTo);
			this.groupBoxDateRange.Controls.Add(this.dateFrom);
			this.groupBoxDateRange.Controls.Add(this.timePickerTo);
			this.groupBoxDateRange.Controls.Add(this.timePickerFrom);
			this.groupBoxDateRange.Location = new System.Drawing.Point(4, 335);
			this.groupBoxDateRange.Name = "groupBoxDateRange";
			this.groupBoxDateRange.Size = new System.Drawing.Size(274, 71);
			this.groupBoxDateRange.TabIndex = 6;
			this.groupBoxDateRange.TabStop = false;
			// 
			// datePickerTo
			// 
			this.datePickerTo.CustomFormat = "";
			this.datePickerTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			this.datePickerTo.Location = new System.Drawing.Point(58, 45);
			this.datePickerTo.Name = "datePickerTo";
			this.datePickerTo.Size = new System.Drawing.Size(91, 20);
			this.datePickerTo.TabIndex = 4;
			this.datePickerTo.Value = new System.DateTime(2019, 1, 1, 0, 0, 0, 0);
			this.datePickerTo.ValueChanged += new System.EventHandler(this.DatePickerToValueChanged);
			// 
			// datePickerFrom
			// 
			this.datePickerFrom.CustomFormat = "";
			this.datePickerFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			this.datePickerFrom.Location = new System.Drawing.Point(58, 19);
			this.datePickerFrom.Name = "datePickerFrom";
			this.datePickerFrom.Size = new System.Drawing.Size(91, 20);
			this.datePickerFrom.TabIndex = 1;
			this.datePickerFrom.Value = new System.DateTime(2001, 1, 1, 0, 0, 0, 0);
			this.datePickerFrom.ValueChanged += new System.EventHandler(this.DatePickerFromValueChanged);
			// 
			// dateTo
			// 
			this.dateTo.Location = new System.Drawing.Point(8, 44);
			this.dateTo.Name = "dateTo";
			this.dateTo.Size = new System.Drawing.Size(53, 24);
			this.dateTo.TabIndex = 3;
			this.dateTo.Text = "to";
			this.dateTo.UseVisualStyleBackColor = true;
			// 
			// dateFrom
			// 
			this.dateFrom.Location = new System.Drawing.Point(8, 18);
			this.dateFrom.Name = "dateFrom";
			this.dateFrom.Size = new System.Drawing.Size(53, 24);
			this.dateFrom.TabIndex = 0;
			this.dateFrom.Text = "from";
			this.dateFrom.UseVisualStyleBackColor = true;
			// 
			// timePickerTo
			// 
			this.timePickerTo.CustomFormat = "";
			this.timePickerTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.timePickerTo.Location = new System.Drawing.Point(159, 45);
			this.timePickerTo.Name = "timePickerTo";
			this.timePickerTo.ShowUpDown = true;
			this.timePickerTo.Size = new System.Drawing.Size(91, 20);
			this.timePickerTo.TabIndex = 5;
			this.timePickerTo.Value = new System.DateTime(2001, 1, 1, 23, 59, 59, 0);
			this.timePickerTo.ValueChanged += new System.EventHandler(this.DatePickerToValueChanged);
			// 
			// timePickerFrom
			// 
			this.timePickerFrom.CustomFormat = "";
			this.timePickerFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.timePickerFrom.Location = new System.Drawing.Point(159, 19);
			this.timePickerFrom.Name = "timePickerFrom";
			this.timePickerFrom.ShowUpDown = true;
			this.timePickerFrom.Size = new System.Drawing.Size(91, 20);
			this.timePickerFrom.TabIndex = 2;
			this.timePickerFrom.Value = new System.DateTime(2001, 1, 1, 0, 0, 0, 0);
			this.timePickerFrom.ValueChanged += new System.EventHandler(this.DatePickerFromValueChanged);
			// 
			// groupBoxDrops
			// 
			this.groupBoxDrops.Controls.Add(this.radioButtonPercent);
			this.groupBoxDrops.Controls.Add(this.radioButtonGames);
			this.groupBoxDrops.Controls.Add(this.numericUpDownWorst);
			this.groupBoxDrops.Controls.Add(this.numericUpDownBest);
			this.groupBoxDrops.Controls.Add(this.labelDropWorst);
			this.groupBoxDrops.Controls.Add(this.labelDropBest);
			this.groupBoxDrops.Enabled = false;
			this.groupBoxDrops.Location = new System.Drawing.Point(24, 256);
			this.groupBoxDrops.Name = "groupBoxDrops";
			this.groupBoxDrops.Size = new System.Drawing.Size(262, 73);
			this.groupBoxDrops.TabIndex = 4;
			this.groupBoxDrops.TabStop = false;
			// 
			// radioButtonPercent
			// 
			this.radioButtonPercent.Location = new System.Drawing.Point(127, 45);
			this.radioButtonPercent.Name = "radioButtonPercent";
			this.radioButtonPercent.Size = new System.Drawing.Size(122, 24);
			this.radioButtonPercent.TabIndex = 5;
			this.radioButtonPercent.TabStop = true;
			this.radioButtonPercent.Text = "percent of games";
			this.radioButtonPercent.UseVisualStyleBackColor = true;
			// 
			// radioButtonGames
			// 
			this.radioButtonGames.Location = new System.Drawing.Point(49, 45);
			this.radioButtonGames.Name = "radioButtonGames";
			this.radioButtonGames.Size = new System.Drawing.Size(68, 24);
			this.radioButtonGames.TabIndex = 4;
			this.radioButtonGames.TabStop = true;
			this.radioButtonGames.Text = "games";
			this.radioButtonGames.UseVisualStyleBackColor = true;
			// 
			// numericUpDownWorst
			// 
			this.numericUpDownWorst.Location = new System.Drawing.Point(180, 19);
			this.numericUpDownWorst.Name = "numericUpDownWorst";
			this.numericUpDownWorst.Size = new System.Drawing.Size(50, 20);
			this.numericUpDownWorst.TabIndex = 3;
			// 
			// numericUpDownBest
			// 
			this.numericUpDownBest.Location = new System.Drawing.Point(65, 19);
			this.numericUpDownBest.Name = "numericUpDownBest";
			this.numericUpDownBest.Size = new System.Drawing.Size(50, 20);
			this.numericUpDownBest.TabIndex = 1;
			// 
			// labelDropWorst
			// 
			this.labelDropWorst.AutoSize = true;
			this.labelDropWorst.Location = new System.Drawing.Point(121, 21);
			this.labelDropWorst.Name = "labelDropWorst";
			this.labelDropWorst.Size = new System.Drawing.Size(53, 13);
			this.labelDropWorst.TabIndex = 2;
			this.labelDropWorst.Text = "and worst";
			// 
			// labelDropBest
			// 
			this.labelDropBest.AutoSize = true;
			this.labelDropBest.Location = new System.Drawing.Point(6, 21);
			this.labelDropBest.Name = "labelDropBest";
			this.labelDropBest.Size = new System.Drawing.Size(53, 13);
			this.labelDropBest.TabIndex = 0;
			this.labelDropBest.Text = "Drop best";
			// 
			// listBoxReportType
			// 
			this.listBoxReportType.FormattingEnabled = true;
			this.listBoxReportType.Items.AddRange(new object[] {
									"Team ladder",
									"Teams vs teams",
									"Solo ladder",
									"Game by game (good for 3 team games)",
									"Game grid (good for many team games)",
									"Game grid condensed",
									"Ascension",
									"Pyramid",
									"Pyramid condensed",
									"Packs",
									"Everything"});
			this.listBoxReportType.Location = new System.Drawing.Point(62, 12);
			this.listBoxReportType.Name = "listBoxReportType";
			this.listBoxReportType.Size = new System.Drawing.Size(211, 147);
			this.listBoxReportType.TabIndex = 1;
			this.listBoxReportType.SelectedIndexChanged += new System.EventHandler(this.ListBoxReportTypeSelectedIndexChanged);
			// 
			// description
			// 
			this.description.Checked = true;
			this.description.CheckState = System.Windows.Forms.CheckState.Checked;
			this.description.Location = new System.Drawing.Point(12, 619);
			this.description.Name = "description";
			this.description.Size = new System.Drawing.Size(88, 24);
			this.description.TabIndex = 20;
			this.description.Tag = "Description";
			this.description.Text = "description";
			this.description.UseVisualStyleBackColor = true;
			// 
			// labelChartType
			// 
			this.labelChartType.Location = new System.Drawing.Point(12, 502);
			this.labelChartType.Name = "labelChartType";
			this.labelChartType.Size = new System.Drawing.Size(59, 23);
			this.labelChartType.TabIndex = 10;
			this.labelChartType.Text = "chart type";
			// 
			// chartType
			// 
			this.chartType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.chartType.FormattingEnabled = true;
			this.chartType.Items.AddRange(new object[] {
									"none",
									"bar",
									"rug plot",
									"bar with rug",
									"box plot",
									"box plot with rug",
									"histogram",
									"kernel density estimate",
									"kernel density estimate with rug"});
			this.chartType.Location = new System.Drawing.Point(73, 499);
			this.chartType.Name = "chartType";
			this.chartType.Size = new System.Drawing.Size(189, 21);
			this.chartType.TabIndex = 11;
			// 
			// labelTitle
			// 
			this.labelTitle.Location = new System.Drawing.Point(12, 173);
			this.labelTitle.Name = "labelTitle";
			this.labelTitle.Size = new System.Drawing.Size(100, 23);
			this.labelTitle.TabIndex = 23;
			this.labelTitle.Text = "Title:";
			// 
			// title
			// 
			this.title.Location = new System.Drawing.Point(62, 170);
			this.title.Name = "title";
			this.title.Size = new System.Drawing.Size(211, 20);
			this.title.TabIndex = 24;
			// 
			// longitudinal
			// 
			this.longitudinal.Checked = true;
			this.longitudinal.CheckState = System.Windows.Forms.CheckState.Checked;
			this.longitudinal.Location = new System.Drawing.Point(153, 619);
			this.longitudinal.Name = "longitudinal";
			this.longitudinal.Size = new System.Drawing.Size(128, 24);
			this.longitudinal.TabIndex = 25;
			this.longitudinal.Tag = "Longitudinal";
			this.longitudinal.Text = "longitudinal chart";
			this.longitudinal.UseVisualStyleBackColor = true;
			// 
			// FormReport
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(298, 694);
			this.Controls.Add(this.longitudinal);
			this.Controls.Add(this.title);
			this.Controls.Add(this.labelTitle);
			this.Controls.Add(this.chartType);
			this.Controls.Add(this.labelChartType);
			this.Controls.Add(this.description);
			this.Controls.Add(this.listBoxReportType);
			this.Controls.Add(this.groupBoxDrops);
			this.Controls.Add(this.groupBoxDateRange);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.orderBy);
			this.Controls.Add(this.numericUpDownAtLeastN);
			this.Controls.Add(this.numericUpDownTopN);
			this.Controls.Add(this.labelAtLeastGames);
			this.Controls.Add(this.labelTopWhat);
			this.Controls.Add(this.labelOrderBy);
			this.Controls.Add(this.atLeastN);
			this.Controls.Add(this.showTopN);
			this.Controls.Add(this.showComments);
			this.Controls.Add(this.showPoints);
			this.Controls.Add(this.showColours);
			this.Controls.Add(this.dropGames);
			this.Controls.Add(this.scaleGames);
			this.Controls.Add(this.labelReport);
			this.Name = "FormReport";
			this.Text = "Report";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormReportFormClosed);
			this.Shown += new System.EventHandler(this.FormReportShown);
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownTopN)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownAtLeastN)).EndInit();
			this.groupBoxDateRange.ResumeLayout(false);
			this.groupBoxDrops.ResumeLayout(false);
			this.groupBoxDrops.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownWorst)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownBest)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.CheckBox longitudinal;
		private System.Windows.Forms.TextBox title;
		private System.Windows.Forms.Label labelTitle;
		private System.Windows.Forms.CheckBox dateFrom;
		private System.Windows.Forms.CheckBox dateTo;
		private System.Windows.Forms.ComboBox chartType;
		private System.Windows.Forms.Label labelChartType;
		private System.Windows.Forms.DateTimePicker timePickerFrom;
		private System.Windows.Forms.DateTimePicker timePickerTo;
		private System.Windows.Forms.CheckBox description;
		private System.Windows.Forms.ListBox listBoxReportType;
		private System.Windows.Forms.Label labelDropBest;
		private System.Windows.Forms.Label labelDropWorst;
		private System.Windows.Forms.NumericUpDown numericUpDownBest;
		private System.Windows.Forms.NumericUpDown numericUpDownWorst;
		private System.Windows.Forms.RadioButton radioButtonGames;
		private System.Windows.Forms.RadioButton radioButtonPercent;
		private System.Windows.Forms.GroupBox groupBoxDrops;
		private System.Windows.Forms.DateTimePicker datePickerFrom;
		private System.Windows.Forms.DateTimePicker datePickerTo;
		private System.Windows.Forms.GroupBox groupBoxDateRange;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.ComboBox orderBy;
		private System.Windows.Forms.NumericUpDown numericUpDownAtLeastN;
		private System.Windows.Forms.NumericUpDown numericUpDownTopN;
		private System.Windows.Forms.Label labelAtLeastGames;
		private System.Windows.Forms.Label labelTopWhat;
		private System.Windows.Forms.Label labelOrderBy;
		private System.Windows.Forms.CheckBox atLeastN;
		private System.Windows.Forms.CheckBox showTopN;
		private System.Windows.Forms.CheckBox showComments;
		private System.Windows.Forms.CheckBox showPoints;
		private System.Windows.Forms.CheckBox showColours;
		private System.Windows.Forms.CheckBox dropGames;
		private System.Windows.Forms.CheckBox scaleGames;
		private System.Windows.Forms.Label labelReport;
	}
}
