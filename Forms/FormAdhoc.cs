using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Svg;
using Zoom;

namespace Torn.UI
{
	public partial class FormAdhoc : Form
	{
		SvgDocument document;
		ZoomReport report;
		double aspectRatio = 1.0;

		public ZoomReport Report
		{
			get { return report; }
			set
			{
				report = value;
				using (StringWriter sw = new StringWriter())
				{
					sw.Write(report.ToSvg(true));
					document = SvgDocument.FromSvg<SvgDocument>(sw.ToString());
				}
				Text = report.Title;
				aspectRatio = document.ViewBox.Width / document.ViewBox.Height;

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
			document.Width = new SvgUnit(SvgUnitType.Pixel, panelDisplay.Width);
			document.Height = new SvgUnit(SvgUnitType.Pixel, (int)(panelDisplay.Width / aspectRatio));
			panelDisplay.BackgroundImage = document.Draw();
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
	}
}
