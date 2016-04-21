using System;

namespace OMapper.Types.Mappings
{
    /// <summary>
    ///     Map CLR properties to SQL columns, and contains the column to get data from a ResultSet
    /// </summary>
    sealed class ColumnMapping
    {
        internal Type ClrPropertyType;
        internal String ClrProperty;
        internal String ToSqlTableColumn;
        internal String FromResultSetColumn;
        

        internal ColumnMapping(Type propertyType, String clrProperty)
        {
            if (propertyType == null)
                throw new ArgumentNullException("propertyType");

            // Initially all points to the name of the clrProperty (convention is used)
            FromResultSetColumn = ToSqlTableColumn = ClrProperty = clrProperty;
            ClrPropertyType = propertyType;
        }

        public override int GetHashCode()
        {
            return this.ClrProperty.GetHashCode();
        }
    }

}
