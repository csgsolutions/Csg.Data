using System.Data;
using System.Data.Common;

namespace Csg.Data;

/// <summary>
///     Provides a base implementation for a class which implements <see cref="IDatabase" />.
/// </summary>
/// <typeparam name="TScope"></typeparam>
public abstract class DatabaseBase<TScope> : IDatabase
    where TScope : IDbScope
{
    /// <summary>
    ///     Initializes a new instance with the given connection string and factory.
    /// </summary>
    /// <param name="connectionString"></param>
    /// <param name="factory"></param>
    public DatabaseBase(string connectionString, DbProviderFactory factory)
    {
        ConnectionString = connectionString;
        Factory = factory;
    }

    /// <summary>
    ///     Gets the connection string associated with the database.
    /// </summary>
    public virtual string ConnectionString { get; protected set; }

    /// <summary>
    ///     Gets the factory passed to the constructor.
    /// </summary>
    public virtual DbProviderFactory Factory { get; protected set; }

    /// <summary>
    ///     Creates an instance of a <see cref="System.Data.Common.DbConnection" /> using the configured provider name and
    ///     connection string.
    /// </summary>
    /// <returns></returns>
    public virtual DbConnection CreateConnection()
    {
        var connection = Factory.CreateConnection();
        connection.ConnectionString = ConnectionString;
        return connection;
    }

    /// <summary>
    ///     Creates an open connection wrapped in a <see cref="IDbScope" />.
    /// </summary>
    /// <returns></returns>
    public abstract TScope Begin();

    /// <summary>
    ///     Creates an open connection and transaction wrapped in a <see cref="IDbScope" />.
    /// </summary>
    /// <returns></returns>
    public abstract TScope BeginTransaction();

    /// <summary>
    ///     Creates an open connection wrapped in a <see cref="IDbScope" /> using the given transaction isolation level.
    /// </summary>
    /// <param name="isoLevel"></param>
    /// <returns></returns>
    public abstract TScope BeginTransaction(IsolationLevel isoLevel);


    #region Explcits for IDbScope

    IDbScope IDatabase.Begin()
    {
        return Begin();
    }

    IDbScope IDatabase.BeginTransaction()
    {
        return BeginTransaction();
    }

    IDbScope IDatabase.BeginTransaction(IsolationLevel isoLevel)
    {
        return BeginTransaction(isoLevel);
    }

    #endregion
}