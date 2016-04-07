using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomComponents.Services.Types
{
    public class QuerySearchOptions
    {
        public string OrderByColumn { get; private set; }
        public bool Ascending { get; private set; }

        public QuerySearchOptions(string orderByColumn, bool ascending)
        {
            this.OrderByColumn = orderByColumn;
            this.Ascending = ascending;
        }
    }
}
