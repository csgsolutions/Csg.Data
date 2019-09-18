using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csg.Data.Sql
{
    public struct SqlPagingOptions
    {
        /// <summary>
        /// Gets or sets a value that determines the limit on the total row count to be returned from a query.
        /// </summary>
        public int Limit { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates the starting position (zero based) of the rows that will be returned from the query.
        /// </summary>
        public int Offset { get; set; }
    }
}
