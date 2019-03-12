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
    public static class DbQueryBuilderExtensions
    {
        /// <summary>
        /// Creates a <see cref="IDbQueryBuilder"/> associated with the given <see cref="IDbScope"/> which can be used to build and execute a SQL statement.
        /// </summary>
        /// <param name="scope">The connection with which the resulting <see cref="IDbQueryBuilder"/> will be associated with.</param>
        /// <param name="commandText">The table expression, table name, or object name to use as the target of the query.</param>
        /// <returns></returns>
        public static IDbQueryBuilder QueryBuilder(this Csg.Data.IDbScope scope, string commandText)
        {
            return new DbQueryBuilder(commandText, scope.Connection, scope.Transaction);
        }

        /// <summary>
        /// Creates a <see cref="IDbQueryBuilder"/> associated with the given <see cref="IDbConnection"/> which can be used to build and execute a SQL statement.
        /// </summary>
        /// <param name="connection">The connection with which the resulting <see cref="IDbCommand"/> will be associated with.</param>
        /// <param name="commandText">The table expression, table name, or object name to use as the target of the query.</param>
        /// <returns></returns>
        public static IDbQueryBuilder QueryBuilder(this System.Data.IDbConnection connection, string commandText)
        {
            return new DbQueryBuilder(commandText, connection);
        }

        /// <summary>
        /// Creates a <see cref="IDbQueryBuilder"/> associated with the given <see cref="IDbTransaction"/> which can be used to build and execute a SQL SELECT statement.
        /// </summary>
        /// <param name="transaction">The transaction with which the resulting <see cref="IDbCommand"/> will be associated with.</param>
        /// <param name="commandText">The table expression, table name, or object name to use as the target of the query.</param>
        /// <returns></returns>
        public static IDbQueryBuilder QueryBuilder(this System.Data.IDbTransaction transaction, string commandText)
        {
            return new DbQueryBuilder(commandText, transaction.Connection, transaction);
        }

        /// <summary>
        /// Executes the query and returns an open data reader.
        /// </summary>
        /// <returns></returns>
        public static IDataReader ExecuteReader(this IDbQueryBuilder query)
        {
            return query.CreateCommand().ExecuteReader();
        }

        /// <summary>
        /// Executes the query and returns the first column from the first row in the result set.
        /// </summary>
        /// <param name="query">The query builder instance.</param>
        /// <returns></returns>
        public static object ExecuteScalar(this IDbQueryBuilder query)
        {
            return query.CreateCommand().ExecuteScalar();
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
            var cmd = query.CreateCommand();

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
            var cmd = query.CreateCommand();

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
                fork.SelectColumns.Add(SqlColumn.Parse(fork.Root, field));
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
            fork.Distinct = true;
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
            var group = new DbQueryWhereClause(query.Fork(), SqlLogic.And);

            expression.Invoke(group);

            return group.Complete();
        }

        /// <summary>
        /// Adds a set of WHERE clause conditions defined by the given expression, grouped by logical OR.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static IDbQueryBuilder WhereAny(this IDbQueryBuilder query, Action<IDbQueryWhereClause> expression)
        {
            var group = new DbQueryWhereClause(query.Fork(), SqlLogic.Or);

            expression.Invoke(group);

            return group.Complete();
        }

        /// <summary>
        /// Creates a WHERE clause equality comparison for a field and value in the form ([fieldName] = [equalsValue])
        /// </summary>
        /// <typeparam name="T">The type of the query builder.</typeparam>
        /// <typeparam name="TValue">The data type of the right operand</typeparam>
        /// <param name="where">The query builder instance</param>
        /// <param name="fieldName">The name of the field to use as the expression on the left of the operator.</param>
        /// <param name="equalsValue">The value of the parameter created for the right side of the operator.</param>
        ///         /// <param name="dbType">The data type of the database field.</param>
        /// <param name="size">The size of the database field (for fixed length data types).</param>
        /// <returns></returns>
        public static IDbQueryWhereClause FieldEquals<TValue>(this IDbQueryWhereClause where, string fieldName, TValue equalsValue, DbType? dbType = null, int? size = null)
        {
            return where.FieldMatch<TValue>(fieldName, SqlOperator.Equal, equalsValue, dbType: dbType, size: size);
        }

        /// <summary>
        /// Creates a WHERE clause equality comparison for a field and value in the form ([fieldName] = [value]), where the value data type is <see cref="String"/>.
        /// </summary>
        /// <param name="where">The query builder instance</param>
        /// <param name="fieldName">The name of the field to use as the expression on the left of the operator.</param>
        /// <param name="equalsValue">The value of the parameter created for the right side of the operator.</param>
        /// <param name="isAnsi">Is the database field an ANSI string or Unicode string?</param>
        /// <param name="length">Is the database field fixed length or variable length?</param>
        /// <returns></returns>
        public static IDbQueryWhereClause FieldEquals(this IDbQueryWhereClause where, string fieldName, string equalsValue, bool isAnsi = false, int? length = null)
        {
            return where.FieldMatch(fieldName, SqlOperator.Equal, equalsValue, isAnsi, length);
        }
        
        /// <summary>
        /// Creates a WHERE clause comparison for a field and value in the form ([fieldName] [operator] [value])
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="where"></param>
        /// <param name="fieldName">The name of the field to use as the expression on the left of the operator.</param>
        /// <param name="operator">The comparison operator to use.</param>
        /// <param name="value">The value of the parameter created for the right side of the operator.</param>
        /// <param name="dbType">The data type of the database field.</param>
        /// <param name="size">The size of the database field (for fixed length data types).</param>
        /// <returns></returns>
        public static IDbQueryWhereClause FieldMatch<TValue>(this IDbQueryWhereClause where, string fieldName, Csg.Data.Sql.SqlOperator @operator, TValue value, DbType? dbType = null, int? size = null) 
        {
            if (value is IDbTypeProvider)
            {
                where.AddFilter(new Csg.Data.Sql.SqlCompareFilter(where.Root, fieldName, @operator, ((IDbTypeProvider)value).GetDbType(), value));
            }
            else
            {
                where.AddFilter(new Csg.Data.Sql.SqlCompareFilter<TValue>(where.Root, fieldName, @operator, value) {
                    //TODO: Is AnsiString the right default?
                    DataType = dbType.HasValue ? dbType.Value : DbType.AnsiString,
                    Size = size,        
                });
            }

            return where;
        }

        /// <summary>
        /// Creates a WHERE clause comparison for a field and value in the form ([fieldName] [operator] [value]), where the value data type is <see cref="String"/>.
        /// </summary>
        /// <param name="where">The query builder instance</param>
        /// <param name="fieldName">The name of the field to use as the expression on the left of the operator.</param>
        /// <param name="operator">The comparison operator to use.</param>
        /// <param name="value">The value of the parameter created for the right side of the operator.</param>
        /// <param name="isAnsi">Is the database field an ANSI string or Unicode string?</param>
        /// <param name="length">Is the database field fixed length or variable length?</param>
        /// <returns></returns>
        public static IDbQueryWhereClause FieldMatch(this IDbQueryWhereClause where, string fieldName, Csg.Data.Sql.SqlOperator @operator, string value, bool isAnsi = false, int? length = null) 
        {
            var filter = new Csg.Data.Sql.SqlCompareFilter<string>(where.Root, fieldName, @operator, value);

            if (isAnsi && length.HasValue && length.Value > 0)
            {
                filter.DataType = DbType.AnsiStringFixedLength;
            }
            else if (isAnsi)
            {
                filter.DataType = DbType.AnsiString;
            }
            else if (length.HasValue && length.Value > 0)
            {
                filter.DataType = DbType.StringFixedLength;
            }
            else
            {
                filter.DataType = DbType.String;
            }

            if (length.HasValue && length.Value >= 0)
            {
                filter.Size = length.Value;
            }

            where.AddFilter(filter);

            return where;
        }

        /// <summary>
        /// Creates a WHERE clause comparison for a field and value in the form ([fieldName] LIKE [value]), where the value data type is <see cref="String"/>.
        /// </summary>
        /// <param name="where">The query builder instance</param>
        /// <param name="fieldName">The name of the field to use as the expression on the left of the operator.</param>
        /// <param name="operator">The comparison operator to use.</param>
        /// <param name="value">The value of the parameter created for the right side of the operator.</param>
        /// <param name="isAnsi">Is the database field an ANSI string or Unicode string?</param>
        /// <param name="length">Is the database field fixed length or variable length?</param>
        /// <returns></returns>
        public static IDbQueryWhereClause StringMatch(this IDbQueryWhereClause where, string fieldName, Csg.Data.Sql.SqlWildcardDecoration @operator, string value, bool isAnsi = false, int? length = null)
        {
            var filter = new Csg.Data.Sql.SqlStringMatchFilter(where.Root, fieldName, @operator, value);

            if (isAnsi && length.HasValue && length.Value > 0)
            {
                filter.DataType = DbType.AnsiStringFixedLength;
            }
            else if (isAnsi)
            {
                filter.DataType = DbType.AnsiString;
            }
            else if (length.HasValue && length.Value > 0)
            {
                filter.DataType = DbType.StringFixedLength;
            }
            else
            {
                filter.DataType = DbType.String;
            }

            if (length.HasValue && length.Value >= 0)
            {
                filter.Size = length.Value;
            }

            where.AddFilter(filter);

            return where;
        }

        /// <summary>
        /// Creates a WHERE clause equality comparison for a field equal to NULL ([fieldName] IS NULL)
        /// </summary>
        /// <param name="where">The query builder instance</param>
        /// <param name="fieldName">The name of the field to use as the expression on the left of the operator.</param>
        /// <returns></returns>
        public static IDbQueryWhereClause FieldIsNull(this IDbQueryWhereClause where, string fieldName)
        {
            where.AddFilter(new SqlNullFilter(where.Root, fieldName, true));
            return where;
        }

        /// <summary>
        /// Creates a WHERE clause equality comparison for a field not equal to NULL ([fieldName] IS NOT NULL)
        /// </summary>
        /// <param name="where">The query builder instance</param>
        /// <param name="fieldName">The name of the field to use as the expression on the left of the operator.</param>
        /// <returns></returns>
        public static IDbQueryWhereClause FieldIsNotNull(this IDbQueryWhereClause where, string fieldName)
        {
            where.AddFilter(new SqlNullFilter(where.Root, fieldName, false));
            return where;
        }

        /// <summary>
        /// Creates a WHERE clause comparison for a field and value in the form ([fieldName] [operator] [value]), where the value data type is <see cref="DateTime"/>.
        /// </summary>
        /// <param name="where">The query builder instance</param>
        /// <param name="fieldName">The name of the field to use as the expression on the left of the operator.</param>
        /// <param name="operator">The comparison operator to use.</param>
        /// <param name="value">The value of the parameter created for the right side of the operator.</param>
        /// <returns></returns>
        public static IDbQueryWhereClause FieldMatch(this IDbQueryWhereClause where, string fieldName, SqlOperator @operator, DateTime value) 
        {
            where.AddFilter(new Csg.Data.Sql.SqlCompareFilter(where.Root, fieldName, @operator, DbType.DateTime2, value));
            return where;
        }

        /// <summary>
        /// Creates a WHERE clause comparison for a field and value in the form ([fieldName] [operator] [value]), where the value data type is <see cref="DateTimeOffset"/>.
        /// </summary>
        /// <param name="where">The query builder instance</param>
        /// <param name="fieldName">The name of the field to use as the expression on the left of the operator.</param>
        /// <param name="operator">The comparison operator to use.</param>
        /// <param name="value">The value of the parameter created for the right side of the operator.</param>
        /// <returns></returns>
        public static IDbQueryWhereClause FieldMatch(this IDbQueryWhereClause where, string fieldName, SqlOperator @operator, DateTimeOffset value)
        {
            where.AddFilter(new Csg.Data.Sql.SqlCompareFilter(where.Root, fieldName, @operator, DbType.DateTimeOffset, value));
            return where;
        }

        /// <summary>
        /// Creates a WHERE clause comparison for a field and value in the form ([fieldName] &gt;= [begin] AND [fieldName] &lt;= [end]), where the value data type is <see cref="DateTime"/>.
        /// </summary>
        /// <param name="where">The query builder instance</param>
        /// <param name="fieldName">The name of the field to use as the expression on the left of the operator.</param>
        /// <param name="begin">The begin value of the range.</param>
        /// <param name="end">The end value of the range.</param>
        /// <param name="dbType">The data type of the database field.</param>
        /// <param name="size">The size of the database field (for fixed length data types).</param>
        /// <returns></returns>
        public static IDbQueryWhereClause FieldBetween<TValue>(this IDbQueryWhereClause where, string fieldName, TValue begin, TValue end, DbType? dbType = null, int? size = null)
        {
            return where
                .FieldMatch(fieldName, SqlOperator.GreaterThanOrEqual, begin, dbType: dbType, size: size)
                .FieldMatch(fieldName, SqlOperator.LessThanOrEqual, end, dbType: dbType, size: size);
        }

        /// <summary>
        /// Creates a WHERE clause comparison for a field and value in the form ([fieldName] &gt;= [begin] AND [fieldName] &lt;= [end]), where the value data type is <see cref="DateTime"/>.
        /// </summary>
        /// <param name="where">The query builder instance</param>
        /// <param name="fieldName">The name of the field to use as the expression on the left of the operator.</param>        
        /// <param name="begin">The begin date/time of the range.</param>
        /// <param name="end">The end date/time of the range.</param>
        /// <returns></returns>
        public static IDbQueryWhereClause FieldBetween(this IDbQueryWhereClause where, string fieldName, DateTime begin, DateTime end)
        {
            return where
                .FieldMatch(fieldName, SqlOperator.GreaterThanOrEqual, begin)
                .FieldMatch(fieldName, SqlOperator.LessThanOrEqual, end);
        }

        /// <summary>
        /// Creates a WHERE clause comparison for a field and value in the form ([fieldName] &gt;= [begin] AND [fieldName] &lt;= [end]), where the value data type is <see cref="DateTimeOffset"/>.
        /// </summary>
        /// <param name="where">The query builder instance</param>
        /// <param name="fieldName">The name of the field to use as the expression on the left of the operator.</param>        
        /// <param name="begin">The begin date/time of the range.</param>
        /// <param name="end">The end date/time of the range.</param>
        /// <returns></returns>
        public static IDbQueryWhereClause FieldBetween(this IDbQueryWhereClause where, string fieldName, DateTimeOffset begin, DateTimeOffset end)
        {
            return where
                .FieldMatch(fieldName, SqlOperator.GreaterThanOrEqual, begin)
                .FieldMatch(fieldName, SqlOperator.LessThanOrEqual, end);
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
                fork.OrderBy.Add(new Csg.Data.Sql.SqlOrderColumn() { ColumnName = field, SortDirection = Csg.Data.Sql.DbSortDirection.Ascending });
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
                fork.OrderBy.Add(new Csg.Data.Sql.SqlOrderColumn() { ColumnName = field, SortDirection = Csg.Data.Sql.DbSortDirection.Descending });
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
            fork.CommandTimeout = timeout;
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

            fork.Parameters.Add(new DbParameterValue() { 
                ParameterName = name,
                DbType = dbType,                
                Value = value,
                Size = size
            });

            return fork;
        }

        public static IDbQueryBuilder 

    }
}
