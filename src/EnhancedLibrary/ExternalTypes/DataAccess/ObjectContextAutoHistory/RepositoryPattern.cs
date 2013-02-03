using System;
using System.Linq;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Data;

namespace EnhancedLibrary.ExternalTypes.DataAccess.ObjectContextAutoHistory
{
    /// <summary>
    ///     By inherit from this class, you get the Repository Pattern to query the datasource.
    /// </summary>
    public class RepositoryPattern : IRepositoryPattern
    {
        readonly ObjectContext m_context;


        public RepositoryPattern(ObjectContext context) 
        {
            if ( context == null )
                throw new ArgumentNullException("context");
            
            if (context.GetType().BaseType != typeof(ObjectContext))
                throw new InvalidOperationException("context must derive directly from ObjectContext");

            m_context = context;
        }


        ObjectSet<T> Table<T>() where T : EntityObject
        {

            //
            // As the entity framework creates the properties with the same name of the Type we want to access,
            // it is really easy to map those types to properties throught reflection
            // Get the property of the context with the name of the type.
            //

            return (ObjectSet<T>) m_context.GetType().GetProperty(typeof(T).Name).GetValue(m_context, null);
        }


        


        /// <summary>
        ///     Allow Queries with LINQ to Entities throught IQueryable interface
        /// </summary>
        public IQueryable<T> Query<T>() where T : EntityObject
        {
            return Table<T>();
        }


        /// <summary>
        ///     Insert the e object in specific table.
        ///     The inserted object is only on database after Synchronize was called.
        /// </summary>
        public void Insert<T>(T e) where T : EntityObject
        {
            Table<T>().AddObject(e);
        }


        /// <summary>
        ///     Delete the e object from specific table.
        ///     The deleted object is only removed from database after Synchronize was called.
        /// </summary>
        public void Delete<T>(T e) where T : EntityObject
        {
            Table<T>().DeleteObject(e);
        }


        /// <summary>
        ///     Synchronize the database with all pending operations.
        /// </summary>
        public void Synchronize() {
            m_context.SaveChanges();
        }


        /// <summary>
        ///     Free all managed resources such the connection and ObjectContext associated with the repository
        /// </summary>
        public void Dispose()
        {
            m_context.Dispose();
        }


        /// <summary>
        ///     Get the connection to the repository
        /// </summary>
        public IDbConnection RepositoryConnection
        {
            get { return m_context.Connection; }
        }
    }
}
