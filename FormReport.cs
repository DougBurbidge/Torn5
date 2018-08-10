using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Torn.Report;

//TODO: make form less tall.
//TODO: make date pickers also capable of picking time.

namespace Torn.UI
{
	/// <summary>
	/// Allow user to create or edit a report template.
	/// </summary>
	public partial class FormReport : Form
	{
		public ReportTemplate ReportTemplate { get; set; }
		public DateTime From { set { datePickerFrom.Value = value; } get { return datePickerFrom.Value; } }
		public DateTime To { set { datePickerTo.Value = value; } get { return datePickerTo.Value; } }
		
		public FormReport()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			timePickerFrom.CustomFormat = CultureInfo.CurrentUICulture.DateTimeFormat.ShortTimePattern;
			timePickerTo.CustomFormat = CultureInfo.CurrentUICulture.DateTimeFormat.ShortTimePattern;
		}
		
		void FormReportShown(object sender, EventArgs e)
		{
			listBoxReportType.Focus();
			
			if (ReportTemplate != null)
			{
				listBoxReportType.SelectedIndex = (int)ReportTemplate.ReportType - 1;

				foreach (Control c in this.Controls)
					if (c is CheckBox && c.Tag != null)
						((CheckBox)c).Checked = ReportTemplate.Settings.Contains((string)c.Tag);

				radioButtonGames.Checked = ReportTemplate.Drops == null ? false : ReportTemplate.Drops.CountBest > 0 || ReportTemplate.Drops.CountWorst > 0;
				radioButtonPercent.Checked = ReportTemplate.Drops == null ? false : ReportTemplate.Drops.PercentBest > 0 || ReportTemplate.Drops.PercentWorst > 0;
				numericUpDownBest.Value = ReportTemplate.Drops == null ? 0 : (Decimal)Math.Max(ReportTemplate.Drops.CountBest, ReportTemplate.Drops.PercentBest);
				numericUpDownWorst.Value = ReportTemplate.Drops == null ? 0 : (Decimal)Math.Max(ReportTemplate.Drops.CountWorst, ReportTemplate.Drops.PercentWorst);

				if (ReportTemplate.From != null && (DateTime)ReportTemplate.From >= datePickerFrom.MinDate && (DateTime)ReportTemplate.From <= datePickerFrom.MaxDate)
					datePickerFrom.Value = (DateTime)ReportTemplate.From;
				if (ReportTemplate.To != null && (DateTime)ReportTemplate.To >= datePickerTo.MinDate && (DateTime)ReportTemplate.To <= datePickerTo.MaxDate)
					datePickerTo.Value = (DateTime)ReportTemplate.To;

				string s = ReportTemplate.Settings.Find(x => x.StartsWith("TopN=", StringComparison.OrdinalIgnoreCase));
				bool b = !string.IsNullOrEmpty(s);
				showTopN.Checked = b;
				numericUpDownTopN.Enabled = b;
				numericUpDownTopN.Value = b ? int.Parse(s.Substring(s.IndexOf('=')+1), CultureInfo.InvariantCulture) : 0;

				s = ReportTemplate.Settings.Find(x => x.StartsWith("AtLeastN=", StringComparison.OrdinalIgnoreCase));
				b = !string.IsNullOrEmpty(s);
				atLeastN.Checked = b;
				numericUpDownAtLeastN.Enabled = b;
				numericUpDownAtLeastN.Value = b ? int.Parse(s.Substring(s.IndexOf('=')+1), CultureInfo.InvariantCulture) : 0;
			}

//			ListBoxReportTypeSelectedIndexChanged(null, null);
		}

		void FormReportFormClosed(object sender, FormClosedEventArgs e)
		{
			if (this.DialogResult == DialogResult.OK)
			{
				if (ReportTemplate == null)
					ReportTemplate = new ReportTemplate();

				ReportTemplate.ReportType = (ReportType)(listBoxReportType.SelectedIndex + 1);

				ReportTemplate.Settings.Clear();
				foreach (Control c in this.Controls)
					if (c is CheckBox && c.Tag != null && c.Enabled && ((CheckBox)c).Checked)
						ReportTemplate.Settings.Add((string)c.Tag);

				if (dropGames.Checked)
				{
					if (ReportTemplate.Drops == null)
						ReportTemplate.Drops = new Drops();

					if (radioButtonGames.Checked)
					{
						ReportTemplate.Drops.CountBest = (int)numericUpDownBest.Value;
						ReportTemplate.Drops.CountWorst = (int)numericUpDownWorst.Value;
					}
					else if (radioButtonPercent.Checked)
					{
						ReportTemplate.Drops.PercentBest = (double)numericUpDownBest.Value;
						ReportTemplate.Drops.PercentWorst = (double)numericUpDownWorst.Value;
					}
				}

				if (showTopN.Checked)
					ReportTemplate.Settings.Add("ShowTopN=" + numericUpDownTopN.Value.ToString(CultureInfo.InvariantCulture));

				if (atLeastN.Checked)
					ReportTemplate.Settings.Add("AtLeastN=" + numericUpDownAtLeastN.Value.ToString(CultureInfo.InvariantCulture));
				
				ReportTemplate.From = dateRange.Checked ? datePickerFrom.Value : (DateTime?)null;
				ReportTemplate.To = dateRange.Checked ? datePickerTo.Value : (DateTime?)null;
			}
		}
		
		void ListBoxReportTypeSelectedIndexChanged(object sender, EventArgs e)
		{
			int i = listBoxReportType.SelectedIndex;

			scaleGames.Enabled = i == 0;
			dropGames.Enabled = i == 0 || i == 2 || i == 4;
			dateRange.Enabled = true;
			showColours.Enabled = i == 0;
			showPoints.Enabled = i == 1;
			showComments.Enabled = i == 2;
			scattergram.Enabled = i == 2;
			showTopN.Enabled = i == 0 || i == 2;
			numericUpDownTopN.Enabled = i == 0 || i == 2;
			labelTopWhat.Enabled = i == 0 || i == 2;
			atLeastN.Enabled = i == 0 || i == 2;
			numericUpDownAtLeastN.Enabled = i == 0 || i == 2;
			labelAtLeastGames.Enabled = i == 0 || i == 2;
			orderBy.Enabled = i == 0 || i == 2;
			labelOrderBy.Enabled = i == 0 || i == 2;
			description.Enabled = true;

			labelTopWhat.Text = i == 2 ? "players" : "teams";
		}
		
		void DateRangeCheckedChanged(object sender, EventArgs e)
		{
			groupBoxDateRange.Enabled = dateRange.Checked;
		}
		
		void DropGamesCheckedChanged(object sender, EventArgs e)
		{
			groupBoxDrops.Enabled = dropGames.Checked;
		}
	}
}
