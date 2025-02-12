using System;
using System.Data;

namespace Csg.Data;

/// <summary>
///     Represents a connection and transaction for a database.
/// </summary>
public interface IDbScope : IDisposable
{
    /// <summary>
    ///     When implemented in a derived class, gets an open instance of <see cref="System.Data.IDbConnection" />.
    /// </summary>
    IDbConnection Connection { get; }

    /// <summary>
    ///     When implemented in a derived class, gets the active <see cref="System.Data.IDbTransaction" />.
    /// </summary>
    IDbTransaction Transaction { get; }

    /// <summary>
    ///     When implemented in a derived class, commits the database transaction if one exists, otherwise does nothing.
    /// </summary>
    /// <returns>True if a transaction existed to commit, false otherwise.</returns>
    bool Commit();

    /// <summary>
    ///     When implemented in a derived class, ensures the current scope has an active transaction, and if not, creates one.
    /// </summary>
    /// <param name="isoLevel"></param>
    bool EnsureTransaction(IsolationLevel isoLevel);

    /// <summary>
    ///     When implemented in a derived class, rolls back the transaction without disposing the connection.
    /// </summary>
    void RollbackTransaction();
}