using Csg.Data.Sql;
using System;
using System.Text;

namespace Csg.Data.Abstractions
{
    public interface ISelectQueryBuilder
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
        ISelectQueryBuilder Fork();

        /// <summary>
        /// When implemented in a derived class, gets the current builder configuration.
        /// </summary>
        ISelectQueryBuilderOptions Configuration { get; }
    }
}
