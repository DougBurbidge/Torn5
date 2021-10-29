
namespace Torn.UI
{
	partial class FormReports
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
			this.listViewReports = new System.Windows.Forms.ListView();
			this.colReport = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.colTitle = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.colOptions = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.buttonAdd = new System.Windows.Forms.Button();
			this.buttonDelete = new System.Windows.Forms.Button();
			this.buttonUp = new System.Windows.Forms.Button();
			this.buttonDown = new System.Windows.Forms.Button();
			this.buttonEdit = new System.Windows.Forms.Button();
			this.groupBoxOutputFormat = new System.Windows.Forms.GroupBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.radioCsv = new System.Windows.Forms.RadioButton();
			this.radioTsv = new System.Windows.Forms.RadioButton();
			this.radioTables = new System.Windows.Forms.RadioButton();
			this.radioSvg = new System.Windows.Forms.RadioButton();
			this.buttonDefaults = new System.Windows.Forms.Button();
			this.groupBoxOutputFormat.SuspendLayout();
			this.SuspendLayout();
			// 
			// listViewReports
			// 
			this.listViewReports.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listViewReports.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colReport,
            this.colTitle,
            this.colOptions});
			this.listViewReports.FullRowSelect = true;
			this.listViewReports.HideSelection = false;
			this.listViewReports.Location = new System.Drawing.Point(12, 41);
			this.listViewReports.Name = "listViewReports";
			this.listViewReports.Size = new System.Drawing.Size(608, 312);
			this.listViewReports.TabIndex = 5;
			this.listViewReports.UseCompatibleStateImageBehavior = false;
			this.listViewReports.View = System.Windows.Forms.View.Details;
			this.listViewReports.DoubleClick += new System.EventHandler(this.ButtonEditClick);
			this.listViewReports.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listViewReports_KeyDown);
			// 
			// colReport
			// 
			this.colReport.Text = "Report";
			this.colReport.Width = 100;
			// 
			// colTitle
			// 
			this.colTitle.Text = "Title";
			// 
			// colOptions
			// 
			this.colOptions.Text = "Options";
			this.colOptions.Width = 440;
			// 
			// buttonAdd
			// 
			this.buttonAdd.Location = new System.Drawing.Point(12, 12);
			this.buttonAdd.Name = "buttonAdd";
			this.buttonAdd.Size = new System.Drawing.Size(75, 23);
			this.buttonAdd.TabIndex = 0;
			this.buttonAdd.Text = "&Add";
			this.buttonAdd.UseVisualStyleBackColor = true;
			this.buttonAdd.Click += new System.EventHandler(this.ButtonAddClick);
			// 
			// buttonDelete
			// 
			this.buttonDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonDelete.Location = new System.Drawing.Point(174, 12);
			this.buttonDelete.Name = "buttonDelete";
			this.buttonDelete.Size = new System.Drawing.Size(75, 23);
			this.buttonDelete.TabIndex = 2;
			this.buttonDelete.Text = "&Delete";
			this.buttonDelete.UseVisualStyleBackColor = true;
			this.buttonDelete.Click += new System.EventHandler(this.ButtonDeleteClick);
			// 
			// buttonUp
			// 
			this.buttonUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonUp.Location = new System.Drawing.Point(255, 12);
			this.buttonUp.Name = "buttonUp";
			this.buttonUp.Size = new System.Drawing.Size(75, 23);
			this.buttonUp.TabIndex = 3;
			this.buttonUp.Text = "Move Up";
			this.buttonUp.UseVisualStyleBackColor = true;
			this.buttonUp.Click += new System.EventHandler(this.ButtonUpClick);
			// 
			// buttonDown
			// 
			this.buttonDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonDown.Location = new System.Drawing.Point(336, 12);
			this.buttonDown.Name = "buttonDown";
			this.buttonDown.Size = new System.Drawing.Size(75, 23);
			this.buttonDown.TabIndex = 4;
			this.buttonDown.Text = "Move Down";
			this.buttonDown.UseVisualStyleBackColor = true;
			this.buttonDown.Click += new System.EventHandler(this.ButtonDownClick);
			// 
			// buttonEdit
			// 
			this.buttonEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonEdit.Location = new System.Drawing.Point(93, 12);
			this.buttonEdit.Name = "buttonEdit";
			this.buttonEdit.Size = new System.Drawing.Size(75, 23);
			this.buttonEdit.TabIndex = 1;
			this.buttonEdit.Text = "&Edit";
			this.buttonEdit.UseVisualStyleBackColor = true;
			this.buttonEdit.Click += new System.EventHandler(this.ButtonEditClick);
			// 
			// groupBoxOutputFormat
			// 
			this.groupBoxOutputFormat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBoxOutputFormat.Controls.Add(this.label2);
			this.groupBoxOutputFormat.Controls.Add(this.label1);
			this.groupBoxOutputFormat.Controls.Add(this.radioCsv);
			this.groupBoxOutputFormat.Controls.Add(this.radioTsv);
			this.groupBoxOutputFormat.Controls.Add(this.radioTables);
			this.groupBoxOutputFormat.Controls.Add(this.radioSvg);
			this.groupBoxOutputFormat.Location = new System.Drawing.Point(12, 359);
			this.groupBoxOutputFormat.Name = "groupBoxOutputFormat";
			this.groupBoxOutputFormat.Size = new System.Drawing.Size(608, 71);
			this.groupBoxOutputFormat.TabIndex = 6;
			this.groupBoxOutputFormat.TabStop = false;
			this.groupBoxOutputFormat.Text = "Output format";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(259, 46);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(119, 23);
			this.label2.TabIndex = 5;
			this.label2.Text = "(best for data export)";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(22, 46);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(63, 23);
			this.label1.TabIndex = 4;
			this.label1.Text = "(prettiest)";
			// 
			// radioCsv
			// 
			this.radioCsv.Location = new System.Drawing.Point(353, 19);
			this.radioCsv.Name = "radioCsv";
			this.radioCsv.Size = new System.Drawing.Size(64, 24);
			this.radioCsv.TabIndex = 3;
			this.radioCsv.TabStop = true;
			this.radioCsv.Tag = 3;
			this.radioCsv.Text = "CSV";
			this.radioCsv.UseVisualStyleBackColor = true;
			this.radioCsv.CheckedChanged += new System.EventHandler(this.RadioCheckedChanged);
			// 
			// radioTsv
			// 
			this.radioTsv.Location = new System.Drawing.Point(243, 19);
			this.radioTsv.Name = "radioTsv";
			this.radioTsv.Size = new System.Drawing.Size(64, 24);
			this.radioTsv.TabIndex = 2;
			this.radioTsv.TabStop = true;
			this.radioTsv.Tag = 2;
			this.radioTsv.Text = "TSV";
			this.radioTsv.UseVisualStyleBackColor = true;
			this.radioTsv.CheckedChanged += new System.EventHandler(this.RadioCheckedChanged);
			// 
			// radioTables
			// 
			this.radioTables.Location = new System.Drawing.Point(116, 19);
			this.radioTables.Name = "radioTables";
			this.radioTables.Size = new System.Drawing.Size(92, 24);
			this.radioTables.TabIndex = 1;
			this.radioTables.TabStop = true;
			this.radioTables.Tag = 1;
			this.radioTables.Text = "HTML tables";
			this.radioTables.UseVisualStyleBackColor = true;
			this.radioTables.CheckedChanged += new System.EventHandler(this.RadioCheckedChanged);
			// 
			// radioSvg
			// 
			this.radioSvg.Location = new System.Drawing.Point(6, 19);
			this.radioSvg.Name = "radioSvg";
			this.radioSvg.Size = new System.Drawing.Size(80, 24);
			this.radioSvg.TabIndex = 0;
			this.radioSvg.TabStop = true;
			this.radioSvg.Tag = 0;
			this.radioSvg.Text = "HTML SVG";
			this.radioSvg.UseVisualStyleBackColor = true;
			this.radioSvg.CheckedChanged += new System.EventHandler(this.RadioCheckedChanged);
			// 
			// buttonDefaults
			// 
			this.buttonDefaults.Location = new System.Drawing.Point(514, 12);
			this.buttonDefaults.Name = "buttonDefaults";
			this.buttonDefaults.Size = new System.Drawing.Size(104, 23);
			this.buttonDefaults.TabIndex = 7;
			this.buttonDefaults.Text = "Add default reports";
			this.buttonDefaults.UseVisualStyleBackColor = true;
			this.buttonDefaults.Click += new System.EventHandler(this.buttonDefaults_Click);
			// 
			// FormReports
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(632, 442);
			this.Controls.Add(this.buttonDefaults);
			this.Controls.Add(this.groupBoxOutputFormat);
			this.Controls.Add(this.buttonEdit);
			this.Controls.Add(this.buttonDown);
			this.Controls.Add(this.buttonUp);
			this.Controls.Add(this.buttonDelete);
			this.Controls.Add(this.buttonAdd);
			this.Controls.Add(this.listViewReports);
			this.Name = "FormReports";
			this.Text = "Reports";
			this.Shown += new System.EventHandler(this.FormReportsShown);
			this.groupBoxOutputFormat.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		private System.Windows.Forms.RadioButton radioSvg;
		private System.Windows.Forms.RadioButton radioTables;
		private System.Windows.Forms.RadioButton radioTsv;
		private System.Windows.Forms.RadioButton radioCsv;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.GroupBox groupBoxOutputFormat;
		private System.Windows.Forms.ColumnHeader colTitle;
		private System.Windows.Forms.ColumnHeader colOptions;
		private System.Windows.Forms.ColumnHeader colReport;
		private System.Windows.Forms.Button buttonEdit;
		private System.Windows.Forms.Button buttonDown;
		private System.Windows.Forms.Button buttonUp;
		private System.Windows.Forms.Button buttonDelete;
		private System.Windows.Forms.Button buttonAdd;
		private System.Windows.Forms.ListView listViewReports;
		private System.Windows.Forms.Button buttonDefaults;
	}
}
