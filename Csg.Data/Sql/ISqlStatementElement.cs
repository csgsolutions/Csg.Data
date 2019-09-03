using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csg.Data.Sql
{
    public interface ISqlStatementElement
    {
        void Render(Abstractions.ISqlTextWriter writer);
    }
}
