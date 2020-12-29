using System;
using System.Data;

namespace Csg.Data
{
    internal static class util
    {
        public static System.InvalidOperationException InvalidOperationException(string format, object arg0)
        {
            return new InvalidOperationException(string.Format(format, arg0));
        }

        public static object ConvertValue(object value, DbType dbType)
        {
            return DbConvert.ConvertValue(value, dbType);
        }
    }
}
