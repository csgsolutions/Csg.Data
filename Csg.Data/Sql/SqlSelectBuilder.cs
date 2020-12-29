using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Csg.Data.Abstractions;

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
            int indexOfOrderBy = commandText.IndexOf(TSQL_ORDER_BY, StringComparison.OrdinalIgnoreCase);
                        
            if (string.IsNullOrEmpty(commandText))
            {
                throw util.InvalidOperationException(ErrorMessage.GenericValueCannotBeEmptyOrNull, "commandText");
            }

            //commandText = commandText.Trim();

            //if (commandText.EndsWith(";"))
            //{
            //    commandText = commandText.Substring(0, commandText.Length - 1);
            //}

            commandText = SqlDerivedTable.TrimCommandText(commandText);

            if (indexOfOrderBy >= 0)
            {
                this.Table = SqlTableBase.Create(commandText.Substring(0, indexOfOrderBy));
                this.OrderBy.Add(commandText.Substring(indexOfOrderBy + TSQL_ORDER_BY.Length + 1));
            }
            else
            {
                this.Table = SqlTableBase.Create(commandText);
            }
        }

        public SqlSelectBuilder(ISqlProvider provider)
        {
            this.GenerateFormattedSql = false;
            this.Provider = provider;
            this.SelectColumns = new List<ISqlColumn>();
            this.Joins = new List<ISqlJoin>();
            this.Filters = new List<ISqlFilter>();
            this.OrderBy = new List<SqlOrderColumn>();
        }

        public SqlSelectBuilder(string commandText, ISqlProvider provider) : this(provider)
        {
            this.ParseInternal(commandText);
        }

        public SqlSelectBuilder(ISqlTable table, ISqlProvider provider) : this(provider)
        {
            this.Table = table;
        }

        public static SqlSelectBuilder JoinTarget(string commandText, ISqlProvider provider)
        {
            return new SqlSelectBuilder(commandText, provider)
            {
                Wrapped = true,
                Aliased = true
            };
        }

        public static SqlSelectBuilder JoinTarget(ISqlTable table, ISqlProvider provider)
        {
            return new SqlSelectBuilder(table, provider)
            {
                Wrapped = true,
                Aliased = true
            };
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        [Obsolete("Used SqlSelectBuilder(ISqlProvider provider) instead.")]

        public SqlSelectBuilder() : this(SqlServer.SqlServerProvider.Instance)
        { 
        }

        /// <summary>
        /// Creates an instance using the given command text for the <see cref="Table"/>.
        /// </summary>
        /// <param name="commandText"></param>
        [Obsolete("Used SqlSelectBuilder(string commandText, ISqlProvider provider) instead.")]

        public SqlSelectBuilder(string commandText) : this(commandText, SqlServer.SqlServerProvider.Instance)
        {
        }

        public bool Wrapped { get; set; } = false;

        public bool Aliased { get; set; } = false;

        /// <summary>
        /// Gets a value that indicates if the output SQL text should have line breaks and other formatting.
        /// </summary>
        public virtual bool GenerateFormattedSql { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates if only distinct values should be returned from the query.
        /// </summary>
        public virtual bool SelectDistinct { get; set; }
        
        /// <summary>
        /// Gets or sets the primary table used in the query. This will be used as the first table rendered directly after the FROM keyword.
        /// </summary>
        public ISqlTable Table { get; set; }

        /// <summary>
        /// Gets a collection of table joins used to join other tables into the resulting query.
        /// </summary>
        public IList<ISqlJoin> Joins { get; set; } 

        /// <summary>
        /// Gets a collection of columns to be used in the query.
        /// </summary>
        public IList<ISqlColumn> SelectColumns { get; set; }

        /// <summary>
        /// Gets a collection of <see cref="SqlOrderColumn"/> which control how the ORDER BY keyword is rendered, if at all.
        /// </summary>
        public IList<SqlOrderColumn> OrderBy { get; set; }

        /// <summary>
        /// Gets a collection of filter objects that will follow the WHERE keyword.
        /// </summary>
        public IList<ISqlFilter> Filters { get; set; }

        /// <summary>
        /// Gets the sql text writer used when this query is rendered.
        /// </summary>
        public ISqlProvider Provider { get; set; }
       
        /// <summary>
        /// Gets the paging options that will be applied to the query.
        /// </summary>
        public SqlPagingOptions? PagingOptions { get; set; }

        /// <summary>
        /// Renders the query.
        /// </summary>
        /// <returns></returns>
        public SqlStatement Render()
        {
            return this.Render(false);
        }

        /// <summary>
        /// Gets or sets a SQL statement that will be prefixed to the rendered query with a statement separater afterwards. This can be used to set query options.
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// Gets or sets a SQL statment that will be appended to the end of the rendered query after a statement separaeter (semicolon).
        /// </summary>
        public string Suffix { get; set; }

        /// <summary>
        /// Renders the query.
        /// </summary>
        /// <param name="supressEndStatement">True if you want to supress statement terminating characters (semicolon)</param>
        /// <returns></returns>
        public SqlStatement Render(bool supressEndStatement)
        {
            var writer = this.Provider.CreateWriter();

            writer.Format = this.GenerateFormattedSql;

            writer.Render(this);

            if (!supressEndStatement)
            {
                writer.WriteEndStatement();
            }
            
            return new SqlStatement(writer.ToString(), writer.BuildArguments.Parameters);
        }
        
        /// <summary>
        /// Renders the query to the given text writer.
        /// </summary>
        /// <param name="writer"></param>
        public void Render(ISqlTextWriter writer)
        {
            writer.Render(this, wrapped: this.Wrapped, aliased: this.Aliased);
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
        public virtual void CompileInternal(SqlBuildArguments args)
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

        public void Compile(SqlBuildArguments args)
        {
            this.CompileInternal(args);
            args.AssignAlias(this);
        }

        void ISqlStatementElement.Render(ISqlTextWriter writer)
        {
            writer.Render(this, this.Wrapped, this.Aliased);
        }
    }
}
