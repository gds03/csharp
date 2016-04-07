using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CustomComponents.Mvc.UserControls.Models.GridView
{
    public class DataSource
    {
        public IEnumerable Items { get; set; }
        public int Total { get; set; }


        public DataSource(IEnumerable items, int total)
        {
            if (items == null)
                throw new ArgumentNullException("items");

            if (total < 0)
                throw new ArgumentException("total < 0");

            this.Items = items;
            this.Total = total;
        }
    }



    public class DataSource<TSource> : DataSource
    {
        public new IEnumerable<TSource> Items { get; set; }

        public DataSource(IEnumerable<TSource> items, int total)
            : base(items, total)
        {
            if (items == null)
                throw new ArgumentNullException("items");

            this.Items = items;
        }
    }

}