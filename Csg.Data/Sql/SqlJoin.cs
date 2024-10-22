using System.Collections.Generic;

namespace Csg.Data.Sql;

public class SqlJoin : ISqlJoin
{
    private List<ISqlFilter> _conditions;

    public SqlJoin(ISqlTable leftTable, SqlJoinType joinType, ISqlTable rightTable)
    {
        LeftTable = leftTable;
        JoinType = joinType;
        RightTable = rightTable;
    }

    public SqlJoin(ISqlTable leftTable, SqlJoinType joinType, ISqlTable rightTable, IEnumerable<ISqlFilter> conditions)
    {
        LeftTable = leftTable;
        JoinType = joinType;
        RightTable = rightTable;
        Conditions.AddRange(conditions);
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

    #region ISqlJoin Members

    ISqlTable ISqlJoin.JoinedTable => RightTable;

    #endregion

    #region ISqlStatementElement Members

    void ISqlStatementElement.Render(SqlTextWriter writer, SqlBuildArguments args)
    {
        writer.RenderJoin(LeftTable, JoinType, RightTable, Conditions, args);
    }

    #endregion
}