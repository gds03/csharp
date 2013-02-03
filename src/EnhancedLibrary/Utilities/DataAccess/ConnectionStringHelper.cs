using System;
using System.Data.EntityClient;

namespace EnhancedLibrary.Utilities.DataAccess
{
    public static class ConnectionStringHelper
    {
        public static string SQL2005NotTrusted(String ServerIp, String DbName, String UserName, String Password)
        {
            return String.Format("Data Source={0}; Initial Catalog={1}; User Id={2}; Password={3};",
                ServerIp, DbName, UserName, Password
            );
        }


        public static string SQL2005NotTrustedMARS(String ServerIp, String DbName, String UserName, String Password)
        {
            return SQL2005NotTrusted(ServerIp, DbName, UserName, Password) + "MultipleActiveResultSets=True;";
        }

        public static string SQL2005Trusted(String ServerIp, String DbName)
        {
            return String.Format("Data Source={0}; Initial Catalog={1}; Integrated Security=SSPI;",
                ServerIp, DbName
            );
        }

        public static string EntitySQL2005(string modelName, string sqlConnectionString)
        {
            EntityConnectionStringBuilder entityConnectionSb = new EntityConnectionStringBuilder();

            entityConnectionSb.Provider = "System.Data.SqlClient";
            entityConnectionSb.ProviderConnectionString = sqlConnectionString;
            entityConnectionSb.Metadata = string.Format(@"res://*/{0}.csdl|res://*/{0}.ssdl|res://*/{0}.msl", modelName);

            return entityConnectionSb.ToString();
        }
    }
}
