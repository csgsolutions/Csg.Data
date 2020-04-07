using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Csg.Data.Abstractions;
using Csg.Data.Common;
using Csg.Data.Sql;

namespace Csg.Data.Common
{
    /// <summary>
    /// Provides a query command builder to create and execute a SELECT statement against an ADO.NET (System.Data.Common) supported database.
    /// </summary>
    [DebuggerDisplay("CommandText = {Render().CommandText}, Parameters = {ParameterString()}")]
    public class DbQueryBuilder : SelectQueryBuilder, IDbQueryBuilder
    { 
        /// <summary>
        /// Creates a new instance using the given table expression and connection.
        /// </summary>
        /// <param name="sql">The name of a table, a table expression, or other object that can be the target of a SELECT query.</param>
        /// <param name="commandAdapter">The database connection.</param>
        public DbQueryBuilder(string sql, 
            IQueryFeatureAdapter commandAdapter, 
            ISqlProvider provider = null) 
            : base(sql, provider)
        {
            this.Features = commandAdapter;
        } 

        /// <summary>
        /// Creates a new instance using the given table expression and connection.
        /// </summary>
        /// <param name="sql">The name of a table, a table expression, or other object that can be the target of a SELECT query.</param>
        /// <param name="commandAdapter">The database connection.</param>
        public DbQueryBuilder(ISqlTable table,
            IQueryFeatureAdapter commandAdapter,
            ISqlProvider provider = null) : base(table, provider)
        {
            this.Features = commandAdapter;
        }
        
        /// <summary>
        /// Gets the feature provider for the query builder.
        /// </summary>
        public virtual IQueryFeatureAdapter Features { get; protected set; }

        /// <summary>
        /// Creates a new instance of <see cref="DbQueryBuilder"/> configured in the same manner as the existing one.
        /// </summary>
        /// <returns></returns>
        public override ISelectQueryBuilder Fork()
        {
            var builder = new DbQueryBuilder(this.Root, this.Features, this.Provider);
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
    }
}