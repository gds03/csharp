using System.Collections.Generic;
using Infragistics.Win.UltraWinGrid;
using System.IO;
using System;
using Microsoft.Office.Interop.Excel;
using EnhancedLibrary.ExtensionMethods.Business;

namespace EnhancedLibrary.ExtensionMethods.Graphics
{
    public static class UltraGridExtensions
    {
        /// <summary>
        ///     Binds a list of TItems to the grid.
        /// </summary>
        public static void BindGrid<TItems>(this UltraGrid grid, IList<TItems> items) 
        {
            if ( items == null || items.Count == 0 ) {
                grid.DataSource = new List<TItems>();
            }
            else {
                grid.DataSource = items;
            }

            grid.DataBind();
        }


        /// <summary>
        ///     Binds a list of TItems to the grid.
        /// </summary>
        public static void SetDataBinding<TItems>(this UltraGrid grid, IList<TItems> items)
        {
            grid.SetDataBinding(items, "", true, true);
        }

        /// <summary>
        ///     Activate the active row again.
        ///     This method can be util if you have an event that fires when an active row becames active.
        /// </summary>
        public static void ActiveRowRefresh(this UltraGrid grid)
        {
            var bk = grid.ActiveRow;

            grid.ActiveRow = null;
            grid.ActiveRow = bk;

            grid.ActiveRow.Activate();
        }


        /// <summary>
        ///     Returns the selected object that are selected in the grid
        /// </summary>
        public static T GetSelectedObject<T>(this UltraGrid grid)
        {
            if (grid.ActiveRow == null)
                return default(T);

            return (T) grid.ActiveRow.ListObject;
        }



        /// <summary>
        ///     Clears the content of the current grid
        /// </summary>
        /// <param name="grid"></param>
        public static void Clear<T>(this UltraGrid grid)
        {
            grid.SetDataBinding(new List<T>(),"", true, true);
        }









        /// <summary>
        /// Exports an UltraGrid data to an Excel document
        /// </summary>
        /// <param name="grid">UltraGrid</param>
        /// <param name="excelFilePath">excel document full file path</param>
        /// <param name="displayHiddenColumns">set if hidden columns will be included in the excel file</param>
        public static void ExportExcel(this UltraGrid grid, string excelFilePath, bool displayHiddenColumns)
        {
            if ( File.Exists(excelFilePath) )
            {
                try
                {
                    File.Delete(excelFilePath);
                }
                catch ( Exception )
                {
                    throw new IOException();
                }
            }

            if ( grid == null )
                throw new ArgumentNullException("ug");


            if ( excelFilePath.IsNE() )
                throw new ArgumentNullException("excelFilePath");


            //
            // Create Application instance

            Microsoft.Office.Interop.Excel.Application appEx = null;
            Workbook wb = null;
            Worksheet ws = null;


            try
            {
                appEx = new Microsoft.Office.Interop.Excel.Application();
                wb = appEx.AddWorkBook();
                ws = wb.GetWorkSheet(1);

                // Header
                WriteHeaderToExcel(grid, ws, displayHiddenColumns);

                // Body
                WriteBodyToExcel(grid, ws, displayHiddenColumns);


                // Persist data in the file
                ws.SaveAs(excelFilePath, XlSaveAsAccessMode.xlNoChange);
            }

            finally
            {
                if ( wb != null ) { wb.Close(); wb = null; }
                if ( appEx != null ) { appEx.Quit(); appEx = null; }
            }
        }












        #region Internal Methods
        

        static void WriteHeaderToExcel(UltraGrid grid, Worksheet ws, bool displayHiddenColumns)
        {
            int columnIdx = 1;
            var headerColumns = GetHeaderColumns(grid, displayHiddenColumns);

            foreach ( var header in headerColumns )
            {
                Range excelRange = ws.GetCell(1, columnIdx++);
                excelRange.Value2 = header.Header.Caption;
            }
        }

        static void WriteBodyToExcel(UltraGrid grid, Worksheet ws, bool displayHiddenColumns)
        {
            var headerColumns = GetHeaderColumns(grid, displayHiddenColumns);

            for ( int i = 0; i < grid.Rows.Count; i++ )
            {
                int columnIdx = 1;

                foreach ( var column in headerColumns )
                {
                    UltraGridCell cell = grid.Rows[i].Cells[column.Key];
                    Range excelRange = ws.GetCell(i + 2, columnIdx++);

                    object value = cell.Value;

                    if ( value.GetType() == typeof(DateTime) )
                        excelRange.Value = value;
                    else
                        excelRange.Value2 = value;
                }
            }
        }



        /// <summary>
        ///     Returns a List of UltraGridColumns that are visible
        /// </summary>
        static LinkedList<UltraGridColumn> GetHeaderColumns(UltraGrid grid, bool displayHiddenColumns)
        {
            LinkedList<UltraGridColumn> result = new LinkedList<UltraGridColumn>();

            if ( !displayHiddenColumns )
            {
                //
                // Only add the columns that are visible in the layout

                foreach ( var column in grid.DisplayLayout.Bands[0].Columns )
                {
                    if ( column.IsVisibleInLayout )
                        result.AddLast(column);
                }
            }

            else
            {
                //
                // Add all columns

                foreach ( var column in grid.DisplayLayout.Bands[0].Columns )
                {
                    result.AddLast(column);
                }
            }

            return result;
        }



        #endregion
    }
}
