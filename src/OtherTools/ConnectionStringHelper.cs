using System;

namespace ExtensionMethods.Utilities
{
    public static class ConnectionStringHelper
    {
        public static string SQL2005NotTrusted(String ServerIp, String DbName, String UserName, String Password)
        {
            return String.Format("Data Source={0}; Initial Catalog={1}; User Id={2}; Password={3};",
                ServerIp, DbName, UserName, Password
            );
        }

        public static string SQL2005Trusted(String ServerIp, String DbName)
        {
            return String.Format("Data Source={0}; Initial Catalog={1}; Integrated Security=SSPI;",
                ServerIp, DbName
            );
        }
    }
}
