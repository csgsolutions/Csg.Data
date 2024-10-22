using Csg.Data.Sql.Tests.Mock;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csg.Data.Sql.Tests;

[TestClass]
public class DbScopeTests
{
    [TestMethod]
    public void TestDisposeConnection()
    {
        var conn = new MockConnection();

        Assert.IsTrue(conn.IsClosed());

        using (new DbScope(conn))
        {
            Assert.IsTrue(conn.IsClosed());
            conn.Open();
            Assert.IsTrue(conn.IsOpen());
        }

        Assert.IsTrue(conn.IsClosed());
    }

    [TestMethod]
    public void TestCommitCallsTransactionCommit()
    {
        var conn = new MockConnection();
        var tran = new MockTransaction();

        using (var scope = new DbScope(conn, tran))
        {
            Assert.IsFalse(tran.IsCommited);
            scope.Commit();
            Assert.IsTrue(tran.IsCommited);
            Assert.IsNull(scope.Transaction);
        }
    }

    [TestMethod]
    public void TestDiposeCallsTransactionDispose()
    {
        var conn = new MockConnection();
        var tran = new MockTransaction();

        using (new DbScope(conn, tran))
        {
            Assert.IsFalse(tran.IsCommited);
            Assert.IsFalse(tran.IsDisposed);
        }

        Assert.IsTrue(tran.IsDisposed);
    }
}