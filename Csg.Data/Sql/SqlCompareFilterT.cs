using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csg.Data.Sql
{
    /// <summary>
    /// Renders a simple comparison filter that compares a column to a value.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class SqlCompareFilter<TValue> : ISqlFilter
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public SqlCompareFilter()
        {
            this.EncodeValueAsLiteral = false;
        }

        /// <summary>
        /// Creates a new instance with the given table, column, operator, and value
        /// </summary>
        /// <param name="table"></param>
        /// <param name="columnName"></param>
        /// <param name="operator"></param>
        /// <param name="value"></param>
        public SqlCompareFilter(ISqlTable table, string columnName, SqlOperator @operator, TValue value) : this()
        {
            this.Table = table;
            this.ColumnName = columnName;
            this.Operator = @operator;
            this.DataType = util.ConvertTypeCodeToDbType(Type.GetTypeCode(typeof(TValue)));
            this.Value = value;            
        }

        /// <summary>
        /// Gets or sets the table associated with the <see cref="ColumnName"/> property.
        /// </summary>
        public ISqlTable Table { get; set; }

        /// <summary>
        /// Gets or sets the column name from <see cref="Table"/> that will be used in the comparison.
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// Gets or sets the comparison operator.
        /// </summary>
        public SqlOperator Operator { get; set; }

        /// <summary>
        /// Gets or sets the value to compare.
        /// </summary>
        public TValue Value { get; set; }

        /// <summary>
        /// Gets or sets the value data type.
        /// </summary>
        public System.Data.DbType DataType { get; set; }

        /// <summary>
        /// Gets or sets the maximum size, in bytes, of the value.
        /// </summary>
        public int? Size { get; set; }
        
        /// <summary>
        /// Gets or sets whether to encode the <see cref="Value"/> as literal in the rendered SQL statement, or to use parameters.
        /// </summary>
        public bool EncodeValueAsLiteral { get; set; }

        #region ISqlClause Members

        void ISqlStatementElement.Render(SqlTextWriter writer, SqlBuildArguments args)
        {
            writer.WriteBeginGroup();
            writer.WriteColumnName(this.ColumnName, args.TableName(this.Table));
            writer.WriteOperator(this.Operator);

            if (this.EncodeValueAsLiteral) 
            {
                writer.WriteLiteralValue(this.Value);
            }
            else
            {
                writer.WriteParameter(args.CreateParameter(this.Value, this.DataType));
            }           

            writer.WriteEndGroup();
        }

        #endregion
    }
}
