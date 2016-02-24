using CustomComponents.Mvc.UserControls.Models.GridView.Pagers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CustomComponents.Mvc.UserControls.Models.GridView
{
    public class GridContext
    {
        // consts
        internal const bool DEFAULT_ISCLICKABLE = true;
        internal const bool DEFAULT_ISSHOWINGRESULTSCOUNT = true;




        #region Properties

        /// <summary>Adds the class 'IsClickable' to each tr in table tbody</summary>
        public bool IsClickable { get; set; }

        /// <summary>Adds the class 'hand' to each th in table thead</summary>
        public bool IsSortable { get; set; }

        /// <summary>Determinates if results count are showed for the user</summary>
        public bool IsToShowResultsCount { get; set; }

        /// <summary>The pager that is used to display pagination</summary>
        public Pager Pager { get; set; }

        /// <summary>Data structure that contains the actual items and total items to be displayed</summary>
        public DataSource Data { get; set; }

        /// <summary>Data Structure based on a dictionary, that map columns (keys) to Columns </summary>
        public ColumnsOptions ColumnMappings { get; set; }

        /// <summary>Url where grid will make requests when paginating and sorting.</summary>
        public string CallbackUrl { get; set; }

        /// <summary>Form ID where the grid will extract fields and merge when paginating and sorting</summary>
        public string FormID { get; set; }

        #endregion



        // ctor
        public GridContext()
        {
            this.IsClickable = DEFAULT_ISCLICKABLE;
            this.IsToShowResultsCount = DEFAULT_ISSHOWINGRESULTSCOUNT;
        }
    }
}