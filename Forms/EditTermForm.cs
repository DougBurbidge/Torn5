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

namespace Torn5.Forms
{
    public partial class EditTermForm : Form
    {
        public TermRecord Term { get; set; }

        public EditTermForm()
        {
            InitializeComponent();
        }

        private void EditTerm_Load(object sender, EventArgs e)
        {
            typeSelector.DataSource = Enum.GetNames(typeof(TermType));
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
