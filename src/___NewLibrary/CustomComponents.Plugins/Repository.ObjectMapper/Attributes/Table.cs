using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ObjectMapper.Attributes
{
    public sealed class Table : Attribute
    {
        internal String OverridedName;

        internal Table(String tableName)
        {
            OverridedName = tableName;
        }
    }
}
