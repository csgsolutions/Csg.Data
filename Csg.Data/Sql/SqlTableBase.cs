using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Csg.Data.Sql
{
    public abstract class SqlTableBase : ISqlTable
    {
        public static ISqlTable Create(string sql)
        {
            //check if this is a derived table (an in-line SELECT statement)
            if (Regex.IsMatch(sql, @"\bSELECT\b", RegexOptions.Compiled | RegexOptions.IgnoreCase))
            {
                return new SqlDerivedTable(sql);
            }
            return new SqlTable(sql);
        }

        public SqlTableBase()
        {
            _joins = new List<ISqlJoin>();
        }

        private List<ISqlJoin> _joins;

        protected abstract void RenderInternal(Abstractions.ISqlTextWriter writer);

        #region ISqlClause Members

        void ISqlStatementElement.Render(Abstractions.ISqlTextWriter writer)
        {
            this.RenderInternal(writer);            
        }

        #endregion

        #region ISqlTable2 Members

        void ISqlTable.Compile(SqlBuildArguments args)
        {
            args.AssignAlias(this);

            if (_joins == null)
                return;

            foreach (var join in _joins)
            {
                join.JoinedTable.Compile(args);
            }
        }

        #endregion
    }
}
