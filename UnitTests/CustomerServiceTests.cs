using DeveloperTest.Business;
using DeveloperTest.Database;
using DeveloperTest.Database.Models;
using DeveloperTest.Models;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Linq;

namespace UnitTests
{
    public class CustomerServiceTests
    {
        private ApplicationDbContext _dbContext;

        private CustomerService _customerService;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new ApplicationDbContext(options);

            _dbContext.Database.EnsureCreated();

            _dbContext.AddRange(
                new Customer { CustomerId = 1, Name = "Customer1", CustomerType = _dbContext.CustomerTypes.Single(x => x.CustomerTypeId == 1) },
                new Customer { CustomerId = 2, Name = "Customer2", CustomerType = _dbContext.CustomerTypes.Single(x => x.CustomerTypeId == 2) },
                new Customer { CustomerId = 3, Name = "Customer3", CustomerType = _dbContext.CustomerTypes.Single(x => x.CustomerTypeId == 2) });

            _dbContext.SaveChanges();

            _dbContext.AddRange(
                new Job { JobId = 1, Engineer = "Engineer1", When = new DateTime(2021, 07, 08), Customer = _dbContext.Customers.Single(x => x.CustomerId == 3) },
                new Job { JobId = 2, Engineer = "Engineer2", When = new DateTime(2021, 07, 09), Customer = _dbContext.Customers.Single(x => x.CustomerId == 1) },
                new Job { JobId = 3, Engineer = "Engineer3", When = new DateTime(2021, 07, 10), Customer = _dbContext.Customers.Single(x => x.CustomerId == 2) },
                new Job { JobId = 4, Engineer = "Engineer4", When = new DateTime(2021, 07, 11), Customer = _dbContext.Customers.Single(x => x.CustomerId == 3) });

            _dbContext.SaveChanges();

            _customerService = new CustomerService(_dbContext);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Dispose();
        }

        [Test]
        public void GetCustomers_Returns_All_CustomerModels()
        {   
            var result = _customerService.GetCustomers();

            Assert.That(result.Count, Is.EqualTo(3));

            Assert.That(result[0].CustomerId, Is.EqualTo(1));
            Assert.That(result[0].Name, Is.EqualTo("Customer1"));
            Assert.That(result[0].CustomerTypeId, Is.EqualTo(1));
            Assert.That(result[0].CustomerTypeDescription, Is.EqualTo("Small"));

            Assert.That(result[1].CustomerId, Is.EqualTo(2));
            Assert.That(result[1].Name, Is.EqualTo("Customer2"));
            Assert.That(result[1].CustomerTypeId, Is.EqualTo(2));
            Assert.That(result[1].CustomerTypeDescription, Is.EqualTo("Large"));

            Assert.That(result[2].CustomerId, Is.EqualTo(3));
            Assert.That(result[2].Name, Is.EqualTo("Customer3"));
            Assert.That(result[2].CustomerTypeId, Is.EqualTo(2));
            Assert.That(result[2].CustomerTypeDescription, Is.EqualTo("Large"));
        }

        [Test]
        public void GetCustomer_Given_Valid_CustomerId_Then_Returns_Correctly_Mapped_CustomerModel()
        {
            var result = _customerService.GetCustomer(2);

            Assert.That(result.CustomerId, Is.EqualTo(2));
            Assert.That(result.Name, Is.EqualTo("Customer2"));
            Assert.That(result.CustomerTypeId, Is.EqualTo(2));
            Assert.That(result.CustomerTypeDescription, Is.EqualTo("Large"));
        }

        [Test]
        public void GetCustomer_Given_Invalid_CustomerId_Then_Returns_Null()
        {
            var result = _customerService.GetCustomer(99);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void CreateCustomer_Given_Valid_Model_Then_Creates_CustomerRecord()
        {
            var baseModel = new BaseCustomerModel
            {
                Name = "New Customer Record",
                CustomerTypeId = 1
            };

            _customerService.CreateCustomer(baseModel);

            var storedRecord = _dbContext.Customers.SingleOrDefault(x => x.Name == baseModel.Name && x.CustomerType.CustomerTypeId == baseModel.CustomerTypeId);

            Assert.That(storedRecord, Is.Not.Null);
        }

        [Test]
        public void CreateCustomer_Given_Valid_Model_Then_Returns_Correctly_Mapped_CustomerModel()
        {
            var baseModel = new BaseCustomerModel
            {
                Name = "New Customer Record",
                CustomerTypeId = 1
            };

            var result = _customerService.CreateCustomer(baseModel);

            var storedRecord = _dbContext.Customers.SingleOrDefault(x => x.Name == baseModel.Name && x.CustomerType.CustomerTypeId == baseModel.CustomerTypeId);

            Assert.That(result.CustomerId, Is.EqualTo(storedRecord.CustomerId));
            Assert.That(result.Name, Is.EqualTo("New Customer Record"));
            Assert.That(result.CustomerTypeId, Is.EqualTo(1));
            Assert.That(result.CustomerTypeDescription, Is.EqualTo("Small"));
        }

        [Test]
        public void CreateCustomer_Given_Invalid_CustomerTypeId_Then_Exception_Thrown()
        {
            var baseModel = new BaseCustomerModel
            {
                Name = "New Customer Record",
                CustomerTypeId = 99
            };

            Assert.Throws<InvalidOperationException>(() => { _customerService.CreateCustomer(baseModel); });
        }

        [Test]
        public void GetCustomerTypes_Returns_All_CustomerTypeModels()
        {
            var result = _customerService.GetCustomerTypes();

            Assert.That(result.Count, Is.EqualTo(2));

            Assert.That(result[0].CustomerTypeId, Is.EqualTo(1));
            Assert.That(result[0].Description, Is.EqualTo("Small"));

            Assert.That(result[1].CustomerTypeId, Is.EqualTo(2));
            Assert.That(result[1].Description, Is.EqualTo("Large"));
        }

        [Test]
        public void GetCustomerType_Given_Valid_CustomerTypeId_Then_Returns_Correctly_Mapped_CustomerTypeModel()
        {
            var result = _customerService.GetCustomerType(2);

            Assert.That(result.CustomerTypeId, Is.EqualTo(2));
            Assert.That(result.Description, Is.EqualTo("Large"));
        }

        [Test]
        public void GetCustomerType_Given_Invalid_CustomerTypeId_Then_Returns_Null()
        {
            var result = _customerService.GetCustomerType(99);

            Assert.That(result, Is.Null);
        }
    }
}