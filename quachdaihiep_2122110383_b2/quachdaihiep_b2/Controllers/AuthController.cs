
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using quachdaihiep_b2.Data;
using quachdaihiep_b2.Model;

namespace quachdaihiep_b2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // Đăng nhập
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel loginModel)
        {
            if (loginModel == null || string.IsNullOrEmpty(loginModel.Email) || string.IsNullOrEmpty(loginModel.Password))
            {
                return BadRequest("Email và mật khẩu không được để trống.");
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == loginModel.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginModel.Password, user.Password))
            {
                return Unauthorized("Thông tin đăng nhập không hợp lệ.");
            }

            var token = GenerateJwtToken(user);
            return Ok(new
            {
                token,
                username = user.Username,
                email = user.Email
            });
        }

        // Tạo JWT Token
        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Lấy tất cả username
        [HttpGet("usernames")]
        public IActionResult GetAllUsernames()
        {
            var usernames = _context.Users
                .Select(u => u.Username)
                .ToList();

            return Ok(usernames);
        }

        // Lấy username theo id
        [HttpGet("username/{id}")]
        public IActionResult GetUsernameById(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound("Không tìm thấy người dùng.");
            }

            return Ok(new { user.Username });
        }
    }

    // Model truyền vào khi đăng nhập
    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
