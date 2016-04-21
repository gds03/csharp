using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Repository.OMapper.Internal
{
    public class ParameterizedCommandPlaceHolder
    {
        internal SelectCommand Select { get; private set; }

        internal string InsertCmd;
        internal string DeleteCmd;


        public ParameterizedCommandPlaceHolder()
        {
            Select = new SelectCommand();
            InsertCmd = DeleteCmd = null;
        }
    }



    internal class SelectCommand
    {
        internal List<Filter> Filters { get; private set; }

        internal string Cmd;

        public SelectCommand()
        {
            Filters = new List<Filter>();
        }

        internal bool ContainsExpression(Expression ex)
        {
            if (ex == null)
                return Filters.Any(x => x.Hash == 0);

            int h = ex.GetHashCode();
            return Filters.Any(x => x.Hash == h);
        }
    }



    internal class Filter
    {
        public int Hash { get; private set; }

        public string FilterText { get; private set; }

        public Filter(string filterText, Expression filter)
        {
            if (string.IsNullOrEmpty(filterText))
                throw new ArgumentNullException("filterText");

            Hash = (filter == null) ? 0 : filter.GetHashCode();
        }
    }
}
