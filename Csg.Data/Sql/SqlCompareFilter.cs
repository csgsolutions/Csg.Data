using System.Data;

namespace Csg.Data.Sql;

/// <summary>
///     Provides basic T-SQL comparison such as Column = value, Column &gt; value, etc.
/// </summary>
public class SqlCompareFilter : SqlCompareFilter<object>
{
    public SqlCompareFilter(ISqlTable table, string columnName, SqlOperator @operator, DbType dataType, object value) :
        base(table, columnName, @operator, value)
    {
        DataType = dataType;
    }
}