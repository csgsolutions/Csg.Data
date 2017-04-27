using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csg.Data.Sql
{
    public interface ISqlTableElement
    {
        ISqlTable Table { get; set; }
    }
}
