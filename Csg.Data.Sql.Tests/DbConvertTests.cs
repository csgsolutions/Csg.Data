using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csg.Data.Sql.Tests;

[TestClass]
public class DbConvertTests
{
    [TestMethod]
    public void Test_TypeToDbType()
    {
        Assert.AreEqual(DbType.Boolean, DbConvert.TypeToDbType(typeof(bool)));
        Assert.AreEqual(DbType.Boolean, DbConvert.TypeToDbType(typeof(bool?)));

        Assert.AreEqual(DbType.Byte, DbConvert.TypeToDbType(typeof(byte)));
        Assert.AreEqual(DbType.Byte, DbConvert.TypeToDbType(typeof(byte?)));

        Assert.AreEqual(DbType.Int16, DbConvert.TypeToDbType(typeof(short)));
        Assert.AreEqual(DbType.Int16, DbConvert.TypeToDbType(typeof(short?)));

        Assert.AreEqual(DbType.Int32, DbConvert.TypeToDbType(typeof(int)));
        Assert.AreEqual(DbType.Int32, DbConvert.TypeToDbType(typeof(int?)));

        Assert.AreEqual(DbType.Int64, DbConvert.TypeToDbType(typeof(long)));
        Assert.AreEqual(DbType.Int64, DbConvert.TypeToDbType(typeof(long?)));

        Assert.AreEqual(DbType.Single, DbConvert.TypeToDbType(typeof(float)));
        Assert.AreEqual(DbType.Single, DbConvert.TypeToDbType(typeof(float?)));

        Assert.AreEqual(DbType.Double, DbConvert.TypeToDbType(typeof(double)));
        Assert.AreEqual(DbType.Double, DbConvert.TypeToDbType(typeof(double?)));

        Assert.AreEqual(DbType.Decimal, DbConvert.TypeToDbType(typeof(decimal)));
        Assert.AreEqual(DbType.Decimal, DbConvert.TypeToDbType(typeof(decimal?)));

        Assert.AreEqual(DbType.String, DbConvert.TypeToDbType(typeof(string)));
        Assert.AreEqual(DbType.String, DbConvert.TypeToDbType(typeof(string)));

        Assert.AreEqual(DbType.Guid, DbConvert.TypeToDbType(typeof(Guid)));
        Assert.AreEqual(DbType.Guid, DbConvert.TypeToDbType(typeof(Guid?)));

        Assert.AreEqual(DbType.DateTime2, DbConvert.TypeToDbType(typeof(DateTime)));
        Assert.AreEqual(DbType.DateTime2, DbConvert.TypeToDbType(typeof(DateTime?)));

        Assert.AreEqual(DbType.DateTimeOffset, DbConvert.TypeToDbType(typeof(DateTimeOffset)));
        Assert.AreEqual(DbType.DateTimeOffset, DbConvert.TypeToDbType(typeof(DateTimeOffset?)));

        Assert.AreEqual(DbType.Time, DbConvert.TypeToDbType(typeof(TimeSpan)));
        Assert.AreEqual(DbType.Time, DbConvert.TypeToDbType(typeof(TimeSpan?)));

        Assert.AreEqual(DbType.StringFixedLength, DbConvert.TypeToDbType(typeof(char)));
        Assert.AreEqual(DbType.StringFixedLength, DbConvert.TypeToDbType(typeof(char?)));

        Assert.AreEqual(DbType.Binary, DbConvert.TypeToDbType(typeof(byte[])));
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
        var table = SqlTableBase.Create("foo");
        var filter1 = new SqlListFilter<int>(table, "Foo", new List<int>());
        var filter2 = new SqlListFilter<int?>(table, "Foo", new List<int?>());

        Assert.AreEqual(DbType.Int32, filter1.DataType);
        Assert.AreEqual(DbType.Int32, filter2.DataType);
    }
}