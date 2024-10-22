namespace Csg.Data.Sql;

/// <summary>
///     Represents an EXISTS({selectStatement}) filter critera in a SQL statement.
/// </summary>
public class SqlExistFilter : ISqlFilter
{
    /// <summary>
    ///     Initializes an instance of the filter.
    /// </summary>
    /// <param name="selectStatement"></param>
    public SqlExistFilter(SqlSelectBuilder selectStatement)
    {
        Statement = selectStatement;
    }

    /// <summary>
    ///     Gets or sets the SQL statement that will be executed inside the EXISTS.
    /// </summary>
    public SqlSelectBuilder Statement { get; set; }

    void ISqlStatementElement.Render(SqlTextWriter writer, SqlBuildArguments args)
    {
        writer.WriteBeginGroup();
        writer.Write("EXISTS ");
        writer.WriteBeginGroup();
        Statement.Render(writer, args);
        writer.WriteEndGroup();
        writer.WriteEndGroup();
    }
}