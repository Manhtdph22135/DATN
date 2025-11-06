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
            var query = from p in _contextShop.Products
                join pd in _contextShop.ProductDetails on p.ProductId equals pd.ProductId
                join c in _contextShop.ProductCategories on p.CategoryId equals c.CategoryId
                join m in _contextShop.Materials on pd.MaterialId equals m.MaterialId
                join s in _contextShop.Sizes on pd.SizeId equals s.SizeId
                join co in _contextShop.Colors on pd.ColorId equals co.ColorId
                //where p.Status == false ? 
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
            return Json(query);
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
