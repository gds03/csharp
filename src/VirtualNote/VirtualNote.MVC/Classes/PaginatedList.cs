using System;
using System.Collections.Generic;
using System.Linq;

namespace VirtualNote.MVC.Classes
{
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex  { get; private set; }
        public int PageSize   { get; private set; }
        public int ItemsCount { get; private set; }
        public int PagesCount { get; private set; }

        public PaginatedList(IEnumerable<T> source, int pageIndex, int pageSize) 
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            ItemsCount = source.Count();
            PagesCount = (int)Math.Ceiling(ItemsCount / (double)PageSize);

            AddRange(source);
        }

        public bool HasPreviousPage {
            get {
                return (PageIndex > 0);
            }
        }

        public bool HasNextPage {
            get {
                return (PageIndex + 1 < PagesCount);
            }
        }
    }
}