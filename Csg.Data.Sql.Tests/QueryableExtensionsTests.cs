using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace TestProject
{
    [TestClass]
    public class QueryableExtensionTests
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
        public void TestTakePage1()
        {
            var list = GetIntList(999).AsQueryable();

            int pageCount;

            var page = Csg.Data.Linq.QueryableExtensions.TakePage<int>(list, 1, 10, out pageCount);

            Assert.AreEqual(100, pageCount);
            Assert.AreEqual(10, page.Count());
            Assert.AreEqual(1, page.First());
            Assert.AreEqual(10, page.Last());

            page = Csg.Data.Linq.QueryableExtensions.TakePage<int>(list, 2, 10, out pageCount);

            Assert.AreEqual(100, pageCount);
            Assert.AreEqual(10, page.Count());
            Assert.AreEqual(11, page.First());
            Assert.AreEqual(20, page.Last());

            page = Csg.Data.Linq.QueryableExtensions.TakePage<int>(list, 101, 10, out pageCount);

            Assert.AreEqual(100, pageCount);
            Assert.AreEqual(9, page.Count());
            Assert.AreEqual(991, page.First());
            Assert.AreEqual(999, page.Last());
        }

        [TestMethod]
        public void TestTakePage2()
        {
            var list = GetIntList(999).AsQueryable();

            int pageCount;
            int itemCount;

            var page = Csg.Data.Linq.QueryableExtensions.TakePage<int>(list, 1, 10, out pageCount, out itemCount);

            Assert.AreEqual(999, itemCount);
            Assert.AreEqual(100, pageCount);
            Assert.AreEqual(10, page.Count());
            Assert.AreEqual(1, page.First());
            Assert.AreEqual(10, page.Last());

            page = Csg.Data.Linq.QueryableExtensions.TakePage<int>(list, 2, 10, out pageCount, out itemCount);

            Assert.AreEqual(999, itemCount);
            Assert.AreEqual(100, pageCount);
            Assert.AreEqual(10, page.Count());
            Assert.AreEqual(11, page.First());
            Assert.AreEqual(20, page.Last());

            page = Csg.Data.Linq.QueryableExtensions.TakePage<int>(list, 101, 10, out pageCount, out itemCount);

            Assert.AreEqual(999, itemCount);
            Assert.AreEqual(100, pageCount);
            Assert.AreEqual(9, page.Count());
            Assert.AreEqual(991, page.First());
            Assert.AreEqual(999, page.Last());
        }

        [TestMethod]
        public void TestOrderBy()
        {
            var list = GetListOfPeople();
                        
            var sortedByLastName = Csg.Data.Linq.QueryableExtensions.OrderBy(list.AsQueryable(), "LastName");
            Assert.AreEqual(5, sortedByLastName.Count());
            Assert.AreEqual("Dole", sortedByLastName.First().LastName);
            Assert.AreEqual("Zulu", sortedByLastName.Last().LastName);

            var sortedByLastNameDesc = Csg.Data.Linq.QueryableExtensions.OrderBy(list.AsQueryable(), "LastName", sortAsc: false);
            Assert.AreEqual(5, sortedByLastName.Count());
            Assert.AreEqual("Zulu", sortedByLastNameDesc.First().LastName);
            Assert.AreEqual("Dole", sortedByLastNameDesc.Last().LastName);
        }
    }
}
