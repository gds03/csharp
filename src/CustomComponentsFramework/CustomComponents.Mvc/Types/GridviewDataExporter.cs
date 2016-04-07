using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CustomComponents.Mvc.Types
{
    /// <summary>
    ///     Indicates that this column is not considerated in export process to Excel.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class ExcelExclude : Attribute
    {

    }

    public class GridViewDataExporter
    {
        public const string EXCEL_CONTENTTYPE = "application/vnd.ms-excel";
        public const string EXCEL_EXTENSION = ".xls";

        private Type ItemsType { get; set; }

        /// <summary>
        ///     Returns the Stream that is ready to be transformed
        /// </summary>
        public Stream PrepareExportStream<T>(IEnumerable<T> source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            ItemsType = typeof(T);

            //
            // Fill grid first with source

            GridView gv = new GridView();
            StyleGrid(gv);

            gv.RowDataBound += gv_RowDataBound;
            gv.DataSource = source;
            gv.DataBind();


            //
            // Fill stream with grid HTML

            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            gv.RenderControl(htw);
            string htmlFullString = sw.ToString();

            return GenerateStreamFromString(htmlFullString);
        }


        /// <summary>
        ///     Returns the Stream that is ready to be transformed
        /// </summary>
        public Stream PrepareExportStream<T>(T source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            return PrepareExportStream(new[] { source });
        }




        #region Helpers



        void gv_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            var rowType = e.Row.RowType;

            if (rowType == DataControlRowType.DataRow || rowType == DataControlRowType.Header)
            {
                //
                // Check if ExcelExclude is in each property of the object and if it is, hide that column

                int idxColumn = 0;
                foreach (var property in ItemsType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    bool showColumn = true;

                    // custom attributes for level
                    foreach (var propertyAttr in property.GetCustomAttributes(true))
                    {
                        ExcelExclude excelExclude = propertyAttr as ExcelExclude;

                        if (excelExclude != null)
                        {

                            //
                            // Used in Header and DataRow

                            e.Row.Cells[idxColumn].Visible = false;
                            showColumn = false;
                            break;
                        }

                        DisplayAttribute displayAttribute = propertyAttr as DisplayAttribute;

                        if (rowType == DataControlRowType.Header && displayAttribute != null)
                        {
                            ResourceManager resourceManager = GetResourceFromAttribute(displayAttribute);

                            string FinalDescription = resourceManager.GetString(displayAttribute.Name);

                            if (string.IsNullOrEmpty(FinalDescription))
                                FinalDescription = displayAttribute.Description;

                            // 
                            // set description within table

                            if (idxColumn < e.Row.Cells.Count)
                                e.Row.Cells[idxColumn].Text = FinalDescription;
                        }
                    }

                    // property for level
                    if (showColumn && rowType == DataControlRowType.DataRow)
                    {
                        object value = property.GetValue(e.Row.DataItem);

                        if (value != null)
                        {
                            // 
                            // adjust values of cells

                            if (idxColumn < e.Row.Cells.Count)
                                e.Row.Cells[idxColumn].Text = value.ToString();
                        }
                    }

                    idxColumn++;
                }
            }
        }

        private static ResourceManager GetResourceFromAttribute(DisplayAttribute displayAttribute)
        {
            if (displayAttribute == null)
                throw new ArgumentNullException("displayAttribute");

            if (displayAttribute.ResourceType == null)
                throw new InvalidOperationException("ResourceType for current displayAttribute is null");

            Type resourceType = displayAttribute.ResourceType;
            const BindingFlags flags = BindingFlags.Static |
                                       BindingFlags.GetProperty |
                                       BindingFlags.InvokeMethod |
                                       BindingFlags.Public |
                                       BindingFlags.FlattenHierarchy;

            //
            // each resource file have ResourceManager static property to obtain the ResourceManager

            object result = resourceType.GetProperty("ResourceManager", flags)
                                        .GetValue(null, null);

            return (ResourceManager)result;
        }



        private static MemoryStream GenerateStreamFromString(string str)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream, new UnicodeEncoding());
            writer.Write(str);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }





        private static void StyleGrid(GridView gv)
        {
            gv.HeaderStyle.BackColor = System.Drawing.Color.FromName("#507CD1");
            gv.HeaderStyle.Font.Bold = true;
            gv.HeaderStyle.ForeColor = System.Drawing.Color.White;
            gv.RowStyle.BackColor = System.Drawing.Color.FromName("EFF3FB");
            gv.AlternatingRowStyle.BackColor = System.Drawing.Color.Silver;
        }


        #endregion

    }
}
