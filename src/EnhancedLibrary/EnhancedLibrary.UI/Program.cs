using System;
using EnhancedLibrary.ExtensionMethods.Business;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace EnhancedLibrary.UI
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new SearchableDbComboUI());
            }

            catch ( Exception ex )
            {
                string superMsg = ex.PrepareMessage();
                MessageBox.Show(superMsg);
            }
        }
    }
}
