using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csg.Data.Sql
{
    /// <summary>
    /// Provides a T-SQL comparison for a column having a NULL value or not.
    /// </summary>
    public class SqlNullFilter : SqlSingleColumnFilterBase
    {
        public SqlNullFilter(ISqlTable table, string columnName, bool isNull)
            : base(table, columnName)
        {
            this.IsNull = isNull;
        }

        public bool IsNull
        {
            get;
            set;
        }

        protected override void RenderInternal(SqlTextWriter writer, SqlBuildArguments args)
        {
            //TODO: Make this impl agnostic
            writer.WriteBeginGroup();
            writer.WriteColumnName(this.ColumnName, args.TableName(this.Table));
            writer.Write((this.IsNull) ? " IS NULL" : " IS NOT NULL");
            writer.WriteEndGroup();
        }
    }
}
