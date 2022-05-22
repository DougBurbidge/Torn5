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
				printReport.Report = value;
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
			printReport.Image = panelDisplay.BackgroundImage;
			timerRedraw.Enabled = false;
		}

		private void ButtonRerenderClick(object sender, EventArgs e)
		{
			Cursor.Current = Cursors.WaitCursor;
			try
			{
				panelDisplay.BackgroundImage = report.ToBitmap(panelDisplay.Width, panelDisplay.Height, true);
				printReport.Image = panelDisplay.BackgroundImage;
			}
			finally
			{
				Cursor.Current = Cursors.Default;
			}
		}
	}
}
