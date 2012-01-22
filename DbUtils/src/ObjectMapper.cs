using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;
using System.Threading;

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


    // Used to map SQL Columns to CLR Properties
    internal sealed class CostumMapping
    {
        internal String _fromSql;
        internal String _toClrProperty;

        public CostumMapping(String fromSql, String toClrProperty)
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



        private static Dictionary<Type, List<CostumMapping>> PropertiesForType(Type type)
        {
            var props = new Dictionary<Type, List<CostumMapping>>(_propertiesToMap) { { type, new List<CostumMapping>() } };

            foreach ( PropertyInfo pi in type.GetProperties(BindingFlags.Public | BindingFlags.Instance) )
            {
                bool mapProperty = true;
                string sqlColumn = pi.Name;         // default behavior is map by convention names from clr to sql

                //
                // Each property can have N costum attributes
                foreach ( object o in pi.GetCustomAttributes(false) )
                {
                    if ( o is Exclude ) {
                        mapProperty = false;
                        break;
                    }

                    BindTo bt = o as BindTo;

                    if( bt != null )
                    {
                        sqlColumn = bt._sqlColumn;  // replace default behaviour to bindTo column
                        break;
                    }
                }

                if ( mapProperty )
                    props[type].Add(new CostumMapping(sqlColumn, pi.Name));
            }

            return props;
        }

        private static void FillPropertiesForType(Type type) {
            do 
            {
                List<CostumMapping> l;

                if (_propertiesToMap.TryGetValue(type, out l)) // Typically, this is the most common case to occur
                    break;

                //
                // Properties must be mapped! - multiple threads can be here.. (Altought isn't a commun case)
                // 

                Dictionary<Type, List<CostumMapping>> backup = _propertiesToMap;
                var newMappings = PropertiesForType(type);

                if (_propertiesToMap == backup && Interlocked.CompareExchange(ref _propertiesToMap, newMappings, backup) == backup)
                    break;

            } while (true);
        }





        public static IList<T> MapTo<T>(DbDataReader reader)
        {
            if ( reader == null )
                throw new NullReferenceException("reader cannot be null");

            if ( reader.IsClosed )
                throw new InvalidOperationException("reader connection is closed and objects cannot be mapped");

            Type type = typeof(T);

            // Lock-Free
            FillPropertiesForType(type);

            //
            // If we are here, the properties for specific type are filled and never be touched (modified) again.
            // 

            // Map cursor lines from database to CLR objects based on T

            List<T> bundle = new List<T>();

            while ( reader.Read() )
            {
                T newInstance = (T)Activator.CreateInstance(type);
                Type newInstanceRep = newInstance.GetType();            // Mirror instance to reflect newInstance

                // Map properties to the newInstance
                foreach (CostumMapping map in _propertiesToMap[type])
                {
                    object value = reader[map._fromSql];
                    newInstanceRep.GetProperty(map._toClrProperty).SetValue(newInstance, value, null);     // WARNING: Conversion Types..
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
