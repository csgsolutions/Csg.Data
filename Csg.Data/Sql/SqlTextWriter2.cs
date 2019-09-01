﻿using Csg.Data.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csg.Data.Sql
{
    public class SqlTextWriter2 : System.IO.TextWriter, ISqlTextWriter
    {
        private System.IO.TextWriter InnerWriter;
        private const string COLUMN_SEPERATOR = ",";
        private const string JOIN_SEPERATOR = " ";
        private const string QUOTE = "\"";
        private const string QUOTEQUOTE = "\"\"";

        /// <summary>
        /// Gets or sets a value that indicates if the writer should output "pretty" SQL that includes un-necessary line breaks and such.
        /// </summary>
        public bool Format { get; set; }

        /// <summary>
        /// Gets or sets a value that indiciates if the writer should write quoted Identifiers.
        /// </summary>
        public bool UseQuotedIdentifiers { get; set; }

        public SqlBuildArguments args { get; set; }

        public SqlTextWriter2() : base()
        {
            this.InnerWriter = new System.IO.StringWriter();
        }

        public SqlTextWriter2(System.IO.TextWriter writer) : base()
        {
            this.InnerWriter = writer;
        }

        public SqlTextWriter2(StringBuilder sb) : base()
        {
            this.InnerWriter = new System.IO.StringWriter(sb);
        }

        #region SQL SERVER Specific Statics

        public static string FormatSqlServerIdentifier(string value, bool useQuotedIdentifiers = false)
        {
            if (useQuotedIdentifiers && value.StartsWith(QUOTE) && value.EndsWith(QUOTE) && value.Length > 2)
            {
                value = value.Substring(1, value.Length - 2);
            }
            else if (value.StartsWith("[") && value.EndsWith("]") && value.Length > 2)
            {
                value = value.Substring(1, value.Length - 2);
            }

            if (useQuotedIdentifiers || value.Contains("[") || value.Contains("]"))
            {
                value = value.Replace(QUOTE, QUOTEQUOTE);
                value = string.Concat(QUOTE, value, QUOTE);
            }
            else
            {
                value = string.Concat("[", value, "]");
            }

            return value;
        }

        public static string ConvertSqlOperatorToString(SqlOperator oper)
        {
            switch (oper)
            {
                case SqlOperator.Equal: return "=";
                case SqlOperator.GreaterThan: return ">";
                case SqlOperator.GreaterThanOrEqual: return ">=";
                case SqlOperator.LessThan: return "<";
                case SqlOperator.LessThanOrEqual: return "<=";
                case SqlOperator.NotEqual: return "<>";
                default: throw new InvalidOperationException("Invalid operator");
            }
        }

        public static string ConvertSqlAggregateToString(SqlAggregate aggregateType)
        {
            switch (aggregateType)
            {
                case SqlAggregate.Minimum: return "MIN";
                case SqlAggregate.Maximum: return "MAX";
                case SqlAggregate.Sum: return "SUM";
                case SqlAggregate.Count: return "COUNT";
                case SqlAggregate.CountDistinct: return "COUNT";
                case SqlAggregate.Average: return "AVG";
                case SqlAggregate.StDev: return "STDEV";
                default: throw new InvalidOperationException("Invalid aggregate");
            }
        }

        public static string ConvertSqlLogicToString(SqlLogic logic)
        {
            return logic.ToString().ToUpper();
        }

        public static string ConvertDbSortDirectionToString(DbSortDirection direction)
        {
            switch (direction)
            {
                case DbSortDirection.Ascending: return "ASC";
                case DbSortDirection.Descending: return "DESC";
                default: return string.Empty;
            }
        }

        public static string FormatTableName(string name)
        {
            var parts = name.Split(new char[] { '.' });
            for (int x = 0; x < parts.Length; x++)
            {
                parts[x] = FormatSqlServerIdentifier(parts[x]);
            }

            return string.Join(SqlConstants.DOT, parts);
        }

        #endregion

        #region Language Specifics

        protected virtual string FormatIdentifier(string value)
        {
            return FormatSqlServerIdentifier(value, this.UseQuotedIdentifiers);
        }

        public virtual string FormatColumnName(string name)
        {
            return this.FormatIdentifier(name);
        }

        public virtual string FormatQualifiedIdentifierName(string name)
        {
            return FormatTableName(name);
        }

        public virtual string ConvertOperatorToString(SqlOperator @operator)
        {
            return ConvertSqlOperatorToString(@operator);
        }

        public virtual string ConvertAggregateToString(SqlAggregate aggregate)
        {
            return ConvertSqlAggregateToString(aggregate);
        }

        public virtual string ConvertLogicToString(SqlLogic logic)
        {
            return ConvertSqlLogicToString(logic);
        }

        public virtual string ConvertSortDirectionToString(DbSortDirection direction)
        {
            return ConvertDbSortDirectionToString(direction);
        }

        public virtual string ColumnSeperator
        {
            get
            {
                return COLUMN_SEPERATOR;
            }
        }

        public override Encoding Encoding => throw new NotImplementedException();

        #endregion

        public void WriteColumnName(string columnName)
        {
            Write(this.FormatColumnName(columnName));
        }

        public void WriteColumnName(string columnName, string tableName)
        {
            WriteTableName(tableName);
            Write(SqlConstants.DOT);
            WriteColumnName(columnName);
        }

        public void WriteColumnName(string columnName, string tableName, string alias)
        {
            WriteTableName(tableName);

            Write(SqlConstants.DOT);
            WriteColumnName(columnName);

            if (!string.IsNullOrEmpty(alias) && !alias.Equals(columnName))
            {
                WriteSpace();
                Write(SqlConstants.AS);
                WriteSpace();
                WriteColumnName(alias);
            }
        }

        public void WriteTableName(string tableName)
        {
            this.WriteTableName(tableName, null);
        }

        public void WriteTableName(string tableName, string alias)
        {
            Write(FormatQualifiedIdentifierName(tableName));
            if (!string.IsNullOrEmpty(alias))
            {
                WriteSpace();
                Write(SqlConstants.AS);
                WriteSpace();
                Write(this.FormatIdentifier(alias));
            }
        }

        public void WriteParameter(string parameterName)
        {
            this.Write("@");
            this.Write(parameterName);
        }

        public void WriteAggregateColumn(string columnName, string tableName, SqlAggregate aggregateType, string outputName)
        {
            Write(ConvertSqlAggregateToString(aggregateType));
            WriteBeginGroup();
            if (aggregateType == SqlAggregate.CountDistinct)
            {
                Write(SqlConstants.DISTINCT);
                WriteSpace();
            }
            WriteColumnName(columnName, tableName);
            WriteEndGroup();
            WriteSpace();
            Write(SqlConstants.AS);
            WriteSpace();
            WriteColumnName(outputName);
        }

        public void WriteAggregate(string columnName, string tableName, SqlAggregate aggregateType)
        {
            Write(ConvertSqlAggregateToString(aggregateType));
            WriteBeginGroup();
            WriteColumnName(columnName, tableName);
            WriteEndGroup();
        }

        public void WriteOperator(SqlOperator oper)
        {
            Write(SqlTextWriter.ConvertSqlOperatorToString(oper));
        }

        public void WriteSelect()
        {
            this.Write(SqlConstants.SELECT);
        }

        public virtual void WriteBeginGroup()
        {
            Write(SqlConstants.BEGINGROUP);
        }

        public virtual void WriteEndGroup()
        {
            Write(SqlConstants.ENDGROUP);
        }

        public virtual void WriteLogic(SqlLogic logic)
        {
            this.Write(logic.ToString().ToUpper());
        }

        public virtual void WriteEndStatement()
        {
            Write(SqlConstants.SEMICOLON);
        }

        public void WriteSpace()
        {
            Write(SqlConstants.SPACE);
        }

        public virtual void WriteQuotedString(string value)
        {
            value = value.Replace(SqlConstants.SINGLE_QUOTE, SqlConstants.DOUBLE_SINGLE_QUOTE);
            Write(SqlConstants.SINGLE_QUOTE);
            Write(value);
            Write(SqlConstants.SINGLE_QUOTE);
        }

        public virtual void WriteLiteralValue(object value)
        {
            //TODO: What the crap does this do with dates and other things that would need to be quoted besides strings?
            if (value is string)
            {
                WriteQuotedString((string)value);
            }
            else
            {
                Write(value);
            }
        }

        public virtual void RenderAll<T>(IEnumerable<T> items, string seperator)
        {
            bool first = true;
            foreach (var item in items)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    this.Write(seperator);
                }

                RenderObject(item);
            }
        }

        public void RenderObject(object obj)
        {
            //TODO: Consider typeof(ISqlTextWriter) intead???
            var renderMethod = typeof(SqlTextWriter2).GetMethod("Render", new Type[] { obj.GetType() });
            if (renderMethod == null)
            {
                throw new NotSupportedException();
            }

            renderMethod.Invoke(this, new object[] { obj });
        }


        public void WriteNewLine()
        {
            if (this.Format)
            {
                Write("\r\n");
            }
        }

        public void WriteSortColumn(string columnName, DbSortDirection direction)
        {
            this.WriteColumnName(columnName);
            if (direction != DbSortDirection.None)
            {
                this.WriteSpace();
                this.Write(ConvertDbSortDirectionToString(direction));
            }
        }

        public virtual void WriteRankOver(string columnName, string tableName, SqlAggregate aggregateType, bool rankDescending)
        {
            Write(SqlConstants.RANK_OVER);
            WriteBeginGroup();
            Write(SqlConstants.ORDER_BY);
            WriteSpace();
            Write(ConvertSqlAggregateToString(aggregateType));
            WriteBeginGroup();
            WriteColumnName(columnName, tableName);
            WriteEndGroup();
            if (rankDescending)
            {
                WriteSpace();
                Write(SqlConstants.DESC);
            }
            WriteEndGroup();
        }

        public virtual void WriteRankOver(string columnName, string tableName, SqlAggregate aggregateType, string alias, bool rankDescending)
        {
            this.WriteRankOver(columnName, tableName, aggregateType, rankDescending);
            WriteSpace();
            Write(SqlConstants.AS);
            WriteSpace();
            WriteColumnName(alias);
        }



        // -----------------------------------------------

        public void Render(SqlColumn src)
        {
            if (src.Aggregate == SqlAggregate.None)
            {
                this.WriteColumnName(src.ColumnName, args.TableName(src.Table), src.Alias);
            }
            else
            {
                this.WriteAggregateColumn(src.ColumnName, args.TableName(src.Table), src.Aggregate, src.Alias);
            }
        }

        public void Render(SqlColumnCompareFilter src)
        {
            this.WriteBeginGroup();
            this.WriteColumnName(src.LeftColumnName, args.TableName(src.LeftTable));
            this.WriteOperator(src.Operator);
            this.WriteColumnName(src.RightColumnName, args.TableName(src.RightTable));
            this.WriteEndGroup();
        }

        public void Render(SqlCompareFilter src)
        {
            this.WriteBeginGroup();
            this.WriteColumnName(src.ColumnName, args.TableName(src.Table));
            this.WriteOperator(src.Operator);

            if (src.EncodeValueAsLiteral)
            {
                this.WriteLiteralValue(src.Value);
            }
            else
            {
                this.WriteParameter(args.CreateParameter(src.Value, src.DataType));
            }

            this.WriteEndGroup();
        }

        public void Render(SqlCountFilter src)
        {
            args.AssignAlias(src.SubQueryTable);

            var subquery = new SqlSelectBuilder(src.SubQueryTable);
            var subQueryColumn = new SqlColumn(src.SubQueryTable, src.SubQueryColumn);
            subQueryColumn.Aggregate = SqlAggregate.Count;
            subQueryColumn.Alias = "Cnt";
            subquery.Columns.Add(subQueryColumn);

            foreach (var filter in src.SubQueryFilters)
            {
                subquery.Filters.Add(filter);
            }

            this.WriteBeginGroup();
            this.WriteBeginGroup();
            subquery.Render(this, args);
            this.WriteEndGroup();

            this.WriteSpace();
            this.WriteOperator(src.CountOperator);
            this.WriteSpace();
            this.WriteParameter(args.CreateParameter(src.CountValue, System.Data.DbType.Int32));

            this.WriteEndGroup();
        }

        public void Render(SqlDateFilter src)
        {
            this.WriteBeginGroup();
            this.WriteColumnName(src.ColumnName, args.TableName(src.Table));
            this.WriteOperator(SqlOperator.GreaterThanOrEqual);
            this.WriteParameter(args.CreateParameter(src.BeginDate.Date, System.Data.DbType.DateTime));
            this.WriteSpace();
            this.Write(SqlConstants.AND);
            this.WriteSpace();
            this.WriteColumnName(src.ColumnName, args.TableName(src.Table));
            this.WriteOperator(SqlOperator.LessThanOrEqual);
            //TODO: Get data type from object
            this.WriteParameter(args.CreateParameter(src.EndDate.Date, System.Data.DbType.DateTime));
            this.WriteEndGroup();
        }

        public void Render(SqlDateTimeFilter src)
        {
            this.WriteBeginGroup();
            this.WriteColumnName(src.ColumnName, args.TableName(src.Table));
            this.WriteOperator(SqlOperator.GreaterThanOrEqual);
            this.WriteParameter(args.CreateParameter(src.BeginDate, System.Data.DbType.DateTime));
            this.WriteSpace();
            this.Write(SqlConstants.AND);
            this.WriteSpace();
            this.WriteColumnName(src.ColumnName, args.TableName(src.Table));
            this.WriteOperator(SqlOperator.LessThanOrEqual);
            //TODO: Get data type from object
            this.WriteParameter(args.CreateParameter(src.EndDate, System.Data.DbType.DateTime));
            this.WriteEndGroup();
        }

        public void Render(SqlExistFilter src)
        {
            this.WriteBeginGroup();
            this.Write("EXISTS ");
            this.WriteBeginGroup();
            src.Statement.Render(this, args);
            this.WriteEndGroup();
            this.WriteEndGroup();
        }

        public void Render(SqlExpressionSelectColumn src)
        {
            var expr = src.Expression;

            expr = expr.Replace("{0}", args.TableName(src.Table));
            if (src.ReferenceTables != null && src.ReferenceTables.Count > 0)
            {
                for (var i = 0; i < src.ReferenceTables.Count; i++)
                {
                    expr = expr.Replace(string.Concat("{", i + 1, "}"), args.TableName(src.ReferenceTables[i]));
                }
            }

            this.WriteBeginGroup();
            this.Write(expr);
            this.WriteEndGroup();

            this.WriteSpace();
            this.Write(SqlConstants.AS);
            this.WriteSpace();
            this.WriteColumnName(src.Alias);
        }

        public void Render(SqlFilterCollection src)
        {
            if (src.Count <= 0)
                return;
            this.WriteBeginGroup();

            this.RenderAll(src, string.Concat(" ", ((src.Logic == SqlLogic.And) ? SqlConstants.AND : SqlConstants.OR).ToString().ToUpper(), " "));

            this.WriteEndGroup();
        }

        public void Render(SqlJoin src)
        {
            this.WriteSpace();
            this.Write(src.JoinType.ToString().ToUpper());
            this.WriteSpace();
            this.Write("JOIN");
            this.WriteSpace();

            //TODO: This is causing joined tables to render their conditions here, which is not right.
            src.RightTable.Render(this, args);

            if (src.JoinType != SqlJoinType.Cross)
            {
                this.WriteSpace();
                this.Write(SqlConstants.ON);
                this.WriteSpace();
                this.RenderAll<ISqlFilter>(src.Conditions, string.Concat(SqlConstants.SPACE, ConvertSqlLogicToString(SqlLogic.And), SqlConstants.SPACE));
            }

            this.WriteNewLine();
        }

        public void Render(SqlListFilter src)
        {
            //TODO: make this impl agnostic
            bool first = true;

            if (src.Values == null)
            {
                throw new InvalidOperationException(string.Format(ErrorMessage.SqlListFilter_CollectionIsEmpty, this.ColumnName));
            }

            this.WriteBeginGroup();
            this.WriteColumnName(src.ColumnName, args.TableName(src.Table));
            this.WriteSpace();

            if (src.NotInList)
            {
                this.Write(SqlConstants.NOT);
                this.WriteSpace();
            }
            this.Write(SqlConstants.IN);
            this.WriteSpace();

            this.WriteBeginGroup();
            foreach (var v in src.Values)
            {
                if (!first)
                    this.Write(",");
                else
                    first = false;

                switch (src.DataType)
                {
                    case System.Data.DbType.String:
                    case System.Data.DbType.StringFixedLength:
                    case System.Data.DbType.AnsiString:
                    case System.Data.DbType.AnsiStringFixedLength:
                    case System.Data.DbType.Object:
                        this.WriteParameter(args.CreateParameter(v.ToString(), src.DataType, src.Size)); break;
                    case System.Data.DbType.Boolean:
                        this.Write(Convert.ToBoolean(v) ? 1 : 0); break;
                    case System.Data.DbType.Int16:
                    case System.Data.DbType.Int32:
                    case System.Data.DbType.Int64:
                        if (src.UseLiteralNumbers)
                        {
                            this.Write(Convert.ToInt64(v).ToString());
                        }
                        else
                        {
                            this.WriteParameter(args.CreateParameter(v, src.DataType, src.Size));
                        }
                        break;
                    default:
                        this.WriteParameter(args.CreateParameter(v, src.DataType, src.Size)); break;
                }
            }

            if (first)
            {
                throw new InvalidOperationException(string.Format(ErrorMessage.SqlListFilter_CollectionIsEmpty, src.ColumnName));
            }

            this.WriteEndGroup();
            this.WriteEndGroup();
        }

        public void Render<T>(SqlLiteralColumn<T> src)
        {
            this.WriteLiteralValue(src.Value);
            if (src.Alias != null)
            {
                this.WriteSpace();
                this.Write(SqlConstants.AS);
                this.WriteSpace();
                this.WriteColumnName(src.Alias);
            }
        }

        public void Render(SqlNullFilter src)
        {
            this.WriteBeginGroup();
            this.WriteColumnName(src.ColumnName, args.TableName(src.Table));
            this.Write((src.IsNull) ? " IS NULL" : " IS NOT NULL");
            this.WriteEndGroup();
        }

        public void Render(SqlOrderColumn src)
        {
            this.WriteSortColumn(src.ColumnName, src.SortDirection);
        }

        public void Render(SqlParameterFilter src)
        {
            this.WriteBeginGroup();
            this.WriteColumnName(src.ColumnName, args.TableName(src.Table));
            this.WriteOperator(src.Operator);
            this.Write(src.ParameterName);
            this.WriteEndGroup();
        }

        public void Render(SqlRankColumn src)
        {
            this.WriteRankOver(src.ColumnName, args.TableName(src.Table), src.Aggregate, src.Alias, src.RankDescending);
        }

        public void Render(SqlRawFilter src)
        {
            var resolvedArguments = src.Arguments.Select(arg =>
            {
                if (arg is ISqlTable table)
                {
                    return this.FormatQualifiedIdentifierName(args.TableName(table));
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

            this.WriteBeginGroup();
            this.Write(string.Format(src.SqlText, resolvedArguments));
            this.WriteEndGroup();
        }

        public void Render(SqlStringMatchFilter src)
        {
            string s = src.Value;
            if (s == null)
            {
                s = string.Empty;
            }

            this.WriteBeginGroup();
            this.WriteColumnName(src.ColumnName, args.TableName(src.Table));
            this.WriteSpace();
            this.Write(SqlConstants.LIKE);
            this.WriteSpace();
            this.WriteParameter(args.CreateParameter(SqlStringMatchFilter.DecorateValue(src.Value, src.Operator), src.DataType));
            this.WriteEndGroup();
        }

        public void Render(SqlSubQueryFilter src)
        {
            this.WriteBeginGroup();

            this.WriteColumnName(src.ColumnName, args.TableName(src.Table));
            this.WriteSpace();

            if (src.Condition == SubQueryMode.NotInList)
            {
                this.Write(SqlConstants.NOT);
                this.WriteSpace();
            }

            this.Write(SqlConstants.IN);

            this.WriteSpace();

            args.AssignAlias(src.SubQueryTable);

            var builder = new SqlSelectBuilder(src.SubQueryTable);
            var subQueryColumn = new SqlColumn(src.SubQueryTable, src.SubQueryColumn);
            builder.Columns.Add(subQueryColumn);

            foreach (var filter in src.SubQueryFilters)
            {
                builder.Filters.Add(filter);
            }

            this.WriteBeginGroup();
            builder.Render(this, args);
            this.WriteEndGroup();

            this.WriteEndGroup();
        }

        public void Render(SqlTable src)
        {
            this.WriteTableName(src.TableName, args.TableName(src));
        }

        public void Render(SqlDerivedTable src)
        {
            this.WriteBeginGroup();
            this.Write(src.CommandText.TrimEnd(new char[] { ';' }));
            this.WriteEndGroup();
            this.WriteSpace();
            this.Write(SqlConstants.AS);
            this.WriteSpace();
            this.Write(SqlDataColumn.Format(args.TableName(src)));
        }

        public void RenderValue(SqlColumn src)
        {
            if (src.Aggregate == SqlAggregate.None)
            {
                this.WriteColumnName(src.ColumnName, args.TableName(src.Table));
            }
            else
            {
                this.WriteAggregate(src.ColumnName, args.TableName(src.Table), src.Aggregate);
            }
        }
    }
}
