using E_Commerce_API.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_Commerce_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "UserOrAdmin")]
    public class ProfileController : ControllerBase
    {
        private readonly UserManager<User> _userManager;

        public ProfileController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet("profile")]

        [Consumes("application/json")]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status200OK)]
         [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
         [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Get My Profile")]
        [EndpointDescription("Get My Profile From Syestem")]
        public async Task<IActionResult> GetProfile()
        {
             var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

             var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound("User not found");

             return Ok(new
            {
                user.FullName,
                user.UserName,
                user.DateOfBirth,
                user.Email,
                user.PhoneNumber,
                user.Country
            });
        }
    }
    }
