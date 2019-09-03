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
    public class SqlCompareFilter<TValue> : SqlCompareFilter, ISqlFilter
    {

        /// <summary>
        /// Creates a new instance with the given table, column, operator, and value
        /// </summary>
        /// <param name="table"></param>
        /// <param name="columnName"></param>
        /// <param name="operator"></param>
        /// <param name="value"></param>
        public SqlCompareFilter(ISqlTable table, string columnName, SqlOperator @operator, TValue value) : base(table, columnName, @operator, DbConvert.TypeToDbType(typeof(TValue)), value)
        {
        }

        /// <summary>
        /// Gets the value of <see cref="SqlCompareFilter.Value"/> cast to the correct data type.
        /// </summary>
        /// <returns></returns>
        public TValue GetValue()
        {
            return (TValue)this.Value;
        }

        /// <summary>
        /// Sets the underlying value for comparison;
        /// </summary>
        /// <param name="value"></param>
        public void SetValue(TValue value)
        {
            this.Value = value;
        }
                
        #region ISqlClause Members

        void ISqlStatementElement.Render(SqlTextWriter writer, SqlBuildArguments args)
        {
            writer.Render(this);
        }

        #endregion
    }
}
