using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csg.Data.Sql
{
    /// <summary>
    /// Represents a element of SQL statement that can be rendered.
    /// </summary>
    public interface ISqlStatementElement
    {
        /// <summary>
        /// Renders the statement to the given writer/arguments.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="args"></param>
        void Render(SqlTextWriter writer, SqlBuildArguments args);
    }
}
