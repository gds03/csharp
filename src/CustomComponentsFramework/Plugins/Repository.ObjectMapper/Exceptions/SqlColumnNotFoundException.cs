using System;

namespace Repository.ObjectMapper.Exceptions
{
    public sealed class SqlColumnNotFoundException : Exception
    {
        internal SqlColumnNotFoundException(string msg) : base(msg) { }
    }
}
