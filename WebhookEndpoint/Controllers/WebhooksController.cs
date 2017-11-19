using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebhookEndpoint.Models;
using WebhookEndpoint.Services;

namespace WebhookEndpoint.Controllers
{
    [Route("api/[controller]")]
    public class WebhooksController : Controller
    {
        private readonly IWebhookService _service;

        public WebhooksController(IWebhookService service)
        {
            service = _service;
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
