using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using System.Data.EntityClient;
using System.Data;
using EnhancedLibrary.Types.ObjectContextAutoHistory.HistoryColumns;

namespace EnhancedLibrary.Types.ObjectContextAutoHistory
{
    /// <summary>
    ///     Create instances of this class when you don't want to delete items from database, and when you insert, update or delete,
    ///     update specific columns in a table
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectContextAutoHistory<T> 
        where T : struct
    {
        public enum DeleteTranslation
        {
            EntryDelete = 0,
            EntryUpdate = 1
        }



        readonly T m_userId;
        readonly DeleteTranslation m_deleteTrans;
        readonly ObjectContext m_context;





        public ObjectContextAutoHistory(ObjectContext context, DeleteTranslation deleteTranslation, T UserId)
        {
            if ( context == null )
                throw new ArgumentNullException("context");

            m_context = context;
            m_userId = UserId;
            m_deleteTrans = deleteTranslation;

            m_context.SavingChanges += (o, e) =>
            {
                m_context.ObjectStateManager.GetObjectStateEntries(EntityState.Added | EntityState.Modified | EntityState.Deleted).ToList().ForEach(x =>
                {
                    switch ( x.State )
                    {
                        case EntityState.Added:

                            if ( TryUpdate_If_IUserAltNotNullable(x.Entity) )
                                break;

                            IUserCreate<T> objCria;
                            if ( ( objCria = x.Entity as IUserCreate<T> ) != null )
                            {
                                objCria.DataCria = DateTime.Now;
                                objCria.UserCria = m_userId;
                            }

                            break;

                        case EntityState.Modified:

                            if ( TryUpdate_If_IUserAltNotNullable(x.Entity) )
                                break;

                            IUserUpdate<T> objAlt;
                            if ( ( objAlt = x.Entity as IUserUpdate<T> ) != null )
                            {
                                objAlt.DataAlt = DateTime.Now;
                                objAlt.UserAlt = m_userId;
                            }

                            break;

                        case EntityState.Deleted:

                            if ( TryUpdate_If_IUserAltNotNullable(x.Entity) )
                                break;

                            IUserDelete<T> objDel;
                            if ( ( objDel = x.Entity as IUserDelete<T> ) != null && m_deleteTrans == DeleteTranslation.EntryUpdate )
                            {
                                objDel.DataDel = DateTime.Now;
                                objDel.UserDel = m_userId;

                                // Instead of delete, change the state to modified to send update command to database.
                                //

                                x.ChangeState(EntityState.Modified);
                            }
                            break;
                    }
                });
            };
        }



        /// <summary>
        ///     Get the ObjectContext object associated with the instance
        /// </summary>
        public ObjectContext Context
        {
            get { return m_context; }
        }






        bool TryUpdate_If_IUserAltNotNullable(object obj)
        {
            IUserUpdateNotNullable<T> objAltNotNullable;

            if ( ( objAltNotNullable = obj as IUserUpdateNotNullable<T> ) != null )
            {
                objAltNotNullable.DataAlt = DateTime.Now;
                objAltNotNullable.UserAlt = m_userId;

                return true;
            }

            return false;
        }
    }
}
