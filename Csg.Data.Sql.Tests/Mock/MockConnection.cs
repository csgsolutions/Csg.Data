using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csg.Data
{
    public class MockConnection : System.Data.IDbConnection
    {

        private System.Data.ConnectionState InternalState { get; set; }

        public MockConnection()
        {
            this.InternalState = System.Data.ConnectionState.Closed;
        }

        public System.Data.IDbTransaction BeginTransaction(System.Data.IsolationLevel il)
        {
            throw new NotImplementedException();
        }

        public System.Data.IDbTransaction BeginTransaction()
        {
            throw new NotImplementedException();
        }

        public void ChangeDatabase(string databaseName)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            this.InternalState = System.Data.ConnectionState.Closed;
        }

        public string ConnectionString
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public int ConnectionTimeout
        {
            get { throw new NotImplementedException(); }
        }

        public System.Data.IDbCommand CreateCommand()
        {
            return new MockCommand()
            {
                Connection = this
            };
        }

        public string Database
        {
            get { throw new NotImplementedException(); }
        }

        public void Open()
        {
            this.InternalState = System.Data.ConnectionState.Open;
        }

        public System.Data.ConnectionState State
        {
            get { return this.InternalState; }
        }

        public void Dispose()
        {
            this.Close();
        }

        public bool IsOpen()
        {
            return this.State == System.Data.ConnectionState.Open;
        }

        public bool IsClosed()
        {
            return this.State == System.Data.ConnectionState.Closed;
        }
    }

}
