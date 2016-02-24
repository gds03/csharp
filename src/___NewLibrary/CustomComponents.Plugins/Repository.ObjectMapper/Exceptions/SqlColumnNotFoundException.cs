using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ObjectMapper.Exceptions
{
    public sealed class SqlColumnNotFoundException : Exception
    {
        internal SqlColumnNotFoundException(string msg) : base(msg) { }
    }
}
