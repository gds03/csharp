using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ObjectMapper.Attributes
{
    public sealed class BindFrom : Attribute
    {
        internal String OverridedReadColumn;

        public BindFrom(String sqlColumnResult)
        {
            OverridedReadColumn = sqlColumnResult;
        }
    }
}
