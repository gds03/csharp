using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CustomComponents.Mvc.UserControls.Models.GridView.Pagers
{
    public abstract class Pager
    {
        /// <summary>Determines if last page is displayed</summary>
        public int LastPage { get; private set; }

        /// <summary>Gets the current page</summary>
        public int CurrentPage { get; private set; }





        /// <summary>Determines if previous link is displayed</summary>
        public abstract bool hasPrevious { get; }

        /// <summary>Determines if next link is displayed</summary>
        public abstract bool hasNext { get; }

        /// <summary>Determines if first link is displayed</summary>
        public abstract bool showFirst { get; }

        /// <summary>Determines if last link is displayed</summary>
        public abstract bool showLast { get; }


        public Pager(int currentPage, int numItems, int totalItems)
        {
            if (currentPage < 1)
                throw new ArgumentNullException("currentPage < 1");

            if (numItems <= 0)
                throw new ArgumentNullException("numItems <= 0");

            // current page
            this.CurrentPage = currentPage;
            int bucket = (currentPage - 1) * numItems;

            // last page
            this.LastPage = (totalItems / numItems);
            if ((totalItems % numItems) > 1) { LastPage++; }
        }
    }
}