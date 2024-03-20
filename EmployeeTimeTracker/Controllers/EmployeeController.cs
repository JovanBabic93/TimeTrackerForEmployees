using EmployeeTimeTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace EmployeeTimeTracker.Controllers
{
    [Route("employee")]
    public class EmployeeController : Controller
    {
        Uri baseAdress = new Uri("https://rc-vault-fap-live-1.azurewebsites.net/api/gettimeentries?code=vO17RnE8vuzXzPJo5eaLLjXjmRW07law99QTD90zat9FfOQJKKUcgQ==");
        private readonly HttpClient _client;
        public EmployeeController()
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAdress;
        }
        [HttpGet]
        public IActionResult Index()

        {
            //Fetching the data
            List<Employee> employees = new List<Employee>();
            HttpResponseMessage response = _client.GetAsync(baseAdress).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                employees = JsonConvert.DeserializeObject<List<Employee>>(data);

                var groupedEmployees = employees
                    .Where(e => e.EmployeeName != null) // Filter out entries with null EmployeeName(option)
                    .GroupBy(e => e.EmployeeName)
                    .Select(group => new Employee
                    {
                        EmployeeName = group.Key,
                        TotalTimeWorked = Math.Round(group.Sum(e => (e.EndTimeUtc - e.StarTimeUtc).TotalHours), 2)
                    })
                    .OrderByDescending(e => e.TotalTimeWorked)
                    .ToList();            

                return View(groupedEmployees);
            }

            // If request fails, return an empty list
            return View(employees);
        }
    }
}
