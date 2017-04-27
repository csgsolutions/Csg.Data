using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Csg.Data.Linq
{

    /// <summary>
    /// Extension methods for use with IQueryable's
    /// </summary>
    public static class QueryableExtensions
    {
        /// <summary>
        /// Gets a single page of items from a sequence.
        /// </summary>
        /// <typeparam name="T">The data type of the result items.</typeparam>
        /// <param name="query">The sequence</param>
        /// <param name="pageNumber">The page number to retrieve, starting at 1.</param>
        /// <param name="pageSize">The number of items in each page.</param>
        /// <param name="pageCount">Provides the total number of pages available.</param>
        /// <returns></returns>
        public static IEnumerable<T> TakePage<T>(this IQueryable<T> query, int pageNumber, int pageSize, out int pageCount)
        {
            int itemCount;
            return TakePage(query, pageNumber, pageSize, out pageCount, out itemCount);
        }

        /// <summary>
        /// Gets a single page of items from a sequence.
        /// </summary>
        /// <typeparam name="T">The data type of the result items.</typeparam>
        /// <param name="query">The sequence</param>
        /// <param name="pageNumber">The page number to retrieve, starting at 1.</param>
        /// <param name="pageSize">The number of items in each page.</param>
        /// <param name="pageCount">Provides the total number of pages available.</param>
        /// <param name="itemCount">Provides the total number of items availabe.</param>
        /// <returns></returns>
        public static IEnumerable<T> TakePage<T>(this IQueryable<T> query, int pageNumber, int pageSize, out int pageCount, out int itemCount)
        {
            if (pageNumber < 1)
                throw new ArgumentException("The value for 'page' must be greater than or equal to 1", "pageNumber");

            itemCount = query.Count();

            pageCount = (int)Math.Ceiling((double)itemCount / (double)pageSize);

            if (pageNumber > pageCount)
                pageNumber = pageCount;

            if (pageNumber > 1)
                return query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            else
                return query.Take(pageSize);
        }
        
        /// <summary>
        /// Returns an ordered <see cref="IQueryable"/> after creating the OrderBy method from the specified sort expression
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="source"></param>
        /// <param name="propertyName">The property name of the object on which sorting should be evaluated.</param>
        /// <param name="sortAsc">Pass true to sort ascending, or false to sort descending.</param>
        /// <returns></returns>
        public static IQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> source, string propertyName, bool sortAsc = true) where TEntity : class
        {
            var type = typeof(TEntity);
            string methodName = "OrderBy";

            if (!sortAsc)
            {
                methodName += "Descending";
            }

            var property = type.GetTypeInfo().GetProperty(propertyName);

            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExp = Expression.Lambda(propertyAccess, parameter);
            MethodCallExpression resultExp = Expression.Call(typeof(Queryable), methodName, new Type[] { type, property.PropertyType }, source.Expression, Expression.Quote(orderByExp));

            return (IQueryable<TEntity>)source.Provider.CreateQuery(resultExp);
        }

        /// <summary>
        /// Filters a sequence of values using a predicate based on a string match with the given field and value.
        /// </summary>
        /// <typeparam name="T">The type of elements of source.</typeparam>
        /// <param name="source">The sequence to be filtered.</param>
        /// <param name="fieldName">The field name of the element on which to filter.</param>
        /// <param name="value">A string value that has been decorated with the wildcard character * appropriately for the requested type of match.</param>
        /// <param name="applyWhenNull">If true, the filter will be applied even if the <paramref name="value"/> parameter is null. The default is false.</param>
        /// <remarks>This method adds a predicate to the query that compares a field to a string value using Equals, Contains, EndsWith or StartsWith depending on how the value is decorated with wildcard characters.</remarks>
        /// <returns></returns>
        public static IQueryable<T> WhereLike<T>(this IQueryable<T> source, string fieldName, string value, bool applyWhenNull = false)
        {
            if (!applyWhenNull && string.IsNullOrEmpty(value))
            {
                return source;
            }

            var valueTrimmed = value.Trim('*');
            var type = typeof(T);
            var parameterExpr = Expression.Parameter(type, "p");
            var valueExpr = Expression.Constant(valueTrimmed, typeof(string));

            string compareMethodName = "Equals";

            if (value.StartsWith("*") && value.EndsWith("*"))
            {
                compareMethodName = "Contains";
            }
            else if (value.StartsWith("*"))
            {
                compareMethodName = "EndsWith";
            }
            else if (value.EndsWith("*"))
            {
                compareMethodName = "StartsWith";
            }
            else
            {
                compareMethodName = "Equals";
            }

            var compareMethod = typeof(string).GetTypeInfo().GetMethod(compareMethodName, new[] { typeof(string) });
            var memberExpr = Expression.Call(Expression.PropertyOrField(parameterExpr, fieldName), compareMethod, valueExpr);
            var whereExpr = Expression.Lambda<Func<T, bool>>(Expression.Equal(memberExpr, Expression.Constant(true)), parameterExpr);
            var methodCallExpr = Expression.Call(typeof(Queryable), "Where", new Type[] { type }, source.Expression, whereExpr);

            return source.Provider.CreateQuery<T>(methodCallExpr);
        }
    }
}
