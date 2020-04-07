using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csg.Data.Abstractions
{
    public interface ISqlProvider
    {
        ISqlTextWriter CreateWriter();
    }
}
