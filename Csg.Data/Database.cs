using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Csg.Data
{
    /// <summary>
    /// Provides a base implementation for a class which implements <see cref="IDatabase"/>.
    /// </summary>
    /// <typeparam name="TScope"></typeparam>
    public abstract class DatabaseBase<TScope> : IDatabase
        where TScope : IDbScope
    {
        /// <summary>
        /// Initializes a new instance with the given connection string and factory.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="factory"></param>
        public DatabaseBase(string connectionString, DbProviderFactory factory)
        {
            this.ConnectionString = connectionString;
            this.Factory = factory;
        }

        /// <summary>
        /// Gets the connection string associated with the database.
        /// </summary>
        public virtual string ConnectionString { get; protected set; }

        /// <summary>
        /// Gets the factory passed to the constructor. 
        /// </summary>
        public virtual DbProviderFactory Factory { get; protected set; }
        
        /// <summary>
        /// Creates an open connection wrapped in a <see cref="IDbScope"/>.
        /// </summary>
        /// <returns></returns>
        public abstract TScope Begin();

        /// <summary>
        /// Creates an open connection and transaction wrapped in a <see cref="IDbScope"/>.
        /// </summary>
        /// <returns></returns>
        public abstract TScope BeginTransaction();

        /// <summary>
        /// Creates an open connection wrapped in a <see cref="IDbScope"/> using the given transaction isolation level.
        /// </summary>
        /// <param name="isoLevel"></param>
        /// <returns></returns>
        public abstract TScope BeginTransaction(IsolationLevel isoLevel);

        /// <summary>
        /// Creates an instance of a <see cref="System.Data.Common.DbConnection"/> using the configured provider name and connection string. 
        /// </summary>
        /// <returns></returns>
        public virtual DbConnection CreateConnection()
        {
            var connection = this.Factory.CreateConnection();
            connection.ConnectionString = this.ConnectionString;
            return connection;
        }


        #region Explcits for IDbScope

        IDbScope IDatabase.Begin()
        {
            return this.Begin();
        }

        IDbScope IDatabase.BeginTransaction()
        {
            return this.BeginTransaction();
        }

        IDbScope IDatabase.BeginTransaction(IsolationLevel isoLevel)
        {
            return this.BeginTransaction(isoLevel);
        }

        #endregion
    }
}
