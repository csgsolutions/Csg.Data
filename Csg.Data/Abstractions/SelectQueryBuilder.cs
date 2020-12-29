using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Csg.Data.Sql;

namespace Csg.Data.Abstractions
{
    /// <summary>
    /// This class provides a base class for building query builders targeting a specific database type or feature set.
    /// </summary>
    [DebuggerDisplay("CommandText = {Render().CommandText}, Parameters = {ParameterString()}")]
    public abstract class SelectQueryBuilder : SqlSelectBuilder, ISelectQueryBuilder, ISelectQueryBuilderOptions
    {
        /// <summary>
        /// Gets or sets a value that indicates if the query builder will generate formatted SQL by default. Applies to all instances.
        /// </summary>
        public static bool DefaultGenerateFormattedSql = true;

        /// <summary>
        /// Creates a new instance using the given table expression and connection.
        /// </summary>
        /// <param name="sql">The name of a table, a table expression, or other object that can be the target of a SELECT query.</param>
        protected SelectQueryBuilder(string sql, Abstractions.ISqlProvider provider = null) : base(sql, provider ?? SqlProviderFactory.DefaultProvider)
        {
            this.GenerateFormattedSql = DefaultGenerateFormattedSql;
            this.Parameters = new List<DbParameterValue>();
        }

        /// <summary>
        /// Creates a new instance using the given table expression and connection.
        /// </summary>
        /// <param name="sql">The name of a table, a table expression, or other object that can be the target of a SELECT query.</param>
        protected SelectQueryBuilder(ISqlTable table, Abstractions.ISqlProvider provider = null) : base(table, provider ?? SqlProviderFactory.DefaultProvider)
        {
            this.GenerateFormattedSql = DefaultGenerateFormattedSql;
            this.Parameters = new List<DbParameterValue>();
        }

        /// <summary>
        /// Returns the root table of the query. This is the table listed immmediately after the FROM clause
        /// </summary>
        public virtual ISqlTable Root { get => this.Table; }

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

            return stmt;
        }

        /// <summary>
        /// Creates a new instance of <see cref="DbQueryBuilder"/> configured in the same manner as the existing one.
        /// </summary>
        /// <returns></returns>
        public abstract ISelectQueryBuilder Fork();

        /// <summary>
        /// Gets the command text returned from the <see cref="Render"/> method.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Render().CommandText;
        }

        #region Builder options

        int? ISelectQueryBuilderOptions.CommandTimeout { get => this.CommandTimeout; set => this.CommandTimeout = value; }

        bool ISelectQueryBuilderOptions.SelectDistinct { get => this.SelectDistinct; set => this.SelectDistinct = value; }

        ICollection<DbParameterValue> ISelectQueryBuilderOptions.Parameters => this.Parameters;

        ICollection<ISqlFilter> ISelectQueryBuilderOptions.Filters => this.Filters;

        ICollection<ISqlJoin> ISelectQueryBuilderOptions.Joins => this.Joins;

        IList<SqlOrderColumn> ISelectQueryBuilderOptions.OrderBy => this.OrderBy;

        IList<ISqlColumn> ISelectQueryBuilderOptions.SelectColumns => this.SelectColumns;

        #endregion

        #region querybuilder2

        SqlStatement ISelectQueryBuilder.Render()
        {
            return this.Render();
        }

        ISelectQueryBuilder ISelectQueryBuilder.Fork()
        {
            return this.Fork();
        }

        ISqlTable ISelectQueryBuilder.Root => this.Root;

        ISelectQueryBuilderOptions ISelectQueryBuilder.Configuration => this;

        #endregion                

        internal string ParameterString()
        {
            return string.Join(", ", this.Render().Parameters.Select(s => s.ParameterName + "=" +s.Value));
        }

        public override void CompileInternal(SqlBuildArguments args)
        {
            base.CompileInternal(args);
            args.Parameters.AddRange(this.Parameters);
        }
    }
}