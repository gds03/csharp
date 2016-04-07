using Repository.ObjectMapper.Interfaces;
using Repository.ObjectMapper.Providers;
using Repository.ObjectMapper.Types;
using Repository.ObjectMapper.Types.Mappings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Repository.ObjectMapper.Internal.Commands.Impl
{
    internal partial class CommandsForTypeSchema : CommandsForTypeSchemaBase, ISqlCommandTextGenerator
    {
        /// <summary>
        ///     Creates a SQL string that will represent Select statement.
        ///     This method will use parameterized queries.
        /// </summary>
        /// <typeparam name="T">The type of object being mapped.</typeparam>
        /// <param name="type">The object type</param>
        /// <param name="filter">The predicate to apply on where condition.</param>
        /// <returns>The SQL Command</returns>
        public string SelectCommand<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            StringBuilder cmdTxt = new StringBuilder();

            //
            // Obtain local copy because another thread can change the reference of _typesSchema
            // and we need iterate in a secure way.
            //

            TypeSchema schema = ObjectMapper.s_TypesSchemaMapper[typeof(T)];         // Get schema information for specific Type


            cmdTxt.Append("select ");

            // Select all columns that are mapped
            foreach (ColumnMapping cm in schema.Columns.Values)
            {
                cmdTxt.Append("[{0}], ".Frmt(cm.FromResultSetColumn));
            }

            cmdTxt.Remove(cmdTxt.Length - 2, 2); // Remove last ,
            cmdTxt.Append(" from [{0}] ".Frmt(schema.TableName));

            if (predicate != null)
            {
                //
                // Apply filter
                //

                cmdTxt.Append("where ");
                String filtered = PredicateParserProvider.Current.ParseFilter(predicate.Body);

                cmdTxt.Append(filtered);
            }

            return cmdTxt.ToString();
        }
    }
}
