using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using VirtualNote.Common.ExtensionMethods;
using VirtualNote.Database.Configurations;
using VirtualNote.Database.DomainObjects;

namespace VirtualNote.Database
{
    public sealed class MemoryRepository : IRepository
    {
        readonly IDictionary<Type, List<Object>> _db = new Dictionary<Type, List<object>>();
        readonly IDictionary<Type, Int32> _dbIdentities = new Dictionary<Type, Int32>();
        readonly IDictionary<Type, String> _dbColumnNamesIdentities = new Dictionary<Type, String>();

        public MemoryRepository()
        {
            SetPrimaryKeys();
            // SetDeleteConstraints();

            this.AddDefaultData(true);
        }

        void SetPrimaryKeys()
        {
            SetIdentity<Member>(m => m.UserID);
            SetIdentity<Client>(c => c.UserID);
            SetIdentity<Project>(p => p.ProjectID);
            SetIdentity<Issue>(i => i.IssueID);
            SetIdentity<Comment>(c => c.CommentID);
        }



        List<Object> Table<T>()
        {
            return _db.ReturnOrCreate(typeof(T), () => new List<object>());
        }

        Int32 GetIdentity(Type t)
        {
            return _dbIdentities.ReturnOrCreate(t, () => 1);
        }

        void IncrementIdentityFor<T>(T obj)
        {
            String primaryKeyForObj = _dbColumnNamesIdentities[typeof(T)];
            Type objBaseType = typeof(T).GetFirstType();
            int objIdentity = GetIdentity(objBaseType);

            //
            // Por reflexao coloca id na chave primaria
            obj.GetType().GetProperty(primaryKeyForObj).SetValue(obj, objIdentity, null);

            //
            // Incrementa o identity do tipo
            _dbIdentities[objBaseType]++;
        }




        public MemoryRepository SetIdentity<T>(Expression<Func<T, Object>> identity)
        {
            _dbColumnNamesIdentities.Add(typeof(T), identity.PropertyName());
            return this;
        }

        public IQueryable<T> Query<T>() where T : class, IDomainObject
        {
            return _db.Where(p => typeof(T).IsAssignableFrom(p.Key)).SelectMany(p => p.Value)
                   .Cast<T>().AsQueryable();
        }

        public void Insert<T>(T obj) where T : class, IDomainObject
        {
            Table<T>().Add(obj);
            IncrementIdentityFor(obj);
        }



        public void Delete<T>(T obj) where T : class, IDomainObject
        {
            Table<T>().Remove(obj);
        }

        public void SaveToDisk()
        {
            
        }

        public void Dispose()
        {
            
        }
    }
}
