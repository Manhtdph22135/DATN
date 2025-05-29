using API.Models;

namespace API.DOT
{
    public class ProductUpdateDto
    {
        public Product Products { get; set; }
        public ProductDetail ProductDetails { get; set; }
        public ProductCategory ProductCategories { get; set; }
        public Material Materials { get; set; }
        public Size Sizes { get; set; }
        public Color Colors { get; set; }
    }
}
