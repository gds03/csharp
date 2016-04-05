
//
// Released at: 17 February 2012
// Author: Goncalo Dias
//
//
// Last updated date: 05-04-2016
//



using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Reflection;
using System.Threading;
using System.Text;
using System.Data;
using Repository.ObjectMapper.Attributes;
using Repository.ObjectMapper.Exceptions;
using Repository.ObjectMapper.Types;
using System.Linq.Expressions;
using System.Diagnostics;
using Repository.ObjectMapper.Types.Mappings;

namespace Repository.ObjectMapper
{

    public class ObjectMapper : IDisposable
    {

        #region Instance Fields




        //
        // member fields

        bool m_disposed;
        readonly DbConnection m_connection;
        readonly DbTransaction m_transaction;
        readonly int m_commandTimeout = 30;

        // queue for commands
        readonly List<InsertCommand> m_insertCommandsQueue   = new List<InsertCommand>();
        readonly List<object> m_updateObjectsQueue           = new List<object>();
        readonly List<object> m_deleteObjectQueue            = new List<object>();





        #endregion




        #region Static Fields

        internal static readonly BindingFlags s_bindingflags = BindingFlags.Public | BindingFlags.Instance;

        const int SchemaInitCapacity = 53;
        const int OperatorsInitCapacity = 23;
        const int ClrTypesMappingCapacity = 47;



        // 
        // For specific type, stores the properties that must be mapped from SQL
        // (Accessed in context of multiple threads)
        private static volatile Dictionary<Type, TypeSchema> s_TypesToMetadataMapper = new Dictionary<Type, TypeSchema>(SchemaInitCapacity);

        // Map expressionType (LINQ expression nodes to strings (e.g && -> AND, || -> OR, etc..)
        private static readonly Dictionary<ExpressionType, String> s_ExpressionMapper = new Dictionary<ExpressionType, string>(OperatorsInitCapacity);

        // Maps .CLR types to SQL types
        private static readonly Dictionary<Type, String> s_ClrTypeToSqlTypeMapper = new Dictionary<Type, string>(ClrTypesMappingCapacity);

        // helper classes
        class InsertCommand
        {
            public String CommandText { get; private set; }

            public object Object { get; private set; }

            public InsertCommand(string insertCmd, object obj)
            {
                if (string.IsNullOrEmpty(insertCmd))
                    throw new ArgumentException("insertCmd");

                if (obj == null)
                    throw new ArgumentNullException("obj");

                this.CommandText = insertCmd;
                this.Object = obj;
            }
        }



        #endregion




        #region Constructors


        static ObjectMapper()
        {
            SetExpressionOperator();
            SetClrToSqlConversions();
        }



        /// <summary>
        ///     Initialize ObjectMapper with specified connectionString and with a default command timeout of 30 seconds
        /// </summary>
        /// <param name="connectionString"></param>
        public ObjectMapper(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException("connectionString");

            m_connection = new SqlConnection(connectionString);
        }


        /// <summary>
        ///     Initialize ObjectMapper with specified connection and with a default command timeout of 30 seconds
        /// </summary>
        /// <param name="connection"></param>
        public ObjectMapper(DbConnection connection, DbTransaction transaction = null)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");

            m_connection = connection;
            m_transaction = transaction;
        }


        /// <summary>
        ///     Initialize ObjectMapper with specified connection and with specified command timeout
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="commandTimeout"></param>
        public ObjectMapper(DbConnection connection, int commandTimeout, DbTransaction transaction = null)
            : this(connection, transaction)
        {
            m_commandTimeout = commandTimeout;
        }


        ~ObjectMapper()
        {
            Dispose(false);
        }



        #endregion





     




        #region Static Auxiliary Methods



        private static void SetExpressionOperator()
        {
            s_ExpressionMapper.Add(ExpressionType.AndAlso, "AND");
            s_ExpressionMapper.Add(ExpressionType.Equal, "=");
            s_ExpressionMapper.Add(ExpressionType.GreaterThan, ">");
            s_ExpressionMapper.Add(ExpressionType.GreaterThanOrEqual, ">=");
            s_ExpressionMapper.Add(ExpressionType.LessThan, "<");
            s_ExpressionMapper.Add(ExpressionType.LessThanOrEqual, "<=");
            s_ExpressionMapper.Add(ExpressionType.Modulo, "%");
            s_ExpressionMapper.Add(ExpressionType.Multiply, "*");
            s_ExpressionMapper.Add(ExpressionType.NotEqual, "<>");
            s_ExpressionMapper.Add(ExpressionType.OrElse, "OR");
            s_ExpressionMapper.Add(ExpressionType.Subtract, "-");
            s_ExpressionMapper.Add(ExpressionType.Add, "+");
        }

        private static void SetClrToSqlConversions()
        {
            s_ClrTypeToSqlTypeMapper.Add(typeof(Boolean), "bit");
            s_ClrTypeToSqlTypeMapper.Add(typeof(Byte), "tinyint");
            s_ClrTypeToSqlTypeMapper.Add(typeof(Int16), "smallint");
            s_ClrTypeToSqlTypeMapper.Add(typeof(Int32), "int");
            s_ClrTypeToSqlTypeMapper.Add(typeof(Int64), "bigint");
            s_ClrTypeToSqlTypeMapper.Add(typeof(Decimal), "decimal");
            s_ClrTypeToSqlTypeMapper.Add(typeof(Single), "float");
            s_ClrTypeToSqlTypeMapper.Add(typeof(Double), "float");
            s_ClrTypeToSqlTypeMapper.Add(typeof(Enum), "int");
            s_ClrTypeToSqlTypeMapper.Add(typeof(Char), "smallint");
            s_ClrTypeToSqlTypeMapper.Add(typeof(String), "nvarchar(max)");
            s_ClrTypeToSqlTypeMapper.Add(typeof(Char[]), "nvarchar(max)");
            s_ClrTypeToSqlTypeMapper.Add(typeof(DateTime), "datetime");
            s_ClrTypeToSqlTypeMapper.Add(typeof(DateTimeOffset), "datetime2");
            s_ClrTypeToSqlTypeMapper.Add(typeof(TimeSpan), "time");
            s_ClrTypeToSqlTypeMapper.Add(typeof(Byte[]), "image");
            s_ClrTypeToSqlTypeMapper.Add(typeof(Guid), "uniqueidentifier");
            s_ClrTypeToSqlTypeMapper.Add(typeof(Object), "sql_variant");

            // For nullables
            s_ClrTypeToSqlTypeMapper.Add(typeof(Boolean?), "bit");
            s_ClrTypeToSqlTypeMapper.Add(typeof(Byte?), "tinyint");
            s_ClrTypeToSqlTypeMapper.Add(typeof(Int16?), "smallint");
            s_ClrTypeToSqlTypeMapper.Add(typeof(Int32?), "int");
            s_ClrTypeToSqlTypeMapper.Add(typeof(Int64?), "bigint");
            s_ClrTypeToSqlTypeMapper.Add(typeof(Decimal?), "decimal");
            s_ClrTypeToSqlTypeMapper.Add(typeof(Single?), "float");
            s_ClrTypeToSqlTypeMapper.Add(typeof(Double?), "float");
            s_ClrTypeToSqlTypeMapper.Add(typeof(Char?), "smallint");
            s_ClrTypeToSqlTypeMapper.Add(typeof(DateTime?), "datetime");
            s_ClrTypeToSqlTypeMapper.Add(typeof(DateTimeOffset?), "datetime2");
            s_ClrTypeToSqlTypeMapper.Add(typeof(TimeSpan?), "time");
            s_ClrTypeToSqlTypeMapper.Add(typeof(Guid?), "uniqueidentifier");
        }

        /// <summary>
        ///  Converts value into string correctly formatted.
        /// </summary>
        private static String PrepareValue(object value)
        {
            if (value == null)
                return "NULL";

            // We must know the concrete type
            Type type = value.GetType();

            if (type == typeof(bool))
                return ((bool)value) ? "1" : "0";

            if (type == typeof(DateTime))
            {
                DateTime d = (DateTime)value;
                return "'" + d.ToString("yyyy-MM-dd HH:mm:ss") + "'";
            }

            if (type == typeof(Nullable<DateTime>))
            {
                DateTime? dn = (DateTime?)value;

                if (dn.HasValue)
                {
                    return "'" + dn.Value.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                }
                else
                {
                    return dn.ToString();
                }
            }

            if (type == typeof(Guid) || type == typeof(String) || type == typeof(Char) || type == typeof(Char[]))
                return "'" + value.ToString() + "'";


            // return normal
            return value.ToString();
        }

        /// <summary>
        ///     Create TypeSchema object representing the type to not use reflection all the time.
        /// </summary>
        private static TypeSchema CreateSchema(Type type)
        {
            TypeSchema newSchema = new TypeSchema(type);
            
            // Search for Table attribute on the type
            foreach (object o in type.GetCustomAttributes(true))
            {
                Table t = o as Table;

                if (t != null)
                {
                    newSchema.TableName = t.OverridedName;       // override the default name
                    break;                                          // We are done.
                }
            }

            // Iterate over each property of the type
            foreach (PropertyInfo pi in type.GetProperties(s_bindingflags))
            {
                bool mapProperty = true;                                    // Always to map, unless specified Exclude costum attribute
                bool isPrimaryKey = false;                                  // Only if attribute were found, sets this flag to true
                bool isIdentity = false;                                    // For each type, we must have only one Entity

                ColumnMapping columnMapping = new ColumnMapping(pi.Name);      // By convention all mappings match the propertyName


                // Iterate over each attribute on context property
                foreach (object o in pi.GetCustomAttributes(false))
                {
                    if (o is Exclude)
                    {
                        mapProperty = false;
                        break;                                              // break immediately the inner loop - and don't map this property
                    }

                    PrimaryKey k = o as PrimaryKey;

                    if (k != null)
                    {
                        isPrimaryKey = true;
                        continue;
                    }

                    Identity i = o as Identity;

                    if (i != null)
                    {
                        isIdentity = true;
                        continue;
                    }

                    BindFrom bf = o as BindFrom;

                    if (bf != null)
                    {
                        columnMapping.FromResultSetColumn = bf.OverridedReadColumn;      // override read column behavior
                        continue;
                    }

                    BindTo bt = o as BindTo;

                    if (bt != null)
                    {
                        columnMapping.ToSqlTableColumn = bt.OverridedSqlColumn;          // override CUD behavior
                        continue;
                    }

                    StoredProc sp = o as StoredProc;

                    if (sp != null)
                    {
                        ProcMapping pm = new ProcMapping(pi.Name, sp.Mode);

                        if (sp.ParameterName != null)
                            pm.Map.To = sp.ParameterName;                            // override sp parameter

                        newSchema.Procedures.Add(pi.Name, pm);
                        continue;
                    }
                }

                if (mapProperty)
                {

                    //
                    // We are here if Exclude wasn't present on the property
                    // 

                    newSchema.Columns.Add(pi.Name, columnMapping);

                    if (isPrimaryKey)
                    {
                        // Add on keys collection
                        newSchema.Keys.Add(pi.Name, new KeyMapping(columnMapping.ToSqlTableColumn, pi.Name));
                    }

                    if (isIdentity)
                    {
                        //
                        // Only can exist one identity!
                        //

                        if (newSchema.IdentityPropertyName != null)
                            throw new InvalidOperationException("Type {0} cannot have multiple identity columns".Frmt(type.Name));

                        newSchema.IdentityPropertyName = pi.Name;
                    }
                }
            }

            return newSchema;            
        }



        // Used by ConfigureMetadataFor
        private static Dictionary<Type, TypeSchema> NewCopyWithAddedTypeSchema(Type type)
        {

            TypeSchema schema = ObjectMapper.CreateSchema(type);
            return new Dictionary<Type, TypeSchema>(s_TypesToMetadataMapper) { { type, schema } };
        }



        /// <summary>
        ///     Loads or adds metadata (Schema) to the static dictionary that holds all the information related to all types handled by this type.
        /// </summary>
        /// <param name="type">The type that will cause metadata to be loaded or added.</param>
        private static void ConfigureMetadataFor(Type type)
        {
            Debug.Assert(type != null);

            do
            {
                TypeSchema s;

                if (s_TypesToMetadataMapper.TryGetValue(type, out s)) // Typically, this is the most common case to occur
                    break;

                //
                // Schema must be setted! - multiple threads can be here;
                // Threads can be here concurrently when a new type is added, 
                // or then 2 or more threads are setting the same type metadata
                // 

                Dictionary<Type, TypeSchema> backup = s_TypesToMetadataMapper;                   // Get a local copy for each thread.
                var newSchema = ObjectMapper.NewCopyWithAddedTypeSchema(type);                   // Copy and add metadata for specific Type


#pragma warning disable 420

                if (s_TypesToMetadataMapper == backup && Interlocked.CompareExchange(ref s_TypesToMetadataMapper, newSchema, backup) == backup)
                    break;

#pragma warning restore 420

            }
            while (true);
        }


        /// <summary>
        ///     Get SQL column mapping to the respective propertyName.
        /// </summary>
        private static String GetMappingForProperty(Type t, String propertyName)
        {
            TypeSchema schema = s_TypesToMetadataMapper[t];
            Debug.Assert(schema != null);

            ColumnMapping cm = schema.Columns[propertyName];
            Debug.Assert(cm != null);

            return cm.ToSqlTableColumn;
        }

        private static IList<T> MapTo<T>(DbDataReader reader, bool CloseDbReader = true) where T : class
        {
            if (reader == null)
                throw new NullReferenceException("reader cannot be null");

            if (reader.IsClosed)
                throw new InvalidOperationException("reader connection is closed and objects cannot be mapped");

            if (!reader.HasRows)
                return new List<T>();


            Type type = typeof(T);
            TypeSchema schema = s_TypesToMetadataMapper[type];

            //
            // If we are here, the properties for specific type are filled 
            // and never be touched (modified) again for the type.
            // 

            // Map cursor lines from database to CLR objects based on T

            List<T> objectsQueue = new List<T>();

            while (reader.Read())
            {
                T newInstance = (T)Activator.CreateInstance(type);
                Type newInstanceRep = newInstance.GetType();            // Mirror instance to reflect newInstance

                // Map properties to the newInstance
                foreach (ColumnMapping map in schema.Columns.Values)
                {
                    object value;
                    string sqlColumn = map.FromResultSetColumn;

                    try { value = reader[sqlColumn]; }
                    catch (IndexOutOfRangeException)
                    {
                        throw new SqlColumnNotFoundException("Sql column with name: {0} is not found".Frmt(sqlColumn));
                    }

                    PropertyInfo ctxProperty = newInstanceRep.GetProperty(map.ClrProperty);

                    //
                    // Nullable condition checker!
                    //

                    if (value.GetType() == typeof(DBNull))
                    {
                        if (ctxProperty.PropertyType.IsPrimitive)
                        {
                            throw new PropertyMustBeNullable(
                                "Property {0} must be nullable for mapping a null value".Frmt(ctxProperty.Name)
                            );
                        }

                        value = null;
                    }



                    //
                    // Set property value
                    // 


                    ctxProperty.SetValue(newInstance, value, null);     // WARNING: Conversion Types..
                }

                // Add element to the collection
                objectsQueue.Add(newInstance);
            }

            if (CloseDbReader)
            {
                // Free Connection Resources
                reader.Close();
                reader.Dispose();
            }

            T[] arrayData = new T[objectsQueue.Count];
            objectsQueue.CopyTo(arrayData, 0);

            return arrayData;
        }





        #region Parser for Where Filter


        // Recursive algorithm
        private static String ParseFilter(Expression expr)
        {
            //
            // This is a simple filter, we only want (and parse) simple expressions.
            // 

            BinaryExpression bExpr;
            MemberExpression mExpr;
            ConstantExpression cExpr;

            if ((bExpr = expr as BinaryExpression) != null)
            {
                //
                // We have 2 expressions (left and right)
                // We get the first left result use the operator and get the right part 
                // (that can contain another BinaryExpression For example)
                // 

                StringBuilder innerText = new StringBuilder();
                String recursiveResult;

                recursiveResult = ObjectMapper.ParseFilter(bExpr.Left);                 // Go left

                innerText.Append("( ");
                innerText.Append(recursiveResult);
                innerText.Append(" ");

                innerText.Append(s_ExpressionMapper[bExpr.NodeType]);      // Map node types to SQL operators
                innerText.Append(" ");

                recursiveResult = ObjectMapper.ParseFilter(bExpr.Right);               // Go right

                innerText.Append(recursiveResult);
                innerText.Append(" )");

                return innerText.ToString();
            }

            if ((cExpr = expr as ConstantExpression) != null)
            {
                return ObjectMapper.PrepareValue(cExpr.Value);
            }

            if ((mExpr = expr as MemberExpression) == null)
                throw new NotSupportedException("unsupported filter");

            // expr is of MemberExpression type            
            Expression innerExpr = mExpr.Expression;

            ParameterExpression pInnerExpr;
            MemberExpression mInnerExpr;
            ConstantExpression cInnerExpr;
            BinaryExpression bInnerExpr;

            if ((bInnerExpr = innerExpr as BinaryExpression) != null)
                return ObjectMapper.ParseFilter(bInnerExpr);                                          // Go recursive    

            if ((pInnerExpr = innerExpr as ParameterExpression) != null)
            {
                return ObjectMapper.GetMappingForProperty(pInnerExpr.Type, mExpr.Member.Name);        // We must map property of the type to SQL Column
            }

            if ((cInnerExpr = innerExpr as ConstantExpression) != null)
            {
                object obj = cInnerExpr.Value;                                           // Get anonymous object (captured by the compiler)

                FieldInfo value = obj.GetType().GetField(mExpr.Member.Name);             // Get the field of the anonymous object 
                return ObjectMapper.PrepareValue(value.GetValue(obj));
            }

            if ((mInnerExpr = innerExpr as MemberExpression) == null)
                throw new NotSupportedException("unsupported filter");

            // innerExpr is MemberExpression!
            if ((cInnerExpr = mInnerExpr.Expression as ConstantExpression) == null)
                throw new NotSupportedException("unsupported filter");

            object objValue = cInnerExpr.Value;                                                     // Get anonymous object (captured by the compiler)

            FieldInfo fieldValue = objValue.GetType().GetField(mInnerExpr.Member.Name);             // Get the field of the anonymous object 
            object obj2 = fieldValue.GetValue(objValue);                                            // Get the object in the field

            return ObjectMapper.PrepareValue(obj2.GetType().GetProperty(mExpr.Member.Name).GetValue(obj2, null));       // Based on the object, finally get the value in the property 
        }



        #endregion




        #endregion




        #region Instance Auxiliary Methods


        /// <summary>
        ///   Free the DbConnection associated with the ObjectMapper  
        /// </summary>
        private void Dispose(bool explicitlyCalled)
        {
            if (m_disposed)
                throw new ObjectDisposedException(typeof(ObjectMapper).Name);

            if (explicitlyCalled)
            {
                // get rid of managed resources
                if (m_connection != null)
                {
                    if (m_connection.State != ConnectionState.Closed)
                        m_connection.Close();

                    m_connection.Dispose();
                }

                m_disposed = true;
            }

            // get rid of unmanaged resources            

            //
            // When an object is executing its finalization code, it should not reference other objects, because finalizers do not execute in any particular order. 
            // If an executing finalizer references another object that has already been finalized, the executing finalizer will fail.
        }


        private void OpenConnection()
        {
            if (m_connection == null)
                throw new NullReferenceException("connection is null");

            // Try open the connection if not opened!
            if (m_connection.State != ConnectionState.Open)
                m_connection.Open();
        }

        private void CloseConnection()
        {
            if (m_connection == null)
                throw new NullReferenceException("connection is null");

            // Try open the connection if not opened!
            if (m_connection.State != ConnectionState.Closed)
                m_connection.Close();
        }


        private DbCommand CmdForConnection(CommandType type, String text)
        {
            DbCommand comm = m_connection.CreateCommand();

            comm.CommandType = type;
            comm.CommandText = text;
            comm.CommandTimeout = m_commandTimeout;
            comm.Transaction = m_transaction;

            return comm;
        }



        /// <summary>
        ///     Cleanup commands that are on hold.
        /// </summary>        
        private void ClearnupCommands()
        {
            m_insertCommandsQueue.Clear();
            m_updateObjectsQueue.Clear();
            m_deleteObjectQueue.Clear();
        }

        /// <summary>
        ///     Create a DB command and executes against database.
        ///     If the object that belongs to the insertCmd has Identity it will be updated.
        /// </summary>
        /// <returns>true if has identity and was updated, otherwise return false.</returns>
        private bool ExecuteInsert_UpdateIdentity(InsertCommand insertCmd)
        {
            Debug.Assert(insertCmd != null);
            Debug.Assert(!string.IsNullOrEmpty(insertCmd.CommandText));
            Debug.Assert(insertCmd.Object != null);

            Type insertObjRepresentor = insertCmd.Object.GetType();
            TypeSchema schema = s_TypesToMetadataMapper[insertObjRepresentor];

            // Command Setup parameters
            DbCommand cmd = CmdForConnection(CommandType.Text, insertCmd.CommandText);

            // If Identity is not setted, execute the query and ignore the rest
            if (schema.IdentityPropertyName == null)
            {
                cmd.ExecuteNonQuery();
                return false;
            }

            //
            // The type have identity column and we must set the identity to instance of the object
            //

            // Execute scalar query
            object scope_identity;

            if ((scope_identity = cmd.ExecuteScalar()) == null)
                throw new InvalidOperationException("{0}:{1} is not an identify column in database".Frmt(insertObjRepresentor.Name, schema.IdentityPropertyName));

            // Set scope_identity to object property identity
            PropertyInfo pi = insertObjRepresentor.GetProperty(schema.IdentityPropertyName);

            // Convert type from db to property type
            object converted_identity = Convert.ChangeType(scope_identity, pi.PropertyType);

            // Set property identity
            pi.SetValue(insertCmd.Object, converted_identity, null);
            return true;
        }



        private bool ExecuteUpdate(string updateCmd)
        {
            Debug.Assert(!string.IsNullOrEmpty(updateCmd));

            // Command Setup parameters
            DbCommand cmd = CmdForConnection(CommandType.Text, updateCmd);
            return cmd.ExecuteNonQuery() > 0;
        }




        private bool ExecuteDelete(string deleteCmd)
        {
            Debug.Assert(!string.IsNullOrEmpty(deleteCmd));
            // Command Setup parameters
            DbCommand cmd = CmdForConnection(CommandType.Text, deleteCmd);
            return cmd.ExecuteNonQuery() > 0;
        }



        #endregion




        #region Commands Dynamic SQL Preparers



        private static String PrepareSelectCmd<T>(Type type, Expression<Func<T, bool>> filter) where T : class
        {
            StringBuilder cmdTxt = new StringBuilder();

            //
            // Obtain local copy because another thread can change the reference of _typesSchema
            // and we need iterate in a secure way.
            //

            TypeSchema schema = s_TypesToMetadataMapper[type];         // Get schema information for specific Type


            cmdTxt.Append("select ");

            // Select all columns that are mapped
            foreach (ColumnMapping cm in schema.Columns.Values)
            {
                cmdTxt.Append("[{0}], ".Frmt(cm.FromResultSetColumn));
            }

            cmdTxt.Remove(cmdTxt.Length - 2, 2); // Remove last ,
            cmdTxt.Append(" from [{0}] ".Frmt(schema.TableName));

            if (filter != null)
            {
                //
                // Apply filter
                //

                cmdTxt.Append("where ");
                String filtered = ObjectMapper.ParseFilter(filter.Body);

                cmdTxt.Append(filtered);
            }

            return cmdTxt.ToString();
        }

        private static String PrepareInsertCmd<T>(Type type, T obj, Type objRepresentor, string scopeIdentity) where T : class
        {

            //
            // Obtain local copy because another thread can change the reference of _typesSchema
            // and we need iterate in a secure way.
            //

            TypeSchema schema = s_TypesToMetadataMapper[type];         // Get schema information for specific Type


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
            cmdTxt.Append(" select SCOPE_IDENTITY() as [{0}]', N'".Frmt(scopeIdentity));


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
                cmdTxt.Append("@{0} {1}, ".Frmt(paramIndex++, s_ClrTypeToSqlTypeMapper[propertyType]));    // Map CLR property to SqlColumn Type 
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

        private static String PrepareUpdateCmd<T>(Type type, T obj) where T : class
        {
            Type objRepresentor = obj.GetType();

            //
            // Obtain local copy because another thread can change the reference of _typesSchema
            // and we need iterate in a secure way.
            //

            TypeSchema schema = s_TypesToMetadataMapper[type];

            if (schema.Keys.Count == 0)
                throw new InvalidOperationException("Type {0} must have at least one key for updating".Frmt(type.Name));

            //
            // Update only if we have keys, to find the tuple
            // 

            StringBuilder cmdTxt = new StringBuilder("exec sp_executesql N'update [{0}] set ".Frmt(schema.TableName));


            // Build Set clause
            int paramIndex = 0;
            foreach (ColumnMapping cm in schema.Columns.Values)
            {
                if (cm.ClrProperty == schema.IdentityPropertyName)                            // Identity Column never's updated!
                    continue;

                cmdTxt.Append("[{0}] = @{1}, ".Frmt(cm.ToSqlTableColumn, paramIndex++));        // [Column] = @0, [Column2] = @1 ...
            }


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
                if (cm.ClrProperty == schema.IdentityPropertyName)                                          // Identity Column never's updated!
                    continue;

                // set sql type based on property type of the object
                Type propertyType = objRepresentor.GetProperty(cm.ClrProperty).PropertyType;
                cmdTxt.Append("@{0} {1}, ".Frmt(paramIndex++, s_ClrTypeToSqlTypeMapper[propertyType]));    // Map CLR property to SqlColumn Type 
            }

            // Set the types of parameters for where region
            foreach (KeyMapping map in schema.Keys.Values)
            {

                // set sql type based on property type of the object
                Type propertyType = objRepresentor.GetProperty(map.From).PropertyType;
                cmdTxt.Append("@{0} {1}, ".Frmt(paramIndex++, s_ClrTypeToSqlTypeMapper[propertyType]));    // Map CLR property to SqlColumn Type 
            }

            cmdTxt.Remove(cmdTxt.Length - 2, 2);    // Remove last
            cmdTxt.Append("', ");   // Close quote and add comma


            //
            // Set data of parameters in set region
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

            // Set data of parameters in where region
            foreach (KeyMapping map in schema.Keys.Values)
            {
                PropertyInfo pi = objRepresentor.GetProperty(map.From);
                String valueTxt = ObjectMapper.PrepareValue(pi.GetValue(obj, null));                          // Can contain quotes, based on property type

                cmdTxt.Append("@{0} = {1}, ".Frmt(paramIndex++, valueTxt));
            }


            cmdTxt.Remove(cmdTxt.Length - 2, 2); // Remove last ,
            return cmdTxt.ToString();
        }

        private static String PrepareDeleteCmd<T>(Type type, T obj) where T : class
        {
            Type objRepresentor = obj.GetType();

            //
            // Obtain local copy because another thread can change the reference of _typesSchema
            // and we need iterate in a secure way.
            //

            TypeSchema schema = s_TypesToMetadataMapper[type];

            if (schema.Keys.Count == 0)
                throw new InvalidOperationException("Type {0} must have at least one key for deleting".Frmt(type.Name));


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

                cmdTxt.Append("@{0} {1}, ".Frmt(paramIndex++, s_ClrTypeToSqlTypeMapper[propertyType]));
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
                String valueTxt = PrepareValue(pi.GetValue(obj, null));         // Can contain quotes, based on property type

                cmdTxt.Append("@{0} = {1}, ".Frmt(paramIndex++, valueTxt));
            }

            cmdTxt.Remove(cmdTxt.Length - 2, 2); // Remove last ,
            return cmdTxt.ToString();
        }



        #endregion








        #region Public Interface



        /// <summary>
        ///     Maps the result set to a list of T by convention, and leave the possibility to pass the commandType, commandText and DbParameters.
        /// </summary>
        /// <typeparam name="T">The type of the object that you want to Map</typeparam>
        /// <param name="commandType">The type of the command</param>
        /// <param name="commandText">If using stored procedure, must be the stored procedure name, otherwise the dynamic sql</param>
        /// <param name="parameters">The parameters that command use. (optional)</param>
        /// <returns>A list of objects with their properties filled that aren't annotated with [Exclude] attribute</returns>
        public IList<T> Select<T>(CommandType commandType, string commandText, params DbParameter[] parameters) where T : class
        {
            // Lock-Free
            ConfigureMetadataFor(typeof(T));

            //
            // If we are here, the properties for specific type are filled 
            // and never be touched (modified) again for the type.
            // 

            // Command Setup parameters
            DbCommand comm = CmdForConnection(commandType, commandText);

            // Set parameters
            if (parameters != null && parameters.Length > 0)
                comm.Parameters.AddRange(parameters);

            // Open connection if not opened
            OpenConnection();
            return ObjectMapper.MapTo<T>(comm.ExecuteReader());
        }


        /// <summary>
        ///     Maps the result sets to the array of objects where index 0 contains the first result set, and index 1, the second result set.
        /// </summary>
        /// <typeparam name="T1">The type of the object that you want to Map in the first result set</typeparam>
        /// <typeparam name="T2">The type of the object that you want to Map in the second result set</typeparam>
        /// <param name="commandType">The type of the command</param>
        /// <param name="commandText">If using stored procedure, must be the stored procedure name, otherwise the dynamic sql</param>
        /// <param name="parameters">The parameters that command use. (optional)</param>
        /// <returns>An array of objects with 2 positions, where the first position contains a list of T1 and the second position with a list of T2 objects.</returns>
        public object[] Select<T1, T2>(CommandType commandType, string commandText, params DbParameter[] parameters)
            where T1 : class
            where T2 : class
        {
            // Lock-Free
            ConfigureMetadataFor(typeof(T1));
            ConfigureMetadataFor(typeof(T2));

            //
            // If we are here, the properties for specific type are filled 
            // and never be touched (modified) again for the type.
            // 

            // Command Setup parameters
            DbCommand comm = CmdForConnection(commandType, commandText);

            // Set parameters
            if (parameters != null && parameters.Length > 0)
                comm.Parameters.AddRange(parameters);

            // Open connection if not opened
            OpenConnection();

            // Allocate memory
            object[] result = new object[2];
            DbDataReader reader;

            result[0] = ObjectMapper.MapTo<T1>(reader = comm.ExecuteReader(), false);
            reader.NextResult();
            result[1] = ObjectMapper.MapTo<T2>(reader);

            // return reference
            return result;
        }




        /// <summary>
        ///     Maps the result sets to the array of objects where index 0 contains the first result set, index 1 the second result set and index 2, the third result set.
        /// </summary>
        /// <typeparam name="T1">The type of the object that you want to Map in the first result set</typeparam>
        /// <typeparam name="T2">The type of the object that you want to Map in the second result set</typeparam>
        /// <typeparam name="T3">The type of the object that you want to Map in the third result set</typeparam>
        /// <param name="commandType">The type of the command</param>
        /// <param name="commandText">If using stored procedure, must be the stored procedure name, otherwise the dynamic sql</param>
        /// <param name="parameters">The parameters that command use. (optional)</param>
        /// <returns>An array of objects with 3 positions, where the first position contains a list of T1, the second position with a list of T2, and the third position with a list of T3 objects.</returns>
        public object[] Select<T1, T2, T3>(CommandType commandType, string commandText, params DbParameter[] parameters)
            where T1 : class
            where T2 : class
            where T3 : class
        {
            // Lock-Free
            ConfigureMetadataFor(typeof(T1));
            ConfigureMetadataFor(typeof(T2));
            ConfigureMetadataFor(typeof(T3));

            //
            // If we are here, the properties for specific type are filled 
            // and never be touched (modified) again for the type.
            // 

            // Command Setup parameters
            DbCommand comm = CmdForConnection(commandType, commandText);

            // Set parameters
            if (parameters != null && parameters.Length > 0)
                comm.Parameters.AddRange(parameters);

            // Open connection if not opened
            OpenConnection();

            // Allocate memory
            object[] result = new object[3];
            DbDataReader reader;

            result[0] = ObjectMapper.MapTo<T1>(reader = comm.ExecuteReader(), false);
            reader.NextResult();
            result[1] = ObjectMapper.MapTo<T2>(reader, false);
            reader.NextResult();
            result[2] = ObjectMapper.MapTo<T3>(reader);

            // return reference
            return result;
        }







        /// <summary>
        ///     Maps the table from database to a list of T by convention.
        /// </summary>
        /// <typeparam name="T">The type of the object that you want to Map</typeparam>
        /// <returns>A list of objects with their properties filled that aren't annotated with [Exclude] attribute</returns>
        public IList<T> Select<T>() where T : class
        {
            return Select<T>(null);
        }


        /// <summary>
        ///     Maps the table from database to a list of T by convention that satisfy the filter.
        /// </summary>
        /// <typeparam name="T">The type of the object that you want to Map</typeparam>
        /// <param name="filter">The filter that you must use to filter a sub part of the result</param>
        /// <returns>A list of objects with their properties filled that aren't annotated with [Exclude] attribute</returns>
        public IList<T> Select<T>(Expression<Func<T, bool>> filter) where T : class
        {
            Type type = typeof(T);

            // Lock-Free
            ConfigureMetadataFor(type);

            //
            // If we are here, the properties for specific type are filled 
            // and never be touched (modified) again for the type.
            // 

            // Prepare select statement for type
            String selectCmd = PrepareSelectCmd(type, filter);

            // Command Setup parameters
            DbCommand cmd = CmdForConnection(CommandType.Text, selectCmd);

            // Open connection if not opened
            OpenConnection();
            return ObjectMapper.MapTo<T>(cmd.ExecuteReader());
        }





        /// <summary>
        ///     Inserts the object on database and update the identity property in CLR object (if annotated with)
        ///     The property annotated with Identity Attribute is ignored on insert command.
        /// </summary>
        /// <typeparam name="T">The type of the object that you want to insert</typeparam>
        /// <param name="obj">The object that you want to insert</param>
        public void Insert<T>(T obj) where T : class
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            Type objRepresentor = obj.GetType();

            string scopy_id_name = "Scope_Identity";

            // Lock-Free
            ConfigureMetadataFor(objRepresentor);

            //
            // If we are here, the properties for specific type are filled 
            // and never be touched (modified) again for the type.
            // 

            TypeSchema schema = s_TypesToMetadataMapper[objRepresentor];

            // Prepare insert statement for type
            String insertCmd = PrepareInsertCmd(objRepresentor, obj, objRepresentor, scopy_id_name);
            m_insertCommandsQueue.Add(new InsertCommand(insertCmd, obj));
        }


        /// <summary>
        ///     Based on primary key of the type, insert collection of objects passed by parameter in database.
        /// </summary>
        /// <param name="objects">The objects that you want to insert</param>
        public void InsertMany(params object[] objects)
        {
            if (objects != null)
            {
                foreach (object o in objects)
                    this.Insert(o);
            }
        }

        /// <summary>
        ///     Based on primary key of the type, update the object on database
        /// </summary>
        /// <typeparam name="T">The type of the object that you want to update. Note: This type must be annotated with [Key]</typeparam>
        /// <param name="obj">The object that you want to update</param>
        public void Update<T>(T obj) where T : class
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            // Lock-Free
            ConfigureMetadataFor(obj.GetType());

            //
            // If we are here, the properties for specific type are filled 
            // and never be touched (modified) again for the type.
            // 

            m_updateObjectsQueue.Add(obj);
        }



        /// <summary>
        ///     Based on primary key of the type, update collection of objects passed by parameter in database.
        /// </summary>
        /// <param name="objects">The objects that you want to update</param>
        public void UpdateMany(params object[] objects)
        {
            if (objects != null)
            {
                foreach (object o in objects)
                    this.Update(o);
            }
        }




        /// <summary>
        ///     Based on primary key of the type, delete the object from database
        /// </summary>
        /// <typeparam name="T">The type of the object that you want to delete. Note: This type must be annotated with [Key]</typeparam>
        /// <param name="obj">The object that you want to delete</param>
        /// <returns>The number of affected rows in database</returns>
        public void Delete<T>(T obj) where T : class
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            ConfigureMetadataFor(obj.GetType());

            //
            // If we are here, the properties for specific type are filled 
            // and never be touched (modified) again for the type.
            // 

            m_deleteObjectQueue.Add(obj);
        }

        /// <summary>
        ///     Based on primary key of the type, delete collection of objects passed by parameter in database.
        /// </summary>
        /// <param name="objects">The objects that you want to delete</param>
        public void DeleteMany(params object[] objects)
        {
            if (objects != null)
            {
                foreach (object o in objects)
                    this.Delete(o);
            }
        }





        public int Submit()
        {
            int operationsPerformed = 0;

            // Open connection if not opened
            OpenConnection();

            for (int i = 0; i < m_insertCommandsQueue.Count; i++)
            {
                InsertCommand insertCmd = this.m_insertCommandsQueue[i];
                bool insertResult = ExecuteInsert_UpdateIdentity(insertCmd);
                operationsPerformed++;

                if (!insertResult)
                    continue;
            }

            for (int i = 0; i < m_updateObjectsQueue.Count; i++)
            {
                object @objUpdate = this.m_updateObjectsQueue[i];
                
                // Prepare update statement for type
                String updateCmd = PrepareUpdateCmd(objUpdate.GetType(), @objUpdate);
                bool updateResult = ExecuteUpdate(updateCmd);

                if( updateResult )
                    operationsPerformed++;

            }

            for (int i = 0; i < m_deleteObjectQueue.Count; i++)
            {
                object @objDelete = this.m_deleteObjectQueue[i];

                // Prepare delete statement for type
                String deleteCmd = PrepareDeleteCmd(@objDelete.GetType(), @objDelete);
                bool deleteResult = ExecuteDelete(deleteCmd);

                if (deleteResult)
                    operationsPerformed++;
            }

            CloseConnection();
            ClearnupCommands();
            return operationsPerformed;

        }


        /// <summary>
        ///     Build DbParameters that match with the mode and execute the stored procedure.
        ///     All stored procedures must have the same name of parameters for all modes when using this method.
        /// </summary>
        /// <typeparam name="T">The type that must have their properties annotated with [StoredProc]</typeparam>
        /// <param name="obj">The object that you want to retrieve the information and build sql parameters dynamically based on their values</param>
        /// <param name="mode">The mode of the procedure</param>
        /// <param name="procedureName">The name of the procedure</param>
        /// <returns>The number of affected rows in database</returns>
        public int ExecuteProc<T>(T obj, SPMode mode, string procedureName) where T : class
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            Type type = obj.GetType();
            ConfigureMetadataFor(type);


            //
            // If we are here, the properties for specific type are filled 
            // and never be touched (modified) again for the type.
            // 

            // Command Setup parameters
            DbCommand cmd = CmdForConnection(CommandType.StoredProcedure, procedureName);


            //

            Type objRepresentor = obj.GetType();
            TypeSchema schema = s_TypesToMetadataMapper[type];

            foreach (ProcMapping pm in schema.Procedures.Values)
            {
                if ((pm.Mode & mode) == mode)      // Mode match!
                {
                    object value = objRepresentor.GetProperty(pm.Map.From, s_bindingflags).GetValue(obj, null);
                    object value2 = value == null ? DBNull.Value : value;

                    cmd.Parameters.Add(new SqlParameter(pm.Map.To, value2));
                }
            }

            // Open connection if not opened
            OpenConnection();
            return cmd.ExecuteNonQuery();
        }


        /// <summary>
        ///     Execute the query on the database.
        /// </summary>
        /// <param name="commandType">The type of the command</param>
        /// <param name="commandText">If using stored procedure, must be the stored procedure name, otherwise the dynamic sql</param>
        /// <param name="parameters">The parameters that command use. (optional)</param>
        /// <returns>The number of affected rows in database</returns>
        public int Execute(CommandType commandType, string commandText, params DbParameter[] parameters)
        {
            if (string.IsNullOrEmpty(commandText))
                throw new ArgumentException("commandText");

            // Command Setup parameters
            DbCommand comm = CmdForConnection(commandType, commandText);

            // Set parameters
            if (parameters != null && parameters.Length > 0)
                comm.Parameters.AddRange(parameters);

            // Open connection if not opened
            OpenConnection();
            return comm.ExecuteNonQuery();
        }


        /// <summary>
        ///     Execute the query on the database.
        /// </summary>
        /// <param name="commandType">The type of the command</param>
        /// <param name="commandText">If using stored procedure, must be the stored procedure name, otherwise the dynamic sql</param>
        /// <param name="parameters">The parameters that command use. (optional)</param>
        /// <returns>The first column of the first row in the ResultSet returned by the query</returns>
        public object ExecuteScalar(CommandType commandType, string commandText, params DbParameter[] parameters)
        {
            if (string.IsNullOrEmpty(commandText))
                throw new ArgumentException("commandText");

            // Command Setup parameters
            DbCommand comm = CmdForConnection(commandType, commandText);

            // Set parameters
            if (parameters != null && parameters.Length > 0)
                comm.Parameters.AddRange(parameters);

            // Open connection if not opened
            OpenConnection();
            return comm.ExecuteScalar();
        }



        /// <summary>
        ///   Free the DbConnection associated with the ObjectMapper  
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }





        /// <summary>
        ///     Returns the connection that ObjectMapper is associated
        /// </summary>
        public DbConnection Connection
        {
            get { return m_connection; }
        }


        #endregion

    }




    internal static class StringExtensions
    {
        internal static string Frmt(this String str, params object[] objs)
        {
            return string.Format(str, objs);
        }
    }
}