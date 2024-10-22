using System;
using System.Collections;
using System.Data;

namespace Csg.Data.Sql;

/// <summary>
///     Creates a T-SQL WHERE filter comparing a table column to a list of values.
/// </summary>
public class SqlListFilter : SqlSingleColumnFilterBase
{
    /// <summary>
    ///     Initializes a new instance.
    /// </summary>
    /// <param name="table">The table source for the column</param>
    /// <param name="columnName">The table column to filter on.</param>
    /// <param name="dataType">The data type of the column</param>
    /// <param name="values">The list of values to compare with.</param>
    public SqlListFilter(ISqlTable table, string columnName, DbType dataType, IEnumerable values) : base(table,
        columnName)
    {
        NotInList = false;
        DataType = dataType;
        Values = values;
    }

    /// <summary>
    ///     Gets or sets a value that indicates if the operator applied is 'NOT IN' or 'IN'
    /// </summary>
    public bool NotInList { get; set; }

    /// <summary>
    ///     Gets or sets the values to compare.
    /// </summary>
    public IEnumerable Values { get; set; }

    /// <summary>
    ///     Writes numeric values as literals instead of parameterized values.
    /// </summary>
    public bool UseLiteralNumbers { get; set; }

    /// <summary>
    ///     Writes the filter to the specified <see cref="SqlTextWriter" /> and adds any parameters to the specified
    ///     <see cref="SqlBuildArguments" />.
    /// </summary>
    /// <param name="writer">The instance of <see cref="SqlTextWriter" /> to write the T-SQL statement to.</param>
    /// <param name="args">An instance of <see cref="SqlBuildArguments" /> to use for parameters.</param>
    protected override void RenderInternal(SqlTextWriter writer, SqlBuildArguments args)
    {
        //TODO: make this impl agnostic
        var first = true;

        if (Values == null)
            throw new InvalidOperationException(string.Format(ErrorMessage.SqlListFilter_CollectionIsEmpty,
                ColumnName));

        writer.WriteBeginGroup();
        writer.WriteColumnName(ColumnName, args.TableName(Table));
        writer.WriteSpace();

        if (NotInList)
        {
            writer.Write(SqlConstants.NOT);
            writer.WriteSpace();
        }

        writer.Write(SqlConstants.IN);
        writer.WriteSpace();

        writer.WriteBeginGroup();
        foreach (var v in Values)
        {
            if (!first)
                writer.Write(",");
            else
                first = false;

            switch (DataType)
            {
                case DbType.String:
                case DbType.StringFixedLength:
                case DbType.AnsiString:
                case DbType.AnsiStringFixedLength:
                case DbType.Object:
                    writer.WriteParameter(args.CreateParameter(v.ToString(), DataType, Size));
                    break;
                case DbType.Boolean:
                    writer.Write(Convert.ToBoolean(v) ? 1 : 0);
                    break;
                case DbType.Int16:
                case DbType.Int32:
                case DbType.Int64:
                    if (UseLiteralNumbers)
                        writer.Write(Convert.ToInt64(v).ToString());
                    else
                        writer.WriteParameter(args.CreateParameter(v, DataType, Size));
                    break;
                default:
                    writer.WriteParameter(args.CreateParameter(v, DataType, Size));
                    break;
            }
        }

        if (first)
            throw new InvalidOperationException(string.Format(ErrorMessage.SqlListFilter_CollectionIsEmpty,
                ColumnName));

        writer.WriteEndGroup();
        writer.WriteEndGroup();
    }
}