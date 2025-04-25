    using Microsoft.AspNetCore.Mvc;
    using Microsoft.IdentityModel.Tokens;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using quachdaihiep_b2.Data;
    using quachdaihiep_b2.DTO;
    using quachdaihiep_b2.Model;
    using AutoMapper;
    using BCrypt.Net;
    //using Microsoft.AspNetCore.Authorization;

    namespace quachdaihiep_b2.Controllers
    {
        [ApiController]
        [Route("api/[controller]")]
        public class UserController : ControllerBase
        {
            private readonly AppDbContext _context;
            private readonly IConfiguration _config;
            private readonly IMapper _mapper;

            public UserController(AppDbContext context, IConfiguration config, IMapper mapper)
            {
                _context = context;
                _config = config;
                _mapper = mapper;
            }

            // Đăng ký người dùng
            [HttpPost("register")]
            public async Task<IActionResult> Register([FromBody] UserDTO userInput)
            {
                if (string.IsNullOrWhiteSpace(userInput.Password) || userInput.Password.Length < 6)
                {
                    return BadRequest("Mật khẩu phải có ít nhất 6 ký tự.");
                }

                // Kiểm tra xem người dùng đã tồn tại chưa
                if (_context.Users.Any(u => u.Username == userInput.Username))
                {
                    return BadRequest("Người dùng đã tồn tại.");
                }

                // Mã hóa mật khẩu
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(userInput.Password);

                // Ánh xạ từ DTO sang entity
                var user = _mapper.Map<User>(userInput);
                user.Password = hashedPassword; // Đảm bảo mật khẩu được mã hóa

                // Thêm người dùng vào cơ sở dữ liệu
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Ánh xạ lại từ User entity sang UserDTO để trả về
                var userDto = _mapper.Map<UserDTO>(user);
                return Ok(userDto);
            }


            // Đăng nhập người dùng
            [HttpPost("login")]
            public IActionResult Login([FromBody] UserDTO userInput)
            {
                var user = _context.Users.FirstOrDefault(u => u.Username == userInput.Username);
                if (user == null || !BCrypt.Net.BCrypt.Verify(userInput.Password, user.Password))
                {
                    return Unauthorized("Tên đăng nhập hoặc mật khẩu không đúng.");
                }

                // Ánh xạ từ User entity sang UserDTO để trả về thông tin người dùng
                var userDto = _mapper.Map<UserDTO>(user);

                // Tạo JWT token
                var token = GenerateJwtToken(user);

                return Ok(new
                {
                    token,
                    user = userDto  // Trả về UserDTO thay vì User entity
                });
            }


            // Hàm tạo JWT token
            private string GenerateJwtToken(User user)
            {
                var keyString = _config["Jwt:Key"] ?? "supersecretkey1234567890123456"; // fallback key
                var key32 = keyString.PadRight(32, 'x');

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key32));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                };

                var token = new JwtSecurityToken(
                    issuer: _config["Jwt:Issuer"],
                    audience: _config["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(1),
                    signingCredentials: creds
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
        }
    }
