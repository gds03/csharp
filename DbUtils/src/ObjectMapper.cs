using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Reflection;
using System.Threading;
using System.Text;
using System.Data;
using System.Linq.Expressions;


namespace DbMapper
{

    #region Mapper Attributes (public)

    [Flags]
    public enum SPMode
    {
        Insert = 1,
        Update = 2,
        Delete = 4
    }

    public sealed class StoredProc : Attribute
    {
        internal String ParameterName;
        internal SPMode Mode;

        public StoredProc(SPMode mode)
        {
            Mode = mode;
        }

        public StoredProc(SPMode mode, String name) : this(mode)
        {
            ParameterName = name;
        }
    }

    public sealed class Identity : Attribute
    {

    }

    public sealed class Table : Attribute
    {
        internal String OverridedName;

        internal Table(String tableName)
        {
            OverridedName = tableName;
        }
    }

    public sealed class Key : Attribute
    {

    }

    public sealed class Exclude : Attribute
    {

    }

    public sealed class BindFrom : Attribute
    {
        internal String OverridedReadColumn;

        public BindFrom(String sqlColumnResult)
        {
            OverridedReadColumn = sqlColumnResult;
        }
    }

    public sealed class BindTo : Attribute
    {
        internal String OverridedSqlColumn;

        public BindTo(String sqlColumnSchema)
        {
            OverridedSqlColumn = sqlColumnSchema;
        }
    }


    #endregion



    #region Mapper Exceptions (public)

    public sealed class SqlColumnNotFoundException : Exception
    {
        internal SqlColumnNotFoundException(string msg) : base(msg) { }
    }

    public sealed class PropertyMustBeNullable : Exception
    {
        internal PropertyMustBeNullable(string msg) : base(msg) { }
    }

    #endregion



    #region Mapper Extensions (public)



    internal static class StringExtensions
    {
        public static String Frmt(this String str, params object[] args)
        {
            return String.Format(str, args);
        }
    }


    #endregion




    public class ObjectMapper
    {



        private readonly DbConnection Connection;      // The only Instance variable




        #region Static Fields

        private static readonly BindingFlags Flags = BindingFlags.Public | BindingFlags.Instance;



        // 
        // For specific type, stores the properties that must be mapped from SQL
        // (Accessed in context of multiple threads)
        private static volatile Dictionary<Type, TypeSchema> TypesSchema = new Dictionary<Type, TypeSchema>();     

        // Map expressionType (LINQ expression nodes to strings (e.g && -> AND, || -> OR, etc..)
        private static readonly Dictionary<ExpressionType, String> ExpressionOperator = new Dictionary<ExpressionType, string>();


        #endregion





        static ObjectMapper()
        {
            ExpressionOperator.Add(ExpressionType.AndAlso, "AND");
            ExpressionOperator.Add(ExpressionType.Equal, "=");
            ExpressionOperator.Add(ExpressionType.GreaterThan, ">");
            ExpressionOperator.Add(ExpressionType.GreaterThanOrEqual, ">=");
            ExpressionOperator.Add(ExpressionType.LessThan, "<");
            ExpressionOperator.Add(ExpressionType.LessThanOrEqual, "<=");
            ExpressionOperator.Add(ExpressionType.Modulo, "%");
            ExpressionOperator.Add(ExpressionType.Multiply, "*");
            ExpressionOperator.Add(ExpressionType.NotEqual, "<>");
            ExpressionOperator.Add(ExpressionType.OrElse, "OR");
            ExpressionOperator.Add(ExpressionType.Subtract, "-");
            ExpressionOperator.Add(ExpressionType.Add, "+");


        }







        #region Mapper Protected Classes

        protected sealed class TypeSchema
        {
            internal String TableName;                  // If != null overrides the type name (used for CUD operations)
            internal IList<KeyMapping> Keys;            // Stores the keys of the type (to uniquelly identify the one entity)
            internal IList<CostumMapping> Mappings;     // For each property, we have a costum mapping
            internal IList<ProcMapping> Procedures;     // Stores parameters that must be used when ExecuteProc command is executed to send them to Stored Procedures
            internal String IdentityPropertyName;       // If != null, this stores the property of the type that is identity
            

            internal TypeSchema()
            {
                Mappings    = new List<CostumMapping>();
                Keys        = new List<KeyMapping>();
                Procedures  = new List<ProcMapping>();
            }

            internal TypeSchema(String tableName)
                : this()
            {
                TableName = tableName;
            }
        }

        protected sealed class CostumMapping
        {
            internal String ClrProperty;
            internal String ToSqlTableColumn;
            internal String FromResultSetColumn;

            internal CostumMapping(String clrProperty)
            {
                // Initially all points to the name of the clrProperty (convention is used)
                FromResultSetColumn = ToSqlTableColumn = ClrProperty = clrProperty;
            }
        }

        protected sealed class KeyMapping
        {
            internal String From;
            internal String To;            

            public KeyMapping(String to, String from)
            {
                To = to;
                From = from;
            }
        }

        // Map CLR property type to a stored procedure parameter
        protected sealed class ProcMapping
        {
            internal KeyMapping Map;        
            internal SPMode Mode;

            internal ProcMapping(String clrProperty, SPMode mode)
            {
                // Initially points to the name of the clrProperty (convention is used)
                Map = new KeyMapping(clrProperty, clrProperty);
                Mode = mode;
            }
        }


        private sealed class ExpressionUtilFilterClass
        {
            internal Type ParameterType;
            internal String Data;
        }


        #endregion




        #region Static Auxiliary Methods


        private static String PrepareColumnType(PropertyInfo pi, object obj)
        {
            object value = pi.GetValue(obj, null);

            if ( value == null )
                return "NULL";

            if ( pi.PropertyType == typeof(bool) )
                return ( (bool)value ) ? "'1'" : "'0'";

            if ( pi.PropertyType == typeof(DateTime) || pi.PropertyType == typeof(Nullable<DateTime>) )
            {
                DateTime d = (DateTime)value;
                return "convert(datetime, '{0}', 105)".Frmt(d);
            }


            // return default
            return "'" + value.ToString() + "'";
        }

        // Used by ConfigureMetadataFor
        private static Dictionary<Type, TypeSchema> NewCopyWithAddedTypeSchema(Type type)
        {
            // Copy last dictionary and add new Schema for type (local for each thread)
            var result = new Dictionary<Type, TypeSchema>(TypesSchema) { { type, new TypeSchema() } };

            // Set table name (By convention have the same name that the type)
            result[type].TableName = type.Name;

            // Search for Table attribute on the type
            foreach ( object o in type.GetCustomAttributes(false) )
            {
                Table t = o as Table;

                if ( t != null )
                {
                    result[type].TableName = t.OverridedName;       // override the default name
                    break;                                          // We are done.
                }
            }


            // Iterate over each property of the type
            foreach ( PropertyInfo pi in type.GetProperties(Flags) )
            {
                bool mapProperty = true;                                // Always to map, unless specified Exclude costum attribute
                bool isKey = false;                                     // Only if attribute were found, sets this flag to true
                bool isIdentity = false;                                // For each type, we must have only one Entity

                CostumMapping mapVar = new CostumMapping(pi.Name);      // By convention all mappings match the propertyName


                // Iterate over each attribute on context property
                foreach ( object o in pi.GetCustomAttributes(false) )
                {
                    if ( o is Exclude )
                    {
                        mapProperty = false;
                        break;                  // break immediately and don't map this property
                    }

                    Key k = o as Key;

                    if ( k != null )
                    {
                        isKey = true;
                        continue;
                    }

                    Identity i = o as Identity;

                    if ( i != null )
                    {
                        isIdentity = true;
                        continue;
                    }

                    BindFrom bf = o as BindFrom;

                    if ( bf != null )
                    {
                        mapVar.FromResultSetColumn = bf.OverridedReadColumn;      // override read column behavior
                        continue;
                    }

                    BindTo bt = o as BindTo;

                    if ( bt != null )
                    {
                        mapVar.ToSqlTableColumn = bt.OverridedSqlColumn;          // override CUD behavior
                        continue;
                    }

                    StoredProc sp = o as StoredProc;

                    if ( sp != null )
                    {
                        ProcMapping pm = new ProcMapping(pi.Name, sp.Mode);

                        if ( sp.ParameterName != null )
                            pm.Map.To = sp.ParameterName;                        // override sp parameter

                        result[type].Procedures.Add(pm);
                        continue;
                    }
                }

                if ( mapProperty )
                {

                    //
                    // We are here if Exclude wasn't present on the property
                    // 

                    result[type].Mappings.Add(mapVar);

                    if ( isKey )
                    {
                        // Add on keys collection
                        result[type].Keys.Add(new KeyMapping(mapVar.ToSqlTableColumn, pi.Name));
                    }

                    if ( isIdentity )
                    {
                        //
                        // Only can exist one identity!
                        //

                        if ( result[type].IdentityPropertyName != null )
                            throw new InvalidOperationException("Type {0} cannot have multiple identity columns".Frmt(type.Name));

                        result[type].IdentityPropertyName = pi.Name;
                    }
                }
            }

            return result;
        }



        private static void ConfigureMetadataFor(Type type)
        {
            do
            {
                TypeSchema s;

                if ( TypesSchema.TryGetValue(type, out s) ) // Typically, this is the most common case to occur
                    break;

                //
                // Schema must be setted! - multiple threads can be here;
                // Threads can be here concurrently when a new type is added, 
                // or then 2 or more threads are setting the same type metadata
                // 

                Dictionary<Type, TypeSchema> backup = TypesSchema;     // Get a local copy for each thread.
                var newSchema = NewCopyWithAddedTypeSchema(type);      // Copy and add metadata for specific Type


                #pragma warning disable 420

                if ( TypesSchema == backup && Interlocked.CompareExchange(ref TypesSchema, newSchema, backup) == backup )
                    break;

                #pragma warning restore 420

            } while ( true );
        }

        private static String GetMappingForProperty(Type t, String propertyName)
        {
            TypeSchema schema = TypesSchema[t];

            foreach ( CostumMapping cm in schema.Mappings )
            {
                if ( cm.ClrProperty == propertyName )
                    return cm.ToSqlTableColumn;
            }

            throw new InvalidOperationException("shouldn't be here'");
        }

        private static IList<T> MapTo<T>(DbDataReader reader) {
            if (reader == null)
                throw new NullReferenceException("reader cannot be null");

            if (reader.IsClosed)
                throw new InvalidOperationException("reader connection is closed and objects cannot be mapped");

            if (!reader.HasRows)
                return new List<T>();


            Type type = typeof(T);
            TypeSchema schema = TypesSchema[type];

            //
            // If we are here, the properties for specific type are filled 
            // and never be touched (modified) again for the type.
            // 

            // Map cursor lines from database to CLR objects based on T

            List<T> bundle = new List<T>();

            while (reader.Read()) {
                T newInstance = (T)Activator.CreateInstance(type);
                Type newInstanceRep = newInstance.GetType();            // Mirror instance to reflect newInstance

                // Map properties to the newInstance
                foreach (CostumMapping map in schema.Mappings) {
                    object value;
                    string sqlColumn = map.FromResultSetColumn;

                    try { value = reader[sqlColumn]; }
                    catch (IndexOutOfRangeException) {
                        throw new SqlColumnNotFoundException("Sql column with name: {0} is not found".Frmt(sqlColumn));
                    }

                    PropertyInfo ctxProperty = newInstanceRep.GetProperty(map.ClrProperty);

                    //
                    // Nullable condition checker!
                    //

                    if (value.GetType() == typeof(DBNull)) {
                        if (ctxProperty.PropertyType.IsPrimitive) {
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
                bundle.Add(newInstance);
            }

            // Free Connection Resources
            reader.Close();
            reader.Dispose();

            return bundle;
        }





        #region Parser for Where Filter


        private static String ParseFilter(Expression expr)
        {
            return ParseFilter(expr, new ExpressionUtilFilterClass()).Data.ToString();
        }

        // Recursive algorithm
        private
        static
        ExpressionUtilFilterClass                            // Return type
        ParseFilter(
            Expression expr,
            ExpressionUtilFilterClass info          // Used to pass information in recursive manner
        )
        {

            Type exprType = expr.GetType();

            if ( exprType != typeof(BinaryExpression) && exprType != typeof(MemberExpression) && exprType != typeof(ConstantExpression) )
                throw new InvalidOperationException("unsupported filter");


            if ( exprType == typeof(BinaryExpression) )
            {
                //
                // We have 2 expressions (left and right)
                // 

                BinaryExpression bExpr = (BinaryExpression)expr;
                ExpressionUtilFilterClass recursion;

                StringBuilder subOperation = new StringBuilder();
                recursion = ParseFilter(bExpr.Left, info);              // Go left in depth - we don't know the type yet

                subOperation.Append("( ");
                subOperation.Append(recursion.Data);
                subOperation.Append(" ");

                subOperation.Append(ExpressionOperator[bExpr.NodeType]);
                subOperation.Append(" ");

                recursion = ParseFilter(bExpr.Right, recursion);               // Pass reference that contains type information!

                subOperation.Append(recursion.Data);
                subOperation.Append(" )");

                // Affect data subpart and pass to upper caller
                recursion.Data = subOperation.ToString();

                return recursion;
            }

            else
            {
                MemberExpression mExpr;
                ConstantExpression cExpr;

                //
                // We need distinct if we are accessing to capturated variables (need map to sql) or constant variables
                //

                if ( ( mExpr = expr as MemberExpression ) != null )
                {
                    if ( ( ( mExpr.Expression as ParameterExpression ) ) != null )
                    {
                        info.ParameterType = mExpr.Expression.Type;        // Type of parameter (must be untouched)
                        info.Data = GetMappingForProperty(info.ParameterType, mExpr.Member.Name);                     // Must have a map to SQL (criar metodo que faz mapeamento)!!!!!!!!!!!!!!!!!

                        return info;
                    }
                    else
                    {
                        cExpr = (ConstantExpression)mExpr.Expression;

                        object obj = cExpr.Value;               // Get anonymous object
                        string objField = mExpr.Member.Name;

                        FieldInfo value = obj.GetType().GetField(objField);  // Read native value
                        string nativeData = value.GetValue(obj).ToString();

                        info.Data = nativeData;
                        return info;
                    }
                }
                else
                {
                    cExpr = (ConstantExpression)expr;
                    string nativeData = cExpr.Value.ToString();

                    info.Data = nativeData;
                    return info;
                }
            }
        }



        #endregion




        #endregion




        #region Instance Auxiliary Methods


        private void SetupConnection() {
            if (Connection == null)
                throw new NullReferenceException("connection is null");

            if (Connection.State != ConnectionState.Open) {
                try {
                    Connection.Open();
                }
                catch (Exception) { throw new InvalidOperationException("connection is not in good conditions"); }
            }

        }


        #endregion




        #region Commands Dynamic SQL Preparers



        private static String PrepareSelectCmd<T>(Type type, Expression<Func<T, bool>> filter)
        {
            StringBuilder cmdTxt = new StringBuilder();

            //
            // Obtain local copy because another thread can change the reference of _typesSchema
            // and we need iterate in a secure way.
            //

            TypeSchema schema = TypesSchema[type];         // Get schema information for specific Type


            cmdTxt.Append("SELECT ");

            // Select all columns that are mapped
            foreach ( CostumMapping cm in schema.Mappings )
            {
                cmdTxt.Append(cm.FromResultSetColumn);
                cmdTxt.Append(", ");
            }

            cmdTxt.Remove(cmdTxt.Length - 2, 2); // Remove last ,
            cmdTxt.Append(" FROM {0} ".Frmt(schema.TableName));

            if ( filter != null )
            {
                //
                // Apply filter
                //

                cmdTxt.Append("WHERE ");
                String filtered = ParseFilter(filter.Body);

                cmdTxt.Append(filtered);
            }

            return cmdTxt.ToString();
        }

        private static String PrepareInsertCmd<T>(Type type, T obj, Type objRepresentor, string scopeIdentity)
        {
            StringBuilder cmdTxt = new StringBuilder();

            //
            // Obtain local copy because another thread can change the reference of _typesSchema
            // and we need iterate in a secure way.
            //

            TypeSchema schema = TypesSchema[type];         // Get schema information for specific Type

            cmdTxt.Append("INSERT INTO {0} (".Frmt(schema.TableName));


            // Build header (exclude identities)
            foreach ( CostumMapping cm in schema.Mappings )
            {
                if ( cm.ClrProperty == schema.IdentityPropertyName )
                    continue;

                cmdTxt.Append(cm.ToSqlTableColumn);
                cmdTxt.Append(", ");
            }

            cmdTxt.Remove(cmdTxt.Length - 2, 2); // Remove last ,
            cmdTxt.Append(") values (");

            // Build body (exclude identities)
            foreach ( CostumMapping cm in schema.Mappings )
            {
                if ( cm.ClrProperty == schema.IdentityPropertyName )
                    continue;

                PropertyInfo pi = objRepresentor.GetProperty(cm.ClrProperty);
                String valueTxt = PrepareColumnType(pi, obj);

                cmdTxt.Append(valueTxt);
                cmdTxt.Append(", ");
            }

            cmdTxt.Remove(cmdTxt.Length - 2, 2); // Remove last ,
            cmdTxt.Append(")");
            cmdTxt.Append(" select SCOPE_IDENTITY() as [{0}]".Frmt(scopeIdentity));

            return cmdTxt.ToString();
        }

        private static String PrepareUpdateCmd<T>(Type type, T obj)
        {
            Type objRepresentor = obj.GetType();

            //
            // Obtain local copy because another thread can change the reference of _typesSchema
            // and we need iterate in a secure way.
            //

            TypeSchema schema = TypesSchema[type];

            if ( schema.Keys.Count == 0 )
                throw new InvalidOperationException("Type {0} must have at least one key for updating".Frmt(type.Name));

            //
            // Update only if we have keys, to find the tuple
            // 

            StringBuilder cmdTxt = new StringBuilder();
            cmdTxt.Append("UPDATE {0} SET ".Frmt(schema.TableName));


            // Build Set clause
            foreach ( CostumMapping cm in schema.Mappings )
            {
                if ( cm.ClrProperty == schema.IdentityPropertyName )
                    continue;

                PropertyInfo pi = objRepresentor.GetProperty(cm.ClrProperty);
                String valueTxt = PrepareColumnType(pi, obj);

                cmdTxt.Append("{0} = {1}, ".Frmt(cm.ToSqlTableColumn, valueTxt));
            }

            cmdTxt.Remove(cmdTxt.Length - 2, 2); // Remove last ,

            // Build Where clause
            cmdTxt.Append(" WHERE ");

            int count = 0;
            foreach ( KeyMapping map in schema.Keys )
            {
                PropertyInfo pi = objRepresentor.GetProperty(map.From);
                String valueTxt = PrepareColumnType(pi, obj);

                cmdTxt.Append("{0} = {1}".Frmt(map.To, valueTxt));

                if ( ( count + 1 ) < schema.Keys.Count )
                    cmdTxt.Append(" AND ");

                count++;
            }

            return cmdTxt.ToString();
        }

        private static String PrepareDeleteCmd<T>(Type type, T obj)
        {
            Type objRepresentor = obj.GetType();

            //
            // Obtain local copy because another thread can change the reference of _typesSchema
            // and we need iterate in a secure way.
            //

            TypeSchema schema = TypesSchema[type];

            if ( schema.Keys.Count == 0 )
                throw new InvalidOperationException("Type {0} must have at least one key for deleting".Frmt(type.Name));


            //
            // Delete only if we have keys, to find the tuple
            // 

            StringBuilder cmdTxt = new StringBuilder();
            cmdTxt.Append("DELETE FROM {0}".Frmt(schema.TableName));


            // Build Where clause if keys are defined
            cmdTxt.Append(" WHERE ");

            int count = 0;
            foreach ( KeyMapping map in schema.Keys )
            {
                PropertyInfo pi = objRepresentor.GetProperty(map.From);
                String valueTxt = PrepareColumnType(pi, obj);

                cmdTxt.Append("{0} = {1}".Frmt(map.To, valueTxt));

                if ( ( count + 1 ) < schema.Keys.Count )
                    cmdTxt.Append(" AND ");

                count++;
            }

            return cmdTxt.ToString();
        }



        #endregion






        public ObjectMapper(DbConnection connection)
        {
            if ( connection == null )
                throw new NullReferenceException("connection cannot be null");

            Connection = connection;
        }







        #region Public Interface



        /// <summary>
        ///     Maps the result set to a list of T by convention, and leave the possibility to pass the commandType, commandText and DbParameters.
        /// </summary>
        /// <typeparam name="T">The type of the object that you want to get</typeparam>
        /// <param name="commandType">The type of the command</param>
        /// <param name="commandText">If using stored procedure, must be the stored procedure name, otherwise the dynamic sql</param>
        /// <param name="parameters">The parameters that command use. (optional)</param>
        /// <returns>A list of T objects with their properties filled that aren't annotated with [Exclude]</returns>
        public IList<T> Select<T>(CommandType commandType, string commandText, params DbParameter[] parameters)
        {
            Type type = typeof(T);

            // Lock-Free
            ConfigureMetadataFor(type);

            //
            // If we are here, the properties for specific type are filled 
            // and never be touched (modified) again for the type.
            // 

            DbCommand comm = Connection.CreateCommand();

            comm.CommandType = commandType;
            comm.CommandText = commandText;

            // Set parameters
            if ( parameters != null )
                comm.Parameters.AddRange(parameters);

            // Open connection if not opened
            SetupConnection();  
            return MapTo<T>(comm.ExecuteReader());
        }



        /// <summary>
        ///     Maps the table from database to a list of T by convention.
        /// </summary>
        /// <typeparam name="T">The type of the object that you want to get</typeparam>
        /// <returns>A list of T objects with their properties filled that aren't annotated with [Exclude]</returns>
        public IList<T> Select<T>() 
        {
            return Select<T>(null);
        }


        /// <summary>
        ///     Maps the table tuples from database to a list of T by convention that satisfy the filter.
        /// </summary>
        /// <typeparam name="T">The type of the object that you want to get</typeparam>
        /// <param name="filter">The filter that you must use to filter a sub part of the result</param>
        /// <returns>A list of T objects with their properties filled that aren't annotated with [Exclude]</returns>
        public IList<T> Select<T>(Expression<Func<T, bool>> filter)
        {
            Type type = typeof(T);

            // Lock-Free
            ConfigureMetadataFor(type);

            //
            // If we are here, the properties for specific type are filled 
            // and never be touched (modified) again for the type.
            // 

            String selectCmd = PrepareSelectCmd(type, filter);
            DbCommand cmd = Connection.CreateCommand();

            cmd.CommandType = CommandType.Text;     // dynamic SQL
            cmd.CommandText = selectCmd;

            // Open connection if not opened
            SetupConnection();  
            return MapTo<T>(cmd.ExecuteReader());
        }





        /// <summary>
        ///     Inserts the object on database and update the identity property in CLR object (if annotated with)
        ///     The property annotated with Identity Attribute is ignored on insert command.
        /// </summary>
        /// <typeparam name="T">The type of the object that you want to insert</typeparam>
        /// <param name="obj">The object that you want to insert</param>
        /// <returns>The number of affected rows in database</returns>
        public int Insert<T>(T obj)
        {
            Type type = typeof(T);
            Type objRepresentor = obj.GetType();

            string scopy_id_name = "Scope_Identity";

            // Lock-Free
            ConfigureMetadataFor(type); 

            //
            // If we are here, the properties for specific type are filled 
            // and never be touched (modified) again for the type.
            // 

            TypeSchema schema = TypesSchema[type];

            String insertCmd = PrepareInsertCmd(type, obj, objRepresentor, scopy_id_name);
            DbCommand cmd = Connection.CreateCommand();

            cmd.CommandType = CommandType.Text;
            cmd.CommandText = insertCmd;

            // Open connection if not opened
            SetupConnection();  

            // Execute query and if identity defined execute scalar sql command
            if(schema.IdentityPropertyName == null)
                return cmd.ExecuteNonQuery();
                        
            //
            // The type have identity column and we must set the identity to instance of the object
            //

            // Execute scalar query
            object scope_identity;

            if( (scope_identity = cmd.ExecuteScalar()) == null )
                return 0;
            
            // Set scope_identity to object property identity
            PropertyInfo pi = objRepresentor.GetProperty(schema.IdentityPropertyName);

            // Convert type from db to property type
            object converted_identity = Convert.ChangeType(scope_identity, pi.PropertyType);

            // Set property identity
            pi.SetValue(obj, converted_identity, null);

            return 1;
        }



        /// <summary>
        ///     Based on primary key of the type, update the object on database
        /// </summary>
        /// <typeparam name="T">The type of the object that you want to update. Note: This type must be annotated with [Key]</typeparam>
        /// <param name="obj">The object that you want to update</param>
        /// <returns>The number of affected rows in database</returns>
        public int Update<T>(T obj)
        {
            Type type = typeof(T);

            // Lock-Free
            ConfigureMetadataFor(type); 

            //
            // If we are here, the properties for specific type are filled 
            // and never be touched (modified) again for the type.
            // 

            String updateCmd = PrepareUpdateCmd(type, obj);
            DbCommand cmd = Connection.CreateCommand();

            cmd.CommandType = CommandType.Text;
            cmd.CommandText = updateCmd;

            // Open connection if not opened
            SetupConnection();  
            return cmd.ExecuteNonQuery();
        }




        /// <summary>
        ///     Based on primary key of the type, delete the object from database
        /// </summary>
        /// <typeparam name="T">The type of the object that you want to delete. Note: This type must be annotated with [Key]</typeparam>
        /// <param name="obj">The object that you want to delete</param>
        /// <returns>The number of affected rows in database</returns>
        public int Delete<T>(T obj)
        {
            Type type = typeof(T);

            // Lock-Free
            ConfigureMetadataFor(type);

            //
            // If we are here, the properties for specific type are filled 
            // and never be touched (modified) again for the type.
            // 

            String deleteCmd = PrepareDeleteCmd(type, obj);
            DbCommand cmd = Connection.CreateCommand();

            cmd.CommandType = CommandType.Text;
            cmd.CommandText = deleteCmd;

            // Open connection if not opened
            SetupConnection();  
            return cmd.ExecuteNonQuery();
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
        public int ExecuteProc<T>(T obj, SPMode mode, string procedureName)
        {
            Type type = typeof(T);

            // Lock-Free
            ConfigureMetadataFor(type);



            //
            // If we are here, the properties for specific type are filled 
            // and never be touched (modified) again for the type.
            // 


            DbCommand cmd = Connection.CreateCommand();

            cmd.CommandType = CommandType.StoredProcedure;      // Procedure
            cmd.CommandText = procedureName;

            //

            Type objRepresentor = obj.GetType();
            TypeSchema schema = TypesSchema[type];

            foreach ( ProcMapping pm in schema.Procedures )
            {
                if ( ( pm.Mode & mode ) == mode )      // Mode match!
                {
                    object value = objRepresentor.GetProperty(pm.Map.From, Flags).GetValue(obj, null);
                    object value2 = value == null ? DBNull.Value : value;

                    cmd.Parameters.Add(new SqlParameter(pm.Map.To, value2));
                }
            }

            // Open connection if not opened
            SetupConnection();  
            return cmd.ExecuteNonQuery();
        }


        /// <summary>
        ///     Execute the query against database.
        /// </summary>
        /// <param name="commandType">The type of the command</param>
        /// <param name="commandText">If using stored procedure, must be the stored procedure name, otherwise the dynamic sql</param>
        /// <param name="parameters">The parameters that command use. (optional)</param>
        /// <returns>The number of affected rows in database</returns>
        public int Execute(CommandType commandType, string commandText, params DbParameter[] parameters) {
            DbCommand comm = Connection.CreateCommand();

            comm.CommandType = commandType;
            comm.CommandText = commandText;

            // Set parameters
            if (parameters != null)
                comm.Parameters.AddRange(parameters);

            // Open connection if not opened
            SetupConnection();  
            return comm.ExecuteNonQuery();

        }


        #endregion


    }
}