using System.Collections.Generic;

namespace Csg.Data.Sql;

/// <summary>
///     Creates a T-SQL WHERE filter comparing a table column to a list of values.
/// </summary>
/// <typeparam name="T">The .NET data type of the table column.</typeparam>
public class SqlListFilter<T> : SqlListFilter
{
    /// <summary>
    ///     Initializes a new instance.
    /// </summary>
    /// <param name="table">The table source for the column</param>
    /// <param name="columnName">The table column to filter on.</param>
    /// <param name="values">The list of values to compare with.</param>
    public SqlListFilter(ISqlTable table, string columnName, IEnumerable<T> values) : base(table, columnName,
        DbConvert.TypeToDbType(typeof(T)), values)
    {
    }
}