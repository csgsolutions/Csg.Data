using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Csg.Data
{
    [Obsolete("Stop using this. Use something else")]
    /// <summary>
    /// Extension methods on enumerable sets
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Converts a set of queryable objects into and array of arrays.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">A queryable set of items.</param>
        /// <returns></returns>
        public static IList<object[]> ToArrayTable<T>(this IEnumerable<T> source)
        {
            return ToArrayTable(source, null);
        }

        /// <summary>
        /// Converts a set of queryable objects into and array of arrays, with only the given property name list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">A queryable set of items.</param>
        /// <param name="propertyNames">A list of property names on T that will become the resulting table headers.</param>
        /// <returns></returns>
        public static IList<object[]> ToArrayTable<T>(this IEnumerable<T> source, params string[] propertyNames)
        {
            var items = source.ToList();
            var properties = typeof(T).GetTypeInfo().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                .Where(x => propertyNames == null || propertyNames.Contains(x.Name))
                .ToList();

            var table = new List<object[]>();

            table.Add(properties.Select(s => s.Name).ToArray());

            object[] row;

            foreach (var item in items)
            {
                row = new object[properties.Count];

                for (int i = 0; i < properties.Count; i++)
                {
                    row[i] = properties[i].GetValue(item, null);
                }

                table.Add(row);
            }

            return table;
        }
    }
}
