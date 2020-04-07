using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csg.Data.Abstractions
{
    /// <summary>
    /// When implemented in a derived class, provides the ability to add filters to a query expression.
    /// </summary>
    public interface IWhereClause
    {
        /// <summary>
        /// Adds the given filter to the underlying query expression.
        /// </summary>
        /// <param name="filter"></param>
        IWhereClause AddFilter(Csg.Data.Sql.ISqlFilter filter);

        /// <summary>
        /// Gets the root table of the underlying query.
        /// </summary>
        Csg.Data.Sql.ISqlTable Root { get; }
    }
}
