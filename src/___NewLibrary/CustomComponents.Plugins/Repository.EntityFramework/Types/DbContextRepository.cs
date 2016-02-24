using CustomComponents.Database.Types.Generic;
using CustomComponents.Repository.Interfaces;
using Repository.EntityFramework.Enums;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomComponents.Core.ExtensionMethods;

namespace Repository.EntityFramework.Types
{
    /// <summary>
    ///     By inherit from this class, you get the Repository Pattern to query the datasource.
    /// </summary>
    public class EFDbContextRepository : IRepository
    {
        // Fields
        public DbContext DbContext { get; private set; }
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
        public event Action<IRepository> ExaclyBeforeSaveCalled;



        public EFDbContextRepository(DbContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            DbContext = context;
            ObjectContext = ((IObjectContextAdapter)context).ObjectContext;

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

        DbSet<T> Table<T>() where T : class
        {

            //
            // As the entity framework creates the properties with the same name of the Type we want to access,
            // it is really easy to map those types to properties throught reflection
            // Get the property of the context with the name of the type.
            //

            return (DbSet<T>)DbContext.GetType().GetProperty(typeof(T).Name).GetValue(DbContext, null);
        }





        /// <summary>
        ///     Allow Queries with LINQ to Entities throught IQueryable interface
        /// </summary>
        public QueryResult<T> Query<T>() where T : class
        {
            return new QueryResult<T>(Table<T>());
        }


        /// <summary>
        ///     Insert the e object in specific table.
        ///     The inserted object is only on database after Synchronize was called.
        /// </summary>
        public IRepository Insert<T>(T e) where T : class
        {
            Table<T>().Add(e);
            return this;
        }


        /// <summary>
        ///     Delete the e object from specific table.
        ///     The deleted object is only removed from database after Synchronize was called.
        /// </summary>
        public IRepository Delete<T>(T e) where T : class
        {
            Table<T>().Remove(e);
            return this;
        }


        /// <summary>
        ///     Synchronize the database with all pending operations.
        /// </summary>
        public IRepository Synchronize()
        {
            if (ExaclyBeforeSaveCalled != null)
                ExaclyBeforeSaveCalled(this);

            DbContext.SaveChanges();
            return this;
        }


        /// <summary>
        ///     Free all managed resources such the connection and ObjectContext associated with the repository
        /// </summary>
        public void Dispose()
        {
            DbContext.Dispose();
        }


        /// <summary>
        ///     Get the connection to the repository
        /// </summary>
        public System.Data.IDbConnection RepositoryConnection
        {
            get { return DbContext.Database.Connection; }
        }


        public TResult ExecuteUsing<TResult>(Func<IRepository, TResult> externMethod)
        {
            using (EFDbContextRepository callerInstance = this)
            {
                return externMethod(callerInstance);
            }
        }


        /// <summary>
        ///     Template code to request a transaction, execute code and give the user the repository to make some operations on it, and at the end commit and sync all data to db
        /// </summary>
        public void ExecuteBlock(Action<IRepository> externMethod, Action<Exception> exceptionMethod = null)
        {
            using (EFDbContextRepository callerInstance = this)
            {
                using (DbConnection efConnection = callerInstance.ObjectContext.Connection)
                {
                    efConnection.Open();
                    DbTransaction efTran = null;

                    try
                    {
                        // start transaction
                        efTran = callerInstance.ObjectContext.Connection.BeginTransaction();

                        // Call user function
                        externMethod(callerInstance);

                        // sync (unit of work)
                        callerInstance.Synchronize();

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
    }
}
