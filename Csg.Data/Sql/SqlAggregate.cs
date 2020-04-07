using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csg.Data.Sql
{
    /// <summary>
    /// Represents the T-SQL aggregate function to apply on a field when the query supports GROUP BY.
    /// </summary>
    public enum SqlAggregate
    {
        /// <summary>
        /// No aggregate will be applied
        /// </summary>
        None,
        /// <summary>
        /// The minimum value
        /// </summary>
        Minimum,
        /// <summary>
        /// The maximum value
        /// </summary>
        Maximum,
        /// <summary>
        /// The average value
        /// </summary>
        Average,
        /// <summary>
        /// The sum value
        /// </summary>
        Sum,
        /// <summary>
        /// The count of rows
        /// </summary>
        Count,
        /// <summary>
        /// The count of distinct rows
        /// </summary>
        CountDistinct,
        /// <summary>
        /// The standard deviation
        /// </summary>
        StDev
    }
}
