using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csg.Data.Sql
{

    /// <summary>
    /// Represents an EXISTS({selectStatement}) filter critera in a SQL statement.
    /// </summary>
    public class SqlExistFilter : Csg.Data.Sql.ISqlFilter
    {
        /// <summary>
        /// Initializes an instance of the filter.
        /// </summary>
        /// <param name="selectStatement"></param>
        public SqlExistFilter(SqlSelectBuilder selectStatement)
        {
            this.Statement = selectStatement;
        }

        /// <summary>
        /// Gets or sets the SQL statement that will be executed inside the EXISTS.
        /// </summary>
        public SqlSelectBuilder Statement { get; set; }

        void ISqlStatementElement.Render(SqlTextWriter writer, SqlBuildArguments args)
        {
            writer.Render(this);            
        }
    }
}
