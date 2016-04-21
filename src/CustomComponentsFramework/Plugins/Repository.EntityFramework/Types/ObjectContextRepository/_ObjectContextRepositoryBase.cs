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
using System.Linq.Expressions;

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






   






        public abstract IRepository Insert<TEntity>(TEntity @object) where TEntity : class;

        public abstract IRepository Delete<TEntity>(TEntity @object) where TEntity : class;

        public abstract QueryResult<TEntity> Query<TEntity>() where TEntity : class;

        public abstract IList<T> Query<T>(Expression<Func<T, bool>> predicate) where T : class;

        
        public IRepository Submit()
        {
            if (ExaclyBeforeSaveCalled != null)
                ExaclyBeforeSaveCalled(this);

            ObjectContext.SaveChanges();
            return this;
        }


        public void Dispose()
        {
            ObjectContext.Dispose();
        }



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
