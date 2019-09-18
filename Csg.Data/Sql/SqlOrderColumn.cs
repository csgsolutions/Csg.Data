using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csg.Data.Sql
{
    public class SqlOrderColumn : ISqlStatementElement
    {
        public static SqlOrderColumn Parse(string sortSegment)
        {
            var parts = sortSegment.Split(' ');
            var result = new SqlOrderColumn();

            result.ColumnName = parts[0];

            if (parts.Length == 2)
            {
                result.SortDirection = parts[1].StartsWith("ASC", StringComparison.OrdinalIgnoreCase) ? DbSortDirection.Ascending : DbSortDirection.Descending;
            }

            return result;
        }

        public string ColumnName { get; set; }

        public DbSortDirection SortDirection { get; set; }

        #region ISqlStatementElement Members

        void ISqlStatementElement.Render(Abstractions.ISqlTextWriter writer)
        {
            writer.Render(this);
        }

        #endregion
    }
}
