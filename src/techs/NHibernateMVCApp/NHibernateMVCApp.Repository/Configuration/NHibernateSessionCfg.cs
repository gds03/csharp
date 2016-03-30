using NHibernate;
using NHibernate.Cfg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHibernateMVCApp.Repository.Configuration
{
    public static class NHibernateSessionHelper
    {
        public static ISessionFactory GetNHibernateSessionFactory(string connStringKeyName)
        {
            if (string.IsNullOrEmpty(connStringKeyName))
                throw new ArgumentNullException("connStringKeyName");

            var configs = new NHibernate.Cfg.Configuration();

            configs.DataBaseIntegration(i =>
            {
                i.Dialect<NHibernate.Dialect.MsSql2012Dialect>();
                i.Driver<NHibernate.Driver.Sql2008ClientDriver>();
                i.ConnectionStringName = connStringKeyName;
                i.ConnectionProvider<NHibernate.Connection.DriverConnectionProvider>();
            });

            configs.CurrentSessionContext<NHibernate.Context.WebSessionContext>();

            // assembly of poco classes
            configs.AddAssembly(typeof(NHibernateMVCApp.Repository.Mappings.Product).Assembly);

            return configs.BuildSessionFactory();
        }
    }
}
