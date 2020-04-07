using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csg.Data.Sql
{
    /// <summary>
    /// Represents a column in a SELECT statement defined as a raw unquoted value.
    /// </summary>
    public class SqlRawColumn : ISqlColumn
    {
        /// <summary>
        /// Initializes a new instance with the given value.
        /// </summary>
        /// <param name="value">The raw value expression to be rendered</param>
        /// <param name="args">Argument values referenced in the value</param>
        public SqlRawColumn(string value, params object[] args)
        {
            this.Value = value;
            this.Arguments = args;
        }

        /// <summary>
        /// Initializes a new instance with the given value and alias.
        /// </summary>
        /// <param name="value">The raw value expression to be rendered</param>
        /// <param name="alias">The column alias to asssign</param>
        /// <param name="args">Argument values referenced in the value</param>
        public SqlRawColumn(string value, string alias, params object[] args)
        {
            this.Value = value;
            this.Alias = alias;
        }

        /// <summary>
        /// Gets a value that indicates if the column is an aggregate.
        /// </summary>
        public bool IsAggregate
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Get or sets the <see cref="ISqlTable"/> table associated with the column.
        /// </summary>
        public ISqlTable Table { get; set; }

        /// <summary>
        /// Gets or sets the raw value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the column alias to use when rendering the SELECT statement.
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// A collection of arguments that will be used to replace string format placeholders {0}, {1}, etc. Argument values can be tables, parameters, or literal values.
        /// </summary>
        public object[] Arguments { get; set; }

        /// <summary>
        /// Gets the column alias to use when rendering the SELECT statement.
        /// </summary>
        /// <returns>The name of the column to use.</returns>
        public string GetAlias()
        {
            return this.Alias;
        }


        /// <summary>
        /// Renders the raw value to the writer with its alias, if it exists
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="args"></param>
        public void Render(SqlTextWriter writer, SqlBuildArguments args)
        {
            RenderValueExpression(writer, args);

            if (this.Alias != null)
            {
                writer.WriteSpace();
                writer.Write(SqlConstants.AS);
                writer.WriteSpace();
                writer.WriteColumnName(this.Alias);
            }
        }

        /// <summary>
        /// Renders only the raw value to the writer
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="args"></param>
        public void RenderValueExpression(SqlTextWriter writer, SqlBuildArguments args)
        {
            string[] resolvedArguments = new string[0];

            if (this.Arguments != null)
            {
                resolvedArguments = this.Arguments.Select(arg =>
                {
                    if (arg is ISqlTable table)
                    {
                        return writer.FormatQualifiedIdentifierName(args.TableName(table));
                    }
                    else if (arg is System.Data.Common.DbParameter dbParam)
                    {
                        return string.Concat("@", dbParam.ParameterName);
                    }
                    else if (arg is DbParameterValue paramValue)
                    {
                        return string.Concat("@", paramValue.ParameterName);
                    }
                    else if (arg is string)
                    {
                        return string.Concat("@", args.CreateParameter(arg.ToString(), System.Data.DbType.String));
                    }
                    else
                    {
                        return string.Concat("@", args.CreateParameter(arg, System.Data.DbType.String));
                    }
                }).ToArray();
            }

            writer.Write(string.Format(this.Value, resolvedArguments));
        }
    }
}
