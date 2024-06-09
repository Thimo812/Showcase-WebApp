using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Showcase_WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private ILogger _logger;

        public LoginController(ILogger<LoginController> logger) { _logger = logger; }

        [HttpGet]
        public async Task<IActionResult> GetLoginStatus()
        {
            return User.Identity.IsAuthenticated ? Ok() : Unauthorized();
        }
    }
}
