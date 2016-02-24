using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ObjectMapper.Attributes
{
    public sealed class BindTo : Attribute
    {
        internal String OverridedSqlColumn;

        public BindTo(String sqlColumnSchema)
        {
            OverridedSqlColumn = sqlColumnSchema;
        }
    }
}
