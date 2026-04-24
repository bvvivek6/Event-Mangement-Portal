using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EventManagementDataAccessLayer.Models;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace EventManagementServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserProfileController : ControllerBase
    {
        private readonly EventManagementDbContext _context;

        public UserProfileController(EventManagementDbContext context)
        {
            _context = context;
        }

        [HttpGet("profile")]
        public IActionResult GetProfile()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value ?? User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
            {
                return Unauthorized(new { message = "Invalid token claims" });
            }

            var user = _context.Users
                .Select(u => new 
                {
                    u.Id,
                    u.Firstname,
                    u.Lastname,
                    u.Email,
                    u.Phone,
                    u.Bio,
                })
                .FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            return Ok(user);
        }
   }
}
