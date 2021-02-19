using Csg.Data.Sql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csg.Data
{
    /// <summary>
    /// Extensions for adding filters to a <see cref="IDbQueryWhereClause"/>.
    /// </summary>
    public static class DbWhereClauseExtensions
    {
        /// <summary>
        /// Creates a WHERE clause equality comparison for a field and value in the form ([fieldName] = [equalsValue])
        /// </summary>
        /// <typeparam name="TValue">The data type of the right operand</typeparam>
        /// <param name="where">The query builder instance</param>
        /// <param name="fieldName">The name of the field to use as the expression on the left of the operator.</param>
        /// <param name="equalsValue">The value of the parameter created for the right side of the operator.</param>
        /// <param name="dbType">The data type of the database field.</param>
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
                where.AddFilter(new Csg.Data.Sql.SqlCompareFilter<TValue>(where.Root, fieldName, @operator, value)
                {
                    DataType = dbType.HasValue ? dbType.Value : DbConvert.TypeToDbType(typeof(TValue)),
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
        /// Creates a T-SQL WHERE IN filter comparing a table column to a list of matching values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="fieldName">The field name to compare against the list</param>
        /// <param name="values">A list of values to compare</param>
        /// <param name="useLiteralNumbers">When true, numeric types will rendered as literals intead of as parameters.</param>
        /// <returns></returns>
        public static IDbQueryWhereClause FieldIn<T>(this IDbQueryWhereClause where, string fieldName, IEnumerable<T> values, bool useLiteralNumbers = false)
        {
            where.AddFilter(new Csg.Data.Sql.SqlListFilter<T>(where.Root, fieldName, values) { UseLiteralNumbers = useLiteralNumbers });
            return where;
        }

        /// <summary>
        /// Creates a T-SQL WHERE NOT IN filter comparing a table column to a list of matching values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="fieldName">The field name to compare against the list</param>
        /// <param name="values">A list of values to compare</param>
        /// <param name="useLiteralNumbers">When true, numeric types will rendered as literals intead of as parameters.</param>
        /// <returns></returns>
        public static IDbQueryWhereClause FieldNotIn<T>(this IDbQueryWhereClause where, string fieldName, IEnumerable<T> values, bool useLiteralNumbers = false)
        {
            where.AddFilter(new Csg.Data.Sql.SqlListFilter<T>(where.Root, fieldName, values) { NotInList = true, UseLiteralNumbers = useLiteralNumbers });
            return where;
        }

        /// <summary>
        /// Adds an EXISTS(selectStatement) filter critera.
        /// </summary>
        /// <param name="where"></param>
        /// <param name="innerQuery">The value to render as the inner SELECT statement</param>
        /// <returns></returns>
        public static IDbQueryWhereClause Exists(this IDbQueryWhereClause where, SqlSelectBuilder innerQuery)
        {
            where.AddFilter(new Csg.Data.Sql.SqlExistFilter(innerQuery));
            return where;
        }

        /// <summary>
        /// Adds an EXISTS(selectStatement) filter critera.
        /// </summary>
        /// <param name="where"></param>
        /// <param name="innerQuery">The value to render as the inner SELECT statement</param>
        /// <returns></returns>
        public static IDbQueryWhereClause Exists(this IDbQueryWhereClause where, string sqlText, Action<IDbQueryWhereClause> subQueryFilters)
        {
            var innerTable = SqlTable.Create(sqlText);
            var innerQuery = new SqlSelectBuilder(innerTable);
            var innerWhere = new DbQueryWhereClause(innerTable, SqlLogic.And);
            innerQuery.Columns.Add(new SqlRawColumn("1"));
            subQueryFilters(innerWhere);
            innerWhere.ApplyTo(innerQuery.Filters);            
            where.AddFilter(new Sql.SqlExistFilter(innerQuery));

            return where;
        }

        /// <summary>
        /// Adds a {columnName} IN | NOT IN (SELECT {subQueryColumnName} FROM {sqlText} WHERE {SubQueryConditions})
        /// </summary>
        /// <param name="where"></param>
        /// <param name="columnName"></param>
        /// <param name="sqlText"></param>
        /// <param name="subQueryColumnName"></param>
        /// <param name="condition"></param>
        /// <param name="subQueryFilters"></param>
        /// <returns></returns>
        internal static IDbQueryWhereClause FieldSubQuery(IDbQueryWhereClause where, string columnName, string sqlText, string subQueryColumnName, SubQueryMode condition, Action<IDbQueryWhereClause> subQueryFilters)
        {
            var subQueryFilter = new Csg.Data.Sql.SqlSubQueryFilter(where.Root, SqlTable.Create(sqlText))
            {
                ColumnName = columnName,
                Condition = condition,
                SubQueryColumn = subQueryColumnName,
            };

            var subQueryWhere = new DbQueryWhereClause(subQueryFilter.SubQueryTable, SqlLogic.And);

            subQueryFilters(subQueryWhere);
            subQueryWhere.ApplyTo(subQueryFilter.SubQueryFilters);

            where.AddFilter(subQueryFilter);

            return where;
        }

        /// <summary>
        /// Adds a {columnName} IN (SELECT {subQueryColumnName} FROM {sqlText} WHERE {SubQueryConditions})
        /// </summary>
        /// <param name="where"></param>
        /// <param name="columnName">The column name on the table the condition is being added to.</param>
        /// <param name="sqlText">SQL text that defines the sub query.</param>
        /// <param name="subQueryColumnName">The column to select from the sub query and match against <paramref name="columnName"/></param>
        /// <param name="subQueryFilters">A set of filters to narrow the sub-query</param>
        /// <returns></returns>
        public static IDbQueryWhereClause FieldInSubQuery(this IDbQueryWhereClause where, string columnName, string sqlText, string subQueryColumnName, Action<IDbQueryWhereClause> subQueryFilters)
        {
            return FieldSubQuery(where, columnName, sqlText, subQueryColumnName, SubQueryMode.InList, subQueryFilters);
        }

        /// <summary>
        /// Adds a {columnName} NOT IN (SELECT {subQueryColumnName} FROM {sqlText} WHERE {SubQueryConditions})
        /// </summary>
        /// <param name="where"></param>
        /// <param name="columnName">The column name on the table the condition is being added to.</param>
        /// <param name="sqlText">SQL text that defines the sub query.</param>
        /// <param name="subQueryColumnName">The column to select from the sub query and match against <paramref name="columnName"/></param>
        /// <param name="subQueryFilters">A set of filters to narrow the sub-query</param>
        /// <returns></returns>
        public static IDbQueryWhereClause FieldNotInSubQuery(this IDbQueryWhereClause where, string columnName, string sqlText, string subQueryColumnName, Action<IDbQueryWhereClause> subQueryFilters)
        {
            return FieldSubQuery(where, columnName, sqlText, subQueryColumnName, SubQueryMode.NotInList, subQueryFilters);
        }

        /// <summary>
        /// Adds a (SELECT COUNT({countFieldName}) FROM {sqlText} WHERE {subQueryConditions}) {subQueryOperator} {countValue} condition to the query; 
        /// </summary>
        /// <param name="where"></param>
        /// <param name="sqlText"></param>
        /// <param name="countColumnName"></param>
        /// <param name="operator"></param>
        /// <param name="countValue"></param>
        /// <param name="subQueryWhere"></param>
        /// <returns></returns>
        public static IDbQueryWhereClause SubQueryCount(this IDbQueryWhereClause where, string sqlText, string countColumnName, SqlOperator @operator, int countValue, Action<IDbQueryWhereClause> subQueryWhere)
        {
            var sqf = new SqlCountFilter(where.Root, SqlTable.Create(sqlText), countColumnName, @operator, countValue);
            var sqfWhere = new DbQueryWhereClause(sqf.SubQueryTable, SqlLogic.And);
            subQueryWhere(sqfWhere);
            sqfWhere.ApplyTo(sqf.SubQueryFilters);
            where.AddFilter(sqf);
            return where;
        }
    }
}
