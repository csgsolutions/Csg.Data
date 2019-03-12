using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csg.Data.Sql
{
    public class SqlOptimizeForOption : ISqlStatementElement
    {
        public void Render(SqlTextWriter writer, SqlBuildArguments args)
        {
            var parameter = args.Parameters.FirstOrDefault(x => x.ParameterKey == this.ParameterKey);

            if (parameter == null)
            {
                throw new Exception($"No parameter could be found with key {this.ParameterKey}");
            }

            writer.WriteOptimizeForParameterValue(parameter);
        }

        public string ParameterKey { get; set; }
    }
}

