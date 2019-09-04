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
            Csg.Data.DbQueryBuilder.DefaultGenerateFormattedSql = false;
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
    }
}
