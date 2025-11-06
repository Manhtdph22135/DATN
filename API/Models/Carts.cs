namespace API.Models
{
    public class Carts
    {
        public int CartID { get; set; }
        public int CustomerID { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int Status { get; set; } // 0 = Open, 1 = CheckedOut
        public ICollection<CartDetails> Details { get; set; } = new List<CartDetails>();
    }
}
