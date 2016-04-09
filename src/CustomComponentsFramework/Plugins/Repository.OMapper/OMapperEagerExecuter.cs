using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Repository.OMapper
{
    public class OMapperEagerExecuter : OMapperCRUDSupport
    {

        #region Constructors





        /// <summary>
        ///     Initialize OMapper with specified connectionString and with a default command timeout of 30 seconds
        /// </summary>
        /// <param name="connectionString"></param>
        public OMapperEagerExecuter(string connectionString) : base(connectionString)
        {

        }


        /// <summary>
        ///     Initialize OMapper with specified connection and with a default command timeout of 30 seconds
        /// </summary>
        /// <param name="connection"></param>
        public OMapperEagerExecuter(DbConnection connection, DbTransaction transaction = null) : base(connection, transaction)
        {

        }


        /// <summary>
        ///     Initialize OMapper with specified connection and with specified command timeout
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="commandTimeout"></param>
        public OMapperEagerExecuter(DbConnection connection, int commandTimeout, DbTransaction transaction = null)
            : base(connection, commandTimeout, transaction)
        {
      
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
        ///     Based on primary key of the type, delete the object from database
        /// </summary>
        /// <typeparam name="T">The type of the object that you want to delete. Note: This type must be annotated with [Key]</typeparam>
        /// <param name="obj">The object that you want to delete</param>
        /// <returns>The number of affected rows in database</returns>
        public void Update<T>(T obj) where T : class
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            // Lock-Free
            AddMetadataFor(obj.GetType());

            //
            // If we are here, the properties for specific type are filled 
            // and never be touched (modified) again for the type.
            // 

            String SQLUpdateCommand = m_sqlCommandsProvider.UpdateCommand(obj, obj.GetType().GetProperties(OMapper.s_PropertiesFlags).Select(x => x.Name).ToArray());
            ExecuteUpdate(SQLUpdateCommand);
        }

        /// <summary>
        ///     Based on primary key of the type, delete collection of objects passed by parameter in database.
        /// </summary>
        /// <param name="objects">The objects that you want to delete</param>
        public void UpdateMany(params object[] objects)
        {
            if (objects != null)
            {
                foreach (object o in objects)
                    this.Update(o);
            }
        }






        #endregion


        #region Internal




        #endregion



        #region Protected

        #endregion Instance Methods



        protected override void InsertHandler<T>(T obj)
        {
            // Prepare insert statement for type
            String SQLInsertCommand = m_sqlCommandsProvider.InsertCommand(obj);
            ExecuteInsert(obj, SQLInsertCommand);
        }


        protected override void DeleteHandler<T>(T obj)
        {
            String SQLDeleteCommand = m_sqlCommandsProvider.DeleteCommand(obj);
            ExecuteDelete(SQLDeleteCommand);
        }



        #endregion




    }
}
