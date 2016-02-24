using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ObjectMapper
{
    [Flags]
    public enum SPMode
    {
        Insert = 1,
        Update = 2,
        Delete = 4
    }
}
