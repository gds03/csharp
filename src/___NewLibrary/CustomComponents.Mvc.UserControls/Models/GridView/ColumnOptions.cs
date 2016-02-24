using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CustomComponents.Mvc.UserControls.Models.GridView
{
    public class ColumnsOptions : Dictionary<string, Column>
    {
        // columns must be unique so we use build in dictionary to check keys      
    }

    public class Column
    {
        // consts
        internal const bool DEFAULT_ISVISIBLE = true;
        internal const bool DEFAULT_ISIDENTITY = false;

        // properties
        public bool IsVisible { get; set; }
        public bool IsIdentity { get; set; }
        public string Header { get; set; }

        // ctor
        public Column()
        {
            this.IsVisible = DEFAULT_ISVISIBLE;
            this.IsIdentity = DEFAULT_ISIDENTITY;
        }

        public Column(bool isIdentity)
        {
            this.IsIdentity = isIdentity;
        }

        public Column(string header, bool isVisible = DEFAULT_ISVISIBLE)
        {
            this.Header = header;
            this.IsVisible = isVisible;
        }
    }
}