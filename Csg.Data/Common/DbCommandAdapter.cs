using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csg.Data.Common
{
    public class DbCommandAdapter : Abstractions.IDbCommandAdapter
    {
        public DbCommandAdapter(System.Data.IDbConnection connection)
        {
            this.Connection = connection;
        }

        public DbCommandAdapter(System.Data.IDbConnection connection, System.Data.IDbTransaction transaction)
        {
            this.Connection = connection;
            this.Transaction = transaction;
        }

        public System.Data.IDbConnection Connection { get; set; }

        public System.Data.IDbTransaction Transaction { get; set; }

        public TCommand CreateCommand<TCommand>(IDbQueryBuilder builder)
        {
            // TCommand can be assinged to IDbCommand, which means it derives from it,
            // which means we are probably doing the right thing.
            if (typeof(IDbCommand).IsAssignableFrom(typeof(TCommand)))
            {
                var stmt = builder.Render();
                var cmd = stmt.CreateCommand(this.Connection);

                cmd.Transaction = this.Transaction;

                if (builder.Configuration.CommandTimeout.HasValue)
                {
                    cmd.CommandTimeout = builder.Configuration.CommandTimeout.Value;
                }

                return (TCommand)cmd;
            }
            else
            {
                throw new InvalidOperationException($"The given command type {typeof(TCommand).FullName} cannot be created by this adapter.");
            }
        }
    }
}
