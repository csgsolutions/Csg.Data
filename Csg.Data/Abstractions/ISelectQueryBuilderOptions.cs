using Csg.Data.Sql;
using System.Collections.Generic;

namespace Csg.Data.Abstractions
{
    public interface ISelectQueryBuilderOptions
    {
        int? CommandTimeout { get; set; }

        bool SelectDistinct { get; set; }

        ICollection<DbParameterValue> Parameters { get; }

        IList<ISqlColumn> SelectColumns { get; }

        ICollection<ISqlFilter> Filters { get; }

        ICollection<ISqlJoin> Joins { get; }

        IList<SqlOrderColumn> OrderBy { get; }

        SqlPagingOptions? PagingOptions { get; set; }

        string Prefix { get; set; }

        string Suffix { get; set; }
    }
}
