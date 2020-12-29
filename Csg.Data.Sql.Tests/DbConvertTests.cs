using System;
using System.Collections.Generic;
using System.Data;
using Csg.Data;
using Csg.Data.Sql;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestProject
{
    [TestClass]
    public class DbConvertTests
    {
        [TestMethod]
        public void Test_TypeToDbType()
        {
            Assert.AreEqual(System.Data.DbType.Boolean, DbConvert.TypeToDbType(typeof(bool)));
            Assert.AreEqual(System.Data.DbType.Boolean, DbConvert.TypeToDbType(typeof(bool?)));
            
            Assert.AreEqual(System.Data.DbType.Byte, DbConvert.TypeToDbType(typeof(byte)));
            Assert.AreEqual(System.Data.DbType.Byte, DbConvert.TypeToDbType(typeof(byte?)));
            
            Assert.AreEqual(System.Data.DbType.Int16, DbConvert.TypeToDbType(typeof(short)));
            Assert.AreEqual(System.Data.DbType.Int16, DbConvert.TypeToDbType(typeof(short?)));

            Assert.AreEqual(System.Data.DbType.Int32, DbConvert.TypeToDbType(typeof(int)));
            Assert.AreEqual(System.Data.DbType.Int32, DbConvert.TypeToDbType(typeof(int?)));

            Assert.AreEqual(System.Data.DbType.Int64, DbConvert.TypeToDbType(typeof(long)));
            Assert.AreEqual(System.Data.DbType.Int64, DbConvert.TypeToDbType(typeof(long?)));

            Assert.AreEqual(System.Data.DbType.Single, DbConvert.TypeToDbType(typeof(float)));
            Assert.AreEqual(System.Data.DbType.Single, DbConvert.TypeToDbType(typeof(float?)));

            Assert.AreEqual(System.Data.DbType.Double, DbConvert.TypeToDbType(typeof(double)));
            Assert.AreEqual(System.Data.DbType.Double, DbConvert.TypeToDbType(typeof(double?)));

            Assert.AreEqual(System.Data.DbType.Decimal, DbConvert.TypeToDbType(typeof(decimal)));
            Assert.AreEqual(System.Data.DbType.Decimal, DbConvert.TypeToDbType(typeof(decimal?)));

            Assert.AreEqual(System.Data.DbType.String, DbConvert.TypeToDbType(typeof(string)));
            Assert.AreEqual(System.Data.DbType.String, DbConvert.TypeToDbType(typeof(string)));

            Assert.AreEqual(System.Data.DbType.Guid, DbConvert.TypeToDbType(typeof(Guid)));
            Assert.AreEqual(System.Data.DbType.Guid, DbConvert.TypeToDbType(typeof(Guid?)));

            Assert.AreEqual(System.Data.DbType.DateTime2, DbConvert.TypeToDbType(typeof(DateTime)));
            Assert.AreEqual(System.Data.DbType.DateTime2, DbConvert.TypeToDbType(typeof(DateTime?)));

            Assert.AreEqual(System.Data.DbType.DateTimeOffset, DbConvert.TypeToDbType(typeof(DateTimeOffset)));
            Assert.AreEqual(System.Data.DbType.DateTimeOffset, DbConvert.TypeToDbType(typeof(DateTimeOffset?)));

            Assert.AreEqual(System.Data.DbType.Time, DbConvert.TypeToDbType(typeof(TimeSpan)));
            Assert.AreEqual(System.Data.DbType.Time, DbConvert.TypeToDbType(typeof(TimeSpan?)));

            Assert.AreEqual(System.Data.DbType.StringFixedLength, DbConvert.TypeToDbType(typeof(char)));
            Assert.AreEqual(System.Data.DbType.StringFixedLength, DbConvert.TypeToDbType(typeof(char?)));

            Assert.AreEqual(System.Data.DbType.Binary, DbConvert.TypeToDbType(typeof(byte[])));
        }        

        [TestMethod]
        public void SqlCompareFilter_GetsCorrectType()
        {
            var filter1 = new SqlCompareFilter<bool>();
            var filter2 = new SqlCompareFilter<bool?>();

            Assert.AreEqual(DbType.Boolean, filter1.DataType);
            Assert.AreEqual(DbType.Boolean, filter2.DataType);
        }

        [TestMethod]
        public void SqlListFilter_GetsCorrectType()
        {
            var table = SqlTable.Create("foo");
            var filter1 = new SqlListFilter<int>(table, "Foo", new List<int>());
            var filter2 = new SqlListFilter<int?>(table, "Foo", new List<int?>());

            Assert.AreEqual(DbType.Int32, filter1.DataType);
            Assert.AreEqual(DbType.Int32, filter2.DataType);
        }
    }


}



