using System;
using System.Collections.Generic;

namespace Csg.Data.Sql;

/// <summary>
///     Renders a T-SQL expression based column for a SELECT statement.
/// </summary>
[Obsolete("Use SqlRawColumn instead")]
public class SqlExpressionSelectColumn : SqlColumnBase
{
    private IList<ISqlTable> _referenceTables;

    /// <summary>
    ///     Creates a new instance with the given table, expression, and column name alias.
    /// </summary>
    /// <param name="table"></param>
    /// <param name="expression"></param>
    /// <param name="alias"></param>
    public SqlExpressionSelectColumn(ISqlTable table, string expression, string alias) : base(table, alias)
    {
        Expression = expression;
    }

    /// <summary>
    ///     Gets or sets the SQL expression providing the value for the column.
    /// </summary>
    public string Expression { get; set; }

    /// <summary>
    ///     Gets a collection of tables that can be referenced in the sql expression.
    /// </summary>
    /// <remarks>
    ///     The first table in the ReferenceTables collection can be represented by the string {1}, the second table {2}, and
    ///     so forth.
    /// </remarks>
    public IList<ISqlTable> ReferenceTables
    {
        get
        {
            if (_referenceTables == null) _referenceTables = new List<ISqlTable>();
            return _referenceTables;
        }
    }

    /// <summary>
    ///     Renders the portion of the SQL statement that will be used as the value.
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="args"></param>
    protected override void RenderValueExpression(SqlTextWriter writer, SqlBuildArguments args)
    {
        var expr = Expression;

        expr = expr.Replace("{0}", args.TableName(Table));
        if (_referenceTables != null && _referenceTables.Count > 0)
            for (var i = 0; i < _referenceTables.Count; i++)
                expr = expr.Replace(string.Concat("{", i + 1, "}"), args.TableName(_referenceTables[i]));
        writer.WriteBeginGroup();
        writer.Write(expr);
        writer.WriteEndGroup();
    }

    /// <summary>
    ///     Renders the entire SQL statement.
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="args"></param>
    protected override void Render(SqlTextWriter writer, SqlBuildArguments args)
    {
        RenderValueExpression(writer, args);
        writer.WriteSpace();
        writer.Write(SqlConstants.AS);
        writer.WriteSpace();
        writer.WriteColumnName(Alias);
    }
}