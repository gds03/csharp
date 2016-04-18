using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using Repository.OMapper.Internal;
using Repository.OMapper.Internal.Proxies;
using System.Reflection;
using Repository.OMapper.Types;
using System.Diagnostics;
using System.Threading;

namespace Repository.OMapper
{
    /// <summary>
    ///     With this class you extend all functionality for a ORM.
    ///     You can Select, Insert, Change Objects returned by select, Delete and Submit().
    ///     All the commands will run in a transactional manner.
    /// </summary>
    public class OMapperContextExecuter : OMapperCRUDSupportBase
    { 
        // queue for commands
        readonly Dictionary<long, ProxyObjectInfo> m_changedObjects         = new Dictionary<long, ProxyObjectInfo>();
        readonly List<object> m_insertCommandsQueue                         = new List<object>();
        readonly List<object> m_deleteObjectQueue                           = new List<object>();

        private volatile int ProxiesMapped                                  = 0;



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
                foreach (ProxyObjectInfo changedObject in m_changedObjects.Values)
                {
                    string[] propertiesChanged = changedObject.PropertiesChanged.Keys.ToArray();
                    if (propertiesChanged != null && propertiesChanged.Length > 0)
                    {
                        object o = changedObject.ProxyObject;

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

        internal override T MapToInstance<T>(TypeSchema ts)
        {
            TypeSchemaProxy tsp = ts as TypeSchemaProxy;
            Debug.Assert(tsp != null);
            Debug.Assert(tsp.ProxyType != null);
            return (T)Activator.CreateInstance(tsp.ProxyType);
        }



        internal override TypeSchema CreateSchema(Type type)
        {
            TypeSchema ts = base.CreateSchema(type);

            // Create specific TypeSchema 
            TypeSchemaProxy tsp = new TypeSchemaProxy(ts);
            return tsp;
        }



        internal override void MapToHandleProperty(object newInstance, PropertyInfo property, object propertyValue)
        {
            // we need to access field instead of property to not cause Proxy setter execution behind the scenes.
            GetBackingField(newInstance, property.Name).SetValue(newInstance, propertyValue);
        }



        internal override void InsertHandler<T>(T obj)
        {
            m_insertCommandsQueue.Add(obj);
        }

        internal override void DeleteHandler<T>(T obj)
        {
            m_deleteObjectQueue.Add(obj);
        }


        /// <summary>
        ///     Method that will be called by Proxy Objects
        /// </summary>
        /// <param name="proxyObj"></param>
        /// <param name="propertyToUpdate"></param>
        public void PutObjectForUpdate(object proxyObj, string propertyToUpdate)
        {
            if (proxyObj == null)
                throw new ArgumentNullException("proxyObj");

            if (string.IsNullOrEmpty(propertyToUpdate))
                throw new ArgumentException("propertyToUpdate");

            PropertyInfo pi = null;
            if ((pi = proxyObj.GetType().GetProperty(ProxyCreator.PROXY_InternalID, OMapper.s_PropertiesFlags)) == null)
                throw new InvalidOperationException("THis method cannot be called explicitly. Proxy objects call this method to indicate properties that are changing.");

            ProxyObjectInfo soi = null;
            long ProxyID = (long) pi.GetValue(proxyObj, null);

            if (!m_changedObjects.TryGetValue(ProxyID, out soi))

            {
                soi = new ProxyObjectInfo(proxyObj);
                m_changedObjects.Add(ProxyID, soi);
            }

            soi.PropertiesChanged.Add(propertyToUpdate, true);

        }



        #endregion



        #region Protected

       






        #endregion



        #region Private




        private static string _getBackingFieldName(string propertyName)
        {
            return string.Format("<{0}>k__BackingField", propertyName);
        }

        private static FieldInfo GetBackingField(object obj, string propertyName)
        {
            return obj.GetType().BaseType.GetField(_getBackingFieldName(propertyName), BindingFlags.Instance | BindingFlags.NonPublic);
        }


        /// <summary>
        ///     Cleanup commands that are on hold. Called on Submit().
        /// </summary>        
        private void CleanupCommands()
        {
            m_insertCommandsQueue.Clear();
            m_changedObjects.Clear();
            m_deleteObjectQueue.Clear();
        }



        private void SetEventHandler()
        {
            OnNewInstanceCreated += (objInstance) =>
            {

                // indicate that we are a proxy.
                objInstance.GetType().GetProperty(ProxyCreator.PROXY_InternalID, s_PropertiesFlags).SetValue(objInstance, Interlocked.Increment(ref ProxiesMapped), null);
                objInstance.GetType().GetProperty(ProxyCreator.OMapper_PROPERTY_NAME, s_PropertiesFlags).SetValue(objInstance, this, null);

                // changes will be tracked by proxy properties overload that will contact this object.
            };
        }


        


        #endregion


        #endregion Instance Methods

    }
}