using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodeToUMLNotation.Controls
{
    public class RicherTextbox : RichTextBox
    {
        // fields
        private FindDialog m_withEventsField_findDialog;

        private int m_foundIndex;
        private string m_foundWord;

        // props
        private FindDialog findDialog
        {
            get { return m_withEventsField_findDialog; }
            set
            {
                if (m_withEventsField_findDialog != null)
                {
                    m_withEventsField_findDialog.Find -= findDialog_Find;
                }
                m_withEventsField_findDialog = value;
                if (m_withEventsField_findDialog != null)
                {
                    m_withEventsField_findDialog.Find += findDialog_Find;
                }
            }
        }

        // events
        private void RicherTextbox_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control & e.KeyCode == Keys.F)
            {
                if (findDialog == null)
                    findDialog = new FindDialog();

                this.HideSelection = false;

                // set the text selected when open
                findDialog.inputTextbox.Select(0, findDialog.inputTextbox.Text.Length);

                // show
                findDialog.ShowDialog();
            }
        }

        // bounded to Find event when the FindDialog is created.
        private void findDialog_Find(string findWhat, RichTextBoxFinds findOption)
        {
            int findIndex = 0;
            if (findWhat.Equals(m_foundWord))
                findIndex = m_foundIndex;
            if ( (findOption & RichTextBoxFinds.Reverse) == RichTextBoxFinds.Reverse)
            {
                findIndex = this.Find(findWhat, 0, findIndex, findOption);
            }
            else
            {
                findIndex = this.Find(findWhat, findIndex, findOption);
            }
            if (findIndex > 0)
            {
                m_foundWord = findWhat;
                if ( (findOption & RichTextBoxFinds.Reverse) == RichTextBoxFinds.Reverse)
                {
                    m_foundIndex = findIndex;
                }
                else
                {
                    m_foundIndex = findIndex + findWhat.Length;
                }
            }
        }

        public RicherTextbox()
        {
            KeyUp += RicherTextbox_KeyUp;
        }
    }
}
