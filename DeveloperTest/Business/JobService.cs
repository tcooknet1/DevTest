using System.Linq;
using DeveloperTest.Business.Interfaces;
using DeveloperTest.Database;
using DeveloperTest.Database.Models;
using DeveloperTest.Models;
using Microsoft.EntityFrameworkCore;

namespace DeveloperTest.Business
{
    public class JobService : IJobService
    {
        private readonly ApplicationDbContext context;
        
        public JobService(ApplicationDbContext context)
        {
            this.context = context;
        }

        public JobModel[] GetJobs()
        {
            return context.Jobs.Select(x => new JobModel
            {
                JobId = x.JobId,
                Engineer = x.Engineer,
                When = x.When,
                CustomerId = x.Customer != null ? x.Customer.CustomerId : 0,
                CustomerName = x.Customer != null ? x.Customer.Name : "Unknown",
                CustomerTypeDescription = x.Customer != null ? x.Customer.CustomerType.Description : null,
            }).OrderBy(x => x.JobId).ToArray();
        }

        public JobModel GetJob(int jobId)
        {
            return context.Jobs.Where(x => x.JobId == jobId).Select(x => new JobModel
            {
                JobId = x.JobId,
                Engineer = x.Engineer,
                When = x.When,
                CustomerId = x.Customer != null ? x.Customer.CustomerId : 0,
                CustomerName = x.Customer != null ? x.Customer.Name : "Unknown",
                CustomerTypeDescription = x.Customer != null ? x.Customer.CustomerType.Description : null,
            }).SingleOrDefault();
        }

        public JobModel CreateJob(BaseJobModel model)
        {
            // TODO: implement caching to reduce lookup cost
            var customer = context.Customers
                .Include(x => x.CustomerType)
                .Single(x => x.CustomerId == model.CustomerId );

            var addedJob = context.Jobs.Add(new Job
            {
                Engineer = model.Engineer,
                When = model.When,
                Customer = customer
            });

            context.SaveChanges();

            return new JobModel
            {
                JobId = addedJob.Entity.JobId,
                Engineer = addedJob.Entity.Engineer,
                When = addedJob.Entity.When,
                CustomerId = addedJob.Entity.Customer.CustomerId,
                CustomerName = addedJob.Entity.Customer.Name,
                CustomerTypeDescription = addedJob.Entity.Customer.CustomerType.Description,
            };
        }
    }
}
