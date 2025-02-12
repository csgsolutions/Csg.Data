namespace Csg.Data.Sql;

/// <summary>
///     Used to specified the wildcard decoration on T-SQL LIKE operator based filters.
/// </summary>
public enum SqlWildcardDecoration
{
    BeginsWith,
    Contains,
    EndsWith,
    None
}