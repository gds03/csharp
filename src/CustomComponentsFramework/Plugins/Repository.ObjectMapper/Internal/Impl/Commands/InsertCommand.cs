using Repository.ObjectMapper.Interfaces;
using Repository.ObjectMapper.Types;
using Repository.ObjectMapper.Types.Mappings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Repository.ObjectMapper.Internal.Commands.Impl
{
    internal partial class CommandsForTypeSchema : CommandsForTypeSchemaBase, ISqlCommandTextGenerator
    {
        const string scopy_id_name = "Scope_Identity";


        /// <summary>
        ///     Creates a SQL string that will represent Insert statement.
        ///     This method will use parameterized queries.
        /// </summary>
        /// <param name="obj">The object that will be updated with identity</param>
        /// <returns>The SQL Command</returns>
        public String InsertCommand(object obj)
        {
            Debug.Assert(obj != null);

            //
            // Obtain local copy because another thread can change the reference of _typesSchema
            // and we need iterate in a secure way.
            //


            Type objRepresentor = obj.GetType();
            TypeSchema schema = ObjectMapper.s_TypesSchemaMapper[objRepresentor];         // Get schema information for specific Type


            StringBuilder cmdTxt = new StringBuilder("exec sp_executesql N'insert [{0}] (".Frmt(schema.TableName));


            // Build header (exclude identities)
            foreach (ColumnMapping cm in schema.Columns.Values)
            {
                if (cm.ClrProperty == schema.IdentityPropertyName)                    // Identity Column never's updated!
                    continue;

                cmdTxt.Append("[{0}], ".Frmt(cm.ToSqlTableColumn));
            }

            cmdTxt.Remove(cmdTxt.Length - 2, 2); // Remove last ,
            cmdTxt.Append(") values (");

            // Build body (exclude identities)
            int paramIndex = 0;
            foreach (ColumnMapping cm in schema.Columns.Values)
            {
                if (cm.ClrProperty == schema.IdentityPropertyName)                    // Identity Column never's updated!
                    continue;

                cmdTxt.Append("@{0}, ".Frmt(paramIndex++));
            }

            cmdTxt.Remove(cmdTxt.Length - 2, 2); // Remove last ,
            cmdTxt.Append(")");
            cmdTxt.Append(" select SCOPE_IDENTITY() as [{0}]', N'".Frmt(scopy_id_name));


            //
            // Set parameter indexes and types,                                                     @0 varchar(max), @1 int, ...
            //

            paramIndex = 0;
            foreach (ColumnMapping cm in schema.Columns.Values)
            {
                if (cm.ClrProperty == schema.IdentityPropertyName)                            // Identity Column never's updated!
                    continue;

                // set sql type based on property type of the object
                Type propertyType = objRepresentor.GetProperty(cm.ClrProperty).PropertyType;
                cmdTxt.Append("@{0} {1}, ".Frmt(paramIndex++, ObjectMapper.s_ClrToSqlTypesMapper[propertyType]));    // Map CLR property to SqlColumn Type 
            }

            cmdTxt.Remove(cmdTxt.Length - 2, 2);    // Remove last
            cmdTxt.Append("', ");   // Close quote and add comma


            //
            // Set parameter indexes and data
            //

            paramIndex = 0;
            foreach (ColumnMapping cm in schema.Columns.Values)
            {
                if (cm.ClrProperty == schema.IdentityPropertyName)                            // Identity Column never's updated!
                    continue;

                PropertyInfo pi = objRepresentor.GetProperty(cm.ClrProperty);
                String valueTxt = ObjectMapper.PrepareValue(pi.GetValue(obj, null));                         // Can contain quotes, based on property type

                cmdTxt.Append("@{0} = {1}, ".Frmt(paramIndex++, valueTxt));
            }

            cmdTxt.Remove(cmdTxt.Length - 2, 2); // Remove last ,
            return cmdTxt.ToString();
        }
    }
}
