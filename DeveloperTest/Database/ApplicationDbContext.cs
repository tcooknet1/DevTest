using Microsoft.EntityFrameworkCore;
using DeveloperTest.Database.Models;

namespace DeveloperTest.Database
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerType> CustomerTypes { get; set; }


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CustomerType>()
                .HasKey(x => x.CustomerTypeId);

            modelBuilder.Entity<CustomerType>()
                .HasData(new[]
                {
                    new CustomerType { CustomerTypeId = 1, Description="Small" },
                    new CustomerType { CustomerTypeId = 2, Description="Large" },
                });

            modelBuilder.Entity<Customer>()
                .HasKey(x => x.CustomerId);

            modelBuilder.Entity<Customer>()
                .Property(x => x.CustomerId)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Customer>()
                .HasOne(x => x.CustomerType)
                .WithMany(x => x.Customers)
                .IsRequired();

            modelBuilder.Entity<Job>()
                .HasKey(x => x.JobId);

            modelBuilder.Entity<Job>()
                .Property(x => x.JobId)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Job>()
                .HasOne(x => x.Customer)
                .WithMany(x => x.Jobs)
                .IsRequired(false);
        }
    }
}
