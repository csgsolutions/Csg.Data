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
        /// When implemented in a derived class, gets a collection of SELECT columns.
        /// </summary>
        IList<ISqlColumn> SelectColumns { get; }

        /// <summary>
        /// When implemented in a derived class, gets or sets a value that indicates if SELECT DISTINCT should be used.
        /// </summary>   
        bool SelectDistinct { get; set; }

        /// <summary>
        /// When implemented in a derived class, adds a JOIN to the FROM clause of the query.
        /// </summary>
        void AddJoin(ISqlJoin join);

        /// <summary>
        /// When implemented in a derived class, adds a WHERE clause to the query.
        /// </summary>
        void AddFilter(ISqlFilter filter);

        /// <summary>
        /// When implemented in a derived class, gets the list of columns to order by.
        /// </summary>
        IList<SqlOrderColumn> OrderBy { get; }

        /// <summary>
        /// When implemented in a derived class, gets the underlying database connection.
        /// </summary>
        System.Data.IDbConnection Connection { get; }

        /// <summary>
        /// When implemented in a derived class, gets the underlying database transaction.
        /// </summary>
        System.Data.IDbTransaction Transaction { get; }

        /// <summary>
        /// When implemented in a derived class, gets a collection of parameters.
        /// </summary>
        ICollection<DbParameterValue> Parameters { get; }

        /// <summary>
        /// When implemented in a derived class, gets or sets the command timeout.
        /// </summary>
        int CommandTimeout { get; set; }

        /// <summary>
        /// When implemented in a derived class, returns an initalized database command.
        /// </summary>
        /// <returns></returns>
        IDbCommand CreateCommand();

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
    }
}
