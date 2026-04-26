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
            var role = User.FindFirst(ClaimTypes.Role)?.Value ?? User.FindFirst("role")?.Value;
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value ?? User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
            {
                return Unauthorized(new { message = "Invalid token claims" });
            }

            object? profile = role switch
            {
                "Admin" => _context.Admins
                    .Where(a => a.Id == userId)
                    .Select(a => new
                    {
                        a.Id,
                        a.Firstname,
                        a.Lastname,
                        a.Email,
                        Phone = a.Phone,
                        Bio = (string?)null,
                        Role = "Admin"
                    })
                    .FirstOrDefault(),
                "Organizer" => _context.Organizers
                    .Where(o => o.Id == userId)
                    .Select(o => new
                    {
                        o.Id,
                        o.Firstname,
                        o.Lastname,
                        o.Email,
                        Phone = o.Contactphone,
                        o.Bio,
                        Role = "Organizer",
                        o.Organizationname
                    })
                    .FirstOrDefault(),
                _ => _context.Users
                    .Where(u => u.Id == userId)
                    .Select(u => new
                    {
                        u.Id,
                        u.Firstname,
                        u.Lastname,
                        u.Email,
                        u.Phone,
                        u.Bio,
                        Role = "User"
                    })
                    .FirstOrDefault()
            };

            if (profile == null)
            {
                return NotFound(new { message = "User not found" });
            }

            return Ok(profile);
        }
   }
}
