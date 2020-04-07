using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csg.Data.Sql
{
    /// <summary>
    /// Represents a SQL join expression to a table
    /// </summary>
    public interface ISqlJoin : ISqlStatementElement
    {
        /// <summary>
        /// Gets the joined table.
        /// </summary>
        ISqlTable JoinedTable { get; }
    }
}
