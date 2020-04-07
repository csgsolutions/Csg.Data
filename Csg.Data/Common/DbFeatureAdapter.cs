using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csg.Data.Common
{
    public class DbFeatureAdapter : IQueryFeatureAdapter
    {
        public DbFeatureAdapter(System.Data.IDbConnection connection)
        {
            this.Connection = connection;
        }

        public DbFeatureAdapter(System.Data.IDbConnection connection, System.Data.IDbTransaction transaction)
        {
            this.Connection = connection;
            this.Transaction = transaction;
        }

        public System.Data.IDbConnection Connection { get; set; }

        public System.Data.IDbTransaction Transaction { get; set; }

        public TFeature Get<TFeature>(IDbQueryBuilder builder)
        {
            // TCommand can be assinged to IDbCommand, which means it derives from it,
            // which means we are probably doing the right thing.
            if (typeof(IDbCommand).IsAssignableFrom(typeof(TFeature)))
            {
                var stmt = builder.Render();
                var cmd = stmt.CreateCommand(this.Connection);

                cmd.Transaction = this.Transaction;

                if (builder.Configuration.CommandTimeout.HasValue)
                {
                    cmd.CommandTimeout = builder.Configuration.CommandTimeout.Value;
                }

                return (TFeature)cmd;
            }
            else if (typeof(IDbConnection).IsAssignableFrom(typeof(TFeature)))
            {
                return (TFeature)this.Connection;
            }
            else if (typeof(IDbTransaction).IsAssignableFrom(typeof(TFeature)))
            {
                return (TFeature)this.Transaction;
            }
            else
            {
                throw new InvalidOperationException($"The feature {typeof(TFeature).FullName} cannot be created by this adapter.");
            }
        }
    }

    public class DbCommandFeature
    {

    }

}
