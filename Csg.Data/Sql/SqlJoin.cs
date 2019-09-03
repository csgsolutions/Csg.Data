using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csg.Data.Sql
{
    public class SqlJoin : ISqlJoin
    {
        public SqlJoin(ISqlTable leftTable, SqlJoinType joinType, ISqlTable rightTable)
        {
            this.LeftTable = leftTable;
            this.JoinType = joinType;
            this.RightTable = rightTable;
        }

        public SqlJoin(ISqlTable leftTable, SqlJoinType joinType, ISqlTable rightTable, IEnumerable<ISqlFilter> conditions)
        {
            this.LeftTable = leftTable;
            this.JoinType = joinType;
            this.RightTable = rightTable;
            this.Conditions.AddRange(conditions);
        }

        public ISqlTable LeftTable { get; set; }
        public SqlJoinType JoinType { get; set; }
        public ISqlTable RightTable { get; set; }

        public List<ISqlFilter> Conditions
        {
            get
            {
                if (_conditions == null)
                    _conditions = new List<ISqlFilter>();
                return _conditions;
            }
        }
        private List<ISqlFilter> _conditions;

        #region ISqlJoin Members

        ISqlTable ISqlJoin.JoinedTable
        {
            get { return this.RightTable; }
        }

        #endregion

        #region ISqlStatementElement Members

        void ISqlStatementElement.Render(Abstractions.ISqlTextWriter writer)
        {
            writer.Render(this);
        }

        #endregion
    }
}
