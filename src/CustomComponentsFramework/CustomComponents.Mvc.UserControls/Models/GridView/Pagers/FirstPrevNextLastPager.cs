using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CustomComponents.Mvc.UserControls.Models.GridView.Pagers
{
    public class FirstPrevNextLastPager : Pager
    {
        public FirstPrevNextLastPager(int currentPage, int numItems, int totalItems)
            : base(currentPage, numItems, totalItems)
        {

        }

        public override bool hasPrevious { get { return CurrentPage > 1; } }
        public override bool hasNext { get { return CurrentPage < LastPage; } }

        public override bool showFirst { get { return true; } }
        public override bool showLast { get { return true; } }
    }
}