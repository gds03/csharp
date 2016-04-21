using OMapper.Types.Mappings;
using System;
using System.Collections.Generic;

namespace OMapper.Types
{
    /// <summary>
    ///     Contains the necessary information about the table, the keys, the columns, and the identity column.
    /// </summary>
    internal class TypeSchema
    {
        internal Type CLRType;
        internal String TableName;                                                       // If != null overrides the type name (used for CUD operations)
        internal String IdentityPropertyName;                                            // If != null, this stores the property of the type that is identity

        internal IDictionary<string, KeyMapping> Keys;                                   // Stores the keys of the type (to uniquelly identify the one entity)

        internal IDictionary<string, ColumnMapping> Columns;                             // For each property, we have a custom mapping  

        internal IDictionary<string, ProcMapping> Procedures;                            // Stores parameters that must be used when ExecuteProc command is executed to send them to Stored Procedures


        internal TypeSchema(TypeSchema ts)
        {
            if (ts == null)
                throw new ArgumentNullException("ts");

            CLRType = ts.CLRType;
            TableName = ts.TableName;
            IdentityPropertyName = ts.IdentityPropertyName;

            Keys = ts.Keys;
            Columns = ts.Columns;
            Procedures = ts.Procedures;
        }



        internal TypeSchema(Type clrType)
        {
            if (clrType == null)
                throw new ArgumentNullException("clrType");

            CLRType = clrType;
            TableName = clrType.Name;

            int totalPropertiesCount = clrType.GetProperties(OMapper.s_PropertiesFlags).Length;
            Keys = new Dictionary<string, KeyMapping>(totalPropertiesCount);
            Columns = new Dictionary<string, ColumnMapping>(totalPropertiesCount);
            Procedures = new Dictionary<string, ProcMapping>(8);
        }
    }
}
