using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Csg.Data.Sql;

namespace Csg.Data;

/// <summary>
///     Provides a query command builder to create and execute a SELECT statement against a database.
/// </summary>
[DebuggerDisplay("CommandText = {Render().CommandText}, Parameters = {ParameterString()}")]
public class DbQueryBuilder : IDbQueryBuilder, ISqlStatementElement
{
    /// <summary>
    ///     Gets or sets a value that indicates if the query builder will generate formatted SQL by default. Applies to all
    ///     instances.
    /// </summary>
    public static bool GenerateFormattedSql = true;

    private DbQueryBuilder(IDbConnection connection, IDbTransaction transaction)
    {
        Connection = connection;
        Transaction = transaction;
    }

    /// <summary>
    ///     Creates a new instance using the given table expression and connection.
    /// </summary>
    /// <param name="sql">The name of a table, a table expression, or other object that can be the target of a SELECT query.</param>
    /// <param name="connection">The database connection.</param>
    public DbQueryBuilder(string sql, IDbConnection connection) : this(connection, null)
    {
        Root = SqlTableBase.Create(sql);
        Parameters = new List<DbParameterValue>();
        SelectColumns = new List<ISqlColumn>();
        Joins = new List<ISqlJoin>();
        Filters = new List<ISqlFilter>();
        OrderBy = new List<SqlOrderColumn>();
    }

    /// <summary>
    ///     Creates a new instance using the given table expression, connection, and transaction.
    /// </summary>
    /// <param name="sql">The name of a table, a table expression, or other object that can be the target of a SELECT query.</param>
    /// <param name="connection">The database connection.</param>
    /// <param name="transaction">The database transaction.</param>
    public DbQueryBuilder(string sql, IDbConnection connection, IDbTransaction transaction) : this(sql, connection)
    {
        Transaction = transaction;
    }

    /// <summary>
    ///     Gets or sets the collection of joins..
    /// </summary>
    protected ICollection<ISqlJoin> Joins { get; set; }

    /// <summary>
    ///     Gets or sets the collection of filters.
    /// </summary>
    protected ICollection<ISqlFilter> Filters { get; set; }

    /// <summary>
    ///     Returns the root table of the query. This is the table listed immmediately after the FROM clause
    /// </summary>
    public ISqlTable Root { get; protected set; }

    /// <summary>
    ///     When implemented in a derived class, gets a collection of SELECT columns.
    /// </summary>
    public IList<ISqlColumn> SelectColumns { get; protected set; }

    /// <summary>
    ///     Gets or sets a SQL statement that will be prefixed to the rendered query with a statement separater afterwards.
    ///     This can be used to set query options.
    /// </summary>
    public string Prefix { get; set; }

    /// <summary>
    ///     Gets or sets a SQL statment that will be appended to the end of the rendered query after a statement separaeter
    ///     (semicolon).
    /// </summary>
    public string Suffix { get; set; }

    /// <summary>
    ///     Adds a JOIN to the FROM clause of the query.
    /// </summary>
    /// <param name="join"></param>
    public void AddJoin(ISqlJoin join)
    {
        Joins.Add(join);
    }

    /// <summary>
    ///     Adds a WHERE clause to the query.
    /// </summary>
    /// <param name="filter"></param>
    public void AddFilter(ISqlFilter filter)
    {
        Filters.Add(filter);
    }

    /// <summary>
    ///     Gets the list of columns to order by.
    /// </summary>
    public IList<SqlOrderColumn> OrderBy { get; protected set; }

    /// <summary>
    ///     Gets the connection associated with the query.
    /// </summary>
    public IDbConnection Connection { get; }

    /// <summary>
    ///     Gets the transaction associated with the query.
    /// </summary>
    public IDbTransaction Transaction { get; }

    /// <summary>
    ///     Gets or sets the query execution timeout.
    /// </summary>
    public int CommandTimeout { get; set; }

    /// <summary>
    ///     Gets or sets a value that indicates if SELECT DISTINCT will be used.
    /// </summary>
    public bool Distinct { get; set; }

    /// <summary>
    ///     Gets the paging options used with the query
    /// </summary>
    public SqlPagingOptions? PagingOptions { get; set; }

    /// <summary>
    ///     Gets the parameter value collection.
    /// </summary>
    public ICollection<DbParameterValue> Parameters { get; protected set; }

    /// <summary>
    ///     Returns an initalized database command.
    /// </summary>
    /// <returns></returns>
    public IDbCommand CreateCommand()
    {
        var stmt = Render();
        var cmd = stmt.CreateCommand(Connection);

        cmd.Transaction = Transaction;
        cmd.CommandTimeout = CommandTimeout;

        return cmd;
    }

    /// <summary>
    ///     Creates a new instance of <see cref="DbQueryBuilder" /> configured in the same manner as the existing one.
    /// </summary>
    /// <returns></returns>
    public IDbQueryBuilder Fork()
    {
        var builder = new DbQueryBuilder(Connection, Transaction);
        builder.CommandTimeout = CommandTimeout;
        builder.Root = Root;
        builder.Joins = new List<ISqlJoin>(Joins);
        builder.Filters = new List<ISqlFilter>(Filters);
        builder.SelectColumns = new List<ISqlColumn>(SelectColumns);
        builder.Distinct = Distinct;
        builder.Parameters = new List<DbParameterValue>(Parameters);
        builder.OrderBy = new List<SqlOrderColumn>(OrderBy);
        builder.PagingOptions = PagingOptions;
        builder.Prefix = Prefix;
        builder.Suffix = Suffix;
        return builder;
    }

    SqlStatement IDbQueryBuilder.Render()
    {
        return Render();
    }

    void ISqlStatementElement.Render(SqlTextWriter writer, SqlBuildArguments args)
    {
        CreateSelectBuilder(false).Render(writer, args);

        foreach (var param in Parameters) args.Parameters.Add(param);
    }

    /// <summary>
    ///     Gets a SQL statement for the given query.
    /// </summary>
    /// <param name="generateFormattedSql">Indicates if SQL should be indented, have new-line characters, etc.</param>
    /// <returns></returns>
    public SqlStatement Render(bool? generateFormattedSql = null)
    {
        var builder = CreateSelectBuilder(generateFormattedSql);

        var stmt = builder.Render();

        foreach (var param in Parameters) stmt.Parameters.Add(param);

        return stmt;
    }

    /// <summary>
    ///     Gets a SQL statement for the given query.
    /// </summary>
    /// <param name="generateFormattedSql">Indicates if SQL should be indented, have new-line characters, etc.</param>
    /// <returns></returns>
    protected SqlSelectBuilder CreateSelectBuilder(bool? generateFormattedSql = null)
    {
        return new SqlSelectBuilder(Root, Joins, SelectColumns, Filters, OrderBy)
        {
            SelectDistinct = Distinct,
            GenerateFormattedSql = generateFormattedSql ?? GenerateFormattedSql,
            PagingOptions = PagingOptions,
            Prefix = Prefix,
            Suffix = Suffix
        };
    }

    /// <summary>
    ///     Gets the command text returned from the <see cref="Render" /> method.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return Render().CommandText;
    }

    internal string ParameterString()
    {
        return string.Join(", ", Render().Parameters.Select(s => s.ParameterName + "=" + s.Value));
    }
}