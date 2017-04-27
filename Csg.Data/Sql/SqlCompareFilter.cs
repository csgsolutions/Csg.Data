namespace Csg.Data.Sql
{
    /// <summary>
    /// Provides basic T-SQL comparison such as Column = value, Column &gt; value, etc.
    /// </summary>
    public class SqlCompareFilter : SqlCompareFilter<object>
    {
        public SqlCompareFilter(ISqlTable table, string columnName, SqlOperator @operator, System.Data.DbType dataType, object value) : base(table, columnName, @operator, value)
        {
            this.DataType = dataType;
        }                
    }
}
