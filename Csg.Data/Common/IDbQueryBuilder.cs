using Csg.Data.Abstractions;
using Csg.Data.Sql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Csg.Data.Common
{
    /// <summary>
    /// When implemented in a derived class, provides a query builder on an active connection and/or transaction for which to build and execute a SELECT statement.
    /// </summary>
    public interface IDbQueryBuilder : ISelectQueryBuilder
    {
        IQueryFeatureAdapter Features { get; }
    }
}
