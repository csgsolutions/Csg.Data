using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csg.Data
{
    /// <summary>
    /// When implemented in a derived class, provides a database data type.
    /// </summary>
    public interface IDbTypeProvider
    {
        /// <summary>
        /// Gets the database data type.
        /// </summary>
        /// <returns></returns>
        System.Data.DbType GetDbType();
    }
}