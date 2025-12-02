using System.ComponentModel.DataAnnotations;

namespace API.DOT
{
    public class LoginDOT
    {
        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; } = string.Empty;
    }
    public class AccountNhanVienDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        public int RoleId { get; set; }
    }

}
