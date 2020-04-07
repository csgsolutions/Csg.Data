using Csg.Data.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csg.Data.Common
{
    public interface IQueryFeatureAdapter
    {
        TFeature Get<TFeature>(IDbQueryBuilder builder);
    }
}
