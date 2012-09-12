using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.ComponentModel;

namespace ExtensionMethods
{
    public static class IComponentExtensions
    {
        readonly static Dictionary<string, int> s_class_to_ecran  = new Dictionary<string, int>();
        readonly static string[]                s_validExtensions = { ".cs", ".vb" };



        public static int GetEcranId(this IComponent component, string connectionStringDb)
        {
            // Get leaf className
            string leafClassName = component.GetType().Name;

            // Get Id for current form (leaf form)
            return s_class_to_ecran.ReadKey(leafClassName, () => GetId(leafClassName, connectionStringDb));
        }


        // s_validExtensions
        static int GetId(string className, string connectionStringDb)
        {
            using ( SqlConnection conn = new SqlConnection(connectionStringDb) ) 
            {
                conn.Open();

                foreach ( string ext in s_validExtensions ) {
                    SqlCommand comm = conn.CreateCommand();

                    comm.CommandText = "select top 1 EcraId from ARQAppModulosEcrans where VBForm like '{0}'".Frmt(className + ext);
                    comm.CommandType = System.Data.CommandType.Text;

                    try {
                        return (int) comm.ExecuteScalar();
                        // Found, so stop searching...
                    }

                    catch ( SqlException ) {
                        // Not found so try continue
                    }
                }

                throw new InvalidOperationException("You must define in ARQAppModulosEcrans table a new Ecran with name {0}".Frmt(className));
            }
        }



    }
}
