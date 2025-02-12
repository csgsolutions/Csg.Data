using System;

namespace Csg.Data.Sql;

/// <summary>
///     Creates a filter that matches a data column if it is between (beginDate &gt;= date &lt;= endDate) two dates
///     ignoring time.
/// </summary>
public class SqlDateFilter : SqlDateTimeFilter
{
    public SqlDateFilter(ISqlTable table, string columnName, DateTime beginDate, DateTime endDate) : base(table,
        columnName, beginDate, endDate)
    {
    }

    protected override DateTime GetBeginDate()
    {
        return base.GetBeginDate().Date;
    }

    protected override DateTime GetEndDate()
    {
        return base.GetEndDate().Date;
    }

    protected override void WriteColumnName(SqlTextWriter writer, SqlBuildArguments args)
    {
        //TODO: This is causing left-side casting in a filter, which is awful.
        writer.WriteCast(ColumnName, "date", args.TableName(Table));
    }
}