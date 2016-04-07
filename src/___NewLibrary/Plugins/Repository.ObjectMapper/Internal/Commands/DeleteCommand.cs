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
        ///     Creates a SQL string that will represent Delete statement.
        ///     This method will use parameterized queries.
        /// </summary>
        /// <typeparam name="T">The type of object being mapped.</typeparam>
        /// <param name="type">The object type</param>
        /// <param name="obj">The object that delete command is being build from.</param>
        /// <returns>The SQL Command</returns>
        internal static String PrepareDeleteCmd<T>(ObjectMapper orm, T obj) where T : class
        {
            Debug.Assert(orm != null);
            Debug.Assert(obj != null);

            Type objRepresentor = obj.GetType();

            //
            // Obtain local copy because another thread can change the reference of _typesSchema
            // and we need iterate in a secure way.
            //

            TypeSchema schema = ObjectMapper.s_TypesToMetadataMapper[objRepresentor];

            if (schema.Keys.Count == 0)
                throw new InvalidOperationException("Type {0} must have at least one key for deleting".Frmt(objRepresentor.Name));


            //
            // Delete only if we have keys, to find the tuple
            // 

            StringBuilder cmdTxt = new StringBuilder("exec sp_executesql N'delete [{0}]".Frmt(schema.TableName));

            // Build Where clause if keys are defined
            cmdTxt.Append(" where ");

            int count = 0, paramIndex = 0;
            foreach (KeyMapping map in schema.Keys.Values)
            {
                cmdTxt.Append("[{0}] = @{1}".Frmt(map.To, paramIndex++));

                if ((count + 1) < schema.Keys.Count)
                    cmdTxt.Append(" and ");

                count++;
            }


            cmdTxt.Append("', N'");


            //
            // Set parameter indexes and types,                                                     @0 varchar(max), @1 int, ...
            //

            paramIndex = 0;
            foreach (KeyMapping map in schema.Keys.Values)
            {
                Type propertyType = objRepresentor.GetProperty(map.From).PropertyType;

                cmdTxt.Append("@{0} {1}, ".Frmt(paramIndex++, ObjectMapper.s_ClrTypeToSqlTypeMapper[propertyType]));
            }

            cmdTxt.Remove(cmdTxt.Length - 2, 2);    // Remove last
            cmdTxt.Append("', ");   // Close quote and add comma



            //
            // Set parameter indexes and data
            //

            paramIndex = 0;
            foreach (KeyMapping map in schema.Keys.Values)
            {
                PropertyInfo pi = objRepresentor.GetProperty(map.From);
                String valueTxt = ObjectMapper.PrepareValue(pi.GetValue(obj, null));         // Can contain quotes, based on property type

                cmdTxt.Append("@{0} = {1}, ".Frmt(paramIndex++, valueTxt));
            }

            cmdTxt.Remove(cmdTxt.Length - 2, 2); // Remove last ,
            return cmdTxt.ToString();
        }
    }
}
