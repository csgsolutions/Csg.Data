using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csg.Data.Sql;
using System.Data;
using System.Collections.ObjectModel;
using System.Data.Common;
using Csg.Data.Abstractions;

namespace Csg.Data
{
    /// <summary>
    /// Provides extension methods for the <see cref="ISelectQueryBuilder"/>.
    /// </summary>
    public static partial class SelectQueryBuilderExtensions
    {
        /// <summary>
        /// Populates the field select list for the resulting SELECT statement with the given field name(s).
        /// </summary>
        /// <param name="query">The query builder instance.</param>
        /// <param name="fields">A set of field names to select.</param>
        /// <returns></returns>
        public static T Select<T>(this T query, params string[] fields) where T: ISelectQueryBuilder
        {
            return query.Select<T>((IEnumerable<string>)fields);
        }

        /// <summary>
        /// Populates the field select list for the resulting SELECT statement with the given field names.
        /// </summary>
        /// <param name="query">The query builder instance.</param>
        /// <param name="fields">A set of field names to select.</param>
        /// <returns></returns>
        public static T Select<T>(this T query, IEnumerable<string> fields) where T : ISelectQueryBuilder
        {
            var fork = query.Fork();

            foreach (var field in fields)
            {
                fork.Configuration.SelectColumns.Add(SqlColumn.Parse(fork.Root, field));
            }

            return (T)fork;
        }

        /// <summary>
        /// Sets the resulting query to use SELECT DISTINCT.
        /// </summary>
        /// <param name="query">The query builder instance.</param>
        /// <returns></returns>
        public static T Distinct<T>(this T query) where T : ISelectQueryBuilder
        {
            var fork = query.Fork();

            fork.Configuration.SelectDistinct = true;

            return (T)fork;
        }

        /// <summary>
        /// Adds a set of WHERE clause conditions defined by the given expression, grouped by logical AND.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static T Where<T>(this T query, Action<IWhereClause> expression) where T : ISelectQueryBuilder
        {
            query = (T)query.Fork();
            var group = new WhereClause(query.Root, SqlLogic.And);

            expression.Invoke(group);
            group.ApplyToQuery(query);

            return (T)query;
        }

        /// <summary>
        /// Adds a set of WHERE clause conditions defined by the given expression, grouped by logical OR.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static T WhereAny<T>(this T query, Action<IWhereClause> expression) where T : ISelectQueryBuilder
        {
            query = (T)query.Fork();
            var group = new WhereClause(query.Root, SqlLogic.Or);

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
        public static TQuery WhereAny<TQuery,TItem>(this TQuery query, IEnumerable<TItem> collection, Action<IWhereClause, TItem> expression) where TQuery : ISelectQueryBuilder
        {
            query = (TQuery)query.Fork();
            var group = new WhereClause(query.Root, SqlLogic.Or);

            foreach (var item in collection)
            {
                var innerGroup = new WhereClause(query.Root, SqlLogic.And);
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
        public static TQuery WhereAny<TQuery, TItem>(this TQuery query, IList<TItem> list, Action<IWhereClause, TItem, int> expression) where TQuery : ISelectQueryBuilder
        {
            query = (TQuery)query.Fork();
            var group = new WhereClause(query.Root, SqlLogic.Or);

            for (var i = 0; i < list.Count; i++)
            {
                var innerGroup = new WhereClause(query.Root, SqlLogic.And);
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
        public static T OrderBy<T>(this T query, params string[] fields) where T:ISelectQueryBuilder
        {
            var fork = query.Fork();

            foreach (var field in fields)
            {
                fork.Configuration.OrderBy.Add(new Csg.Data.Sql.SqlOrderColumn() { ColumnName = field, SortDirection = Csg.Data.Sql.DbSortDirection.Ascending });
            }

            return (T)fork;
        }

        /// <summary>
        /// Adds ORDER BY fields to a query for descending sort.
        /// </summary>
        /// <param name="query">The query builder instance</param>
        /// <param name="fields">A set of field expressions to order the query by.</param>
        /// <returns></returns>
        public static T OrderByDescending<T>(this T query, params string[] fields) where T:ISelectQueryBuilder
        {
            var fork = query.Fork();
            foreach (var field in fields)
            {
                fork.Configuration.OrderBy.Add(new Csg.Data.Sql.SqlOrderColumn() { ColumnName = field, SortDirection = Csg.Data.Sql.DbSortDirection.Descending });
            }
            return (T)fork;
        }

        /// <summary>
        /// Sets the execution command timeout for the query.
        /// </summary>
        /// <param name="query">The query builder instance</param>
        /// <param name="timeout">The timeout value in seconds.</param>
        /// <returns></returns>
        public static T Timeout<T>(this T query, int timeout) where T : ISelectQueryBuilder
        {
            var fork = query.Fork();
            fork.Configuration.CommandTimeout = timeout;
            return (T)fork;
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
        public static T AddParameter<T>(this T query, string name, object value, DbType dbType, int? size = null) where T : ISelectQueryBuilder
        {
            var fork = query.Fork();

            fork.Configuration.Parameters.Add(new DbParameterValue()
            {
                ParameterName = name,
                DbType = dbType,
                Value = value,
                Size = size
            });

            return (T)fork;
        }

        /// <summary>
        /// Adds an arbitrary parameter to the resulting query.
        /// </summary>
        /// <param name="query">The query builder instance</param>
        /// <param name="parameter">The parameter to add.</param>
        /// <returns></returns>
        public static T AddParameter<T>(this T query, DbParameterValue parameter) where T : ISelectQueryBuilder
        {
            var fork = query.Fork();

            fork.Configuration.Parameters.Add(parameter);

            return (T)fork;
        }

        /// <summary>
        /// Adds limit or offset conditions to the query.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="limit">The maximum number of rows to return.</param>
        /// <param name="offset">The zero-based index of the first row to return.</param>
        /// <returns></returns>
        public static T Limit<T>(this T query, int limit = 0, int offset = 0) where T : ISelectQueryBuilder
        {
            if (query.Configuration.OrderBy.Count <= 0)
            {
                throw new InvalidOperationException(ErrorMessage.LimitOrOffsetWithoutOrderBy);
            }

            query = (T)query.Fork();

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
        public static T Prefix<T>(this T query, string prefix) where T : ISelectQueryBuilder
        {
            query = (T)query.Fork();
            query.Configuration.Prefix = prefix;
            return query;
        }

        /// <summary>
        /// Sets a SQL statment that will be appended to the end of the rendered query after a statement separaeter (semicolon).
        /// </summary>
        /// <param name="query"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        public static T Suffix<T>(this T query, string suffix) where T : ISelectQueryBuilder
        {
            query = (T)query.Fork();
            query.Configuration.Suffix = suffix;
            return query;
        }

    }
}
