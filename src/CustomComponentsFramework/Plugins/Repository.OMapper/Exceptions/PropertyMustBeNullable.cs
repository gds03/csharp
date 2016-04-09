using System;

namespace Repository.OMapper.Exceptions
{

    public sealed class PropertyMustBeNullable : Exception
    {
        internal PropertyMustBeNullable(string msg) : base(msg) { }
    }
}
