using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EnhancedLibrary.Utilities.DataAccess;

namespace EnhancedLibrary.UI
{
    public partial class SearchableDbComboUI : Form
    {
        public SearchableDbComboUI()
        {
            InitializeComponent();

            searchableDbCombo1.ConnectionString = ConnectionStringHelper.SQL2005NotTrusted("lrtdatasqls304", "PMCCQ", "dsides9", "123456");
            searchableDbCombo1.TableName = "GVVeiculos";
            searchableDbCombo1.ColumnName = "Matricula";
            searchableDbCombo1.HasEnrollmentBehavior = true;
        }
    }
}
