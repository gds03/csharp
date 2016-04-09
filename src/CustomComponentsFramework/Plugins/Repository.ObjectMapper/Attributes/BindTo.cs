using System;

namespace Repository.OMapper.Attributes
{
    public sealed class BindTo : Attribute
    {
        internal String OverridedSqlColumn;

        public BindTo(String sqlColumnSchema)
        {
            OverridedSqlColumn = sqlColumnSchema;
        }
    }
}
