using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csg.Data.Sql
{
    public interface ISqlJoin : ISqlStatementElement
    {
        ISqlTable JoinedTable { get; }
        //        IList<ISqlFilter> Conditions { get; }
    }
}
