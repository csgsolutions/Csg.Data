using System;
using System.Data;

namespace Csg.Data;

/// <summary>
///     Utilities for converting and mapping various database related types.
/// </summary>
public static class DbConvert
{
    /// <summary>
    ///     Converts a given value into the given Dbtype.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="dbType"></param>
    /// <returns></returns>
    public static object ConvertValue(object value, DbType dbType)
    {
        if (value == null || value is DBNull) return null;

        switch (dbType)
        {
            case DbType.AnsiString:
            case DbType.AnsiStringFixedLength:
            case DbType.String:
            case DbType.StringFixedLength:
                return Convert.ToString(value);
            case DbType.Byte:
                return Convert.ToByte(value);
            case DbType.Currency:
                return Convert.ToDecimal(value);
            case DbType.Date:
                return Convert.ToDateTime(value).Date;
            case DbType.DateTime:
                return Convert.ToDateTime(value);
            case DbType.DateTime2:
                return Convert.ToDateTime(value);
            case DbType.DateTimeOffset:
                return value is string ? DateTimeOffset.Parse(value.ToString()) : value;
            case DbType.Decimal:
                return Convert.ToDecimal(value);
            case DbType.Double:
                return Convert.ToDouble(value);
            case DbType.Int16:
                return Convert.ToInt16(value);
            case DbType.Int32:
                return Convert.ToInt32(value);
            case DbType.Int64:
                return Convert.ToInt64(value);
            case DbType.Single:
                return Convert.ToSingle(value);
            case DbType.Boolean:
                return Convert.ToBoolean(value);
            case DbType.Time:
                return TimeSpan.Parse(value.ToString());
            default:
                return value;
        }
    }

    /// <summary>
    ///     Converts the given type code value into it's appropriate DbType.
    /// </summary>
    /// <param name="typeCode"></param>
    /// <returns></returns>
    public static DbType TypeCodeToDbType(TypeCode typeCode)
    {
        switch (typeCode)
        {
            case TypeCode.Boolean:
            {
                return DbType.Boolean;
            }
            case TypeCode.Char:
            {
                return DbType.StringFixedLength;
            }
            case TypeCode.SByte:
            {
                return DbType.SByte;
            }
            case TypeCode.Byte:
            {
                return DbType.Byte;
            }
            case TypeCode.Int16:
            {
                return DbType.Int16;
            }
            case TypeCode.UInt16:
            {
                return DbType.UInt16;
            }
            case TypeCode.Int32:
            {
                return DbType.Int32;
            }
            case TypeCode.UInt32:
            {
                return DbType.UInt32;
            }
            case TypeCode.Int64:
            {
                return DbType.Int64;
            }
            case TypeCode.UInt64:
            {
                return DbType.UInt64;
            }
            case TypeCode.Single:
            {
                return DbType.Single;
            }
            case TypeCode.Double:
            {
                return DbType.Double;
            }
            case TypeCode.Decimal:
            {
                return DbType.Decimal;
            }
            case TypeCode.DateTime:
            {
                return DbType.DateTime;
            }
            case TypeCode.String:
            {
                return DbType.String;
            }
        }

        return DbType.Object;
    }

    /// <summary>
    ///     Gets the <see cref="DbType" /> for a System type.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static DbType TypeToDbType(Type type)
    {
        var underlyingType = Nullable.GetUnderlyingType(type);
        type = underlyingType ?? type;

        if (type == typeof(byte)) return DbType.Byte;
        if (type == typeof(short)) return DbType.Int16;
        if (type == typeof(int)) return DbType.Int32;
        if (type == typeof(long)) return DbType.Int64;
        if (type == typeof(bool)) return DbType.Boolean;
        if (type == typeof(Guid)) return DbType.Guid;
        if (type == typeof(DateTime)) return DbType.DateTime2;
        if (type == typeof(DateTimeOffset)) return DbType.DateTimeOffset;
        if (type == typeof(TimeSpan)) return DbType.Time;
        if (type == typeof(string)) return DbType.String;
        if (type == typeof(char)) return DbType.StringFixedLength;
        if (type == typeof(byte[])) return DbType.Binary;
        if (type == typeof(decimal)) return DbType.Decimal;
        if (type == typeof(float)) return DbType.Single;
        if (type == typeof(double)) return DbType.Double;

        return DbType.Object;
    }

    /// <summary>
    ///     Converts the given SQL Server type name into the appropriate <see cref="DbType" />.
    /// </summary>
    /// <param name="typeName"></param>
    /// <returns></returns>
    public static DbType SqlServerTypeNameToDbType(string typeName)
    {
        if (typeName == null) throw new ArgumentNullException(nameof(typeName));


        switch (typeName.ToUpperInvariant())
        {
            case "BIT": return DbType.Boolean;
            case "TINYINT": return DbType.Byte;
            case "SMALLINT": return DbType.Int16;
            case "INT": return DbType.Int32;
            case "BIGINT": return DbType.Int64;
            case "CHAR": return DbType.AnsiStringFixedLength;
            case "NCHAR": return DbType.StringFixedLength;
            case "VARCHAR": return DbType.AnsiString;
            case "NVARCHAR": return DbType.String;
            case "TEXT": return DbType.AnsiString;
            case "NTEXT": return DbType.String;
            case "BINARY": return DbType.Binary;
            case "IMAGE": return DbType.Binary;
            case "VARBINARY": return DbType.Binary;
            case "DATE": return DbType.Date;
            case "SMALLDATETIME": return DbType.DateTime;
            case "DATETIME": return DbType.DateTime;
            case "DATETIME2": return DbType.DateTime2;
            case "DATETIMEOFFSET": return DbType.DateTimeOffset;
            case "TIME": return DbType.Time;
            case "FLOAT": return DbType.Double;
            case "REAL": return DbType.Single;
            case "DECIMAL": return DbType.Decimal;
            case "NUMERIC": return DbType.Decimal;
            case "UNIQUEIDENTIFIER": return DbType.Guid;
            case "MONEY": return DbType.Decimal;
            case "SMALLMONEY": return DbType.Decimal;
            default:
                throw new NotSupportedException(
                    "Unsupported data type name encountered in SqlServerTypeNameToDbType: " + typeName);
        }
    }
}