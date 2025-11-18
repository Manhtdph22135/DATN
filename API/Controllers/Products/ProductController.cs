using API.Context;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using API.DOT;

namespace API.Controllers.Products
{
    [Route("api/[controller]")]
    public class ProductController : Controller
    {
        private readonly DbContextShop _contextShop;
        public ProductController(DbContextShop contextShop)
        {
            _contextShop = contextShop;
        }
        
       
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var today = DateOnly.FromDateTime(DateTime.Now); // Lấy ngày giờ hiện tại

            var query = from p in _contextShop.Products
                        join pd in _contextShop.ProductDetails on p.ProductId equals pd.ProductId
                        join c in _contextShop.ProductCategories on p.CategoryId equals c.CategoryId
                        join m in _contextShop.Materials on pd.MaterialId equals m.MaterialId
                        join s in _contextShop.Sizes on pd.SizeId equals s.SizeId
                        join co in _contextShop.Colors on pd.ColorId equals co.ColorId

                        // --- THÊM PHẦN JOIN BẢNG PROMOTION (Left Join) ---
                        join promo in _contextShop.Promotions on p.ProductId equals promo.ProductId into promoGroup
                        from pm in promoGroup.DefaultIfEmpty()
                            // -------------------------------------------------

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
                            c.CategoryId, // ID danh mục
                            m.MaterialName,
                            s.SizeName,
                            s.SizeId,     // ID size
                            co.ColorName,
                            co.ColorId,   // ID màu

                            // --- TÍNH TOÁN GIÁ & KHUYẾN MÃI ---

                            // 1. Kiểm tra xem có khuyến mãi hợp lệ không?
                            // (Phải "Đang hoạt động" và còn trong thời hạn)
                            IsPromoActive = pm != null
                                            && pm.Status == "Đang hoạt động"
                                            && pm.StartDate <= today
                                            && pm.EndDate >= today,

                            // 2. Giá gốc (Là giá niêm yết trong bảng Product)
                            OriginalPrice = p.Price,

                            // 3. Giá bán (Nếu có KM thì tính giá giảm, nếu không thì giữ nguyên)
                            // Giả sử DiscountValue là % (ví dụ 50 nghĩa là 50%)
                            Price = (pm != null && pm.Status == "Đang hoạt động" && pm.StartDate <= today && pm.EndDate >= today)
                                    ? p.Price * (100 - pm.DiscountValue) / 100
                                    : p.Price,

                            // 4. % Giảm giá (Để hiển thị thẻ đỏ -33%)
                            DiscountPercent = (pm != null && pm.Status == "Đang hoạt động" && pm.StartDate <= today && pm.EndDate >= today)
                                              ? pm.DiscountValue
                                              : 0
                            // ----------------------------------
                        };

            var result = await query.ToListAsync();
            return Json(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var query = from p in _contextShop.Products
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
            return Json(query);
        }
        // Fixes: 
        // 1. Adds await and ToListAsync() to make the method truly async and resolve CS1998 warning.
        // 2. Changes route to avoid conflict by using "by-category/{categoryId}" instead of "{categoryId}".
        [HttpGet("by-category/{categoryId:int}")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsByCategory(int categoryId)
        {
            var query = from p in _contextShop.Products
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
            return Json(result);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProducts(int id, [FromBody] ProductUpdateDto dto)
        {
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
                .FirstOrDefaultAsync(pd => pd.ProductDetailId == dto.ProductDetails.ProductDetailId && pd.ProductId == id);

            if (existingProductDetail == null)
            {
                return NotFound("Product detail not found.");
            }

            existingProduct.ProductName = dto.Products.ProductName;
            existingProduct.Price = dto.Products.Price;
            existingProduct.CategoryId = dto.Products.CategoryId;
            existingProduct.Status = dto.Products.Status;
            existingProduct.UpdateAt = DateTime.Now;

            if (!await _contextShop.Sizes.AnyAsync(s => s.SizeId == dto.Sizes.SizeId))
                return NotFound("Size not found.");
            if (!await _contextShop.Colors.AnyAsync(c => c.ColorId == dto.Colors.ColorId))
                return NotFound("Color not found.");
            if (!await _contextShop.Materials.AnyAsync(m => m.MaterialId == dto.Materials.MaterialId))
                return NotFound("Material not found.");

            existingProductDetail.SizeId = dto.Sizes.SizeId;
            existingProductDetail.ColorId = dto.Colors.ColorId;
            existingProductDetail.MaterialId = dto.Materials.MaterialId;
            existingProductDetail.StockQuantity = dto.ProductDetails.StockQuantity;
            existingProductDetail.Image = dto.ProductDetails.Image;

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
       

        [HttpGet("Materials")]
        public async Task<IActionResult> GetMaterials()
        {
            var data = await _contextShop.Materials
                .Select(m => new {
                    id = m.MaterialId,     // Trả về 'id'
                    name = m.MaterialName   // Trả về 'name'
                })
                .ToListAsync();

            return Ok(data);
        }
        [HttpGet("Sizes")] 
        
        public async Task<IActionResult> GetSizes()
        {
            var data = await _contextShop.Sizes
                .Select(s => new {
                    id = s.SizeId,       // Tên là 'id'
                    name = s.SizeName    // Tên là 'name'
                })
                .ToListAsync();
            return Ok(data);
        }
        [HttpGet("Colors")] // Hoặc [HttpGet("Colors")] nếu thêm vào ProductController
       
        public async Task<IActionResult> GetColors()
        {
            var data = await _contextShop.Colors
                .Select(c => new {
                    id = c.ColorId,     // Tên là 'id'
                    name = c.ColorName // Tên là 'name'
                      // Giả sử cột mã hex của bạn là ColorCode
                })
                .ToListAsync();
            return Ok(data);
        }
        // POST: api/Product
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct([FromBody] ProductUpdateDto dto)
        {
            if (dto.Products == null || dto.ProductDetails == null)
            {
                return BadRequest("Product or ProductDetail data is missing.");
            }

            // Validate foreign keys
            if (!await _contextShop.ProductCategories.AnyAsync(c => c.CategoryId == dto.Products.CategoryId))
                return NotFound("Category not found.");
            if (!await _contextShop.Sizes.AnyAsync(s => s.SizeId == dto.Sizes.SizeId))
                return NotFound("Size not found.");
            if (!await _contextShop.Colors.AnyAsync(c => c.ColorId == dto.Colors.ColorId))
                return NotFound("Color not found.");
            if (!await _contextShop.Materials.AnyAsync(m => m.MaterialId == dto.Materials.MaterialId))
                return NotFound("Material not found.");

            // Create Product
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

            // Create ProductDetail
            var productDetail = new ProductDetail
            {
                ProductId = product.ProductId,
                SizeId = dto.Sizes.SizeId,
                ColorId = dto.Colors.ColorId,
                MaterialId = dto.Materials.MaterialId,
                StockQuantity = dto.ProductDetails.StockQuantity,
                Image = dto.ProductDetails.Image
            };
            _contextShop.ProductDetails.Add(productDetail);
            await _contextShop.SaveChangesAsync();

            return CreatedAtAction("GetProduct", new { id = product.ProductId }, product);
        }
        // DELETE: api/Customer/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var customer = await _contextShop.Products.FindAsync(id);
            if (customer == null)
            {
                return NotFound("Không tìm thấy id khách hàng");
            }

            _contextShop.Products.Remove(customer);
            await _contextShop.SaveChangesAsync();
            return Ok("Xoá Thành Công");
        }
        private bool ProductExit(int id)
        {
            return _contextShop.Products.Any(e => e.ProductId == id);
        }
    }
}
