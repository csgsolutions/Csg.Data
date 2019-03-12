using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csg.Data.Sql
{
    public interface ISqlTextWriter
    {
        void PushIndent();

        void PopIndent();

        void Write(string value);

        void WriteLine(string value);

        void WriteParameterReference(string parameterName);

        void WriteFragment(SqlColumn column, SqlBuildArguments args, bool valueOnly=false);

        void WriteFragment(SqlTable table, SqlBuildArguments args);

        void WriteFragment(SqlCompareFilter filter, SqlBuildArguments args);

        void WriteFragment(SqlColumnCompareFilter filter, SqlBuildArguments args);        
    }
}
