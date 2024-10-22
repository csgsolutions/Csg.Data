using System.Data;

namespace Csg.Data.Sql;

/// <summary>
///     Provides a base class for a filter that compares against a single column
/// </summary>
public abstract class SqlSingleColumnFilterBase : ISqlFilter
{
    public SqlSingleColumnFilterBase(ISqlTable table, string columnName) : this(table, columnName, DbType.Object)
    {
    }

    public SqlSingleColumnFilterBase(ISqlTable table, string columnName, DbType dataType)
    {
        Table = table;
        ColumnName = columnName;
        DataType = dataType;
    }


    /// <summary>
    ///     Gets or sets the related table.
    /// </summary>
    public ISqlTable Table { get; set; }

    /// <summary>
    ///     Gets or sets the data column name
    /// </summary>
    public string ColumnName { get; set; }

    /// <summary>
    ///     Gets or sets the value data type.
    /// </summary>
    public DbType DataType { get; set; }

    /// <summary>
    ///     Gets or sets the maximum size, in bytes, of the value.
    /// </summary>
    public int? Size { get; set; }

    #region ISqlStatementElement Members

    void ISqlStatementElement.Render(SqlTextWriter writer, SqlBuildArguments args)
    {
        RenderInternal(writer, args);
    }

    #endregion

    #region ISqlFilter Members

    protected abstract void RenderInternal(SqlTextWriter writer, SqlBuildArguments args);

    #endregion
}