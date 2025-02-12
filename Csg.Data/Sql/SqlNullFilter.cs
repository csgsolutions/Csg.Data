namespace Csg.Data.Sql;

/// <summary>
///     Provides a T-SQL comparison for a column having a NULL value or not.
/// </summary>
public class SqlNullFilter : SqlSingleColumnFilterBase
{
    public SqlNullFilter(ISqlTable table, string columnName, bool isNull)
        : base(table, columnName)
    {
        IsNull = isNull;
    }

    public bool IsNull { get; set; }

    protected override void RenderInternal(SqlTextWriter writer, SqlBuildArguments args)
    {
        //TODO: Make this impl agnostic
        writer.WriteBeginGroup();
        writer.WriteColumnName(ColumnName, args.TableName(Table));
        writer.Write(IsNull ? " IS NULL" : " IS NOT NULL");
        writer.WriteEndGroup();
    }
}