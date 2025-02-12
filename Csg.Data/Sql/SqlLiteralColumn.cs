namespace Csg.Data.Sql;

/// <summary>
///     Represents a column in a SELECT statement defined as a literal value.
/// </summary>
public class SqlLiteralColumn<TValue> : ISqlColumn
{
    /// <summary>
    ///     Initializes a new instance with the given value.
    /// </summary>
    /// <param name="value"></param>
    public SqlLiteralColumn(TValue value)
    {
        Value = value;
    }

    /// <summary>
    ///     Initializes a new instance with the given value and alias.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="alias"></param>
    public SqlLiteralColumn(TValue value, string alias)
    {
        Value = value;
        Alias = alias;
    }

    /// <summary>
    ///     Gets or sets the literal value.
    /// </summary>
    public TValue Value { get; set; }

    /// <summary>
    ///     Gets or sets the column alias to use when rendering the SELECT statement.
    /// </summary>
    public string Alias { get; set; }

    /// <summary>
    ///     Gets a value that indicates if the column is an aggregate.
    /// </summary>
    public bool IsAggregate => false;

    /// <summary>
    ///     Get or sets the <see cref="ISqlTable" /> table associated with the column.
    /// </summary>
    public ISqlTable Table { get; set; }

    /// <summary>
    ///     Gets the column alias to use when rendering the SELECT statement.
    /// </summary>
    /// <returns>The name of the column to use.</returns>
    public string GetAlias()
    {
        return Alias;
    }

    public void Render(SqlTextWriter writer, SqlBuildArguments args)
    {
        writer.WriteLiteralValue(Value);
        if (Alias != null)
        {
            writer.WriteSpace();
            writer.Write(SqlConstants.AS);
            writer.WriteSpace();
            writer.WriteColumnName(Alias);
        }
    }

    public void RenderValueExpression(SqlTextWriter writer, SqlBuildArguments args)
    {
        writer.WriteLiteralValue(Value);
    }
}