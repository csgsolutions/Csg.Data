using System.Collections.Generic;

namespace Csg.Data.Sql;

/// <summary>
///     Creates a filter condition matching {ColumnName} { IN | NOT IN } (SELECT {SubQueryColumn} FROM {SubQueryTable}
///     WHERE {SubQueryFilters})
/// </summary>
public class SqlSubQueryFilter : ISqlFilter
{
    private List<ISqlFilter> _subQueryFilters;

    public SqlSubQueryFilter(ISqlTable table, ISqlTable subQueryTable)
    {
        Table = table;
        SubQueryTable = subQueryTable;
    }

    public SqlSubQueryFilter(ISqlTable table, string subQueryTable)
    {
        Table = table;
        SubQueryTable = SqlTableBase.Create(subQueryTable);
    }

    //// FROM <leftTable> WHERE <leftTable>.<matchColumnName> IN (SELECT <matchColumnName> FROM <subquery> WHERE <matchColumnName> = <leftTable>.<matchColumnName> AND <filterColumnName> <filterOperator> <filterValue>)
    //public static SqlSubQueryFilter Create(ISqlTable leftTable, ISqlTable subQuery, string selectColumnName, string matchColumnName, string filterColumnName, SqlOperator @filterOperator, System.Data.DbType filterType, object filterValue)
    //{
    //    var sqf = new SqlSubQueryFilter(leftTable, subQuery)
    //    {
    //        ColumnName = leftColumnName,
    //        SubQueryColumn = subQueryColumnName,
    //        SubQueryMode = SubQueryMode.InList
    //    };

    //    sqf.SubQueryFilters.Add(new SqlColumnCompareFilter(leftTable, matchColumnName, SqlOperator.Equal, subQuery));
    //    sqf.SubQueryFilters.Add(new SqlCompareFilter(subQuery, filterColumnName, filterOperator, filterType, filterValue));

    //    return sqf;
    //}

    //public static SqlSubQueryFilter Create(ISqlTable leftTable, ISqlTable subQuery, string selectColumnName, string matchColumnName, string filterColumnName, SqlWildcardDecoration @filterOperator, string filterValue, bool isAnsiString = false)
    //{
    //    var sqf = new SqlSubQueryFilter(leftTable, subQuery)
    //    {
    //        ColumnName = leftColumnName,
    //        SubQueryColumn = subQueryColumnName,
    //        SubQueryMode = SubQueryMode.InList
    //    };

    //    sqf.SubQueryFilters.Add(new SqlColumnCompareFilter(leftTable, matchColumnName, SqlOperator.Equal, subQuery));
    //    sqf.SubQueryFilters.Add(new SqlStringMatchFilter(subQuery, filterColumnName, filterOperator, filterValue)
    //    {
    //        DataType = isAnsiString ? System.Data.DbType.AnsiString : System.Data.DbType.String
    //    });

    //    return sqf;
    //}

    /// <summary>
    ///     Gets or sets the table to filter.
    /// </summary>
    public ISqlTable Table { get; set; }

    /// <summary>
    ///     Gets or sets the column in the left table to compare to the sub-query.
    /// </summary>
    public string ColumnName { get; set; }

    /// <summary>
    ///     Gets or sets the column in the sub-query to match the left column against.
    /// </summary>
    public string SubQueryColumn { get; set; }

    /// <summary>
    ///     Gets or sets the sub-query table expression
    /// </summary>
    public ISqlTable SubQueryTable { get; set; }

    /// <summary>
    ///     Gets a collection of filters to apply to the sub-query.
    /// </summary>
    public ICollection<ISqlFilter> SubQueryFilters
    {
        get
        {
            if (_subQueryFilters == null) _subQueryFilters = new List<ISqlFilter>();
            return _subQueryFilters;
        }
    }

    /// <summary>
    ///     Gets or sets a value that indicates if the operator applied is 'NOT IN' or 'IN'
    /// </summary>
    public SubQueryMode Condition { get; set; }

    void ISqlStatementElement.Render(SqlTextWriter writer, SqlBuildArguments args)
    {
        writer.WriteBeginGroup();

        writer.WriteColumnName(ColumnName, args.TableName(Table));
        writer.WriteSpace();

        if (Condition == SubQueryMode.NotInList)
        {
            writer.Write(SqlConstants.NOT);
            writer.WriteSpace();
        }

        writer.Write(SqlConstants.IN);

        writer.WriteSpace();

        args.AssignAlias(SubQueryTable);

        var builder = new SqlSelectBuilder(SubQueryTable);
        var subQueryColumn = new SqlColumn(SubQueryTable, SubQueryColumn);
        builder.Columns.Add(subQueryColumn);

        foreach (var filter in SubQueryFilters) builder.Filters.Add(filter);

        writer.WriteBeginGroup();
        builder.Render(writer, args);
        writer.WriteEndGroup();

        writer.WriteEndGroup();
    }
}

public enum SubQueryMode
{
    InList,
    NotInList
}