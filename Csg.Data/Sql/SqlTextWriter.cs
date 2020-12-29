using Csg.Data.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csg.Data.Sql
{

    /// <summary>
    /// A SQL text writer for Microsoft SQL Server 2008 and later
    /// </summary>
    public class SqlTextWriter : System.IO.TextWriter, ISqlTextWriter
    {
        private System.IO.TextWriter InnerWriter;

        private const string COLUMN_SEPERATOR = ",";
        private const string JOIN_SEPERATOR = " ";
        private const string QUOTE = "\"";
        private const string QUOTEQUOTE = "\"\"";
        
        public SqlTextWriter(ISqlProvider provider) : base()
        {
            this.InnerWriter = new System.IO.StringWriter();
            this.BuildArguments = new SqlBuildArguments();
            this.Provider = provider;
        }

        public SqlTextWriter(System.IO.TextWriter writer, ISqlProvider provider) : base()
        {
            this.InnerWriter = writer;
            this.BuildArguments = new SqlBuildArguments();
            this.Provider = provider;
        }        

        public SqlTextWriter(StringBuilder sb, ISqlProvider provider) : base()
        {
            this.InnerWriter = new System.IO.StringWriter(sb);
            this.BuildArguments = new SqlBuildArguments();
            this.Provider = provider;
        }

        public Abstractions.ISqlProvider Provider { get; private set; }

        /// <summary>
        /// Gets or sets a value that indicates if the writer should output "pretty" SQL that includes un-necessary line breaks and such.
        /// </summary>
        public virtual bool Format { get; set; }

        /// <summary>
        /// Gets or sets the build arguments that will be used with this writer.
        /// </summary>
        public virtual SqlBuildArguments BuildArguments { get; protected set; }

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

        /// <summary>
        /// Gets or sets a value that indiciates if the writer should write quoted Identifiers.
        /// </summary>
        public virtual bool UseQuotedIdentifiers { get; set; }

        /// <summary>
        /// Gets the prefix value that will be used before a named parameter.
        /// </summary>
        public virtual string ParameterNamePrefix { get => "@"; }

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

        public virtual string ColumnSeperator { get => COLUMN_SEPERATOR; }

        #endregion

        #region ISqlTextWriter

        public override void Write(char value)
        {
            InnerWriter.Write(value);
        }

        public override Encoding Encoding
        {
            get { return this.InnerWriter.Encoding; }
        }

        public override string ToString()
        {
            return this.InnerWriter.ToString();
        }
        
        #region ISqlTextWriter

        public virtual void WriteNewLine()
        {
            if (this.Format)
            {
                Write("\r\n");
            }
        }

        public virtual void WriteColumnName(string columnName)
        {
            Write(this.FormatColumnName(columnName));
        }

        public virtual void WriteColumnName(string columnName, string tableName)
        {
            WriteTableName(tableName);
            Write(SqlConstants.DOT);
            WriteColumnName(columnName);
        }

        public virtual void WriteColumnName(string columnName, string tableName, string alias)
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

        public virtual void WriteTableName(string tableName)
        {
            this.WriteTableName(tableName, null);
        }

        public virtual void WriteTableName(string tableName, string alias)
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

        public virtual void WriteParameter(string parameterName)
        {
            this.Write(this.ParameterNamePrefix);
            this.Write(parameterName);
        }

        public virtual void WriteAggregateColumn(string columnName, string tableName, SqlAggregate aggregateType, string outputName)
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

        public virtual void WriteAggregate(string columnName, string tableName, SqlAggregate aggregateType)
        {
            Write(ConvertSqlAggregateToString(aggregateType));
            WriteBeginGroup();
            WriteColumnName(columnName, tableName);
            WriteEndGroup();
        }

        public virtual void WriteOperator(SqlOperator oper)
        {
            Write(SqlTextWriter.ConvertSqlOperatorToString(oper));
        }

        public virtual void WriteSelect()
        {
            this.Write(SqlConstants.SELECT);
        }



        public virtual void WriteSortColumn(string columnName, DbSortDirection direction)
        {
            this.WriteColumnName(columnName);
            if (direction != DbSortDirection.None)
            {
                this.WriteSpace();
                this.Write(ConvertDbSortDirectionToString(direction));
            }
        }


        public virtual void WriteDerivedTable(string commandText, string alias)
        {
            this.WriteBeginGroup();
            this.Write(commandText);
            this.WriteEndGroup();
            this.WriteSpace();
            this.Write(SqlConstants.AS);
            this.WriteSpace();
            this.WriteTableName(FormatTableName(alias));
        }


        public void WriteSpace()
        {
            Write(SqlConstants.SPACE);
        }


        public void WriteComma()
        {
            Write(SqlConstants.COMMA);
        }


        public void WriteColumnSeperator()
        {
            this.WriteComma();
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

        public virtual void WriteCast(string columnName, string sqlDataTypeName, string tableName)
        {
            Write(SqlConstants.CAST);
            WriteBeginGroup();
            WriteColumnName(columnName, tableName);
            WriteSpace();
            Write(SqlConstants.AS);
            WriteSpace();
            Write(sqlDataTypeName);
            WriteEndGroup();
        }

        public virtual void WriteCast(string sqlDataTypeName, Action content)
        {
            Write(SqlConstants.CAST);
            WriteBeginGroup();
            content.Invoke();
            WriteSpace();
            Write(SqlConstants.AS);
            WriteSpace();
            Write(sqlDataTypeName);
            WriteEndGroup();
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

        #endregion

        #region Custom Render Methods

        protected virtual void RenderAll<T>(IEnumerable<T> items, string seperator) where T : ISqlStatementElement
        {
            this.RenderAll(items, seperator, (a, b) => { a.Render(b); });
        }

        protected virtual void RenderAll<T>(IEnumerable<T> items, string seperator, Action<T, SqlTextWriter> renderAction) where T : ISqlStatementElement
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

                renderAction(item, this);
            }
        }

        protected virtual void RenderSelect(IEnumerable<ISqlColumn> columns, SqlBuildArguments args)
        {
            this.RenderSelect(columns, args);
        }

        protected virtual void RenderSelect(IEnumerable<ISqlColumn> columns, SqlBuildArguments args, bool distinct)
        {
            if (columns == null)
                throw new ArgumentNullException("columns");
            if (args == null)
                throw new ArgumentNullException("args");

            var items = columns.ToArray();
            this.WriteSelect();
            this.WriteSpace();

            if (distinct)
            {
                this.Write(SqlConstants.DISTINCT);
                this.WriteSpace();
            }

            if (items.Length > 0)
            {
                if (this.Format)
                {
                    this.RenderAll(items, string.Concat(COLUMN_SEPERATOR, "\r\n"));
                }
                else
                {
                    this.RenderAll(items, COLUMN_SEPERATOR);
                }
            }
            else
            {
                this.Write("*");
            }
            this.WriteNewLine();
        }

        protected virtual void RenderFrom(ISqlTable table, SqlBuildArguments args)
        {
            if (table == null)
                throw new ArgumentNullException("table");
            if (args == null)
                throw new ArgumentNullException("args");

            this.WriteSpace();
            this.Write(SqlConstants.FROM);
            this.WriteSpace();

            table.Render(this);
            this.WriteNewLine();
        }

        protected virtual void RenderWhere(IEnumerable<ISqlFilter> filters, SqlLogic logic, SqlBuildArguments args)
        {
            if (filters == null)
                throw new ArgumentNullException("filters");
            if (args == null)
                throw new ArgumentNullException("args");

            var items = filters.ToArray();
            if (items.Length > 0)
            {
                this.WriteSpace();
                this.Write(SqlConstants.WHERE);
                this.WriteSpace();
                this.RenderAll(items, string.Concat(SqlConstants.SPACE, ConvertSqlLogicToString(logic), SqlConstants.SPACE));
            }
        }

        protected virtual void RenderGroupBy(IEnumerable<ISqlColumn> columns, SqlBuildArguments args)
        {
            if (columns == null)
                throw new ArgumentNullException("columns");
            if (args == null)
                throw new ArgumentNullException("args");

            var items = columns.ToArray();
            if (items.Length > 0)
            {
                this.WriteSpace();
                this.Write(SqlConstants.GROUP_BY);
                this.WriteSpace();
                this.RenderAll(items, COLUMN_SEPERATOR, (a, b) => { a.RenderValueExpression(b); });
            }
            this.WriteNewLine();
        }

        protected virtual void RenderOrderBy(IEnumerable<SqlOrderColumn> columns, SqlBuildArguments args)
        {
            if (columns == null)
                throw new ArgumentNullException("columns");
            if (args == null)
                throw new ArgumentNullException("args");

            var items = columns.ToArray();

            if (items.Length > 0)
            {
                this.WriteSpace();
                this.Write(SqlConstants.ORDER_BY);
                this.WriteSpace();
                this.RenderAll(items, COLUMN_SEPERATOR);
            }

            this.WriteNewLine();
        }

        protected void RenderObject(object obj)
        {
            //TODO: Consider typeof(ISqlTextWriter) intead???
            var renderMethod = this.GetType().GetMethod("Render", new Type[] { obj.GetType() });
            if (renderMethod == null)
            {
                throw new NotSupportedException();
            }

            renderMethod.Invoke(this, new object[] { obj });
        }

        public virtual void RenderJoin(ISqlTable leftTable, SqlJoinType joinType, ISqlTable rightTable, IEnumerable<ISqlFilter> conditions, SqlBuildArguments args)
        {
            this.WriteSpace();
            this.Write(joinType.ToString().ToUpper());
            this.WriteSpace();
            this.Write("JOIN");
            this.WriteSpace();

            //TODO: This is causing joined tables to render their conditions here, which is not right.
            rightTable.Render(this);

            if (joinType != SqlJoinType.Cross)
            {
                this.WriteSpace();
                this.Write(SqlConstants.ON);
                this.WriteSpace();
                this.RenderAll<ISqlFilter>(conditions, string.Concat(SqlConstants.SPACE, ConvertSqlLogicToString(SqlLogic.And), SqlConstants.SPACE));
            }
            this.WriteNewLine();
        }

        public virtual void RenderJoins(IEnumerable<ISqlJoin> joins, SqlBuildArguments args)
        {
            this.RenderAll(joins, string.Empty);
        }

        #endregion

        #region T-SQL Specific Write Methods
               
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

        public virtual void WriteExpression(string expression, IList<object> arguments)
        {
            if (arguments != null)
            {
                var resolvedArguments = arguments.Select(arg =>
                {
                    if (arg is ISqlTable table)
                    {
                        return this.FormatQualifiedIdentifierName(BuildArguments.TableName(table));
                    }
                    else if (arg is System.Data.Common.DbParameter dbParam)
                    {
                        return string.Concat(this.ParameterNamePrefix, BuildArguments.CreateParameter(dbParam.Value, dbParam.DbType));
                    }
                    else if (arg is DbParameterValue paramValue)
                    {
                        return string.Concat(this.ParameterNamePrefix, BuildArguments.CreateParameter(paramValue.Value, paramValue.DbType, paramValue.Size));
                    }
                    else
                    {
                        return string.Concat(this.ParameterNamePrefix, BuildArguments.CreateParameter(arg.ToString(), System.Data.DbType.String));
                    }
                }).ToArray();

                this.Write(string.Format(expression, resolvedArguments));
            }
            else
            {
                this.Write(expression);
            }            
        }

        public virtual void WriteOffsetLimit(SqlPagingOptions options)
        {
            //https://docs.microsoft.com/en-us/sql/t-sql/queries/select-order-by-clause-transact-sql?view=sql-server-2017#using-offset-and-fetch-to-limit-the-rows-returned
            //e.g. OFFSET 10 ROWS FETCH NEXT 10 ROWS ONLY

            if (options.Offset > 0 || options.Limit > 0)
            {
                this.Write($" OFFSET {options.Offset} ROWS");
            }

            if (options.Limit > 0)
            {
                this.Write($" FETCH NEXT {options.Limit} ROWS ONLY");
            }
        }

        #endregion


        // -----------------------------------------------

        public virtual void Render(SqlColumn src)
        {
            if (src.Aggregate == SqlAggregate.None)
            {
                this.WriteColumnName(src.ColumnName, BuildArguments.TableName(src.Table), src.Alias);
            }
            else
            {
                this.WriteAggregateColumn(src.ColumnName, BuildArguments.TableName(src.Table), src.Aggregate, src.Alias);
            }
        }

        public virtual void Render(SqlColumnCompareFilter src)
        {
            this.WriteBeginGroup();
            this.WriteColumnName(src.LeftColumnName, BuildArguments.TableName(src.LeftTable));
            this.WriteOperator(src.Operator);
            this.WriteColumnName(src.RightColumnName, BuildArguments.TableName(src.RightTable));
            this.WriteEndGroup();
        }

        public virtual void Render(SqlCompareFilter src)
        {
            this.WriteBeginGroup();
            this.WriteColumnName(src.ColumnName, BuildArguments.TableName(src.Table));
            this.WriteOperator(src.Operator);

            if (src.EncodeValueAsLiteral)
            {
                this.WriteLiteralValue(src.Value);
            }
            else
            {
                this.WriteParameter(BuildArguments.CreateParameter(src.Value, src.DataType));
            }

            this.WriteEndGroup();
        }

        public virtual void Render(SqlCountFilter src)
        {
            BuildArguments.AssignAlias(src.SubQueryTable);

            var subquery = new SqlSelectBuilder(src.SubQueryTable, this.Provider);
            var subQueryColumn = new SqlColumn(src.SubQueryTable, src.SubQueryColumn);
            subQueryColumn.Aggregate = SqlAggregate.Count;
            subQueryColumn.Alias = "Cnt";
            subquery.SelectColumns.Add(subQueryColumn);

            foreach (var filter in src.SubQueryFilters)
            {
                subquery.Filters.Add(filter);
            }

            this.WriteBeginGroup();

            this.Render(subquery, wrapped: true);
            this.WriteSpace();
            this.WriteOperator(src.CountOperator);
            this.WriteSpace();
            this.WriteParameter(BuildArguments.CreateParameter(src.CountValue, System.Data.DbType.Int32));

            this.WriteEndGroup();
        }

        public virtual void Render(SqlDateFilter src)
        {
            this.WriteBeginGroup();
            this.WriteColumnName(src.ColumnName, BuildArguments.TableName(src.Table));
            this.WriteOperator(SqlOperator.GreaterThanOrEqual);
            this.WriteParameter(BuildArguments.CreateParameter(src.BeginDate.Date, System.Data.DbType.DateTime));
            this.WriteSpace();
            this.Write(SqlConstants.AND);
            this.WriteSpace();
            this.WriteColumnName(src.ColumnName, BuildArguments.TableName(src.Table));
            this.WriteOperator(SqlOperator.LessThanOrEqual);
            //TODO: Get data type from object
            this.WriteParameter(BuildArguments.CreateParameter(src.EndDate.Date, System.Data.DbType.DateTime));
            this.WriteEndGroup();
        }

        public virtual void Render(SqlDateTimeFilter src)
        {
            this.WriteBeginGroup();
            if (src is SqlDateFilter)
            {
                this.WriteCast("date", () =>
                {
                    this.WriteColumnName(src.ColumnName, BuildArguments.TableName(src.Table));
                });
            }
            else
            {
                this.WriteColumnName(src.ColumnName, BuildArguments.TableName(src.Table));
            }

            this.WriteOperator(SqlOperator.GreaterThanOrEqual);
            this.WriteParameter(BuildArguments.CreateParameter(src.BeginDate, System.Data.DbType.DateTime));
            this.WriteSpace();
            this.Write(SqlConstants.AND);
            this.WriteSpace();

            if (src is SqlDateFilter)
            {
                this.WriteCast("date", () =>
                {
                    this.WriteColumnName(src.ColumnName, BuildArguments.TableName(src.Table));
                });
            }
            else
            {
                this.WriteColumnName(src.ColumnName, BuildArguments.TableName(src.Table));
            }

            this.WriteOperator(SqlOperator.LessThanOrEqual);
            //TODO: Get data type from object
            this.WriteParameter(BuildArguments.CreateParameter(src.EndDate, System.Data.DbType.DateTime));
            this.WriteEndGroup();
        }

        public virtual void Render(SqlExistFilter src)
        {
            // (EXISTS ( <src.Statement> ))
            this.WriteBeginGroup();
            this.Write("EXISTS ");
            this.WriteBeginGroup();
            src.Statement.Render(this);
            this.WriteEndGroup();
            this.WriteEndGroup();
        }

        public virtual void Render(SqlRawColumn src)
        {
            // <src.Expression>  AS <alias>
            this.RenderValue(src);

            if (src.Alias != null)
            {
                this.WriteSpace();
                this.Write(SqlConstants.AS);
                this.WriteSpace();
                this.WriteColumnName(src.Alias);
            }
        }

        public virtual void Render(SqlFilterCollection src)
        {
            if (src.Count <= 0)
                return;

            if (src.Count > 1)
            {
                this.WriteBeginGroup();
            }

            this.RenderAll(src, string.Concat(" ", ((src.Logic == SqlLogic.And) ? SqlConstants.AND : SqlConstants.OR).ToString().ToUpper(), " "));

            if (src.Count > 1)
            {
                this.WriteEndGroup();
            }
        }

        public virtual void Render(SqlJoin src)
        {
            this.WriteSpace();
            this.Write(src.JoinType.ToString().ToUpper());
            this.WriteSpace();
            this.Write("JOIN");
            this.WriteSpace();

            //TODO: This is causing joined tables to render their conditions here, which is not right.
            src.RightTable.Render(this);

            if (src.JoinType != SqlJoinType.Cross)
            {
                this.WriteSpace();
                this.Write(SqlConstants.ON);
                this.WriteSpace();
                this.RenderAll<ISqlFilter>(src.Conditions, string.Concat(SqlConstants.SPACE, ConvertSqlLogicToString(SqlLogic.And), SqlConstants.SPACE));
            }

            this.WriteNewLine();
        }

        public virtual void Render(SqlListFilter src)
        {
            //TODO: make this impl agnostic
            bool first = true;

            if (src.Values == null)
            {
                throw new InvalidOperationException(string.Format(ErrorMessage.SqlListFilter_CollectionIsEmpty, src.ColumnName));
            }

            this.WriteBeginGroup();
            this.WriteColumnName(src.ColumnName, BuildArguments.TableName(src.Table));
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
                        this.WriteParameter(BuildArguments.CreateParameter(v.ToString(), src.DataType, src.Size)); break;
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
                            this.WriteParameter(BuildArguments.CreateParameter(v, src.DataType, src.Size));
                        }
                        break;
                    default:
                        this.WriteParameter(BuildArguments.CreateParameter(v, src.DataType, src.Size)); break;
                }
            }

            if (first)
            {
                throw new InvalidOperationException(string.Format(ErrorMessage.SqlListFilter_CollectionIsEmpty, src.ColumnName));
            }

            this.WriteEndGroup();
            this.WriteEndGroup();
        }

        public virtual void Render<T>(SqlLiteralColumn<T> src)
        {
            this.RenderValue(src);
            if (src.Alias != null)
            {
                this.WriteSpace();
                this.Write(SqlConstants.AS);
                this.WriteSpace();
                this.WriteColumnName(src.Alias);
            }
        }

        public virtual void Render(SqlNullFilter src)
        {
            this.WriteBeginGroup();
            this.WriteColumnName(src.ColumnName, BuildArguments.TableName(src.Table));
            this.Write((src.IsNull) ? " IS NULL" : " IS NOT NULL");
            this.WriteEndGroup();
        }

        public virtual void Render(SqlOrderColumn src)
        {
            this.WriteSortColumn(src.ColumnName, src.SortDirection);
        }

        public virtual void Render(SqlParameterFilter src)
        {
            this.WriteBeginGroup();
            this.WriteColumnName(src.ColumnName, BuildArguments.TableName(src.Table));
            this.WriteOperator(src.Operator);
            this.Write(src.ParameterName);
            this.WriteEndGroup();
        }

        public virtual void Render(SqlRankColumn src)
        {
            this.WriteRankOver(src.ColumnName, BuildArguments.TableName(src.Table), src.Aggregate, src.Alias, src.RankDescending);
        }

        public virtual void Render(SqlRawFilter src)
        {
            var resolvedArguments = src.Arguments.Select(arg =>
            {
                if (arg is ISqlTable table)
                {
                    return this.FormatQualifiedIdentifierName(BuildArguments.TableName(table));
                }
                else if (arg is System.Data.Common.DbParameter dbParam)
                {
                    return string.Concat("@", BuildArguments.CreateParameter(dbParam.Value, dbParam.DbType));
                }
                else if (arg is DbParameterValue paramValue)
                {
                    return string.Concat("@", BuildArguments.CreateParameter(paramValue.Value, paramValue.DbType, paramValue.Size));
                }
                else
                {
                    return string.Concat("@", BuildArguments.CreateParameter(arg.ToString(), System.Data.DbType.String));
                }
            }).ToArray();

            this.WriteBeginGroup();
            this.Write(string.Format(src.SqlText, resolvedArguments));
            this.WriteEndGroup();
        }

        public virtual void Render(SqlStringMatchFilter src)
        {
            string s = src.Value;
            if (s == null)
            {
                s = string.Empty;
            }

            this.WriteBeginGroup();
            this.WriteColumnName(src.ColumnName, BuildArguments.TableName(src.Table));
            this.WriteSpace();
            this.Write(SqlConstants.LIKE);
            this.WriteSpace();
            this.WriteParameter(BuildArguments.CreateParameter(SqlStringMatchFilter.DecorateValue(src.Value, src.Operator), src.DataType));
            this.WriteEndGroup();
        }

        public virtual void Render(SqlSubQueryFilter src)
        {
            this.WriteBeginGroup();
            this.WriteColumnName(src.ColumnName, BuildArguments.TableName(src.Table));
            this.WriteSpace();

            if (src.Condition == SubQueryMode.NotInList)
            {
                this.Write(SqlConstants.NOT);
                this.WriteSpace();
            }

            this.Write(SqlConstants.IN);

            this.WriteSpace();

            BuildArguments.AssignAlias(src.SubQueryTable);

            var builder = new SqlSelectBuilder(src.SubQueryTable, this.Provider);
            var subQueryColumn = new SqlColumn(src.SubQueryTable, src.SubQueryColumn);

            builder.SelectColumns.Add(subQueryColumn);

            foreach (var filter in src.SubQueryFilters)
            {
                builder.Filters.Add(filter);
            }
            
            this.Render(builder, wrapped: true);

            this.WriteEndGroup();
        }

        public virtual void Render(SqlTable src)
        {
            this.WriteTableName(src.TableName, BuildArguments.TableName(src));
        }

        public virtual void Render(SqlDerivedTable src)
        {
            this.WriteBeginGroup();
            // This needs to do something better than this. We need to remove all ending whitespace characters in reverse
            // until we hit the first non-whitespace char or non-semicolon
            this.Write(src.CommandText.TrimEnd(new char[] { ';' }));
            this.WriteEndGroup();
            this.WriteSpace();
            this.Write(SqlConstants.AS);
            this.WriteSpace();
            this.WriteTableName(BuildArguments.TableName(src));
        }

        public virtual void RenderValue(SqlColumn src)
        {
            if (src.Aggregate == SqlAggregate.None)
            {
                this.WriteColumnName(src.ColumnName, BuildArguments.TableName(src.Table));
            }
            else
            {
                this.WriteAggregate(src.ColumnName, BuildArguments.TableName(src.Table), src.Aggregate);
            }
        }

        public virtual void Render(SqlSelectBuilder selectBuilder, bool wrapped = false, bool aliased = false)
        {
            if (aliased)
            {
                ((ISqlTable)selectBuilder).Compile(BuildArguments);
            }
            else
            {
                selectBuilder.CompileInternal(BuildArguments);
            }
                                    
            // build refs for all the referenced tables            

            if (wrapped)
            {
                this.WriteBeginGroup();
            }

            if (selectBuilder.Prefix != null)
            {
                this.Write(selectBuilder.Prefix);
                if (!wrapped)
                {
                    this.WriteEndStatement();
                }
            }

            // SELECT
            this.RenderSelect(selectBuilder.SelectColumns, BuildArguments, selectBuilder.SelectDistinct);

            // FROM
            this.RenderFrom(selectBuilder.Table, BuildArguments);

            // JOINS
            if (selectBuilder.Joins.Count > 0)
            {
                this.RenderJoins(selectBuilder.Joins, BuildArguments);
            }

            // WHERE
            this.RenderWhere(selectBuilder.Filters, SqlLogic.And, BuildArguments);

            // GROUP BY
            if (selectBuilder.SelectColumns.Count(x => x.IsAggregate) > 0)
            {
                this.RenderGroupBy(selectBuilder.SelectColumns.Where(x => !x.IsAggregate), BuildArguments);
            }

            // ORDER BY
            this.RenderOrderBy(selectBuilder.OrderBy, BuildArguments);

            // OFFSET / LIMIT
            if (selectBuilder.PagingOptions.HasValue)
            {
                this.WriteOffsetLimit(selectBuilder.PagingOptions.Value);
            }

            if (selectBuilder.Suffix != null)
            {
                if (!wrapped)
                {
                    this.WriteEndStatement();
                }
                this.Write(selectBuilder.Suffix);
            }

            // if we are using this as a subquery, or in a join, we need to wrap/alias it.
            if (wrapped)
            {
                this.WriteEndGroup();

                if (aliased)
                {
                    this.WriteSpace();
                    this.Write(SqlConstants.AS);
                    this.WriteSpace();
                    this.WriteTableName(BuildArguments.TableName(selectBuilder));
                }
            }
        }

        public virtual void RenderValue(SqlRawColumn src)
        {
            this.WriteExpression(src.Value, src.Arguments);
        }

        public virtual void RenderValue<T>(SqlLiteralColumn<T> src)
        {
            this.WriteLiteralValue(src.Value);
        }

        #endregion

    }


}
