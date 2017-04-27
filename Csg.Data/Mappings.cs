using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csg.Data
{
    /// <summary>
    /// Conversion utitilites for 
    /// </summary>
    public static class Mappings
    {
        /// <summary>
        /// Get the <see cref="System.Data.DbType"/> for the given Microsoft SQL Server data type name.
        /// </summary>
        /// <param name="sqlDbTypeName"></param>
        /// <returns></returns>
        [Obsolete("Use DbConvert.SqlServerTypeNameToDbType() instead.")]
        public static System.Data.DbType SqlDbTypeNameToDbType(string sqlDbTypeName)
        {
            return DbConvert.SqlServerTypeNameToDbType(sqlDbTypeName);
        }
    }
}
