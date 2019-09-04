using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csg.Data.Sql
{
    public static class Extensions
    {
        public static SqlCompareFilter Add(this ICollection<ISqlFilter> collection, ISqlTable table, string columnName, SqlOperator oper, System.Data.DbType dataType, object value)
        {
            var item = new SqlCompareFilter(table, columnName, oper, dataType, value);
            collection.Add(item);
            return item;
        }

        public static void Add(this ICollection<SqlOrderColumn> collection, string sortExpression)
        {            
            var parts = sortExpression.Split(',');
            foreach (var part in parts)
            {
                collection.Add(SqlOrderColumn.Parse(part));
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
