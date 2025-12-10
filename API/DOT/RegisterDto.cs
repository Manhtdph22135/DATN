using System.ComponentModel.DataAnnotations;

namespace API.DOT
{
    public class RegisterDto
    {
        public string Username { get; set; }

        // --- SỬA Ở ĐÂY: Bỏ bớt các điều kiện phức tạp nếu có ---
        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string ConfirmPassword { get; set; }

        public int RoleId { get; set; }

        // Thông tin cá nhân
        public string? Ho { get; set; } // Cho phép null (thêm dấu ?) nếu không bắt buộc
        public string? Ten { get; set; } // Cho phép null

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Định dạng email không hợp lệ")]
        public string Email { get; set; }

        public string? Phone { get; set; }

        public bool Sex { get; set; }

        // Lưu ý: Nếu bạn đã sửa JS gửi chuỗi "yyyy-MM-dd" thì để DateTime? hoặc DateOnly? đều được
        public DateTime? Dob { get; set; }

        public string? Address { get; set; }
    }
    public class UpdateAccountCustomerDto
    {
        public int AccountId { get; set; }

        // nếu muốn bắt buộc phải nhập mật khẩu hiện tại thì để [Required]
        public string? CurrentPassword { get; set; }

        [Required]
        public string NewPassword { get; set; } = string.Empty;
    }
}
