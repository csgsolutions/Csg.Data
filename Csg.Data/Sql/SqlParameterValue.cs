using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Csg.Data.Sql
{
    public class DbParameterValue
    {
        public string ParameterName { get; set; }

        public object Value { get; set; }

        public System.Data.DbType DbType { get; set; }

        public int? Size { get; set; }

        public virtual IDbDataParameter AddToCommand(IDbCommand command)
        {
            var p = command.CreateParameter();
            p.ParameterName = this.ParameterName;
            p.Value = this.Value;
            p.DbType = this.DbType;
            if (this.Size.HasValue)
            {
                p.Size = this.Size.Value;
            }
            command.Parameters.Add(p);
            return p;
        }

        public string ParameterKey { get; set; }
    }
}
