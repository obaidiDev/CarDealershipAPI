using cdms.DTOs;
using cdms.Models;
using cdms.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace cdms.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserService _users;
        private readonly OtpService _otps;
        private readonly IConfiguration _config;

        public AuthController(UserService users, OtpService otps, IConfiguration config)
        {
            _users = users;
            _otps = otps;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest(new { error = "Email and password required" });

            var exists = await _users.GetByEmailAsync(dto.Email);
            if (exists != null) return BadRequest(new { error = "Email already exists" });

            var user = new User
            {
                Email = dto.Email,
                PasswordHash = UserService.HashPassword(dto.Password),
                Role = string.IsNullOrWhiteSpace(dto.Role) ? "Customer" : dto.Role,
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber,
                IsActive = true
            };

            await _users.AddAsync(user);
            _otps.Generate(user.Id, "register");

            return Ok(new { message = "User created. Check console for OTP to confirm registration." });
        }

        [HttpPost("verify-register")]
        public async Task<IActionResult> VerifyRegister([FromBody] VerifyDto dto)
        {
            var user = await _users.GetByEmailAsync(dto.Email);
            if (user == null) return BadRequest(new { error = "User not found" });

            if (!_otps.Validate(user.Id, dto.Code, "register"))
                return BadRequest(new { error = "Invalid or expired OTP" });

            user.IsActive = true;
            return Ok(new { message = "Registration confirmed. You can now login." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _users.GetByEmailAsync(dto.Email);
            if (user == null) return BadRequest(new { error = "Invalid credentials" });
            if (!user.IsActive) return BadRequest(new { error = "Account not active" });
            if (!await _users.VerifyPasswordAsync(user, dto.Password)) return BadRequest(new { error = "Invalid credentials" });

            _otps.Generate(user.Id, "login");
            return Ok(new { message = "OTP sent to console. Verify to receive JWT." });
        }

        [HttpPost("verify-login")]
        public async Task<IActionResult> VerifyLogin([FromBody] VerifyDto dto)
        {
            var user = await _users.GetByEmailAsync(dto.Email);
            if (user == null) return BadRequest(new { error = "User not found" });

            if (!_otps.Validate(user.Id, dto.Code, "login"))
                return BadRequest(new { error = "Invalid or expired OTP" });

            var key = _config["Jwt:Key"] ?? "THIS_IS_A_DEMO_SECRET";
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddMinutes(60),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Ok(new { token = tokenHandler.WriteToken(token) });
        }
    }
}
