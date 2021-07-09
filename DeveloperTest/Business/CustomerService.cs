using System.Linq;
using DeveloperTest.Business.Interfaces;
using DeveloperTest.Database;
using DeveloperTest.Database.Models;
using DeveloperTest.Models;

namespace DeveloperTest.Business
{
    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDbContext _context;
        
        public CustomerService(ApplicationDbContext context)
        {
            _context = context;
        }

        public CustomerModel[] GetCustomers()
        {
            return _context.Customers.Select(x => new CustomerModel
            {
                CustomerId = x.CustomerId,
                Name = x.Name,
                CustomerTypeId = x.CustomerType.CustomerTypeId,
                CustomerTypeDescription = x.CustomerType.Description
            }).OrderBy(x => x.CustomerId).ToArray();
        }

        public CustomerModel GetCustomer(int customerId)
        {
            return _context.Customers.Where(x => x.CustomerId == customerId).Select(x => new CustomerModel
            {
                CustomerId = x.CustomerId,
                Name = x.Name,
                CustomerTypeId = x.CustomerType.CustomerTypeId,
                CustomerTypeDescription = x.CustomerType.Description
            }).SingleOrDefault();
        }

        public CustomerModel CreateCustomer(BaseCustomerModel model)
        {
            // TODO: implement caching to reduce lookup cost
            var customerType = _context.CustomerTypes.Single(x => x.CustomerTypeId == model.CustomerTypeId);

            var addedCustomer = _context.Customers.Add(new Customer
            {
                Name = model.Name,
                CustomerType = customerType
            });

            _context.SaveChanges();

            return new CustomerModel
            {
                CustomerId = addedCustomer.Entity.CustomerId,
                Name = addedCustomer.Entity.Name,
                CustomerTypeId = addedCustomer.Entity.CustomerType.CustomerTypeId,
                CustomerTypeDescription = addedCustomer.Entity.CustomerType.Description
            };
        }

        public CustomerTypeModel[] GetCustomerTypes()
        {
            return _context.CustomerTypes.Select(x => new CustomerTypeModel
            {
                CustomerTypeId = x.CustomerTypeId,
                Description = x.Description,
            }).OrderBy(x => x.CustomerTypeId).ToArray();
        }

        public CustomerType GetCustomerType(int id)
        {
            return _context.CustomerTypes.SingleOrDefault(x => x.CustomerTypeId == id);
        }
    }
}
