using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace TestProject
{
    [TestClass]
    public class EnumerableExtensionTests
    {
        private class Person
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }            
        }

        private IList<int> GetIntList(int count)
        {
            var list = new List<int>();

            for (var i = 1; i <= count; i++) 
            {
                list.Add(i);
            }

            return list;
        }

        private IList<Person> GetListOfPeople()
        {
            var list = new List<Person>();

            list.Add(new Person() { FirstName = "Bob", LastName = "Dole" });
            list.Add(new Person() { FirstName = "Jane", LastName = "Smith" });
            list.Add(new Person() { FirstName = "Aaron", LastName = "Zulu" });
            list.Add(new Person() { FirstName = "Stephen", LastName = "Grump" });
            list.Add(new Person() { FirstName = "Charles", LastName = "Munchkin" });

            return list;
        }

        [TestMethod]
        public void TestToArrayTable()
        {
            var list = GetListOfPeople();

            list.Add(new Person() { FirstName = "Null", LastName = null });

            var arrayTable = Csg.Data.EnumerableExtensions.ToArrayTable(list.AsQueryable());

            Assert.AreEqual(list.Count() + 1, arrayTable.Count());
            Assert.AreEqual(2, arrayTable.First().Count());
            Assert.IsTrue(arrayTable.All(x => x.Length == 2));

            Assert.AreEqual("FirstName", arrayTable[0][0]);
            Assert.AreEqual("LastName", arrayTable[0][1]);

            Assert.AreEqual("Bob", arrayTable[1][0].ToString());
            Assert.AreEqual("Dole", arrayTable[1][1].ToString());            
        }

        [TestMethod]
        public void TestToArrayTableWithExplicitPropertyNames()
        {
            var list = GetListOfPeople();

            list.Add(new Person() { FirstName = "Null", LastName = null });

            var arrayTable = Csg.Data.EnumerableExtensions.ToArrayTable(list.AsQueryable(), "LastName");

            Assert.AreEqual(list.Count() + 1, arrayTable.Count());
            Assert.AreEqual(1, arrayTable.First().Count());
            Assert.IsTrue(arrayTable.All(x => x.Length == 1));

            Assert.AreEqual("LastName", arrayTable[0][0]);

            Assert.AreEqual("Dole", arrayTable[1][0].ToString());
        }

        [TestMethod]
        public void TestToArrayPropertyNamesAreCaseSensative()
        {
            var list = GetListOfPeople();

            list.Add(new Person() { FirstName = "Null", LastName = null });

            var arrayTable = Csg.Data.EnumerableExtensions.ToArrayTable(list.AsQueryable(), "LastName", "FirstNAME");

            Assert.AreEqual(list.Count() + 1, arrayTable.Count());
            Assert.AreEqual(1, arrayTable.First().Count());
            Assert.IsTrue(arrayTable.All(x => x.Length == 1));

            Assert.AreEqual("LastName", arrayTable[0][0]);

            Assert.AreEqual("Dole", arrayTable[1][0].ToString());
        }
    }
}
