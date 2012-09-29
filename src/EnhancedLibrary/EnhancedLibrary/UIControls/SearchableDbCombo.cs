using System;
using EnhancedLibrary.ExtensionMethods.Business;
using System.Linq;
using System.Collections.Generic;
using Infragistics.Win.UltraWinEditors;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.ComponentModel;
using System.Data;
using System.Drawing;

namespace EnhancedLibrary.UIControls
{
    /// <summary>
    ///     Gives to the client, a range of results that the text inserted in the control 
    ///     is contained in a specific column of a specific table for that connectionString.
    /// </summary>
    public class SearchableDbCombo : UltraComboEditor
    {
        /// <summary>
        ///     ConnectionString to database
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        ///     The name of the table in the database, where the search is done
        /// </summary>
        public string TableName { get; set; }


        /// <summary>
        ///     The name of the column of the table, where the search is done
        /// </summary>
        public string ColumnName { get; set; }

        
        /// <summary>
        ///     If enabled, dashes will be inserted automatically, eg: 11-MM-22
        /// </summary>
        public bool HasEnrollmentBehavior { get; set; }


        /// <summary>
        ///     If setted, the text will appear when the combo will be loading the data into memory
        /// </summary>
        public string LoadingText { get; set; }


        // internal cache and fields
        string[] m_cache; bool m_wasBackspace;





        #region Constructors
        

        public SearchableDbCombo()
        {
            //
            // Just for designer to work

        }





        public SearchableDbCombo(string DbConnectionString, string TableToSearch, string ColumnToSearch)
        {
            if ( string.IsNullOrEmpty(DbConnectionString) )
                throw new ArgumentNullException("DbConnectionString");

            if ( string.IsNullOrEmpty(TableToSearch) )
                throw new ArgumentNullException("TableToSearch");

            if ( string.IsNullOrEmpty(ColumnToSearch) )
                throw new ArgumentNullException("ColumnToSearch");

            ConnectionString = DbConnectionString;
            TableName = TableToSearch;
            ColumnName = ColumnToSearch;
        }






        #endregion


       

        #region Internal Properties



        IList<string> Cache
        {
            get
            {
                if ( m_cache == null )
                {
                    if ( !LoadingText.IsNE() )
                        LoadingEffect();

                    // 
                    // Search for everything 

                    var list = Search("");
                    m_cache = new string[list.Count];

                    list.CopyTo(m_cache, 0);
                }

                return m_cache;
            }
        }

        



        #endregion







        #region Events


        // Is called before of OnTextChanged event
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if ( e.KeyChar == (char) Keys.Back )
                m_wasBackspace = true;
            else
                m_wasBackspace = false;
        }



        protected override void OnTextChanged(EventArgs e)
        {
            if ( SelectedText == Text )
                return;

            CloseUp();

            if ( m_wasBackspace && Text == "" )
            {
                LoadAllData();
            }

            else
            {
                if ( HasEnrollmentBehavior && !m_wasBackspace )
                    AddDashIfNecessary();

                LoadSpecificData();
            }

            DropDown();
            Select(Text.Length, Text.Length);
        }

        
        protected override void OnBeforeDropDown(CancelEventArgs args)
        {
            if ( Text == string.Empty )
                LoadAllData();
        }

        
        



        #endregion
       








        #region Internal Methods

        
        
        
        void LoadingEffect()
        {
            Appearance.ForeColor = Color.LightGray;
            Text = LoadingText;
            DropDown();
            CloseUp();
            Text = string.Empty;
            Appearance.ForeColor = Color.Black;
        }



        /// <summary>
        ///     Loads all data into the combobox
        /// </summary>
        void LoadAllData()
        {
            DataSource = Cache;
            DataBind();
        }

        /// <summary>
        ///     Loads only a portion of the data into the combobox
        /// </summary>
        void LoadSpecificData()
        {
            DataSource = FilterCache();
            DataBind();
        }

        
        
        List<string> FilterCache()
        {
            List<string> filteredCache = new List<string>(Cache.Count);

            foreach ( string item in Cache )
            {
                if ( item.Contains(Text, StringComparison.OrdinalIgnoreCase) )
                    filteredCache.Add(item);
            }
            return filteredCache;
        }




        // This method must be highly optimized for speed
        LinkedList<string> Search(String str)
        {
            LinkedList<String> l = new LinkedList<string>();

            using ( SqlConnection conn = new SqlConnection(ConnectionString) )
            {
                SqlCommand command = conn.CreateCommand();

                command.CommandType = CommandType.Text;
                command.CommandText = string.IsNullOrEmpty(str) ? SelectAllCmd() : SelectLikeCmd(str);
                
                conn.Open();
                SqlDataReader reader = command.ExecuteReader(System.Data.CommandBehavior.CloseConnection | System.Data.CommandBehavior.SequentialAccess);

                while ( reader.Read() ) 
                {
                    l.AddLast(reader.GetString(0));
                }
            }

            return l;
        }


        string SelectLikeCmd(string Value)
        {
            return String.Format(@"exec sp_executesql N'select distinct [{0}] 
                                                               from [{1}] 
                                                               where ([{0}] is not null and [{0}] <> '''' and [{0}] like ''%{2}%'')
                                                               order by [{0}] asc'",
                                ColumnName, 
                                TableName, 
                                Value
            );
        }



        string SelectAllCmd()
        {
            return String.Format(@"exec sp_executesql N'select distinct [{0}] 
                                                               from [{1}] 
                                                               where ([{0}] is not null and [{0}] <> '''') 
                                                               order by [{0}] asc'",
                                ColumnName,
                                TableName
            );
        }



        void AddDashIfNecessary()
        {
            if ( Text.Length == 2 || textValue.Length == 5 )
                Text += "-";
        }


        #endregion

    }
}
