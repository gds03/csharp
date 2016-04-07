using Repository.ObjectMapper.Types;
using Repository.ObjectMapper.Types.Mappings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Repository.ObjectMapper.Internal.Commands
{
    internal partial class CommandsForTypeSchema
    {

        /// <summary>
        ///     Creates a SQL string that will represent Update statement.
        ///     This method will use parameterized queries.
        /// </summary>
        /// <typeparam name="T">The type of object being mapped.</typeparam>
        /// <param name="type">The object type</param>
        /// <param name="obj">The object that update command is being build from.</param>
        /// <param name="propertiesChanged">The array of properties that have change since the Select operation</param>
        /// <returns>The SQL Command</returns>
        internal static String PrepareUpdateCmd<T>(ObjectMapper orm, T obj, string[] propertiesChanged) where T : class
        {
            Debug.Assert(orm != null);
            Debug.Assert(obj != null);
            Debug.Assert(propertiesChanged != null);

            Type objRepresentor = obj.GetType();

            //
            // Obtain local copy because another thread can change the reference of _typesSchema
            // and we need iterate in a secure way.
            //

            TypeSchema schema = ObjectMapper.s_TypesToMetadataMapper[objRepresentor];

            if (schema.Keys.Count == 0)
                throw new InvalidOperationException("Type {0} must have at least one key for updating".Frmt(objRepresentor.Name));

            //
            // Update only if we have keys, to find the tuple
            // 

            StringBuilder cmdTxt = new StringBuilder("exec sp_executesql N'update [{0}] set ".Frmt(schema.TableName));


            // Build Set clause
            int paramIndex = 0;
            foreach (ColumnMapping cm in schema.Columns.Values)
            {
                if (cm.ClrProperty == schema.IdentityPropertyName || !propertiesChanged.Any(p => p == cm.ClrProperty))   // Identity Column never's updated nor unchanged properties!
                    continue;

                cmdTxt.Append("[{0}] = @{1}, ".Frmt(cm.ToSqlTableColumn, paramIndex++));        // [Column] = @0, [Column2] = @1 ...
            }

            if (cmdTxt.Length > 1)
                cmdTxt.Remove(cmdTxt.Length - 2, 2);    // Remove last

            // Build Where clause
            cmdTxt.Append(" where ");

            int count = 0;
            foreach (KeyMapping map in schema.Keys.Values)
            {
                cmdTxt.Append("[{0}] = @{1} ".Frmt(map.To, paramIndex++));

                if ((count + 1) < schema.Keys.Count)
                    cmdTxt.Append(" and ");

                count++;
            }

            cmdTxt.Append("', N'"); // Close quote and add comma

            //
            // Set the types of parameters for set region,                                                     @0 varchar(max), @1 int, ...
            //

            paramIndex = 0;
            foreach (ColumnMapping cm in schema.Columns.Values)
            {
                if (cm.ClrProperty == schema.IdentityPropertyName || !propertiesChanged.Any(p => p == cm.ClrProperty))  // Identity Column never's updated nor updated properties!
                    continue;

                // set sql type based on property type of the object
                Type propertyType = objRepresentor.GetProperty(cm.ClrProperty).PropertyType;
                cmdTxt.Append("@{0} {1}, ".Frmt(paramIndex++, ObjectMapper.s_ClrTypeToSqlTypeMapper[propertyType]));    // Map CLR property to SqlColumn Type 
            }

            // Set the types of parameters for where region
            foreach (KeyMapping map in schema.Keys.Values)
            {

                // set sql type based on property type of the object
                Type propertyType = objRepresentor.GetProperty(map.From).PropertyType;
                cmdTxt.Append("@{0} {1}, ".Frmt(paramIndex++, ObjectMapper.s_ClrTypeToSqlTypeMapper[propertyType]));    // Map CLR property to SqlColumn Type 
            }

            if (cmdTxt.Length > 1)
                cmdTxt.Remove(cmdTxt.Length - 2, 2);    // Remove last

            cmdTxt.Append("', ");   // Close quote and add comma


            //
            // Set data of parameters in set region
            //

            paramIndex = 0;
            foreach (ColumnMapping cm in schema.Columns.Values)
            {
                if (cm.ClrProperty == schema.IdentityPropertyName || !propertiesChanged.Any(p => p == cm.ClrProperty))   // Identity Column never's updated nor updated properties!
                    continue;

                PropertyInfo pi = objRepresentor.GetProperty(cm.ClrProperty);
                String valueTxt = ObjectMapper.PrepareValue(pi.GetValue(obj, null));                         // Can contain quotes, based on property type

                cmdTxt.Append("@{0} = {1}, ".Frmt(paramIndex++, valueTxt));
            }

            // Set data of parameters in where region
            foreach (KeyMapping map in schema.Keys.Values)
            {
                PropertyInfo pi = objRepresentor.GetProperty(map.From);
                String valueTxt = ObjectMapper.PrepareValue(pi.GetValue(obj, null));                          // Can contain quotes, based on property type

                cmdTxt.Append("@{0} = {1}, ".Frmt(paramIndex++, valueTxt));
            }

            if (cmdTxt.Length > 1)
                cmdTxt.Remove(cmdTxt.Length - 2, 2); // Remove last ,

            return cmdTxt.ToString();
        }
    }
}
