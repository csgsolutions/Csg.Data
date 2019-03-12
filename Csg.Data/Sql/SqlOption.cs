using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csg.Data.Sql
{
    public abstract class SqlOptionBase : ISqlStatementElement
    {
        public abstract void Render(SqlTextWriter writer, SqlBuildArguments args);
    }
}
