using Microsoft.Office.Interop.Excel;
using System.Reflection;

namespace EnhancedLibrary.ExtensionMethods.Business
{
    /// <summary>
    ///     Extends and simplify the excel interop API
    /// </summary>
    public static class ExcelExtensions
    {
        static readonly Missing s_m = Missing.Value;






        #region Application


        /// <summary>
        ///     Adds a new workbook to the application
        /// </summary>
        /// <param name="appEx"></param>
        /// <returns>The workbook added</returns>
        public static Workbook AddWorkBook(this Application appEx) 
        {
            return appEx.Workbooks.Add(XlWBATemplate.xlWBATWorksheet);
        }


        /// <summary>
        ///     Open a workbook from a filePath
        /// </summary>
        /// <returns>The workbook from filePath</returns>
        public static Workbook OpenWorkBook(this Application appEx, string filePath) 
        {
            return appEx.Workbooks.Open(filePath, s_m, s_m, s_m, s_m, s_m, s_m, s_m, s_m, s_m, s_m, s_m, s_m, s_m);
        }



        #endregion









        #region Workbook




        /// <summary>
        ///     Saves the current workbook to specific filePath with the specific mode.
        /// </summary>
        public static void SaveAs(this Workbook wb, string filePath, XlSaveAsAccessMode mode) {
            wb.SaveAs(filePath, s_m, s_m, s_m, s_m, s_m, mode, s_m, s_m, s_m, s_m, s_m);
        }


        /// <summary>
        ///     Get the worksheet from a index
        /// </summary>
        /// <returns>The worksheet from the book based on a index (start at position 1)</returns>
        public static Worksheet GetWorkSheet(this Workbook book, int index) {
            return (Worksheet) book.Worksheets[index];
        }






        #endregion









        #region Worksheet



        /// <summary>
        ///     Get a specific cell.
        /// </summary>
        /// <returns>A range with 1 cell and 1 column</returns>
        public static Range GetCell(this Worksheet sheet, int row, int column) 
        {
            return (Range) sheet.Cells[row, column];
        }


        /// <summary>
        ///     Get a specific range of cells
        /// </summary>
        /// <returns>A range with a matrix from first cell to the second cell</returns>
        public static Range GetRange(this Worksheet sheet, int r1, int c1, int r2, int c2) 
        {
            return sheet.Range[sheet.Cells[r1, c1], sheet.Cells[r2, c2]];
        }


        /// <summary>
        ///     Count the existing rows in the excel document
        /// </summary>
        /// <returns>The number of rows</returns>
        public static int CountRows(this Worksheet sheet) 
        {
            return sheet.Cells.Find("*", s_m, XlFindLookIn.xlValues, XlLookAt.xlWhole,
                XlSearchOrder.xlByRows, XlSearchDirection.xlPrevious,
                false, false, s_m).Row;
        }



        /// <summary>
        ///     Count the existing columns in the excel document
        /// </summary>
        /// <returns>The number of columns</returns>
        public static int CountColumns(this Worksheet sheet) 
        {
            return sheet.Cells.Find("*", s_m, XlFindLookIn.xlValues, XlLookAt.xlWhole,
                XlSearchOrder.xlByColumns, XlSearchDirection.xlPrevious,
                false, false, s_m).Column;
        }





        #endregion


    }
}
