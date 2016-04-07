﻿using Repository.ObjectMapper.Types;
using System;

namespace Repository.ObjectMapper.Attributes
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