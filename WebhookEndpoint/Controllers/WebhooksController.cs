using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebhookEndpoint.Models;
using WebhookEndpoint.Services;

namespace WebhookEndpoint.Controllers
{
    [Route("api/[controller]")]
    public class WebhooksController : Controller
    {
        private readonly IWebhooksService _service;
        private readonly ILogger<WebhooksController> _logger;

        public WebhooksController(IWebhooksService service, ILogger<WebhooksController> logger)
        {
            _service = service;
            _logger = logger;
        }

        // POST api/values
        [HttpPost]
        public IActionResult Post([FromBody]SfEvent sfEvent)
        {
            _service.ProcessEventAsync(sfEvent);
            return StatusCode(200);
        }
    }
}
