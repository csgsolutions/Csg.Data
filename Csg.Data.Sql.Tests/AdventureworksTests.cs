using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;
using Csg.Data;

namespace TestProject
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    //[TestClass]
    [TestCategory("Integration")]
    public class AdventureworksTests
    {
        private const string _connectionString = @"Data Source=(localdb)\ProjectsV13;Initial Catalog=AdventureWorks;Integrated Security=True;Connect Timeout=60;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        private static System.Data.SqlClient.SqlConnection CreateConnection()
        {
            var c = new System.Data.SqlClient.SqlConnection(_connectionString);
            c.Open();
            return c;
        }

        //[TestMethod]
        public void Test_Person_SelectByType()
        {
            using (var conn = CreateConnection())
            {
                conn.QueryBuilder("Person.Person")
                    .Select("BusinessEntityID", "FirstName", "LastName")
                    .Where(x => x.FieldEquals("PersonType", "EM", System.Data.DbType.StringFixedLength, size: 2))
                    .OrderBy("LastName")
                    .OrderByDescending("FirstName")
                    .ExecuteReader();
            }
        }
    }
}



