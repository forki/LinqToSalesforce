using System;
using System.Linq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace LinqToSalesforce.CsharpTests
{
    [TestFixture]
    public class TranslationTests
    {
        [Test]
        public void WhenSelectBirthAsBornDateAndIdCustomerWhereFirstnameIsPopoAndBirthGreatherOrEqualThan02Feb1985OrderByDate_ShouldBe2ComparisonAnd1SelectTokens()
        {
            var context = new FakeQueryContext();
            var customers = context.GetTable<Customer>();
            var dateTime = new DateTime(1985, 02, 11);
            var query =
                from c in customers
                where c.Firstname == "popo" && c.Birth >= dateTime
                orderby c.Birth descending
                select new
                {
                    c.Id,
                    BornDate = c.Birth
                };
            var soql = query.ToString();

            Assert.AreEqual(@"SELECT Id, Birth FROM Customer WHERE (Firstname = 'popo') AND (Birth >= 1985-02-11T00:00:00Z) ORDER BY Birth DESC", soql);
        }

        [Test]
        public void WhenSelectBirthAsBornDateAndIdCustomerWhereFirstnameIsPopoOrBirthGreatherOrEqualThan02Feb1985OrderByDate_ShouldBe2ComparisonAnd1SelectTokens()
        {
            var context = new FakeQueryContext();
            var customers = context.GetTable<Customer>();
            var dateTime = new DateTime(1985, 02, 11);
            var query =
                from c in customers
                where c.Firstname == "popo" || c.Birth >= dateTime
                orderby c.Birth descending
                select new
                {
                    c.Id,
                    BornDate = c.Birth
                };
            var soql = query.ToString();

            Assert.AreEqual(@"SELECT Id, Birth FROM Customer WHERE (Firstname = 'popo') OR (Birth >= 1985-02-11T00:00:00Z) ORDER BY Birth DESC", soql);
        }


        public class Entity1
        {
            [EntityField(false)]
            public int Id { get; set; }

            [EntityField(false)]
            public string Name { get; set; }

            [EntityField(false)]
            [JsonProperty("Cost")]
            public string Price { get; set; }
        }

        [Test]
        public void WhenProperyHasJsonPropertyAttributes_PropertyNameShouldBeAttributeValue()
        {
            var context = new FakeQueryContext();
            var entities = context.GetTable<Entity1>();
            var query =
                from c in entities
                where c.Name == "popo"
                select c;
            var soql = query.ToString();

            Assert.AreEqual(@"SELECT Id, Name, Cost FROM Entity1 WHERE Name = 'popo'", soql);
        }

        [Test]
        public void WhenOrderedByMultipleFields_ShouldHaveMultipleOrderByPredicates()
        {
            // Arrange
            var context = new FakeQueryContext();

            // Act
            var customers = context.GetTable<Customer>();
            var query = customers.OrderBy(x => x.Id).ThenByDescending(x => x.Lastname);

            // Assert
            var soql = query.ToString();
            Assert.AreEqual("SELECT  FROM Customer  ORDER BY Id, Lastname DESC", soql);
        }

        [Test]
        public void WhenPagingParameters_ShouldHaveLimitAndOffset()
        {
            // Arrange
            var context = new FakeQueryContext();

            // Act
            var customers = context.GetTable<Customer>();
            var query = customers.Skip(10).Take(5);

            // Assert
            var soql = query.ToString();
            Assert.AreEqual("SELECT  FROM Customer  LIMIT 5 OFFSET 10", soql);
        }

    }
}