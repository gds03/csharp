﻿using System;

namespace Repository.ObjectMapper.Types
{

    // Map CLR property type to a stored procedure parameter
    sealed class ProcMapping
    {
        internal KeyMapping Map;
        internal SPMode Mode;

        internal ProcMapping(String clrProperty, SPMode mode)
        {
            // Initially points to the name of the clrProperty (convention is used)
            Map = new KeyMapping(clrProperty, clrProperty);
            Mode = mode;
        }
    }
}
