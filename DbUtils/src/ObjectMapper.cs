using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;
using System.Threading;
using BO_MAC.Extensions;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace DbUtils
{

    #region Mapper Attributes

    
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



    #region Mapper Exceptions

    public sealed class SqlColumnNotFoundException : Exception
    {
        internal SqlColumnNotFoundException(string msg) : base(msg) { }
    }

    public sealed class PropertyMustBeNullable : Exception
    {
        internal PropertyMustBeNullable(string msg) : base(msg) { }
    }
    
    #endregion



    #region Mapper Classes 

    internal sealed class TypeSchema
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

        internal TypeSchema(String tableName) : this()
        {
            TableName = tableName;            
        }
    }

    internal sealed class CostumMapping
    {
        internal String FromSelectColumn;
        internal String ClrProperty;
        internal String BindedToColumn;

        internal CostumMapping(String clrProperty)
        {
            FromSelectColumn = BindedToColumn = ClrProperty = clrProperty;
        }  
    }

    internal sealed class KeyMapping
    {
        internal String SqlColumn;
        internal String ClrProperty;

        public KeyMapping(String sqlColumn, String clrProperty)
        {
            SqlColumn = sqlColumn;
            ClrProperty = clrProperty;
        }
    }


    #endregion





    public class ObjectMapper
    {
        private readonly DbConnection _connection;      // The only Instance variable

        private static readonly BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;


        // For specific type, stores the properties that must be mapped from SQL
        private static volatile Dictionary<Type, TypeSchema> _typesSchema =
                            new Dictionary<Type, TypeSchema>();     // Accessed in context of multiple threads



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




        
        /// <summary>
        ///     Creates a list of T objects based on reader.
        ///     This method must receive reader opened and use convention to map
        ///     sql columns to properties of CLR types.
        ///     You can use Exclude to exclude a property.
        ///     You can use BindTo("PropertyName") to override convention behavior of mapping
        /// </summary>
        /// <typeparam name="T"> The type that must be mapped </typeparam>
        /// <param name="reader"> The reader that must be opened to iterating and mapping the values </param>

        /// <exception cref="NullReferenceException"> Reader is null </exception>
        /// <exception cref="InvalidOperationException"> Reader is closed </exception>
        /// <exception cref="SqlColumnNotFoundException"> Specific column is not found from result set </exception>
        /// <exception cref="PropertyMustBeNullable"> Property that value is mapped is receiving a nullable value and the property is not nullable </exception>
        /// <returns></returns>
        private static IList<T> MapTo<T>(DbDataReader reader)
        {
            if ( reader == null )
                throw new NullReferenceException("reader cannot be null");

            if ( reader.IsClosed )
                throw new InvalidOperationException("reader connection is closed and objects cannot be mapped");

            if ( !reader.HasRows )
                return new List<T>();


            Type type = typeof(T);

            // Lock-Free
            ConfigureMetadataFor(type);

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
                    catch ( IndexOutOfRangeException ) { 
                        throw new SqlColumnNotFoundException("Sql column with name: {0} is not found".FRMT(sqlColumn));
                    }

                    PropertyInfo ctxProperty = newInstanceRep.GetProperty(map.ClrProperty);

                    //
                    // Nullable condition checker!
                    //

                    if ( value.GetType() == typeof(DBNull) ) 
                    {
                        if ( ctxProperty.PropertyType.IsPrimitive ) {
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





        private static String PrepareColumnType(PropertyInfo pi, object obj)
        {
            object value = pi.GetValue(obj, null);

            if(value == null)
                return "NULL";

            if ( pi.PropertyType == typeof(String) )
                return "'" + value.ToString() + "'";

            if ( pi.PropertyType == typeof(bool) )
                return ( (bool)value ) ? "1" : "0";

            if ( pi.PropertyType == typeof(DateTime) )
            {
                DateTime d = (DateTime)value;
                return "convert(datetime, '{0}', 105)".FRMT(d);
            }


            // return default
            return value.ToString();
        }






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






        public ObjectMapper(DbConnection connection)
        {
            _connection = connection;
        }





        public IList<T> Read<T>(CommandType commandType, string commandText, params SqlParameter[] parameters)
        {
            if ( _connection.State == ConnectionState.Closed )
                _connection.Open();

            DbCommand comm = _connection.CreateCommand();

            comm.CommandType = commandType;
            comm.CommandText = commandText;

            // Set parameters
            if ( parameters != null )
                comm.Parameters.AddRange(parameters);

            return MapTo<T>(comm.ExecuteReader());
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


    }
}