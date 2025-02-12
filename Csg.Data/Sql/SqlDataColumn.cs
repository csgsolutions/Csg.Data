using System;
using System.Text.RegularExpressions;

namespace Csg.Data.Sql;

/// <summary>
///     Stores metadata about a database column
/// </summary>
//TODO: WHY DOES THIS CLASS EXIST? IT DOES NOT APPEAR TO BE USED EXCEPT IN TESTS AND HERE IN THIS FILE. THE STATIC FORMAT() FUNCTION IS.
public class SqlDataColumn
{
    private const string FORMAT_REGEX = @"^\[?([\w\s]+)\]?$";
    private const string QUOTE = "\"";

    public SqlDataColumn(string columnName)
    {
        ColumnName = columnName;
        DataType = TypeCode.Object;
    }

    protected internal SqlDataColumn(string columnName, TypeCode datatype)
    {
        ColumnName = columnName;
        DataType = datatype;
    }


    /// <summary>
    ///     Gets or sets the data column name
    /// </summary>
    public string ColumnName { get; set; }

    /// <summary>
    ///     Gets or sets the data type for the column
    /// </summary>
    public TypeCode DataType { get; set; }

    /// <summary>
    ///     Creates a new instance of SqlDataColunm from the specified input string. This method only supports word characters
    ///     and spaces in the column name.
    /// </summary>
    /// <param name="s">The name of a data column.</param>
    /// <returns></returns>
    /// <remarks>This method assumes column names are in SQL Server specific bracket notation.</remarks>
    public static SqlDataColumn Parse(string s)
    {
        //TODO: This method only supports a limited set of characters in column names because of the regex.
        //TODO: This method enforces SQL Server specific bracket notation
        //TODO: This method may be obsolete. It doesn't do anything that new SqlDataColumn() except use a restrictive regex.
        var regex = new Regex(FORMAT_REGEX);
        var m = regex.Match(s);
        if (m.Success)
            return new SqlDataColumn(m.Value);
        throw new FormatException(ErrorMessage.SqlDbColumn_InvalidNameFormat);
    }

    public static string Format(string s)
    {
        //TODO this whole process needs to take into account the target serer type (MySQL, whatever)
        return SqlTextWriter.FormatSqlServerIdentifier(s);
    }

    /// <summary>
    ///     Gets the string representation of a data column name
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return Format(ColumnName);
    }
}