using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Csg.Data.Sql.Tests
{
    [TestClass]
    public class DbQueryBuilderTests
    {
        static DbQueryBuilderTests()
        {
            Csg.Data.DbQueryBuilder.GenerateFormattedSql = false;
        }

        [TestMethod]
        public void TestCreateCommandPopulatesCommandParameters()
        {
            var conn = new MockConnection();
            var query = new DbQueryBuilder("dbo.TableName", conn);

            query.Parameters.Add(new DbParameterValue()
            {
                ParameterName  = "@Param1",
                DbType = System.Data.DbType.Int32,
                Size = 4,
                Value = 123
            });

            var stmt = query.Render();

            Assert.AreEqual(1, stmt.Parameters.Count);
            Assert.AreEqual(System.Data.DbType.Int32, stmt.Parameters.First().DbType);
            Assert.AreEqual(4, stmt.Parameters.First().Size);
            Assert.AreEqual(123, stmt.Parameters.First().Value);

            var cmd = stmt.CreateCommand(conn);

            Assert.AreEqual(conn, cmd.Connection);
            Assert.AreEqual(1, cmd.Parameters.Count);
            Assert.AreEqual(System.Data.DbType.Int32, ((IDbDataParameter)cmd.Parameters[0]).DbType);
            Assert.AreEqual(4, ((IDbDataParameter)cmd.Parameters[0]).Size);
            Assert.AreEqual(123, ((IDbDataParameter)cmd.Parameters[0]).Value);
        }

        [TestMethod]
        public void TestForkCreatesShallowClone()
        {
            var conn = new MockConnection();
            var query = new DbQueryBuilder("dbo.TableName", conn);

            query.OrderBy.Add(new SqlOrderColumn() { ColumnName = "Foo" });
            query.AddJoin(new SqlJoin(query.Root, SqlJoinType.Cross, SqlTable.Create("Blah")));
            query.AddFilter(new SqlNullFilter(query.Root, "Foo", false));
            query.SelectColumns.Add(new SqlColumn(query.Root, "Foo"));
            query.PagingOptions = new SqlPagingOptions()
            {
                Offset = 10,
                Limit = 100
            };
            query.Parameters.Add(new DbParameterValue()
            {
                ParameterName = "@Param1",
                DbType = System.Data.DbType.Int32,
                Size = 4,
                Value = 123
            });
            query.Distinct = true;
            query.CommandTimeout = 123;

            var fork = query.Fork();

            query.OrderBy.Clear();
            query.AddFilter(new SqlNullFilter(query.Root, "Bar", false));
            query.SelectColumns.Clear();
            query.PagingOptions = null;
            query.Parameters.Clear();
            query.CommandTimeout = 999;
            query.Distinct = false;
            query.AddJoin(new SqlJoin(query.Root, SqlJoinType.Cross, SqlTable.Create("blah2")));

            Assert.AreEqual(1, fork.OrderBy.Count);
            Assert.AreEqual(1, fork.SelectColumns.Count);
            Assert.AreEqual(1, fork.Parameters.Count);
            Assert.AreEqual(10, fork.PagingOptions.Value.Offset);
            Assert.AreEqual(100, fork.PagingOptions.Value.Limit);
            Assert.AreEqual(true, fork.Distinct);
            Assert.AreEqual(123, fork.CommandTimeout);
        }

        [TestMethod]
        public void TestQueryBuilderAsSqlStatementRendersParametersToArgs()
        {
            var conn = new MockConnection();
            var query = new DbQueryBuilder("SELECT * FROM dbo.TableName WHERE Foo=@Foo", conn);


            query.Parameters.Add(new DbParameterValue()
            {
                ParameterName = "@Foo",
                DbType = System.Data.DbType.Int32,
                Size = 4,
                Value = 123
            });

            var args = new SqlBuildArguments();
            var writer = new SqlTextWriter();

            ((ISqlStatementElement)query).Render(writer, args);

            Assert.AreEqual(1, args.Parameters.Count);
            Assert.AreEqual("SELECT * FROM (SELECT * FROM dbo.TableName WHERE Foo=@Foo) AS [t0]", writer.ToString());
        }

        [TestMethod]
        public void TestQueryBuilderRenderBatch()
        {
            var conn = new MockConnection();
            var query = new DbQueryBuilder("SELECT * FROM dbo.TableName WHERE Foo=@Foo", conn);

            query.Parameters.Add(new DbParameterValue()
            {
                ParameterName = "@Foo",
                DbType = System.Data.DbType.Int32,
                Size = 4,
                Value = 123
            });

            var stmt = new ISqlStatementElement[] { query }.RenderBatch();

            Assert.AreEqual(1, stmt.Parameters.Count);
            Assert.AreEqual("SELECT * FROM (SELECT * FROM dbo.TableName WHERE Foo=@Foo) AS [t0];\r\n", stmt.CommandText);
        }
    }
}
