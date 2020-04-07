using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Csg.Data.Common
{
    /// <summary>
    /// Extension methods for database connections
    /// </summary>
    public static class DbConnectionExtensions
    {
        /// <summary>
        /// Opens a given database connection if it is not already open, and returns an object, that when disposed, will close the connection only if it was not already open.
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static DbConnectionScope EnsureOpen(this IDbConnection connection)
        {
            return new DbConnectionScope(connection);
        }

        /// <summary>
        /// Opens a given database connection if it is not already open, and returns an object, that when disposed, will close the connection only if it was not already open.
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        public static DbConnectionScope EnsureOpen(this IDbScope scope)
        {
            return new DbConnectionScope(scope.Connection);
        }       
               
    }
}
