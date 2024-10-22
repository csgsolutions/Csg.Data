using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Csg.Data.Sql;

public class SqlTable : SqlTableBase
{
    public SqlTable(string tableName)
    {
        TableName = tableName;
    }

    public string TableName { get; set; }

    protected override void RenderInternal(SqlTextWriter writer, SqlBuildArguments args)
    {
        writer.WriteTableName(TableName, args.TableName(this));
    }
}

public abstract class SqlTableBase : ISqlTable
{
    private readonly List<ISqlJoin> _joins;

    public SqlTableBase()
    {
        _joins = new List<ISqlJoin>();
    }

    #region ISqlClause Members

    void ISqlStatementElement.Render(SqlTextWriter writer, SqlBuildArguments args)
    {
        RenderInternal(writer, args);
    }

    #endregion

    #region ISqlTable2 Members

    void ISqlTable.Compile(SqlBuildArguments args)
    {
        args.AssignAlias(this);

        if (_joins == null)
            return;

        foreach (var join in _joins) join.JoinedTable.Compile(args);
    }

    #endregion

    public static ISqlTable Create(string sql)
    {
        //check if this is a derived table (an in-line SELECT statement)
        if (Regex.IsMatch(sql, @"\bSELECT\b", RegexOptions.Compiled | RegexOptions.IgnoreCase))
            return new SqlDerivedTable(sql);
        return new SqlTable(sql);
    }

    protected abstract void RenderInternal(SqlTextWriter writer, SqlBuildArguments args);
}

public class SqlDerivedTable : SqlTableBase
{
    public SqlDerivedTable(string commandText)
    {
        CommandText = commandText;
    }

    public string CommandText { get; set; }

    protected override void RenderInternal(SqlTextWriter writer, SqlBuildArguments args)
    {
        writer.WriteBeginGroup();
        writer.Write(CommandText.TrimEnd('\r', '\n', ';', ' ', '\t'));
        writer.WriteEndGroup();
        writer.WriteSpace();
        writer.Write(SqlConstants.AS);
        writer.WriteSpace();
        writer.Write(SqlDataColumn.Format(args.TableName(this)));
    }
}