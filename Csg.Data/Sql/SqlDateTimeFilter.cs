using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csg.Data.Sql
{
 
    /// <summary>
    /// Creates a filter that matches a data column if it is between two instances in time.
    /// </summary>
    public class SqlDateTimeFilter : ISqlFilter
    {
        public SqlDateTimeFilter(ISqlTable table, string columnName, DateTime beginDate, DateTime endDate)      
        {
            this.Table = table;
            this.ColumnName = columnName;
            this.BeginDate = beginDate;
            this.EndDate = endDate;
        }

        public ISqlTable Table { get; set; }

        public string ColumnName { get; set; }
    
        public virtual DateTime BeginDate { get; set;}

        public virtual DateTime EndDate { get; set; }

        
        #region ISqlStatementElement Members

        void ISqlStatementElement.Render(SqlTextWriter writer, SqlBuildArguments args)
        {
            writer.Render(this);
        }

        #endregion
    }





}
