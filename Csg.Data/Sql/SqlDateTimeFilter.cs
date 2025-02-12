using System;
using System.Data;

namespace Csg.Data.Sql;

/// <summary>
///     Creates a filter that matches a data column if it is between two instances in time.
/// </summary>
public class SqlDateTimeFilter : ISqlFilter
{
    public SqlDateTimeFilter(ISqlTable table, string columnName, DateTime beginDate, DateTime endDate)
    {
        Table = table;
        ColumnName = columnName;
        BeginDate = beginDate;
        EndDate = endDate;
    }

    public ISqlTable Table { get; set; }

    public string ColumnName { get; set; }

    public DateTime BeginDate { get; set; }

    public DateTime EndDate { get; set; }

    #region ISqlStatementElement Members

    void ISqlStatementElement.Render(SqlTextWriter writer, SqlBuildArguments args)
    {
        writer.WriteBeginGroup();
        WriteColumnName(writer, args);
        writer.WriteOperator(SqlOperator.GreaterThanOrEqual);
        writer.WriteParameter(args.CreateParameter(GetBeginDate(), DbType.DateTime));
        writer.WriteSpace();
        writer.Write(SqlConstants.AND);
        writer.WriteSpace();
        WriteColumnName(writer, args);
        writer.WriteOperator(SqlOperator.LessThanOrEqual);
        writer.WriteParameter(args.CreateParameter(GetEndDate(), DbType.DateTime));
        writer.WriteEndGroup();
    }

    #endregion

    protected virtual DateTime GetBeginDate()
    {
        return BeginDate;
    }

    protected virtual DateTime GetEndDate()
    {
        return EndDate;
    }

    protected virtual void WriteColumnName(SqlTextWriter writer, SqlBuildArguments args)
    {
        writer.WriteColumnName(ColumnName, args.TableName(Table));
    }
}