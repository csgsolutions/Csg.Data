using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace Csg.Data.Sql;

/// <summary>
///     Represents a grouping of T-SQL filtering conditions
/// </summary>
public class SqlFilterCollection : IEnumerable<ISqlFilter>, ISqlFilter
{
    private readonly List<ISqlFilter> _innerList;

    public SqlFilterCollection()
    {
        _innerList = new List<ISqlFilter>();
        Logic = SqlLogic.And;
    }

    public int Count => _innerList.Count;

    public SqlLogic Logic { get; set; }

    IEnumerator<ISqlFilter> IEnumerable<ISqlFilter>.GetEnumerator()
    {
        return _innerList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _innerList.GetEnumerator();
    }

    #region ISqlStatement Members

    public void Render(SqlTextWriter writer, SqlBuildArguments args)
    {
        if (_innerList.Count <= 0)
            return;

        if (Count > 1) writer.WriteBeginGroup();

        writer.RenderAll(this, args,
            string.Concat(" ", (Logic == SqlLogic.And ? SqlConstants.AND : SqlConstants.OR).ToUpper(), " "));

        if (Count > 1) writer.WriteEndGroup();
    }

    #endregion

    public void Add(ISqlFilter filter)
    {
        _innerList.Add(filter);
    }

    public void Add(ISqlTable table, string columnName, SqlOperator oper, DbType dataType, object value)
    {
        _innerList.Add(new SqlCompareFilter(table, columnName, oper, dataType, value));
    }

    public void AddRange(IEnumerable<ISqlFilter> filters)
    {
        _innerList.AddRange(filters);
    }

    public void Clear()
    {
        _innerList.Clear();
    }
}