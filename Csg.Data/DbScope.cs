using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csg.Data
{    
    /// <summary>
    /// A class which provides a database connection and transaction.
    /// </summary>
    public class DbScope : IDbScope
    { 
        /// <summary>
        /// Initializes a new scope with the given connection and transaction.
        /// </summary>
        /// <param name="connection"></param>
        public DbScope(System.Data.IDbConnection connection)
        {
            this.Connection = connection;
        }

        /// <summary>
        /// Initializes a new scope with the given connection and transaction
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        public DbScope(System.Data.IDbConnection connection, System.Data.IDbTransaction transaction)
        {
            this.Connection = connection;
            this.Transaction = transaction;
        }

        /// <summary>
        /// Gets a reference to the connection instance associated with this scope.
        /// </summary>
        public virtual System.Data.IDbConnection Connection { get; protected set; }

        /// <summary>
        /// Gets a reference to the transaction instance associated with this scope.
        /// </summary>
        public virtual System.Data.IDbTransaction Transaction { get; protected set; }

        /// <summary>
        /// Gets or sets the default timeout duration in seconds applied to command objects created by the scope.
        /// </summary>
        public virtual int CommandTimeout { get; set; }

        /// <summary>
        /// Commits the database transaction if one exists, otherwise does nothing.
        /// </summary>
        public virtual bool Commit()
        {
            if (this.Transaction != null)
            {
                this.Transaction.Commit();
                this.Transaction = null;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Disposes the underlying connection and transaction.
        /// </summary>
        public virtual void Dispose()
        {
            if (this.Transaction != null)
            {
                this.Transaction.Dispose();
            }
            this.Connection.Dispose();
        }

        /// <summary>
        /// Ensures the current scope has an active transaction, and if not, creates one.
        /// </summary>
        /// <param name="isoLevel"></param>
        public bool EnsureTransaction(IsolationLevel isoLevel)
        {
            if (this.Transaction == null)
            {
                this.Transaction = this.Connection.BeginTransaction(isoLevel);
                return true;
            }

            return false;
        }
    }   
}
