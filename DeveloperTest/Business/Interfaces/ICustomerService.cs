using DeveloperTest.Database.Models;
using DeveloperTest.Models;

namespace DeveloperTest.Business.Interfaces
{
    public interface ICustomerService
    {
        CustomerModel[] GetCustomers();

        CustomerModel GetCustomer(int customerId);

        CustomerModel CreateCustomer(BaseCustomerModel model);

        CustomerTypeModel[] GetCustomerTypes();

        CustomerType GetCustomerType(int id);
    }
}
