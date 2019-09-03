using System;

namespace Csg.Data.Sql
{
    /// <summary>
    /// Provides basic T-SQL comparison such as Column = value, Column &gt; value, etc.
    /// </summary>
    public class SqlCompareFilter : ISqlFilter
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public SqlCompareFilter()
        {
            this.EncodeValueAsLiteral = false;
        }

        /// <summary>
        /// Creates a filter with the given data type
        /// </summary>
        /// <param name="table"></param>
        /// <param name="columnName"></param>
        /// <param name="operator"></param>
        /// <param name="dataType"></param>
        /// <param name="value"></param>
        public SqlCompareFilter(ISqlTable table, string columnName, SqlOperator @operator, System.Data.DbType dataType, object value) : this() //: base(table, columnName, @operator, value)
        {
            this.Table = table;
            this.ColumnName = columnName;
            this.Operator = @operator;
            this.DataType = dataType;
            this.Value = value;
        }

        /// <summary>
        /// Gets or sets the table associated with the <see cref="ColumnName"/> property.
        /// </summary>
        public virtual ISqlTable Table { get; set; }

        /// <summary>
        /// Gets or sets the column name from <see cref="Table"/> that will be used in the comparison.
        /// </summary>
        public virtual string ColumnName { get; set; }

        /// <summary>
        /// Gets or sets the comparison operator.
        /// </summary>
        public virtual SqlOperator Operator { get; set; }

        /// <summary>
        /// Gets or sets the value to compare.
        /// </summary>
        public virtual object Value { get; set; }

        /// <summary>
        /// Gets or sets the value data type.
        /// </summary>
        public virtual System.Data.DbType DataType { get; set; }

        /// <summary>
        /// Gets or sets the maximum size, in bytes, of the value.
        /// </summary>
        public virtual int? Size { get; set; }

        /// <summary>
        /// Gets or sets whether to encode the <see cref="Value"/> as literal in the rendered SQL statement, or to use parameters.
        /// </summary>
        public bool EncodeValueAsLiteral { get; set; }

        void ISqlStatementElement.Render(SqlTextWriter writer, SqlBuildArguments args)
        {
            writer.Render(this);
        }
    }
}
