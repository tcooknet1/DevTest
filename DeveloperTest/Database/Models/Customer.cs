using System.Collections.Generic;

namespace DeveloperTest.Database.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }

        public string Name { get; set; }

        public CustomerType CustomerType { get; set; }

        public List<Job> Jobs { get; set; }
    }
}
