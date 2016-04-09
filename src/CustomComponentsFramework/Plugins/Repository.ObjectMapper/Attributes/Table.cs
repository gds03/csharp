using System;

namespace Repository.OMapper.Attributes
{
    public sealed class Table : Attribute
    {
        internal String OverridedName;

        internal Table(String tableName)
        {
            OverridedName = tableName;
        }
    }
}
