using System.Data;

namespace Csg.Data;

/// <summary>
///     When implemented in a derived class, provides a database data type.
/// </summary>
public interface IDbTypeProvider
{
    /// <summary>
    ///     Gets the database data type.
    /// </summary>
    /// <returns></returns>
    DbType GetDbType();
}