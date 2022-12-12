using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Torn;

namespace Torn5.Controls
{
    public partial class PointPercentEditor : UserControl
    {
        [Browsable(true)]
        public decimal Points { get => points.Value; set => points.Value = value; }

        [Browsable(true)]
        public decimal Percentage { get => percentage.Value; set => percentage.Value = value; }

        [Browsable(false)]
        public PointPercent pointPercent
        {
            get => new PointPercent(points.Value, percentage.Value);

            set
            {
                points.Value = value.Points;
                percentage.Value = value.Percent;
            }
        }

        public PointPercentEditor()
        {
            InitializeComponent();
        }

        public bool HasValue()
        {
            return Percentage != 0;
        }

        [Browsable(true)]
        [Category("Action")]
        public event EventHandler ValueChanged;

        private void OnChanged(object sender, EventArgs e)
        {
            ValueChanged?.Invoke(this, e);
        }
    }
}
