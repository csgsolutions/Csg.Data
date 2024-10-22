using System.Data;

namespace Csg.Data.Sql.Tests.Mock;

public class MockTransaction : IDbTransaction
{
    public bool IsCommited { get; set; }

    public bool IsDisposed { get; set; }

    public bool IsRolledBack { get; set; }

    public IDbConnection Connection { get; set; }


    public IsolationLevel IsolationLevel { get; set; }


    public void Commit()
    {
        IsCommited = true;
    }

    public void Dispose()
    {
        if (!IsCommited) Rollback();
        IsDisposed = true;
    }

    public void Rollback()
    {
        IsRolledBack = true;
    }
}