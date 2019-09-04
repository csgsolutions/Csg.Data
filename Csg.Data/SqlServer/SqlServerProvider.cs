using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Csg.Data.Abstractions;

namespace Csg.Data.SqlServer
{
    /// <summary>
    /// Provides an implemention of <see cref="ISqlProvider"/> for Microsoft SQL Server.
    /// </summary>
    public class SqlServerProvider : Abstractions.ISqlProvider
    {
        /// <summary>
        /// Gets the singleton provider instance for SQL Server.
        /// </summary>
        public static readonly SqlServerProvider Instance = new SqlServerProvider();

        /// <summary>
        /// Creates a sql text writer optimized for SQL Server.
        /// </summary>
        /// <returns></returns>
        public ISqlTextWriter CreateWriter()
        {
            return new Sql.SqlTextWriter(this);
        }
    }
}
