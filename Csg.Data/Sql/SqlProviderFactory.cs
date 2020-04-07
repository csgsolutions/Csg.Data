using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csg.Data.Sql
{
    public static class SqlProviderFactory
    {
        private static System.Collections.Concurrent.ConcurrentDictionary<System.Type, Abstractions.ISqlProvider> _providers;

        public static Abstractions.ISqlProvider DefaultProvider = SqlServer.SqlServerProvider.Instance;

        static SqlProviderFactory()
        {
            _providers = new System.Collections.Concurrent.ConcurrentDictionary<Type, Abstractions.ISqlProvider>();
        }

        public static Abstractions.ISqlProvider GetProvider(System.Type connectionType)
        {
            if (_providers.TryGetValue(connectionType, out Abstractions.ISqlProvider provider))
            {
                return provider;
            }

            return DefaultProvider;
        }

        public static Abstractions.ISqlProvider GetProvider<T>()
        {
            return GetProvider(typeof(T));
        }

        public static Abstractions.ISqlProvider GetProvider(System.Data.IDbConnection connection)
        {
            return GetProvider(connection.GetType());
        }

        public static void RegisterProvider(Type connectionType, Abstractions.ISqlProvider provider)
        {
            _providers.TryAdd(connectionType, provider);
        }

        public static void RegisterProvider<T>(Abstractions.ISqlProvider provider) where T: System.Data.IDbConnection
        {
            _providers.TryAdd(typeof(T), provider);
        }
    }
}
