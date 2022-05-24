using System;
using System.Windows.Forms;
using Zoom;

namespace Torn5.Controls
{
	public partial class DisplayReport : UserControl
	{
		readonly Timer RedrawTimer = new Timer();

		ZoomReport report;
		public ZoomReport Report
		{
			get { return report; }
			set
			{
				report = value;
				if (Report != null)
					Text = report.Title;

				RedrawTimerTick(null, null);
			}
		}

		public DisplayReport()
		{
			InitializeComponent();
			BackgroundImageLayout = ImageLayout.Zoom;
			RedrawTimer.Interval = 1000;
			RedrawTimer.Tick += RedrawTimerTick;
		}

		private void DisplayReportResize(object sender, EventArgs e)
		{
			RedrawTimer.Enabled = true;  // If the control has resized larger, we want to redraw at higher res. But we don't weant to redraw _lots_ of times, so only do it once per second. If control has resized smaller: meh. Redraw it anyway.
		}
		private void RedrawTimerTick(object sender, EventArgs e)
		{
			if (Report == null)
				BackgroundImage = null;
			else
				BackgroundImage = Report.ToBitmap(Width, Height);

			RedrawTimer.Enabled = false;
		}
	}
}
