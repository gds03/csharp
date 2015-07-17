using CodeToUMLNotation.NRefactoryHelper;
using ICSharpCode.NRefactory.CSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodeToUMLNotation
{
    public partial class ParserForm : Form
    {
        private readonly NRefactoryCom m_comunicator;


        public ParserForm()
        {
            InitializeComponent();
            m_comunicator = new NRefactoryCom(this);
        }

        private void btnAll_Click(object sender, EventArgs e)
        {
            Converted.Text = m_comunicator.Parse(PARSE_TYPE.ALL, ToConvert.Text);
        }

        private void btnFields_Click(object sender, EventArgs e)
        {
            Converted.Text = m_comunicator.Parse(PARSE_TYPE.FIELDS, ToConvert.Text);

            if (Converted.Text == "" || Converted.Text == "\n")
                MessageBox.Show("No fields available in source code");
        }

        private void btnProperties_Click(object sender, EventArgs e)
        {
            Converted.Text = m_comunicator.Parse(PARSE_TYPE.PROPERTIES, ToConvert.Text);

            if (Converted.Text == "" || Converted.Text == "\n")            
                MessageBox.Show("No properties available in source code");
        }

        private void btnMethods_Click(object sender, EventArgs e)
        {
            Converted.Text = m_comunicator.Parse(PARSE_TYPE.METHODS, ToConvert.Text);

            if (Converted.Text == "" || Converted.Text == "\n")
                MessageBox.Show("No methods available in source code");
        }





        
    }
}
