using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHibernateMVCApp.Repository.Types
{
    public class Query<T>
    {
        internal ISession Session { get; private set; } 

        internal Query(ISession session)
        {
            if (session == null)
                throw new ArgumentNullException("session");

            Session = session;
        }
    }
}
