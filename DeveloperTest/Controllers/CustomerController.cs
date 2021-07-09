using Microsoft.AspNetCore.Mvc;
using DeveloperTest.Business.Interfaces;
using DeveloperTest.Models;
using Microsoft.Extensions.Logging;

namespace DeveloperTest.Controllers
{
    [ApiController, Route("[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        private readonly ILogger<CustomerController> _logger;

        public CustomerController(ICustomerService customerService, ILogger<CustomerController> logger)
        {
            _customerService = customerService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_customerService.GetCustomers());
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var customer = _customerService.GetCustomer(id);

            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }

        [HttpGet("customertypes")]
        public IActionResult GetCustomerTypes()
        {
            return Ok(_customerService.GetCustomerTypes());
        }

        [HttpPost]
        public IActionResult Create(BaseCustomerModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Name) || model.Name.Length < 5)
            {
                return BadRequest("Customer Name must be at least 5 characters long");
            }

            // TODO: implement caching to reduce lookup cost
            var customerType = _customerService.GetCustomerType(model.CustomerTypeId);

            if (customerType == null)
            {
                _logger.Log(LogLevel.Error, $"Failed to create Customer, CustomerTypeId '{model.CustomerTypeId}' is invalid");

                return BadRequest("A valid Customer Type is required");
            }

            var customer = _customerService.CreateCustomer(model);

            return Created($"customer/{customer.CustomerId}", customer);
        }
    }
}