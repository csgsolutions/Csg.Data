using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csg.Data.Sql
{

    public class SqlTextWriter : System.IO.TextWriter
    {
        private System.IO.TextWriter InnerWriter;
        
        private const string COLUMN_SEPERATOR = ",";
        private const string JOIN_SEPERATOR = " ";
        private const string QUOTE = "\"";
        private const string QUOTEQUOTE = "\"\"";
        
        public SqlTextWriter() : base()
        {
            this.InnerWriter = new System.IO.StringWriter();
        }

        public SqlTextWriter(System.IO.TextWriter writer) : base()
        {
            this.InnerWriter = writer;
        }        

        public SqlTextWriter(StringBuilder sb) : base()
        {
            this.InnerWriter = new System.IO.StringWriter(sb);
        }

        /// <summary>
        /// Gets or sets a value that indicates if the writer should output "pretty" SQL that includes un-necessary line breaks and such.
        /// </summary>
        public bool Format { get; set; }

        /// <summary>
        /// Gets or sets a value that indiciates if the writer should write quoted Identifiers.
        /// </summary>
        public bool UseQuotedIdentifiers { get; set; }

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

        #endregion

        #region Standard Write Methods

        public override void Write(char value)
        {
            InnerWriter.Write(value);
        }

        public void WriteNewLine()
        {
            if (this.Format)
            {
                Write("\r\n");
            }
        }

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

        public void RenderSelect(IEnumerable<ISqlColumn> columns, SqlBuildArguments args)
        {
            this.RenderSelect(columns, args);
        }

        public void RenderSelect(IEnumerable<ISqlColumn> columns, SqlBuildArguments args, bool distinct)
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
                    this.RenderAll(items, args, string.Concat(COLUMN_SEPERATOR, "\r\n"));
                }
                else
                {
                    this.RenderAll(items, args, COLUMN_SEPERATOR);
                }
            }
            else
            {
                this.Write("*");
            }
            this.WriteNewLine();
        }

        public void WriteFrom()
        {
            this.WriteSpace();
            this.Write(SqlConstants.FROM);
            this.WriteSpace();
        }

        public void RenderFrom(ISqlTable table, SqlBuildArguments args)
        {
            if (table == null)
                throw new ArgumentNullException("table");
            if (args == null)
                throw new ArgumentNullException("args");
            
            this.WriteFrom();
            table.Render(this, args);
            this.WriteNewLine();
        }

        public void WriteWhere()
        {
            this.WriteSpace();
            this.Write(SqlConstants.WHERE);
            this.WriteSpace();
        }

        public void RenderWhere(IEnumerable<ISqlFilter> filters, SqlLogic logic, SqlBuildArguments args)
        {
            if (filters == null)
                throw new ArgumentNullException("filters");
            if (args == null)
                throw new ArgumentNullException("args");

            var items = filters.ToArray();
            if (items.Length > 0)
            {
                this.WriteWhere();
                this.RenderAll(items, args, string.Concat(SqlConstants.SPACE,ConvertSqlLogicToString(logic),SqlConstants.SPACE));
            }
        }

        public void WriteGroupBy()
        {
            this.WriteSpace();
            this.Write(SqlConstants.GROUP_BY);
            this.WriteSpace();
        }

        public void RenderGroupBy(IEnumerable<ISqlColumn> columns, SqlBuildArguments args)
        {
            if (columns == null)
                throw new ArgumentNullException("columns");
            if (args == null)
                throw new ArgumentNullException("args");

            var items = columns.ToArray();
            if (items.Length > 0)
            {
                this.WriteGroupBy();                
                this.RenderAll(items, args, COLUMN_SEPERATOR, (a, b, c) => { a.RenderValueExpression(b,c); });               
            }
            this.WriteNewLine();
        }

        public void WriteOrderBy()
        {
            this.WriteSpace();
            this.Write(SqlConstants.ORDER_BY);
            this.WriteSpace();
        }

        public void RenderOrderBy(IEnumerable<SqlOrderColumn> columns, SqlBuildArguments args)
        {
            if (columns == null)
                throw new ArgumentNullException("columns");
            if (args == null)
                throw new ArgumentNullException("args");

            var items = columns.ToArray();
            
            if (items.Length > 0)
            {
                this.WriteOrderBy();                
                this.RenderAll(items, args, COLUMN_SEPERATOR);
            }
            this.WriteNewLine();
        }

        public void RenderOffsetLimit(SqlPagingOptions options, SqlBuildArguments args)
        {
            //https://docs.microsoft.com/en-us/sql/t-sql/queries/select-order-by-clause-transact-sql?view=sql-server-2017#using-offset-and-fetch-to-limit-the-rows-returned
            //e.g. OFFSET 10 ROWS FETCH NEXT 10 ROWS ONLY

            if (options.Offset > 0)
            {
                this.Write($" OFFSET {options.Offset} ROWS");
            }

            if (options.Limit > 0)
            {
                this.Write($" FETCH NEXT {options.Limit} ROWS ONLY");
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

        public void WriteDerivedTable(string commandText, string alias)
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

        public virtual void RenderJoin(ISqlTable leftTable, SqlJoinType joinType, ISqlTable rightTable, IEnumerable<ISqlFilter> conditions, SqlBuildArguments args)
        {
            this.WriteSpace();
            this.Write(joinType.ToString().ToUpper());
            this.WriteSpace();
            this.Write("JOIN");
            this.WriteSpace();

            //TODO: This is causing joined tables to render their conditions here, which is not right.
            rightTable.Render(this, args);

            if (joinType != SqlJoinType.Cross)
            {
                this.WriteSpace();
                this.Write(SqlConstants.ON);
                this.WriteSpace();
                this.RenderAll<ISqlFilter>(conditions, args, string.Concat(SqlConstants.SPACE, ConvertSqlLogicToString(SqlLogic.And), SqlConstants.SPACE));
            }
            this.WriteNewLine();
        }

        public virtual void RenderJoins(IEnumerable<ISqlJoin> joins, SqlBuildArguments args)
        {
            this.RenderAll(joins, args, string.Empty);
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

        public virtual void WriteCast(string literal, string sqlDataTypeName)
        {
            Write(SqlConstants.CAST);
            WriteBeginGroup();
            Write(literal);
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

        public virtual void RenderAll<T>(IEnumerable<T> items, SqlBuildArguments args, string seperator) where T: ISqlStatementElement
        {
            this.RenderAll(items, args, seperator, (a, b, c) => { a.Render(b, c); });
        }

        public virtual void RenderAll<T>(IEnumerable<T> items, SqlBuildArguments args, string seperator, Action<T,SqlTextWriter, SqlBuildArguments> renderAction) where T : ISqlStatementElement
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
                renderAction(item, this, args);
            }
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

        #endregion

        public override Encoding Encoding
        {
            get { return this.InnerWriter.Encoding; }
        }

        public override string ToString()
        {
            return this.InnerWriter.ToString();
        }

    }


}
