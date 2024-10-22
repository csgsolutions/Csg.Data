using System.Data;

namespace Csg.Data.Sql;

/// <summary>
///     Renders a simple comparison filter that compares a column to a value.
/// </summary>
/// <typeparam name="TValue"></typeparam>
public class SqlCompareFilter<TValue> : ISqlFilter
{
    /// <summary>
    ///     Creates a new instance.
    /// </summary>
    public SqlCompareFilter()
    {
        EncodeValueAsLiteral = false;
        DataType = DbConvert.TypeToDbType(typeof(TValue));
    }

    /// <summary>
    ///     Creates a new instance with the given table, column, operator, and value
    /// </summary>
    /// <param name="table"></param>
    /// <param name="columnName"></param>
    /// <param name="operator"></param>
    /// <param name="value"></param>
    public SqlCompareFilter(ISqlTable table, string columnName, SqlOperator @operator, TValue value) : this()
    {
        Table = table;
        ColumnName = columnName;
        Operator = @operator;
        Value = value;
    }

    /// <summary>
    ///     Gets or sets the table associated with the <see cref="ColumnName" /> property.
    /// </summary>
    public ISqlTable Table { get; set; }

    /// <summary>
    ///     Gets or sets the column name from <see cref="Table" /> that will be used in the comparison.
    /// </summary>
    public string ColumnName { get; set; }

    /// <summary>
    ///     Gets or sets the comparison operator.
    /// </summary>
    public SqlOperator Operator { get; set; }

    /// <summary>
    ///     Gets or sets the value to compare.
    /// </summary>
    public TValue Value { get; set; }

    /// <summary>
    ///     Gets or sets the value data type.
    /// </summary>
    public DbType DataType { get; set; }

    /// <summary>
    ///     Gets or sets the maximum size, in bytes, of the value.
    /// </summary>
    public int? Size { get; set; }

    /// <summary>
    ///     Gets or sets whether to encode the <see cref="Value" /> as literal in the rendered SQL statement, or to use
    ///     parameters.
    /// </summary>
    public bool EncodeValueAsLiteral { get; set; }

    #region ISqlClause Members

    void ISqlStatementElement.Render(SqlTextWriter writer, SqlBuildArguments args)
    {
        writer.WriteBeginGroup();
        writer.WriteColumnName(ColumnName, args.TableName(Table));
        writer.WriteOperator(Operator);

        if (EncodeValueAsLiteral)
            writer.WriteLiteralValue(Value);
        else
            writer.WriteParameter(args.CreateParameter(Value, DataType));

        writer.WriteEndGroup();
    }

    #endregion
}