using System.Security.Claims;
using EventManagementDataAccessLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventManagementServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly EventManagementDbContext _context;

        public AdminController(EventManagementDbContext context)
        {
            _context = context;
        }

        [HttpGet("dashboard")]
        public IActionResult GetDashboard()
        {
            var adminIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (!Guid.TryParse(adminIdValue, out var adminId))
            {
                return Unauthorized(new { message = "Invalid token claims" });
            }

            var admin = _context.Admins
                .Where(a => a.Id == adminId)
                .Select(a => new
                {
                    a.Id,
                    a.Firstname,
                    a.Lastname,
                    a.Email
                })
                .FirstOrDefault();

            if (admin == null)
            {
                return NotFound(new { message = "Admin not found" });
            }

            var stats = new
            {
                users = _context.Users.Count(),
                admins = _context.Admins.Count(),
                organizers = _context.Organizers.Count(),
                events = _context.Events.Count(),
                pendingEvents = _context.Events.Count(e => e.Approvalstatus == "pending")
            };

            return Ok(new
            {
                role = "Admin",
                admin,
                stats
            });
        }
    }
}