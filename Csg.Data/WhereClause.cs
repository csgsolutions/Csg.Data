using Csg.Data.Abstractions;
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
    public class WhereClause : IWhereClause
    {
        private readonly ISqlTable _root;

        /// <summary>
        /// Initializes a new instance with the given table and AND/OR logic.
        /// </summary>
        /// <param name="root"></param>
        /// <param name="logic"></param>
        public WhereClause(ISqlTable root, SqlLogic logic)
        {
            _root = root;
            this.Filters = new SqlFilterCollection() { Logic = logic };
        }

        /// <summary>
        /// Gets or sets the filter collection
        /// </summary>
        public SqlFilterCollection Filters { get; set; }

        /// <summary>
        /// Gets or sets the root table associated with the WHERE clause.
        /// </summary>
        public ISqlTable Root
        {
            get
            {
                return _root;
            }
        }

        /// <summary>
        /// Adds a filter to the where clause
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public IWhereClause AddFilter(ISqlFilter filter)
        {
            this.Filters.Add(filter);
            return this;
        }

        public void ApplyToQuery(ISelectQueryBuilder builder)
        {
            if (this.Filters.Count > 0)
            {
                builder.Configuration.Filters.Add(this.Filters);
            }
        }

        /// <summary>
        /// Adds filters to teh given collection.
        /// </summary>
        /// <param name="filters"></param>
        public void ApplyTo(ICollection<ISqlFilter> filters)
        {
            if (this.Filters.Count > 0)
            {
                filters.Add(this.Filters);
            }
        }
    }
}
