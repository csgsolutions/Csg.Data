using System.Collections.Generic;

namespace Csg.Data.Sql
{

    /// <summary>
    /// Represents a grouping of T-SQL filtering conditions
    /// </summary>
    public class SqlFilterCollection : IEnumerable<ISqlFilter>, ISqlFilter
    {
        private List<ISqlFilter> _innerList;

        public SqlFilterCollection()
        {
            _innerList = new List<ISqlFilter>();
            this.Logic = SqlLogic.And;
        }
        
        public void Add(ISqlFilter filter)
        {
            _innerList.Add(filter);
        }
    
        public void Add(ISqlTable table, string columnName, SqlOperator oper, System.Data.DbType dataType, object value)
        {
            _innerList.Add(new SqlCompareFilter(table, columnName, oper, dataType, value));
        }

        public void AddRange(IEnumerable<ISqlFilter> filters)
        {
            _innerList.AddRange(filters);
        }

        public void Clear()
        {
            this._innerList.Clear();
        }

        public int Count
        {
            get
            {
                return _innerList.Count;
            }
        }

        public SqlLogic Logic { get; set; }

        IEnumerator<ISqlFilter> IEnumerable<ISqlFilter>.GetEnumerator()
        {
            return _innerList.GetEnumerator();
        }

        System.Collections.IEnumerator  System.Collections.IEnumerable.GetEnumerator()
        {
            return _innerList.GetEnumerator();
        }

        #region ISqlStatement Members

        public void Render(Abstractions.ISqlTextWriter writer)
        {
            writer.Render(this);            
        }

        #endregion
    }

}
