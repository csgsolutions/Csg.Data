using System;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csg.Data.Sql.Tests;

[TestClass]
public class ConvertValueTests
{
    [TestMethod]
    public void TestConvertInt16()
    {
        short value = 123;
        object valueAsObject = 123;
        var valueAsString = value.ToString();

        Assert.AreEqual(value, util.ConvertValue(valueAsString, DbType.Int16));
        Assert.AreEqual(value, util.ConvertValue(valueAsObject, DbType.Int16));
    }

    [TestMethod]
    public void TestConvertInt32()
    {
        var value = 123;
        var valueAsString = value.ToString();
        object valueAsObject = 123;

        Assert.AreEqual(value, util.ConvertValue(valueAsString, DbType.Int32));
        Assert.AreEqual(value, util.ConvertValue(valueAsObject, DbType.Int32));
    }

    [TestMethod]
    public void TestConvertInt64()
    {
        long value = 123;
        var valueAsString = value.ToString();
        object valueAsObject = 123;

        Assert.AreEqual(value, util.ConvertValue(valueAsString, DbType.Int64));
        Assert.AreEqual(value, util.ConvertValue(valueAsObject, DbType.Int64));
    }

    [TestMethod]
    public void TestConvertSingle()
    {
        var value = 123.456F;
        var valueAsString = value.ToString();
        object valueAsObject = 123.456;

        Assert.AreEqual(value, util.ConvertValue(valueAsString, DbType.Single));
        Assert.AreEqual(value, util.ConvertValue(valueAsObject, DbType.Single));
    }

    [TestMethod]
    public void TestConvertDouble()
    {
        var value = 123.456;
        var valueAsString = value.ToString();
        object valueAsObject = 123.456;

        Assert.AreEqual(value, util.ConvertValue(valueAsString, DbType.Double));
        Assert.AreEqual(value, util.ConvertValue(valueAsObject, DbType.Double));
    }

    [TestMethod]
    public void TestConvertDecimal()
    {
        var value = 123.456M;
        var valueAsString = value.ToString();
        object valueAsObject = 123.456;

        Assert.AreEqual(value, util.ConvertValue(valueAsString, DbType.Decimal));
        Assert.AreEqual(value, util.ConvertValue(valueAsObject, DbType.Decimal));
    }

    [TestMethod]
    public void TestConvertCurrency()
    {
        var value = 123.456M;
        var valueAsString = value.ToString();
        object valueAsObject = 123.456;

        Assert.AreEqual(value, util.ConvertValue(valueAsString, DbType.Currency));
        Assert.AreEqual(value, util.ConvertValue(valueAsObject, DbType.Currency));
    }

    [TestMethod]
    public void TestConvertBoolean()
    {
        var value = true;
        var valueAsString = value.ToString();
        object valueAsObject = true;

        Assert.AreEqual(value, util.ConvertValue(valueAsString, DbType.Boolean));
        Assert.AreEqual(value, util.ConvertValue(valueAsObject, DbType.Boolean));
        Assert.AreEqual(value, util.ConvertValue(1, DbType.Boolean));
        Assert.AreEqual(false, util.ConvertValue(0, DbType.Boolean));
    }

    [TestMethod]
    public void TestConvertString()
    {
        var value = "123";
        object valueAsObject = "123";

        Assert.AreEqual(value, util.ConvertValue(valueAsObject, DbType.String));
        Assert.AreEqual(value, util.ConvertValue(valueAsObject, DbType.StringFixedLength));
        Assert.AreEqual(value, util.ConvertValue(valueAsObject, DbType.AnsiString));
        Assert.AreEqual(value, util.ConvertValue(valueAsObject, DbType.AnsiStringFixedLength));
    }

    [TestMethod]
    public void TestConvertDateTime()
    {
        var value = "1/1/1900";
        var date = DateTime.Parse(value);
        DateTimeOffset dateo = DateTime.Parse(value);
        object valueAsObject = "1/1/1900";

        Assert.AreEqual(date, util.ConvertValue(valueAsObject, DbType.Date));
        Assert.AreEqual(date, util.ConvertValue(valueAsObject, DbType.DateTime));
        Assert.AreEqual(date, util.ConvertValue(valueAsObject, DbType.DateTime2));
        Assert.AreEqual(dateo, util.ConvertValue(valueAsObject, DbType.DateTimeOffset));
    }
}