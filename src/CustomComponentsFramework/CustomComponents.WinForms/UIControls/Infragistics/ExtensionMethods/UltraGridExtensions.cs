//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace CustomComponents.WinForms.UIControls.Infragistics.ExtensionMethods
//{
//    public static class UltraGridExtensions
//    {
//        /// <summary>
//        ///     Binds a list of TItems to the grid.
//        /// </summary>
//        public static void BindGrid<TItems>(this UltraGrid grid, IList<TItems> items) 
//        {
//            if ( items == null || items.Count == 0 ) {
//                grid.DataSource = new List<TItems>();
//            }
//            else {
//                grid.DataSource = items;
//            }

//            grid.DataBind();
//        }


//        /// <summary>
//        ///     Binds a list of TItems to the grid.
//        /// </summary>
//        public static void SetDataBinding<TItems>(this UltraGrid grid, IList<TItems> items)
//        {
//            grid.SetDataBinding(items, "", true, true);
//        }

//        /// <summary>
//        ///     Activate the active row again.
//        ///     This method can be util if you have an event that fires when an active row becames active.
//        /// </summary>
//        public static void ActiveRowRefresh(this UltraGrid grid)
//        {
//            var bk = grid.ActiveRow;

//            grid.ActiveRow = null;
//            grid.ActiveRow = bk;

//            grid.ActiveRow.Activate();
//        }


//        /// <summary>
//        ///     Returns the selected object that are selected in the grid
//        /// </summary>
//        public static T GetSelectedObject<T>(this UltraGrid grid)
//        {
//            if (grid.ActiveRow == null)
//                return default(T);

//            return (T) grid.ActiveRow.ListObject;
//        }




//        /// <summary>
//        ///     Clears the content of the current grid
//        /// </summary>
//        /// <param name="grid"></param>
//        public static void Clear<T>(this UltraGrid grid)
//        {
//            grid.SetDataBinding(new List<T>(), "", true, true);
//        }









//        /// <summary>
//        /// Exports an UltraGrid data to an Excel document
//        /// </summary>
//        /// <param name="grid">UltraGrid</param>
//        /// <param name="excelFilePath">excel document full file path</param>
//        /// <param name="displayHiddenColumns">set if hidden columns will be included in the excel file</param>
//        public static void ExportExcel(this UltraGrid grid, string excelFilePath, bool displayHiddenColumns)
//        {
//            if ( File.Exists(excelFilePath) )
//            {
//                try { File.Delete(excelFilePath); }
//                catch ( Exception ) { throw new IOException(); }
//            }

//            if ( grid == null )
//                throw new ArgumentNullException("grid");


//            if ( excelFilePath.IsNE() )
//                throw new ArgumentNullException("excelFilePath");


//            //
//            // Create Application instance

//            Microsoft.Office.Interop.Excel.Application appEx = null;
//            Workbook wb = null;
//            Worksheet ws = null;


//            try
//            {
//                appEx = new Microsoft.Office.Interop.Excel.Application();
//                wb = appEx.AddWorkBook();
//                ws = wb.GetWorkSheet(1);

//                // Header
//                WriteHeaderToExcel(grid, ws, displayHiddenColumns);

//                // Body
//                WriteBodyToExcel(grid, ws, displayHiddenColumns);


//                // Persist data in the file
//                ws.SaveAs(excelFilePath, XlSaveAsAccessMode.xlNoChange);
//            }

//            finally
//            {
//                if ( wb != null ) { wb.Close(); wb = null; }
//                if ( appEx != null ) { appEx.Quit(); appEx = null; }
//            }
//        }




//        /// <summary>
//        ///     Add the specified text to the header of the columnName in front of original caption.
//        /// </summary>
//        public static void AddHeaderCaptionText(this UltraGrid grid, string columnName, string text, bool bold = false)
//        {
//            var header = grid.DisplayLayout.Bands[0].Columns[columnName].Header;

//            // Save original caption on tag
//            if ( header.Tag == null )
//                header.Tag = header.Caption;

//            header.Caption = "{0} ({1})".Frmt(header.Tag.ToString(), text);
//            header.Appearance.FontData.Bold = (bold) ? Infragistics.Win.DefaultableBoolean.True : Infragistics.Win.DefaultableBoolean.False;
//        }

//        // aux method of: AddHeaderAveragesCaptions
//        static List<string> GetExistingColumnsForGrid(UltraGrid grid, params string[] columns)
//        {
//            return columns.Aggregate(new List<string>(), (r, c) => {
//                if ( grid.DisplayLayout.Bands[0].Columns.Exists(c) )
//                    r.Add(c);

//                return r;
//            });
//        }

//        /// <summary>
//        ///     Calculates the avarages (if specified ) of the passed columns and set the average on the header of that column.
//        /// </summary>
//        static void _AddHeaderAveragesCaptions(UltraGrid grid, bool calcAverage, bool bold, params string[] columns)
//        {
//            if ( columns == null || columns.Length == 0 )
//                return;

//            // Determine which columns exists in this grid
//            List<string> existColumns = GetExistingColumnsForGrid(grid, columns);
            
//            if( existColumns.Count == 0)
//                throw new InvalidOperationException("none of columns exists in the current grid");

//            Dictionary<string, decimal> data = new Dictionary<string, decimal>(existColumns.Count);

//            foreach ( var row in grid.Rows )
//            {
//                // Only iterate over the columns that exists
//                foreach ( string column in existColumns )
//                {
//                    if ( data.ContainsKey(column) == false )
//                        data.Add(column, 0);

//                    var cellValue = row.Cells[column].OriginalValue ?? 0;
//                    data[column] = (data[column] + Convert.ToDecimal(cellValue));
//                }
//            }

//            // Divide for number of rows
//            if ( calcAverage )
//                existColumns.Iterate(k => data[k] = (data[k] / grid.Rows.Count));

//            // Set on the headers the averages
//            existColumns.Iterate(k => grid.AddHeaderCaptionText(k, data[k].ToString("#.##"), bold));
//        }


//        /// <summary>
//        ///     Calculates the averages of the passed columns and set the average on the header of that columns.
//        /// </summary>
//        public static void AddHeaderAveragesCaptions(this UltraGrid grid, bool bold, params string[] columns)
//        {
//            _AddHeaderAveragesCaptions(grid, true, bold, columns);
//        }


//        /// <summary>
//        ///     Calculates the totals of the passed columns and set the totals on the header of that columns.
//        /// </summary>
//        public static void AddHeaderTotalsCaptions(this UltraGrid grid, bool bold, params string[] columns)
//        {
//            _AddHeaderAveragesCaptions(grid, false, bold, columns);
//        }


//        /// <summary>
//        ///     Returns all the keys of the ColumnsCollections
//        /// </summary>
//        /// <param name="col"></param>
//        /// <returns></returns>
//        public static List<string> GetKeys(this ColumnsCollection col)
//        {
//            List<string> r = new List<string>(col.Count);

//            foreach ( var item in col )
//            {
//                r.Add(item.Key);
//            }

//            return r;
//        }


//        /// <summary>
//        ///     Reset the caption for headers.
//        ///     If null is passed in columns, resets all headers, otherwise return the headers of passed columns.
//        ///     Reseting the header operation , 6is to set the original caption.
//        /// </summary>
//        public static void ResetHeaderCaptions(this UltraGrid grid, params string[] columns)
//        {
//            List<string> existsColumns = ( columns.Length == 0 ) ? grid.DisplayLayout.Bands[0].Columns.GetKeys()
//                                                                 : GetExistingColumnsForGrid(grid, columns);
//            //
//            existsColumns.Iterate(k =>
//            {
//                var header = grid.DisplayLayout.Bands[0].Columns[k].Header;

//                // If not contains the tag, caption assumes key
//                if ( header.Tag != null )
//                {
//                    // If contains tag, caption assumes the tag text (typically setted by AddHeader methods)
//                    header.Caption = header.Tag.ToString();
//                }
//            });
//        }





//        #region Internal Methods
        

//        static void WriteHeaderToExcel(UltraGrid grid, Worksheet ws, bool displayHiddenColumns)
//        {
//            int columnIdx = 1;
//            var headerColumns = GetHeaderColumns(grid, displayHiddenColumns);

//            foreach ( var header in headerColumns )
//            {
//                Range excelRange = ws.GetCell(1, columnIdx++);
//                excelRange.Value2 = header.Header.Caption;
//            }
//        }

//        static void WriteBodyToExcel(UltraGrid grid, Worksheet ws, bool displayHiddenColumns)
//        {
//            var headerColumns = GetHeaderColumns(grid, displayHiddenColumns);

//            for ( int i = 0; i < grid.Rows.Count; i++ )
//            {
//                int columnIdx = 1;

//                foreach ( var column in headerColumns )
//                {
//                    UltraGridCell cell = grid.Rows[i].Cells[column.Key];
//                    Range excelRange = ws.GetCell(i + 2, columnIdx++);

//                    object value = cell.Value;

//                    if ( value != null ) 
//                    {
//                        if ( value.GetType() == typeof(DateTime) )
//                            excelRange.Value = value;
//                        else
//                            excelRange.Value2 = value;
//                    }
//                }
//            }
//        }



//        /// <summary>
//        ///     Returns a List of UltraGridColumns that are visible
//        /// </summary>
//        static LinkedList<UltraGridColumn> GetHeaderColumns(UltraGrid grid, bool displayHiddenColumns)
//        {
//            LinkedList<UltraGridColumn> result = new LinkedList<UltraGridColumn>();

//            if ( !displayHiddenColumns )
//            {
//                //
//                // Only add the columns that are visible in the layout

//                foreach ( var column in grid.DisplayLayout.Bands[0].Columns )
//                {
//                    if ( column.IsVisibleInLayout )
//                        result.AddLast(column);
//                }
//            }

//            else
//            {
//                //
//                // Add all columns

//                foreach ( var column in grid.DisplayLayout.Bands[0].Columns )
//                {
//                    result.AddLast(column);
//                }
//            }

//            return result;
//        }



//        #endregion
//}
