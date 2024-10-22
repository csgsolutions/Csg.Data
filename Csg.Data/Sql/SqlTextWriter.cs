using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Csg.Data.Sql;

public class SqlTextWriter : TextWriter
{
    private const string COLUMN_SEPERATOR = ",";
    private const string JOIN_SEPERATOR = " ";
    private const string QUOTE = "\"";
    private const string QUOTEQUOTE = "\"\"";
    private readonly TextWriter InnerWriter;

    public SqlTextWriter()
    {
        InnerWriter = new StringWriter();
    }

    public SqlTextWriter(TextWriter writer)
    {
        InnerWriter = writer;
    }

    public SqlTextWriter(StringBuilder sb)
    {
        InnerWriter = new StringWriter(sb);
    }

    /// <summary>
    ///     Gets or sets a value that indicates if the writer should output "pretty" SQL that includes un-necessary line breaks
    ///     and such.
    /// </summary>
    public bool Format { get; set; }

    /// <summary>
    ///     Gets or sets a value that indiciates if the writer should write quoted Identifiers.
    /// </summary>
    public bool UseQuotedIdentifiers { get; set; }

    public override Encoding Encoding => InnerWriter.Encoding;

    public override string ToString()
    {
        return InnerWriter.ToString();
    }

    #region SQL SERVER Specific Statics

    public static string FormatSqlServerIdentifier(string value, bool useQuotedIdentifiers = false)
    {
        if (useQuotedIdentifiers && value.StartsWith(QUOTE) && value.EndsWith(QUOTE) && value.Length > 2)
            value = value.Substring(1, value.Length - 2);
        else if (value.StartsWith("[") && value.EndsWith("]") && value.Length > 2)
            value = value.Substring(1, value.Length - 2);

        if (useQuotedIdentifiers || value.Contains("[") || value.Contains("]"))
        {
            value = value.Replace(QUOTE, QUOTEQUOTE);
            value = string.Concat(QUOTE, value, QUOTE);
        }
        else
        {
            value = string.Concat("[", value, "]");
        }

        return value;
    }

    public static string ConvertSqlOperatorToString(SqlOperator oper)
    {
        switch (oper)
        {
            case SqlOperator.Equal: return "=";
            case SqlOperator.GreaterThan: return ">";
            case SqlOperator.GreaterThanOrEqual: return ">=";
            case SqlOperator.LessThan: return "<";
            case SqlOperator.LessThanOrEqual: return "<=";
            case SqlOperator.NotEqual: return "<>";
            default: throw new InvalidOperationException("Invalid operator");
        }
    }

    public static string ConvertSqlAggregateToString(SqlAggregate aggregateType)
    {
        switch (aggregateType)
        {
            case SqlAggregate.Minimum: return "MIN";
            case SqlAggregate.Maximum: return "MAX";
            case SqlAggregate.Sum: return "SUM";
            case SqlAggregate.Count: return "COUNT";
            case SqlAggregate.CountDistinct: return "COUNT";
            case SqlAggregate.Average: return "AVG";
            case SqlAggregate.StDev: return "STDEV";
            default: throw new InvalidOperationException("Invalid aggregate");
        }
    }

    public static string ConvertSqlLogicToString(SqlLogic logic)
    {
        return logic.ToString().ToUpper();
    }

    public static string ConvertDbSortDirectionToString(DbSortDirection direction)
    {
        switch (direction)
        {
            case DbSortDirection.Ascending: return "ASC";
            case DbSortDirection.Descending: return "DESC";
            default: return string.Empty;
        }
    }

    public static string FormatTableName(string name)
    {
        var parts = name.Split(new[] { '.' });
        for (var x = 0; x < parts.Length; x++) parts[x] = FormatSqlServerIdentifier(parts[x]);

        return string.Join(SqlConstants.DOT, parts);
    }

    #endregion

    #region Language Specifics

    protected virtual string FormatIdentifier(string value)
    {
        return FormatSqlServerIdentifier(value, UseQuotedIdentifiers);
    }

    public virtual string FormatColumnName(string name)
    {
        return FormatIdentifier(name);
    }

    public virtual string FormatQualifiedIdentifierName(string name)
    {
        return FormatTableName(name);
    }

    public virtual string ConvertOperatorToString(SqlOperator @operator)
    {
        return ConvertSqlOperatorToString(@operator);
    }

    public virtual string ConvertAggregateToString(SqlAggregate aggregate)
    {
        return ConvertSqlAggregateToString(aggregate);
    }

    public virtual string ConvertLogicToString(SqlLogic logic)
    {
        return ConvertSqlLogicToString(logic);
    }

    public virtual string ConvertSortDirectionToString(DbSortDirection direction)
    {
        return ConvertDbSortDirectionToString(direction);
    }

    public virtual string ColumnSeperator => COLUMN_SEPERATOR;

    #endregion

    #region Standard Write Methods

    public override void Write(char value)
    {
        InnerWriter.Write(value);
    }

    public void WriteNewLine()
    {
        if (Format) Write("\r\n");
    }

    public void WriteColumnName(string columnName)
    {
        Write(FormatColumnName(columnName));
    }

    public void WriteColumnName(string columnName, string tableName)
    {
        WriteTableName(tableName);
        Write(SqlConstants.DOT);
        WriteColumnName(columnName);
    }

    public void WriteColumnName(string columnName, string tableName, string alias)
    {
        WriteTableName(tableName);

        Write(SqlConstants.DOT);
        WriteColumnName(columnName);

        if (!string.IsNullOrEmpty(alias) && !alias.Equals(columnName))
        {
            WriteSpace();
            Write(SqlConstants.AS);
            WriteSpace();
            WriteColumnName(alias);
        }
    }

    public void WriteTableName(string tableName)
    {
        WriteTableName(tableName, null);
    }

    public void WriteTableName(string tableName, string alias)
    {
        Write(FormatQualifiedIdentifierName(tableName));
        if (!string.IsNullOrEmpty(alias))
        {
            WriteSpace();
            Write(SqlConstants.AS);
            WriteSpace();
            Write(FormatIdentifier(alias));
        }
    }

    public void WriteParameter(string parameterName)
    {
        Write("@");
        Write(parameterName);
    }

    public void WriteAggregateColumn(string columnName, string tableName, SqlAggregate aggregateType, string outputName)
    {
        Write(ConvertSqlAggregateToString(aggregateType));
        WriteBeginGroup();
        if (aggregateType == SqlAggregate.CountDistinct)
        {
            Write(SqlConstants.DISTINCT);
            WriteSpace();
        }

        WriteColumnName(columnName, tableName);
        WriteEndGroup();
        WriteSpace();
        Write(SqlConstants.AS);
        WriteSpace();
        WriteColumnName(outputName);
    }

    public void WriteAggregate(string columnName, string tableName, SqlAggregate aggregateType)
    {
        Write(ConvertSqlAggregateToString(aggregateType));
        WriteBeginGroup();
        WriteColumnName(columnName, tableName);
        WriteEndGroup();
    }

    public void WriteOperator(SqlOperator oper)
    {
        Write(ConvertSqlOperatorToString(oper));
    }

    public void WriteSelect()
    {
        Write(SqlConstants.SELECT);
    }

    public void RenderSelect(IEnumerable<ISqlColumn> columns, SqlBuildArguments args)
    {
        RenderSelect(columns, args);
    }

    public void RenderSelect(IEnumerable<ISqlColumn> columns, SqlBuildArguments args, bool distinct)
    {
        if (columns == null)
            throw new ArgumentNullException("columns");
        if (args == null)
            throw new ArgumentNullException("args");

        var items = columns.ToArray();
        WriteSelect();
        WriteSpace();

        if (distinct)
        {
            Write(SqlConstants.DISTINCT);
            WriteSpace();
        }

        if (items.Length > 0)
        {
            if (Format)
                RenderAll(items, args, string.Concat(COLUMN_SEPERATOR, "\r\n"));
            else
                RenderAll(items, args, COLUMN_SEPERATOR);
        }
        else
        {
            Write("*");
        }

        WriteNewLine();
    }

    public void WriteFrom()
    {
        WriteSpace();
        Write(SqlConstants.FROM);
        WriteSpace();
    }

    public void RenderFrom(ISqlTable table, SqlBuildArguments args)
    {
        if (table == null)
            throw new ArgumentNullException("table");
        if (args == null)
            throw new ArgumentNullException("args");

        WriteFrom();
        table.Render(this, args);
        WriteNewLine();
    }

    public void WriteWhere()
    {
        WriteSpace();
        Write(SqlConstants.WHERE);
        WriteSpace();
    }

    public void RenderWhere(IEnumerable<ISqlFilter> filters, SqlLogic logic, SqlBuildArguments args)
    {
        if (filters == null)
            throw new ArgumentNullException("filters");
        if (args == null)
            throw new ArgumentNullException("args");

        var items = filters.ToArray();
        if (items.Length > 0)
        {
            WriteWhere();
            RenderAll(items, args,
                string.Concat(SqlConstants.SPACE, ConvertSqlLogicToString(logic), SqlConstants.SPACE));
        }
    }

    public void WriteGroupBy()
    {
        WriteSpace();
        Write(SqlConstants.GROUP_BY);
        WriteSpace();
    }

    public void RenderGroupBy(IEnumerable<ISqlColumn> columns, SqlBuildArguments args)
    {
        if (columns == null)
            throw new ArgumentNullException("columns");
        if (args == null)
            throw new ArgumentNullException("args");

        var items = columns.ToArray();
        if (items.Length > 0)
        {
            WriteGroupBy();
            RenderAll(items, args, COLUMN_SEPERATOR, (a, b, c) => { a.RenderValueExpression(b, c); });
        }

        WriteNewLine();
    }

    public void WriteOrderBy()
    {
        WriteSpace();
        Write(SqlConstants.ORDER_BY);
        WriteSpace();
    }

    public void RenderOrderBy(IEnumerable<SqlOrderColumn> columns, SqlBuildArguments args)
    {
        if (columns == null)
            throw new ArgumentNullException("columns");
        if (args == null)
            throw new ArgumentNullException("args");

        var items = columns.ToArray();

        if (items.Length > 0)
        {
            WriteOrderBy();
            RenderAll(items, args, COLUMN_SEPERATOR);
        }

        WriteNewLine();
    }

    public void RenderOffsetLimit(SqlPagingOptions options, SqlBuildArguments args)
    {
        //https://docs.microsoft.com/en-us/sql/t-sql/queries/select-order-by-clause-transact-sql?view=sql-server-2017#using-offset-and-fetch-to-limit-the-rows-returned
        //e.g. OFFSET 10 ROWS FETCH NEXT 10 ROWS ONLY

        if (options.Offset > 0 || options.Limit > 0) Write($" OFFSET {options.Offset} ROWS");

        if (options.Limit > 0) Write($" FETCH NEXT {options.Limit} ROWS ONLY");
    }

    public void WriteSortColumn(string columnName, DbSortDirection direction)
    {
        WriteColumnName(columnName);
        if (direction != DbSortDirection.None)
        {
            WriteSpace();
            Write(ConvertDbSortDirectionToString(direction));
        }
    }

    public void WriteDerivedTable(string commandText, string alias)
    {
        WriteBeginGroup();
        Write(commandText);
        WriteEndGroup();
        WriteSpace();
        Write(SqlConstants.AS);
        WriteSpace();
        WriteTableName(FormatTableName(alias));
    }

    public void WriteSpace()
    {
        Write(SqlConstants.SPACE);
    }

    public void WriteComma()
    {
        Write(SqlConstants.COMMA);
    }

    public void WriteColumnSeperator()
    {
        WriteComma();
    }

    public virtual void WriteBeginGroup()
    {
        Write(SqlConstants.BEGINGROUP);
    }

    public virtual void WriteEndGroup()
    {
        Write(SqlConstants.ENDGROUP);
    }

    public virtual void WriteLogic(SqlLogic logic)
    {
        Write(logic.ToString().ToUpper());
    }

    public virtual void WriteEndStatement()
    {
        Write(SqlConstants.SEMICOLON);
    }

    public virtual void RenderJoin(ISqlTable leftTable, SqlJoinType joinType, ISqlTable rightTable,
        IEnumerable<ISqlFilter> conditions, SqlBuildArguments args)
    {
        WriteSpace();
        Write(joinType.ToString().ToUpper());
        WriteSpace();
        Write("JOIN");
        WriteSpace();

        //TODO: This is causing joined tables to render their conditions here, which is not right.
        rightTable.Render(this, args);

        if (joinType != SqlJoinType.Cross)
        {
            WriteSpace();
            Write(SqlConstants.ON);
            WriteSpace();
            RenderAll(conditions, args,
                string.Concat(SqlConstants.SPACE, ConvertSqlLogicToString(SqlLogic.And), SqlConstants.SPACE));
        }

        WriteNewLine();
    }

    public virtual void RenderJoins(IEnumerable<ISqlJoin> joins, SqlBuildArguments args)
    {
        RenderAll(joins, args, string.Empty);
    }

    public virtual void WriteCast(string columnName, string sqlDataTypeName, string tableName)
    {
        Write(SqlConstants.CAST);
        WriteBeginGroup();
        WriteColumnName(columnName, tableName);
        WriteSpace();
        Write(SqlConstants.AS);
        WriteSpace();
        Write(sqlDataTypeName);
        WriteEndGroup();
    }

    public virtual void WriteCast(string literal, string sqlDataTypeName)
    {
        Write(SqlConstants.CAST);
        WriteBeginGroup();
        Write(literal);
        WriteSpace();
        Write(SqlConstants.AS);
        WriteSpace();
        Write(sqlDataTypeName);
        WriteEndGroup();
    }

    public virtual void WriteQuotedString(string value)
    {
        value = value.Replace(SqlConstants.SINGLE_QUOTE, SqlConstants.DOUBLE_SINGLE_QUOTE);
        Write(SqlConstants.SINGLE_QUOTE);
        Write(value);
        Write(SqlConstants.SINGLE_QUOTE);
    }

    public virtual void WriteLiteralValue(object value)
    {
        //TODO: What the crap does this do with dates and other things that would need to be quoted besides strings?
        if (value is string)
            WriteQuotedString((string)value);
        else
            Write(value);
    }

    public virtual void RenderAll<T>(IEnumerable<T> items, SqlBuildArguments args, string seperator)
        where T : ISqlStatementElement
    {
        RenderAll(items, args, seperator, (a, b, c) => { a.Render(b, c); });
    }

    public virtual void RenderAll<T>(IEnumerable<T> items, SqlBuildArguments args, string seperator,
        Action<T, SqlTextWriter, SqlBuildArguments> renderAction) where T : ISqlStatementElement
    {
        var first = true;
        foreach (var item in items)
        {
            if (first)
                first = false;
            else
                Write(seperator);
            renderAction(item, this, args);
        }
    }

    #endregion

    #region T-SQL Specific Write Methods

    public virtual void WriteRankOver(string columnName, string tableName, SqlAggregate aggregateType,
        bool rankDescending)
    {
        Write(SqlConstants.RANK_OVER);
        WriteBeginGroup();
        Write(SqlConstants.ORDER_BY);
        WriteSpace();
        Write(ConvertSqlAggregateToString(aggregateType));
        WriteBeginGroup();
        WriteColumnName(columnName, tableName);
        WriteEndGroup();
        if (rankDescending)
        {
            WriteSpace();
            Write(SqlConstants.DESC);
        }

        WriteEndGroup();
    }

    public virtual void WriteRankOver(string columnName, string tableName, SqlAggregate aggregateType, string alias,
        bool rankDescending)
    {
        WriteRankOver(columnName, tableName, aggregateType, rankDescending);
        WriteSpace();
        Write(SqlConstants.AS);
        WriteSpace();
        WriteColumnName(alias);
    }

    #endregion
}