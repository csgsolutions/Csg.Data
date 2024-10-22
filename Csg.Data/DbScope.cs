using System.Data;

namespace Csg.Data;

/// <summary>
///     A class which provides a database connection and transaction.
/// </summary>
public class DbScope : IDbScope
{
    /// <summary>
    ///     Initializes a new scope with the given connection and transaction.
    /// </summary>
    /// <param name="connection"></param>
    public DbScope(IDbConnection connection)
    {
        Connection = connection;
    }

    /// <summary>
    ///     Initializes a new scope with the given connection and transaction
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="transaction"></param>
    public DbScope(IDbConnection connection, IDbTransaction transaction)
    {
        Connection = connection;
        Transaction = transaction;
    }

    /// <summary>
    ///     Gets or sets the default timeout duration in seconds applied to command objects created by the scope.
    /// </summary>
    public virtual int CommandTimeout { get; set; }

    /// <summary>
    ///     Gets a reference to the connection instance associated with this scope.
    /// </summary>
    public virtual IDbConnection Connection { get; protected set; }

    /// <summary>
    ///     Gets a reference to the transaction instance associated with this scope.
    /// </summary>
    public virtual IDbTransaction Transaction { get; protected set; }

    /// <summary>
    ///     Commits the database transaction if one exists, otherwise does nothing.
    /// </summary>
    public virtual bool Commit()
    {
        if (Transaction != null)
        {
            Transaction.Commit();
            Transaction = null;
            return true;
        }

        return false;
    }

    /// <summary>
    ///     Disposes the underlying connection and transaction.
    /// </summary>
    public virtual void Dispose()
    {
        if (Transaction != null)
        {
            Transaction.Dispose();
            Transaction = null;
        }

        Connection.Dispose();
    }

    /// <summary>
    ///     Rolls back the transaction without disposing the connection.
    /// </summary>
    public virtual void RollbackTransaction()
    {
        if (Transaction != null)
        {
            Transaction.Rollback();
            Transaction = null;
        }
    }

    /// <summary>
    ///     Ensures the current scope has an active transaction, and if not, creates one.
    /// </summary>
    /// <param name="isoLevel"></param>
    public bool EnsureTransaction(IsolationLevel isoLevel)
    {
        if (Transaction == null)
        {
            Transaction = Connection.BeginTransaction(isoLevel);
            return true;
        }

        return false;
    }
}