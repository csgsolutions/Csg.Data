using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csg.Data.Sql
{
    public class SqlRecompileOption : SqlOptionBase
    {
        public override void Render(SqlTextWriter writer, SqlBuildArguments args)
        {
            writer.Write("RECOMPILE");
        }
    }
}
