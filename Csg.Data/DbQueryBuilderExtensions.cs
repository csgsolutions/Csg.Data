using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csg.Data.Sql;
using System.Data;
using System.Collections.ObjectModel;
using System.Data.Common;

namespace Csg.Data
{
    /// <summary>
    /// Provides extension methods for the <see cref="IDbQueryBuilder"/>.
    /// </summary>
    public static partial class DbQueryBuilder2Extensions
    {

        /// <summary>
        /// Populates the field select list for the resulting SELECT statement with the given field name(s).
        /// </summary>
        /// <param name="query">The query builder instance.</param>
        /// <param name="fields">A set of field names to select.</param>
        /// <returns></returns>
        public static IDbQueryBuilder Select(this IDbQueryBuilder query, params string[] fields)
        {
            return query.Select((IEnumerable<string>)fields);
        }

        /// <summary>
        /// Populates the field select list for the resulting SELECT statement with the given field names.
        /// </summary>
        /// <param name="query">The query builder instance.</param>
        /// <param name="fields">A set of field names to select.</param>
        /// <returns></returns>
        public static IDbQueryBuilder Select(this IDbQueryBuilder query, IEnumerable<string> fields)
        {
            var fork = query.Fork();

            foreach (var field in fields)
            {
                fork.Configuration.SelectColumns.Add(SqlColumn.Parse(fork.Root, field));
            }

            return fork;
        }

        /// <summary>
        /// Sets the resulting query to use SELECT DISTINCT.
        /// </summary>
        /// <param name="query">The query builder instance.</param>
        /// <returns></returns>
        public static IDbQueryBuilder Distinct(this IDbQueryBuilder query)
        {
            var fork = query.Fork();
            fork.Configuration.SelectDistinct = true;
            return fork;
        }

        /// <summary>
        /// Adds a set of WHERE clause conditions defined by the given expression, grouped by logical AND.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static IDbQueryBuilder Where(this IDbQueryBuilder query, Action<IDbQueryWhereClause> expression)
        {
            query = query.Fork();
            var group = new DbQueryWhereClause(query.Root, SqlLogic.And);

            expression.Invoke(group);
            group.ApplyToQuery(query);

            return query;
        }

        /// <summary>
        /// Adds a set of WHERE clause conditions defined by the given expression, grouped by logical OR.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static IDbQueryBuilder WhereAny(this IDbQueryBuilder query, Action<IDbQueryWhereClause> expression)
        {
            query = query.Fork();
            var group = new DbQueryWhereClause(query.Root, SqlLogic.Or);

            expression.Invoke(group);
            group.ApplyToQuery(query);

            return query;
        }

        /// <summary>
        /// Adds a set of WHERE clause conditions by looping over the given collection, and then joining them together with the given logic.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="collection"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static IDbQueryBuilder WhereAny<TItem>(this IDbQueryBuilder query, IEnumerable<TItem> collection, Action<IDbQueryWhereClause, TItem> expression)
        {
            query = query.Fork();
            var group = new DbQueryWhereClause(query.Root, SqlLogic.Or);

            foreach (var item in collection)
            {
                var innerGroup = new DbQueryWhereClause(query.Root, SqlLogic.And);
                expression.Invoke(innerGroup, item);
                group.AddFilter(innerGroup.Filters);
            }

            group.ApplyToQuery(query);

            return query;
        }

        /// <summary>
        /// Adds a set of WHERE clause conditions by looping over the given collection, and then joining them together with the given logic.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="collection"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static IDbQueryBuilder WhereAny<TItem>(this IDbQueryBuilder query, IList<TItem> list, Action<IDbQueryWhereClause, TItem, int> expression)
        {
            query = query.Fork();
            var group = new DbQueryWhereClause(query.Root, SqlLogic.Or);

            for (var i = 0; i < list.Count; i++)
            {
                var innerGroup = new DbQueryWhereClause(query.Root, SqlLogic.And);
                expression.Invoke(innerGroup, list[i], i);
                group.AddFilter(innerGroup.Filters);
            }

            group.ApplyToQuery(query);

            return query;
        }

        /// <summary>
        /// Adds ORDER BY fields to a query for ascending sort.
        /// </summary>
        /// <typeparam name="T">The type of the query builder.</typeparam>
        /// <param name="query">The query builder instance</param>
        /// <param name="fields">A set of field expressions to order the query by.</param>
        /// <returns></returns>
        public static IDbQueryBuilder OrderBy(this IDbQueryBuilder query, params string[] fields)
        {
            var fork = query.Fork();

            foreach (var field in fields)
            {
                fork.Configuration.OrderBy.Add(new Csg.Data.Sql.SqlOrderColumn() { ColumnName = field, SortDirection = Csg.Data.Sql.DbSortDirection.Ascending });
            }

            return fork;
        }

        /// <summary>
        /// Adds ORDER BY fields to a query for descending sort.
        /// </summary>
        /// <param name="query">The query builder instance</param>
        /// <param name="fields">A set of field expressions to order the query by.</param>
        /// <returns></returns>
        public static IDbQueryBuilder OrderByDescending(this IDbQueryBuilder query, params string[] fields)
        {
            var fork = query.Fork();
            foreach (var field in fields)
            {
                fork.Configuration.OrderBy.Add(new Csg.Data.Sql.SqlOrderColumn() { ColumnName = field, SortDirection = Csg.Data.Sql.DbSortDirection.Descending });
            }
            return fork;
        }

        /// <summary>
        /// Sets the execution command timeout for the query.
        /// </summary>
        /// <param name="query">The query builder instance</param>
        /// <param name="timeout">The timeout value in seconds.</param>
        /// <returns></returns>
        public static IDbQueryBuilder Timeout(this IDbQueryBuilder query, int timeout)
        {
            var fork = query.Fork();
            fork.Configuration.CommandTimeout = timeout;
            return fork;
        }

        /// <summary>
        /// Adds an arbitrary parameter to the resulting query.
        /// </summary>
        /// <param name="query">The query builder instance</param>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="value">The value of the parameter.</param>
        /// <param name="dbType">The data type of the parameter.</param>
        /// <param name="size">The size of the parameter.</param>
        /// <returns></returns>
        public static IDbQueryBuilder AddParameter(this IDbQueryBuilder query, string name, object value, DbType dbType, int? size = null)
        {
            var fork = query.Fork();

            fork.Configuration.Parameters.Add(new DbParameterValue()
            {
                ParameterName = name,
                DbType = dbType,
                Value = value,
                Size = size
            });

            return fork;
        }

        /// <summary>
        /// Adds an arbitrary parameter to the resulting query.
        /// </summary>
        /// <param name="query">The query builder instance</param>
        /// <param name="parameter">The parameter to add.</param>
        /// <returns></returns>
        public static IDbQueryBuilder AddParameter(this IDbQueryBuilder query, DbParameterValue parameter)
        {
            var fork = query.Fork();

            fork.Configuration.Parameters.Add(parameter);

            return fork;
        }

        /// <summary>
        /// Adds limit or offset conditions to the query.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="limit">The maximum number of rows to return.</param>
        /// <param name="offset">The zero-based index of the first row to return.</param>
        /// <returns></returns>
        public static IDbQueryBuilder Limit(this IDbQueryBuilder query, int limit = 0, int offset = 0)
        {
            if (query.Configuration.OrderBy.Count <= 0)
            {
                throw new InvalidOperationException(ErrorMessage.LimitOrOffsetWithoutOrderBy);
            }

            query = query.Fork();

            query.Configuration.PagingOptions = new SqlPagingOptions()
            {
                Limit = limit,
                Offset = offset
            };

            return query;
        }

        /// <summary>
        /// Sets a SQL statement that will be prefixed to the rendered query with a statement separater afterwards. This can be used to set query options.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static IDbQueryBuilder Prefix(this IDbQueryBuilder query, string prefix)
        {
            query = query.Fork();
            query.Configuration.Prefix = prefix;
            return query;
        }

        /// <summary>
        /// Sets a SQL statment that will be appended to the end of the rendered query after a statement separaeter (semicolon).
        /// </summary>
        /// <param name="query"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        public static IDbQueryBuilder Suffix(this IDbQueryBuilder query, string suffix)
        {
            query = query.Fork();
            query.Configuration.Suffix = suffix;
            return query;
        }

    }
}
