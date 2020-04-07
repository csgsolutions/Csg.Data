using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Csg.Data.Sql;

namespace Csg.Data
{
    /// <summary>
    /// Provides a query command builder to create and execute a SELECT statement against a database.
    /// </summary>
    [DebuggerDisplay("CommandText = {Render().CommandText}, Parameters = {ParameterString()}")]
    public class DbQueryBuilder : SqlSelectBuilder, IDbQueryBuilder, IDbQueryBuilderOptions
    {
        /// <summary>
        /// Gets or sets a value that indicates if the query builder will generate formatted SQL by default. Applies to all instances.
        /// </summary>
        public static bool DefaultGenerateFormattedSql = true;

        /// <summary>
        /// Creates a new instance using the given table expression and connection.
        /// </summary>
        /// <param name="sql">The name of a table, a table expression, or other object that can be the target of a SELECT query.</param>
        /// <param name="commandAdapter">The database connection.</param>
        public DbQueryBuilder(string sql, Abstractions.IDbCommandAdapter commandAdapter, Abstractions.ISqlProvider provider = null) : base(sql, provider ?? SqlProviderFactory.DefaultProvider)
        {
            this.CommandAdapter = commandAdapter;
            this.GenerateFormattedSql = DefaultGenerateFormattedSql;
            this.Parameters = new List<DbParameterValue>();
            //TODO: Detect the sql connection type and create an appropriate text writer???
        }

        /// <summary>
        /// Creates a new instance using the given table expression and connection.
        /// </summary>
        /// <param name="sql">The name of a table, a table expression, or other object that can be the target of a SELECT query.</param>
        /// <param name="commandAdapter">The database connection.</param>
        public DbQueryBuilder(ISqlTable table, Abstractions.IDbCommandAdapter commandAdapter, Abstractions.ISqlProvider provider = null) : base(table, provider ?? SqlProviderFactory.DefaultProvider)
        {
            this.CommandAdapter = commandAdapter;
            this.GenerateFormattedSql = DefaultGenerateFormattedSql;
            this.Parameters = new List<DbParameterValue>();
            //TODO: Detect the sql connection type and create an appropriate text writer???
        }

        /// <summary>
        /// Returns the root table of the query. This is the table listed immmediately after the FROM clause
        /// </summary>
        public virtual ISqlTable Root { get => this.Table; }

        public virtual Abstractions.IDbCommandAdapter CommandAdapter { get; protected set; }

        /// <summary>
        /// Adds a JOIN to the FROM clause of the query.
        /// </summary>
        /// <param name="join"></param>
        public void AddJoin(ISqlJoin join)
        {
            this.Joins.Add(join);
        }

        /// <summary>
        /// Adds a WHERE clause to the query.
        /// </summary>
        /// <param name="filter"></param>
        public void AddFilter(ISqlFilter filter)
        {
            this.Filters.Add(filter);
        }

        /// <summary>
        /// Gets or sets the query execution timeout.
        /// </summary>
        public int? CommandTimeout { get; set; }

        /// <summary>
        /// Gets the paging options used with the query
        /// </summary>
        public SqlPagingOptions? PagingOptions { get; set; }

        /// <summary>
        /// Gets the parameter value collection.
        /// </summary>
        public ICollection<DbParameterValue> Parameters { get; protected set; }

        /// <summary>
        /// Gets a SQL statement for the given query.
        /// </summary>
        /// <param name="generateFormattedSql">Indicates if SQL should be indented, have new-line characters, etc.</param>
        /// <returns></returns>
        public SqlStatement Render(bool? generateFormattedSql = null)
        {
            var stmt = base.Render();

            foreach(var param in this.Parameters)
            {
                stmt.Parameters.Add(param);
            }

            return stmt;
        }

        /// <summary>
        /// Creates a new instance of <see cref="DbQueryBuilder"/> configured in the same manner as the existing one.
        /// </summary>
        /// <returns></returns>
        public DbQueryBuilder Fork()
        {
            var builder = new DbQueryBuilder(this.Table, this.CommandAdapter);
            builder.Joins.AddRange(this.Joins);
            builder.Filters.AddRange(this.Filters);
            builder.SelectColumns.AddRange(this.SelectColumns);
            builder.Parameters.AddRange(this.Parameters);
            builder.OrderBy.AddRange(this.OrderBy);
            builder.CommandTimeout = this.CommandTimeout;
            builder.SelectDistinct = this.SelectDistinct;
            builder.Provider = this.Provider;
            builder.GenerateFormattedSql = this.GenerateFormattedSql;
            builder.PagingOptions = this.PagingOptions;
            builder.Prefix = this.Prefix;
            builder.Suffix = this.Suffix;
            return builder;
        }

        /// <summary>
        /// Gets the command text returned from the <see cref="Render"/> method.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Render().CommandText;
        }



        #region Builder options

        int? IDbQueryBuilderOptions.CommandTimeout { get => this.CommandTimeout; set => this.CommandTimeout = value; }

        bool IDbQueryBuilderOptions.SelectDistinct { get => this.SelectDistinct; set => this.SelectDistinct = value; }

        //IDbTransaction IDbQueryBuilderOptions.Transaction { get => this.Transaction; set => _transaction = value; }

        //IDbConnection IDbQueryBuilderOptions.Connection => this.Connection;

        ICollection<DbParameterValue> IDbQueryBuilderOptions.Parameters => this.Parameters;

        ICollection<ISqlFilter> IDbQueryBuilderOptions.Filters => this.Filters;

        ICollection<ISqlJoin> IDbQueryBuilderOptions.Joins => this.Joins;

        IList<SqlOrderColumn> IDbQueryBuilderOptions.OrderBy => this.OrderBy;

        IList<ISqlColumn> IDbQueryBuilderOptions.SelectColumns => this.SelectColumns;

        #endregion

        #region querybuilder2

        SqlStatement IDbQueryBuilder.Render()
        {
            return this.Render();
        }

        IDbQueryBuilder IDbQueryBuilder.Fork()
        {
            return this.Fork();
        }

        ISqlTable IDbQueryBuilder.Root => this.Root;

        IDbQueryBuilderOptions IDbQueryBuilder.Configuration => this;

        #endregion
                

        internal string ParameterString()
        {
            return string.Join(", ", this.Render().Parameters.Select(s => s.ParameterName + "=" +s.Value));
        }
    }
}