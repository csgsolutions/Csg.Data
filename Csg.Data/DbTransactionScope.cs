using System;
using System.Data;

namespace Csg.Data;

/// <summary>
///     Provides a class that ensures the given db scope has an active transaciton, and if not, begins one.
/// </summary>
public class DbTransactionScope : IDisposable
{
    private readonly IDbScope _scope;

    /// <summary>
    ///     Initializes a transaction scope with the given isolation level.
    /// </summary>
    /// <param name="scope"></param>
    /// <param name="isoLevel"></param>
    public DbTransactionScope(IDbScope scope, IsolationLevel isoLevel)
    {
        _scope = scope;
        OwnsTransaction = scope.EnsureTransaction(isoLevel);
    }

    /// <summary>
    ///     Gets a value that indicates if this object created the transaction.
    /// </summary>
    public bool OwnsTransaction { get; private set; }

    /// <summary>
    ///     Rolls back the underlying transaction if <see cref="OwnsTransaction" /> is true.
    /// </summary>
    public void Dispose()
    {
        if (OwnsTransaction) _scope.RollbackTransaction();
    }

    /// <summary>
    ///     Attempts to commit the transaction.
    /// </summary>
    /// <returns>True if this object created the transaction, False otherwise.</returns>
    public bool TryCommit()
    {
        if (OwnsTransaction)
        {
            _scope.Commit();
            OwnsTransaction = false;
            return true;
        }

        return false;
    }
}