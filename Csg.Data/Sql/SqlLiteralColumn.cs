using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csg.Data.Abstractions;

namespace Csg.Data.Sql
{
    /// <summary>
    /// Represents a column in a SELECT statement defined as a literal value.
    /// </summary>
    public class SqlLiteralColumn<TValue> : ISqlColumn
    {
        /// <summary>
        /// Initializes a new instance with the given value.
        /// </summary>
        /// <param name="value"></param>
        public SqlLiteralColumn(TValue value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Initializes a new instance with the given value and alias.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="alias"></param>
        public SqlLiteralColumn(TValue value, string alias)
        {
            this.Value = value;
            this.Alias = alias;
        }

        /// <summary>
        /// Gets or sets the literal value.
        /// </summary>
        public TValue Value { get; set; }

        /// <summary>
        /// Gets a value that indicates if the column is an aggregate.
        /// </summary>
        public bool IsAggregate
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Get or sets the <see cref="ISqlTable"/> table associated with the column.
        /// </summary>
        public ISqlTable Table { get; set; }

        /// <summary>
        /// Gets or sets the column alias to use when rendering the SELECT statement.
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Gets the column alias to use when rendering the SELECT statement.
        /// </summary>
        /// <returns>The name of the column to use.</returns>
        public string GetAlias()
        {
            return this.Alias;
        }

        public void Render(Abstractions.ISqlTextWriter writer)
        {
            writer.Render(this);
        }

        public void RenderValueExpression(ISqlTextWriter writer)
        {
            writer.RenderValue(this);
        }
    }
}
