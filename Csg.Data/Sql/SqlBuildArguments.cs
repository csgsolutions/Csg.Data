using System;
using System.Collections.Generic;
using System.Data;

namespace Csg.Data.Sql;

/// <summary>
///     The arguments object tracks the parameters and tables used in a query rendering process.
/// </summary>
public class SqlBuildArguments
{
    /// <summary>
    ///     The parameter prefix that will be used when rendering parameter names.
    /// </summary>
    public const string SqlParameterPrefix = "@";

    /// <summary>
    ///     The format of a table name that will be used when rendering.
    /// </summary>
    public const string SqlTableNameFormat = "t{0}";

    private int _paramIndex;

    private readonly List<DbParameterValue> _params;
    private readonly List<ISqlTable> _tables;

    public SqlBuildArguments()
    {
        _params = new List<DbParameterValue>();
        _paramIndex = -1;
        _tables = new List<ISqlTable>();
    }

    /// <summary>
    ///     Gets or sets a value that indicates if the build process should be cancelled.
    /// </summary>
    public bool Cancel { get; set; }

    public IList<DbParameterValue> Parameters => _params;

    /// <summary>
    ///     Creates a new SqlParameter, optionally specifying the dbtype.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="dbType"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    public string CreateParameter(object value, DbType dbType, int? size = null)
    {
        _paramIndex++;

        var p = new DbParameterValue
        {
            ParameterName = string.Concat("p", _paramIndex.ToString()),
            Value = util.ConvertValue(value, dbType),
            DbType = dbType,
            Size = size
        };

        Parameters.Add(p);

        return p.ParameterName;
    }

    public void AssignAlias(ISqlTable table)
    {
        var index = _tables.IndexOf(table);
        if (index < 0) _tables.Add(table);
    }

    public string TableName(ISqlTable table)
    {
        var index = _tables.IndexOf(table);
        if (index < 0) throw new InvalidOperationException("The specified table is not associated with the query.");
        return string.Format(SqlTableNameFormat, index);
    }
}