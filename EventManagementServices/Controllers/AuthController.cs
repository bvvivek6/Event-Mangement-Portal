using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using EventManagementDataAccessLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace EventManagementServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly EventManagementDbContext _context;
        private readonly IConfiguration _configuration;

        private const int SaltSize = 16;
        private const int KeySize = 32;
        private const int Iterations = 100_000;
        private const string PasswordPrefix = "pbkdf2";

        public AuthController(EventManagementDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("signup")]
        public IActionResult Signup([FromBody] AuthSignupRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { message = "Invalid data" });
            }

            var role = NormalizeRole(request.Role);
            if (role == null)
            {
                return BadRequest(new { message = "Invalid role" });
            }

            if (EmailExists(request.Email))
            {
                return Conflict(new { message = "Email already exists" });
            }

            if (string.Equals(role, Roles.Admin, StringComparison.OrdinalIgnoreCase) && !IsAdminRegistrationKeyValid(request.RegistrationKey))
            {
                return Unauthorized(new { message = "Invalid admin registration key" });
            }

            var now = DateTime.UtcNow;
            AuthenticatedAccount account;

            switch (role)
            {
                case Roles.Admin:
                    var admin = new Admin
                    {
                        Id = Guid.NewGuid(),
                        Firstname = request.Firstname?.Trim(),
                        Lastname = request.Lastname?.Trim(),
                        Email = request.Email.Trim(),
                        Password = HashPassword(request.Password),
                        Phone = request.Phone?.Trim(),
                        Createdat = now,
                        Updatedat = now,
                    };

                    _context.Admins.Add(admin);
                    _context.SaveChanges();

                    account = new AuthenticatedAccount(admin.Id, admin.Firstname, admin.Lastname, admin.Email, Roles.Admin);
                    break;

                case Roles.Organizer:
                    var organizer = new Organizer
                    {
                        Id = Guid.NewGuid(),
                        Firstname = request.Firstname?.Trim(),
                        Lastname = request.Lastname?.Trim(),
                        Email = request.Email.Trim(),
                        Password = HashPassword(request.Password),
                        Organizationname = request.OrganizationName?.Trim(),
                        Bio = request.Bio?.Trim(),
                        Profileimageurl = request.ProfileImageUrl?.Trim(),
                        Bannerimageurl = request.BannerImageUrl?.Trim(),
                        Contactphone = request.ContactPhone?.Trim(),
                        Addressline = request.AddressLine?.Trim(),
                        City = request.City?.Trim(),
                        State = request.State?.Trim(),
                        Country = request.Country?.Trim(),
                        Pincode = request.Pincode?.Trim(),
                        Website = request.Website?.Trim(),
                        Facebookurl = request.FacebookUrl?.Trim(),
                        Twitterurl = request.TwitterUrl?.Trim(),
                        Instagramurl = request.InstagramUrl?.Trim(),
                        Linkedinurl = request.LinkedInUrl?.Trim(),
                        Isblocked = false,
                        Isverified = false,
                        Createdat = now,
                        Updatedat = now,
                    };

                    _context.Organizers.Add(organizer);
                    _context.SaveChanges();

                    account = new AuthenticatedAccount(organizer.Id, organizer.Firstname, organizer.Lastname, organizer.Email, Roles.Organizer);
                    break;

                default:
                    var user = new User
                    {
                        Id = Guid.NewGuid(),
                        Firstname = request.Firstname?.Trim(),
                        Lastname = request.Lastname?.Trim(),
                        Email = request.Email.Trim(),
                        Password = HashPassword(request.Password),
                        Phone = request.Phone?.Trim(),
                        Bio = request.Bio?.Trim(),
                        Isverified = false,
                        Isblocked = false,
                        Createdat = now,
                        Updatedat = now,
                    };

                    _context.Users.Add(user);
                    _context.SaveChanges();

                    account = new AuthenticatedAccount(user.Id, user.Firstname, user.Lastname, user.Email, Roles.User);
                    break;
            }

            var token = GenerateJwtToken(account);

            return Ok(new { message = "Signup successful", token, role = account.Role, account });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] AuthLoginRequest loginData)
        {
            if (loginData == null || string.IsNullOrWhiteSpace(loginData.Email) || string.IsNullOrWhiteSpace(loginData.Password))
            {
                return BadRequest(new { message = "Invalid email or password" });
            }

            var role = NormalizeRole(loginData.Role) ?? Roles.User;
            AuthenticatedAccount? account = role switch
            {
                Roles.Admin => AuthenticateAdmin(loginData.Email, loginData.Password),
                Roles.Organizer => AuthenticateOrganizer(loginData.Email, loginData.Password),
                _ => AuthenticateUser(loginData.Email, loginData.Password)
            };

            if (account == null)
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }

            var token = GenerateJwtToken(account);

            return Ok(new { message = "Login successful", token, role = account.Role, account });
        }

        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            return Ok(new { message = "Logout successful" });
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            var account = GetCurrentAccount();
            if (account == null)
            {
                return Unauthorized(new { message = "Invalid token claims" });
            }

            return Ok(account);
        }

        private AuthenticatedAccount? AuthenticateUser(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email.Trim());
            if (user == null || user.Isblocked == true || !VerifyPassword(password, user.Password))
            {
                return null;
            }

            return new AuthenticatedAccount(user.Id, user.Firstname, user.Lastname, user.Email, Roles.User);
        }

        private AuthenticatedAccount? AuthenticateAdmin(string email, string password)
        {
            var admin = _context.Admins.FirstOrDefault(a => a.Email == email.Trim());
            if (admin == null || !VerifyPassword(password, admin.Password))
            {
                return null;
            }

            return new AuthenticatedAccount(admin.Id, admin.Firstname, admin.Lastname, admin.Email, Roles.Admin);
        }

        private AuthenticatedAccount? AuthenticateOrganizer(string email, string password)
        {
            var organizer = _context.Organizers.FirstOrDefault(o => o.Email == email.Trim());
            if (organizer == null || organizer.Isblocked == true || !VerifyPassword(password, organizer.Password))
            {
                return null;
            }

            return new AuthenticatedAccount(organizer.Id, organizer.Firstname, organizer.Lastname, organizer.Email, Roles.Organizer);
        }

        private AuthenticatedAccount? GetCurrentAccount()
        {
            var role = User.FindFirstValue(ClaimTypes.Role) ?? User.FindFirstValue("role");
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            if (!Guid.TryParse(userIdValue, out var userId) || string.IsNullOrWhiteSpace(role))
            {
                return null;
            }

            return role switch
            {
                Roles.Admin => _context.Admins
                    .Where(a => a.Id == userId)
                    .Select(a => new AuthenticatedAccount(a.Id, a.Firstname, a.Lastname, a.Email, Roles.Admin))
                    .FirstOrDefault(),
                Roles.Organizer => _context.Organizers
                    .Where(o => o.Id == userId)
                    .Select(o => new AuthenticatedAccount(o.Id, o.Firstname, o.Lastname, o.Email, Roles.Organizer))
                    .FirstOrDefault(),
                _ => _context.Users
                    .Where(u => u.Id == userId)
                    .Select(u => new AuthenticatedAccount(u.Id, u.Firstname, u.Lastname, u.Email, Roles.User))
                    .FirstOrDefault()
            };
        }

        private string GenerateJwtToken(AuthenticatedAccount account)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? ""));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, account.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, account.Email),
                new Claim(ClaimTypes.Email, account.Email),
                new Claim(ClaimTypes.Name, account.Email),
                new Claim(ClaimTypes.Role, account.Role),
                new Claim("role", account.Role),
                new Claim("Firstname", account.Firstname ?? string.Empty),
                new Claim("Lastname", account.Lastname ?? string.Empty)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private bool EmailExists(string email)
        {
            var normalizedEmail = email.Trim();
            return _context.Users.Any(u => u.Email == normalizedEmail)
                || _context.Admins.Any(a => a.Email == normalizedEmail)
                || _context.Organizers.Any(o => o.Email == normalizedEmail);
        }

        private string? NormalizeRole(string? role)
        {
            if (string.IsNullOrWhiteSpace(role))
            {
                return Roles.User;
            }

            return role.Trim().ToLowerInvariant() switch
            {
                "admin" => Roles.Admin,
                "organizer" => Roles.Organizer,
                "user" => Roles.User,
                _ => null
            };
        }

        private bool IsAdminRegistrationKeyValid(string? registrationKey)
        {
            var configuredKey = _configuration["Admin:RegistrationKey"];
            if (string.IsNullOrWhiteSpace(configuredKey))
            {
                return false;
            }

            return string.Equals(configuredKey.Trim(), registrationKey?.Trim(), StringComparison.Ordinal);
        }

        private static string HashPassword(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(SaltSize);
            var key = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                Iterations,
                HashAlgorithmName.SHA256,
                KeySize);

            return $"{PasswordPrefix}${Iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(key)}";
        }

        private static bool VerifyPassword(string password, string storedPassword)
        {
            if (string.IsNullOrWhiteSpace(storedPassword))
            {
                return false;
            }

            if (!storedPassword.StartsWith($"{PasswordPrefix}$", StringComparison.OrdinalIgnoreCase))
            {
                return string.Equals(password, storedPassword, StringComparison.Ordinal);
            }

            var parts = storedPassword.Split('$', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 4 || !int.TryParse(parts[1], out var iterations))
            {
                return false;
            }

            var salt = Convert.FromBase64String(parts[2]);
            var expectedKey = Convert.FromBase64String(parts[3]);
            var actualKey = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                iterations,
                HashAlgorithmName.SHA256,
                expectedKey.Length);

            return CryptographicOperations.FixedTimeEquals(actualKey, expectedKey);
        }
    }

    public static class Roles
    {
        public const string User = "User";
        public const string Admin = "Admin";
        public const string Organizer = "Organizer";
    }

    public record AuthenticatedAccount(Guid Id, string? Firstname, string? Lastname, string Email, string Role);

    public class AuthLoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string? Role { get; set; }
    }

    public class AuthSignupRequest
    {
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? Phone { get; set; }
        public string? Bio { get; set; }
        public string? OrganizationName { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? BannerImageUrl { get; set; }
        public string? ContactPhone { get; set; }
        public string? AddressLine { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? Pincode { get; set; }
        public string? Website { get; set; }
        public string? FacebookUrl { get; set; }
        public string? TwitterUrl { get; set; }
        public string? InstagramUrl { get; set; }
        public string? LinkedInUrl { get; set; }
        public string? Role { get; set; }
        public string? RegistrationKey { get; set; }
    }
}
