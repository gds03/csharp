using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.EntityFramework.Types
{
    public class ObjectEntryMap
    {
        public List<object> Unchanged { get; private set; }
        public List<object> Added { get; private set; }
        public List<object> Deleted { get; private set; }
        public List<object> Modified { get; private set; }

        public ObjectEntryMap()
        {
            Unchanged = new List<object>();
            Added = new List<object>();
            Deleted = new List<object>();
            Modified = new List<object>();
        }
    }
}
