using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csg.Data.Abstractions;

namespace Csg.Data.Sql
{

    /// <summary>
    /// Renders a column reference.
    /// </summary>
    public class SqlColumn : SqlColumnBase
    {
        /// <summary>
        /// Initializes a new instance with the given table and column name.
        /// </summary>
        /// <param name="table">The table expression that this column references.</param>
        /// <param name="columnName">The name of the column.</param>
        public SqlColumn(ISqlTable table, string columnName) 
            : base(table, columnName)
        {
            this.ColumnName = columnName;
        }

        /// <summary>
        /// Initializes a new instance with the given table, column name, and column alias.
        /// </summary>
        /// <param name="table">The table expression that this column references.</param>
        /// <param name="columnName">The name of the column.</param>
        /// <param name="alias">The alias to use when selecting the column.</param>
        public SqlColumn(ISqlTable table, string columnName, string alias) 
            : base(table, alias)
        {
            this.ColumnName = columnName;
        }

        /// <summary>
        /// Gets or sets the column name.
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// Renders the column name, table expression reference, and alias.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="args"></param>
        protected override void Render(Abstractions.ISqlTextWriter writer)
        {
            writer.Render(this);            
        }

        /// <summary>
        /// Renders only the value expression of the column.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="args"></param>
        protected override void RenderValueExpression(ISqlTextWriter writer)
        {
            writer.RenderValue(this);
        }

        /// <summary>
        /// Creates a <see cref="SqlColumn"/> from the given string expression and table expression.
        /// </summary>
        /// <param name="table">The table expression that this column references.</param>
        /// <param name="columnExpression">A column expression in the form of a single name, or name and alias.</param>
        /// <returns></returns>
        public static SqlColumn Parse(ISqlTable table, string columnExpression)
        {
            string[] parts = columnExpression.Split(new string[] { "AS" }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 2)
            {
                return new SqlColumn(table, parts[0].Trim(), parts[1].Trim());
            }
            else if (parts.Length == 1)
            {
                return new SqlColumn(table, parts[0].Trim());
            }
            else
            {
                throw new FormatException("The format of the columnExpression is not valid.");
            }
        }

    }
}
