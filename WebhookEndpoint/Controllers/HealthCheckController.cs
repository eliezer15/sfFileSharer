using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebhookEndpoint.Models;
using WebhookEndpoint.Services;

namespace WebhookEndpoint.Controllers
{
    [Route("api/[controller]")]
    public class HealthCheckController : Controller
    {
        private readonly IHealthCheckService _service;

        public HealthCheckController(IHealthCheckService healthCheck)
        {
            _service = healthCheck;
        }

        // POST api/values
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return new JsonResult(await _service.GetHealthAsync());
        }
    }
}
