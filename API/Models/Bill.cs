using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace API.Models;

[Index("CustomerId", Name = "IX_Bills_CustomerID")]
[Index("StaffId", Name = "IX_Bills_StaffID")]
[Index("TransportId", Name = "IX_Bills_TransportID")]
public partial class Bill
{
    [Key]
    [Column("BillID")]
    public int BillId { get; set; }

    [StringLength(50)]
    public string OrderCode { get; set; } = null!;

    [Column("StaffID")]
    public int? StaffId { get; set; }

    [Column("CustomerID")]
    public int? CustomerId { get; set; }

    [Column("TransportID")]
    public int? TransportId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreateAt { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal TotalAmount { get; set; }

    [StringLength(50)]
    public string PaymentMethod { get; set; } = null!;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Subtotal { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal VatRate { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal VatAmount { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal DiscountAmount { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Total { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal AmountReceived { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal ChangeAmount { get; set; }

    [StringLength(500)]
    public string? Note { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime PaidAt { get; set; }

    [InverseProperty("Bill")]
    public virtual ICollection<BillDetail> BillDetails { get; set; } = new List<BillDetail>();

    [ForeignKey("CustomerId")]
    [InverseProperty("Bills")]
    public virtual Customer? Customer { get; set; }

    [ForeignKey("StaffId")]
    [InverseProperty("Bills")]
    public virtual Staff? Staff { get; set; }

    [ForeignKey("TransportId")]
    [InverseProperty("Bills")]
    public virtual Transport? Transport { get; set; }
}
