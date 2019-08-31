using Csg.Data.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csg.Data
{
    /// <summary>
    /// Used to build a set of fitlers with the fluent api.
    /// </summary>
    public class DbQueryWhereClause : IDbQueryWhereClause
    {
        private readonly ISqlTable _root;

        public DbQueryWhereClause(ISqlTable root, SqlLogic logic)
        {
            _root = root;
            this.Filters = new SqlFilterCollection() { Logic = logic };
        }

        public SqlFilterCollection Filters { get; set; }

        public ISqlTable Root
        {
            get
            {
                return _root;
            }
        }

        public IDbQueryWhereClause AddFilter(ISqlFilter filter)
        {
            this.Filters.Add(filter);
            return this;
        }

        public void ApplyToQuery(IDbQueryBuilder builder)
        {
            builder.AddFilter(this.Filters);
        }
    }
}
