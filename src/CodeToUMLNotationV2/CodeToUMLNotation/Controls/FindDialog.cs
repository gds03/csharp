using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodeToUMLNotation.Controls
{
    
    partial class FindDialog : Form
    {
        public event FindEventHandler Find;
        public delegate void FindEventHandler(string findWhat, RichTextBoxFinds findOption);

        private void btnCancel_Click(System.Object sender, System.EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        /// <summary>
        ///     If any text set Find button enabled.
        /// </summary>
        private void txtFindWhat_TextChanged(System.Object sender, System.EventArgs e)
        {
            if (txtFindWhat.TextLength > 0)
            {
                btnFind.Enabled = true;
            }
            else
            {
                btnFind.Enabled = false;
            }
        }


        /// <summary>
        ///     When click, see which filter is applied and call the event handler
        /// </summary>
        private void btnFind_Click(System.Object sender, System.EventArgs e)
        {
            if (txtFindWhat.TextLength > 0)
            {
                RichTextBoxFinds findOption = RichTextBoxFinds.None;

                if (chkMatchCase.Checked)
                    findOption = RichTextBoxFinds.MatchCase;

                if (chkWholeWord.Checked)
                    findOption = (findOption | RichTextBoxFinds.WholeWord);

                if (optUp.Checked)
                    findOption = (findOption | RichTextBoxFinds.Reverse);

                if (Find != null)
                {
                    Find(txtFindWhat.Text, findOption);
                }
            }
        }



        public TextBox inputTextbox { get { return txtFindWhat; } }

        public FindDialog()
        {
            InitializeComponent();
        }
    }

}
