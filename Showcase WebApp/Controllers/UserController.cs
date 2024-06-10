using Azure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Showcase_WebApp.data;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;

namespace Showcase_WebApp.Controllers
{
    public class UserRoleUpdate
    {
        public string UserId { get; set; }
        public string RoleName { get; set; }
    }

    public class UserRoleData
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string role { get; set; }
    }

    [ApiController]

    [Route("/api/[controller]")]
    public class UserController : Controller
    {
        private readonly ILogger logger;
        private DataContext _dataContext;

        public UserController(ILogger<ContactFormController> logger, DataContext dataContext)
        {
            this.logger = logger;
            this._dataContext = dataContext;
        }

        [HttpGet("GetUserData")]
        public async Task<IActionResult> get([FromServices] IServiceProvider sp)
        {
            var userManager = sp.GetRequiredService<UserManager<IdentityUser>>();

            var user = await userManager.GetUserAsync(User);
            var roles = await userManager.GetRolesAsync(user);


            if (user == null)
            {
                return BadRequest();
            }

            return Ok(new {userName = user.UserName, role = roles[0] });
        }

        [HttpGet("GetAllUsers")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllUsers([FromServices] IServiceProvider sp)
        {
            var userManager = sp.GetRequiredService<UserManager<IdentityUser>>();

            List<IdentityUser> users = await _dataContext.Users.ToListAsync();

            List<UserRoleData> userRoles = new();

            foreach (var user in users)
            {
                var roles = await userManager.GetRolesAsync(user);

                var userRole = new UserRoleData()
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    role = roles.Count > 0 ? roles[0] : null
                };
                userRoles.Add(userRole);
            }


            return Ok(userRoles);
        }

        [HttpPost("UpdateUserRole")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Update([FromBody] UserRoleUpdate roleUpdate, [FromServices] IServiceProvider sp)
        {
            if (!ModelState.IsValid) return BadRequest();

            var userManager = sp.GetRequiredService<UserManager<IdentityUser>>();

            var user = await userManager.FindByIdAsync(roleUpdate.UserId);

            if (user == null) return BadRequest();

            if (await userManager.IsInRoleAsync(user, roleUpdate.RoleName)) return Ok();

            var roles = await userManager.GetRolesAsync(user);

            if (roles.Contains("Admin")) return Unauthorized();

            await userManager.RemoveFromRolesAsync(user, roles);

            await userManager.AddToRoleAsync(user, roleUpdate.RoleName);

            return Ok();
        }

    }
}
