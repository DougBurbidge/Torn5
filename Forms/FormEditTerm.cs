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
    public partial class FormEditTerm : Form
    {
        public TermRecord Term { get; set; }
        public League League { get; set; }
        private decimal otherTerm = 0;

        public FormEditTerm(League league)
        {
            InitializeComponent();
            League = league;
        }

        public FormEditTerm(League league, TermRecord term)
        {
            InitializeComponent();
            League = league;
            Term = term;
        }

        private void EditTerm_Load(object sender, EventArgs e)
        {
            typeSelector.DataSource = Enum.GetNames(typeof(TermType));
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            TermType.TryParse(typeSelector.Text, out TermType termType);
            Term = new TermRecord(termType, Term?.Time, (int)penalty.Value, reason.Text);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void typeSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(typeSelector.Text == TermType.Yellow.ToString())
            {
                Console.WriteLine(typeSelector.Tag);
                penalty.Value = League.YellowTermValue;
                penalty.Enabled = false;
            }
            if (typeSelector.Text == TermType.Red.ToString())
            {
                penalty.Value = League.RedTermValue;
                penalty.Enabled = false;
            }
            if (typeSelector.Text == TermType.Verbal.ToString())
            {
                penalty.Value = League.VerbalTermValue;
                penalty.Enabled = false;
            }
            if (typeSelector.Text == TermType.Other.ToString())
            {
                penalty.Value = otherTerm;
                penalty.Enabled = true;
            }
        }

        private void FormEditTerm_Shown(object sender, EventArgs e)
        {
            this.CenterToParent();
            if (Term != null)
            {
                penalty.Value = Term.Value;
                typeSelector.Text = Term.Type.ToString();
                reason.Text = Term.Reason;
                Time.Text = Term.Time != null ? Term.Time.ToString() : "Post Game Term";
            }
        }
    }
}
