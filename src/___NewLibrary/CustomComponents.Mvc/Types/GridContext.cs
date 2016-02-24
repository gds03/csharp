using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace CustomComponents.Mvc.Types
{
    #region Grid
    
    public class GridContext
    {
        // < --------- please do not change here --------->
        public const string HIDDEN_FIELD_ID = "Grid_ListItemID";
        public const string DEFAULT_CONTAINER_ID = "DynamicMvcGrid1";
        public const bool SHOW_RESULTS_COUNT = true;

        // Public Properties
        public bool                                     ShowResultsCount { get; set; }
        public string                                   GridContainerID { get; set; }
        public GridValue<object>                        Tuples { get; set; }
        public Dictionary<string, GridColumnMetadata>   Columns { get; set; }
        public ExportOptions                            ExportActionParams { get; set; }
        public bool                                     TdWithColumnName { get; set; }        // Used to be possible to identify the columnName of each cell
        public ResourceManager                          ResourcesManager { get; set; }
        public bool                                     AllowClick { get; set; }


        public GridContext()
        {
            ShowResultsCount = SHOW_RESULTS_COUNT;
            GridContainerID = DEFAULT_CONTAINER_ID;
            AllowClick = true;
        }


        /*
         * { "Header1", "Column1", new GridColumnMetadata("Header1", true)  } -> indicate that is the ID (you should only set one flag to true of the set
         * { "Header2", "Column2", new GridColumnMetadata("Header2") }
         * { "Header3", "Column3", new GridColumnMetadata("Header3") }
         * { "Header4", "Column4", new GridColumnMetadata("Header4") }
         */
    }






    #endregion


    #region Grid Columns

    public class GridColumnMetadata
    {
        public bool                     IsIdentityColumn { get; set; }
        public bool                     ShowColumn { get; set; }
        public bool                     UsePropertyResources { get; set; }

        public string                   HeaderName { get; set; }
        public Func<object, string>     HTML { get; set; }
        public string                   CssClass { get; set; }


        public GridColumnMetadata()
        {
            ShowColumn = true;
            UsePropertyResources = true;
        }

        public GridColumnMetadata(bool usePropertyResources)
            : this()
        {
            UsePropertyResources = usePropertyResources;
        }


        public GridColumnMetadata(string headerName)
            : this()
        {
            this.HeaderName = headerName;
        }

        public GridColumnMetadata(string headerName, bool isIdentityColumn)
            : this(headerName)
        {
            this.IsIdentityColumn = isIdentityColumn;
        }

        public GridColumnMetadata(string headerName, bool isIdentityColumn, bool showColumn)
            : this(headerName, isIdentityColumn)
        {
            this.ShowColumn = showColumn;
        }

        public GridColumnMetadata(string headerName, bool isIdentityColumn, bool showColumn, Func<object, string> userFunction)
            : this(headerName, isIdentityColumn, showColumn)
        {
            if (userFunction == null)
                throw new ArgumentNullException("userFunction");

            HTML = userFunction;
        }

        public GridColumnMetadata SetCssClass(string @class) { this.CssClass = @class; return this; }
    }

    public class GridColumnMetadata<T> : GridColumnMetadata
    {
        public new Func<T, string> HTML { get; set; }

        public GridColumnMetadata(string headerName, bool isIdentityColumn, bool showColumn, Func<T, string> userFunction)
            : base(headerName, isIdentityColumn, showColumn)
        {
            if (userFunction == null)
                throw new ArgumentNullException("userFunction");

            HTML = userFunction;
        }
    }


    #endregion





    #region Grid Tuples


    public abstract class GridPagerModel
    {
        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; }
        public int TotalItems { get; set; }

        public int GetIndex(out int take)
        {
            int index = (CurrentPage - 1) * ItemsPerPage;
            int count = ItemsPerPage;

            if ( index + count > TotalItems )
                count = TotalItems - index;

            take = count;
            return index;
        }
    }


    public class GridValue<T> : GridPagerModel
    {
        public IEnumerable<T> Items { get; set; }
    }

    public class GridValue : GridValue<object>
    {

    }


    #endregion




    #region Grid Export

    public class ExportOptions<TModel>
        where TModel : class
    {
        public string URLExport { get; set; }
        public TModel Model { get; set; }

        public ExportOptions(string url, TModel model = null)
        {
            if ( string.IsNullOrEmpty("url") )
                throw new ArgumentException("url is null or empty");

            URLExport = url;
            Model = model;
        }
    }

    public class ExportOptions : ExportOptions<object>
    {
        public ExportOptions(string url, Object model = null) : base(url, model)
        {

        }
    }

    #endregion
}
