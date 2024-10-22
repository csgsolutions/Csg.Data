namespace Csg.Data.Sql;

/// <summary>
///     Represents the options of table join types.
/// </summary>
public enum SqlJoinType
{
    /// <summary>
    ///     INNER JOIN
    /// </summary>
    Inner,

    /// <summary>
    ///     LEFT JOIN
    /// </summary>
    Left,

    /// <summary>
    ///     CROSS JOIN
    /// </summary>
    Cross
}