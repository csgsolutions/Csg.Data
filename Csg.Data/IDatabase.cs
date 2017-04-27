namespace Csg.Data
{
    /// <summary>
    /// Represents an database
    /// </summary>
    public interface IDatabase
    {
        /// <summary>
        /// When implemented in a derived class, gets the provider specific connection string for the database.
        /// </summary>
        string ConnectionString { get; }

        /// <summary>
        /// When implemented in a derived class, gets an instance of the <see cref="System.Data.Common.DbProviderFactory"/> for the provider associated with the database. 
        /// </summary>
        System.Data.Common.DbProviderFactory Factory { get; }

        /// <summary>
        /// When implemented in a derived class, 
        /// </summary>
        /// <returns></returns>
        System.Data.Common.DbConnection CreateConnection();

        /// <summary>
        /// When implemented in a derived class, opens a database connection and returns an <see cref="IDbScope"/>/>
        /// </summary>
        /// <returns></returns>
        IDbScope Begin();

        /// <summary>
        /// When implemented in a derived class, opens a database connection and begins a transaction.
        /// </summary>
        /// <returns></returns>
        IDbScope BeginTransaction();

        /// <summary>
        /// When implemented in a derived class, opens a database connection and begins a transaction with the given isolation level.
        /// </summary>
        /// <returns></returns>
        IDbScope BeginTransaction(System.Data.IsolationLevel isoLevel);
    }

}

