using Repository.EntityFramework.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.EntityFramework.Types
{

    public class ObjectEntry
    {
        public Entry State { get; internal set; }
        public Object @Object { get; internal set; }
    }
}
