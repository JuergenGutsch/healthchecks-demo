using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using HealthCheck.MainApp.Models;
using Microsoft.Extensions.Diagnostics.HealthChecks;


namespace HealthCheck.MainApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly HealthCheckService _healthCheckService;

        public HomeController(
            HealthCheckService healthCheckService)
        {
            _healthCheckService = healthCheckService;
        }

        public async Task<IActionResult> Health()
        {
            var health = await _healthCheckService.CheckHealthAsync(_ => true);
            
            return View(health);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
