using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Csg.Data
{
    /// <summary>
    /// Provides a class that ensures the given db scope has an active transaciton, and if not, begins one.
    /// </summary>
    public class DbTransactionScope : IDisposable
    {
        private readonly IDbScope _scope;

        /// <summary>
        /// Initializes a transaction scope with the given isolation level.
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="isoLevel"></param>
        public DbTransactionScope(IDbScope scope, IsolationLevel isoLevel)
        {
            _scope = scope;
            this.OwnsTransaction = scope.EnsureTransaction(isoLevel);
        }

        /// <summary>
        /// Gets a value that indicates if this object created the transaction.
        /// </summary>
        public bool OwnsTransaction { get; private set; }
        
        /// <summary>
        /// Attempts to commit the transaction.
        /// </summary>
        /// <returns>True if this object created the transaction, False otherwise.</returns>
        public bool TryCommit()
        {
            if (this.OwnsTransaction)
            {
                _scope.Commit();
                this.OwnsTransaction = false;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Rolls back the underlying transaction if <see cref="OwnsTransaction"/> is true.
        /// </summary>
        public void Dispose()
        {
            if (this.OwnsTransaction)
            {
                _scope.RollbackTransaction();
            }
        }
    }
}
