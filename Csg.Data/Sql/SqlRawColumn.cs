using System.Data;
using System.Data.Common;
using System.Linq;

namespace Csg.Data.Sql;

/// <summary>
///     Represents a column in a SELECT statement defined as a raw unquoted value.
/// </summary>
public class SqlRawColumn : ISqlColumn
{
    /// <summary>
    ///     Initializes a new instance with the given value.
    /// </summary>
    /// <param name="value">The raw value expression to be rendered</param>
    /// <param name="args">Argument values referenced in the value</param>
    public SqlRawColumn(string value, params object[] args)
    {
        Value = value;
        Arguments = args;
    }

    /// <summary>
    ///     Initializes a new instance with the given value and alias.
    /// </summary>
    /// <param name="value">The raw value expression to be rendered</param>
    /// <param name="alias">The column alias to asssign</param>
    /// <param name="args">Argument values referenced in the value</param>
    public SqlRawColumn(string value, string alias, params object[] args)
    {
        Value = value;
        Alias = alias;
    }

    /// <summary>
    ///     Gets or sets the raw value.
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    ///     Gets or sets the column alias to use when rendering the SELECT statement.
    /// </summary>
    public string Alias { get; set; }

    /// <summary>
    ///     A collection of arguments that will be used to replace string format placeholders {0}, {1}, etc. Argument values
    ///     can be tables, parameters, or literal values.
    /// </summary>
    public object[] Arguments { get; set; }

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


    /// <summary>
    ///     Renders the raw value to the writer with its alias, if it exists
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="args"></param>
    public void Render(SqlTextWriter writer, SqlBuildArguments args)
    {
        RenderValueExpression(writer, args);

        if (Alias != null)
        {
            writer.WriteSpace();
            writer.Write(SqlConstants.AS);
            writer.WriteSpace();
            writer.WriteColumnName(Alias);
        }
    }

    /// <summary>
    ///     Renders only the raw value to the writer
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="args"></param>
    public void RenderValueExpression(SqlTextWriter writer, SqlBuildArguments args)
    {
        var resolvedArguments = new string[0];

        if (Arguments != null)
            resolvedArguments = Arguments.Select(arg =>
            {
                if (arg is ISqlTable table)
                    return writer.FormatQualifiedIdentifierName(args.TableName(table));
                if (arg is DbParameter dbParam)
                    return string.Concat("@", dbParam.ParameterName);
                if (arg is DbParameterValue paramValue)
                    return string.Concat("@", paramValue.ParameterName);
                if (arg is string)
                    return string.Concat("@", args.CreateParameter(arg.ToString(), DbType.String));
                return string.Concat("@", args.CreateParameter(arg, DbType.String));
            }).ToArray();

        writer.Write(Value, resolvedArguments);
    }
}