using System;

namespace Repository.OMapper.Types.Mappings
{
    /// <summary>
    ///     Map CLR properties to SQL columns, and contains the column to get data from a ResultSet
    /// </summary>
    sealed class ColumnMapping
    {
        internal String ClrProperty;
        internal String ToSqlTableColumn;
        internal String FromResultSetColumn;

        internal ColumnMapping(String clrProperty)
        {
            // Initially all points to the name of the clrProperty (convention is used)
            FromResultSetColumn = ToSqlTableColumn = ClrProperty = clrProperty;
        }

        public override int GetHashCode()
        {
            return this.ClrProperty.GetHashCode();
        }
    }

}
