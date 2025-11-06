using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace API.Models;

public partial class BillDetail
{
    [Key]
    [Column("BillDetailID")]
    public int BillDetailId { get; set; }

    [Column("BillID")]
    public int BillId { get; set; }                      // required

    [Column("ProductDetailID")]
    public int ProductDetailId { get; set; }             // required

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Total { get; set; }                   // UnitPrice * Quantity

    [ForeignKey(nameof(BillId))]
    [InverseProperty(nameof(Bill.BillDetails))]
    public virtual Bill Bill { get; set; } = default!;

    [ForeignKey(nameof(ProductDetailId))]
    [InverseProperty(nameof(ProductDetail.BillDetails))]
    public virtual ProductDetail ProductDetail { get; set; } = default!;
}
