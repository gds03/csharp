
//
// Released at: 17 February 2012 v1.0
// Author: Goncalo Dias
//
//
// Last updated date: 09-04-2016
//


using Repository.OMapper.Attributes;
using Repository.OMapper.Exceptions;
using Repository.OMapper.Internal.Metadata;
using Repository.OMapper.Types;
using Repository.OMapper.Types.Mappings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Repository.OMapper.Extensions;

namespace Repository.OMapper
{
	public class OMapper : IDisposable
	{
		const int SchemaInitCapacity = 53;
		const IsolationLevel CURRENT_ISOLATION_LEVEL = IsolationLevel.ReadCommitted;




        #region Members

        #region Static

        private static readonly object s_initializationlocker = new object();
        private static volatile Action<InitializationMetadata> s_InitializationUserFunc;
		internal static readonly BindingFlags s_PropertiesFlags = BindingFlags.Public | BindingFlags.Instance;

		// For specific type, stores the properties that must be mapped from SQL (Accessed in context of multiple threads)
		internal static volatile Dictionary<Type, TypeSchema> s_TypesSchemaMapper;
        

        #endregion



        #region Instance 

        // member fields
        protected bool m_disposed;

		protected DbTransaction m_transaction;
		protected readonly DbConnection m_connection;
		protected readonly int m_commandTimeout = 30;



		protected event Action<object> OnMappingOneEntry;


		#endregion


		#endregion







		#region Constructors


		static OMapper()
		{
			s_TypesSchemaMapper = new Dictionary<Type, TypeSchema>(SchemaInitCapacity);
		}





		/// <summary>
		///     Initialize OMapper with specified connectionString and with a default command timeout of 30 seconds
		/// </summary>
		/// <param name="connectionString"></param>
		public OMapper(string connectionString)
		{
			if (string.IsNullOrEmpty(connectionString))
				throw new ArgumentNullException("connectionString");

			m_connection = new SqlConnection(connectionString);
		}


		/// <summary>
		///     Initialize OMapper with specified connection and with a default command timeout of 30 seconds
		/// </summary>
		/// <param name="connection"></param>
		public OMapper(DbConnection connection, DbTransaction transaction = null)
		{
			if (connection == null)
				throw new ArgumentNullException("connection");

			m_connection = connection;
			m_transaction = transaction;
		}


		/// <summary>
		///     Initialize OMapper with specified connection and with specified command timeout
		/// </summary>
		/// <param name="connection"></param>
		/// <param name="commandTimeout"></param>
		public OMapper(DbConnection connection, int commandTimeout, DbTransaction transaction = null)
			: this(connection, transaction)
		{
			m_commandTimeout = commandTimeout;
		}


		~OMapper()
		{
			Dispose(false);
		}




		#endregion







		#region Static Methods




		#region Public


		/// <summary>
		///     Allows user to configure fluently types options/properties that will be used by the OMapper.
		/// </summary>
		/// <param name="userFunc"></param>
		public static void Configuration(Action<InitializationMetadata> userFunc)
		{
			if (userFunc == null)
				throw new ArgumentNullException("userFunc");


            // use lock here since is called once - at startup time.
            lock (s_initializationlocker)
            {

                if (s_InitializationUserFunc != null)
                    throw new InvalidOperationException("Initialization code is already defined");

                userFunc(new InitializationMetadata());
                s_InitializationUserFunc = userFunc;
            }
        }



		#endregion


		#region Internal



		/// <summary>
		///     Converts value into string correctly formatted and supported by SQL.
		/// </summary>
		internal static String PrepareValue(object value)
		{
			if (value == null)
				return "NULL";

			// We must know the concrete type
			Type type = value.GetType();

			if (type == typeof(bool))
				return ((bool)value) ? "1" : "0";

			if (type == typeof(DateTime))
			{
				DateTime d = (DateTime)value;
				return "'" + d.ToString("yyyy-MM-dd HH:mm:ss") + "'";
			}

			if (type == typeof(Nullable<DateTime>))
			{
				DateTime? dn = (DateTime?)value;

				if (dn.HasValue)
				{
					return "'" + dn.Value.ToString("yyyy-MM-dd HH:mm:ss") + "'";
				}
				else
				{
					return dn.ToString();
				}
			}

			if (type == typeof(Guid) || type == typeof(String) || type == typeof(Char) || type == typeof(Char[]))
				return "'" + value.ToString() + "'";


			// return normal
			return value.ToString();
		}


		/// <summary>
		///     Get SQL column mapping to the respective propertyName.
		/// </summary>
		internal static String GetMappingForProperty(Type t, String propertyName)
		{
			TypeSchema schema = s_TypesSchemaMapper[t];
			Debug.Assert(schema != null);

			ColumnMapping cm = schema.Columns[propertyName];
			Debug.Assert(cm != null);

			return cm.ToSqlTableColumn;
		}




		/// <summary>
		///     Loads or adds metadata (Schema) to the static dictionary that holds all the information related to all types handled by OMapper.
		/// </summary>
		/// <param name="type">The type that will cause metadata to be loaded or added.</param>
		internal static TypeSchema AddMetadataFor(Type type)
		{
			Debug.Assert(type != null);
            SpinWait sWait = new SpinWait();

			do
			{
				TypeSchema s;

				if (s_TypesSchemaMapper.TryGetValue(type, out s)) // Typically, this is the most common case to occur
					return s;

				//
				// Schema must be setted! - multiple threads can be here;
				// Threads can be here concurrently when a new type is added, 
				// or then 2 or more threads are setting the same type metadata
				// 

				Dictionary<Type, TypeSchema> backup = s_TypesSchemaMapper;                   // Get a local copy for each thread.
				var newSchema = OMapper.NewCopyWithAddedTypeSchema(type);                   // Copy and add metadata for specific Type


#pragma warning disable 420

				if (s_TypesSchemaMapper == backup && Interlocked.CompareExchange(ref s_TypesSchemaMapper, newSchema, backup) == backup)
					return newSchema[type];
#pragma warning restore 420

                sWait.SpinOnce();

            }
			while (true);
		}


		#endregion


		#region Private

		/// <summary>
		///     Create TypeSchema object representing the type and type properties in database.
		/// </summary>
		private static TypeSchema CreateSchema(Type type)
		{
			TypeSchema newSchema = new TypeSchema(type);

			// Search for Table attribute on the type
			foreach (object o in type.GetCustomAttributes(true))
			{
				Table t = o as Table;

				if (t != null)
				{
					newSchema.TableName = t.OverridedName;       // override the default name
					break;                                          // We are done.
				}
			}

			// Iterate over each property of the type
			foreach (PropertyInfo pi in type.GetProperties(s_PropertiesFlags))
			{
				bool mapProperty = true;                                    // Always to map, unless specified Exclude costum attribute
				bool isPrimaryKey = false;                                  // Only if attribute were found, sets this flag to true
				bool isIdentity = false;                                    // For each type, we must have only one Entity

				ColumnMapping columnMapping = new ColumnMapping(pi.Name);      // By convention all mappings match the propertyName


				// Iterate over each attribute on context property
				foreach (object o in pi.GetCustomAttributes(false))
				{
					if (o is Exclude)
					{
						mapProperty = false;
						break;                                              // break immediately the inner loop - and don't map this property
					}

					PrimaryKey k = o as PrimaryKey;

					if (k != null)
					{
						isPrimaryKey = true;
						continue;
					}

					Identity i = o as Identity;

					if (i != null)
					{
						isIdentity = true;
						continue;
					}

					BindFrom bf = o as BindFrom;

					if (bf != null)
					{
						columnMapping.FromResultSetColumn = bf.OverridedReadColumn;      // override read column behavior
						continue;
					}

					BindTo bt = o as BindTo;

					if (bt != null)
					{
						columnMapping.ToSqlTableColumn = bt.OverridedSqlColumn;          // override CUD behavior
						continue;
					}

					StoredProc sp = o as StoredProc;

					if (sp != null)
					{
						ProcMapping pm = new ProcMapping(pi.Name, sp.Mode);

						if (sp.ParameterName != null)
							pm.Map.To = sp.ParameterName;                            // override sp parameter

						newSchema.Procedures.Add(pi.Name, pm);
						continue;
					}
				}

				if (mapProperty)
				{

					//
					// We are here if Exclude wasn't present on the property
					// 

					newSchema.Columns.Add(pi.Name, columnMapping);

					if (isPrimaryKey)
					{
						// Add on keys collection
						newSchema.Keys.Add(pi.Name, new KeyMapping(columnMapping.ToSqlTableColumn, pi.Name));
					}

					if (isIdentity)
					{
						//
						// Only can exist one identity!
						//

						if (newSchema.IdentityPropertyName != null)
							throw new InvalidOperationException("Type {0} cannot have multiple identity columns".Frmt(type.Name));

						newSchema.IdentityPropertyName = pi.Name;
					}
				}
			}

			return newSchema;
		}



		/// <summary>
		///     Creates a new dictionary with previous dictionary containing all information for previous types and the new added type.
		/// </summary>
		private static Dictionary<Type, TypeSchema> NewCopyWithAddedTypeSchema(Type type)
		{

			TypeSchema schema = OMapper.CreateSchema(type);
			return new Dictionary<Type, TypeSchema>(s_TypesSchemaMapper) { { type, schema } };
		}


        #endregion


        #endregion Static Methods











        #region Instance Methods




        #region Public





        /// <summary>
        ///     Returns the connection that OMapper is holding underneath.
        /// </summary>
        public DbConnection Connection
        {
            get { return m_connection; }
        }

        /// <summary>
        ///   Free the DbConnection associated with the OMapper  
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }




        /// <summary>
        ///     Maps the result set to a list of T by convention, and leave the possibility to pass the commandType, commandText and DbParameters.
        /// </summary>
        /// <typeparam name="T">The type of the object that you want to Map</typeparam>
        /// <param name="commandType">The type of the command</param>
        /// <param name="commandText">If using stored procedure, must be the stored procedure name, otherwise the dynamic sql</param>
        /// <param name="parameters">The parameters that command use. (optional)</param>
        /// <returns>A list of objects with their properties filled that aren't annotated with [Exclude] attribute</returns>
        public IList<T> Select<T>(CommandType commandType, string commandText, params DbParameter[] parameters) where T : class
		{
			// Lock-Free
			AddMetadataFor(typeof(T));

			//
			// If we are here, the properties for specific type are filled 
			// and never be touched (modified) again for the type.
			// 

			// Open connection if not opened
			OpenConnection();

			// Command Setup parameters
			DbCommand comm = CmdForConnection(commandType, commandText);

			// Set parameters
			if (parameters != null && parameters.Length > 0)
				comm.Parameters.AddRange(parameters);

			return MapTo<T>(comm.ExecuteReader());
		}


		/// <summary>
		///     Maps the result sets to the array of objects where index 0 contains the first result set, and index 1, the second result set.
		/// </summary>
		/// <typeparam name="T1">The type of the object that you want to Map in the first result set</typeparam>
		/// <typeparam name="T2">The type of the object that you want to Map in the second result set</typeparam>
		/// <param name="commandType">The type of the command</param>
		/// <param name="commandText">If using stored procedure, must be the stored procedure name, otherwise the dynamic sql</param>
		/// <param name="parameters">The parameters that command use. (optional)</param>
		/// <returns>An array of objects with 2 positions, where the first position contains a list of T1 and the second position with a list of T2 objects.</returns>
		public object[] Select<T1, T2>(CommandType commandType, string commandText, params DbParameter[] parameters)
			where T1 : class
			where T2 : class
		{
			// Lock-Free
			AddMetadataFor(typeof(T1));
			AddMetadataFor(typeof(T2));

			//
			// If we are here, the properties for specific type are filled 
			// and never be touched (modified) again for the type.
			// 

			// Open connection if not opened
			OpenConnection();

			// Command Setup parameters
			DbCommand comm = CmdForConnection(commandType, commandText);

			// Set parameters
			if (parameters != null && parameters.Length > 0)
				comm.Parameters.AddRange(parameters);

			// Allocate memory
			object[] result = new object[2];
			DbDataReader reader;

			result[0] = MapTo<T1>(reader = comm.ExecuteReader(), false);
			reader.NextResult();
			result[1] = MapTo<T2>(reader);

			// return reference
			return result;
		}




		/// <summary>
		///     Maps the result sets to the array of objects where index 0 contains the first result set, index 1 the second result set and index 2, the third result set.
		/// </summary>
		/// <typeparam name="T1">The type of the object that you want to Map in the first result set</typeparam>
		/// <typeparam name="T2">The type of the object that you want to Map in the second result set</typeparam>
		/// <typeparam name="T3">The type of the object that you want to Map in the third result set</typeparam>
		/// <param name="commandType">The type of the command</param>
		/// <param name="commandText">If using stored procedure, must be the stored procedure name, otherwise the dynamic sql</param>
		/// <param name="parameters">The parameters that command use. (optional)</param>
		/// <returns>An array of objects with 3 positions, where the first position contains a list of T1, the second position with a list of T2, and the third position with a list of T3 objects.</returns>
		public object[] Select<T1, T2, T3>(CommandType commandType, string commandText, params DbParameter[] parameters)
			where T1 : class
			where T2 : class
			where T3 : class
		{
			// Lock-Free
			AddMetadataFor(typeof(T1));
			AddMetadataFor(typeof(T2));
			AddMetadataFor(typeof(T3));

			//
			// If we are here, the properties for specific type are filled 
			// and never be touched (modified) again for the type.
			// 

			// Open connection if not opened
			OpenConnection();

			// Command Setup parameters
			DbCommand comm = CmdForConnection(commandType, commandText);

			// Set parameters
			if (parameters != null && parameters.Length > 0)
				comm.Parameters.AddRange(parameters);

			// Allocate memory
			object[] result = new object[3];
			DbDataReader reader;

			result[0] = MapTo<T1>(reader = comm.ExecuteReader(), false);
			reader.NextResult();
			result[1] = MapTo<T2>(reader, false);
			reader.NextResult();
			result[2] = MapTo<T3>(reader);

			// return reference
			return result;
		}








        /// <summary>
        ///     Build DbParameters that match with the mode and execute the stored procedure.
        ///     All stored procedures must have the same name of parameters for all modes when using this method.
        /// </summary>
        /// <typeparam name="T">The type that must have their properties annotated with [StoredProc]</typeparam>
        /// <param name="obj">The object that you want to retrieve the information and build sql parameters dynamically based on their values</param>
        /// <param name="mode">The mode of the procedure</param>
        /// <param name="procedureName">The name of the procedure</param>
        /// <returns>The number of affected rows in database</returns>
        public int ExecuteProc<T>(T obj, SPMode mode, string procedureName) where T : class
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            Type type = obj.GetType();

            // Lock-Free
            AddMetadataFor(type);


            //
            // If we are here, the properties for specific type are filled 
            // and never be touched (modified) again for the type.
            // 

            // Open connection if not opened
            OpenConnection();

            // Command Setup parameters
            DbCommand cmd = CmdForConnection(CommandType.StoredProcedure, procedureName);


            //

            Type objRepresentor = obj.GetType();
            TypeSchema schema = s_TypesSchemaMapper[type];

            foreach (ProcMapping pm in schema.Procedures.Values)
            {
                if ((pm.Mode & mode) == mode)      // Mode match!
                {
                    object value = objRepresentor.GetProperty(pm.Map.From, s_PropertiesFlags).GetValue(obj, null);
                    object value2 = value == null ? DBNull.Value : value;

                    cmd.Parameters.Add(new SqlParameter(pm.Map.To, value2));
                }
            }


            return cmd.ExecuteNonQuery();
        }


        /// <summary>
        ///     Execute the query on the database.
        /// </summary>
        /// <param name="commandType">The type of the command</param>
        /// <param name="commandText">If using stored procedure, must be the stored procedure name, otherwise the dynamic sql</param>
        /// <param name="parameters">The parameters that command use. (optional)</param>
        /// <returns>The number of affected rows in database</returns>
        public int Execute(CommandType commandType, string commandText, params DbParameter[] parameters)
        {
            if (string.IsNullOrEmpty(commandText))
                throw new ArgumentException("commandText");

            // Open connection if not opened
            OpenConnection();

            // Command Setup parameters
            DbCommand comm = CmdForConnection(commandType, commandText);

            // Set parameters
            if (parameters != null && parameters.Length > 0)
                comm.Parameters.AddRange(parameters);

            return comm.ExecuteNonQuery();
        }


        /// <summary>
        ///     Execute the query on the database.
        /// </summary>
        /// <param name="commandType">The type of the command</param>
        /// <param name="commandText">If using stored procedure, must be the stored procedure name, otherwise the dynamic sql</param>
        /// <param name="parameters">The parameters that command use. (optional)</param>
        /// <returns>The first column of the first row in the ResultSet returned by the query</returns>
        public object ExecuteScalar(CommandType commandType, string commandText, params DbParameter[] parameters)
        {
            if (string.IsNullOrEmpty(commandText))
                throw new ArgumentException("commandText");

            // Open connection if not opened
            OpenConnection();

            // Command Setup parameters
            DbCommand comm = CmdForConnection(commandType, commandText);

            // Set parameters
            if (parameters != null && parameters.Length > 0)
                comm.Parameters.AddRange(parameters);

            return comm.ExecuteScalar();
        }







        #endregion





        #region Protected




        protected DbTransaction OpenConnection()
		{
			if (m_connection == null)
				throw new NullReferenceException("connection is null");

			// Try open the connection if not opened!
			if (m_connection.State != ConnectionState.Open)
				m_connection.Open();

			if (m_transaction == null)
				m_transaction = m_connection.BeginTransaction(CURRENT_ISOLATION_LEVEL);

			return m_transaction;

		}

		protected void CloseConnection()
		{
			if (m_connection == null)
				throw new NullReferenceException("connection is null");

			if (m_transaction != null)
			{
				m_transaction.Commit();
				m_transaction.Dispose();
				m_transaction = null;
			}

			// Try open the connection if not opened!
			if (m_connection.State != ConnectionState.Closed)
				m_connection.Close();
		}


		protected DbCommand CmdForConnection(CommandType type, String text)
		{
			DbCommand comm = m_connection.CreateCommand();

			comm.CommandType = type;
			comm.CommandText = text;
			comm.CommandTimeout = m_commandTimeout;
			comm.Transaction = m_transaction;

			return comm;
		}


		/// <summary>
		///     Iterate over the reader and maps properties that are not excluded to the OMapper.
		/// </summary>
		/// <typeparam name="T">the type of objects being returned</typeparam>
		/// <param name="reader">the reader that is currently pointing to the Result set entry</param>
		/// <param name="CloseDbReader">true will close the reader, otherwise will still be open.</param>
		/// <returns>The list of objects within the reader for the type T</returns>
		protected List<T> MapTo<T>(DbDataReader reader, bool CloseDbReader = true) where T : class
		{
			if (reader == null)
				throw new NullReferenceException("reader cannot be null");

			if (reader.IsClosed)
				throw new InvalidOperationException("reader connection is closed and objects cannot be mapped");

			if (!reader.HasRows)
				return new List<T>();


			Type type = typeof(T);
			TypeSchema schema = s_TypesSchemaMapper[type];

			//
			// If we are here, the properties for specific type are filled 
			// and never be touched (modified) again for the type.
			// 

			// Map cursor lines from database to CLR objects based on T

			List<T> objectsQueue = new List<T>();

			while (reader.Read())
			{
				T newInstance = (T)Activator.CreateInstance(type);
				Type newInstanceRep = newInstance.GetType();            // Mirror instance to reflect newInstance

				// Map properties to the newInstance
				foreach (ColumnMapping map in schema.Columns.Values)
				{
					object value;
					string sqlColumn = map.FromResultSetColumn;

					try { value = reader[sqlColumn]; }
					catch (IndexOutOfRangeException)
					{
						throw new SqlColumnNotFoundException("Sql column with name: {0} is not found".Frmt(sqlColumn));
					}

					PropertyInfo ctxProperty = newInstanceRep.GetProperty(map.ClrProperty);

					//
					// Nullable condition checker!
					//

					if (value.GetType() == typeof(DBNull))
					{
						if (ctxProperty.PropertyType.IsPrimitive)
						{
							throw new PropertyMustBeNullable(
								"Property {0} must be nullable for mapping a null value".Frmt(ctxProperty.Name)
							);
						}

						value = null;
					}



					//
					// Set property value
					// 


					ctxProperty.SetValue(newInstance, value, null);     // WARNING: Conversion Types..
				}

				// Add element to the collection
				objectsQueue.Add(newInstance);

				if (OnMappingOneEntry != null)
					OnMappingOneEntry(newInstance);
			}

			if (CloseDbReader)
			{
				// Free Connection Resources
				reader.Close();
				reader.Dispose();
				CloseConnection();
			}

			return objectsQueue;
		}

		#endregion



		#region Private


		/// <summary>
		///     Free the DbConnection associated with the OMapper.
		/// </summary>
		/// <param name="explicitlyCalled">true if called by developer, otherwise called by finalizer.</param>
		private void Dispose(bool explicitlyCalled)
		{
			if (m_disposed)
				throw new ObjectDisposedException(this.GetType().Name);

			if (explicitlyCalled)
			{
				// get rid of managed resources
				if (m_connection != null)
				{
					if (m_connection.State != ConnectionState.Closed)
						m_connection.Close();

					m_connection.Dispose();
				}

				m_disposed = true;
			}

			// get rid of unmanaged resources            

			//
			// When an object is executing its finalization code, it should not reference other objects, because finalizers do not execute in any particular order. 
			// If an executing finalizer references another object that has already been finalized, the executing finalizer will fail.
		}



        #endregion


        #endregion Instance Methods




        


    }
}
