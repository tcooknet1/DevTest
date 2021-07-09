using System;
using Microsoft.AspNetCore.Mvc;
using DeveloperTest.Business.Interfaces;
using DeveloperTest.Models;
using Microsoft.Extensions.Logging;

namespace DeveloperTest.Controllers
{
    [ApiController, Route("[controller]")]
    public class JobController : ControllerBase
    {
        private readonly IJobService jobService;
        private readonly ILogger<JobController> _logger;
        private readonly ICustomerService _customerService;

        public JobController(IJobService jobService, ICustomerService customerService, ILogger<JobController> logger)
        {
            this.jobService = jobService;
            _customerService = customerService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(jobService.GetJobs());
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var job = jobService.GetJob(id);

            if (job == null)
            {
                return NotFound();
            }

            return Ok(job);
        }

        [HttpPost]
        public IActionResult Create(BaseJobModel model)
        { 
            if (model.When.Date < DateTime.Now.Date)
            {
                return BadRequest("Date cannot be in the past");
            }

            // TODO: implement caching to reduce lookup cost
            var customer = _customerService.GetCustomer(model.CustomerId);

            if (customer == null)
            {
                _logger.Log(LogLevel.Error, $"Failed to create Job, CustomerId '{model.CustomerId}' is invalid");

                return BadRequest("Customer is not valid");
            }

            var job = jobService.CreateJob(model);

            return Created($"job/{job.JobId}", job);
        }
    }
}