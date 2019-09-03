using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csg.Data.Sql
{
    /// <summary>
    /// Provides a basic T-SQL filter that compares a table column to a parameter.
    /// </summary>
    public class SqlParameterFilter : ISqlFilter
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="columnName">The name of the table column to compare.</param>
        /// <param name="oper">The operator to use in the comparison.</param>
        /// <param name="parameterName">The name of the parameter to compare the column with.</param>
        public SqlParameterFilter(string columnName, SqlOperator oper,  string parameterName)
        {
            this.ColumnName = columnName;
            this.Operator = oper;
            this.ParameterName = parameterName;
        }

        public ISqlTable Table { get; set; }

        /// <summary>
        /// Gets or sets the database column name to be compared
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// Gets or sets the T-SQL operator used in the comparison
        /// </summary>
        public SqlOperator Operator { get; set; }

        /// <summary>
        /// Gets or sets the name of the parameter the database column will be compared with
        /// </summary>
        public string ParameterName { get; set; }

        public void Render(SqlTextWriter writer, SqlBuildArguments args)
        {
            writer.Render(this);            
        }
    }
}
