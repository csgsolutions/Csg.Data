using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csg.Data.Sql
{
    public interface ISqlTable : ISqlStatementElement
    {
        void Compile(SqlBuildArguments args);
    }
}
