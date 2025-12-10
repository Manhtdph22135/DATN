namespace API.DOT
{
    public class CustomerUpdateDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public bool Gender { get; set; }  // true = Nam, false = Nữ
        public string Dob { get; set; } = string.Empty; // yyyy-MM-dd
        public string Address { get; set; } = string.Empty;
    }
}
