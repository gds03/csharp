using System;

namespace Repository.ObjectMapper.Attributes
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
