using System;
using System.Collections.Generic;
using System.Linq;

namespace Csg.Data.Sql;

/// <summary>
///     Provides methods for parsing, testing, and executing an SQL SELECT statement
/// </summary>
public class SqlSelectBuilder : ISqlTable
{
    private const string TSQL_FMT_ONLY_ON = "SET FMTONLY ON; ";
    private const string TSQL_FMT_ONLY_OFF = "SET FMTONLY OFF; ";
    private const string TSQL_ORDER_BY = "ORDER BY";
    private IList<ISqlColumn> _columns;
    private IList<ISqlFilter> _filters;
    private IList<ISqlJoin> _joins;
    private IList<SqlOrderColumn> _orderBy;

    internal SqlSelectBuilder(ISqlTable table, IEnumerable<ISqlJoin> joins, IList<ISqlColumn> columns,
        IEnumerable<ISqlFilter> filters, IList<SqlOrderColumn> orderBy) : this()
    {
        Table = table;
        _joins = joins.ToList();
        _columns = columns;
        _filters = filters.ToList();
        _orderBy = orderBy;
    }

    public SqlSelectBuilder()
    {
        GenerateFormattedSql = false;
    }

    public SqlSelectBuilder(string commandText) : this()
    {
        ParseInternal(commandText);
    }

    public SqlSelectBuilder(ISqlTable table) : this()
    {
        Table = table;
    }

    /// <summary>
    ///     Gets a value that indicates if the output SQL text should have line breaks and other formatting.
    /// </summary>
    public bool GenerateFormattedSql { get; set; }

    /// <summary>
    ///     Gets or sets a value that indicates if only distinct values should be returned from the query.
    /// </summary>
    public bool SelectDistinct { get; set; }

    /// <summary>
    ///     Gets or sets the primary table used in the query. This will be used as the first table rendered directly after the
    ///     FROM keyword.
    /// </summary>
    public ISqlTable Table { get; set; }

    /// <summary>
    ///     Gets a collection of table joins used to join other tables into the resulting query.
    /// </summary>
    public IList<ISqlJoin> Joins
    {
        get
        {
            if (_joins == null)
                _joins = new List<ISqlJoin>();
            return _joins;
        }
    }

    /// <summary>
    ///     Gets a collection of columns to be used in the query.
    /// </summary>
    public IList<ISqlColumn> Columns
    {
        get
        {
            if (_columns == null)
                _columns = new List<ISqlColumn>();
            return _columns;
        }
    }

    /// <summary>
    ///     Gets a collection of <see cref="SqlOrderColumn" /> which control how the ORDER BY keyword is rendered, if at all.
    /// </summary>
    public IList<SqlOrderColumn> OrderBy
    {
        get
        {
            if (_orderBy == null)
                _orderBy = new List<SqlOrderColumn>();
            return _orderBy;
        }
    }

    /// <summary>
    ///     Gets a collection of filter objects that will follow the WHERE keyword.
    /// </summary>
    public IList<ISqlFilter> Filters
    {
        get
        {
            if (_filters == null)
                _filters = new List<ISqlFilter>();
            return _filters;
        }
    }

    /// <summary>
    ///     Gets the paging options that will be applied to the query.
    /// </summary>
    public SqlPagingOptions? PagingOptions { get; set; }

    /// <summary>
    ///     Gets or sets a SQL statement that will be prefixed to the rendered query with a statement separater afterwards.
    ///     This can be used to set query options.
    /// </summary>
    public string Prefix { get; set; }

    /// <summary>
    ///     Gets or sets a SQL statment that will be appended to the end of the rendered query after a statement separaeter
    ///     (semicolon).
    /// </summary>
    public string Suffix { get; set; }

    void ISqlTable.Compile(SqlBuildArguments args)
    {
        args.AssignAlias(this);
        CompileInternal(args);
    }

    void ISqlStatementElement.Render(SqlTextWriter writer, SqlBuildArguments args)
    {
        CompileInternal(args);
        //writer.WriteBeginGroup();
        RenderInternal(writer, args);
        //writer.WriteEndGroup();
        //writer.WriteSpace();
        //writer.Write(SqlConstants.AS);
        //writer.WriteSpace();
        //writer.Write(SqlDataColumn.Format(args.TableName(this)));
    }

    private void ParseInternal(string commandText)
    {
        //TODO: check sortExpression for SQL injection            
        string orderBy;
        var i = commandText.IndexOf(TSQL_ORDER_BY, StringComparison.OrdinalIgnoreCase);

        if (string.IsNullOrEmpty(commandText))
            throw util.InvalidOperationException(ErrorMessage.GenericValueCannotBeEmptyOrNull, "commandText");

        commandText = commandText.Trim().TrimEnd('\r', '\n', ';', ' ', '\t');

        if (i >= 0)
        {
            Table = SqlTableBase.Create(commandText.Substring(0, i));
            orderBy = commandText.Substring(i + TSQL_ORDER_BY.Length + 1);
            OrderBy.Add(orderBy);
        }
        else
        {
            Table = SqlTableBase.Create(commandText);
            orderBy = null;
        }
    }

    /// <summary>
    ///     Renders the query.
    /// </summary>
    /// <returns></returns>
    public SqlStatement Render()
    {
        return Render(false);
    }

    /// <summary>
    ///     Renders the query.
    /// </summary>
    /// <param name="supressEndStatement">True if you want to supress statement terminating characters (semicolon)</param>
    /// <returns></returns>
    public SqlStatement Render(bool supressEndStatement)
    {
        var writer = new SqlTextWriter { Format = GenerateFormattedSql };
        var args = new SqlBuildArguments();

        CompileInternal(args);

        if (Prefix != null)
        {
            writer.Write(Prefix);
            if (!supressEndStatement) writer.WriteEndStatement();
        }

        RenderInternal(writer, args);

        if (!supressEndStatement) writer.WriteEndStatement();

        if (Suffix != null)
        {
            writer.Write(Suffix);
            if (!supressEndStatement) writer.WriteEndStatement();
        }

        return new SqlStatement(writer.ToString(), args.Parameters);
    }

    /// <summary>
    ///     Renders the query.
    /// </summary>
    /// <returns></returns>
    public void Render(SqlTextWriter writer, SqlBuildArguments args)
    {
        CompileInternal(args);
        RenderInternal(writer, args);
    }

    /// <summary>
    ///     Renders the query with T-SQL 'format only' decorators. This allows the query to be executed to validate the query
    ///     and inspect the resulting schema, but will not return any data.
    /// </summary>
    /// <returns></returns>
    public SqlStatement RenderFormatOnly()
    {
        var s = Render();

        s.CommandText = string.Concat(TSQL_FMT_ONLY_ON, ";", s.CommandText, TSQL_FMT_ONLY_OFF, ";");

        return s;
    }

    /// <summary>
    ///     Compiles the tables used in the query into the given build arguments object.
    /// </summary>
    /// <param name="args"></param>
    protected void CompileInternal(SqlBuildArguments args)
    {
        Table.Compile(args);
        if (Joins.Count > 0)
            foreach (var join in Joins)
                join.JoinedTable.Compile(args);
    }

    /// <summary>
    ///     Renders the query to the given text writer.
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="args"></param>
    protected void RenderInternal(SqlTextWriter writer, SqlBuildArguments args)
    {
        writer.RenderSelect(Columns, args, SelectDistinct);
        writer.RenderFrom(Table, args);
        if (Joins.Count > 0) writer.RenderJoins(Joins, args);
        writer.RenderWhere(Filters, SqlLogic.And, args);

        if (Columns.Count(x => x.IsAggregate) > 0) writer.RenderGroupBy(Columns.Where(x => !x.IsAggregate), args);

        writer.RenderOrderBy(OrderBy, args);
        if (PagingOptions.HasValue) writer.RenderOffsetLimit(PagingOptions.Value, args);
    }
}