using System.Data;

namespace Csg.Data.Sql;

/// <summary>
///     Provides T-SQL fuzzy string filtering using the LIKE operator.
/// </summary>
public class SqlStringMatchFilter : SqlSingleColumnFilterBase
{
    /// <summary>
    ///     Initializes a new instance.
    /// </summary>
    /// <param name="table">The table source for the column.</param>
    /// <param name="columnName">The database column to compare.</param>
    /// <param name="oper">The wildcard decoration type to use.</param>
    /// <param name="value">The value compare the column with.</param>
    public SqlStringMatchFilter(ISqlTable table, string columnName, SqlWildcardDecoration oper, string value) : base(
        table, columnName, DbType.String)
    {
        Operator = oper;
        Value = value;
    }

    /// <summary>
    ///     Gets or sets a value that indicates how the value should be decorated with wildcards.
    /// </summary>
    public SqlWildcardDecoration Operator { get; set; }

    /// <summary>
    ///     Gets or sets the value to compare the column with.
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    ///     Decorates a string value with wildcard characters based on the <see cref="SqlWildcardDecoration" /> provided.
    /// </summary>
    /// <param name="value">A string value.</param>
    /// <param name="decoration">Specifies the type of decoration to use.</param>
    /// <returns></returns>
    public static string DecorateValue(string value, SqlWildcardDecoration decoration)
    {
        if (value.Contains("*") || value.Contains("%")) return value.Replace("*", "%");

        return decoration switch
        {
            SqlWildcardDecoration.BeginsWith => string.Concat(value, "%"),
            SqlWildcardDecoration.Contains => string.Concat("%", value, "%"),
            SqlWildcardDecoration.EndsWith => string.Concat("%", value),
            _ => value
        };
    }

    protected override void RenderInternal(SqlTextWriter writer, SqlBuildArguments args)
    {
        var s = Value ?? string.Empty;

        writer.WriteBeginGroup();
        writer.WriteColumnName(ColumnName, args.TableName(Table));
        writer.WriteSpace();
        writer.Write(SqlConstants.LIKE);
        writer.WriteSpace();
        writer.WriteParameter(args.CreateParameter(DecorateValue(Value, Operator), DataType));
        writer.WriteEndGroup();
    }
}