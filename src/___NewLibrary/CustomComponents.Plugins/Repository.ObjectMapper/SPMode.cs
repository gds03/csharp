using System;

namespace Repository.ObjectMapper
{
    [Flags]
    public enum SPMode
    {
        Insert = 1,
        Update = 2,
        Delete = 4
    }
}
