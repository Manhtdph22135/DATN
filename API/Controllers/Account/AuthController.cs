using API.Context;
using API.DOT;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net; // Cần cài đặt package BCrypt.Net.Core

namespace API.Controllers.Account
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DbContextShop _contextShop;
        private readonly IConfiguration _configuration; // Inject IConfiguration

        // Constructor mới để nhận IConfiguration
        public AuthController(DbContextShop contextShop, IConfiguration configuration)
        {
            _contextShop = contextShop;
            _configuration = configuration;
        }

        // ... (Giữ nguyên [HttpGet] GetAccounts và [HttpPost("register")]) ...

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            // ... (Logic validation như cũ) ...

            if (await _contextShop.Accounts.AnyAsync(a => a.Username == model.Username))
                return BadRequest(new { message = "Username already exists!" });

            if (model.Password != model.ConfirmPassword)
                return BadRequest(new { message = "Passwords do not match!" });

            // === SỬ DỤNG BCRYPT ĐỂ MÃ HÓA MẬT KHẨU ===
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

            // Tạo Account
            var newAccount = new Models.Account
            {
                Username = model.Username,
                PasswordHash = passwordHash, // Mật khẩu đã được băm
                RoleId = model.RoleId != 0 ? model.RoleId : 3, // Mặc định là 3 (Customer)
                CreateAt = DateTime.UtcNow
            };

            // ... (Tạo newCustomer và SaveChangesAsync như cũ) ...

            return Ok(new { message = "Account registered successfully!" });
        }


        [HttpPost("login")]
        // Sử dụng LoginDto để nhận dữ liệu, tránh dùng Models.Account
        public async Task<IActionResult> Login([FromBody] LoginDOT model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var account = await _contextShop.Accounts
                .FirstOrDefaultAsync(a => a.Username == model.Username);

            if (account == null)
                return Unauthorized(new { message = "Invalid username or password!" });

            // === BƯỚC QUAN TRỌNG: XÁC MINH MẬT KHẨU BẰNG BCRYPT ===
            // So sánh Password người dùng nhập (model.Password) với Hash lưu trong DB (account.PasswordHash)
            if (!BCrypt.Net.BCrypt.Verify(model.Password, account.PasswordHash))
                return Unauthorized(new { message = "Invalid username or password!" });

            // Find the related customer (logic giữ nguyên)
            var customer = await _contextShop.Customers
                .FirstOrDefaultAsync(c => c.Email == account.Username || c.Phone == account.Username);

            // Generate JWT token
            string tokenString = GenerateJwtToken(account);

            // Return full data for frontend
            return Ok(new
            {
                message = "Login successful!",
                token = tokenString,
                username = account.Username,
                // Lấy tên nếu tìm thấy customer, nếu không thì dùng Username
                fullName = customer?.FullName ?? account.Username,
                roleId = account.RoleId
            });
        }

        // JWT token generation with claims based on role
        private string GenerateJwtToken(Models.Account account)
        {
            // Lấy Secret Key từ cấu hình (appsettings.json)
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var keyString = jwtSettings["SecretKey"];

            // Giải quyết lỗi ArgumentNullException: Kiểm tra key
            if (string.IsNullOrEmpty(keyString) || keyString.Length < 16)
            {
                // Thay vì ném lỗi 500 mơ hồ, ném lỗi rõ ràng hơn
                throw new InvalidOperationException("JWT Secret Key is missing or too short in appsettings.json.");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, account.Username ?? string.Empty),
                new Claim(ClaimTypes.NameIdentifier, account.AccountId.ToString()),
                // Dùng RoleName nếu bạn có, nếu không thì dùng logic này
                new Claim(ClaimTypes.Role, account.RoleId < 3 ? "Admin" : "User"),
                new Claim("Type", account.RoleId < 3 ? "Admin" : "User")
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}