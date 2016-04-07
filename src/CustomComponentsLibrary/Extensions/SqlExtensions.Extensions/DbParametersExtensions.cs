using SqlExtensions.Extensions.Types;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlExtensions.Extensions
{

    public static class DbParametersExtensions
    {




        /// <summary>
        ///     Convert CustomDbParameters array to SqlParameters array and 
        ///     where the value of the parameter is null or a empty string sets the value to DBNull.Value.
        /// </summary>
        public static DbParameter[] ToDbParameters(this CustomDbParameter[] parameters)
        {
            if (parameters == null)
                return null;

            List<DbParameter> r = new List<DbParameter>(parameters.Length);

            foreach (var parameter in parameters)
            {
                object obj = parameter.Value;

                if (obj == null)
                    r.Add(new SqlParameter(parameter.Name, DBNull.Value));

                else
                {
                    String s;

                    //
                    // If is string and is empty set to null.

                    if ((s = obj as String) != null && s == string.Empty)
                        r.Add(new SqlParameter(parameter.Name, DBNull.Value));

                    else
                    {

                        //
                        // See what value is inside the object, and determinate the respective type

                        AdjustValueIfNullable(ref obj);
                        r.Add(new SqlParameter(parameter.Name, obj));
                    }
                }
            }

            return r.ToArray();
        }






        #region Internal Methods




        static void AdjustValueIfNullable(ref object o)
        {
            if (IsNullableRef<Boolean>(ref o)) return;
            if (IsNullableRef<Byte>(ref o)) return;
            if (IsNullableRef<Int16>(ref o)) return;
            if (IsNullableRef<Int32>(ref o)) return;
            if (IsNullableRef<Int64>(ref o)) return;
            if (IsNullableRef<SByte>(ref o)) return;
            if (IsNullableRef<UInt16>(ref o)) return;
            if (IsNullableRef<UInt32>(ref o)) return;
            if (IsNullableRef<UInt64>(ref o)) return;
            if (IsNullableRef<Decimal>(ref o)) return;
            if (IsNullableRef<Single>(ref o)) return;
            if (IsNullableRef<Double>(ref o)) return;
            if (IsNullableRef<DateTime>(ref o)) return;
        }


        /// <summary>
        ///     Check if the o object is a T nullable type.
        ///     If not, simply return.
        ///     If is, see if contains the value. If not, set DbNull.Value, otherwise 
        ///     set the current Value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <returns></returns>
        static bool IsNullableRef<T>(ref object o) where T : struct
        {
            T? nullableT = o as T?;

            if (nullableT == null)
                return false;

            //
            // Is of T nullable type

            if (!nullableT.HasValue)
            {
                o = DBNull.Value;
            }

            else
            {
                o = nullableT.Value;
            }

            return true;
        }






        #endregion
    }
}
