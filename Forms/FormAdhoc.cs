using System;
using System.IO;
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
			document.Width = new SvgUnit(SvgUnitType.Pixel, Width);
			document.Height = new SvgUnit(SvgUnitType.Pixel, (int)(Width / aspectRatio));
			BackgroundImage = document.Draw();
			timerRedraw.Enabled = false;
		}
	}
}
