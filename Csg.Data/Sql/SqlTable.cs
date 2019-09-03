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

        protected override void RenderInternal(SqlTextWriter writer, SqlBuildArguments args)
        {
            writer.Render(this);
        }
    }

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

        protected abstract void RenderInternal(SqlTextWriter writer, SqlBuildArguments args);

        #region ISqlClause Members

        void ISqlStatementElement.Render(SqlTextWriter writer, SqlBuildArguments args)
        {
            this.RenderInternal(writer, args);            
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

    public class SqlDerivedTable : SqlTableBase
    {
        public SqlDerivedTable(string commandText) : base()
        {
            this.CommandText = commandText;
        }
                
        public string CommandText { get; set; }

        protected override void RenderInternal(SqlTextWriter writer, SqlBuildArguments args)
        {
            writer.Render(this);            
        }        
    }
}
