using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csg.Data.Sql
{
    /// <summary>
    /// Represents a column in a SELECT statement defined as a raw unquoted value.
    /// </summary>
    public class SqlRawColumn : ISqlColumn
    {
        /// <summary>
        /// Initializes a new instance with the given value.
        /// </summary>
        /// <param name="value"></param>
        public SqlRawColumn(string value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Initializes a new instance with the given value and alias.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="alias"></param>
        public SqlRawColumn(string value, string alias)
        {
            this.Value = value;
            this.Alias = alias;
        }

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
        /// Gets or sets the raw value.
        /// </summary>
        public string Value { get; set; }

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

        /// <summary>
        /// Renders the raw value to the writer with its alias, if it exists
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="args"></param>
        public void Render(SqlTextWriter writer, SqlBuildArguments args)
        {
            writer.Write(this.Value);
            if (this.Alias != null)
            {
                writer.WriteSpace();
                writer.Write(SqlConstants.AS);
                writer.WriteSpace();
                writer.WriteColumnName(this.Alias);
            }
        }

        /// <summary>
        /// Renders only the raw value to the writer
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="args"></param>
        public void RenderValueExpression(SqlTextWriter writer, SqlBuildArguments args)
        {
            writer.Write(this.Value);
        }
    }
}
