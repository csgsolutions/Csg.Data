namespace Csg.Data.Sql;

/// <summary>
///     Represents a SQL table.
/// </summary>
public interface ISqlTable : ISqlStatementElement
{
    /// <summary>
    ///     Adds the table to the given arguments colletion so it can be referenced.
    /// </summary>
    /// <param name="args"></param>
    void Compile(SqlBuildArguments args);
}