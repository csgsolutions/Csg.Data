using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Csg.Data.Sql;

namespace Csg.Data.Sql.Tests
{
    [TestClass]
    public class SqlServerTests
    {
        const string ValueWithOpenBracket = "Foo[Bar";
        const string ValueWithCloseBracket = "Foo]Bar";
        const string StupidButValidColumnName1 = "FooBar\"";
        const string StupidButValidColumnName2 = "FooBar]";

        [TestMethod]
        public void TestBracketIdentifierFormatting()
        {
            Assert.AreEqual("[FirstName]", SqlTextWriter.FormatSqlServerIdentifier("FirstName"));
            Assert.AreEqual("[FirstName]", SqlTextWriter.FormatSqlServerIdentifier("[FirstName]"));
            Assert.AreEqual("\"First];--Name\"", SqlTextWriter.FormatSqlServerIdentifier("First];--Name"));
            Assert.AreEqual("\"First]\"\";--Name\"", SqlTextWriter.FormatSqlServerIdentifier("First]\";--Name"));
            Assert.AreEqual("\"[]\"", SqlTextWriter.FormatSqlServerIdentifier("[]"));
            Assert.AreEqual("[\"]", SqlTextWriter.FormatSqlServerIdentifier("\""));
            Assert.AreEqual("[]", SqlTextWriter.FormatSqlServerIdentifier(""));
        }

        [TestMethod]
        public void TestQuotedIdentifierFormatting()
        {
            Assert.AreEqual("\"FirstName\"", SqlTextWriter.FormatSqlServerIdentifier("FirstName", true));
            Assert.AreEqual("\"FirstName\"", SqlTextWriter.FormatSqlServerIdentifier("\"FirstName\"", true));
            Assert.AreEqual("\"First\"\";--Name\"", SqlTextWriter.FormatSqlServerIdentifier("First\";--Name", true));
            Assert.AreEqual("\"[]\"", SqlTextWriter.FormatSqlServerIdentifier("[]", true));
            Assert.AreEqual("\"\"\"\"", SqlTextWriter.FormatSqlServerIdentifier("\"", true));
            Assert.AreEqual("\"\"", SqlTextWriter.FormatSqlServerIdentifier("", true));
        }
    }
}
