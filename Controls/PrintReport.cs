using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using Zoom;

namespace Torn5.Controls
{
	/// <summary>
	/// UI to print and/or save a report template.
	/// </summary>
	/// 
	public partial class PrintReport : UserControl
	{
		[Browsable(true)]
		public DisplayReport DisplayReport { get; set; }
		[Browsable(true)]
		public DisplayReport DisplayReport2 { get; set; }

		readonly SaveFileDialog saveFileDialog = new SaveFileDialog();
		readonly PrintDialog printDialog = new PrintDialog();
		readonly PrintPreviewDialog printPreviewDialog = new PrintPreviewDialog();

		public PrintReport()
		{
			InitializeComponent();
		}

		string fileName;
		private void ButtonSaveClick(object sender, EventArgs e)
		{
			var outputFormat = radioSvg.Checked ? OutputFormat.Svg :
				radioTables.Checked ? OutputFormat.HtmlTable :
				radioTsv.Checked ? OutputFormat.Tsv :
				radioCsv.Checked ? OutputFormat.Csv :
				OutputFormat.Png;

			string file = DisplayReport.Report.Title.Replace('/', '-').Replace(' ', '_') + "." + outputFormat.ToExtension();  // Replace / with - so dates still look OK, and  space with _ to make URLs easier if this file is uploaded to the web.
			saveFileDialog.FileName = Path.GetInvalidFileNameChars().Aggregate(file, (current, c) => current.Replace(c, '_'));  // Replace all other invalid chars with _.

			if (saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				if (radioPng.Checked)
				{
					if (checkBoxScale.Checked)
						DisplayReport.Report.ToBitmap((float)numericScale.Value).Save(saveFileDialog.FileName);
					else
						DisplayReport.BackgroundImage.Save(saveFileDialog.FileName);
				}
				else
				{
					var reports = new ZoomReports() { DisplayReport.Report };

					using (StreamWriter sw = File.CreateText(saveFileDialog.FileName))
						sw.Write(reports.ToOutput(outputFormat));
				}
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
			var pd = DisplayReport.Report.ToPrint();
			if (printDialog.ShowDialog() == DialogResult.OK)
				pd.Print();
		}

		private void ButtonPrintPreviewClick(object sender, EventArgs e)
		{
			printPreviewDialog.Document = DisplayReport.Report.ToPrint();
			printPreviewDialog.ShowDialog();
		}

		private void CheckedChanged(object sender, EventArgs e)
		{
			checkBoxScale.Enabled = radioPng.Checked;
			numericScale.Enabled = radioPng.Checked && checkBoxScale.Checked;
		}

		private void ShowOnTBoardClicked(object sender, EventArgs e)
        {
			int TBOARD_SOCKET = 21570;

			UdpClient udp = new UdpClient();
			IPEndPoint groupEP = new IPEndPoint(IPAddress.Parse("255.255.255.255"), TBOARD_SOCKET);

			string result = DisplayReport.Report.ToTBoard();
			Console.WriteLine(result);

			if(DisplayReport2 != null)
            {
				string cleanedStr = result.Replace("EOSEOS", "");
				string result2 = DisplayReport2.Report.ToTBoard();
				string cleanedStr2 = result2.Replace("DISPLAYREPORTS,", "");
				result = cleanedStr + cleanedStr2;
			}

			List<string> strs = result.Split(510).ToList();

			foreach(string str in strs)
            {
				string index = strs.IndexOf(str).ToString().PadLeft(2, '0');
				string chunk = index + str + "\x00";
				byte[] sendBytes = Encoding.ASCII.GetBytes(chunk);
				udp.Send(sendBytes, sendBytes.Length, groupEP);
			}

			string emptyIndex = strs.Count().ToString().PadLeft(2, '0');
			byte[] sendBytesEnd = Encoding.ASCII.GetBytes(emptyIndex + "\x00");
			udp.Send(sendBytesEnd, sendBytesEnd.Length, groupEP);
		}
    }
	public static class Extensions
	{
		public static IEnumerable<string> Split(this string str, int n)
		{
			if (String.IsNullOrEmpty(str) || n < 1)
			{
				throw new ArgumentException();
			}

			for (int i = 0; i < str.Length; i += n)
			{
				if (str.Length - i > n)
				{
					yield return str.Substring(i, n);
				}
				else
				{
					yield return str.Substring(i, str.Length - i);
				}

			}
		}
	}
}
