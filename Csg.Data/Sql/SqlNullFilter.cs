using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csg.Data.Sql
{
    /// <summary>
    /// Provides a T-SQL comparison for a column having a NULL value or not.
    /// </summary>
    public class SqlNullFilter : SqlSingleColumnFilterBase
    {
        public SqlNullFilter(ISqlTable table, string columnName, bool isNull)
            : base(table, columnName)
        {
            this.IsNull = isNull;
        }

        public bool IsNull
        {
            get;
            set;
        }

        protected override void RenderInternal(Abstractions.ISqlTextWriter writer)
        {
            writer.Render(this);
        }
    }
}
