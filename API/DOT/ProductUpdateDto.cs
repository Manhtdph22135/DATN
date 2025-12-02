using API.Models;

namespace API.DOT
{
    public class ProductUpdateDto
    {
        public Product Products { get; set; } = null!;
        public ProductDetailDto ProductDetails { get; set; } = null!;
        public Material Materials { get; set; } = null!;
        public Size Sizes { get; set; } = null!;
        public Color Colors { get; set; } = null!;
    }

    public class ProductDetailDto
    {
        public int ProductDetailId { get; set; }  // POST để 0
        public int StockQuantity { get; set; }
        public string? Image { get; set; }        // nhận base64 từ Vue
    }
}
