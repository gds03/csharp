﻿using OMapper.Interfaces;
using OMapper.Internal.CLR2SQL;
using OMapper.Providers;
using OMapper.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using OMapper.Extensions;
using OMapper.Internal.Proxies;
using OMapper.Types.Mappings;

namespace OMapper
{
    public abstract class OMapperCRUDSupportBase : OMapper
    {
        const int OperatorsInitCapacity = 23;
        const int ClrTypesMappingCapacity = 47;



        #region Members


        #region Static


        // Map expressionType (LINQ expression nodes to strings (e.g && -> AND, || -> OR, etc..)
        internal static readonly Dictionary<ExpressionType, String> s_ExpressionToSQLMapper;

        // Maps .CLR types to SQL types
        internal static readonly Dictionary<Type, String> s_ClrToSqlTypesMapper;


        #endregion



        #region Instance 

        internal readonly ISqlCommandTextGenerator m_sqlCommandsProvider = CommandsForTypeSchemaProvider.Current;

        #endregion



        #endregion






        #region Constructors



        /// <summary>
        ///     Initialize OMapper with specified connectionString, IsolationLevel ReadCommitted and with a default command timeout of 30 seconds
        /// </summary>
        /// <param name="connectionString"></param>
        public OMapperCRUDSupportBase(string connectionString) : base(connectionString)
        {

        }


        /// <summary>
        ///     Initialize OMapper with specified connectionString and with a default command timeout of 30 seconds
        /// </summary>
        /// <param name="connectionString"></param>
        public OMapperCRUDSupportBase(string connectionString, IsolationLevel isolationLevel) : base(connectionString,isolationLevel )
        {
 
        }


        /// <summary>
        ///     Initialize OMapper with specified connection and with a default command timeout of 30 seconds
        /// </summary>
        /// <param name="connection"></param>
        public OMapperCRUDSupportBase(DbTransaction transaction) : base(transaction)
        {

        }


        /// <summary>
        ///     Initialize OMapper with specified connection and with specified command timeout
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="commandTimeout"></param>
        public OMapperCRUDSupportBase(int commandTimeout, DbTransaction transaction)
            : base(commandTimeout, transaction)
        {

        }


        static OMapperCRUDSupportBase()
        {
            s_ExpressionToSQLMapper = CLRExpressionLinq2SQLExpressionLogicNames.GetConversions(OperatorsInitCapacity);
            s_ClrToSqlTypesMapper = CLRTypes2SQLTypes.GetConversions(ClrTypesMappingCapacity);
        }



        #endregion





        #region Static Methods




        #region Public


        public static Type GetTypeFor(object obj)
        {
            return ProxyCreator.IsProxy(obj) ? obj.GetType().BaseType : obj.GetType();
        }

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
        ///     Maps the table from database to a list of T by convention.
        /// </summary>
        /// <typeparam name="T">The type of the object that you want to Map</typeparam>
        /// <returns>A list of objects with their properties filled that aren't annotated with [Exclude] attribute</returns>
        public IList<T> Select<T>() where T : class
        {
            return Select<T>(null);
        }


        /// <summary>
        ///     Maps the table from database to a list of T by convention that satisfy the filter.
        /// </summary>
        /// <typeparam name="T">The type of the object that you want to Map</typeparam>
        /// <param name="filter">The filter that you must use to filter a sub part of the result</param>
        /// <returns>A list of objects with their properties filled that aren't annotated with [Exclude] attribute</returns>
        public IList<T> Select<T>(Expression<Func<T, bool>> filter) where T : class
        {
            // Lock-Free
            TypeSchema schema = base.AddMetadataFor(typeof(T));
            
            //
            // If we are here, the properties for specific type are filled 
            // and never be touched (modified) again for the type.
            // 

            // Prepare select statement for type

            String SQLSelectCommand = m_sqlCommandsProvider.SelectCommand(filter);

            return base.OpenCloseConnection(false, () =>
            {
                // Command Setup parameters
                DbCommand cmd = base.CreateCommandForCurrentConnection(CommandType.Text, SQLSelectCommand);
                return base.MapTo<T>(cmd.ExecuteReader());
            });
        }



        /// <summary>
        ///     Inserts the object on database and update the identity property in CLR object (if annotated with)
        ///     The property annotated with Identity Attribute is ignored on insert command.
        /// </summary>
        /// <typeparam name="T">The type of the object that you want to insert</typeparam>
        /// <param name="obj">The object that you want to insert</param>
        public void Insert<T>(T obj) where T : class
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            // Lock-Free
            base.AddMetadataFor(obj.GetType());

            //
            // If we are here, the properties for specific type are filled 
            // and never be touched (modified) again for the type.
            // 

            InsertHandler(obj);
        }


        /// <summary>
        ///     Based on primary key of the type, insert collection of objects passed by parameter in database.
        /// </summary>
        /// <param name="objects">The objects that you want to insert</param>
        public void InsertMany(params object[] objects)
        {
            if (objects != null)
            {
                foreach (object o in objects)
                    Insert(o);
            }
        }




        /// <summary>
        ///     Based on primary key of the type, delete the object from database
        /// </summary>
        /// <typeparam name="T">The type of the object that you want to delete. Note: This type must be annotated with [Key]</typeparam>
        /// <param name="obj">The object that you want to delete</param>
        /// <returns>The number of affected rows in database</returns>
        public void Delete<T>(T obj) where T : class
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            // Lock-Free
            base.AddMetadataFor(GetTypeFor(obj));

            //
            // If we are here, the properties for specific type are filled 
            // and never be touched (modified) again for the type.
            // 

            DeleteHandler(obj);
        }

        /// <summary>
        ///     Based on primary key of the type, delete collection of objects passed by parameter in database.
        /// </summary>
        /// <param name="objects">The objects that you want to delete</param>
        public void DeleteMany(params object[] objects)
        {
            if (objects != null)
            {
                foreach (object o in objects)
                    Delete(o);
            }
        }


        #endregion


        #region Internal





        internal abstract void InsertHandler<T>(T obj) where T : class;

        internal abstract void DeleteHandler<T>(T obj) where T : class;









        /// <summary>
        ///     Create a DB insert command and executes against database.
        ///     If the object that belongs to the insertCmd has Identity property it will be updated through reflection automatically.
        /// </summary>
        /// <returns>true if has identity and was updated, otherwise return false.</returns>
        internal bool ExecuteInsert(object obj, string insertCmd)
        {
            Debug.Assert(obj != null);
            Debug.Assert(insertCmd != null);

            Type insertObjRepresentor = obj.GetType();
            TypeSchema schema = s_TypesSchemaMapper[insertObjRepresentor];

            // Command Setup parameters
            DbCommand cmd = base.CreateCommandForCurrentConnection(CommandType.Text, insertCmd);

            // If Identity is not setted, execute the query and ignore the rest
            if (schema.IdentityPropertyName == null)
            {
                cmd.ExecuteNonQuery();
                return false;
            }

            //
            // The type have identity column and we must set the identity to instance of the object
            //

            // Execute scalar query
            object scope_identity;

            if ((scope_identity = cmd.ExecuteScalar()) == null)
                throw new InvalidOperationException("{0}:{1} is not an identify column in database".Frmt(insertObjRepresentor.Name, schema.IdentityPropertyName));

            // Set scope_identity to object property identity
            PropertyInfo pi = insertObjRepresentor.GetProperty(schema.IdentityPropertyName);

            // Convert type from db to property type
            object converted_identity = Convert.ChangeType(scope_identity, pi.PropertyType);

            // Set property identity
            pi.SetValue(obj, converted_identity, null);
            return true;
        }


        /// <summary>
        ///     Create a DB update command and executes against database. Called by Submit()
        /// </summary>
        /// <returns>true if has rows were updated, otherwise return false.</returns>
        internal bool ExecuteUpdate(string updateCmd)
        {
            Debug.Assert(!string.IsNullOrEmpty(updateCmd));

            // Command Setup parameters
            DbCommand cmd = base.CreateCommandForCurrentConnection(CommandType.Text, updateCmd);
            return cmd.ExecuteNonQuery() > 0;
        }



        /// <summary>
        ///     Create a DB delete command and executes against database. Called by Submit()
        /// </summary>
        /// <returns>true if has rows were deleted, otherwise return false.</returns>
        internal bool ExecuteDelete(string deleteCmd)
        {
            Debug.Assert(!string.IsNullOrEmpty(deleteCmd));
            // Command Setup parameters
            DbCommand cmd = base.CreateCommandForCurrentConnection(CommandType.Text, deleteCmd);
            return cmd.ExecuteNonQuery() > 0;
        }






        #endregion



        #region Protected



        #endregion



        #region Private




        #endregion


        #endregion Instance Methods


    }
}
