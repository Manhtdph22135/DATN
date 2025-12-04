using API.Context;
using API.DOT;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
// using BCrypt.Net; // Đã bỏ thư viện này vì không dùng nữa

namespace API.Controllers.Account
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DbContextShop _contextShop;
        private readonly IConfiguration _configuration;

        public AuthController(DbContextShop contextShop, IConfiguration configuration)
        {
            _contextShop = contextShop;
            _configuration = configuration;
        }
        [HttpGet("get-account")]
        public async Task<IActionResult> GetAccounts()
        {
            var accounts = await _contextShop.Accounts.Where(r => r.RoleId < 3).ToListAsync();
            return Ok(accounts);
        }
        [HttpPost("add-account-nhanvien")]
        public async Task<IActionResult> AddAccountNhanVien([FromBody] AccountNhanVienDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var newAcc = new Models.Account
            {
                Username = dto.Username,
                PasswordHash = dto.Password,  // nếu dùng hash thì hash tại đây
                RoleId = dto.RoleId,
                CreateAt = DateTime.UtcNow
            };

            _contextShop.Accounts.Add(newAcc);
            await _contextShop.SaveChangesAsync();

            return Ok(new { message = "Thêm tài khoản nhân viên thành công!" });
            return Ok(GetAccounts);
        }
        [HttpPut("update-account-nhanvien/{id}")]
        public async Task<IActionResult> UpdateAccountNhanVien(int id, [FromBody] AccountNhanVienDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var existingAcc = await _contextShop.Accounts.FindAsync(id);
            if (existingAcc == null)
                return NotFound(new { message = "Tài khoản không tồn tại!" });
            existingAcc.Username = dto.Username;
            existingAcc.PasswordHash = dto.Password; // nếu dùng hash thì hash tại đây
            existingAcc.RoleId = dto.RoleId;
            _contextShop.Accounts.Update(existingAcc);
            await _contextShop.SaveChangesAsync();
            return Ok(new { message = "Cập nhật tài khoản nhân viên thành công!" });
            return Ok(GetAccounts);
        }
        [HttpDelete("delete-account-nhanvien/{id}")]
        public async Task<IActionResult> DeleteAccountNhanVien(int id)
        {
            var existingAcc = await _contextShop.Accounts.FindAsync(id);
            if (existingAcc == null)
                return NotFound(new { message = "Tài khoản không tồn tại!" });
            _contextShop.Accounts.Remove(existingAcc);
            await _contextShop.SaveChangesAsync();
            return Ok(new { message = "Xóa tài khoản nhân viên thành công!" });
            return Ok(GetAccounts);
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            // 1. Validate dữ liệu cơ bản
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (model.Password != model.ConfirmPassword)
                return BadRequest(new { message = "Mật khẩu xác nhận không khớp!" });

            // 2. Kiểm tra Username đã tồn tại chưa
            if (await _contextShop.Accounts.AnyAsync(a => a.Username == model.Username))
                return BadRequest(new { message = "Tên đăng nhập đã tồn tại!" });

            // 3. Kiểm tra Email hoặc SĐT đã tồn tại trong bảng Customer chưa (tùy chọn, nhưng nên có)
            if (await _contextShop.Customers.AnyAsync(c => c.Email == model.Email))
                return BadRequest(new { message = "Email này đã được sử dụng!" });

            if(await _contextShop.Customers.AnyAsync(c => c.Phone == model.Phone))
                return BadRequest(new { message = "Số điện thoại này đã được sử dụng!" });

            // === BẮT ĐẦU TRANSACTION (Giao dịch) ===
            // Mục đích: Đảm bảo cả Account và Customer cùng được lưu thành công. 
            // Nếu 1 trong 2 lỗi, toàn bộ sẽ được hoàn tác (Rollback).
            using var transaction = await _contextShop.Database.BeginTransactionAsync();

            try
            {
                // --- BƯỚC A: TẠO ACCOUNT ---
                var newAccount = new Models.Account
                {
                    Username = model.Username,
                    // Lưu password thường (theo yêu cầu của bạn). 
                    // Nếu sau này muốn mã hóa thì dùng: BCrypt.Net.BCrypt.HashPassword(model.Password)
                    PasswordHash = model.Password,
                    RoleId = model.RoleId != 0 ? model.RoleId : 3, // Mặc định là Khách hàng (3)
                    CreateAt = DateTime.UtcNow
                };

                _contextShop.Accounts.Add(newAccount);
                await _contextShop.SaveChangesAsync(); // Lưu ngay để sinh ra AccountId

                // --- BƯỚC B: XỬ LÝ NGÀY SINH (DateOnly) ---
                DateOnly? birthDate = null;
                if (model.Dob != null)
                {
                    // Chuyển đổi từ DateTime (do API nhận) sang DateOnly (DB lưu)
                    birthDate = DateOnly.FromDateTime(model.Dob.Value);
                }

                // --- BƯỚC C: TẠO CUSTOMER ---
                var newCustomer = new Models.Customer
                {
                    // Gán AccountId vừa sinh ra ở trên vào đây -> QUAN TRỌNG NHẤT
                    AccountId = newAccount.AccountId,

                    FullName = $"{model.Ho} {model.Ten}".Trim(),
                    Email = model.Email,
                    Phone = model.Phone,
                    Gender = model.Sex, // true: Nam, false: Nữ
                    Dob = birthDate,
                    Address = model.Address,
                    CreateAt = DateTime.UtcNow,

                    // Các giá trị mặc định khác
                    RankMember = "Đồng",
                    Point = 1
                };

                _contextShop.Customers.Add(newCustomer);
                await _contextShop.SaveChangesAsync();

                // --- BƯỚC D: HOÀN TẤT ---
                await transaction.CommitAsync(); // Xác nhận lưu vào DB thật sự

                return Ok(new { message = "Đăng ký tài khoản thành công!" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                var innerMsg = ex.InnerException?.Message;

                Console.WriteLine("Lỗi đăng ký: " + ex);               // log đầy đủ
                Console.WriteLine("Inner: " + innerMsg);               // log inner

                return StatusCode(500, new
                {
                    message = "Lỗi Server khi đăng ký",
                    error = ex.Message,
                    inner = innerMsg
                });
            }

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDOT model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var account = await _contextShop.Accounts
                .FirstOrDefaultAsync(a => a.Username == model.Username);

            if (account == null)
                return Unauthorized(new { message = "Invalid username or password!" });

            // === THAY ĐỔI 2: SO SÁNH CHUỖI THƯỜNG ===
            // So sánh thẳng mật khẩu nhập vào với mật khẩu trong DB
            if (account.PasswordHash != model.Password)
            {
                return Unauthorized(new { message = "Invalid username or password!" });
            }

            // ... Phần tìm Customer và tạo Token giữ nguyên ...
            var customer = await _contextShop.Customers
                .FirstOrDefaultAsync(c => c.Email == account.Username || c.Phone == account.Username);

            string tokenString = GenerateJwtToken(account);

            return Ok(new
            {
                message = "Login successful!",
                token = tokenString,
                username = account.Username,
                fullName = customer?.FullName ?? account.Username,
                roleId = account.RoleId
            });
        }

        // ... Hàm GenerateJwtToken giữ nguyên ...
        private string GenerateJwtToken(Models.Account account)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var keyString = jwtSettings["SecretKey"];

            if (string.IsNullOrEmpty(keyString) || keyString.Length < 16)
            {
                throw new InvalidOperationException("JWT Secret Key is missing or too short.");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, account.Username ?? string.Empty),
                new Claim(ClaimTypes.NameIdentifier, account.AccountId.ToString()), // Sửa lại AccountId viết hoa cho đúng chuẩn C# thường dùng
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