using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;

namespace ExtensionMethods
{
    public sealed class CustomDbParameter
    {
        private object value;
        private string name;

        public CustomDbParameter(string Name, object Value) {
            this.value = Value;
            this.name = Name;
        }

        /// <summary>
        ///     Return the value associated with the parameter
        /// </summary>
        public object Value { get { return value; } }

        /// <summary>
        ///     Return the name of the parameter
        /// </summary>
        public string Name { get { return name; } }
    }

    public static class DbParametersExtensions
    {
        static void AdjustValueIfNullable(ref object o) 
        {
            if ( IsNullableRef<Boolean>(ref o) ) return;
            if ( IsNullableRef<Byte>(ref o) ) return;
            if ( IsNullableRef<Int16>(ref o) ) return;
            if ( IsNullableRef<Int32>(ref o) ) return;
            if ( IsNullableRef<Int64>(ref o) ) return;
            if ( IsNullableRef<SByte>(ref o) ) return;
            if ( IsNullableRef<UInt16>(ref o) ) return;
            if ( IsNullableRef<UInt32>(ref o) ) return;
            if ( IsNullableRef<UInt64>(ref o) ) return;
            if ( IsNullableRef<Decimal>(ref o) ) return;
            if ( IsNullableRef<Single>(ref o) ) return;
            if ( IsNullableRef<Double>(ref o) ) return;
            if ( IsNullableRef<DateTime>(ref o) ) return;
        }


        static bool IsNullableRef<T>(ref object o) where T : struct {
            Nullable<T> t = o as Nullable<T>;

            if ( t == null )
                return false;

            if ( !t.HasValue )
                o = DBNull.Value;
            else o = t.Value;

            return true;
        }




        /// <summary>
        ///     Convert CustomDbParameters array to SqlParameters array and where the value of the parameter is null
        ///     sets the value to DBNull.Value
        /// </summary>
        public static DbParameter[] ToDbParameters(this CustomDbParameter[] parameters) 
        {
            if ( parameters == null )
                return null;

            List<DbParameter> r = new List<DbParameter>(parameters.Length);

            foreach ( var o in parameters ) 
            {
                object obj = o.Value;

                if ( obj == null )
                    r.Add(new SqlParameter(o.Name, DBNull.Value));

                else 
                {
                    String s;
                    
                    if ( (s = obj as String) != null && s == string.Empty )
                        r.Add(new SqlParameter(o.Name, DBNull.Value));

                    else {
                        AdjustValueIfNullable(ref obj);
                        r.Add(new SqlParameter(o.Name, obj));
                    }
                }
            }

            return r.ToArray();
        }
    }
}
