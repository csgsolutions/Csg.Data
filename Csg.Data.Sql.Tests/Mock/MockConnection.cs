using System;
using System.Data;

namespace Csg.Data.Sql.Tests.Mock;

public class MockConnection : IDbConnection
{
    public MockConnection()
    {
        InternalState = ConnectionState.Closed;
    }

    private ConnectionState InternalState { get; set; }

    public IDbTransaction BeginTransaction(IsolationLevel il)
    {
        throw new NotImplementedException();
    }

    public IDbTransaction BeginTransaction()
    {
        throw new NotImplementedException();
    }

    public void ChangeDatabase(string databaseName)
    {
        throw new NotImplementedException();
    }

    public void Close()
    {
        InternalState = ConnectionState.Closed;
    }

    public string ConnectionString
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public int ConnectionTimeout => throw new NotImplementedException();

    public IDbCommand CreateCommand()
    {
        return new MockCommand
        {
            Connection = this
        };
    }

    public string Database => throw new NotImplementedException();

    public void Open()
    {
        InternalState = ConnectionState.Open;
    }

    public ConnectionState State => InternalState;

    public void Dispose()
    {
        Close();
    }

    public bool IsOpen()
    {
        return State == ConnectionState.Open;
    }

    public bool IsClosed()
    {
        return State == ConnectionState.Closed;
    }
}