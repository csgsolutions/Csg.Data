using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csg.Data.Sql
{
    /// <summary>
    /// Utility extensions
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Adds a <see cref="SqlCompareFilter"/> to the given collection.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="table"></param>
        /// <param name="columnName"></param>
        /// <param name="oper"></param>
        /// <param name="dataType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static SqlCompareFilter Add(this ICollection<ISqlFilter> collection, ISqlTable table, string columnName, SqlOperator oper, System.Data.DbType dataType, object value)
        {
            var item = new SqlCompareFilter(table, columnName, oper, dataType, value);
            collection.Add(item);
            return item;
        }

        /// <summary>
        /// Adds <see cref="SqlOrderColumn"/> columns to the given order collection by parsing each value in the given sort expression
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="sortExpression">A SQL sort expression such as 'FirstName, LastName DESC'.</param>
        public static void Add(this ICollection<SqlOrderColumn> collection, string sortExpression)
        {            
            var parts = sortExpression.Split(',');
            foreach (var part in parts)
            {
                collection.Add(SqlOrderColumn.Parse(part));
            }
        }

        /// <summary>
        /// Renders each of the given elements into a single statement. This can be used to execute a batch.
        /// </summary>
        /// <param name="elements"></param>
        /// <returns></returns>
        public static SqlStatementBatch RenderBatch(this IEnumerable<ISqlStatementElement> elements, Abstractions.ISqlProvider provider)
        {
            var writer = provider.CreateWriter();
            int count = 0;

            foreach (var element in elements)
            {
                count++;
                element.Render(writer);
                writer.WriteEndStatement();
            }

            return new SqlStatementBatch(count, writer.ToString(), writer.BuildArguments.Parameters);
        }

        /// <summary>
        /// Creates a column referencing the given table.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="columnName"></param>
        /// <param name="alias"></param>
        /// <returns></returns>
        public static ISqlColumn Column(this ISqlTable table, string columnName, string alias)
        {
            return new SqlColumn(table, columnName, alias);
        }

        /// <summary>
        /// Creates a set of columns referencing the given table.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="columnExpressions"></param>
        /// <returns></returns>
        public static IEnumerable<ISqlColumn> Columns(this ISqlTable table, params string[] columnExpressions)
        {
            foreach (var columnExpr in columnExpressions)
            {
                yield return SqlColumn.Parse(table, columnExpr);
            }
        }

        internal static void AddRange<T>(this ICollection<T> items, IEnumerable<T> itemsToAdd)
        {
            itemsToAdd = itemsToAdd ?? throw new ArgumentNullException(nameof(itemsToAdd));
            foreach (var item in itemsToAdd)
            {
                items.Add(item);
            }
        }


    }
}
