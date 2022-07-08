
namespace Torn5.Controls
{
	partial class PrintReport
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.groupBoxOutputFormat = new System.Windows.Forms.GroupBox();
			this.label2 = new System.Windows.Forms.Label();
			this.radioPng = new System.Windows.Forms.RadioButton();
			this.buttonPrintPreview = new System.Windows.Forms.Button();
			this.buttonPrint = new System.Windows.Forms.Button();
			this.buttonShow = new System.Windows.Forms.Button();
			this.radioCsv = new System.Windows.Forms.RadioButton();
			this.buttonSave = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.radioTsv = new System.Windows.Forms.RadioButton();
			this.radioTables = new System.Windows.Forms.RadioButton();
			this.radioSvg = new System.Windows.Forms.RadioButton();
			this.numericScale = new System.Windows.Forms.NumericUpDown();
			this.groupBoxOutputFormat.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericScale)).BeginInit();
			this.SuspendLayout();
			// 
			// groupBoxOutputFormat
			// 
			this.groupBoxOutputFormat.Controls.Add(this.numericScale);
			this.groupBoxOutputFormat.Controls.Add(this.label2);
			this.groupBoxOutputFormat.Controls.Add(this.radioPng);
			this.groupBoxOutputFormat.Controls.Add(this.buttonPrintPreview);
			this.groupBoxOutputFormat.Controls.Add(this.buttonPrint);
			this.groupBoxOutputFormat.Controls.Add(this.buttonShow);
			this.groupBoxOutputFormat.Controls.Add(this.radioCsv);
			this.groupBoxOutputFormat.Controls.Add(this.buttonSave);
			this.groupBoxOutputFormat.Controls.Add(this.label1);
			this.groupBoxOutputFormat.Controls.Add(this.radioTsv);
			this.groupBoxOutputFormat.Controls.Add(this.radioTables);
			this.groupBoxOutputFormat.Controls.Add(this.radioSvg);
			this.groupBoxOutputFormat.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBoxOutputFormat.Location = new System.Drawing.Point(0, 0);
			this.groupBoxOutputFormat.Name = "groupBoxOutputFormat";
			this.groupBoxOutputFormat.Size = new System.Drawing.Size(64, 480);
			this.groupBoxOutputFormat.TabIndex = 2;
			this.groupBoxOutputFormat.TabStop = false;
			this.groupBoxOutputFormat.Text = "Save As";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(9, 180);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(52, 40);
			this.label2.TabIndex = 4;
			this.label2.Text = "(best for data export)";
			// 
			// radioPng
			// 
			this.radioPng.Location = new System.Drawing.Point(6, 278);
			this.radioPng.Name = "radioPng";
			this.radioPng.Size = new System.Drawing.Size(56, 23);
			this.radioPng.TabIndex = 6;
			this.radioPng.Tag = 3;
			this.radioPng.Text = "PNG";
			this.radioPng.UseVisualStyleBackColor = true;
			this.radioPng.Click += new System.EventHandler(this.radioPng_Click);
			// 
			// buttonPrintPreview
			// 
			this.buttonPrintPreview.Location = new System.Drawing.Point(6, 445);
			this.buttonPrintPreview.Name = "buttonPrintPreview";
			this.buttonPrintPreview.Size = new System.Drawing.Size(53, 23);
			this.buttonPrintPreview.TabIndex = 10;
			this.buttonPrintPreview.Text = "Preview";
			this.buttonPrintPreview.UseVisualStyleBackColor = true;
			this.buttonPrintPreview.Click += new System.EventHandler(this.ButtonPrintPreviewClick);
			// 
			// buttonPrint
			// 
			this.buttonPrint.Location = new System.Drawing.Point(6, 416);
			this.buttonPrint.Name = "buttonPrint";
			this.buttonPrint.Size = new System.Drawing.Size(52, 23);
			this.buttonPrint.TabIndex = 9;
			this.buttonPrint.Text = "&Print";
			this.buttonPrint.UseVisualStyleBackColor = true;
			this.buttonPrint.Click += new System.EventHandler(this.ButtonPrintClick);
			// 
			// buttonShow
			// 
			this.buttonShow.Enabled = false;
			this.buttonShow.Location = new System.Drawing.Point(6, 358);
			this.buttonShow.Name = "buttonShow";
			this.buttonShow.Size = new System.Drawing.Size(52, 52);
			this.buttonShow.TabIndex = 8;
			this.buttonShow.Text = "Show saved report";
			this.buttonShow.UseVisualStyleBackColor = true;
			this.buttonShow.Click += new System.EventHandler(this.ButtonShowClick);
			// 
			// radioCsv
			// 
			this.radioCsv.Location = new System.Drawing.Point(6, 215);
			this.radioCsv.Name = "radioCsv";
			this.radioCsv.Size = new System.Drawing.Size(56, 32);
			this.radioCsv.TabIndex = 5;
			this.radioCsv.Tag = 3;
			this.radioCsv.Text = "CSV";
			this.radioCsv.UseVisualStyleBackColor = true;
			// 
			// buttonSave
			// 
			this.buttonSave.Location = new System.Drawing.Point(6, 329);
			this.buttonSave.Name = "buttonSave";
			this.buttonSave.Size = new System.Drawing.Size(52, 23);
			this.buttonSave.TabIndex = 7;
			this.buttonSave.Text = "&Save";
			this.buttonSave.UseVisualStyleBackColor = true;
			this.buttonSave.Click += new System.EventHandler(this.ButtonSaveClick);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(6, 67);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(55, 23);
			this.label1.TabIndex = 1;
			this.label1.Text = "(prettiest)";
			// 
			// radioTsv
			// 
			this.radioTsv.Location = new System.Drawing.Point(6, 158);
			this.radioTsv.Name = "radioTsv";
			this.radioTsv.Size = new System.Drawing.Size(56, 24);
			this.radioTsv.TabIndex = 3;
			this.radioTsv.Tag = 2;
			this.radioTsv.Text = "TSV";
			this.radioTsv.UseVisualStyleBackColor = true;
			// 
			// radioTables
			// 
			this.radioTables.Location = new System.Drawing.Point(6, 93);
			this.radioTables.Name = "radioTables";
			this.radioTables.Size = new System.Drawing.Size(56, 32);
			this.radioTables.TabIndex = 2;
			this.radioTables.Tag = 1;
			this.radioTables.Text = "HTML tables";
			this.radioTables.UseVisualStyleBackColor = true;
			// 
			// radioSvg
			// 
			this.radioSvg.Checked = true;
			this.radioSvg.Location = new System.Drawing.Point(6, 32);
			this.radioSvg.Name = "radioSvg";
			this.radioSvg.Size = new System.Drawing.Size(56, 32);
			this.radioSvg.TabIndex = 0;
			this.radioSvg.TabStop = true;
			this.radioSvg.Tag = 0;
			this.radioSvg.Text = "HTML SVG";
			this.radioSvg.UseVisualStyleBackColor = true;
			// 
			// numericScale
			// 
			this.numericScale.DecimalPlaces = 2;
			this.numericScale.Location = new System.Drawing.Point(6, 303);
			this.numericScale.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
			this.numericScale.Name = "numericScale";
			this.numericScale.Size = new System.Drawing.Size(52, 20);
			this.numericScale.TabIndex = 11;
			this.numericScale.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericScale.Visible = false;
			// 
			// PrintReport
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.groupBoxOutputFormat);
			this.Name = "PrintReport";
			this.Size = new System.Drawing.Size(64, 480);
			this.groupBoxOutputFormat.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.numericScale)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBoxOutputFormat;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.RadioButton radioPng;
		private System.Windows.Forms.Button buttonPrintPreview;
		private System.Windows.Forms.Button buttonPrint;
		private System.Windows.Forms.Button buttonShow;
		private System.Windows.Forms.RadioButton radioCsv;
		private System.Windows.Forms.Button buttonSave;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.RadioButton radioTsv;
		private System.Windows.Forms.RadioButton radioTables;
		private System.Windows.Forms.RadioButton radioSvg;
		private System.Windows.Forms.NumericUpDown numericScale;
	}
}
