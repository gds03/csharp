using System;
using System.Collections.Generic;

namespace VirtualNote.Kernel.Types
{
    public class PaginatedList<T> : List<T>
    {
        public int CurrentPage{ get; private set; }
        public int PageSize   { get; private set; }
        public int Total      { get; private set; }
        public int PagesCount { get; private set; }

        public PaginatedList(IEnumerable<T> source,
            int pageIndex, int pageSize, int total) 
        {
            AddRange(source);

            CurrentPage = pageIndex;
            PageSize = pageSize;
            Total = total;

            PagesCount = (int)Math.Ceiling(Total / (double)PageSize);
        }

        public bool HasPreviousPage {
            get { return CurrentPage > 1; }
        }

        public bool HasNextPage {
            get { return CurrentPage < PagesCount; }
        }
    }
}