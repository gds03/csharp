//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace CustomComponents.WinForms.UIControls.Infragistics
//{
//   /// <summary>
//    ///     Gives to the client, a range of results that the text inserted in the control 
//    ///     is contained in a specific column of a specific table for that connectionString.
//    /// </summary>
//    public class SearchableDbCombo : UltraComboEditor
//    {
//        /// <summary>
//        ///     The connectionString to the database where data came from
//        /// </summary>
//        [
//            Category("Options"),
//            Description("The connectionString to the database where data came from")
//        ]
//        public string ConnectionString { get; set; }

//        /// <summary>
//        ///     The name of the table where the control will search.
//        /// </summary>
//        [
//            Category("Options"),
//            Description("The name of the table where the control will search.")
//        ]
//        public string TableName { get; set; }


//        /// <summary>
//        ///     The name of the column within the table, where the control will search.
//        /// </summary>
//        [
//            Category("Options"),
//            Description("The name of the column within the table, where the control will search.")
//        ]
//        public string ColumnName { get; set; }


//        /// <summary>
//        ///     If enabled, dashes will be inserted automatically while you type an enrollment, eg: 11-MM-22
//        /// </summary>
//        [
//            Category("Options"),
//            Description("If enabled, dashes will be inserted automatically while you type an enrollment, eg: 11-MM-22")
//        ]
//        public bool HasEnrollmentBehavior { get; set; }


//        /// <summary>
//        ///     If enabled, the text will appear when the combo does the first attemp to load data into memory.
//        /// </summary>
//        [
//            Category("Options"),
//            Description("If enabled, the text will appear when the combo does the first attemp to load data into memory.")
//        ]
//        public string LoadingText { get; set; }


//        /// <summary>
//        ///     If enabled, the control search in physical memory for the text. Otherwise search on database.
//        /// </summary>
//        [
//            Category("Options"),
//            Description("If enabled, the control search in physical memory for the text. Otherwise search on database.")
//        ]
//        public bool LocalSearch { get; set; }





//        // internal cache and fields
//        string[] m_cache; bool m_wasBackspace;





//        #region Constructors


//        public SearchableDbCombo()
//        {
//            //
//            // Just for designer to work

//        }





//        public SearchableDbCombo(string DbConnectionString, string TableToSearch, string ColumnToSearch)
//        {
//            if (string.IsNullOrEmpty(DbConnectionString))
//                throw new ArgumentNullException("DbConnectionString");

//            if (string.IsNullOrEmpty(TableToSearch))
//                throw new ArgumentNullException("TableToSearch");

//            if (string.IsNullOrEmpty(ColumnToSearch))
//                throw new ArgumentNullException("ColumnToSearch");

//            ConnectionString = DbConnectionString;
//            TableName = TableToSearch;
//            ColumnName = ColumnToSearch;
//        }






//        #endregion








//        #region Internal Properties



//        IList<string> Cache
//        {
//            get
//            {
//                if (m_cache == null)
//                {
//                    if (!LoadingText.IsNE())
//                        LoadingEffect();

//                    // 
//                    // Search for everything and save in memory

//                    var list = Search("");
//                    m_cache = new string[list.Count];

//                    list.CopyTo(m_cache, 0);
//                }

//                return m_cache;
//            }
//        }


//        IList<string> Db
//        {
//            get
//            {
//                return Search("").ToArray();
//            }
//        }





//        #endregion







//        #region Events


//        // Is called before of OnTextChanged event
//        protected override void OnKeyPress(KeyPressEventArgs e)
//        {
//            if (e.KeyChar == (char)Keys.Back)
//                m_wasBackspace = true;
//            else
//                m_wasBackspace = false;

//            base.OnKeyPress(e);
//        }



//        protected override void OnTextChanged(EventArgs e)
//        {
//            if (SelectedText == Text)
//                return;

//            CloseUp();

//            if (m_wasBackspace && Text == "")
//            {
//                LoadAllData();
//            }

//            else
//            {
//                if (HasEnrollmentBehavior && !m_wasBackspace)
//                    AddDashIfNecessary();

//                LoadSpecificData();
//            }

//            DropDown();
//            Select(Text.Length, Text.Length);

//            base.OnTextChanged(e);
//        }


//        protected override void OnBeforeDropDown(CancelEventArgs args)
//        {
//            if (Text == string.Empty)
//                LoadAllData();

//            base.OnBeforeDropDown(args);
//        }






//        #endregion









//        #region Internal Methods




//        void LoadingEffect()
//        {
//            Appearance.ForeColor = Color.LightGray;
//            Text = LoadingText;
//            DropDown();
//            CloseUp();
//            Text = string.Empty;
//            Appearance.ForeColor = Color.Black;
//        }



//        /// <summary>
//        ///     Loads all data into the combobox
//        /// </summary>
//        void LoadAllData()
//        {
//            DataSource = (LocalSearch) ? Cache : Db;
//            DataBind();
//        }

//        /// <summary>
//        ///     Loads only a portion of the data into the combobox
//        /// </summary>
//        void LoadSpecificData()
//        {
//            DataSource = (LocalSearch) ? FilterCache() : FilterDb();
//            DataBind();
//        }



//        List<string> FilterCache()
//        {
//            List<string> filteredCache = new List<string>(Cache.Count);

//            foreach (string item in Cache)
//            {
//                if (item.Contains(Text, StringComparison.OrdinalIgnoreCase))
//                    filteredCache.Add(item);
//            }
//            return filteredCache;
//        }


//        List<string> FilterDb()
//        {
//            return Search(Text).ToList();
//        }




//        // This method must be highly optimized for speed
//        LinkedList<string> Search(String str)
//        {
//            LinkedList<String> l = new LinkedList<string>();

//            using (SqlConnection conn = new SqlConnection(ConnectionString))
//            {
//                SqlCommand command = conn.CreateCommand();

//                command.CommandType = CommandType.Text;
//                command.CommandText = string.IsNullOrEmpty(str) ? SelectAllCmd() : SelectLikeCmd(str);

//                conn.Open();

//                using (SqlDataReader reader = command.ExecuteReader(System.Data.CommandBehavior.CloseConnection | System.Data.CommandBehavior.SequentialAccess))
//                {
//                    while (reader.Read())
//                    {
//                        l.AddLast(reader.GetString(0));
//                    }
//                }
//            }

//            return l;
//        }


//        string SelectLikeCmd(string Value)
//        {
//            return String.Format(@"exec sp_executesql N'select distinct [{0}] 
//                                                               from [{1}] 
//                                                               where ([{0}] is not null and [{0}] <> '''' and [{0}] like ''%{2}%'')
//                                                               order by [{0}] asc'",
//                                ColumnName,
//                                TableName,
//                                Value
//            );
//        }



//        string SelectAllCmd()
//        {
//            return String.Format(@"exec sp_executesql N'select distinct [{0}] 
//                                                               from [{1}] 
//                                                               where ([{0}] is not null and [{0}] <> '''') 
//                                                               order by [{0}] asc'",
//                                ColumnName,
//                                TableName
//            );
//        }



//        void AddDashIfNecessary()
//        {
//            if (Text.Length == 2 || textValue.Length == 5)
//                Text += "-";
//        }


//        #endregion
//    }
//}
