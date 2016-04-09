using Repository.OMapper.Types;
using System;

namespace Repository.OMapper.Attributes
{

    public sealed class StoredProc : Attribute
    {
        internal String ParameterName;
        internal SPMode Mode;

        public StoredProc(SPMode mode)
        {
            Mode = mode;
        }

        public StoredProc(SPMode mode, String name)
            : this(mode)
        {
            ParameterName = name;
        }
    }
}
