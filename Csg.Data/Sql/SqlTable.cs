using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Csg.Data.Sql
{
    public class SqlTable : SqlTableBase
    {
        public SqlTable(string tableName) : base()
        {
            this.TableName = tableName;
        }

        public string TableName { get; set; }

        protected override void RenderInternal(Abstractions.ISqlTextWriter writer)
        {
            writer.Render(this);
        }
    }
}
