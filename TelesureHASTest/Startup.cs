using AzureFunctions.Extensions.Swashbuckle;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TelesureHASTest;
using TelesureHASTest.Logger;

[assembly: FunctionsStartup(typeof(Startup))]
namespace TelesureHASTest
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddDbContext<CompanyDBContext>(
                  options => options.UseSqlServer("Server=(localdb)\\MSSQLLocalDB; Database=Company;Trusted_Connection=True;MultipleActiveResultSets=true"));

            builder.Services.AddScoped<ICustomLogger, CustomLogger>();

            builder.AddSwashBuckle(Assembly.GetExecutingAssembly());
        }
    }
}
