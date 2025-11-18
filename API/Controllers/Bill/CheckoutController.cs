using API.Context;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers.Bill
{
    [ApiController]
    [Route("api/[controller]")]
    public class CheckoutController : Controller
    {
        private readonly DbContextShop _ctx;
        public CheckoutController(DbContextShop ctx) => _ctx = ctx;

        [HttpPost]
        public async Task<IActionResult> Checkout([FromBody] CheckoutRequest req)
        {
            if (req.Items == null || req.Items.Count == 0)
                return BadRequest("Giỏ hàng trống.");

            const decimal VAT_RATE = 0.08m;

            await using var tx = await _ctx.Database.BeginTransactionAsync();

            try
            {
                var pdIds = req.Items.Select(i => i.ProductDetailId).ToList();
                var pds = await _ctx.ProductDetails
                                   .Include(x => x.Product)
                                   .Where(x => pdIds.Contains(x.ProductDetailId))
                                   .ToListAsync();

                // Move the creation of the 'bill' object before the 'details' list is populated, so that its BillId can be assigned to BillDetail objects after SaveChanges.
                var bill = new Models.Bill
                {
                    OrderCode = req.OrderCode,
                    CustomerId = req.CustomerId,
                    PaymentMethod = req.PaymentMethod,
                    Subtotal = 0m, // will be updated after calculating subtotal
                    VatRate = VAT_RATE,
                    VatAmount = 0m, // will be updated after calculating vatAmount
                    DiscountAmount = 0m, // will be updated after calculating discountAmount
                    Total = 0m, // will be updated after calculating total
                    AmountReceived = req.AmountReceived,
                    ChangeAmount = 0m, // will be updated after calculating change
                    Note = req.Note,
                    CreateAt = DateTime.Now,
                    PaidAt = DateTime.Now
                };

                var details = new List<BillDetail>();
                decimal subtotal = 0m;

                foreach (var i in req.Items)
                {
                    var pd = pds.FirstOrDefault(x => x.ProductDetailId == i.ProductDetailId);
                    if (pd == null)
                        return NotFound($"Không tìm thấy biến thể (ProductDetailId={i.ProductDetailId}).");

                    if (i.Quantity <= 0)
                        return BadRequest("Số lượng phải > 0.");

                    if (pd.StockQuantity < i.Quantity)
                        return BadRequest($"Sản phẩm {pd.ProductId} không đủ tồn. Còn {pd.StockQuantity}, cần {i.Quantity}");

                    var unitPrice = (decimal)(pd.Product?.Price ?? 0);
                    var lineTotal = unitPrice * i.Quantity;

                    subtotal += lineTotal;

                    details.Add(new BillDetail
                    {
                        // BillId will be set after bill is saved and BillId is generated
                        ProductDetailId = pd.ProductDetailId,
                        Quantity = i.Quantity,
                        UnitPrice = unitPrice,
                        Total = unitPrice * i.Quantity
                    });

                    // trừ kho
                    pd.StockQuantity -= i.Quantity;
                    _ctx.ProductDetails.Update(pd);
                }

                var vatAmount = subtotal * VAT_RATE;
                var percentDiscount = req.DiscountPercent > 0 ? subtotal * (req.DiscountPercent / 100m) : 0m;
                var discountAmount = Math.Max(req.DiscountAmount, percentDiscount);
                var total = subtotal + vatAmount - discountAmount;

                bill.Subtotal = subtotal;
                bill.VatAmount = vatAmount;
                bill.DiscountAmount = discountAmount;
                bill.Total = total;
                bill.ChangeAmount = Math.Max(0, req.AmountReceived - total);
                bill.TotalAmount = bill.Total;

                _ctx.Add(bill);
                await _ctx.SaveChangesAsync();     // lấy BillId

                foreach (var d in details)
                {
                    d.BillId = bill.BillId;
                    _ctx.Add(d);
                }

                await _ctx.SaveChangesAsync();
                await tx.CommitAsync();

                return Ok(new
                {
                    billId = bill.BillId,
                    orderCode = bill.OrderCode,
                    total = bill.Total,
                    change = bill.ChangeAmount,
                    paidAt = bill.PaidAt
                });
            }
            catch (DbUpdateConcurrencyException)
            {
                await tx.RollbackAsync();
                return Conflict("Xung đột tồn kho, vui lòng thử lại.");
            }
            catch (DbUpdateException ex)
            {
                await tx.RollbackAsync();
                return StatusCode(500, "DbUpdateException: " + (ex.InnerException?.Message ?? ex.Message));
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return StatusCode(500, "Lỗi xử lý: " + ex.Message);
            }
        }
    }
    public class CheckoutItemDto
    {
        public int ProductDetailId { get; set; }
        public int ProductId { get; set; }      // optional
        public int Quantity { get; set; }
    }

    public class CheckoutRequest
    {
        public string OrderCode { get; set; } = default!;
        public int? CustomerId { get; set; }
        public string PaymentMethod { get; set; } = "cash";
        public decimal AmountReceived { get; set; }
        public decimal DiscountAmount { get; set; }     // ưu tiên số tiền cố định
        public decimal DiscountPercent { get; set; }    // hoặc %
        public string? Note { get; set; }
        public List<CheckoutItemDto> Items { get; set; } = new();
    }

}
