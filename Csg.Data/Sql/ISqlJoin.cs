namespace Csg.Data.Sql;

/// <summary>
///     Represents a SQL join expression to a table
/// </summary>
public interface ISqlJoin : ISqlStatementElement
{
    /// <summary>
    ///     Gets the joined table.
    /// </summary>
    ISqlTable JoinedTable { get; }
}