using Csg.Data.Sql;

namespace Csg.Data;

/// <summary>
///     When implemented in a derived class, provides the ability to add filters to a query expression.
/// </summary>
public interface IDbQueryWhereClause
{
    /// <summary>
    ///     Gets the root table of the underlying query.
    /// </summary>
    ISqlTable Root { get; }

    /// <summary>
    ///     Adds the given filter to the underlying query expression.
    /// </summary>
    /// <param name="filter"></param>
    IDbQueryWhereClause AddFilter(ISqlFilter filter);
}