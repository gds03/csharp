using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomComponents.Core.Types.Generic
{

    public class TableInfo<TModel>
    {
        public IEnumerable<TModel> Items { get; set; }
        public int Count { get; set; }

        public TableInfo(int count, IEnumerable<TModel> items)
        {
            if (count < 0)
                throw new InvalidOperationException("count cannot be < 0");

            if (items == null)
                throw new ArgumentNullException("items");

            this.Items = items;
            this.Count = count;
        }



        public TableInfo<TCast> Cast<TCast>()
        {
            var i = Items.Cast<TCast>();
            TableInfo<TCast> r = new TableInfo<TCast>(Count, i);
            return r;
        }
    }  
}
