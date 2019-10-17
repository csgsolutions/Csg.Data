
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Csg.Data.Sql;
using System.Collections.Generic;
using System.Linq;
using Csg.Data;

namespace TestProject
{
    [TestClass]
    public class FluentAPITests
    {
        static FluentAPITests()
        {
            Csg.Data.DbQueryBuilder.GenerateFormattedSql = false;
        }

        [TestMethod]
        public void TestFluentJoinMultipleFilterCollectionsWithOrLogic()
        {
            var expectSql = "SELECT * FROM [dbo].[Product] AS [t0] WHERE ([t0].[IsActive]=@p0) AND ((([t0].[ProductCategoryID]=@p1) AND ([t0].[SupplierID]=@p2) AND ([t0].[ThingName] IN (@p3,@p4,@p5))) OR (([t0].[ProductCategoryID]=@p6) AND ([t0].[SupplierID]=@p7) AND ([t0].[ThingName] IN (@p8,@p9,@p10))));";
            IDbQueryBuilder builder = new Csg.Data.DbQueryBuilder("dbo.Product", new MockConnection());

            var listOfThings1 = new string[] { "a", "b", "c" };
            var listOfThings2 = new string[] { "d", "e", "f" };

            var listOfCriteria = new Tuple<int, int, string[]>[]
            {
                new Tuple<int,int,string[]>(123,456,listOfThings1),
                new Tuple<int,int,string[]>(123,456,listOfThings2)
            };

            builder = builder.Where(x => x.FieldEquals<bool>("IsActive", true));
            builder = builder.WhereAny(
                (IEnumerable<Tuple<int, int, string[]>>)listOfCriteria,
                (x, f) => x.FieldEquals("ProductCategoryID", f.Item1)
                        .FieldEquals("SupplierID", f.Item2)
                        .FieldIn("ThingName", f.Item3)
            );

            var stmt = builder.Render();
            Assert.IsNotNull(stmt.CommandText);
            Assert.AreEqual(expectSql, stmt.CommandText);
        }

        [TestMethod]
        public void TestFluentJoinMultipleFilterCollectionsWithOrLogicIndexed()
        {
            var expectSql = "SELECT * FROM [dbo].[Product] AS [t0] WHERE ([t0].[IsActive]=@p0) AND ((([t0].[ProductCategoryID]=@p1) AND ([t0].[SupplierID]=@p2) AND ([t0].[ThingName] IN (@p3,@p4,@p5))) OR (([t0].[ProductCategoryID]=@p6) AND ([t0].[SupplierID]=@p7) AND ([t0].[ThingName] IN (@p8,@p9,@p10))));";
            IDbQueryBuilder builder = new Csg.Data.DbQueryBuilder("dbo.Product", new MockConnection());

            var listOfThings1 = new string[] { "a", "b", "c" };
            var listOfThings2 = new string[] { "d", "e", "f" };

            var listOfCriteria = new Tuple<int, int, string[]>[]
            {
                new Tuple<int,int,string[]>(123,456,listOfThings1),
                new Tuple<int,int,string[]>(123,456,listOfThings2)
            };

            builder = builder.Where(x => x.FieldEquals<bool>("IsActive", true));
            builder = builder.WhereAny(
                listOfCriteria,
                (x, f, i) => x.FieldEquals("ProductCategoryID", f.Item1)
                        .FieldEquals("SupplierID", f.Item2)
                        .FieldIn("ThingName", f.Item3)
            );

            var stmt = builder.Render();
            Assert.IsNotNull(stmt.CommandText);
            Assert.AreEqual(expectSql, stmt.CommandText);
        }

        [TestMethod]
        public void TestFluentFieldInSubQuery()
        {
            var expectSql = "SELECT * FROM [dbo].[Product] AS [t0] WHERE ([t0].[ProductID] IN (SELECT [t1].[ProductID] FROM [dbo].[ProductAttribute] AS [t1] WHERE (([t1].[AttributeName]=@p0) AND ([t1].[AttributeValue] IN (@p1,@p2)))));";
            //               SELECT * FROM [dbo].[Product] AS [t0] WHERE ([t0].[ProductID] IN (SELECT [t1].[ProductID] FROM [dbo].[ProductAttribute] AS [t1] WHERE (([t0].[AttributeName]=@p0) AND ([t0].[AttributeValue] IN (@p1,@p2)))));
            IDbQueryBuilder builder = new Csg.Data.DbQueryBuilder("dbo.Product", new MockConnection());

            builder = builder.Where(where => where.FieldInSubQuery("ProductID", "dbo.ProductAttribute", "ProductID",
                subWhere => subWhere.FieldMatch("AttributeName", SqlOperator.Equal, "Color", isAnsi: true)
                        .FieldIn("AttributeValue", new string[] { "Red", "Green" })
            ));

            var stmt = builder.Render();
            Assert.IsNotNull(stmt.CommandText);
            Assert.AreEqual(expectSql, stmt.CommandText);
            Assert.AreEqual(3, stmt.Parameters.Count);
        }

        [TestMethod]
        public void TestFluentSubQueryCount()
        {
            var expectSql = "SELECT * FROM [dbo].[Product] AS [t0] WHERE ((SELECT COUNT([t1].[ProductID]) AS [Cnt] FROM [dbo].[ProductAttribute] AS [t1] WHERE ([t1].[AttributeName]=@p0)) > @p1);";
            //               SELECT * FROM [dbo].[Product] AS [t0] WHERE ((SELECT COUNT([t1].[ProductID]) AS [Cnt] FROM [dbo].[ProductAttribute] AS [t1] WHERE ([t1].[AttributeName]=@p0)) > @p1);
            IDbQueryBuilder builder = new Csg.Data.DbQueryBuilder("dbo.Product", new MockConnection());

            builder = builder.Where(where => where.SubQueryCount("dbo.ProductAttribute", "ProductID", SqlOperator.GreaterThan, 1,
                subWhere => subWhere.FieldMatch("AttributeName", SqlOperator.Equal, "Color", isAnsi: true)
            ));

            var stmt = builder.Render();
            Assert.IsNotNull(stmt.CommandText);
            Assert.AreEqual(expectSql, stmt.CommandText);
            Assert.AreEqual(2, stmt.Parameters.Count);
        }

        [TestMethod]
        public void TestFluentPagingOptions()
        {
            string test = "SELECT * FROM [dbo].[Product] AS [t0] ORDER BY [PersonID] ASC OFFSET 50 ROWS FETCH NEXT 10 ROWS ONLY;";
           
            var stmt = new Csg.Data.DbQueryBuilder("dbo.Product", new MockConnection())
                .OrderBy("PersonID")
                .Limit(10, 50)
                .Render();

            Assert.AreEqual(test, stmt.CommandText, true);
        }

        [TestMethod]
        public void TestFluentAny()
        {
            string test = "SELECT * FROM [dbo].[Product] AS [t0] WHERE (([t0].[ProductID]=@p0) OR ([t0].[ProductID]=@p1));";

            var stmt = new Csg.Data.DbQueryBuilder("dbo.Product", new MockConnection())
                .Where(x => x.Any(y => y.FieldEquals("ProductID", 1).FieldEquals("ProductID",2)))
                .Render();

            Assert.AreEqual(test, stmt.CommandText, true);
        }

        [TestMethod]
        public void TestFluentLimit_WithoutAnyOrderBy_Throws()
        {
            var ex = Assert.ThrowsException<InvalidOperationException>(() =>
            {
                var stmt = new Csg.Data.DbQueryBuilder("dbo.Product", new MockConnection())
                .Limit(10, 50);
            });

            Assert.AreEqual("A query cannot have a limit or offset without an order by expression.", ex.Message);
        }

    }
}
