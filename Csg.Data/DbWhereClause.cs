using System.Collections.Generic;
using Csg.Data.Sql;

namespace Csg.Data;

/// <summary>
///     Used to build a set of fitlers with the fluent api.
/// </summary>
public class DbQueryWhereClause : IDbQueryWhereClause
{
    /// <summary>
    ///     Initializes a new instance with the given table and AND/OR logic.
    /// </summary>
    /// <param name="root"></param>
    /// <param name="logic"></param>
    public DbQueryWhereClause(ISqlTable root, SqlLogic logic)
    {
        Root = root;
        Filters = new SqlFilterCollection { Logic = logic };
    }

    /// <summary>
    ///     Gets or sets the filter collection
    /// </summary>
    public SqlFilterCollection Filters { get; set; }

    /// <summary>
    ///     Gets or sets the root table associated with the WHERE clause.
    /// </summary>
    public ISqlTable Root { get; }

    /// <summary>
    ///     Adds a filter to the where clause
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    public IDbQueryWhereClause AddFilter(ISqlFilter filter)
    {
        Filters.Add(filter);
        return this;
    }

    /// <summary>
    ///     Applies filters to the given query builder.
    /// </summary>
    /// <param name="builder"></param>
    public void ApplyToQuery(IDbQueryBuilder builder)
    {
        if (Filters.Count > 0) builder.AddFilter(Filters);
    }

    /// <summary>
    ///     Adds filters to teh given collection.
    /// </summary>
    /// <param name="filters"></param>
    public void ApplyTo(ICollection<ISqlFilter> filters)
    {
        if (Filters.Count > 0) filters.Add(Filters);
    }
}