namespace Csg.Data.Sql;

/// <summary>
///     Provides an interface for classes that provide T-SQL field/column values in a SELECT statement.
/// </summary>
public interface ISqlColumn : ISqlTableElement, ISqlStatementElement
{
    /// <summary>
    ///     When implemented in a derived class, gets a value that indicates if the column is using an aggregate function.
    /// </summary>
    /// <remarks>This value is used by the <see cref="SqlSelectBuilder" /> to determine if a GROUP BY is needed in the query.</remarks>
    bool IsAggregate { get; }

    /// <summary>
    ///     When implemented in a derived class, renders the portion of a SELECT column that would be rendered before the AS
    ///     keyword.
    /// </summary>
    /// <param name="writer">An instance of a T-SQL compatible text writer.</param>
    /// <param name="args">An instance of <see cref="SqlBuildArguments" />.</param>
    void RenderValueExpression(SqlTextWriter writer, SqlBuildArguments args);

    /// <summary>
    ///     When implemented in a derived class, gets the portion of a SELECT column that would be renderd after the AS
    ///     keyword.
    /// </summary>
    /// <returns>A string</returns>
    string GetAlias();
}