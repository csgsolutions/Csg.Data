using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csg.Data.Sql.Tests;

/// <summary>
///     Summary description for UnitTest1
/// </summary>
[TestClass]
public class SqlSelectBuilderTests
{
    [TestMethod]
    public void TestColumnFormatting()
    {
        var col1 = new SqlDataColumn("[FieldName]");
        var col2 = new SqlDataColumn("FieldName");

        Assert.AreEqual(col1.ToString(), "[FieldName]");
        Assert.AreEqual(col1.ToString(), col2.ToString());
    }

    [TestMethod]
    public void TestColumnAliasing()
    {
        const string test = "SELECT [t0].[Foo] AS [Bar] FROM [TableName] AS [t0];";

        var q = new SqlSelectBuilder("TableName");
        q.Columns.Add(new SqlColumn(q.Table, "Foo", "Bar"));

        var s = q.Render();

        Assert.AreEqual(s.CommandText, test);
    }

    [TestMethod]
    public void TestExpressionColumn()
    {
        const string test = "SELECT (Foo + 1) AS [Bar] FROM [TableName] AS [t0];";

        var q = new SqlSelectBuilder("TableName");
        var col = new SqlRawColumn("(Foo + 1)", "Bar")
        {
            Table = q.Table
        };
        q.Columns.Add(col);
        var s = q.Render();

        Assert.AreEqual(s.CommandText, test);
    }

    [TestMethod]
    public void TestParse()
    {
        const string test = "SELECT * FROM (SELECT * FROM DimWidget WHERE WidgetID LIKE 'a%') AS [t0] ORDER BY [WidgetName];";
        var q = new SqlSelectBuilder("SELECT * FROM DimWidget WHERE WidgetID LIKE 'a%' ORDER BY [WidgetName];");

        var s = q.Render();

        Assert.IsTrue(string.Equals(s.CommandText, test, StringComparison.OrdinalIgnoreCase),
            "Output CommandText does not match expected result");
    }

    [TestMethod]
    public void TestParseOrderBy()
    {
        const string expected = "SELECT * FROM (SELECT * FROM Report_Nrt) AS [t0] ORDER BY [DivisionName];";
        const string query = "SELECT * FROM Report_Nrt ORDER BY DivisionName";
        var q = new SqlSelectBuilder(query);

        var s = q.Render();

        Assert.IsTrue(string.Equals(s.CommandText, expected, StringComparison.OrdinalIgnoreCase),
            "Output CommandText does not match expected result");
    }

    [TestMethod]
    public void TestSort()
    {
        const string test = "SELECT * FROM (SELECT * FROM DimWidget WHERE WidgetID LIKE 'a%') AS [t0] ORDER BY [WidgetName],[WidgetID];";
        var q = new SqlSelectBuilder("SELECT * FROM DimWidget WHERE WidgetID LIKE 'a%' ORDER BY [WidgetName];");

        q.OrderBy.Add("[WidgetID]");

        var s = q.Render();

        Assert.IsTrue(string.Equals(s.CommandText, test, StringComparison.OrdinalIgnoreCase),
            "Output CommandText does not match expected result");
    }

    [TestMethod]
    public void TestEqualFilter()
    {
        const string test = "SELECT * FROM (SELECT WidgetName,WidgetID FROM DimWidget) AS [t0] WHERE ([t0].[WidgetID]=@p0) ORDER BY [WidgetName];";
        var q = new SqlSelectBuilder("SELECT WidgetName,WidgetID FROM DimWidget ORDER BY [WidgetName];");

        q.Filters.Add(q.Table, "WidgetID", SqlOperator.Equal, DbType.Int32, 10);

        var s = q.Render();


        Assert.IsTrue(s.Parameters.Count > 0, "Parameter count should be 1.");
        Assert.IsTrue(s.Parameters.Count(x => (int)x.Value == 10) == 1, "Parameter value not correct");
        Assert.IsTrue(string.Equals(s.CommandText, test, StringComparison.OrdinalIgnoreCase),
            "Output CommandText does not match expected result");

        var sqlCmd = new SqlCommand();
        s.SetupCommand(sqlCmd);
        Assert.AreEqual(1, sqlCmd.Parameters.Count);
        Assert.IsInstanceOfType(sqlCmd.Parameters[0], typeof(SqlParameter));
    }

    [TestMethod]
    public void TestNotEqualFilter()
    {
        const string test = "SELECT * FROM (SELECT WidgetName,WidgetID FROM DimWidget) AS [t0] WHERE ([t0].[OfficeKey]<>@p0) ORDER BY [WidgetName];";
        var q = new SqlSelectBuilder("SELECT WidgetName,WidgetID FROM DimWidget ORDER BY [WidgetName];");

        q.Filters.Add(q.Table, "OfficeKey", SqlOperator.NotEqual, DbType.Int32, 10);

        var s = q.Render();

        Assert.IsTrue(s.Parameters.Count > 0, "Parameter count should be 1.");
        Assert.IsTrue((int)s.Parameters.Single().Value == 10, "Parameter value not correct");
        Assert.IsTrue(string.Equals(s.CommandText, test, StringComparison.OrdinalIgnoreCase),
            "Output CommandText does not match expected result");
    }

    [TestMethod]
    public void TestGreaterThanFilter()
    {
        const string test = "SELECT * FROM (SELECT WidgetName,WidgetID FROM DimWidget) AS [t0] WHERE ([t0].[OfficeKey]>@p0) ORDER BY [WidgetName];";
        var q = new SqlSelectBuilder("SELECT WidgetName,WidgetID FROM DimWidget ORDER BY [WidgetName];");

        q.Filters.Add(q.Table, "OfficeKey", SqlOperator.GreaterThan, DbType.Int32, 10);
        var s = q.Render();

        Assert.IsTrue(s.Parameters.Count > 0, "Parameter count should be 1.");
        Assert.IsTrue((int)s.Parameters.Single().Value == 10, "Parameter value not correct");
        Assert.IsTrue(string.Equals(s.CommandText, test, StringComparison.OrdinalIgnoreCase),
            "Output CommandText does not match expected result");
    }

    [TestMethod]
    public void TestLessThanFilter()
    {
        const string test = "SELECT * FROM (SELECT WidgetName,WidgetID FROM DimWidget) AS [t0] WHERE ([t0].[OfficeKey]<@p0) ORDER BY [WidgetName];";
        var q = new SqlSelectBuilder("SELECT WidgetName,WidgetID FROM DimWidget ORDER BY [WidgetName];");

        q.Filters.Add(q.Table, "OfficeKey", SqlOperator.LessThan, DbType.Int32, 10);

        var s = q.Render();

        Assert.IsTrue(s.Parameters.Count > 0, "Parameter count should be 1.");
        Assert.IsTrue((int)s.Parameters.Single().Value == 10, "Parameter value not correct");
        Assert.IsTrue(string.Equals(s.CommandText, test, StringComparison.OrdinalIgnoreCase),
            "Output CommandText does not match expected result");
    }

    [TestMethod]
    public void TestGreaterThanOrEqualFilter()
    {
        const string test = "SELECT * FROM (SELECT WidgetName,WidgetID FROM DimWidget) AS [t0] WHERE ([t0].[OfficeKey]>=@p0) ORDER BY [WidgetName];";
        var q = new SqlSelectBuilder("SELECT WidgetName,WidgetID FROM DimWidget ORDER BY [WidgetName];");

        q.Filters.Add(q.Table, "OfficeKey", SqlOperator.GreaterThanOrEqual, DbType.Int32, 10);

        var s = q.Render();

        Assert.IsTrue(s.Parameters.Count > 0, "Parameter count should be 1.");
        Assert.IsTrue((int)s.Parameters.Single().Value == 10, "Parameter value not correct");
        Assert.IsTrue(string.Equals(s.CommandText, test, StringComparison.OrdinalIgnoreCase),
            "Output CommandText does not match expected result");
    }

    [TestMethod]
    public void TestLessThanOrEqualFilter()
    {
        const string test = "SELECT * FROM (SELECT WidgetName,WidgetID FROM DimWidget) AS [t0] WHERE ([t0].[OfficeKey]<=@p0) ORDER BY [WidgetName];";
        var q = new SqlSelectBuilder("SELECT WidgetName,WidgetID FROM DimWidget ORDER BY [WidgetName];");

        q.Filters.Add(q.Table, "OfficeKey", SqlOperator.LessThanOrEqual, DbType.Int32, 10);

        var s = q.Render();

        Assert.IsTrue(s.Parameters.Count > 0, "Parameter count should be 1.");
        Assert.IsTrue((int)s.Parameters.Single().Value == 10, "Parameter value not correct");
        Assert.IsTrue(string.Equals(s.CommandText, test, StringComparison.OrdinalIgnoreCase),
            "Output CommandText does not match expected result");
    }

    [TestMethod]
    public void TestDateTimeFilter()
    {
        const string test = "SELECT * FROM (SELECT WidgetName,WidgetID FROM DimWidget) AS [t0] WHERE ([t0].[CreateDate]>=@p0 AND [t0].[CreateDate]<=@p1) ORDER BY [WidgetName];";
        var q = new SqlSelectBuilder("SELECT WidgetName,WidgetID FROM DimWidget ORDER BY [WidgetName];");
        var beginDate = DateTime.Parse("2011-01-01 05:00");
        var endDate = DateTime.Parse("2011-01-01 23:30");

        q.Filters.Add(new SqlDateTimeFilter(q.Table, "CreateDate", beginDate, endDate));

        var s = q.Render();

        Assert.IsTrue(s.Parameters.Count > 0, "Parameter count should be 1.");
        Assert.IsTrue((DateTime)s.Parameters.ToList()[0].Value == beginDate, "Begin date does not match parameter");
        Assert.IsTrue((DateTime)s.Parameters.ToList()[1].Value == endDate, "End date does not match parameter");
        Assert.IsTrue(string.Equals(s.CommandText, test, StringComparison.OrdinalIgnoreCase),
            "Output CommandText does not match expected result");
    }

    [TestMethod]
    public void TestDateFilter()
    {
        const string test = "SELECT * FROM (SELECT WidgetName,WidgetID FROM DimWidget) AS [t0] WHERE (CAST([t0].[CreateDate] as date)>=@p0 AND CAST([t0].[CreateDate] as date)<=@p1) ORDER BY [WidgetName];";
        var q = new SqlSelectBuilder("SELECT WidgetName,WidgetID FROM DimWidget ORDER BY [WidgetName];");
        var beginDate = DateTime.Parse("2011-01-01 05:00");
        var endDate = DateTime.Parse("2011-01-01 23:30");

        q.Filters.Add(new SqlDateFilter(q.Table, "CreateDate", beginDate, endDate));

        var s = q.Render();

        Assert.IsTrue(s.Parameters.Count == 2, "Parameter count should be 2.");
        Assert.IsTrue(s.Parameters.All(x => (DateTime)x.Value == ((DateTime)x.Value).Date),
            "Date params should not have time in parameter.");
        Assert.IsTrue(string.Equals(s.CommandText, test, StringComparison.OrdinalIgnoreCase),
            "Output CommandText does not match expected result");
    }

    [TestMethod]
    public void TestNullFilter()
    {
        var test =
            "SELECT * FROM (SELECT WidgetName,WidgetID FROM DimWidget) AS [t0] WHERE ([t0].[Foreman] IS NULL) ORDER BY [WidgetName];";
        var q = new SqlSelectBuilder("SELECT WidgetName,WidgetID FROM DimWidget ORDER BY [WidgetName];");

        q.Filters.Add(new SqlNullFilter(q.Table, "Foreman", true));

        var s = q.Render();

        Assert.IsTrue(s.Parameters.Count <= 0, "Parameter count should be 0.");
        Assert.IsTrue(string.Equals(s.CommandText, test, StringComparison.OrdinalIgnoreCase),
            "Output CommandText does not match expected result");

        test =
            "SELECT * FROM (SELECT WidgetName,WidgetID FROM DimWidget) AS [t0] WHERE ([t0].[Foreman] IS NOT NULL) ORDER BY [WidgetName];";
        q.Filters.Clear();
        q.Filters.Add(new SqlNullFilter(q.Table, "Foreman", false));
        s = q.Render();
        Assert.IsTrue(string.Equals(s.CommandText, test, StringComparison.OrdinalIgnoreCase),
            "Output CommandText does not match expected result");
    }

    [TestMethod]
    public void TestStringFilter()
    {
        const string test = "SELECT * FROM (SELECT WidgetName,WidgetID FROM DimWidget) AS [t0] WHERE ([t0].[Foreman] LIKE @p0) ORDER BY [WidgetName];";
        var q = new SqlSelectBuilder("SELECT WidgetName,WidgetID FROM DimWidget ORDER BY [WidgetName];");

        q.Filters.Add(new SqlStringMatchFilter(q.Table, "Foreman", SqlWildcardDecoration.BeginsWith, "B"));
        var s = q.Render();

        Assert.IsTrue(string.Equals(s.CommandText, test, StringComparison.OrdinalIgnoreCase),
            "Output CommandText does not match expected result");

        Assert.IsTrue(s.Parameters.Count == 1, "Parameter count should be 1.");
        Assert.IsTrue((string)s.Parameters.Single().Value == "B%", "Parameter value is incorrect");

        q.Filters.Clear();
        q.Filters.Add(new SqlStringMatchFilter(q.Table, "Foreman", SqlWildcardDecoration.EndsWith, "B"));
        s = q.Render();

        Assert.IsTrue(s.Parameters.Count == 1, "Parameter count should be 1.");
        Assert.IsTrue((string)s.Parameters.Single().Value == "%B", "Parameter value is incorrect");

        q.Filters.Clear();
        q.Filters.Add(new SqlStringMatchFilter(q.Table, "Foreman", SqlWildcardDecoration.Contains, "B"));
        s = q.Render();

        Assert.IsTrue(s.Parameters.Count == 1, "Parameter count should be 1.");
        Assert.IsTrue((string)s.Parameters.Single().Value == "%B%", "Parameter value is incorrect");
    }

    [TestMethod]
    public void TestGroupBy()
    {
        const string test = "SELECT [t0].[OfficeKey],COUNT([t0].[WidgetID]) AS [MeterCount] FROM (SELECT WidgetName,WidgetID,OfficeKey FROM DimWidget) AS [t0] GROUP BY [t0].[OfficeKey] ORDER BY [WidgetName];";
        var q = new SqlSelectBuilder("SELECT WidgetName,WidgetID,OfficeKey FROM DimWidget ORDER BY [WidgetName];");

        q.Columns.Add(new SqlColumn(q.Table, "OfficeKey"));
        q.Columns.Add(new SqlColumn(q.Table, "WidgetID", "MeterCount") { Aggregate = SqlAggregate.Count });

        var s = q.Render();

        Assert.IsTrue(string.Equals(s.CommandText, test, StringComparison.OrdinalIgnoreCase),
            "Output CommandText does not match expected result");
    }

    [TestMethod]
    public void TestListFilterInt32()
    {
        const string test = "SELECT * FROM (SELECT * FROM Products) AS [t0] WHERE ([t0].[ProductID] IN (@p0,@p1,@p2)) ORDER BY [ProductName];";
        var q = new SqlSelectBuilder("SELECT * FROM Products ORDER BY [ProductName]");
        var values = new List<int>();

        values.Add(1);
        values.Add(2);
        values.Add(3);

        q.Filters.Add(new SqlListFilter<int>(q.Table, "ProductID", values));
        var s = q.Render();

        Assert.AreEqual(test, s.CommandText, true);
        Assert.AreEqual(3, s.Parameters.Count());
    }

    [TestMethod]
    public void TestListFilterInt32Literal()
    {
        const string test = "SELECT * FROM (SELECT * FROM Products) AS [t0] WHERE ([t0].[ProductID] IN (1,2,3)) ORDER BY [ProductName];";
        var q = new SqlSelectBuilder("SELECT * FROM Products ORDER BY [ProductName]");
        var values = new List<int>();

        values.Add(1);
        values.Add(2);
        values.Add(3);

        q.Filters.Add(new SqlListFilter<int>(q.Table, "ProductID", values) { UseLiteralNumbers = true });
        var s = q.Render();

        Assert.AreEqual(test, s.CommandText, true);
        Assert.AreEqual(0, s.Parameters.Count());
    }

    [TestMethod]
    public void TestListFilterBoolean()
    {
        const string test = "SELECT * FROM (SELECT * FROM Products) AS [t0] WHERE ([t0].[ProductID] IN (1,0,1)) ORDER BY [ProductName];";
        var q = new SqlSelectBuilder("SELECT * FROM Products ORDER BY [ProductName]");
        var values = new List<bool>();

        values.Add(true);
        values.Add(false);
        values.Add(true);

        q.Filters.Add(new SqlListFilter<bool>(q.Table, "ProductID", values));
        var s = q.Render();

        Assert.IsTrue(string.Equals(s.CommandText, test, StringComparison.OrdinalIgnoreCase),
            "Output CommandText does not match expected result");
    }

    [TestMethod]
    public void TestListFilterString()
    {
        const string test = "SELECT * FROM (SELECT * FROM Products) AS [t0] WHERE ([t0].[ProductName] IN (@p0,@p1,@p2)) ORDER BY [ProductName];";
        var q = new SqlSelectBuilder("SELECT * FROM Products ORDER BY [ProductName]");
        var values = new List<string>();

        values.Add("blah");
        values.Add("it's cool");
        values.Add("woohoo");

        q.Filters.Add(new SqlListFilter<string>(q.Table, "ProductName", values));

        var s = q.Render();

        Assert.IsTrue(string.Equals(s.CommandText, test, StringComparison.OrdinalIgnoreCase),
            "Output CommandText does not match expected result");
        Assert.IsTrue(s.Parameters.Any(x => (string)x.Value == values[0]), "Parameter at index 0 value is incorrect");
        Assert.IsTrue(s.Parameters.Any(x => (string)x.Value == values[1]), "Parameter at index 1 value is incorrect");
        Assert.IsTrue(s.Parameters.Any(x => (string)x.Value == values[2]), "Parameter at index 2 value is incorrect");
    }

    [TestMethod]
    public void TestQueryWithoutOrderBy()
    {
        var test = "SELECT * FROM (SELECT TOP 10 * FROM Products) AS [t0];";
        var q = new SqlSelectBuilder("SELECT TOP 10 * FROM Products");

        var s = q.Render();

        Assert.IsTrue(string.Equals(s.CommandText, test, StringComparison.OrdinalIgnoreCase),
            "Output CommandText does not match expected result");
    }

    [TestMethod]
    public void TestJPsQuery()
    {
        var query = @"select top 10 
CorporateKey, 
WidgetName, 
CommunicationStatus, 
DataVolume, 
DataVolumeYesterday, 
DateKey 
from facGadget Inner Join DimWidget on facGadget.GadgetKey = DimWidget.GadgetKey";
        var test = string.Concat("SELECT * FROM (", query, ") AS [t0];");
        var q = new SqlSelectBuilder(query);

        var s = q.Render();

        Assert.IsTrue(string.Equals(s.CommandText, test, StringComparison.OrdinalIgnoreCase),
            "Output CommandText does not match expected result");
    }

    [TestMethod]
    public void TestRank()
    {
        var test1 =
            "SELECT [t0].[WidgetID],RANK() OVER(ORDER BY SUM([t0].[DataVolumeYesterday])) AS [DataVolumeRank] FROM [facGadget] AS [t0] GROUP BY [t0].[WidgetID];";
        var test2 =
            "SELECT [t0].[WidgetID],RANK() OVER(ORDER BY SUM([t0].[DataVolumeYesterday]) DESC) AS [DataVolumeRank] FROM [facGadget] AS [t0] GROUP BY [t0].[WidgetID];";
        var q = new SqlSelectBuilder("facGadget");

        q.Columns.Clear();
        q.Columns.Add(new SqlColumn(q.Table, "WidgetID"));
        q.Columns.Add(new SqlRankColumn(q.Table, "DataVolumeYesterday", SqlAggregate.Sum, "DataVolumeRank", false));
        var s = q.Render();
        Assert.IsTrue(string.Equals(s.CommandText, test1, StringComparison.OrdinalIgnoreCase),
            "Output CommandText does not match expected result");

        q.Columns.Clear();
        q.Columns.Add(new SqlColumn(q.Table, "WidgetID"));
        q.Columns.Add(new SqlRankColumn(q.Table, "DataVolumeYesterday", SqlAggregate.Sum, "DataVolumeRank", true));
        s = q.Render();

        Assert.IsTrue(string.Equals(s.CommandText, test2, StringComparison.OrdinalIgnoreCase),
            "Output CommandText does not match expected result");
    }

    [TestMethod]
    public void TestTableNameFormatting()
    {
        const string test1 = "[table]";
        const string test2 = "[schema].[table]";
        const string test3 = "[server].[schema].[table]";

        var s = SqlTextWriter.FormatTableName("table");
        Assert.AreEqual(s, test1);
        s = SqlTextWriter.FormatTableName("[table]");
        Assert.AreEqual(s, test1);

        s = SqlTextWriter.FormatTableName("schema.table");
        Assert.AreEqual(s, test2);
        s = SqlTextWriter.FormatTableName("schema.[table]");
        Assert.AreEqual(s, test2);
        s = SqlTextWriter.FormatTableName("[schema].[table]");
        Assert.AreEqual(s, test2);

        s = SqlTextWriter.FormatTableName("server.schema.table");
        Assert.AreEqual(s, test3);
        s = SqlTextWriter.FormatTableName("server.[schema].table");
        Assert.AreEqual(s, test3);
        s = SqlTextWriter.FormatTableName("[server].[schema].table");
        Assert.AreEqual(s, test3);
        s = SqlTextWriter.FormatTableName("[server].[schema].[table]");
        Assert.AreEqual(s, test3);
    }

    [TestMethod]
    public void TestJoins()
    {
        const string test1 = "SELECT [t1].[Foo],[t1].[Bar],[t2].[Test1],[t2].[Test2] FROM [Table1] AS [t0] INNER JOIN [dbo].[Table2] AS [t1] ON ([t1].[Table1ID]=[t0].[Table1ID]) LEFT JOIN [dbo].[Table3] AS [t2] ON ([t2].[Table1ID]=[t0].[Table1ID]) AND ([t2].[Table2ID]=[t1].[Table2ID]);";
        const string test2 = "SELECT [t1].[Foo],[t1].[Bar],[t2].[Test1],[t2].[Test2] FROM [Table1] AS [t0] INNER JOIN [dbo].[Table2] AS [t1] ON ([t1].[Table1ID]=[t0].[Table1ID]) LEFT JOIN [dbo].[Table3] AS [t2] ON ([t2].[Table1ID]=[t0].[Table1ID]) AND ([t2].[Table2ID]=[t1].[Table2ID]) WHERE ([t0].[SomeField]=@p0);";
        //string test3 = "SELECT RS.* FROM (SELECT [t0].*,[t1].[Foo],[t1].[Bar],[t2].[Test1],[t2].[Test2] FROM (Table1) AS t0 INNER JOIN [dbo].[Table2] AS t1 ON t1.[Table1ID]=[t0].[Table1ID] LEFT JOIN [dbo].[Table3] AS [t2] ON [t2].[Table1ID]=[t0].[Table1ID] AND [t2].[Table2ID]=t1.[Table2ID]) RS WHERE (RS.[SomeField]=@p0);";
        const string test4 = "SELECT [t1].[Foo],[t1].[Bar],[t2].[Test1],[t2].[Test2],[t3].[Test1] FROM [Table1] AS [t0] INNER JOIN [dbo].[Table2] AS [t1] ON ([t1].[Table1ID]=[t0].[Table1ID]) LEFT JOIN [dbo].[Table3] AS [t2] ON ([t2].[Table1ID]=[t0].[Table1ID]) AND ([t2].[Table2ID]=[t1].[Table2ID]) CROSS JOIN [dbo].[Table4] AS [t3];";

        var q = new SqlSelectBuilder("Table1");

        var table2 = new SqlTable("dbo.Table2");
        q.Joins.AddInner(q.Table, table2,
            new SqlColumnCompareFilter(table2, "Table1ID", SqlOperator.Equal, q.Table, "Table1ID"));
        q.Columns.Add(new SqlColumn(table2, "Foo"));
        q.Columns.Add(new SqlColumn(table2, "Bar"));

        var table3 = new SqlTable("dbo.Table3");
        q.Joins.AddLeft(q.Table, table3,
            new SqlColumnCompareFilter(table3, "Table1ID", SqlOperator.Equal, q.Table, "Table1ID"),
            new SqlColumnCompareFilter(table3, "Table2ID", SqlOperator.Equal, table2, "Table2ID"));
        q.Columns.Add(new SqlColumn(table3, "Test1"));
        q.Columns.Add(new SqlColumn(table3, "Test2"));

        var s = q.Render();
        Assert.IsTrue(string.Equals(s.CommandText, test1, StringComparison.OrdinalIgnoreCase),
            "Output CommandText does not match expected result");

        q.Filters.Add(new SqlCompareFilter<string>(q.Table, "SomeField", SqlOperator.Equal, "Value"));
        s = q.Render();
        Assert.IsTrue(string.Equals(s.CommandText, test2, StringComparison.OrdinalIgnoreCase),
            "Output CommandText does not match expected result");

        q.Filters.Clear();
        var table4 = new SqlTable("dbo.Table4");
        q.Joins.AddCross(q.Table, table4);
        q.Columns.Add(new SqlColumn(table4, "Test1"));
        s = q.Render();
        Assert.IsTrue(string.Equals(s.CommandText, test4, StringComparison.OrdinalIgnoreCase),
            "Output CommandText does not match expected result");
    }

    [TestMethod]
    public void TestCountDistinct()
    {
        const string test1 = "SELECT COUNT(DISTINCT [t0].[Foo]) AS [FooCount],[t0].[Bar] FROM [dbo].[FooBar] AS [t0] GROUP BY [t0].[Bar];";
        var q = new SqlSelectBuilder("dbo.FooBar");

        q.Columns.Add(new SqlColumn(q.Table, "Foo", "FooCount") { Aggregate = SqlAggregate.CountDistinct });
        q.Columns.Add(new SqlColumn(q.Table, "Bar"));

        var stmt = q.Render();

        Assert.AreEqual(test1, stmt.CommandText, true);
    }

    [TestMethod]
    public void TestDerivedTableWrapsWithParens()
    {
        var table = new SqlDerivedTable("SELECT * FROM [Foo]");
        var args = new SqlBuildArguments();
        var writer = new SqlTextWriter();

        args.AssignAlias(table);

        ((ISqlTable)table).Render(writer, args);

        Assert.IsTrue(string.Equals(writer.ToString(), "(SELECT * FROM [Foo]) AS [" + args.TableName(table) + "]"));
    }

    [TestMethod]
    public void TestDerivedTableRemovesSemicolons()
    {
        var table = new SqlDerivedTable("SELECT * FROM [Foo];");
        var args = new SqlBuildArguments();
        var writer = new SqlTextWriter();

        args.AssignAlias(table);

        ((ISqlTable)table).Render(writer, args);

        Assert.AreEqual("(SELECT * FROM [Foo]) AS [" + args.TableName(table) + "]", writer.ToString());
    }


    [TestMethod]
    public void TestExistFilter()
    {
        var outerQuery = new SqlSelectBuilder("SELECT * FROM Stuff ORDER BY [WidgetName];");
        var innerQuery = new SqlSelectBuilder("[Foo]");
        innerQuery.Columns.Add(new SqlLiteralColumn<int>(1));

        outerQuery.Filters.Add(new SqlExistFilter(innerQuery));

        var stmt = outerQuery.Render();

        Assert.AreEqual(
            "SELECT * FROM (SELECT * FROM Stuff) AS [t0] WHERE (EXISTS (SELECT 1 FROM [Foo] AS [t1])) ORDER BY [WidgetName];",
            stmt.CommandText);
    }

    [TestMethod]
    public void TestLiteralColumn()
    {
        var query = new SqlSelectBuilder("[Foo]");

        query.Columns.Add(new SqlLiteralColumn<int>(1));
        query.Columns.Add(new SqlLiteralColumn<string>("2", "Two"));

        var stmt = query.Render();

        Assert.AreEqual("SELECT 1,'2' AS [Two] FROM [Foo] AS [t0];", stmt.CommandText);
    }

    [TestMethod]
    public void TestRawColumn()
    {
        var query = new SqlSelectBuilder("[Foo]");

        query.Columns.Add(new SqlRawColumn("NULL", "Nothing"));
        query.Columns.Add(new SqlRawColumn("1234"));

        var stmt = query.Render();

        Assert.AreEqual("SELECT NULL AS [Nothing],1234 FROM [Foo] AS [t0];", stmt.CommandText);
    }

    [TestMethod]
    public void TestRawFilter()
    {
        const string test = "SELECT * FROM [DimWidget] AS [t0] WHERE ((SELECT COUNT(*) FROM dbo.WidgetComment WHERE WidgetID = [t0].WidgetID) > @p0);";
        var q = new SqlSelectBuilder("DimWidget");

        q.Filters.Add(new SqlRawFilter("(SELECT COUNT(*) FROM dbo.WidgetComment WHERE WidgetID = {0}.WidgetID) > {1}",
            q.Table, new DbParameterValue
            {
                Value = 3,
                DbType = DbType.Int32
            }));

        var s = q.Render();

        Assert.AreEqual(1, s.Parameters.Count, "Parameter count should be 1.");
        Assert.AreEqual(test, s.CommandText, true);
    }

    [TestMethod]
    public void TestSubQueryCountFilter()
    {
        const string test =
            "SELECT * FROM [DimWidget] AS [t0] WHERE ((SELECT COUNT([t1].[WidgetCommentID]) AS [Cnt] FROM [dbo].[WidgetComment] AS [t1] WHERE ([t0].[WidgetID]=[t1].[WidgetID])) > @p0);";
        //             SELECT * FROM [DimWidget] AS [t0] WHERE ((SELECT COUNT([t1].[WidgetCommentID]) AS [Cnt] FROM [dbo].[WidgetComment] AS [t1] WHERE ([t0].[WidgetID]=[t1].[WidgetID])) > @p0);
        var q = new SqlSelectBuilder("DimWidget");

        var widgetComment = SqlTableBase.Create("dbo.WidgetComment");

        var sqf = new SqlCountFilter(q.Table, widgetComment, "WidgetCommentID", SqlOperator.GreaterThan, 3);
        sqf.SubQueryFilters.Add(new SqlColumnCompareFilter(q.Table, "WidgetID", SqlOperator.Equal, widgetComment));
        q.Filters.Add(sqf);

        var s = q.Render();

        Assert.IsTrue(s.Parameters.Count > 0, "Parameter count should be 1.");
        Assert.AreEqual(test, s.CommandText, true);
    }

    [TestMethod]
    public void TestPagingOptionsZeroOffset()
    {
        const string test =
            "SELECT * FROM [DimWidget] AS [t0] ORDER BY [WidgetID] OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY;";
        //             SELECT * FROM [DimWidget] AS [t0] FROM [dbo].[WidgetComment] AS [t1] WHERE ([t0].[WidgetID]=[t1].[WidgetID])) > @p0);
        var q = new SqlSelectBuilder("DimWidget");

        q.OrderBy.Add(new SqlOrderColumn { ColumnName = "WidgetID" });
        q.PagingOptions = new SqlPagingOptions
        {
            Limit = 10,
            Offset = 0
        };

        var s = q.Render();

        Assert.AreEqual(test, s.CommandText, true);
    }

    [TestMethod]
    public void TestPagingOptions()
    {
        const string test =
            "SELECT * FROM [DimWidget] AS [t0] ORDER BY [WidgetID] OFFSET 50 ROWS FETCH NEXT 10 ROWS ONLY;";
        //             SELECT * FROM [DimWidget] AS [t0] FROM [dbo].[WidgetComment] AS [t1] WHERE ([t0].[WidgetID]=[t1].[WidgetID])) > @p0);
        var q = new SqlSelectBuilder("DimWidget");

        q.OrderBy.Add(new SqlOrderColumn { ColumnName = "WidgetID" });
        q.PagingOptions = new SqlPagingOptions
        {
            Limit = 10,
            Offset = 50
        };

        var s = q.Render();

        Assert.AreEqual(test, s.CommandText, true);
    }

    [TestMethod]
    public void TestPagingOffsetOnly()
    {
        const string test = "SELECT * FROM [DimWidget] AS [t0] ORDER BY [WidgetID] OFFSET 50 ROWS;";
        //             SELECT * FROM [DimWidget] AS [t0] FROM [dbo].[WidgetComment] AS [t1] WHERE ([t0].[WidgetID]=[t1].[WidgetID])) > @p0);
        var q = new SqlSelectBuilder("DimWidget");

        q.OrderBy.Add(new SqlOrderColumn { ColumnName = "WidgetID" });
        q.PagingOptions = new SqlPagingOptions
        {
            Offset = 50
        };

        var s = q.Render();

        Assert.AreEqual(test, s.CommandText, true);
    }

    [TestMethod]
    public void Extensions_RenderBatch()
    {
        const string test =
            "SELECT * FROM [dbo].[DimWidget] AS [t0] WHERE ([t0].[WidgetID]=@p0);\r\nSELECT * FROM [dbo].[DimWidget] AS [t1] WHERE ([t1].[WidgetID]=@p1);\r\nSELECT * FROM [dbo].[DimWidget] AS [t2] WHERE ([t2].[WidgetID]=@p2);\r\n";
        var collection = new List<SqlSelectBuilder>();

        var q1 = new SqlSelectBuilder("dbo.DimWidget");
        q1.Filters.Add(new SqlCompareFilter<int>(q1.Table, "WidgetID", SqlOperator.Equal, 1));
        collection.Add(q1);

        var q2 = new SqlSelectBuilder("dbo.DimWidget");
        q2.Filters.Add(new SqlCompareFilter<int>(q2.Table, "WidgetID", SqlOperator.Equal, 2));
        collection.Add(q2);

        var q3 = new SqlSelectBuilder("dbo.DimWidget");
        q3.Filters.Add(new SqlCompareFilter<int>(q3.Table, "WidgetID", SqlOperator.Equal, 3));
        collection.Add(q3);

        var stmt = collection.RenderBatch();

        Assert.AreEqual(3, stmt.Count);
        Assert.AreEqual(3, stmt.Parameters.Count);
        Assert.AreEqual(1, stmt.Parameters.First().Value);
        Assert.AreEqual(2, stmt.Parameters.Skip(1).First().Value);
        Assert.AreEqual(3, stmt.Parameters.Last().Value);
        Assert.AreEqual(test, stmt.CommandText, true);
    }
}