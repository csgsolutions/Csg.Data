using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Csg.Data.Sql.Tests.Mock;

public class MockParameterCollection : List<IDbDataParameter>, IDataParameterCollection
{
    public object this[string parameterName]
    {
        get { return this.SingleOrDefault(x => x.ParameterName == parameterName); }
        set => throw new NotImplementedException();
    }

    public bool Contains(string parameterName)
    {
        return this.Any(x => x.ParameterName == parameterName);
    }

    public int IndexOf(string parameterName)
    {
        return IndexOf(this.SingleOrDefault(x => x.ParameterName == parameterName));
    }

    public void RemoveAt(string parameterName)
    {
        throw new NotImplementedException();
    }
}

public class MockParameter : IDbDataParameter
{
    public DbType DbType { get; set; }

    public ParameterDirection Direction { get; set; }


    public bool IsNullable { get; set; }


    public string ParameterName { get; set; }

    public byte Precision { get; set; }

    public byte Scale { get; set; }

    public int Size { get; set; }

    public string SourceColumn { get; set; }


    public DataRowVersion SourceVersion { get; set; }

    public object Value { get; set; }
}

public class MockCommand : IDbCommand
{
    public MockCommand()
    {
        Parameters = new MockParameterCollection();
    }

    public string CommandText { get; set; }

    public int CommandTimeout { get; set; }


    public CommandType CommandType { get; set; }


    public IDbConnection Connection { get; set; }


    public IDataParameterCollection Parameters { get; set; }


    public IDbTransaction Transaction { get; set; }

    public UpdateRowSource UpdatedRowSource { get; set; }

    public void Cancel()
    {
        throw new NotImplementedException();
    }

    public IDbDataParameter CreateParameter()
    {
        return new MockParameter();
    }

    public void Dispose()
    {
    }

    public int ExecuteNonQuery()
    {
        throw new NotImplementedException();
    }

    public IDataReader ExecuteReader()
    {
        throw new NotImplementedException();
    }

    public IDataReader ExecuteReader(CommandBehavior behavior)
    {
        throw new NotImplementedException();
    }

    public object ExecuteScalar()
    {
        throw new NotImplementedException();
    }

    public void Prepare()
    {
        throw new NotImplementedException();
    }
}