using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csg.Data.Sql
{
    /// <summary>
    /// Represents the options of sort directions for a database query.
    /// </summary>
    public enum DbSortDirection
    {
        /// <summary>
        /// Unspecified sort direction
        /// </summary>
        None,
        /// <summary>
        /// Sort ascending
        /// </summary>
        Ascending,
        /// <summary>
        /// Sort descending
        /// </summary>
        Descending
    }
}
