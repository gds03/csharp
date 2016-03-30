using NHibernate;
using NHibernateMVCApp.Repository.Interfaces;
using NHibernateMVCApp.Repository.Types.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHibernateMVCApp.Repository.Types
{
    public class Repository : IRepository
    {
        protected ISession m_hbSession;

        public Repository()
        {
            m_hbSession = ISessionFactoryWrapper.SessionFactory.GetCurrentSession();
        }

        public Query<TValue> Query<TValue>() where TValue : new()
        {
            return new Query<TValue>(m_hbSession);
        }
        

        public TValue Insert<TValue>(TValue @object) where TValue : new()
        {
            using ( ITransaction tran = this.m_hbSession.BeginTransaction() )
            {
                 this.m_hbSession.Save(@object);
                 tran.Commit();
            }
            return @object;
        }

        public void Update<TValue>(TValue @object) where TValue : new()
        {
            using (ITransaction tran = this.m_hbSession.BeginTransaction())
            {
                this.m_hbSession.SaveOrUpdate(@object);
                tran.Commit();
            }
        }

        public void Delete<TValue>(TValue @object) where TValue : new()
        {
            using (ITransaction tran = this.m_hbSession.BeginTransaction())
            {
                this.m_hbSession.Delete(@object);
                tran.Commit();
            }
        }

        public TValue[] GetAll<TValue>() where TValue : new()
        {
            var icriteria = this.m_hbSession.CreateCriteria(typeof(TValue));
            return icriteria.List<TValue>().ToArray();
        } 

        public TValue GetById<TValue, TId>(TId id)
            where TValue : new()
            where TId : struct
        {
            return (TValue)this.m_hbSession.Load(typeof(TValue), id);
        }

       
    }
}
