﻿using System;

namespace Repository.OMapper.Types
{
    [Flags]
    public enum SPMode
    {
        Insert = 1,
        Update = 2,
        Delete = 4
    }
}
