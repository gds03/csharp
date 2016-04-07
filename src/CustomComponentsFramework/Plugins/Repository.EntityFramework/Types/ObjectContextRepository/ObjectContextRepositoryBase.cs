using CustomComponents.Repository.Interfaces;
using Repository.EntityFramework.Enums;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomComponents.Core.ExtensionMethods;
using CustomComponents.Database.Types.Generic;
using System.Data.Common;
using System.Data;
using Repository.EntityFramework.Interfaces;
using CustomComponents.Repository.Types.Generic;

namespace Repository.EntityFramework.Types.ObjectContextRepository
{
    /// <summary>
    ///     By inherit from this class, you get the Repository Pattern to query the datasource.
    /// </summary>
    public abstract class ObjectContextRepositoryBase : IRepository, IDatabaseStored
    {
        // Fields
        public ObjectContext ObjectContext { get; private set; }


        //
        // Events

        /// <summary>
        ///     Method that is called, passing the object that is to be inserted on db.
        /// </summary>
        public event Action<object> EntityAdded;

        /// <summary>
        ///     Method that is called, passing the object that is to be updated on db.
        /// </summary>
        public event Action<object> EntityModified;

        /// <summary>
        ///     Method that is called, passing the object that is to be deleted on db.
        ///     If this method return true, the object is to be deleted, if not, the command generated is to update this entry
        /// </summary>
        public event Func<object, bool> EntityDeleted;

        /// <summary>
        ///     Give the chance to execute some code before save is called.
        /// </summary>
        public event Callback ExaclyBeforeSaveCalled;



        public ObjectContextRepositoryBase(ObjectContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            ObjectContext = context;
            ObjectContext.SavingChanges += OnSavingChanges;
        }


        // Called before send commands to db.
        void OnSavingChanges(object sender, EventArgs e)
        {
            ObjectContext.ObjectStateManager.GetObjectStateEntries(EntityState.Added | EntityState.Modified | EntityState.Deleted)
                        .ToList()
                        .ForEach(x =>
                        {
                            switch (x.State)
                            {
                                case EntityState.Added:
                                    if (EntityAdded != null)
                                        EntityAdded(x.Entity);

                                    break;

                                case EntityState.Modified:

                                    if (EntityModified != null)
                                        EntityModified(x.Entity);

                                    break;

                                case EntityState.Deleted:

                                    if (EntityDeleted != null)
                                    {
                                        bool InsteadOfUpdateDeleteEntityInDb = EntityDeleted(x.Entity);

                                        if (!InsteadOfUpdateDeleteEntityInDb)
                                            x.ChangeState(EntityState.Modified);    // Instead of delete, change the state to modified to send update command to database.
                                    }

                                    break;
                            }
                        });
        }


        /// <summary>
        ///     Get a map that represents a list of objects in given state.
        /// </summary>
        public ObjectEntryMap GetEntries(Type EntityType = null, Entry state = (Entry.Added | Entry.Deleted | Entry.Modified | Entry.Unchanged))
        {
            ObjectEntryMap result = new ObjectEntryMap();

            foreach (var entry in ObjectContext.ObjectStateManager.GetObjectStateEntries((EntityState)state))
            {
                if ((EntityType == null) || (EntityType != null && entry.Entity.IsOfType(EntityType)))
                {
                    switch (entry.State)
                    {
                        case EntityState.Unchanged:
                            result.Unchanged.Add(entry.Entity);
                            break;

                        case EntityState.Added:
                            result.Added.Add(entry.Entity);
                            break;

                        case EntityState.Deleted:
                            result.Deleted.Add(entry.Entity);
                            break;

                        case EntityState.Modified:
                            result.Modified.Add(entry.Entity);
                            break;
                    }
                }
            }

            return result;
        }


        /// <summary>
        ///     Get a flatten array that represents a list of objects in given state.
        /// </summary>
        public ObjectEntry[] GetEntriesFlatten(Type EntityType = null, Entry state = (Entry.Added | Entry.Deleted | Entry.Modified | Entry.Unchanged))
        {
            return ObjectContext.ObjectStateManager
                                .GetObjectStateEntries((EntityState)state)
                                .Aggregate(new List<ObjectEntry>(), (l, entry) =>
                                {
                                    if ((EntityType == null) || (EntityType != null && entry.Entity.IsOfType(EntityType)))
                                    {
                                        l.Add(new ObjectEntry
                                        {
                                            State = (Entry)entry.State,
                                            @Object = entry.Entity
                                        });
                                    }

                                    return l;
                                })
                                .ToArray();
        }



        private IRepositorySet<TEntity> Set<TEntity>() where TEntity : class
        {
            //
            // As the entity framework creates the properties with the same name of the Type we want to access,
            // it is really easy to map those types to properties throught reflection
            // Get the property of the context with the name of the type.
            //
            
            var propertyValue = ObjectContext.GetType().GetProperty(typeof(TEntity).Name).GetValue(ObjectContext, null);

            //

            return SetHook<TEntity>(propertyValue);

        }

        protected abstract IRepositorySet<TEntity> SetHook<TEntity>(object DbSetOrObjectSet) where TEntity : class;
       

        /// <summary>
        ///     Allow Queries with LINQ to Entities throught IQueryable interface
        /// </summary>
        public QueryResult<TEntity> Query<TEntity>() where TEntity : class
        {
            return new QueryResult<TEntity>(Set<TEntity>().Query);
        }


        /// <summary>
        ///     Insert the e object in specific table.
        ///     The inserted object is only on database after Synchronize was called.
        /// </summary>
        public IRepository Insert<TEntity>(TEntity e) where TEntity : class
        {
            Set<TEntity>().Add(e);
            return this;
        }


        /// <summary>
        ///     Delete the e object from specific table.
        ///     The deleted object is only removed from database after Synchronize was called.
        /// </summary>
        public IRepository Delete<TEntity>(TEntity e) where TEntity : class
        {
            Set<TEntity>().Remove(e);
            return this;
        }


        /// <summary>
        ///     Synchronize the database with all pending operations.
        /// </summary>
        public IRepository Submit()
        {
            if (ExaclyBeforeSaveCalled != null)
                ExaclyBeforeSaveCalled(this);

            ObjectContext.SaveChanges();
            return this;
        }


        /// <summary>
        ///     Free all managed resources such the connection and ObjectContext associated with the repository
        /// </summary>
        public void Dispose()
        {
            ObjectContext.Dispose();
        }


        /// <summary>
        ///     Get the connection to the repository
        /// </summary>
        public IDbConnection RepositoryConnection
        {
            get { return ObjectContext.Connection; }
        }


 
        /// <summary>
        ///     Template code to request a transaction, execute code and give the user the repository to make some operations on it, and at the end commit and sync all data to db
        /// </summary>
        public void ExecuteBlock(Callback externMethod, ExceptionCallback exceptionMethod = null)
        {
            using (IRepository callerInstance = this)
            {
                IDatabaseStored db = this as IDatabaseStored;
                if (db == null)
                    throw new InvalidCastException("db");

                using (IDbConnection efConnection = RepositoryConnection)
                {
                    efConnection.Open();
                    IDbTransaction efTran = null;

                    try
                    {
                        // start transaction
                        efTran = RepositoryConnection.BeginTransaction();

                        // Call user function
                        externMethod(callerInstance);

                        // sync (unit of work)
                        db.Submit();

                        // persist
                        efTran.Commit();
                    }

                    catch (Exception e)
                    {
                        if (efTran != null)
                            efTran.Rollback();      // some exception occur, roll back to initial state.

                        if (exceptionMethod == null)
                            throw;

                        else { exceptionMethod(e); }
                    }
                }
            }
        }

        public TResult ExecuteUsing<TResult>(CallbackResult<TResult> externMethod)
        {
            using (IRepository callerInstance = this)
            {
                return externMethod(callerInstance);
            }
        }
    }
}
