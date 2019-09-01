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
            var resolvedArguments = this.Arguments.Select(arg =>
            {
                if (arg is ISqlTable table)
                {
                    return writer.FormatQualifiedIdentifierName(args.TableName(table));
                }
                else if (arg is System.Data.Common.DbParameter dbParam)
                {
                    return string.Concat("@", args.CreateParameter(dbParam.Value, dbParam.DbType));
                }
                else if (arg is DbParameterValue paramValue)
                {
                    return string.Concat("@", args.CreateParameter(paramValue.Value, paramValue.DbType, paramValue.Size));
                }
                else
                {
                    return string.Concat("@", args.CreateParameter(arg.ToString(), System.Data.DbType.String));
                }
            }).ToArray();

            writer.WriteBeginGroup();
            writer.Write(string.Format(this.SqlText, resolvedArguments));
            writer.WriteEndGroup();
        }
    }
}
