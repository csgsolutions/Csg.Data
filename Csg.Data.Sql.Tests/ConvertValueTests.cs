using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestProject
{
    [TestClass]
    public class ConvertValueTests
    {
        [TestMethod]
        public void TestConvertInt16()
        {
            short value = 123;
            object valueAsObject = 123;
            string valueAsString = value.ToString();            

            Assert.AreEqual(value, Csg.Data.util.ConvertValue(valueAsString, System.Data.DbType.Int16));
            Assert.AreEqual(value, Csg.Data.util.ConvertValue(valueAsObject, System.Data.DbType.Int16));
        }

        [TestMethod]
        public void TestConvertInt32()
        {
            int value = 123;
            string valueAsString = value.ToString();
            object valueAsObject = 123;

            Assert.AreEqual(value, Csg.Data.util.ConvertValue(valueAsString, System.Data.DbType.Int32));
            Assert.AreEqual(value, Csg.Data.util.ConvertValue(valueAsObject, System.Data.DbType.Int32));
        }

        [TestMethod]
        public void TestConvertInt64()
        {
            long value = 123;
            string valueAsString = value.ToString();
            object valueAsObject = 123;

            Assert.AreEqual(value, Csg.Data.util.ConvertValue(valueAsString, System.Data.DbType.Int64));
            Assert.AreEqual(value, Csg.Data.util.ConvertValue(valueAsObject, System.Data.DbType.Int64));
        }

        [TestMethod]
        public void TestConvertSingle()
        {
            Single value = 123.456F;
            string valueAsString = value.ToString();
            object valueAsObject = 123.456;

            Assert.AreEqual(value, Csg.Data.util.ConvertValue(valueAsString, System.Data.DbType.Single));
            Assert.AreEqual(value, Csg.Data.util.ConvertValue(valueAsObject, System.Data.DbType.Single));
        }

        [TestMethod]
        public void TestConvertDouble()
        {
            Double value = 123.456;
            string valueAsString = value.ToString();
            object valueAsObject = 123.456;

            Assert.AreEqual(value, Csg.Data.util.ConvertValue(valueAsString, System.Data.DbType.Double));
            Assert.AreEqual(value, Csg.Data.util.ConvertValue(valueAsObject, System.Data.DbType.Double));
        }

        [TestMethod]
        public void TestConvertDecimal()
        {
            decimal value = 123.456M;
            string valueAsString = value.ToString();
            object valueAsObject = 123.456;

            Assert.AreEqual(value, Csg.Data.util.ConvertValue(valueAsString, System.Data.DbType.Decimal));
            Assert.AreEqual(value, Csg.Data.util.ConvertValue(valueAsObject, System.Data.DbType.Decimal));
        }

        [TestMethod]
        public void TestConvertCurrency()
        {
            decimal value = 123.456M;
            string valueAsString = value.ToString();
            object valueAsObject = 123.456;

            Assert.AreEqual(value, Csg.Data.util.ConvertValue(valueAsString, System.Data.DbType.Currency));
            Assert.AreEqual(value, Csg.Data.util.ConvertValue(valueAsObject, System.Data.DbType.Currency));
        }

        [TestMethod]
        public void TestConvertBoolean()
        {
            bool value = true;
            string valueAsString = value.ToString();
            object valueAsObject = true;

            Assert.AreEqual(value, Csg.Data.util.ConvertValue(valueAsString, System.Data.DbType.Boolean));
            Assert.AreEqual(value, Csg.Data.util.ConvertValue(valueAsObject, System.Data.DbType.Boolean));
            Assert.AreEqual(value, Csg.Data.util.ConvertValue(1, System.Data.DbType.Boolean));
            Assert.AreEqual(false, Csg.Data.util.ConvertValue(0, System.Data.DbType.Boolean));
        }

        [TestMethod]
        public void TestConvertString()
        {
            string value = "123";
            object valueAsObject = "123";

            Assert.AreEqual(value, Csg.Data.util.ConvertValue(valueAsObject, System.Data.DbType.String));
            Assert.AreEqual(value, Csg.Data.util.ConvertValue(valueAsObject, System.Data.DbType.StringFixedLength));
            Assert.AreEqual(value, Csg.Data.util.ConvertValue(valueAsObject, System.Data.DbType.AnsiString));
            Assert.AreEqual(value, Csg.Data.util.ConvertValue(valueAsObject, System.Data.DbType.AnsiStringFixedLength));            
        }

        [TestMethod]
        public void TestConvertDateTime()
        {
            string value = "1/1/1900";
            DateTime date = DateTime.Parse(value);
            DateTimeOffset dateo = DateTime.Parse(value);
            object valueAsObject = "1/1/1900";

            Assert.AreEqual(date, Csg.Data.util.ConvertValue(valueAsObject, System.Data.DbType.Date));
            Assert.AreEqual(date, Csg.Data.util.ConvertValue(valueAsObject, System.Data.DbType.DateTime));
            Assert.AreEqual(date, Csg.Data.util.ConvertValue(valueAsObject, System.Data.DbType.DateTime2));
            Assert.AreEqual(dateo, Csg.Data.util.ConvertValue(valueAsObject, System.Data.DbType.DateTimeOffset));
        }
    }
}



