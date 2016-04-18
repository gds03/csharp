using Repository.OMapper.Internal.Proxies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repository.OMapper.Types
{
    internal class TypeSchemaProxy : TypeSchema
    {
        public Type ProxyType { get; private set; }

        internal TypeSchemaProxy(TypeSchema ts) : base(ts)
        {
            ProxyType = ProxyMapper.Map(ts.CLRType);
        }
    }
}

