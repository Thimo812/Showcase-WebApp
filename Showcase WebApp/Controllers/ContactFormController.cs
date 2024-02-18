using Microsoft.AspNetCore.Mvc;
using Showcase_WebApp.Exceptions;
using Showcase_WebApp.Managers;
using Showcase_WebApp.Models;
using System.Configuration;
using System.Net;

namespace Showcase_WebApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ContactFormController : ControllerBase
    {
        private readonly ILogger _logger;

        private ContactFormManager _contactFormManager;

        public ContactFormController(ILogger<ContactFormController> logger)
        {
            _logger = logger;
            _contactFormManager = new ContactFormManager();
        }

        [HttpPost("CreateRequest")]
        public async Task<IActionResult> Create([FromBody] ContactForm contactForm)
        {
            var responseCode = await _contactFormManager.SendMail(contactForm);
            return new StatusCodeResult(responseCode);
        }
    }
}
