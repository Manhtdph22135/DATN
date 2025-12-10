using API.Context;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.DOT;
using Microsoft.AspNetCore.Http;

namespace API.Controllers.Products
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly DbContextShop _contextShop;
        private readonly IWebHostEnvironment _env;

        public ProductController(DbContextShop contextShop, IWebHostEnvironment env)
        {
            _contextShop = contextShop;
            _env = env;
        }

        // GET: api/Product
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);

            var query =
                from p in _contextShop.Products
                join pd in _contextShop.ProductDetails on p.ProductId equals pd.ProductId
                join c in _contextShop.ProductCategories on p.CategoryId equals c.CategoryId
                join m in _contextShop.Materials on pd.MaterialId equals m.MaterialId
                join s in _contextShop.Sizes on pd.SizeId equals s.SizeId
                join co in _contextShop.Colors on pd.ColorId equals co.ColorId
                join promo in _contextShop.Promotions on p.ProductId equals promo.ProductId into promoGroup
                from pm in promoGroup.DefaultIfEmpty()
                select new
                {
                    p.ProductId,
                    p.ProductName,
                    p.CreatedAt,
                    p.UpdateAt,
                    p.Status,
                    pd.ProductDetailId,
                    pd.StockQuantity,
                    pd.Image,
                    c.CategoryName,
                    c.CategoryId,
                    m.MaterialName,
                    s.SizeName,
                    s.SizeId,
                    co.ColorName,
                    co.ColorId,

                    IsPromoActive = pm != null
                                    && pm.Status == "Đang hoạt động"
                                    && pm.StartDate <= today
                                    && pm.EndDate >= today,

                    OriginalPrice = p.Price,

                    Price = (pm != null
                            && pm.Status == "Đang hoạt động"
                            && pm.StartDate <= today
                            && pm.EndDate >= today)
                            ? p.Price * (100 - pm.DiscountValue) / 100
                            : p.Price,

                    DiscountPercent = (pm != null
                                      && pm.Status == "Đang hoạt động"
                                      && pm.StartDate <= today
                                      && pm.EndDate >= today)
                                      ? pm.DiscountValue
                                      : 0
                };

            var result = await query.ToListAsync();
            return Ok(result);
        }

        // GET: api/Product/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetProduct(int id)
        {
            var query =
                from p in _contextShop.Products
                join pd in _contextShop.ProductDetails on p.ProductId equals pd.ProductId
                join c in _contextShop.ProductCategories on p.CategoryId equals c.CategoryId
                join m in _contextShop.Materials on pd.MaterialId equals m.MaterialId
                join s in _contextShop.Sizes on pd.SizeId equals s.SizeId
                join co in _contextShop.Colors on pd.ColorId equals co.ColorId
                where p.ProductId == id
                select new
                {
                    p.ProductId,
                    p.ProductName,
                    p.Price,
                    p.CreatedAt,
                    p.UpdateAt,
                    p.Status,
                    pd.ProductDetailId,
                    pd.StockQuantity,
                    pd.Image,
                    c.CategoryName,
                    c.Trademark,
                    m.MaterialName,
                    s.SizeName,
                    co.ColorName
                };

            var result = await query.ToListAsync();
            if (!result.Any())
            {
                return NotFound("Product not found.");
            }

            return Ok(result);
        }

        // GET: api/Product/by-category/3
        [HttpGet("by-category/{categoryId:int}")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsByCategory(int categoryId)
        {
            var query =
                from p in _contextShop.Products
                join pd in _contextShop.ProductDetails on p.ProductId equals pd.ProductId
                join c in _contextShop.ProductCategories on p.CategoryId equals c.CategoryId
                join m in _contextShop.Materials on pd.MaterialId equals m.MaterialId
                join s in _contextShop.Sizes on pd.SizeId equals s.SizeId
                join co in _contextShop.Colors on pd.ColorId equals co.ColorId
                where c.CategoryId == categoryId
                select new
                {
                    p.ProductId,
                    p.ProductName,
                    p.Price,
                    p.CreatedAt,
                    p.UpdateAt,
                    p.Status,
                    pd.ProductDetailId,
                    pd.StockQuantity,
                    pd.Image,
                    c.CategoryName,
                    c.Trademark,
                    m.MaterialName,
                    s.SizeName,
                    co.ColorName
                };

            var result = await query.ToListAsync();
            return Ok(result);
        }

        // PUT: api/Product/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutProducts(int id, [FromBody] ProductUpdateDto dto)
        {
            if (dto == null || dto.Products == null || dto.ProductDetails == null)
            {
                return BadRequest("Invalid body.");
            }

            if (id != dto.Products.ProductId)
            {
                return BadRequest("Product ID mismatch.");
            }

            var existingProduct = await _contextShop.Products.FindAsync(id);
            if (existingProduct == null)
            {
                return NotFound("Product not found.");
            }

            var existingProductDetail = await _contextShop.ProductDetails
                .FirstOrDefaultAsync(pd => pd.ProductDetailId == dto.ProductDetails.ProductDetailId
                                           && pd.ProductId == id);

            if (existingProductDetail == null)
            {
                return NotFound("Product detail not found.");
            }

            if (!await _contextShop.Sizes.AnyAsync(s => s.SizeId == dto.Sizes.SizeId))
                return NotFound("Size not found.");
            if (!await _contextShop.Colors.AnyAsync(c => c.ColorId == dto.Colors.ColorId))
                return NotFound("Color not found.");
            if (!await _contextShop.Materials.AnyAsync(m => m.MaterialId == dto.Materials.MaterialId))
                return NotFound("Material not found.");

            existingProduct.ProductName = dto.Products.ProductName;
            existingProduct.Price = dto.Products.Price;
            existingProduct.CategoryId = dto.Products.CategoryId;
            existingProduct.Status = dto.Products.Status;
            existingProduct.UpdateAt = DateTime.Now;

            existingProductDetail.SizeId = dto.Sizes.SizeId;
            existingProductDetail.ColorId = dto.Colors.ColorId;
            existingProductDetail.MaterialId = dto.Materials.MaterialId;
            existingProductDetail.StockQuantity = dto.ProductDetails.StockQuantity;
            // TẠM THỜI KHÔNG CẬP NHẬT ẢNH TỪ FRONTEND JSON
            // existingProductDetail.Image = dto.ProductDetails.Image;

            try
            {
                await _contextShop.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_contextShop.Products.Any(e => e.ProductId == id))
                {
                    return NotFound("Product not found during update.");
                }
                throw;
            }

            return Ok("Product and product detail updated successfully.");
        }

        // POST: api/Product
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct([FromBody] ProductUpdateDto dto)
        {
            try
            {
                Console.WriteLine("PostProduct called");

                if (dto == null)
                    return BadRequest("Body is null.");

                if (dto.Products == null || dto.ProductDetails == null)
                    return BadRequest("Product or ProductDetail data is missing.");

                if (!await _contextShop.ProductCategories.AnyAsync(c => c.CategoryId == dto.Products.CategoryId))
                    return NotFound("Category not found.");

                if (!await _contextShop.Sizes.AnyAsync(s => s.SizeId == dto.Sizes.SizeId))
                    return NotFound("Size not found.");

                if (!await _contextShop.Colors.AnyAsync(c => c.ColorId == dto.Colors.ColorId))
                    return NotFound("Color not found.");

                if (!await _contextShop.Materials.AnyAsync(m => m.MaterialId == dto.Materials.MaterialId))
                    return NotFound("Material not found.");

                var product = new Product
                {
                    ProductName = dto.Products.ProductName,
                    Price = dto.Products.Price,
                    CategoryId = dto.Products.CategoryId,
                    Status = dto.Products.Status,
                    CreatedAt = DateTime.Now,
                    UpdateAt = DateTime.Now
                };

                _contextShop.Products.Add(product);
                await _contextShop.SaveChangesAsync();

                var productDetail = new ProductDetail
                {
                    ProductId = product.ProductId,
                    SizeId = dto.Sizes.SizeId,
                    ColorId = dto.Colors.ColorId,
                    MaterialId = dto.Materials.MaterialId,
                    StockQuantity = dto.ProductDetails.StockQuantity,
                    // TẠM THỜI KHÔNG LƯU ẢNH TỪ JSON
                    Image = null
                };

                _contextShop.ProductDetails.Add(productDetail);
                await _contextShop.SaveChangesAsync();

                Console.WriteLine("PostProduct success, id = " + product.ProductId);
                return CreatedAtAction(nameof(GetProduct),
                    new { id = product.ProductId }, product);
            }
            catch (Exception ex)
            {
                Console.WriteLine("PostProduct error: " + ex);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Internal server error in PostProduct");
            }
        }

        // DELETE: api/Product/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _contextShop.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound("Không tìm thấy sản phẩm.");
            }

            _contextShop.Products.Remove(product);
            await _contextShop.SaveChangesAsync();
            return Ok("Xoá Thành Công");
        }

        private bool ProductExist(int id)
        {
            return _contextShop.Products.Any(e => e.ProductId == id);
        }
    }
}
