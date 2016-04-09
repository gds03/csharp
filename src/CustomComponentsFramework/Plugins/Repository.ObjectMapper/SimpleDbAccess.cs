using Repository.OMapper.Attributes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace Repository.OMapper
{
    public class SimpleDbAccess
    {
        public string ConnectionString { get; internal set; }



        public SimpleDbAccess(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException("connectionString");

            this.ConnectionString = connectionString;
        }



        public TResult ExecuteBlock<TResult>(Dictionary<string, object> parameters, Func<SqlCommand, Exception, TResult> userFunc)
        {
            return ExecuteBlock(parameters, null, userFunc);
        }


        public TResult ExecuteBlock<TResult>(Dictionary<string, object> parameters, SqlTransaction transaction, Func<SqlCommand, Exception, TResult> userFunc)
        {
            SqlConnection connection = new SqlConnection(ConnectionString);

            try
            {
                connection.Open();
                SqlCommand cmd = connection.CreateCommand();

                if (transaction != null)
                    cmd.Transaction = transaction;		// use command within transaction

                // fill up command parameters
                if (parameters != null && parameters.Count > 0)
                {

                    foreach (var kvp in parameters)
                    {
                        string ParamName = kvp.Key;
                        object ParamValue = kvp.Value;

                        SqlParameter p = new SqlParameter(ParamName, ParamValue == null ? DBNull.Value : (object)ParamValue);
                        cmd.Parameters.Add(p);
                    }
                }

                // invoke user func with parameters filled
                return userFunc(cmd, null);
            }

            catch (Exception ex)
            {
                if (userFunc != null)
                {
                    return userFunc(null, ex);
                }

                else throw;
            }

            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }




        public static List<T> MapTo<T>(SqlDataReader reader)
        {
            LinkedList<T> result = new LinkedList<T>();
            string[] propertiesName = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public).Select(x => x.Name).ToArray();

            // For each line in query result set
            while (reader.Read())
            {
                // create T object
                T newT = Activator.CreateInstance<T>();

                // Set up all properties in newT object
                foreach (var propertyName in propertiesName)
                {
                    // assumes to search by convention
                    string ColumnToSearch = propertyName;

                    // verify if user wants to search a specific column instead
                    Exclude[] myIgnoreAttributes = (Exclude[])newT.GetType().GetProperty(propertyName).GetCustomAttributes(typeof(Exclude), true);

                    if (myIgnoreAttributes == null || myIgnoreAttributes.Length == 0)
                    {
                        // verify if user wants to search a specific column instead
                        BindFrom[] myAttributes = (BindFrom[])newT.GetType().GetProperty(propertyName).GetCustomAttributes(typeof(BindFrom), true);

                        if (myAttributes != null)
                        {
                            if (myAttributes.Length > 1)
                                throw new InvalidOperationException("Only ony MappingAttribute can be setted in each property");

                            if (myAttributes.Length == 1)
                                ColumnToSearch = myAttributes[0].OverridedReadColumn;
                        }

                        // get the value
                        object value = reader[ColumnToSearch];

                        if (string.IsNullOrEmpty(value.ToString()))
                            value = string.Empty;

                        // set value in newT object
                        newT.GetType().GetProperty(propertyName).SetValue(newT, value, null);

                    }
                }

                result.AddLast(newT);
            }

            return result.ToList();
        }
    }
}
