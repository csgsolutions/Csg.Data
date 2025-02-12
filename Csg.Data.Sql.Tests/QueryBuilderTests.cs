using System.Data;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csg.Data.Sql.Tests;

[TestClass]
public class QueryBuilderTests
{
    static QueryBuilderTests()
    {
        DbQueryBuilder.GenerateFormattedSql = false;
    }

    [TestMethod]
    public void TestSelect()
    {
        var test = "SELECT * FROM [dbo].[Contact] AS [t0];";
        var builder = new SqlSelectBuilder();
        builder.Table = new SqlTable("dbo.Contact");
        var stmt = builder.Render();

        Assert.IsNotNull(stmt.CommandText);
        Assert.AreEqual(stmt.CommandText, test);
    }

    [TestMethod]
    public void TestSelectColumns()
    {
        var test =
            "SELECT [t0].[LastName],[t0].[FirstName],[t0].[FullName] AS [DisplayName] FROM [dbo].[Contact] AS [t0];";
        var builder = new SqlSelectBuilder();

        builder.Table = new SqlTable("dbo.Contact");
        builder.Columns.Add(new SqlColumn(builder.Table, "LastName"));
        builder.Columns.Add(new SqlColumn(builder.Table, "FirstName"));
        builder.Columns.Add(new SqlColumn(builder.Table, "FullName", "DisplayName"));

        var stmt = builder.Render();

        Assert.IsNotNull(stmt.CommandText);
        Assert.AreEqual(stmt.CommandText, test);
    }

    [TestMethod]
    public void TestSelectOrderBy()
    {
        var test =
            "SELECT [t0].[LastName],[t0].[FirstName] FROM [dbo].[Contact] AS [t0] ORDER BY [LastName] ASC,[FirstName] DESC;";
        var builder = new SqlSelectBuilder();

        builder.Table = new SqlTable("dbo.Contact");
        builder.Columns.Add(new SqlColumn(builder.Table, "LastName"));
        builder.Columns.Add(new SqlColumn(builder.Table, "FirstName"));
        builder.OrderBy.Add(new SqlOrderColumn { ColumnName = "LastName", SortDirection = DbSortDirection.Ascending });
        builder.OrderBy.Add(new SqlOrderColumn
            { ColumnName = "FirstName", SortDirection = DbSortDirection.Descending });

        var stmt = builder.Render();

        Assert.IsNotNull(stmt.CommandText);
        Assert.AreEqual(stmt.CommandText, test);
    }

    [TestMethod]
    public void TestSelectFilters()
    {
        var test =
            "SELECT [t0].[LastName],[t0].[FirstName] FROM [dbo].[Contact] AS [t0] WHERE ([t0].[LastName]=@p0) AND ([t0].[FirstName]>@p1) ORDER BY [LastName] ASC,[FirstName] DESC;";
        var builder = new SqlSelectBuilder();

        builder.Table = new SqlTable("dbo.Contact");
        builder.Columns.Add(new SqlColumn(builder.Table, "LastName"));
        builder.Columns.Add(new SqlColumn(builder.Table, "FirstName"));
        builder.OrderBy.Add(new SqlOrderColumn { ColumnName = "LastName", SortDirection = DbSortDirection.Ascending });
        builder.OrderBy.Add(new SqlOrderColumn
            { ColumnName = "FirstName", SortDirection = DbSortDirection.Descending });
        builder.Filters.Add(new SqlCompareFilter<string>(builder.Table, "LastName", SqlOperator.Equal, "Buchanan"));
        builder.Filters.Add(new SqlCompareFilter<string>(builder.Table, "FirstName", SqlOperator.GreaterThan, "a"));

        var stmt = builder.Render();

        Assert.IsNotNull(stmt.CommandText);
        Assert.AreEqual(stmt.CommandText, test);
        Assert.AreEqual(stmt.Parameters.ToList()[0].Value, "Buchanan");
        Assert.AreEqual(stmt.Parameters.ToList()[1].Value, "a");
    }

    [TestMethod]
    public void TestSelectGroupBy()
    {
        var test =
            "SELECT [t0].[LastName],COUNT([t0].[FirstName]) AS [Count] FROM [dbo].[Contact] AS [t0] GROUP BY [t0].[LastName] ORDER BY [Count] DESC;";
        var builder = new SqlSelectBuilder();

        builder.Table = new SqlTable("dbo.Contact");
        builder.Columns.Add(new SqlColumn(builder.Table, "LastName"));
        builder.Columns.Add(new SqlColumn(builder.Table, "FirstName", "Count") { Aggregate = SqlAggregate.Count });
        builder.OrderBy.Add(new SqlOrderColumn { ColumnName = "Count", SortDirection = DbSortDirection.Descending });

        var stmt = builder.Render();

        Assert.IsNotNull(stmt.CommandText);
        Assert.AreEqual(stmt.CommandText, test);
    }

    [TestMethod]
    public void TestJoinSelect()
    {
        var test =
            "SELECT [t0].[FooID],[t1].[Name] AS [BarName] FROM [dbo].[Foo] AS [t0] INNER JOIN [dbo].[Bar] AS [t1] ON ([t0].[BarID]=[t1].[BarID]);";
        var builder = new SqlSelectBuilder();

        var foo = new SqlTable("dbo.Foo");
        var bar = new SqlTable("dbo.Bar");

        builder.Joins.AddInner(foo, bar, "BarID");

        builder.Table = foo;
        builder.Columns.Add(new SqlColumn(foo, "FooID"));
        builder.Columns.Add(new SqlColumn(bar, "Name", "BarName"));

        var stmt = builder.Render();

        Assert.IsNotNull(stmt.CommandText);
        Assert.AreEqual(stmt.CommandText, test);
    }

    [TestMethod]
    public void TestSubQueryFilter()
    {
        var table = "SELECT * FROM dbo.[Foo]";
        var subQuery = "SELECT [SubFooID],[Blah] FROM dbo.[Bar]";
        var test =
            "SELECT * FROM (SELECT * FROM dbo.[Foo]) AS [t0] WHERE ([t0].[FooID] IN (SELECT [t1].[SubFooID] FROM (SELECT [SubFooID],[Blah] FROM dbo.[Bar]) AS [t1] WHERE ([t1].[Blah]=@p0)));";
        var builder = new SqlSelectBuilder(table);

        var filter = new SqlSubQueryFilter(builder.Table, subQuery)
        {
            ColumnName = "FooID",
            SubQueryColumn = "SubFooID"
        };

        filter.SubQueryFilters.Add(filter.SubQueryTable, "Blah", SqlOperator.Equal, DbType.String, "Test123");

        builder.Filters.Add(filter);

        var stmt = builder.Render();

        Assert.AreEqual(stmt.CommandText, test);
        Assert.AreEqual(stmt.Parameters.Single().Value, "Test123");
    }

    [TestMethod]
    public void TestSelectDistinctColumns()
    {
        var test =
            "SELECT DISTINCT [t0].[LastName],[t0].[FirstName],[t0].[FullName] AS [DisplayName] FROM [dbo].[Contact] AS [t0];";
        var builder = new SqlSelectBuilder();

        builder.Table = new SqlTable("dbo.Contact");
        builder.Columns.Add(new SqlColumn(builder.Table, "LastName"));
        builder.Columns.Add(new SqlColumn(builder.Table, "FirstName"));
        builder.Columns.Add(new SqlColumn(builder.Table, "FullName", "DisplayName"));
        builder.SelectDistinct = true;

        var stmt = builder.Render();

        Assert.IsNotNull(stmt.CommandText);
        Assert.AreEqual(stmt.CommandText, test);
    }

    [TestMethod]
    public void TestJoinMultipleFilterCollectionsWithOrLogic()
    {
        var expectSql =
            "SELECT * FROM [dbo].[Product] AS [t0] WHERE ([t0].[IsActive]=@p0) AND ((([t0].[ProductCategoryID]=@p1) AND ([t0].[SupplierID]=@p2) AND ([t0].[ThingName] IN (@p3,@p4,@p5))) OR (([t0].[ProductCategoryID]=@p6) AND ([t0].[SupplierID]=@p7) AND ([t0].[ThingName] IN (@p8,@p9,@p10))));";
        var builder = new SqlSelectBuilder("dbo.Product");

        var listOfThings1 = new[] { "a", "b", "c" };
        var listOfThings2 = new[] { "d", "e", "f" };

        builder.Filters.Add(builder.Table, "IsActive", SqlOperator.Equal, DbType.Boolean, true);

        var productCategories = new SqlFilterCollection { Logic = SqlLogic.Or };

        var productCategory1 = new SqlFilterCollection();
        productCategory1.Add(builder.Table, "ProductCategoryID", SqlOperator.Equal, DbType.Int32, 123);
        productCategory1.Add(builder.Table, "SupplierID", SqlOperator.Equal, DbType.Int32, 456);
        productCategory1.Add(new SqlListFilter<string>(builder.Table, "ThingName", listOfThings1));

        var productCategory2 = new SqlFilterCollection();
        productCategory2.Add(builder.Table, "ProductCategoryID", SqlOperator.Equal, DbType.Int32, 123);
        productCategory2.Add(builder.Table, "SupplierID", SqlOperator.Equal, DbType.Int32, 456);
        productCategory2.Add(new SqlListFilter<string>(builder.Table, "ThingName", listOfThings2));

        productCategories.Add(productCategory1);
        productCategories.Add(productCategory2);
        builder.Filters.Add(productCategories);

        var stmt = builder.Render();
        Assert.IsNotNull(stmt.CommandText);
        Assert.AreEqual(expectSql, stmt.CommandText);
    }

    [TestMethod]
    public void TestPrefix()
    {
        var test = "SET ROWCOUNT 10;SELECT [t0].[FirstName] FROM [dbo].[Contact] AS [t0];";
        var builder = new SqlSelectBuilder();

        builder.Table = new SqlTable("dbo.Contact");
        builder.Columns.Add(new SqlColumn(builder.Table, "FirstName"));
        builder.Prefix = "SET ROWCOUNT 10";

        var stmt = builder.Render();

        Assert.IsNotNull(stmt.CommandText);
        Assert.AreEqual(stmt.CommandText, test);
    }

    [TestMethod]
    public void TestSuffix()
    {
        var test = "SELECT [t0].[FirstName] FROM [dbo].[Contact] AS [t0];A Suffix Statement Goes Here;";
        var builder = new SqlSelectBuilder();

        builder.Table = new SqlTable("dbo.Contact");
        builder.Columns.Add(new SqlColumn(builder.Table, "FirstName"));
        builder.Suffix = "A Suffix Statement Goes Here";

        var stmt = builder.Render();

        Assert.IsNotNull(stmt.CommandText);
        Assert.AreEqual(stmt.CommandText, test);
    }
}