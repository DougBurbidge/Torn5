using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Torn;

namespace Torn5.Controls
{
	public partial class GradeEditor : UserControl
	{
		[Browsable(true)]
		public string GradeName { get => textName.Text; set => textName.Text = value; }

		[Browsable(true)]
		public decimal Points { get => numericPoints.Value; set => numericPoints.Value = value; }

		[Browsable(true)]
		public bool HasPenalty { get => checkPenalty.Checked; set => checkPenalty.Checked = value; }

		[Browsable(true)]
		public bool HasBonus { get => checkBonus.Checked; set => checkBonus.Checked = value; }

		[Browsable(false)]
		public Grade Grade
		{ 
			get => new Grade(textName.Text, numericPoints.Value, checkPenalty.Checked, checkBonus.Checked);

			set
			{
				textName.Text = value.Name;
				numericPoints.Value = value.Points;
				checkPenalty.Checked = value.HasPenalty;
				checkBonus.Checked = value.HasBonus;
			}
		}

		public GradeEditor()
		{
			InitializeComponent();
		}

		public bool HasValue()
		{
			return !string.IsNullOrEmpty(GradeName) || Points != 0;
		}

		[Browsable(true)]
		[Category("Action")]
		public event EventHandler ValueChanged;

		private void OnChanged(object sender, EventArgs e)
		{
			ValueChanged?.Invoke(this, e);
		}

		private void CheckPenaltyCheckedChanged(object sender, EventArgs e)
		{
			if (checkPenalty.Checked)
				checkBonus.Checked = false;

			ValueChanged?.Invoke(this, e);
		}

		private void CheckBonusCheckedChanged(object sender, EventArgs e)
		{
			if (checkBonus.Checked)
				checkPenalty.Checked = false;

			ValueChanged?.Invoke(this, e);
		}
	}
}
