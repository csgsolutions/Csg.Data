namespace Csg.Data.Sql;

/// <summary>
///     Represents an element that has a reference to a <see cref="ISqlTable" />.
/// </summary>
public interface ISqlTableElement
{
    /// <summary>
    ///     Gets or sets the table reference.
    /// </summary>
    ISqlTable Table { get; set; }
}