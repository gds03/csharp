using System;

namespace Repository.OMapper.Exceptions
{
    public sealed class SqlColumnNotFoundException : Exception
    {
        internal SqlColumnNotFoundException(string msg) : base(msg) { }
    }
}
