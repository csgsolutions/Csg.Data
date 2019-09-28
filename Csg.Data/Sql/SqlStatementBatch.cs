using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csg.Data.Sql
{
    /// <summary>
    /// Represents a batch of multiple SQL statements combined into a single object.
    /// </summary>
    public class SqlStatementBatch : SqlStatement
    {
        /// <summary>
        /// Initializes a new instance with the given statement count and command text.
        /// </summary>
        /// <param name="statementCount"></param>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        public SqlStatementBatch(int statementCount, string commandText, ICollection<DbParameterValue> parameters) : base(commandText, parameters)
        {
            this.Count = statementCount;
        }

        /// <summary>
        /// Gets the number of statements in the batch.
        /// </summary>
        public int Count { get; protected set; }
    }
}
