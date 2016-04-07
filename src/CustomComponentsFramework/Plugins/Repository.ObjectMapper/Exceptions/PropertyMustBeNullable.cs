using System;

namespace Repository.ObjectMapper.Exceptions
{

    public sealed class PropertyMustBeNullable : Exception
    {
        internal PropertyMustBeNullable(string msg) : base(msg) { }
    }
}
