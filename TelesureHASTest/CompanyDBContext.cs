using Microsoft.EntityFrameworkCore;
using TelesureHASTest.Models;

namespace TelesureHASTest
{
    public class CompanyDBContext : DbContext
    {
        public CompanyDBContext()
        {

        }

        public CompanyDBContext(DbContextOptions<CompanyDBContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB; Database=Company;Trusted_Connection=True;MultipleActiveResultSets=true");
            }
        }

        public DbSet<Person> People { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<TitleLookup> Titles { get; set; }
    }
}
