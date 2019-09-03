using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csg.Data.Sql
{
    public class SqlRawFilter : Csg.Data.Sql.ISqlFilter
    {
        public SqlRawFilter(string sqlText, params object[] args)
        {
            this.SqlText = sqlText;
            this.Arguments = args;
        }

        public string SqlText { get; set; }

        public object[] Arguments { get; set; }

        void ISqlStatementElement.Render(SqlTextWriter writer, SqlBuildArguments args)
        {
            writer.Render(this);           
        }
    }
}
