using API.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Statistical
{
    public class StatisticalController : Controller
    {
        private readonly DbContextShop _contextShop;
        public StatisticalController(DbContextShop contextShop)
        {
            _contextShop = contextShop;
        }
        [HttpGet("get-tong-doanh-thu")]
        public IActionResult GetTongDoanhThu()
        {
            var tongDoanhThu = _contextShop.Bills.Sum(o => o.TotalAmount);
            return Ok(new { TongDoanhThu = tongDoanhThu });
        }
        [HttpGet("get-tong-don-hang")]
        public IActionResult GetTongDonHang()
        {
            var tongDonHang = _contextShop.Bills.Count();
            return Ok(new { TongDonHang = tongDonHang });
        }
        [HttpGet("get-tong-khach-hang")]
        public IActionResult GetTongKhachHang()
        {
            var tongKhachHang = _contextShop.Customers.Count();
            return Ok(new { TongKhachHang = tongKhachHang });
        }
        [HttpGet("get-tong-doanh-thu-theo-nam")]
        public IActionResult GetTongDoanhThuTheoNam(int nam)
        {
            var tongDoanhThuNam = _contextShop.Bills
                .Where(o => o.CreateAt.Year == nam)
                .Sum(o => o.TotalAmount);
            return Ok(new { Nam = nam, TongDoanhThu = tongDoanhThuNam });
        }
        [HttpGet("get-doanh-thu-theo-thang")]
        public IActionResult GetDoanhThuTheoThang(int nam)
        {
            var doanhThuTheoThang = _contextShop.Bills
                .Where(o => o.CreateAt.Year == nam)
                .GroupBy(o => o.CreateAt.Month)
                .Select(g => new
                {
                    Thang = g.Key,
                    TongDoanhThu = g.Sum(o => o.TotalAmount)
                })
                .OrderBy(x => x.Thang)
                .ToList();
            return Ok(doanhThuTheoThang);
        }
        [HttpGet("get-san-pham-ban-chay")]
        public IActionResult GetSanPhamBanChay(int top)
        {
            var sanPhamBanChay = _contextShop.BillDetails
                .GroupBy(bd => bd.ProductDetail.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    ProductName = g.First().ProductDetail.Product != null ? g.First().ProductDetail.Product.ProductName : "",
                    SoLuongBan = g.Sum(bd => bd.Quantity)
                })
                .OrderByDescending(x => x.SoLuongBan)
                .Take(top)
                .ToList();
            return Ok(sanPhamBanChay);
        }
        [HttpGet("get-don-hang-gan-day")]
        public IActionResult GetDonHangGanDay(int days)
        {
            var dateThreshold = DateTime.Now.AddDays(-days);
            var bills = (from b in _contextShop.Bills
                         where b.CreateAt >= dateThreshold
                         join c in _contextShop.Customers on b.CustomerId equals c.CustomerId
                         join s in _contextShop.Staffs on b.StaffId equals s.StaffId into staffJoin
                         from s in staffJoin.DefaultIfEmpty()
                         orderby b.CreateAt descending
                         select new
                         {
                             id = b.BillId,
                             billId = b.BillId,
                             orderCode = b.OrderCode,
                             customerName = c.FullName,
                             customerPhone = c.Phone,
                             customerEmail = c.Email,
                             customerAddress = c.Address ?? "",
                             subtotal = _contextShop.BillDetails.Where(d => d.BillId == b.BillId).Sum(d => (decimal?)(d.Total) ?? 0m),
                             discount = 0, // Add logic if you have discount
                             tax = 0, // Add logic if you have tax
                             total = b.TotalAmount,
                             paymentMethod = b.PaymentMethod,
                             status = b.PaidAt <= DateTime.Now ? "paid" : "unpaid",
                             createdAt = b.CreateAt,
                             paymentDate = b.PaidAt, // Replace with actual payment date if available
                             note = "", // Add note if you have
                             items = (from bd in _contextShop.BillDetails
                                      join pd in _contextShop.ProductDetails on bd.ProductDetailId equals pd.ProductDetailId
                                      join p in _contextShop.Products on pd.ProductId equals p.ProductId
                                      join sz in _contextShop.Sizes on pd.SizeId equals sz.SizeId
                                      join clr in _contextShop.Colors on pd.ColorId equals clr.ColorId
                                      where bd.BillId == b.BillId
                                      select new
                                      {
                                          id = bd.BillDetailId,
                                          name = p.ProductName,
                                          price = bd.UnitPrice,
                                          quantity = bd.Quantity,
                                          image = pd.Image ?? "",
                                          color = clr.ColorName,
                                          size = sz.SizeName
                                      }).ToList()
                         }).ToList();

            return Ok(bills);
        }
    }
}
