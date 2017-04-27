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
        None,
        Minimum,
        Maximum,
        Average,
        Sum,
        Count,
        CountDistinct,
        StDev
    }
}
