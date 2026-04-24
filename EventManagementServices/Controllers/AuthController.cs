using Microsoft.AspNetCore.Mvc;
using EventManagementDataAccessLayer.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EventManagementServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly EventManagementDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(EventManagementDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("signup")]
        public IActionResult Signup([FromBody] User signupData)
        {
            if (signupData == null)
            {
                return BadRequest(new { message = "Invalid data" });
            }

            // Check if user exists
            var existingUser = _context.Users.FirstOrDefault(u => u.Email == signupData.Email);
            if (existingUser != null)
            {
                return Conflict(new { message = "User already exists" });
            }

            signupData.Id = Guid.NewGuid();
            signupData.Createdat = DateTime.UtcNow;
            signupData.Updatedat = DateTime.UtcNow;
            signupData.Isverified = false;
            signupData.Isblocked = false;

            _context.Users.Add(signupData);
            _context.SaveChanges();

            return Ok(new { message = "Signup successful", user = new { signupData.Id, signupData.Email } });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest loginData)
        {
            if (loginData == null || string.IsNullOrEmpty(loginData.Email) || string.IsNullOrEmpty(loginData.Password))
            {
                return BadRequest(new { message = "Invalid email or password" });
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == loginData.Email && u.Password == loginData.Password);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }

            var token = GenerateJwtToken(user);

            return Ok(new { message = "Login successful", token = token, user = new { user.Id, user.Email, user.Firstname, user.Lastname } });
        }

        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? ""));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("Firstname", user.Firstname ?? ""),
                new Claim("Lastname", user.Lastname ?? "")
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
