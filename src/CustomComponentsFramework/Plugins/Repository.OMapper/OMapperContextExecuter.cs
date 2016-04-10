using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Reflection;
using System.Threading;
using System.Data;
using Repository.OMapper.Attributes;
using Repository.OMapper.Exceptions;
using Repository.OMapper.Types;
using System.Linq.Expressions;
using System.Diagnostics;
using Repository.OMapper.Types.Mappings;
using Repository.OMapper.Internal;
using Repository.OMapper.Internal.CLR2SQL;
using Repository.OMapper.Providers;
using Repository.OMapper.Interfaces;
using Repository.OMapper.Internal.Metadata;

namespace Repository.OMapper
{
    /// <summary>
    ///     With this class you extend all functionality for a ORM.
    ///     You can Select, Insert, Change Objects returned by select, Delete and Submit().
    ///     All the commands will run in a transactional manner.
    /// </summary>
    public class OMapperContextExecuter : OMapperCRUDSupport
    { 
        // queue for commands
        readonly List<SelectObjectInfo> m_selectedObjects    = new List<SelectObjectInfo>();
        readonly List<object> m_insertCommandsQueue          = new List<object>();
        readonly List<object> m_deleteObjectQueue            = new List<object>();



        #region Contructors





        /// <summary>
        ///     Initialize OMapper with specified connectionString, IsolationLevel ReadCommitted and with a default command timeout of 30 seconds
        /// </summary>
        /// <param name="connectionString"></param>
        public OMapperContextExecuter(string connectionString) : base(connectionString)
        {
            SetEventHandler();
        }


        /// <summary>
        ///     Initialize OMapper with specified connectionString and with a default command timeout of 30 seconds
        /// </summary>
        /// <param name="connectionString"></param>
        public OMapperContextExecuter(string connectionString, IsolationLevel isolationLevel) : base(connectionString, isolationLevel)
        {
            SetEventHandler();
        }


        /// <summary>
        ///     Initialize OMapper with specified connection and with a default command timeout of 30 seconds
        /// </summary>
        /// <param name="connection"></param>
        public OMapperContextExecuter(DbTransaction transaction = null) : base(transaction)
        {
            SetEventHandler();
        }


        /// <summary>
        ///     Initialize OMapper with specified connection and with specified command timeout
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="commandTimeout"></param>
        public OMapperContextExecuter(int commandTimeout, DbTransaction transaction)
            : base(commandTimeout, transaction)
        {
            SetEventHandler();
        }
        



        #endregion


















        #region Static Methods




        #region Public


        #endregion


        #region Internal




        #endregion


        #region Protected


        #endregion



        #region Private




        #endregion


        #endregion Static Methods











        #region Instance Methods




        #region Public





        /// <summary>
        ///     Submit all the changes within the current instance.
        ///     All entities obtained and updated and all entities that were inserted and deleted will be persisted in data storage.
        /// </summary>
        /// <returns>The number commands that ran sucessfully against database.</returns>
        public int Submit()
        {
            int operationsPerformed = 0;

            return OpenCloseConnection(true, () =>
            {
                // Inserts
                foreach (object @objInsert in m_insertCommandsQueue)
                {
                    // Prepare insert statement for type
                    String SQLInsertCommand = m_sqlCommandsProvider.InsertCommand(@objInsert);
                    bool insertResult = ExecuteInsert(objInsert, SQLInsertCommand);
                    operationsPerformed++;

                    if (!insertResult)
                        continue;
                }

                // Updates
                foreach (SelectObjectInfo selectInfo in m_selectedObjects)
                {
                    string[] propertiesChanged = selectInfo.GetPropertiesChanged();
                    if (propertiesChanged != null)
                    {
                        object o = selectInfo.Object;

                        // Prepare update statement for type
                        String SQLUpdateCommand = m_sqlCommandsProvider.UpdateCommand(o, propertiesChanged);
                        bool updateResult = ExecuteUpdate(SQLUpdateCommand);

                        if (updateResult)
                            operationsPerformed++;
                    }
                }

                // Deletes
                foreach (object @objDelete in m_deleteObjectQueue)
                {
                    // Prepare delete statement for type
                    String SQLDeleteCommand = m_sqlCommandsProvider.DeleteCommand(@objDelete);
                    bool deleteResult = ExecuteDelete(SQLDeleteCommand);

                    if (deleteResult)
                        operationsPerformed++;
                }

                CleanupCommands();
                return operationsPerformed;
            });
        }

        #endregion


        #region Internal




        #endregion



        #region Protected

        protected override void InsertHandler<T>(T obj)
        {
            m_insertCommandsQueue.Add(obj);
        }

        protected override void DeleteHandler<T>(T obj)
        {
            m_deleteObjectQueue.Add(obj);
        }

      



        #endregion



        #region Private



        private void SetEventHandler()
        {
            OnMappingOneEntry += (objInstance) =>
            {
                // Track those elements behind the scenes for update scenarios
                m_selectedObjects.Add(new SelectObjectInfo(objInstance));
            };
        }


        /// <summary>
        ///     Cleanup commands that are on hold. Called on Submit().
        /// </summary>        
        private void CleanupCommands()
        {
            m_insertCommandsQueue.Clear();
            m_selectedObjects.Clear();
            m_deleteObjectQueue.Clear();
        }


        #endregion


        #endregion Instance Methods

    }
}