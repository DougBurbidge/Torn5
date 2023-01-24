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
    public partial class ManageTerms : Form
    {
        public ServerPlayer Player { get; set; }
        public ManageTerms()
        {
            InitializeComponent();
        }

        private void ManageTerms_Shown(object sender, EventArgs e)
        {
            termList.Items.Clear();
            if (Player?.TermRecords != null)
            {
                foreach (TermRecord term in Player.TermRecords)
                {
                    ListViewItem item = new ListViewItem(term.Time.ToString());
                    item.SubItems.Add(term.Type.ToString());
                    item.SubItems.Add(term.Value.ToString());
                    item.SubItems.Add(term.Reason);
                    item.Tag = term.Time;
                    termList.Items.Add(item);
                }
            }
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            new EditTerm
            {

            }.ShowDialog();
        }
    }
}
