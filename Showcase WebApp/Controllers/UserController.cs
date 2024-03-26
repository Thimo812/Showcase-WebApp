using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace Showcase_WebApp.Controllers
{

    [ApiController]
    [Route("/api/[controller]")]
    public class UserController : Controller
    {
        private readonly ILogger logger;

        public UserController(ILogger<ContactFormController> logger)
        {
            this.logger = logger;
        }

        [HttpGet("GetUsername")]
        public async Task<IActionResult> get()
        {
            var userName = User.Identity.Name;

            if (userName == null)
            {
                return BadRequest();
            }

            return Ok(userName);
        }
    }
}
