using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Zoom;

namespace Torn.UI
{
	public partial class FormAdhoc : Form
	{
		ZoomReport report;
		public ZoomReport Report
		{
			get { return report; }
			set
			{
				report = value;
				Text = report.Title;
				TimerRedrawTick(null, null);
			}
		}

		public FormAdhoc()
		{
			InitializeComponent();
		}

		private void FormAdhoc_Resize(object sender, System.EventArgs e)
		{
			timerRedraw.Enabled = true;  // If the window has resized larger, we want to redraw at higher res. But we don't weant to redraw _lots_ of times, so only do it once per second. If window has resized smaller: meh. Redraw it anyway.
		}

		private void TimerRedrawTick(object sender, EventArgs e)
		{
			panelDisplay.BackgroundImage = report.ToBitmap(panelDisplay.Width, panelDisplay.Height);
			timerRedraw.Enabled = false;
		}

		string fileName;
		private void ButtonSaveClick(object sender, EventArgs e)
		{
			var outputFormat = radioSvg.Checked ? OutputFormat.Svg :
				radioTables.Checked ? OutputFormat.HtmlTable :
				radioTsv.Checked ? OutputFormat.Tsv :
				OutputFormat.Csv;

			string file = report.Title.Replace('/', '-').Replace(' ', '_') + "." + outputFormat.ToExtension();  // Replace / with - so dates still look OK, and  space with _ to make URLs easier if this file is uploaded to the web.
			saveFileDialog.FileName = Path.GetInvalidFileNameChars().Aggregate(file, (current, c) => current.Replace(c, '_'));  // Replace all other invalid chars with _.

			var reports = new ZoomReports()
			{
				report 
			};

			if (saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				using (StreamWriter sw = File.CreateText(saveFileDialog.FileName))
					sw.Write(reports.ToOutput(outputFormat));
				fileName = saveFileDialog.FileName;
				buttonShow.Enabled = true;
			}
		}

		private void ButtonShowClick(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start(fileName);
		}

		private void ButtonPrintClick(object sender, EventArgs e)
		{
			var pd = report.ToPrint();
			if (printDialog.ShowDialog() == DialogResult.OK)
				pd.Print();
		}

		private void ButtonPrintPreviewClick(object sender, EventArgs e)
		{
			printPreviewDialog.Document = report.ToPrint();
			printPreviewDialog.ShowDialog();
		}

		private void ButtonRerenderClick(object sender, EventArgs e)
		{
			Cursor.Current = Cursors.WaitCursor;
			try
			{
				panelDisplay.BackgroundImage = report.ToBitmap(panelDisplay.Width, panelDisplay.Height, true);
			}
			finally
			{
				Cursor.Current = Cursors.Default;
			}
		}
	}
}
