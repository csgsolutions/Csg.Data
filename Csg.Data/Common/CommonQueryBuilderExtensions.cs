using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Csg.Data;
using Csg.Data.Sql;

namespace Csg.Data.Common
{
    public static class CommonQueryBuilderExtensions
    {
        /// <summary>
        /// Creates a <see cref="IDbQueryBuilder"/> associated with the given <see cref="IDbScope"/> which can be used to build and execute a SQL statement.
        /// </summary>
        /// <param name="scope">The connection with which the resulting <see cref="IDbQueryBuilder"/> will be associated with.</param>
        /// <param name="commandText">The table expression, table name, or object name to use as the target of the query.</param>
        /// <param name="provider">The SQL text provider to use for query generation. If not specified <see cref="Sql.SqlProviderFactory.GetProvider(Type)"/> will be used.</param>
        /// <returns></returns>
        public static IDbQueryBuilder QueryBuilder(this Csg.Data.IDbScope scope, string commandText, Abstractions.ISqlProvider provider = null)
        {
            var adapter = new DbCommandAdapter(scope.Connection, scope.Transaction);

            return new DbQueryBuilder(commandText, adapter, provider ?? SqlProviderFactory.GetProvider(scope.Connection));
        }

        /// <summary>
        /// Creates a <see cref="IDbQueryBuilder"/> associated with the given <see cref="IDbConnection"/> which can be used to build and execute a SQL statement.
        /// </summary>
        /// <param name="connection">The connection with which the resulting <see cref="IDbCommand"/> will be associated with.</param>
        /// <param name="commandText">The table expression, table name, or object name to use as the target of the query.</param>
        /// <param name="provider">The SQL text provider to use for query generation. If not specified <see cref="Sql.SqlProviderFactory.GetProvider(Type)"/> will be used.</param>
        /// <returns></returns>
        public static IDbQueryBuilder QueryBuilder(this System.Data.IDbConnection connection, string commandText, Abstractions.ISqlProvider provider = null)
        {
            var adapter = new DbCommandAdapter(connection);

            return new DbQueryBuilder(commandText, adapter, provider: provider ?? Sql.SqlProviderFactory.GetProvider(connection));
        }

        /// <summary>
        /// Creates a <see cref="IDbQueryBuilder"/> associated with the given <see cref="IDbTransaction"/> which can be used to build and execute a SQL SELECT statement.
        /// </summary>
        /// <param name="transaction">The transaction with which the resulting <see cref="IDbCommand"/> will be associated with.</param>
        /// <param name="commandText">The table expression, table name, or object name to use as the target of the query.</param>
        /// <param name="provider">The SQL text provider to use for query generation. If not specified <see cref="Sql.SqlProviderFactory.GetProvider(Type)"/> will be used.</param>
        /// <returns></returns>
        public static IDbQueryBuilder QueryBuilder(this System.Data.IDbTransaction transaction, string commandText, Abstractions.ISqlProvider provider = null)
        {
            return new DbQueryBuilder(commandText, new DbCommandAdapter(transaction.Connection, transaction), provider: provider ?? Sql.SqlProviderFactory.GetProvider(transaction.Connection));
        }

        /// <summary>
        /// Executes the query and returns an open data reader.
        /// </summary>
        /// <returns></returns>
        public static IDataReader ExecuteReader(this IDbQueryBuilder query)
        {
            return query.CommandAdapter.CreateCommand<IDbCommand>(query).ExecuteReader();
        }

        /// <summary>
        /// Executes the query and returns the first column from the first row in the result set.
        /// </summary>
        /// <param name="query">The query builder instance.</param>
        /// <returns></returns>
        public static object ExecuteScalar(this IDbQueryBuilder query)
        {
            return query.CommandAdapter.CreateCommand<IDbCommand>(query).ExecuteScalar();
        }

        /// <summary>
        /// Executes the query and returns the first column from the first row in the result set, casted to the given type.
        /// </summary>
        /// <param name="query">The query builder instance.</param>
        public static T ExecuteScalar<T>(this IDbQueryBuilder query) where T : struct
        {
            var value = query.ExecuteScalar();

            if (value is T)
            {
                return (T)value;
            }
            else
            {
                return default(T);
            }
        }

        /// <summary>
        /// Executes a T-SQL SELECT statement in format only mode (SET FMTONLY ON) which validates the query and returns the result metadata.
        /// </summary>
        /// <param name="query">The query builder instance.</param>
        /// <param name="schema">Outputs the query metadata.</param>
        /// <param name="errors">Outputs any validation or execution errors encountered.</param>
        /// <returns></returns>
        public static bool GetSchemaTable(this IDbQueryBuilder query, out DataTable schema, out ICollection<Exception> errors)
        {
            var cmd = query.CommandAdapter.CreateCommand<IDbCommand>(query);

            errors = null;
            schema = null;

            try
            {
                using (var reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly))
                {
                    if (reader.FieldCount <= 0)
                    {
                        return false;
                    }

                    schema = reader.GetSchemaTable();
                }

                return true;
            }
            catch (Exception ex)
            {
                errors = new List<Exception>();
                errors.Add(ex);
                return false;
            }
        }

        /// <summary>
        /// Executes a T-SQL SELECT statement in format only mode (SET FMTONLY ON) which validates the query and returns the result metadata.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="schema"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        public static bool GetColumnSchema(this IDbQueryBuilder query, out ReadOnlyCollection<DbColumn> schema, out ICollection<Exception> errors)
        {
            var cmd = query.CommandAdapter.CreateCommand<IDbCommand>(query);

            errors = null;
            schema = null;

            try
            {
                using (var reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly))
                {
                    if (reader.FieldCount <= 0)
                    {
                        return false;
                    }

                    if (reader is System.Data.Common.IDbColumnSchemaGenerator)
                    {
                        schema = ((System.Data.Common.IDbColumnSchemaGenerator)reader).GetColumnSchema();
                        return true;
                    }
                    else
                    {
                        throw new NotSupportedException("The interface System.Data.Common.IDbColumnSchemaGenerator is not supported by the underlying data provider.");
                    }
                }
            }
            catch (Exception ex)
            {
                errors = new List<Exception>();
                errors.Add(ex);
                return false;
            }
        }
    }
}
