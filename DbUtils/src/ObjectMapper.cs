using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;
using System.Threading;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Linq.Expressions;


namespace DbUtils
{

    #region Mapper Attributes (public)

    
    public sealed class Identity : Attribute
    {
        
    }

    public sealed class Table : Attribute
    {
        internal String overridedName;

        internal Table(String tableName)
        {
            overridedName = tableName;
        }
    }

    public sealed class Key : Attribute
    {
        
    }

    public sealed class Exclude : Attribute
    {

    }

    public sealed class SelectMapping : Attribute
    {
        internal String overridedReadColumn;

        public SelectMapping(String sqlColumn)
        {
            overridedReadColumn = sqlColumn;
        }
    }

    public sealed class BindTo : Attribute
    {
        internal String _connectedTo;

        public BindTo(String toSqlColumn)
        {
            _connectedTo = toSqlColumn;
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
        public static String FRMT(this String str, params object[] args)
        {
            return String.Format(str, args);
        }
    }


    #endregion


    public class ObjectMapper
    {



        private readonly DbConnection _connection;      // The only Instance variable




        #region Static Fields

        private static readonly BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
        
                   


        // For specific type, stores the properties that must be mapped from SQL
        private static volatile Dictionary<Type, TypeSchema> _typesSchema =
                            new Dictionary<Type, TypeSchema>();     // Accessed in context of multiple threads


        private static Dictionary<ExpressionType, String> _expressionOperator = new Dictionary<ExpressionType, string>();



        static ObjectMapper()
        {
            _expressionOperator.Add(ExpressionType.AndAlso, "AND");
            _expressionOperator.Add(ExpressionType.Equal, "=");
            _expressionOperator.Add(ExpressionType.GreaterThan, ">");
            _expressionOperator.Add(ExpressionType.GreaterThanOrEqual, ">=");
            _expressionOperator.Add(ExpressionType.LessThan, "<");
            _expressionOperator.Add(ExpressionType.LessThanOrEqual, "<=");
            _expressionOperator.Add(ExpressionType.Modulo, "%");
            _expressionOperator.Add(ExpressionType.Multiply, "*");
            _expressionOperator.Add(ExpressionType.NotEqual, "<>");
            _expressionOperator.Add(ExpressionType.OrElse, "OR");
            _expressionOperator.Add(ExpressionType.Subtract, "-");
            _expressionOperator.Add(ExpressionType.Add, "+");


        }


        #endregion




        #region Mapper Private Classes

        private sealed class TypeSchema
        {
            internal String TableName;                  // If != null overrides the type name (used for CUD operations)
            internal IList<KeyMapping> Keys;            // Stores the keys of the type
            internal IList<CostumMapping> Mappings;     // For each property, we have a costum mapping
            internal String IdentityProperty;           // If != null, this stores the property of the type that is identity

            internal TypeSchema()
            {
                Mappings = new List<CostumMapping>();
                Keys = new List<KeyMapping>();
            }

            internal TypeSchema(String tableName)
                : this()
            {
                TableName = tableName;
            }
        }

        private sealed class CostumMapping
        {
            internal String FromSelectColumn;
            internal String ClrProperty;
            internal String BindedToColumn;

            internal CostumMapping(String clrProperty)
            {
                FromSelectColumn = BindedToColumn = ClrProperty = clrProperty;
            }
        }

        private sealed class KeyMapping
        {
            internal String SqlColumn;
            internal String ClrProperty;

            public KeyMapping(String sqlColumn, String clrProperty)
            {
                SqlColumn = sqlColumn;
                ClrProperty = clrProperty;
            }
        }

        private sealed class ExpressionUtilFilterClass
        {
            internal Type parameterType;
            internal String data;
        }


        #endregion




        #region Static Auxiliary Methods


        private static Dictionary<Type, TypeSchema> NewCopyWithAddedTypeSchema(Type type)
        {
            // Copy last dictionary and add new Schema for type (local for each thread)
            var result = new Dictionary<Type, TypeSchema>(_typesSchema) { { type, new TypeSchema() } };

            // Set table name (By convention have the same name that the type)
            result[type].TableName = type.Name;

            // Search for Table attribute on the type
            foreach ( object o in type.GetCustomAttributes(false) )
            {
                Table t = o as Table;

                if ( t != null ) {
                    result[type].TableName = t.overridedName;       // override the default name
                    break;                                          // We are done.
                }
            }

            
            // Iterate over each property of the type set mappings references
            foreach ( PropertyInfo pi in type.GetProperties(flags) )
            {
                bool mapProperty = true;                                // Always to map, unless specified Exclude costum attribute
                bool isKey       = false;                               // Only if attribute were found, sets this flag to true
                bool isIdentity  = false;                               // For each type, we must have only one Entity

                CostumMapping mapVar = new CostumMapping(pi.Name);      // By convention all mappings match the propertyName


                // Iterate over each attribute on context property
                foreach ( object o in pi.GetCustomAttributes(false) ) {

                    if ( o is Exclude ) {
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

                    SelectMapping selectFrom = o as SelectMapping;

                    if ( selectFrom != null )
                    {
                        mapVar.FromSelectColumn = selectFrom.overridedReadColumn;      // override read column behavior
                        continue;
                    }

                    BindTo bt = o as BindTo;

                    if ( bt != null )
                    {
                        mapVar.BindedToColumn = bt._connectedTo;                        // override CUD behavior
                    }
                }

                if ( mapProperty ) {

                    //
                    // We are here if Exclude wasn't present on the property
                    // 

                    result[type].Mappings.Add(mapVar);

                    if ( isKey )
                    {
                        // Add on keys collection
                        result[type].Keys.Add(new KeyMapping(mapVar.BindedToColumn, pi.Name));
                    }

                    if ( isIdentity )
                    {
                        //
                        // Only can exist one identity!
                        //

                        if ( result[type].IdentityProperty != null )
                            throw new InvalidOperationException("Type {0} cannot have multiple identity columns".FRMT(type.Name));

                        result[type].IdentityProperty = pi.Name;
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

                if ( _typesSchema.TryGetValue(type, out s) ) // Typically, this is the most common case to occur
                    break;

                //
                // Schema must be setted! - multiple threads can be here.. 
                // (Altought isn't a commun case for the same type at the same time)
                // 

                Dictionary<Type, TypeSchema> backup = _typesSchema;     // Get a local copy for each thread.
                var newSchema = NewCopyWithAddedTypeSchema(type);       // Copy and add metadata for specific Type
                

                #pragma warning disable 420

                if ( _typesSchema == backup && Interlocked.CompareExchange(ref _typesSchema, newSchema, backup) == backup )
                    break;

                #pragma warning restore 420

            } while ( true );
        }

        private static String PrepareColumnType(PropertyInfo pi, object obj)
        {
            object value = pi.GetValue(obj, null);

            if(value == null)
                return "NULL";

            if ( pi.PropertyType == typeof(bool) )
                return ( (bool)value ) ? "'1'" : "'0'";

            if ( pi.PropertyType == typeof(DateTime) || pi.PropertyType == typeof(Nullable<DateTime>) )
            {
                DateTime d = (DateTime)value;
                return "convert(datetime, '{0}', 105)".FRMT(d);
            }


            // return default
            return "'" + value.ToString() + "'";
        }

        private static String GetMappingForProperty(Type t, String propertyName)
        {
            TypeSchema schema = _typesSchema[t];

            foreach ( CostumMapping cm in schema.Mappings )
            {
                if ( cm.ClrProperty == propertyName )
                    return cm.BindedToColumn;
            }

            throw new InvalidOperationException("shouldn't be here'");
        }

        private static String ParseFilter(Expression expr)
        {
            return ParseFilter(expr, new ExpressionUtilFilterClass()).data.ToString();
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
                subOperation.Append(recursion.data);
                subOperation.Append(" ");

                subOperation.Append(_expressionOperator[bExpr.NodeType]);
                subOperation.Append(" ");

                recursion = ParseFilter(bExpr.Right, recursion);               // Pass reference that contains type information!

                subOperation.Append(recursion.data);
                subOperation.Append(" )");

                // Affect data subpart and pass to upper caller
                recursion.data = subOperation.ToString();

                return recursion;
            }

            else
            {
                MemberExpression mExpr;
                ParameterExpression pExpr;
                ConstantExpression cExpr;

                //
                // We need distinct if we are accessing to capturated variables (need map to sql) or constant variables
                //

                if ( ( mExpr = expr as MemberExpression ) != null )
                {
                    if ( ( pExpr = ( mExpr.Expression as ParameterExpression ) ) != null )
                    {
                        info.parameterType = mExpr.Expression.Type;        // Type of parameter (must be untouched)
                        info.data = GetMappingForProperty(info.parameterType, mExpr.Member.Name);                     // Must have a map to SQL (criar metodo que faz mapeamento)!!!!!!!!!!!!!!!!!

                        return info;
                    }
                    else
                    {
                        cExpr = (ConstantExpression)mExpr.Expression;

                        object obj = cExpr.Value;               // Get anonymous object
                        string objField = mExpr.Member.Name;

                        FieldInfo value = obj.GetType().GetField(objField);  // Read native value
                        string nativeData = value.GetValue(obj).ToString();

                        info.data = nativeData;
                        return info;
                    }
                }
                else
                {
                    cExpr = (ConstantExpression)expr;
                    string nativeData = cExpr.Value.ToString();

                    info.data = nativeData;
                    return info;
                }
            }
        }


        


        #endregion












        #region Insert




        private static String PrepareInsertCmd<T>(Type type, T obj)
        {
            Type objRepresentor = obj.GetType();
            StringBuilder cmdTxt = new StringBuilder();

            //
            // Obtain local copy because another thread can change the reference of _typesSchema
            // and we need iterate in a secure way.
            //

            TypeSchema schema = _typesSchema[type];         // Get schema information for specific Type
            
            cmdTxt.Append("INSERT INTO {0} (".FRMT(schema.TableName));
            

            // Build header (exclude identities)
            foreach ( CostumMapping cm in schema.Mappings )
            {
                if ( cm.ClrProperty == schema.IdentityProperty ) 
                    continue;

                cmdTxt.Append(cm.BindedToColumn);       
                cmdTxt.Append(", ");
            }

            cmdTxt.Remove(cmdTxt.Length - 2, 2); // Remove last ,
            cmdTxt.Append(") values (");

            // Build body (exclude identities)
            foreach ( CostumMapping cm in schema.Mappings )
            {
                if ( cm.ClrProperty == schema.IdentityProperty )
                    continue;

                PropertyInfo pi = objRepresentor.GetProperty(cm.ClrProperty);
                String valueTxt = PrepareColumnType(pi, obj);

                cmdTxt.Append(valueTxt);
                cmdTxt.Append(", ");
            }

            cmdTxt.Remove(cmdTxt.Length - 2, 2); // Remove last ,
            cmdTxt.Append(")");

            return cmdTxt.ToString();
        }

        private static int _Insert<T>(DbConnection connection, T obj)
        {
            if ( connection == null )
                throw new NullReferenceException("connection is null");

            if ( connection.State != System.Data.ConnectionState.Open )
                throw new InvalidOperationException("connection must be opened");

            Type type = typeof(T);

            // Lock-Free
            ConfigureMetadataFor(type);

            //
            // If we are here, the properties for specific type are filled 
            // and never be touched (modified) again for the type.
            // 

            String insertCmd = PrepareInsertCmd<T>(type, obj);
            DbCommand cmd = connection.CreateCommand();

            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = insertCmd;

            return cmd.ExecuteNonQuery();
        }




        #endregion




        #region Update



        private static String PrepareUpdateCmd<T>(Type type, T obj)
        {
            Type objRepresentor = obj.GetType();
            
            //
            // Obtain local copy because another thread can change the reference of _typesSchema
            // and we need iterate in a secure way.
            //

            TypeSchema schema = _typesSchema[type];
            
            if ( schema.Keys.Count == 0 )
                throw new InvalidOperationException("Type {0} must have at least one key for updating".FRMT(type.Name));

            //
            // Update only if we have keys, to find the tuple
            // 

            StringBuilder cmdTxt = new StringBuilder();
            cmdTxt.Append("UPDATE {0} SET ".FRMT(schema.TableName));  

            
            // Build Set clause
            foreach ( CostumMapping cm in schema.Mappings )
            {
                if ( cm.ClrProperty == schema.IdentityProperty )
                    continue;

                PropertyInfo pi = objRepresentor.GetProperty(cm.ClrProperty);
                String valueTxt = PrepareColumnType(pi, obj);

                cmdTxt.Append("{0} = {1}, ".FRMT(cm.BindedToColumn, valueTxt));
            }

            cmdTxt.Remove(cmdTxt.Length - 2, 2); // Remove last ,

            // Build Where clause
            cmdTxt.Append(" WHERE ");

            int count = 0;
            foreach ( KeyMapping map in schema.Keys )
            {
                PropertyInfo pi = objRepresentor.GetProperty(map.ClrProperty);
                String valueTxt = PrepareColumnType(pi, obj);

                cmdTxt.Append("{0} = {1}".FRMT(map.SqlColumn, valueTxt));

                if ( ( count + 1 ) < schema.Keys.Count )
                    cmdTxt.Append(" AND ");

                count++;
            }  

            return cmdTxt.ToString();
        }


        private static int _Update<T>(DbConnection connection, T obj)
        {
            if ( connection == null )
                throw new NullReferenceException("connection is null");

            if ( connection.State != System.Data.ConnectionState.Open )
                throw new InvalidOperationException("connection must be opened");

            Type type = typeof(T);

            // Lock-Free
            ConfigureMetadataFor(type);

            //
            // If we are here, the properties for specific type are filled 
            // and never be touched (modified) again for the type.
            // 

            String updateCmd = PrepareUpdateCmd<T>(type, obj);
            DbCommand cmd = connection.CreateCommand();

            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = updateCmd;

            return cmd.ExecuteNonQuery();
        }


        #endregion




        #region Delete


        private static String PrepareDeleteCmd<T>(Type type, T obj)
        {
            Type objRepresentor = obj.GetType();

            //
            // Obtain local copy because another thread can change the reference of _typesSchema
            // and we need iterate in a secure way.
            //

            TypeSchema schema = _typesSchema[type];

            if ( schema.Keys.Count == 0 )
                throw new InvalidOperationException("Type {0} must have at least one key for deleting".FRMT(type.Name));


            //
            // Delete only if we have keys, to find the tuple
            // 

            StringBuilder cmdTxt = new StringBuilder();
            cmdTxt.Append("DELETE FROM {0}".FRMT(schema.TableName));              

            
            // Build Where clause if keys are defined
            cmdTxt.Append(" WHERE ");

            int count = 0;
            foreach ( KeyMapping map in schema.Keys )
            {
                PropertyInfo pi = objRepresentor.GetProperty(map.ClrProperty);
                String valueTxt = PrepareColumnType(pi, obj);

                cmdTxt.Append("{0} = {1}".FRMT(map.SqlColumn, valueTxt));

                if ( ( count + 1 ) < schema.Keys.Count )
                    cmdTxt.Append(" AND ");

                count++;
            }

            return cmdTxt.ToString();
        }


        private static int _Delete<T>(DbConnection connection, T obj)
        {
            if ( connection == null )
                throw new NullReferenceException("connection is null");

            if ( connection.State != System.Data.ConnectionState.Open )
                throw new InvalidOperationException("connection must be opened");

            Type type = typeof(T);

            // Lock-Free
            ConfigureMetadataFor(type);

            //
            // If we are here, the properties for specific type are filled 
            // and never be touched (modified) again for the type.
            // 

            String deleteCmd = PrepareDeleteCmd<T>(type, obj);
            DbCommand cmd = connection.CreateCommand();

            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = deleteCmd;

            return cmd.ExecuteNonQuery();
        }


        #endregion




        #region Select / Read


        private static IList<T> _MapTo<T>(DbDataReader reader)
        {
            if ( reader == null )
                throw new NullReferenceException("reader cannot be null");

            if ( reader.IsClosed )
                throw new InvalidOperationException("reader connection is closed and objects cannot be mapped");

            if ( !reader.HasRows )
                return new List<T>();


            Type type = typeof(T);

            //
            // If we are here, the properties for specific type are filled 
            // and never be touched (modified) again for the type.
            // 

            // Map cursor lines from database to CLR objects based on T

            List<T> bundle = new List<T>();

            while ( reader.Read() )
            {
                T newInstance = (T)Activator.CreateInstance(type);
                Type newInstanceRep = newInstance.GetType();            // Mirror instance to reflect newInstance

                // Map properties to the newInstance
                foreach ( CostumMapping map in _typesSchema[type].Mappings )
                {
                    object value;
                    string sqlColumn = map.FromSelectColumn;

                    try { value = reader[sqlColumn]; }
                    catch ( IndexOutOfRangeException )
                    {
                        throw new SqlColumnNotFoundException("Sql column with name: {0} is not found".FRMT(sqlColumn));
                    }

                    PropertyInfo ctxProperty = newInstanceRep.GetProperty(map.ClrProperty);

                    //
                    // Nullable condition checker!
                    //

                    if ( value.GetType() == typeof(DBNull) )
                    {
                        if ( ctxProperty.PropertyType.IsPrimitive )
                        {
                            throw new PropertyMustBeNullable(
                                "Property {0} must be nullable for mapping a null value".FRMT(ctxProperty.Name)
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

        private static String PrepareSelectCmd<T>(Type type, Expression<Func<T, bool>> filter)
        {
            StringBuilder cmdTxt = new StringBuilder();

            //
            // Obtain local copy because another thread can change the reference of _typesSchema
            // and we need iterate in a secure way.
            //

            TypeSchema schema = _typesSchema[type];         // Get schema information for specific Type


            cmdTxt.Append("SELECT ");

            // Select all columns that are mapped
            foreach ( CostumMapping cm in schema.Mappings )
            {
                cmdTxt.Append(cm.FromSelectColumn);
                cmdTxt.Append(", ");
            }

            cmdTxt.Remove(cmdTxt.Length - 2, 2); // Remove last ,
            cmdTxt.Append(" FROM {0} ".FRMT(schema.TableName));

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

        private static Type _PrepareSelect<T>(DbConnection connection)
        {
            if ( connection == null )
                throw new NullReferenceException("connection is null");

            if ( connection.State == ConnectionState.Closed )
                connection.Open();

            Type type = typeof(T);

            // Lock-Free
            ConfigureMetadataFor(type);
            return type;
        }
                
        private static IList<T> _Select<T>(DbConnection connection, Expression<Func<T, bool>> filter)
        {
            Type type = _PrepareSelect<T>(connection);

            //
            // If we are here, the properties for specific type are filled 
            // and never be touched (modified) again for the type.
            // 

            String selectCmd = PrepareSelectCmd<T>(type, filter);
            DbCommand cmd = connection.CreateCommand();

            cmd.CommandType = System.Data.CommandType.Text;     // dynamic SQL
            cmd.CommandText = selectCmd;

            return _MapTo<T>(cmd.ExecuteReader());
        }



        #endregion






        public ObjectMapper(DbConnection connection)
        {
            _connection = connection;
        }





        #region Public Interface




        public IList<T> Select<T>(CommandType commandType, string commandText, params SqlParameter[] parameters) 
        {
            Type type = _PrepareSelect<T>(_connection);

            //
            // If we are here, the properties for specific type are filled 
            // and never be touched (modified) again for the type.
            // 

            DbCommand comm = _connection.CreateCommand();

            comm.CommandType = commandType;
            comm.CommandText = commandText;

            // Set parameters
            if ( parameters != null )
                comm.Parameters.AddRange(parameters);

            return _MapTo<T>(comm.ExecuteReader());
        }

        public IList<T> Select<T>(Expression<Func<T, bool>> filter)
        {
            return _Select<T>(_connection, filter);
        }

        public int Insert<T>(T obj)
        {
            return _Insert(_connection, obj);
        }

        public int Update<T>(T obj)
        {
            return _Update(_connection, obj);
        }

        public int Delete<T>(T obj)
        {
            return _Delete(_connection, obj);
        }





        #endregion


    }
}