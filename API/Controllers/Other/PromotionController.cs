using API.Context;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromotionController : ControllerBase
    {
        private readonly DbContextShop _contextShop;

        public PromotionController(DbContextShop contextShop)
        {
            _contextShop = contextShop;
        }

        // 1. Lấy danh sách Ưu đãi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Promotion>>> GetPromotions()
        {
            return await _contextShop.Promotions.ToListAsync();
        }

        // 2. Thêm Ưu đãi mới
        [HttpPost]
        public async Task<ActionResult<Promotion>> PostPromotion(Promotion promotion)
        {
            // SỬA LỖI DATE: Dùng DateOnly.FromDateTime
            if (promotion.CreatedAt == null) promotion.CreatedAt = DateTime.Now;

            _contextShop.Promotions.Add(promotion);
            await _contextShop.SaveChangesAsync();

            // SỬA LỖI TÊN BIẾN: PromotionId (d thường)
            return CreatedAtAction("GetPromotions", new { id = promotion.PromotionId }, promotion);
        }

        // 3. API KIỂM TRA VOUCHER
        [HttpGet("check-voucher")]
        public async Task<IActionResult> CheckVoucher([FromQuery] string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return BadRequest("Vui lòng nhập mã giảm giá.");
            }

            // *** SỬA LỖI QUAN TRỌNG: Chuyển DateTime sang DateOnly ***
            // Vì database của bạn dùng DateOnly, ta phải so sánh với DateOnly
            var today = DateOnly.FromDateTime(DateTime.Now);

            var promotion = await _contextShop.Promotions
                .FirstOrDefaultAsync(p =>
                    p.Code == code &&
                    p.ProductId == null &&                    // SỬA: ProductId (d thường)
                    p.Status == "Đang hoạt động" &&
                    p.StartDate <= today &&                   // So sánh DateOnly với DateOnly
                    p.EndDate >= today                        // So sánh DateOnly với DateOnly
                );

            if (promotion == null)
            {
                return NotFound("Mã giảm giá không tồn tại hoặc đã hết hạn.");
            }

            return Ok(promotion);
        }
        // Thêm vào PromotionController.cs

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPromotion(int id, Promotion promotion)
        {
            // 1. BỎ kiểm tra, ÉP ID của object bằng ID trên URL luôn
            // (Chú ý: Viết đúng PromotionId hay PromotionID theo model của bạn)
            promotion.PromotionId = id;

            // 2. Xử lý ngày tháng (nếu cần thiết)
            if (promotion.CreatedAt == null) promotion.CreatedAt = DateTime.Now;

            _contextShop.Entry(promotion).State = EntityState.Modified;

            try
            {
                await _contextShop.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PromotionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        private bool PromotionExists(int id)
        {
            return _contextShop.Promotions.Any(e => e.PromotionId == id);
        } 

        // 4. Xóa Ưu đãi
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePromotion(int id)
        {
            var promo = await _contextShop.Promotions.FindAsync(id);
            if (promo == null) return NotFound();

            _contextShop.Promotions.Remove(promo);
            await _contextShop.SaveChangesAsync();
            return Ok("Đã xóa thành công");
        }
    }
}