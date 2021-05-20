using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TelesureHASTest.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using AzureFunctions.Extensions.Swashbuckle.Attribute;
using AzureFunctions.Extensions.Swashbuckle;
using System.Net.Http;
using TelesureHASTest.Logger;

namespace TelesureHASTest
{
    public class CompanyTrigger
    {
        private readonly CompanyDBContext _context;
        private readonly ICustomLogger _customLogger;
         
        public CompanyTrigger(CompanyDBContext context, ICustomLogger customLogger)
        {
            _context = context;
            _customLogger = customLogger;
        }

        [SwaggerIgnore]
        [FunctionName("Swagger")]
        public Task<HttpResponseMessage> Swagger(
              [HttpTrigger(AuthorizationLevel.Function, "get", Route = "swagger/json")] HttpRequestMessage req,
              [SwashBuckleClient] ISwashBuckleClient swashBuckleClient)
        {

            return Task.FromResult(swashBuckleClient.CreateSwaggerJsonDocumentResponse(req));
        }

        [SwaggerIgnore]
        [FunctionName("SwaggerUI")]
        public Task<HttpResponseMessage> SwaggerUI(
              [HttpTrigger(AuthorizationLevel.Function, "get", Route = "swagger/ui")] HttpRequestMessage req,
              [SwashBuckleClient] ISwashBuckleClient swashBuckleClient)
        {

            return Task.FromResult(swashBuckleClient.CreateSwaggerUIResponse(req, "swagger/json"));
        }

        [FunctionName("People")]
        public async Task<IActionResult> People(
              [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
              ILogger log)
        {
            try
            {
                var people = _context.People
                 .Include(p => p.Title)
                 .Select(p => new
                 {
                     Title = p.Title.Name,
                     p.Name,
                     p.Surname
                 })
                 .ToList();

                return new OkObjectResult(people);
            }
            catch (Exception e)
            {
                _customLogger.Error("TelesureHASTest", e.Message);
                _customLogger.Error("TelesureHASTest", e.StackTrace);
                return new BadRequestObjectResult("An error occured while processing your request");
            }

        }

        [FunctionName("Employees")]
        public async Task<IActionResult> Employees(
              [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
              ILogger log)
        {
            try
            {
                var employees = _context.Employees
                 .Include(e => e.Person)
                 .ThenInclude(p => p.Title)
                 .Select(e => new
                 {
                     e.EmployeeNo,
                     Title = e.Person.Title.Name,
                     e.Person.Name,
                     e.Person.Surname
                 })
                 .ToList();

                return new OkObjectResult(employees);
            }
            catch(Exception e)
            {
                _customLogger.Error("TelesureHASTest", e.Message);
                _customLogger.Error("TelesureHASTest", e.StackTrace);
                return new BadRequestObjectResult("An error occured while processing your request");
            }
            
        }

        [FunctionName("Employee")]
        public async Task<IActionResult> Employee(
              [HttpTrigger(AuthorizationLevel.Function, "get", Route = "Employees/{id}")] HttpRequest req, string id,
              ILogger log)
        {
            try
            {
                var employee = _context.Employees
                 .Include(e => e.Person)
                 .ThenInclude(p => p.Title)
                 .Where(p => p.EmployeeNo == id)
                 .Select(e => new
                 {
                     e.EmployeeNo,
                     Title = e.Person.Title.Name,
                     e.Person.Name,
                     e.Person.Surname
                 })
                 .FirstOrDefault();

                return employee != null 
                    ? (ActionResult)new OkObjectResult(employee) 
                    : new BadRequestObjectResult($"Could not locate employee with Employee Number: {id}");
            }
            catch(Exception e)
            {
                _customLogger.Error("TelesureHASTest", e.Message);
                _customLogger.Error("TelesureHASTest", e.StackTrace);
                return new BadRequestObjectResult("An error occured while processing your request");
            }
        }

        [FunctionName("Todos")]
        public async Task<IActionResult> CallExternalAPI(
              [HttpTrigger(AuthorizationLevel.Function, "get", Route = "Todos/{id}")] HttpRequest req,
              string id,
              ILogger log)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                HttpRequestMessage newRequest = new HttpRequestMessage(HttpMethod.Get, string.Format("https://jsonplaceholder.typicode.com/todos/{0}", id));
                HttpResponseMessage response = await httpClient.SendAsync(newRequest);

                return response.IsSuccessStatusCode
                    ? (ActionResult)new OkObjectResult(response.Content.ReadAsStringAsync().Result)
                    : new BadRequestObjectResult("An error occured while processing your request");
            }
            catch (Exception e)
            {
                _customLogger.Error("TelesureHASTest", e.Message);
                _customLogger.Error("TelesureHASTest", e.StackTrace);
                return new BadRequestObjectResult("An error occured while processing your request");
            }
        }

        [FunctionName("SeedData")]
        public async Task<IActionResult> SeedData(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                //this is a function to seed database with test data, only execute once

                log.LogInformation("Seeding Database");

                var title1 = new TitleLookup
                {
                    Name = "Mr."
                };

                var title2 = new TitleLookup
                {
                    Name = "Mrs."
                };

                var title3 = new TitleLookup
                {
                    Name = "Ms."
                };

                var title4 = new TitleLookup
                {
                    Name = "Dr."
                };

                _context.Titles.AddRange(title1, title2, title3, title4);
                await _context.SaveChangesAsync();

                //Person Seed Data

                var person1 = new Person()
                {
                    Name = "Rian",
                    Surname = "van der Merwe",
                    Title = title1
                };

                var person2 = new Person()
                {
                    Name = "Elizabeth",
                    Surname = "van der Merwe",
                    Title = title2
                };

                var person3 = new Person()
                {
                    Name = "Karien",
                    Surname = "van der Merwe",
                    Title = title3
                };

                var person4 = new Person()
                {
                    Name = "Bernard",
                    Surname = "van der Merwe",
                    Title = title4
                };

                _context.People.AddRange(person1, person2, person3, person4);
                await _context.SaveChangesAsync();

                //Employee Seed Data

                var employee1 = new Employee()
                {
                    EmployeeNo = "VAN01",
                    Person = person1
                };

                var employee2 = new Employee()
                {
                    EmployeeNo = "VAN02",
                    Person = person2
                };

                var employee3 = new Employee()
                {
                    EmployeeNo = "VAN03",
                    Person = person3,
                };

                var employee4 = new Employee()
                {
                    EmployeeNo = "VAN04",
                    Person = person4
                };

                _context.Employees.AddRange(employee1, employee2, employee3, employee4);

                await _context.SaveChangesAsync();

                return new OkObjectResult(new
                {
                    TitleCount = _context.Titles.Count(),
                    PeopleCount = _context.People.Count(),
                    EmployeeCount = _context.Employees.Count()
                });
            }
            catch(Exception e)
            {
                _customLogger.Error("TelesureHASTest", e.Message);
                _customLogger.Error("TelesureHASTest", e.StackTrace);
                return new BadRequestObjectResult("An error occured while processing your request");
            }
        }
    }
}
