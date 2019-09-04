using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;
using Csg.Data;
using Csg.Data.Sql;
using System.Data.Common;

namespace TestProject
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class SqlSelectBuilderTests
    {
        [TestMethod]
        public void TestColumnFormatting()
        {
            var col1 = SqlTextWriter.FormatSqlServerIdentifier("[FieldName]");
            var col2 = SqlTextWriter.FormatSqlServerIdentifier("FieldName");            

            Assert.AreEqual(col1.ToString(), "[FieldName]");
            Assert.AreEqual(col1.ToString(),col2.ToString());
        }

        [TestMethod]
        public void TestColumnAliasing()
        {
            string test = "SELECT [t0].[Foo] AS [Bar] FROM [TableName] AS [t0];";

            var q = new SqlSelectBuilder("TableName");

            q.SelectColumns.Add(new SqlColumn(q.Table, "Foo", "Bar"));

            var s = q.Render();

            Assert.AreEqual(test, s.CommandText);
        }

        [TestMethod]
        public void TestExpressionColumn()
        {
            string test = "SELECT (Foo + 1) AS [Bar] FROM [TableName] AS [t0];";

            var q = new SqlSelectBuilder("TableName");
            var col = new SqlExpressionSelectColumn(q.Table, "Foo + 1", "Bar");
            q.SelectColumns.Add(col);
            var s = q.Render();

            Assert.AreEqual(s.CommandText, test);
        }

        [TestMethod]
        public void TestParse()
        {
            string test = "SELECT * FROM (SELECT * FROM DimWidget WHERE WidgetID LIKE 'a%' ) AS [t0] ORDER BY [WidgetName];";
            var q = new SqlSelectBuilder("SELECT * FROM DimWidget WHERE WidgetID LIKE 'a%' ORDER BY [WidgetName];");
            SqlStatement s;
                                   
            s = q.Render();

            Assert.IsTrue(string.Equals(s.CommandText, test, StringComparison.OrdinalIgnoreCase), "Output CommandText does not match expected result");          
        }

        [TestMethod]
        public void TestParseOrderBy()
        {
            string expected = "SELECT * FROM (SELECT * FROM Report_Nrt ) AS [t0] ORDER BY [DivisionName];";
            string query = "SELECT * FROM Report_Nrt ORDER BY DivisionName";
            SqlSelectBuilder q = new SqlSelectBuilder(query);
            SqlStatement s;
                                   
            s = q.Render();

            Assert.IsTrue(string.Equals(s.CommandText, expected, StringComparison.OrdinalIgnoreCase), "Output CommandText does not match expected result");          
        }

        [TestMethod]
        public void TestSort()
        {
            string test = "SELECT * FROM (SELECT * FROM DimWidget WHERE WidgetID LIKE 'a%' ) AS [t0] ORDER BY [WidgetName],[WidgetID];";
            SqlSelectBuilder q = new SqlSelectBuilder("SELECT * FROM DimWidget WHERE WidgetID LIKE 'a%' ORDER BY [WidgetName];");

            q.OrderBy.Add("[WidgetID]");

            SqlStatement s;

            s = q.Render();

            Assert.IsTrue(string.Equals(s.CommandText, test, StringComparison.OrdinalIgnoreCase), "Output CommandText does not match expected result");
        }
        
        [TestMethod]
        public void TestEqualFilter()
        {
            string test = "SELECT * FROM (SELECT WidgetName,WidgetID FROM DimWidget ) AS [t0] WHERE ([t0].[WidgetID]=@p0) ORDER BY [WidgetName];";
            SqlSelectBuilder q = new SqlSelectBuilder("SELECT WidgetName,WidgetID FROM DimWidget ORDER BY [WidgetName];");
            SqlStatement s;

            q.Filters.Add(q.Table, "WidgetID", SqlOperator.Equal, System.Data.DbType.Int32, 10);

            s = q.Render();

                        
            Assert.IsTrue((s.Parameters.Count>0),"Parameter count should be 1.");            
            Assert.IsTrue((s.Parameters.Count(x => (int)x.Value == 10) == 1), "Parameter value not correct");            
            Assert.IsTrue(string.Equals(s.CommandText, test, StringComparison.OrdinalIgnoreCase), "Output CommandText does not match expected result");

            var sqlCmd = new System.Data.SqlClient.SqlCommand();
            s.SetupCommand(sqlCmd);
            Assert.AreEqual(1, sqlCmd.Parameters.Count);
            Assert.IsInstanceOfType(sqlCmd.Parameters[0], typeof(System.Data.SqlClient.SqlParameter));
        }

        [TestMethod]
        public void TestNotEqualFilter()
        {
            string test = "SELECT * FROM (SELECT WidgetName,WidgetID FROM DimWidget ) AS [t0] WHERE ([t0].[OfficeKey]<>@p0) ORDER BY [WidgetName];";
            SqlSelectBuilder q = new SqlSelectBuilder("SELECT WidgetName,WidgetID FROM DimWidget ORDER BY [WidgetName];");
            SqlStatement s;
            SqlFilterCollection filters = new SqlFilterCollection();

            q.Filters.Add(q.Table, "OfficeKey", SqlOperator.NotEqual, System.Data.DbType.Int32, 10);

            s = q.Render();

            Assert.IsTrue((s.Parameters.Count > 0), "Parameter count should be 1.");
            Assert.IsTrue(((int)s.Parameters.Single().Value == 10), "Parameter value not correct");
            Assert.IsTrue(string.Equals(s.CommandText, test, StringComparison.OrdinalIgnoreCase), "Output CommandText does not match expected result");
        }

        [TestMethod]
        public void TestGreaterThanFilter()
        {
            string test = "SELECT * FROM (SELECT WidgetName,WidgetID FROM DimWidget ) AS [t0] WHERE ([t0].[OfficeKey]>@p0) ORDER BY [WidgetName];";
            SqlSelectBuilder q = new SqlSelectBuilder("SELECT WidgetName,WidgetID FROM DimWidget ORDER BY [WidgetName];");
            SqlStatement s;
            SqlFilterCollection filters = new SqlFilterCollection();

            q.Filters.Add(q.Table, "OfficeKey", SqlOperator.GreaterThan, System.Data.DbType.Int32, 10);
            s = q.Render();

            Assert.IsTrue((s.Parameters.Count > 0), "Parameter count should be 1.");
            Assert.IsTrue(((int)s.Parameters.Single().Value == 10), "Parameter value not correct");
            Assert.IsTrue(string.Equals(s.CommandText, test, StringComparison.OrdinalIgnoreCase), "Output CommandText does not match expected result");
        }

        [TestMethod]
        public void TestLessThanFilter()
        {
            string test = "SELECT * FROM (SELECT WidgetName,WidgetID FROM DimWidget ) AS [t0] WHERE ([t0].[OfficeKey]<@p0) ORDER BY [WidgetName];";
            SqlSelectBuilder q = new SqlSelectBuilder("SELECT WidgetName,WidgetID FROM DimWidget ORDER BY [WidgetName];");
            SqlStatement s;
            SqlFilterCollection filters = new SqlFilterCollection();

            q.Filters.Add(q.Table, "OfficeKey", SqlOperator.LessThan, System.Data.DbType.Int32, 10);

            s = q.Render();

            Assert.IsTrue((s.Parameters.Count > 0), "Parameter count should be 1.");
            Assert.IsTrue(((int)s.Parameters.Single().Value == 10), "Parameter value not correct");
            Assert.IsTrue(string.Equals(s.CommandText, test, StringComparison.OrdinalIgnoreCase), "Output CommandText does not match expected result");
        }

        [TestMethod]
        public void TestGreaterThanOrEqualFilter()
        {
            string test = "SELECT * FROM (SELECT WidgetName,WidgetID FROM DimWidget ) AS [t0] WHERE ([t0].[OfficeKey]>=@p0) ORDER BY [WidgetName];";
            SqlSelectBuilder q = new SqlSelectBuilder("SELECT WidgetName,WidgetID FROM DimWidget ORDER BY [WidgetName];");
            SqlStatement s;
            SqlFilterCollection filters = new SqlFilterCollection();

            q.Filters.Add(q.Table, "OfficeKey", SqlOperator.GreaterThanOrEqual, System.Data.DbType.Int32, 10);

            s = q.Render();

            Assert.IsTrue((s.Parameters.Count > 0), "Parameter count should be 1.");
            Assert.IsTrue(((int)s.Parameters.Single().Value == 10), "Parameter value not correct");
            Assert.IsTrue(string.Equals(s.CommandText, test, StringComparison.OrdinalIgnoreCase), "Output CommandText does not match expected result");
        }

        [TestMethod]
        public void TestLessThanOrEqualFilter()
        {
            string test = "SELECT * FROM (SELECT WidgetName,WidgetID FROM DimWidget ) AS [t0] WHERE ([t0].[OfficeKey]<=@p0) ORDER BY [WidgetName];";
            SqlSelectBuilder q = new SqlSelectBuilder("SELECT WidgetName,WidgetID FROM DimWidget ORDER BY [WidgetName];");
            SqlStatement s;
            SqlFilterCollection filters = new SqlFilterCollection();

            q.Filters.Add(q.Table, "OfficeKey", SqlOperator.LessThanOrEqual, System.Data.DbType.Int32, 10);

            s = q.Render();

            Assert.IsTrue((s.Parameters.Count > 0), "Parameter count should be 1.");
            Assert.IsTrue(((int)s.Parameters.Single().Value == 10), "Parameter value not correct");
            Assert.IsTrue(string.Equals(s.CommandText, test, StringComparison.OrdinalIgnoreCase), "Output CommandText does not match expected result");
        }

        [TestMethod]
        public void TestDateTimeFilter()
        {
            string test = "SELECT * FROM (SELECT WidgetName,WidgetID FROM DimWidget ) AS [t0] WHERE ([t0].[CreateDate]>=@p0 AND [t0].[CreateDate]<=@p1) ORDER BY [WidgetName];";
            SqlSelectBuilder q = new SqlSelectBuilder("SELECT WidgetName,WidgetID FROM DimWidget ORDER BY [WidgetName];");
            SqlStatement s;
            SqlFilterCollection filters = new SqlFilterCollection();
            DateTime beginDate = DateTime.Parse("2011-01-01 05:00");
            DateTime endDate = DateTime.Parse("2011-01-01 23:30");

            q.Filters.Add(new SqlDateTimeFilter(q.Table, "CreateDate", beginDate, endDate));

            s = q.Render();

            Assert.IsTrue((s.Parameters.Count > 0), "Parameter count should be 1.");
            Assert.IsTrue(((DateTime)s.Parameters.ToList()[0].Value == beginDate), "Begin date does not match parameter");
            Assert.IsTrue(((DateTime)s.Parameters.ToList()[1].Value == endDate), "End date does not match parameter");
            Assert.AreEqual(test, s.CommandText, "Output CommandText does not match expected result");
        }

        [TestMethod]
        public void TestDateFilter()
        {
            string expectedSql = "SELECT * FROM (SELECT WidgetName,WidgetID FROM DimWidget ) AS [t0] WHERE (CAST([t0].[CreateDate] AS date)>=@p0 AND CAST([t0].[CreateDate] AS date)<=@p1) ORDER BY [WidgetName];";
            //                    SELECT * FROM (SELECT WidgetName,WidgetID FROM DimWidget ) AS [t0] WHERE (CAST([t0].[CreateDate] AS date)>=@p0 AND CAST([t0].[CreateDate] AS date)<=@p1) ORDER BY [WidgetName],[WidgetName];

            SqlSelectBuilder q = new SqlSelectBuilder("SELECT WidgetName,WidgetID FROM DimWidget ORDER BY [WidgetName];");
            SqlStatement s;
            SqlFilterCollection filters = new SqlFilterCollection();
            DateTime beginDate = DateTime.Parse("2011-01-01 05:00");
            DateTime endDate = DateTime.Parse("2011-01-01 23:30");

            var dateFilter = new SqlDateFilter(q.Table, "CreateDate", beginDate, endDate);

            q.Filters.Add(dateFilter);

            s = q.Render();

            Assert.AreEqual(beginDate.Date, dateFilter.BeginDate, "filter value should not have time");
            Assert.AreEqual(endDate.Date, dateFilter.EndDate, "filter value should not have time");
            Assert.IsTrue((s.Parameters.Count == 2), "Parameter count should be 2.");
            Assert.IsTrue(s.Parameters.All(x => (DateTime)x.Value == ((DateTime)x.Value).Date), "Date params should not have time in parameter.");
            Assert.AreEqual(expectedSql, s.CommandText, "Output CommandText does not match expected result");
        }

        [TestMethod]
        public void TestNullFilter()
        {
            string test = "SELECT * FROM (SELECT WidgetName,WidgetID FROM DimWidget ) AS [t0] WHERE ([t0].[Foreman] IS NULL) ORDER BY [WidgetName];";
            SqlSelectBuilder q = new SqlSelectBuilder("SELECT WidgetName,WidgetID FROM DimWidget ORDER BY [WidgetName];");
            SqlStatement s;
            SqlFilterCollection filters = new SqlFilterCollection();

            q.Filters.Add(new SqlNullFilter(q.Table, "Foreman", true));

            s = q.Render();

            Assert.IsTrue((s.Parameters.Count <= 0), "Parameter count should be 0.");            
            Assert.IsTrue(string.Equals(s.CommandText, test, StringComparison.OrdinalIgnoreCase), "Output CommandText does not match expected result");

            test = "SELECT * FROM (SELECT WidgetName,WidgetID FROM DimWidget ) AS [t0] WHERE ([t0].[Foreman] IS NOT NULL) ORDER BY [WidgetName];";
            q.Filters.Clear();
            q.Filters.Add(new SqlNullFilter(q.Table, "Foreman", false));
            s = q.Render();
            Assert.IsTrue(string.Equals(s.CommandText, test, StringComparison.OrdinalIgnoreCase), "Output CommandText does not match expected result");
        }

        [TestMethod]
        public void TestStringFilter()
        {
            string test = "SELECT * FROM (SELECT WidgetName,WidgetID FROM DimWidget ) AS [t0] WHERE ([t0].[Foreman] LIKE @p0) ORDER BY [WidgetName];";
            SqlSelectBuilder q = new SqlSelectBuilder("SELECT WidgetName,WidgetID FROM DimWidget ORDER BY [WidgetName];");
            SqlStatement s;
            SqlFilterCollection filters = new SqlFilterCollection();

            q.Filters.Add(new SqlStringMatchFilter(q.Table, "Foreman", SqlWildcardDecoration.BeginsWith, "B"));
            s = q.Render();

            Assert.IsTrue(string.Equals(s.CommandText, test, StringComparison.OrdinalIgnoreCase), "Output CommandText does not match expected result");

            Assert.IsTrue((s.Parameters.Count == 1), "Parameter count should be 1.");
            Assert.IsTrue(((string)s.Parameters.Single().Value == "B%"), "Parameter value is incorrect");

            q.Filters.Clear();
            q.Filters.Add(new SqlStringMatchFilter(q.Table, "Foreman", SqlWildcardDecoration.EndsWith, "B"));
            s = q.Render();

            Assert.IsTrue((s.Parameters.Count == 1), "Parameter count should be 1.");
            Assert.IsTrue(((string)s.Parameters.Single().Value == "%B"), "Parameter value is incorrect");

            q.Filters.Clear();
            q.Filters.Add(new SqlStringMatchFilter(q.Table, "Foreman", SqlWildcardDecoration.Contains, "B"));
            s = q.Render();

            Assert.IsTrue((s.Parameters.Count == 1), "Parameter count should be 1.");
            Assert.IsTrue(((string)s.Parameters.Single().Value == "%B%"), "Parameter value is incorrect");            
        }

        [TestMethod]
        public void TestGroupBy()
        {
            string test = "SELECT [t0].[OfficeKey],COUNT([t0].[WidgetID]) AS [MeterCount] FROM (SELECT WidgetName,WidgetID,OfficeKey FROM DimWidget ) AS [t0] GROUP BY [t0].[OfficeKey] ORDER BY [WidgetName];";
            SqlSelectBuilder q = new SqlSelectBuilder("SELECT WidgetName,WidgetID,OfficeKey FROM DimWidget ORDER BY [WidgetName];");
            SqlStatement s;
            var cols = new List<ISqlColumn>();

            q.SelectColumns.Add(new SqlColumn(q.Table,"OfficeKey"));
            q.SelectColumns.Add(new SqlColumn(q.Table, "WidgetID", "MeterCount") { Aggregate = SqlAggregate.Count });
                       
            s = q.Render();

            Assert.IsTrue(string.Equals(s.CommandText, test, StringComparison.OrdinalIgnoreCase), "Output CommandText does not match expected result");
        }

        [TestMethod]
        public void TestListFilterInt32()
        {
            string test = "SELECT * FROM (SELECT * FROM Products ) AS [t0] WHERE ([t0].[ProductID] IN (@p0,@p1,@p2)) ORDER BY [ProductName];";
            SqlSelectBuilder q = new SqlSelectBuilder("SELECT * FROM Products ORDER BY [ProductName]");
            SqlStatement s;
            SqlFilterCollection filters = new SqlFilterCollection();
            List<int> values = new List<int>();
            
            values.Add(1);
            values.Add(2);
            values.Add(3);                     
                        
            q.Filters.Add(new SqlListFilter<int>(q.Table, "ProductID", values));   
            s = q.Render();

            Assert.AreEqual(test, s.CommandText, true);
            Assert.AreEqual(3, s.Parameters.Count());            
        }

        [TestMethod]
        public void TestListFilterInt32Literal()
        {
            string test = "SELECT * FROM (SELECT * FROM Products ) AS [t0] WHERE ([t0].[ProductID] IN (1,2,3)) ORDER BY [ProductName];";
            SqlSelectBuilder q = new SqlSelectBuilder("SELECT * FROM Products ORDER BY [ProductName]");
            SqlStatement s;
            SqlFilterCollection filters = new SqlFilterCollection();
            List<int> values = new List<int>();

            values.Add(1);
            values.Add(2);
            values.Add(3);

            q.Filters.Add(new SqlListFilter<int>(q.Table, "ProductID", values) { UseLiteralNumbers = true });
            s = q.Render();

            Assert.AreEqual(test, s.CommandText, true);
            Assert.AreEqual(0, s.Parameters.Count());
        }

        [TestMethod]
        public void TestListFilterBoolean()
        {
            string test = "SELECT * FROM (SELECT * FROM Products ) AS [t0] WHERE ([t0].[ProductID] IN (1,0,1)) ORDER BY [ProductName];";
            SqlSelectBuilder q = new SqlSelectBuilder("SELECT * FROM Products ORDER BY [ProductName]");
            SqlStatement s;
            SqlFilterCollection filters = new SqlFilterCollection();
            List<bool> values = new List<bool>();

            values.Add(true);
            values.Add(false);
            values.Add(true);

            q.Filters.Add(new SqlListFilter<bool>(q.Table, "ProductID", values));
            s = q.Render();

            Assert.IsTrue(string.Equals(s.CommandText, test, StringComparison.OrdinalIgnoreCase), "Output CommandText does not match expected result");
        }

        [TestMethod]
        public void TestListFilterString()
        {
            string test = "SELECT * FROM (SELECT * FROM Products ) AS [t0] WHERE ([t0].[ProductName] IN (@p0,@p1,@p2)) ORDER BY [ProductName];";
            SqlSelectBuilder q = new SqlSelectBuilder("SELECT * FROM Products ORDER BY [ProductName]");
            SqlStatement s;
            SqlFilterCollection filters = new SqlFilterCollection();
            List<string> values = new List<string>();

            values.Add("blah");
            values.Add("it's cool");
            values.Add("woohoo");

            q.Filters.Add(new SqlListFilter<string>(q.Table, "ProductName", values));

            s = q.Render();
            
            Assert.IsTrue(string.Equals(s.CommandText, test, StringComparison.OrdinalIgnoreCase), "Output CommandText does not match expected result");
            Assert.IsTrue(s.Parameters.Any(x => (string)x.Value == values[0]), "Parameter at index 0 value is incorrect");
            Assert.IsTrue(s.Parameters.Any(x => (string)x.Value == values[1]), "Parameter at index 1 value is incorrect");
            Assert.IsTrue(s.Parameters.Any(x => (string)x.Value == values[2]), "Parameter at index 2 value is incorrect");
        }

        [TestMethod]
        public void TestQueryWithoutOrderBy()
        {
            string test = "SELECT * FROM (SELECT TOP 10 * FROM Products) AS [t0];";
            SqlSelectBuilder q = new SqlSelectBuilder("SELECT TOP 10 * FROM Products");
            SqlStatement s;

            s = q.Render();

            Assert.IsTrue(string.Equals(s.CommandText, test, StringComparison.OrdinalIgnoreCase), "Output CommandText does not match expected result");            
        }

        [TestMethod]
        public void TestJPsQuery()
        {            
            string query = @"select top 10 
CorporateKey, 
WidgetName, 
CommunicationStatus, 
DataVolume, 
DataVolumeYesterday, 
DateKey 
from facGadget Inner Join DimWidget on facGadget.GadgetKey = DimWidget.GadgetKey";            
            string test = string.Concat("SELECT * FROM (",query,") AS [t0];");
            SqlSelectBuilder q = new SqlSelectBuilder(query);
            SqlStatement s;

            s = q.Render();

            Assert.IsTrue(string.Equals(s.CommandText, test, StringComparison.OrdinalIgnoreCase), "Output CommandText does not match expected result");            
        }

        [TestMethod]
        public void TestRank()
        {
            string test1 = "SELECT [t0].[WidgetID],RANK() OVER(ORDER BY SUM([t0].[DataVolumeYesterday])) AS [DataVolumeRank] FROM [facGadget] AS [t0] GROUP BY [t0].[WidgetID];";
            string test2 = "SELECT [t0].[WidgetID],RANK() OVER(ORDER BY SUM([t0].[DataVolumeYesterday]) DESC) AS [DataVolumeRank] FROM [facGadget] AS [t0] GROUP BY [t0].[WidgetID];";
            SqlSelectBuilder q = new SqlSelectBuilder("facGadget");
            SqlStatement s;

            q.SelectColumns.Clear();
            q.SelectColumns.Add(new SqlColumn(q.Table, "WidgetID"));
            q.SelectColumns.Add(new SqlRankColumn(q.Table, "DataVolumeYesterday", SqlAggregate.Sum, "DataVolumeRank", false));
            s = q.Render();
            Assert.IsTrue(string.Equals(s.CommandText, test1, StringComparison.OrdinalIgnoreCase), "Output CommandText does not match expected result");

            q.SelectColumns.Clear();
            q.SelectColumns.Add(new SqlColumn(q.Table, "WidgetID"));
            q.SelectColumns.Add(new SqlRankColumn(q.Table, "DataVolumeYesterday", SqlAggregate.Sum, "DataVolumeRank", true));
            s = q.Render();

            Assert.IsTrue(string.Equals(s.CommandText, test2, StringComparison.OrdinalIgnoreCase), "Output CommandText does not match expected result");
        }

        [TestMethod]
        public void TestTableNameFormatting()
        {
            var test1 = "[table]";            
            var test2 = "[schema].[table]";            
            var test3 = "[server].[schema].[table]";
            string s;

            s = SqlTextWriter.FormatTableName("table");
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
            string test1 = "SELECT [t1].[Foo],[t1].[Bar],[t2].[Test1],[t2].[Test2] FROM [Table1] AS [t0] INNER JOIN [dbo].[Table2] AS [t1] ON ([t1].[Table1ID]=[t0].[Table1ID]) LEFT JOIN [dbo].[Table3] AS [t2] ON ([t2].[Table1ID]=[t0].[Table1ID]) AND ([t2].[Table2ID]=[t1].[Table2ID]);";
            string test2 = "SELECT [t1].[Foo],[t1].[Bar],[t2].[Test1],[t2].[Test2] FROM [Table1] AS [t0] INNER JOIN [dbo].[Table2] AS [t1] ON ([t1].[Table1ID]=[t0].[Table1ID]) LEFT JOIN [dbo].[Table3] AS [t2] ON ([t2].[Table1ID]=[t0].[Table1ID]) AND ([t2].[Table2ID]=[t1].[Table2ID]) WHERE ([t0].[SomeField]=@p0);";
            //string test3 = "SELECT RS.* FROM (SELECT [t0].*,[t1].[Foo],[t1].[Bar],[t2].[Test1],[t2].[Test2] FROM (Table1) AS t0 INNER JOIN [dbo].[Table2] AS t1 ON t1.[Table1ID]=[t0].[Table1ID] LEFT JOIN [dbo].[Table3] AS [t2] ON [t2].[Table1ID]=[t0].[Table1ID] AND [t2].[Table2ID]=t1.[Table2ID]) RS WHERE (RS.[SomeField]=@p0);";
            string test4 = "SELECT [t1].[Foo],[t1].[Bar],[t2].[Test1],[t2].[Test2],[t3].[Test1] FROM [Table1] AS [t0] INNER JOIN [dbo].[Table2] AS [t1] ON ([t1].[Table1ID]=[t0].[Table1ID]) LEFT JOIN [dbo].[Table3] AS [t2] ON ([t2].[Table1ID]=[t0].[Table1ID]) AND ([t2].[Table2ID]=[t1].[Table2ID]) CROSS JOIN [dbo].[Table4] AS [t3];";

            SqlSelectBuilder q = new SqlSelectBuilder("Table1");
            SqlStatement s;

            var table2 = new SqlTable("dbo.Table2");
            q.Joins.AddInner(q.Table, table2, new ISqlFilter[] { new SqlColumnCompareFilter(table2, "Table1ID", SqlOperator.Equal, q.Table, "Table1ID") });            
            q.SelectColumns.Add(new SqlColumn(table2, "Foo"));
            q.SelectColumns.Add(new SqlColumn(table2, "Bar"));

            var table3 = new SqlTable("dbo.Table3");
            q.Joins.AddLeft(q.Table, table3, new ISqlFilter[] { 
                new SqlColumnCompareFilter(table3,"Table1ID", SqlOperator.Equal, q.Table, "Table1ID"),
                new SqlColumnCompareFilter(table3, "Table2ID", SqlOperator.Equal, table2, "Table2ID")
            });
            q.SelectColumns.Add(new SqlColumn(table3, "Test1"));
            q.SelectColumns.Add(new SqlColumn(table3, "Test2"));
                                              
            s = q.Render();
            Assert.IsTrue(string.Equals(s.CommandText, test1, StringComparison.OrdinalIgnoreCase), "Output CommandText does not match expected result");

            q.Filters.Add(new SqlCompareFilter<string>(q.Table, "SomeField", SqlOperator.Equal, "Value"));
            s = q.Render();
            Assert.IsTrue(string.Equals(s.CommandText, test2, StringComparison.OrdinalIgnoreCase), "Output CommandText does not match expected result");

            q.Filters.Clear();
            var table4 = new SqlTable("dbo.Table4");
            q.Joins.AddCross(q.Table, table4);
            q.SelectColumns.Add(new SqlColumn(table4, "Test1"));
            s = q.Render();
            Assert.IsTrue(string.Equals(s.CommandText, test4, StringComparison.OrdinalIgnoreCase), "Output CommandText does not match expected result");            
        }

        [TestMethod]
        public void TestCountDistinct()
        {
            string test1 = "SELECT COUNT(DISTINCT [t0].[Foo]) AS [FooCount],[t0].[Bar] FROM [dbo].[FooBar] AS [t0] GROUP BY [t0].[Bar];";
            SqlSelectBuilder q = new SqlSelectBuilder("dbo.FooBar");

            q.SelectColumns.Add(new SqlColumn(q.Table, "Foo", "FooCount") { Aggregate = SqlAggregate.CountDistinct });            
            q.SelectColumns.Add(new SqlColumn(q.Table, "Bar"));

            var stmt = q.Render();

            Assert.AreEqual(test1, stmt.CommandText, true);
        }

        [TestMethod]
        public void TestDerivedTableWrapsWithParens()
        {
            var table = new SqlDerivedTable("SELECT * FROM [Foo]");
            var writer = new SqlTextWriter(Csg.Data.SqlServer.SqlServerProvider.Instance);
            var args = writer.BuildArguments;

            args.AssignAlias(table);            

            ((ISqlTable)table).Render(writer);
            
            Assert.IsTrue(string.Equals(writer.ToString(), "(SELECT * FROM [Foo]) AS [" + args.TableName(table) + "]"));
        }

        [TestMethod]
        public void TestDerivedTableRemovesSemicolons()
        {
            var table = new SqlDerivedTable("SELECT * FROM [Foo];");
            var writer = new SqlTextWriter(Csg.Data.SqlServer.SqlServerProvider.Instance);

            writer.BuildArguments.AssignAlias(table);

            ((ISqlTable)table).Render(writer);

            Assert.IsTrue(string.Equals(writer.ToString(), "(SELECT * FROM [Foo]) AS [" + writer.BuildArguments.TableName(table) + "]"));
        }


        [TestMethod]
        public void TestExistFilter()
        {
            var outerQuery = new SqlSelectBuilder("SELECT * FROM Stuff ORDER BY [WidgetName];");
            var innerQuery = new SqlSelectBuilder("[Foo]");

            innerQuery.SelectColumns.Add(new SqlLiteralColumn<int>(1));
            outerQuery.Filters.Add(new SqlExistFilter(innerQuery));

            var stmt = outerQuery.Render();

            Assert.AreEqual("SELECT * FROM (SELECT * FROM Stuff ) AS [t0] WHERE (EXISTS (SELECT 1 FROM [Foo] AS [t1])) ORDER BY [WidgetName];", stmt.CommandText);          
        }

        [TestMethod]
        public void TestLiteralColumn()
        {
            var query = new SqlSelectBuilder("[Foo]");

            query.SelectColumns.Add(new SqlLiteralColumn<int>(1));
            query.SelectColumns.Add(new SqlLiteralColumn<string>("2", "Two"));

            var stmt = query.Render();

            Assert.AreEqual("SELECT 1,'2' AS [Two] FROM [Foo] AS [t0];", stmt.CommandText);
        }

        [TestMethod]
        public void TestRawFilter()
        {
            string test = "SELECT * FROM [DimWidget] AS [t0] WHERE ((SELECT COUNT(*) FROM dbo.WidgetComment WHERE WidgetID = [t0].WidgetID) > @p0);";
            SqlSelectBuilder q = new SqlSelectBuilder("DimWidget");
            
            q.Filters.Add(new SqlRawFilter("(SELECT COUNT(*) FROM dbo.WidgetComment WHERE WidgetID = {0}.WidgetID) > {1}", q.Table, new DbParameterValue() {
                Value = 3,
                DbType = System.Data.DbType.Int32
            }));

            var s = q.Render();

            Assert.IsTrue((s.Parameters.Count > 0), "Parameter count should be 1.");
            Assert.AreEqual(test, s.CommandText, true);
        }

        [TestMethod]
        public void TestSubQueryCountFilter()
        {
            string test = "SELECT * FROM [DimWidget] AS [t0] WHERE ((SELECT COUNT([t1].[WidgetCommentID]) AS [Cnt] FROM [dbo].[WidgetComment] AS [t1] WHERE ([t0].[WidgetID]=[t1].[WidgetID])) > @p0);";
            //             SELECT * FROM [DimWidget] AS [t0] WHERE ((SELECT COUNT([t1].[WidgetCommentID]) AS [Cnt] FROM [dbo].[WidgetComment] AS [t1] WHERE ([t0].[WidgetID]=[t1].[WidgetID])) > @p0);
            SqlSelectBuilder q = new SqlSelectBuilder("DimWidget");

            var widgetComment = SqlTable.Create("dbo.WidgetComment");

            var sqf = new SqlCountFilter(q.Table, widgetComment, "WidgetCommentID", SqlOperator.GreaterThan, 3);
            sqf.SubQueryFilters.Add(new SqlColumnCompareFilter(q.Table, "WidgetID", SqlOperator.Equal, widgetComment));
            q.Filters.Add(sqf);

            var s = q.Render();

            Assert.IsTrue((s.Parameters.Count > 0), "Parameter count should be 1.");
            Assert.AreEqual(test, s.CommandText, true);
        }
    }
}



