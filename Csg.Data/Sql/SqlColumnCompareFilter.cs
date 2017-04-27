using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csg.Data.Sql
{
    public class SqlColumnCompareFilter : ISqlFilter
    {
        public SqlColumnCompareFilter(ISqlTable leftTable, string columnName, SqlOperator @operator, ISqlTable rightTable)
            : this(leftTable, columnName, @operator, rightTable, columnName)
        {
        }

        public SqlColumnCompareFilter(ISqlTable leftTable, string leftColumnName, SqlOperator @operator, ISqlTable rightTable, string rightColumnName)
        {
            this.LeftTable = leftTable;
            this.LeftColumnName = leftColumnName;
            this.RightColumnName = rightColumnName;
            this.Operator = @operator;
            this.RightTable = rightTable;
        }

        public ISqlTable LeftTable { get; set; }
        public string LeftColumnName { get; set; }
        public SqlOperator Operator { get; set; }
        public ISqlTable RightTable { get; set; }
        public string RightColumnName { get; set; }

        #region ISqlClause Members

        void ISqlStatementElement.Render(SqlTextWriter writer, SqlBuildArguments args)
        {
            writer.WriteBeginGroup();
            writer.WriteColumnName(this.LeftColumnName, args.TableName(this.LeftTable));
            writer.WriteOperator(this.Operator);
            writer.WriteColumnName(this.RightColumnName, args.TableName(this.RightTable));
            writer.WriteEndGroup();
        }

        #endregion
    }
}
