
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Csg.Data.Sql;
using System.Collections.Generic;
using System.Linq;
using Csg.Data;
using Csg.Data.Common;

namespace TestProject
{
    [TestClass]
    public class FluentAPITests
    {
        static FluentAPITests()
        {
            Csg.Data.Common.DbQueryBuilder.DefaultGenerateFormattedSql = false;
        }

        [TestMethod]
        public void TestFluentJoinMultipleFilterCollectionsWithOrLogic()
        {
            var expectSql = "SELECT * FROM [dbo].[Product] AS [t0] WHERE ([t0].[IsActive]=@p0) AND ((([t0].[ProductCategoryID]=@p1) AND ([t0].[SupplierID]=@p2) AND ([t0].[ThingName] IN (@p3,@p4,@p5))) OR (([t0].[ProductCategoryID]=@p6) AND ([t0].[SupplierID]=@p7) AND ([t0].[ThingName] IN (@p8,@p9,@p10))));";
            IDbQueryBuilder builder = new MockConnection().QueryBuilder("dbo.Product");

            var listOfThings1 = new string[] { "a", "b", "c" };
            var listOfThings2 = new string[] { "d", "e", "f" };

            var listOfCriteria = new List<(int ProductCategoryID, int SupplierID, string[] Names)>
            {
                (ProductCategoryID: 123, SupplierID: 456, Names: listOfThings1),
                (ProductCategoryID: 999, SupplierID: 888, Names: listOfThings2),
            };

            builder = builder.Where(x => x.FieldEquals<bool>("IsActive", true));
            builder = builder.WhereAny(
                listOfCriteria,
                (x, f) => x.FieldEquals("ProductCategoryID", f.ProductCategoryID)
                        .FieldEquals("SupplierID", f.SupplierID)
                        .FieldIn("ThingName", f.Names)
            );

            var stmt = builder.Render();
            Assert.IsNotNull(stmt.CommandText);
            Assert.AreEqual(expectSql, stmt.CommandText);
        }

        [TestMethod]
        public void TestFluentJoinMultipleFilterCollectionsWithOrLogicIndexed()
        {
            var expectSql = "SELECT * FROM [dbo].[Product] AS [t0] WHERE ([t0].[IsActive]=@p0) AND ((([t0].[ProductCategoryID]=@p1) AND ([t0].[SupplierID]=@p2) AND ([t0].[ThingName] IN (@p3,@p4,@p5))) OR (([t0].[ProductCategoryID]=@p6) AND ([t0].[SupplierID]=@p7) AND ([t0].[ThingName] IN (@p8,@p9,@p10))));";
            IDbQueryBuilder builder = new MockConnection().QueryBuilder("dbo.Product");

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
            IDbQueryBuilder builder = new MockConnection().QueryBuilder("dbo.Product");

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
            var expectSql = "SELECT * FROM [dbo].[Product] AS [t0] WHERE ((SELECT COUNT([t1].[ProductID]) AS [Cnt] FROM [dbo].[ProductAttribute] AS [t1] WHERE (([t1].[ProductID]=[t0].[ProductID]) AND ([t1].[AttributeName]=@p0))) > @p1);";
            IDbQueryBuilder builder = new MockConnection().QueryBuilder("dbo.Product");

            builder = builder.Where(where => where.SubQueryCount("dbo.ProductAttribute", "ProductID", SqlOperator.GreaterThan, 1,
                subWhere => subWhere.FieldEquals("ProductID", builder.Root, "ProductID").FieldMatch("AttributeName", SqlOperator.Equal, "Color", isAnsi: true)
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
           
            var stmt = new MockConnection().QueryBuilder("dbo.Product")
                .OrderBy("PersonID")
                .Limit(10, 50)
                .Render();

            Assert.AreEqual(test, stmt.CommandText, true);
        }

        [TestMethod]
        public void TestFluentLimit_WithoutAnyOrderBy_Throws()
        {
            var ex = Assert.ThrowsException<InvalidOperationException>(() =>
            {
                var stmt = new MockConnection().QueryBuilder("dbo.Product")
                .Limit(10, 50);
            });

            Assert.AreEqual("A query cannot have a limit or offset without an order by expression.", ex.Message);
        }

        [TestMethod]
        public void TestFluentExists()
        {
            string test = "SELECT * FROM [dbo].[Product] AS [t0] WHERE (EXISTS (SELECT 1 FROM [dbo].[ProductColor] AS [t1] WHERE (([t1].[ProductID]=@p0) AND ([t1].[Color]=@p1))));";
                         //SELECT * FROM [dbo].[Product] AS [t0] WHERE (EXISTS (SELECT 1 FROM [dbo].[ProductColor] AS [t1] WHERE (([t1].[ProductID]=@p0) AND ([t1].[Color]=@p1))));
            var stmt = new MockConnection().QueryBuilder("dbo.Product")
                .Where(x => x.Exists("dbo.ProductColor", sub => sub.FieldEquals("ProductID", 1).FieldEquals("Color","Red")))
                .Render();

            Assert.AreEqual(test, stmt.CommandText, true);
        }

        [TestMethod]
        public void TestFluentPrefix()
        {
            string test = "Prefix Value;SELECT * FROM [dbo].[Product] AS [t0];";
            var stmt = new MockConnection().QueryBuilder("dbo.Product")
                .Prefix("Prefix Value")
                .Render();

            Assert.AreEqual(test, stmt.CommandText, true);
        }

        [TestMethod]
        public void TestFluentSuffix()
        {
            string test = "SELECT * FROM [dbo].[Product] AS [t0];Suffix Value;";
            var stmt = new MockConnection().QueryBuilder("dbo.Product")
                .Suffix("Suffix Value")
                .Render();

            Assert.AreEqual(test, stmt.CommandText, true);
        }

    }
}
