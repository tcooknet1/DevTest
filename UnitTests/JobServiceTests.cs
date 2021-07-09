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
    public class JobServiceTests
    {
        private ApplicationDbContext _dbContext;

        private JobService _jobService;

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

            _jobService = new JobService(_dbContext);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Dispose();
        }

        [Test]
        public void GetJobs_Returns_All_JobModels()
        {   
            var result = _jobService.GetJobs();

            Assert.That(result.Count, Is.EqualTo(4));

            Assert.That(result[0].JobId, Is.EqualTo(1));
            Assert.That(result[0].Engineer, Is.EqualTo("Engineer1"));
            Assert.That(result[0].When, Is.EqualTo(new DateTime(2021, 07, 08)));
            Assert.That(result[0].CustomerId, Is.EqualTo(3));
            Assert.That(result[0].CustomerName, Is.EqualTo("Customer3"));
            Assert.That(result[0].CustomerTypeDescription, Is.EqualTo("Large"));

            Assert.That(result[1].JobId, Is.EqualTo(2));
            Assert.That(result[1].Engineer, Is.EqualTo("Engineer2"));
            Assert.That(result[1].When, Is.EqualTo(new DateTime(2021, 07, 09)));
            Assert.That(result[1].CustomerId, Is.EqualTo(1));
            Assert.That(result[1].CustomerName, Is.EqualTo("Customer1"));
            Assert.That(result[1].CustomerTypeDescription, Is.EqualTo("Small"));

            Assert.That(result[2].JobId, Is.EqualTo(3));
            Assert.That(result[2].Engineer, Is.EqualTo("Engineer3"));
            Assert.That(result[2].When, Is.EqualTo(new DateTime(2021, 07, 10)));
            Assert.That(result[2].CustomerId, Is.EqualTo(2));
            Assert.That(result[2].CustomerName, Is.EqualTo("Customer2"));
            Assert.That(result[2].CustomerTypeDescription, Is.EqualTo("Large"));

            Assert.That(result[3].JobId, Is.EqualTo(4));
            Assert.That(result[3].Engineer, Is.EqualTo("Engineer4"));
            Assert.That(result[3].When, Is.EqualTo(new DateTime(2021, 07, 11)));
            Assert.That(result[3].CustomerId, Is.EqualTo(3));
            Assert.That(result[3].CustomerName, Is.EqualTo("Customer3"));
            Assert.That(result[3].CustomerTypeDescription, Is.EqualTo("Large"));
        }

        [Test]
        public void GetJob_Given_Valid_JobId_Then_Returns_Correctly_Mapped_JobModel()
        {
            var result = _jobService.GetJob(3);

            Assert.That(result.JobId, Is.EqualTo(3));
            Assert.That(result.Engineer, Is.EqualTo("Engineer3"));
            Assert.That(result.When, Is.EqualTo(new DateTime(2021, 07, 10)));
            Assert.That(result.CustomerId, Is.EqualTo(2));
            Assert.That(result.CustomerName, Is.EqualTo("Customer2"));
            Assert.That(result.CustomerTypeDescription, Is.EqualTo("Large"));
        }

        [Test]
        public void GetJob_Given_Invalid_JobId_Then_Returns_Null()
        {
            var result = _jobService.GetJob(99);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void CreateJob_Given_Valid_Model_Then_Creates_JobRecord()
        {
            var baseModel = new BaseJobModel
            {
                CustomerId = 3,
                Engineer = "An engineer",
                When = new DateTime(2022, 2, 2)
            };

            _jobService.CreateJob(baseModel);

            var storedRecord = _dbContext.Jobs.SingleOrDefault(x => 
                x.Customer.CustomerId == baseModel.CustomerId
                && x.Engineer == baseModel.Engineer
                && x.When == baseModel.When);

            Assert.That(storedRecord, Is.Not.Null);
        }

        [Test]
        public void CreateJob_Given_Valid_Model_Then_Returns_Correctly_Mapped_JobModel()
        {
            var baseModel = new BaseJobModel
            {
                CustomerId = 3,
                Engineer = "An engineer",
                When = new DateTime(2022, 2, 2)
            };

            var result = _jobService.CreateJob(baseModel);

            var storedRecord = _dbContext.Jobs.SingleOrDefault(x =>
                x.Customer.CustomerId == baseModel.CustomerId
                && x.Engineer == baseModel.Engineer
                && x.When == baseModel.When);

            Assert.That(result.JobId, Is.EqualTo(storedRecord.JobId));
            Assert.That(result.Engineer, Is.EqualTo("An engineer"));
            Assert.That(result.When, Is.EqualTo(new DateTime(2022, 2, 2)));
            Assert.That(result.CustomerId, Is.EqualTo(3));
            Assert.That(result.CustomerName, Is.EqualTo("Customer3"));
            Assert.That(result.CustomerTypeDescription, Is.EqualTo("Large"));
        }

        [Test]
        public void CreateJob_Given_Invalid_CustomerId_Then_Exception_Thrown()
        {
            var baseModel = new BaseJobModel
            {
                CustomerId = 99,
                Engineer = "An engineer",
                When = new DateTime(2022, 2, 2)
            };

            Assert.Throws<InvalidOperationException>(() => { _jobService.CreateJob(baseModel); });
        }
    }
}