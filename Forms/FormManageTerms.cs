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
    public partial class FormManageTerms : Form
    {
        public ServerPlayer Player { get; set; }
        public League League { get; set; }
        private int initialPenalties = 0;
        public FormManageTerms()
        {
            InitializeComponent();
        }

        private void AddTermToList(TermRecord term)
        {
            ListViewItem item = new ListViewItem(term.Time == null ? "PostGame" : term.Time.ToString()) ;
            item.SubItems.Add(term.Type.ToString());
            item.SubItems.Add(term.Value.ToString());
            item.SubItems.Add(term.Reason);
            item.Tag = term;
            termList.Items.Add(item);
        }

        private void UpdateTermToList(TermRecord term, int index)
        {
            ListViewItem item = new ListViewItem(term.Time == null ? "PostGame" : term.Time.ToString());
            item.SubItems.Add(term.Type.ToString());
            item.SubItems.Add(term.Value.ToString());
            item.SubItems.Add(term.Reason);
            item.Tag = term;
            termList.Items[index] = item;
        }

        private void ManageTerms_Shown(object sender, EventArgs e)
        {
            this.CenterToParent();
            termList.Items.Clear();
            if (Player?.TermRecords != null)
            {
                foreach (TermRecord term in Player.TermRecords)
                {
                    AddTermToList(term);
                    initialPenalties += term.Value;
                }
            }
            Console.WriteLine("Initial Penalties: " + initialPenalties);
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            using (var form = new FormEditTerm(League))
            {
                var result = form.ShowDialog();
                if(result == DialogResult.OK)
                {
                    AddTermToList(form.Term);
                }
            }
        }

        private void termList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (termList.SelectedItems.Count == 1)
            {
                editButton.Enabled = true;
                removeButton.Enabled = true;
            }
            else
            {
                editButton.Enabled = false;
                removeButton.Enabled = false;
            }
        }

        private void editTerm()
        {
            using (var form = new FormEditTerm(League,(TermRecord)termList.SelectedItems[0].Tag))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    int index = termList.SelectedItems[0].Index;
                    UpdateTermToList(form.Term, index);
                }
            }
        }

        private void termList_DoubleClick(object sender, EventArgs e)
        {
            editTerm();
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            editTerm();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            List<TermRecord> terms = new List<TermRecord>();
            int totalPenalties = 0;
            int yellowTerms = 0;
            int redTerms = 0;
            foreach(ListViewItem item in termList.Items)
            {
                TermRecord term = (TermRecord)item.Tag;
                terms.Add(term);
                totalPenalties += term.Value;
                if (term.Type == TermType.Yellow)
                    yellowTerms++;
                if (term.Type == TermType.Red)
                    redTerms++;
            }
            Player.TermRecords = terms;
            Player.Score = Player.Score - initialPenalties + totalPenalties;
            Player.RedCards = redTerms;
            Player.YellowCards = yellowTerms;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            termList.SelectedItems[0].Remove();
        }
    }
}
