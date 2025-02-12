namespace Csg.Data.Sql;

public class SqlColumnCompareFilter : ISqlFilter
{
    public SqlColumnCompareFilter(ISqlTable leftTable, string columnName, SqlOperator @operator, ISqlTable rightTable)
        : this(leftTable, columnName, @operator, rightTable, columnName)
    {
    }

    public SqlColumnCompareFilter(ISqlTable leftTable, string leftColumnName, SqlOperator @operator,
        ISqlTable rightTable, string rightColumnName)
    {
        LeftTable = leftTable;
        LeftColumnName = leftColumnName;
        RightColumnName = rightColumnName;
        Operator = @operator;
        RightTable = rightTable;
    }

    public ISqlTable LeftTable { get; set; }
    public string LeftColumnName { get; set; }
    public SqlOperator Operator { get; set; }
    public ISqlTable RightTable { get; set; }
    public string RightColumnName { get; set; }

    #region ISqlClause Members

    void ISqlStatementElement.Render(SqlTextWriter writer, SqlBuildArguments args)
    {
        writer.WriteBeginGroup();
        writer.WriteColumnName(LeftColumnName, args.TableName(LeftTable));
        writer.WriteOperator(Operator);
        writer.WriteColumnName(RightColumnName, args.TableName(RightTable));
        writer.WriteEndGroup();
    }

    #endregion
}