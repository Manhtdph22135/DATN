namespace API.DOT
{
    public class RegisterDto
    {
        // Account Fields
        // Mặc định, tất cả các trường string/object đều có thể là null, 
        // nhưng nếu có [Required] thì sẽ báo lỗi nếu không gửi.
        // Tôi sẽ loại bỏ tất cả các [Required] để bạn tự kiểm tra trong Controller.

        public string Username { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public int RoleId { get; set; }

        // Customer Fields
        public string Ten { get; set; }
        public string Ho { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool Sex { get; set; }

        // **PHẦN ĐÃ SỬA:** Chuyển từ DateOnly? sang string để tránh lỗi parsing JSON tự động
        public string Dob { get; set; }

        public string Address { get; set; }
    }
}
