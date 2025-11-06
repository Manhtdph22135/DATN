using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace API.Models;

public partial class Bill
{
    [Key]
    [Column("BillID")]
    public int BillId { get; set; }

    [Column("OrderCode")]
    [StringLength(50)]
    public string OrderCode { get; set; } = string.Empty;

    // Nếu chỉ dùng CashierId, có thể dần bỏ StaffId
    [Column("StaffID")]
    public int? StaffId { get; set; }

    //[Column("CashierId")]
    //public int CashierId { get; set; } // required

    [Column("CustomerID")]
    public int? CustomerId { get; set; }

    [Column("TransportID")]
    public int? TransportId { get; set; }

    // Thời điểm tạo hoá đơn
    [Column(TypeName = "datetime")]
    public DateTime CreateAt { get; set; } = DateTime.Now;

    // KHÔNG cần cả TotalAmount và Total; dùng 1 trường Total là đủ
    // Giữ lại TotalAmount nếu đã migrate, nhưng KHÔNG dùng nữa (tuỳ bạn)
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }  // <— DEPRECATED: không dùng, để = Total

    [Required]
    [StringLength(50)]
    public string PaymentMethod { get; set; } = "cash"; // cash/card/transfer/momo/vnpay

    [Column(TypeName = "decimal(18,2)")]
    public decimal Subtotal { get; set; }     // luôn có giá trị

    [Column(TypeName = "decimal(5,2)")]
    public decimal VatRate { get; set; }      // ví dụ 0.08m

    [Column(TypeName = "decimal(18,2)")]
    public decimal VatAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal DiscountAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Total { get; set; }        // Subtotal + VatAmount - DiscountAmount

    [Column(TypeName = "decimal(18,2)")]
    public decimal AmountReceived { get; set; } // tiền đã nhận (cash = khách đưa; card/transfer = Total)

    [Column(TypeName = "decimal(18,2)")]
    public decimal ChangeAmount { get; set; }   // = max(0, AmountReceived - Total)

    [StringLength(500)]
    public string? Note { get; set; }

    // Thời điểm thanh toán (đã chốt)
    [Column(TypeName = "datetime")]
    public DateTime PaidAt { get; set; } = DateTime.Now;

    // Navigation — chỉ MỘT thuộc tính cho mỗi quan hệ
    public virtual Customer? Customer { get; set; }
    public virtual Staff? Staff { get; set; }
    public virtual Transport? Transport { get; set; }


    [InverseProperty("Bill")]
    public virtual ICollection<BillDetail> BillDetails { get; set; } = new List<BillDetail>();
}
