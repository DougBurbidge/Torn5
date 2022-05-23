using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Zoom;

namespace Torn.UI
{
	public partial class FormAdhoc : Form
	{
		public ZoomReport Report
		{
			get { return displayReport.Report; }
			set
			{
				printReport.Report = value;
				displayReport.Report = value;
				if (value != null)
					Text = value.Title;
			}
		}

		public FormAdhoc()
		{
			InitializeComponent();
		}

		private void ButtonRerenderClick(object sender, EventArgs e)
		{
			Cursor.Current = Cursors.WaitCursor;
			try
			{
				displayReport.BackgroundImage = displayReport.Report.ToBitmap(displayReport.Width, displayReport.Height, true);
				printReport.Image = displayReport.BackgroundImage;
			}
			finally
			{
				Cursor.Current = Cursors.Default;
			}
		}
	}
}
