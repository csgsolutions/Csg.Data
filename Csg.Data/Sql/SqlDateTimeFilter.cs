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
    
        public DateTime BeginDate { get; set;}

        public DateTime EndDate { get; set; }

        protected virtual DateTime GetBeginDate()
        {
            return this.BeginDate;
        }

        protected virtual DateTime GetEndDate()
        {
            return this.EndDate;
        }

        protected virtual void WriteColumnName(SqlTextWriter writer, SqlBuildArguments args)
        {           
            writer.WriteColumnName(this.ColumnName, args.TableName(this.Table));
        }
        
        #region ISqlStatementElement Members

        void ISqlStatementElement.Render(SqlTextWriter writer, SqlBuildArguments args)
        {
            writer.WriteBeginGroup();
            this.WriteColumnName(writer, args);
            writer.WriteOperator(SqlOperator.GreaterThanOrEqual);
            writer.WriteParameter(args.CreateParameter(this.GetBeginDate(), System.Data.DbType.DateTime));
            writer.WriteSpace();
            writer.Write(SqlConstants.AND);
            writer.WriteSpace();
            this.WriteColumnName(writer, args);
            writer.WriteOperator(SqlOperator.LessThanOrEqual);
            writer.WriteParameter(args.CreateParameter(this.GetEndDate(), System.Data.DbType.DateTime));
            writer.WriteEndGroup();
        }

        #endregion
    }





}
