using System.Data;
using System.Data.Common;
using System.Linq;

namespace Csg.Data.Sql;

public class SqlRawFilter : ISqlFilter
{
    public SqlRawFilter(string sqlText, params object[] args)
    {
        SqlText = sqlText;
        Arguments = args;
    }

    public string SqlText { get; set; }

    public object[] Arguments { get; set; }

    void ISqlStatementElement.Render(SqlTextWriter writer, SqlBuildArguments args)
    {
        var resolvedArguments = new string[0];

        if (Arguments != null)
            resolvedArguments = Arguments.Select(arg =>
            {
                if (arg is ISqlTable table)
                    return writer.FormatQualifiedIdentifierName(args.TableName(table));
                if (arg is DbParameter dbParam)
                    return string.Concat("@", dbParam.ParameterName);
                if (arg is DbParameterValue paramValue)
                    return string.Concat("@",
                        args.CreateParameter(paramValue.ParameterName, paramValue.DbType, paramValue.Size));
                if (arg is string)
                    return string.Concat("@", args.CreateParameter(arg.ToString(), DbType.String));
                return string.Concat("@", args.CreateParameter(arg, DbType.String));
            }).ToArray();

        writer.WriteBeginGroup();
        writer.Write(SqlText, resolvedArguments);
        writer.WriteEndGroup();
    }
}