using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;

namespace Csg.Data.Sql
{
    /// <summary>
    /// Provides methods for parsing, testing, and executing an SQL SELECT statement
    /// </summary>
    public class SqlSelectBuilder : ISqlTable
    {
        private const string TSQL_FMT_ONLY_ON = "SET FMTONLY ON; ";
        private const string TSQL_FMT_ONLY_OFF = "SET FMTONLY OFF; ";
        private const string TSQL_ORDER_BY = "ORDER BY";

        private void ParseInternal(string commandText)
        {
            //TODO: check sortExpression for SQL injection            
            string orderBy;
            int i = commandText.IndexOf(TSQL_ORDER_BY, StringComparison.OrdinalIgnoreCase);
                        
            if (string.IsNullOrEmpty(commandText))
            {
                throw util.InvalidOperationException(ErrorMessage.GenericValueCannotBeEmptyOrNull, "commandText");
            }

            commandText = commandText.Trim();

            if (commandText.EndsWith(";"))
            {
                commandText = commandText.Substring(0, commandText.Length - 1);
            }

            if (i >= 0)
            {
                this.Table = SqlTableBase.Create(commandText.Substring(0, i));
                orderBy = commandText.Substring(i + TSQL_ORDER_BY.Length + 1);
                this.OrderBy.Add(orderBy);
            }
            else
            {
                this.Table = SqlTableBase.Create(commandText);
                orderBy = null;
            }
        }

        internal SqlSelectBuilder(ISqlTable table, IEnumerable<ISqlJoin> joins, IList<ISqlColumn> columns, IEnumerable<ISqlFilter> filters, IList<SqlOrderColumn> orderBy) : this()
        {
            this.Table = table;
            _joins = joins.ToList();
            _columns = columns;
            _filters = filters.ToList();
            _orderBy = orderBy;
        }

        public SqlSelectBuilder()
        {
            this.GenerateFormattedSql = false;
        }

        public SqlSelectBuilder(string commandText) : this()
        {
            this.ParseInternal(commandText);
        }

        public SqlSelectBuilder(ISqlTable table) : this()
        {
            this.Table = table;
        }

        /// <summary>
        /// Gets a value that indicates if the output SQL text should have line breaks and other formatting.
        /// </summary>
        public bool GenerateFormattedSql { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates if only distinct values should be returned from the query.
        /// </summary>
        public bool SelectDistinct { get; set; }
        
        /// <summary>
        /// Gets or sets the primary table used in the query. This will be used as the first table rendered directly after the FROM keyword.
        /// </summary>
        public ISqlTable Table { get; set; }

        /// <summary>
        /// Gets a collection of table joins used to join other tables into the resulting query.
        /// </summary>
        public IList<ISqlJoin> Joins
        {
            get
            {
                if (_joins == null)
                    _joins = new List<ISqlJoin>();
                return _joins;
            }
        }
        private IList<ISqlJoin> _joins;

        /// <summary>
        /// Gets a collection of columns to be used in the query.
        /// </summary>
        public IList<ISqlColumn> Columns
        {
            get
            {
                if (_columns == null)
                    _columns = new List<ISqlColumn>();
                return _columns;
            }
        }
        private IList<ISqlColumn> _columns;

        /// <summary>
        /// Gets a collection of <see cref="SqlOrderColumn"/> which control how the ORDER BY keyword is rendered, if at all.
        /// </summary>
        public IList<SqlOrderColumn> OrderBy
        {
            get
            {
                if (_orderBy == null)
                    _orderBy = new List<SqlOrderColumn>();
                return _orderBy;
            }
        }
        private IList<SqlOrderColumn> _orderBy;

        /// <summary>
        /// Gets a collection of filter objects that will follow the WHERE keyword.
        /// </summary>
        public IList<ISqlFilter> Filters
        {
            get
            {
                if (_filters == null)
                    _filters = new List<ISqlFilter>();
                return _filters;
            }
        }
        private IList<ISqlFilter> _filters;

        /// <summary>
        /// 
        /// </summary>
        public ICollection<SqlOptionBase> Options
        {
            get
            {
                if (_options == null)
                {
                    _options = new List<SqlOptionBase>();
                }
                return _options;
            }
            set
            {
                _options = value;
            }
        }
        private ICollection<SqlOptionBase> _options;

        /// <summary>
        /// Renders the query.
        /// </summary>
        /// <returns></returns>
        public SqlStatement Render()
        {
            return this.Render(false);
        }

        /// <summary>
        /// Renders the query.
        /// </summary>
        /// <param name="supressEndStatement">True if you want to supress statement terminating characters (semicolon)</param>
        /// <returns></returns>
        public SqlStatement Render(bool supressEndStatement)
        {
            var writer = new SqlTextWriter() { Format = this.GenerateFormattedSql };
            var args = new SqlBuildArguments();

            this.Render(writer, args);

            if (!supressEndStatement)
            {
                writer.WriteEndStatement();
            }

            return new SqlStatement(writer.ToString(), args.Parameters);
        }

        /// <summary>
        /// Renders the query.
        /// </summary>
        /// <returns></returns>
        public void Render(SqlTextWriter writer, SqlBuildArguments args)
        {
            this.CompileInternal(args);
            this.RenderInternal(writer, args);
        }

        /// <summary>
        /// Renders the query with T-SQL 'format only' decorators. This allows the query to be executed to validate the query and inspect the resulting schema, but will not return any data.
        /// </summary>
        /// <returns></returns>
        public SqlStatement RenderFormatOnly()
        {
            SqlStatement s = this.Render();

            s.CommandText = string.Concat(new string[] { TSQL_FMT_ONLY_ON, ";", s.CommandText, TSQL_FMT_ONLY_OFF, ";" });

            return s;
        }
        
        /// <summary>
        /// Compiles the tables used in the query into the given build arguments object.
        /// </summary>
        /// <param name="args"></param>
        protected void CompileInternal(SqlBuildArguments args)
        {
            this.Table.Compile(args);
            if (this.Joins.Count > 0)
            {
                foreach (var join in this.Joins)
                {
                    join.JoinedTable.Compile(args);
                }
            }
        }

        /// <summary>
        /// Renders the query to the given text writer.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="args"></param>
        protected void RenderInternal(SqlTextWriter writer, SqlBuildArguments args)
        {
            writer.RenderSelect(this.Columns, args, this.SelectDistinct);
            writer.RenderFrom(this.Table, args);
            if (this.Joins.Count > 0)
            {
                writer.RenderJoins(this.Joins, args);
            }
            writer.RenderWhere(this.Filters, SqlLogic.And, args);

            if (this.Columns.Count(x => x.IsAggregate) > 0)
            {
                writer.RenderGroupBy(this.Columns.Where(x => !x.IsAggregate), args);
            }

            writer.RenderOrderBy(this.OrderBy, args);

            if (_options != null && _options.Count > 0)
            {
                writer.RenderOptions(_options, args);
            }
        }
                
        void ISqlTable.Compile(SqlBuildArguments args)
        {
            args.AssignAlias(this);
            this.CompileInternal(args);
        }

        void ISqlStatementElement.Render(SqlTextWriter writer, SqlBuildArguments args)
        {
            writer.WriteBeginGroup();
            this.RenderInternal(writer, args);
            writer.WriteEndGroup();
            writer.WriteSpace();
            writer.Write(SqlConstants.AS);
            writer.WriteSpace();
            writer.Write(SqlDataColumn.Format(args.TableName(this)));
        }
    }
}
