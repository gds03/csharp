using NHibernate;
using NHibernate.Context;
using NHibernateMVCApp.Repository.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHibernateMVCApp.Repository.Types.Wrappers
{
    public static class ISessionFactoryWrapper
    {
        public static ISessionFactory SessionFactory { get; private set; }

        public static void Init(string connStringName)
        {
            if (string.IsNullOrEmpty(connStringName))
                throw new ArgumentNullException("connStringName");

            SessionFactory = NHibernateSessionHelper.GetNHibernateSessionFactory(connStringName);
        }

        public static void BeginRequest()
        {
            var s = SessionFactory.OpenSession();
            CurrentSessionContext.Bind(s);
        }


        public static void EndRequest()
        {
            var s = CurrentSessionContext.Unbind(SessionFactory);
            s.Dispose();
        }
    }
}
