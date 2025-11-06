namespace API.Models
{
    public class CartDetails
    {
        public int CartDetailID { get; set; }
        public int CartID { get; set; }
        public int ProductDetailID { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }
        public Carts Cart { get; set; }
    }
}
