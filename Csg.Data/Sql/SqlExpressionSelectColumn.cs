using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csg.Data.Abstractions;

namespace Csg.Data.Sql
{
    /// <summary>
    /// Renders a T-SQL expression based column for a SELECT statement.
    /// </summary>
    public class SqlExpressionSelectColumn : SqlColumnBase
    {
        /// <summary>
        /// Creates a new instance with the given table, expression, and column name alias.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="expression"></param>
        /// <param name="alias"></param>
        public SqlExpressionSelectColumn(ISqlTable table, string expression, string alias) : base(table, alias)
        {
            this.Expression = expression;
        }

        /// <summary>
        /// Gets or sets the SQL expression providing the value for the column.
        /// </summary>
        public string Expression { get; set; }

        /// <summary>
        /// Gets a collection of tables and parameter values that can be referenced by index {0}, {1}, etc.
        /// </summary>
        public IList<object> Arguments
        {
            get
            {
                if (_arguments == null)
                {
                    _arguments = new List<object>();
                }
                return _arguments;
            }
        }
        private IList<object> _arguments;

        /// <summary>
        /// Renders the entire SQL statement.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="args"></param>
        protected override void Render(SqlTextWriter writer, SqlBuildArguments args)
        {
            writer.Render(this);          
        }

        protected override void RenderValueExpression(ISqlTextWriter writer, SqlBuildArguments args)
        {
            writer.RenderValue(this);
        }
    }
}
