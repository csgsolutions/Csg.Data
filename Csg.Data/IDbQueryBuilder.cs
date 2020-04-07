using Csg.Data.Abstractions;
using Csg.Data.Sql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Csg.Data
{

    /// <summary>
    /// When implemented in a derived class, provides a query builder on an active connection and/or transaction for which to build and execute a SELECT statement.
    /// </summary>
    public interface IDbQueryBuilder
    {
        /// <summary>
        /// When implemented in a derived class, returns the root table of the query. This is the table listed immmediately after the FROM clause
        /// </summary>
        ISqlTable Root { get; }

        /// <summary>
        /// Gets a SQL statement for the given query.
        /// </summary>
        /// <returns></returns>
        SqlStatement Render();

        /// <summary>
        /// When implemented in a derived class, creates a copy of the query builder
        /// </summary>
        /// <returns></returns>
        IDbQueryBuilder Fork();

        /// <summary>
        /// When implemented in a derived class, gets the current builder configuration.
        /// </summary>
        IDbQueryBuilderOptions Configuration { get; }

        IQueryFeatureAdapter CommandAdapter { get; }
    }

    public interface IDbQueryBuilderOptions
    {
        int? CommandTimeout { get; set; }

        bool SelectDistinct { get; set; }

        ICollection<DbParameterValue> Parameters { get; }

        IList<ISqlColumn> SelectColumns { get; }

        ICollection<ISqlFilter> Filters { get; }

        ICollection<ISqlJoin> Joins { get; }

        IList<SqlOrderColumn> OrderBy { get; }

        SqlPagingOptions? PagingOptions { get; set; }

        string Prefix { get; set; }

        string Suffix { get; set; }
    }
}
