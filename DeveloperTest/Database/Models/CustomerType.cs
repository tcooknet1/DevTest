using System.Collections.Generic;

namespace DeveloperTest.Database.Models
{
    public class CustomerType
    {
        public int CustomerTypeId { get; set; }

        public string Description { get; set; }

        public List<Customer> Customers { get; set; }
    }
}
