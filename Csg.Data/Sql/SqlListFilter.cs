using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csg.Data.Sql
{
    /// <summary>
    /// Creates a T-SQL WHERE filter comparing a table column to a list of values.
    /// </summary>
    /// <typeparam name="T">The .NET data type of the table column.</typeparam>
    public class SqlListFilter: SqlSingleColumnFilterBase
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="table">The table source for the column</param>
        /// <param name="columnName">The table column to filter on.</param>
        /// <param name="dataType">The data type of the column</param>
        /// <param name="values">The list of values to compare with.</param>
        public SqlListFilter(ISqlTable table, string columnName, System.Data.DbType dataType, System.Collections.IEnumerable values): base(table, columnName)
        {
            this.NotInList = false;
            this.DataType = dataType;
            this.Values = values;
        }

        /// <summary>
        /// Gets or sets a value that indicates if the operator applied is 'NOT IN' or 'IN'
        /// </summary>
        public bool NotInList
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the values to compare.
        /// </summary>
        public System.Collections.IEnumerable Values
        {
            get;
            set;
        }

        /// <summary>
        /// Writes numeric values as literals instead of parameterized values.
        /// </summary>
        public bool UseLiteralNumbers { get; set; }

        /// <summary>
        /// Writes the filter to the specified <see cref="SqlTextWriter"/> and adds any parameters to the specified <see cref="SqlBuildArguments"/>.
        /// </summary>
        /// <param name="writer">The instance of <see cref="SqlTextWriter"/> to write the T-SQL statement to.</param>
        /// <param name="args">An instance of <see cref="SqlBuildArguments"/> to use for parameters.</param>
        protected override void RenderInternal(SqlTextWriter writer, SqlBuildArguments args)
        {
            writer.Render(this);
        }
        
    }
}
