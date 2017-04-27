using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Csg.Data
{
    public class MockTransaction : System.Data.IDbTransaction
    {

        public IDbConnection Connection { get; set; }
        

        public IsolationLevel IsolationLevel { get; set; }

        public bool IsCommited { get; set; }

        public bool IsDisposed { get; set; }

        public bool IsRolledBack { get; set; }


        public void Commit()
        {
            this.IsCommited = true;
        }

        public void Dispose()
        {
            if (!this.IsCommited)
            {
                this.Rollback();
            }
            this.IsDisposed = true;
        }

        public void Rollback()
        {
            this.IsRolledBack = true;
        }
    }

}
