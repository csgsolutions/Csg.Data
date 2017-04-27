using Csg.Data.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csg.Data
{
    internal class DbQueryWhereClause : IDbQueryWhereClause
    {
        public DbQueryWhereClause(IDbQueryBuilder query, SqlLogic logic)
        {
            this.Query = query;
            this.Filters = new SqlFilterCollection() { Logic = logic };
        }

        protected SqlFilterCollection Filters { get; set; }

        public IDbQueryBuilder Query { get; set; }

        public ISqlTable Root
        {
            get
            {
                return this.Query.Root;
            }
        }

        public void AddFilter(ISqlFilter filter)
        {
            this.Filters.Add(filter);
        }

        public IDbQueryBuilder Complete()
        {
            this.Query.AddFilter(this.Filters);
            return this.Query;
        }
    }
}
