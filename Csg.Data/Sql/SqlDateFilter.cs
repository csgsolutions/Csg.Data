using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csg.Data.Sql
{
    /// <summary>
    /// Creates a filter that matches a data column if it is between (beginDate &gt;= date &lt;= endDate) two dates ignoring time.
    /// </summary>
    public class SqlDateFilter : SqlDateTimeFilter
    {
        public SqlDateFilter(ISqlTable table, string columnName, DateTime beginDate, DateTime endDate) : base(table, columnName, beginDate, endDate)
        {
        }

        public override DateTime BeginDate { get => base.BeginDate; set => base.BeginDate = value.Date; }

        public override DateTime EndDate { get => base.EndDate; set => base.EndDate = value.Date; }

    }
}
