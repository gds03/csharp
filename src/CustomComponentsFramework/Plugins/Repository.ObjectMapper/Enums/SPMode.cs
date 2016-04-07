using System;

namespace Repository.ObjectMapper.Types
{
    [Flags]
    public enum SPMode
    {
        Insert = 1,
        Update = 2,
        Delete = 4
    }
}
