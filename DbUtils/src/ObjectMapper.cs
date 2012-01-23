using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;
using System.Threading;
using BO_MAC.Extensions;

namespace DbUtils
{

    #region Inner Classes

    public sealed class Exclude : Attribute
    {

    }

    public sealed class BindTo : Attribute
    {
        internal String _sqlColumn;

        public BindTo(String sqlColumn)
        {
            _sqlColumn = sqlColumn;
        }
    }

    public sealed class SqlColumnNotFoundException : Exception
    {
        internal SqlColumnNotFoundException(string msg) : base(msg) { }
    }

    public sealed class PropertyMustBeNullable : Exception
    {
        internal PropertyMustBeNullable(string msg) : base(msg) { }
    }


    // Used to map SQL Columns to CLR Properties
    internal sealed class CostumMapping
    {
        internal String _fromSql;           // When null, by convention, the mapper must get data from column with the same name of the clrProperty!
        internal String _toClrProperty;

        internal CostumMapping(String toClrProperty)
        {
            _toClrProperty = toClrProperty;
            _fromSql = null;
        }

        internal CostumMapping(String fromSql, String toClrProperty)
        {
            _fromSql = fromSql;
            _toClrProperty = toClrProperty;
        }
    }


    #endregion

    public static class ObjectMapper
    {
        // For specific type, stores the properties that must be mapped from SQL
        private static volatile Dictionary<Type, List<CostumMapping>> _propertiesToMap =
                            new Dictionary<Type, List<CostumMapping>>();                   // Accessed in context of multiple threads



        private static Dictionary<Type, List<CostumMapping>> NewCopyWithAddedTypeProperties(Type type)
        {
            //
            // Copies most recent dictionary and add new List for specific type (this is local to the thread)

            var props = new Dictionary<Type, List<CostumMapping>>(_propertiesToMap) { { type, new List<CostumMapping>() } };

            foreach ( PropertyInfo pi in type.GetProperties(BindingFlags.Public | BindingFlags.Instance) )
            {
                bool mapProperty = true;                                // Always to map, unless specified Exclude costum attribute
                CostumMapping mapVar = new CostumMapping(pi.Name);      // CLR property is always the property name of the type

                //
                // Each property can have N costum attributes
                foreach ( object o in pi.GetCustomAttributes(false) ) {

                    if ( o is Exclude ) {
                        mapProperty = false;    // don't map and exit!
                        break;
                    }

                    BindTo bt = o as BindTo;

                    if ( bt != null )           // override map behaviour and exit!
                    {
                        mapVar._fromSql = bt._sqlColumn;
                        break;
                    }
                }

                if ( mapProperty )
                    props[type].Add(mapVar);
            }

            return props;
        }

        private static void FillPropertiesForType(Type type)
        {
            do
            {
                List<CostumMapping> l;

                if ( _propertiesToMap.TryGetValue(type, out l) ) // Typically, this is the most common case to occur
                    break;

                //
                // Properties must be mapped! - multiple threads can be here.. 
                // (Altought isn't a commun case)
                // 

                Dictionary<Type, List<CostumMapping>> backup = _propertiesToMap;
                var newMappings = NewCopyWithAddedTypeProperties(type);

                if ( _propertiesToMap == backup && Interlocked.CompareExchange(ref _propertiesToMap, newMappings, backup) == backup )
                    break;

            } while ( true );
        }




        // 
        // This is a static method, so can be called by multiple threads
        //


        
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
        public static IList<T> MapTo<T>(DbDataReader reader)
        {
            if ( reader == null )
                throw new NullReferenceException("reader cannot be null");

            if ( reader.IsClosed )
                throw new InvalidOperationException("reader connection is closed and objects cannot be mapped");

            if ( !reader.HasRows )
                return new List<T>();


            Type type = typeof(T);

            // Lock-Free
            FillPropertiesForType(type);

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
                foreach ( CostumMapping map in _propertiesToMap[type] )
                {
                    object value;
                    string sqlColumn = map._fromSql == null ? map._toClrProperty 
                                                            : map._fromSql;

                    try { value = reader[sqlColumn]; }
                    catch ( IndexOutOfRangeException ) { 
                        throw new SqlColumnNotFoundException("Sql column with name: {0} is not found".FRMT(sqlColumn));
                    }

                    PropertyInfo ctxProperty = newInstanceRep.GetProperty(map._toClrProperty);

                    //
                    // Nullable condition
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

    }
}
