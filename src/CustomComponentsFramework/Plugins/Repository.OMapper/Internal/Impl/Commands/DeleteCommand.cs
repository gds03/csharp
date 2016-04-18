using Repository.OMapper.Interfaces;
using Repository.OMapper.Types;
using Repository.OMapper.Types.Mappings;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using Repository.OMapper.Extensions;
using Repository.OMapper.Internal.Converters;

namespace Repository.OMapper.Internal.Commands.Impl
{
    internal partial class CommandsForTypeSchema : ISqlCommandTextGenerator
    {
        /// <summary>
        ///     Creates a SQL string that will represent Delete statement.
        ///     This method will use parameterized queries.
        /// </summary>
        /// <param name="obj">The object that delete command is being build from.</param>
        /// <returns>The SQL Command</returns>
        public String DeleteCommand(object obj)
        {
            Debug.Assert(obj != null);

            Type objRepresentor = OMapperCRUDSupportBase.GetTypeFor(obj);

            //
            // Obtain local copy because another thread can change the reference of _typesSchema
            // and we need iterate in a secure way.
            //

            TypeSchema schema = OMapper.s_TypesSchemaMapper[objRepresentor];

            if (schema.Keys.Count == 0)
                throw new InvalidOperationException("Type {0} must have at least one key for deleting".Frmt(objRepresentor.Name));


            //
            // Delete only if we have keys, to find the tuple
            // 

            StringBuilder cmdTxt = new StringBuilder(4000).Append("exec sp_executesql N'delete [{0}]".Frmt(schema.TableName));

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

                cmdTxt.Append("@{0} {1}, ".Frmt(paramIndex++, OMapperCRUDSupportBase.s_ClrToSqlTypesMapper[propertyType]));
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
                String valueTxt = ValueToSQLConverter.Convert(pi.GetValue(obj, null));         // Can contain quotes, based on property type

                cmdTxt.Append("@{0} = {1}, ".Frmt(paramIndex++, valueTxt));
            }

            cmdTxt.Remove(cmdTxt.Length - 2, 2); // Remove last ,
            return cmdTxt.ToString();
        }
    }
}
