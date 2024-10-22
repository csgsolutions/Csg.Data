using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csg.Data.Sql.Tests;

[TestClass]
public class SqlServerTests
{
    private const string ValueWithOpenBracket = "Foo[Bar";
    private const string ValueWithCloseBracket = "Foo]Bar";
    private const string StupidButValidColumnName1 = "FooBar\"";
    private const string StupidButValidColumnName2 = "FooBar]";

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