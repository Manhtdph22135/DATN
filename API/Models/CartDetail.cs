using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace API.Models;

[Index("CartId", Name = "IX_CartDetails_CartID")]
[Index("ProductDetailId", Name = "IX_CartDetails_ProductDetailID")]
public partial class CartDetail
{
    [Key]
    [Column("CartDetailID")]
    public int CartDetailId { get; set; }

    [Column("CartID")]
    public int CartId { get; set; }

    [Column("ProductDetailID")]
    public int ProductDetailId { get; set; }

    public int Quantity { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal UnitPrice { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Total { get; set; }

    [ForeignKey("CartId")]
    [InverseProperty("CartDetails")]
    public virtual Cart Cart { get; set; } = null!;

    [ForeignKey("ProductDetailId")]
    [InverseProperty("CartDetails")]
    public virtual ProductDetail ProductDetail { get; set; } = null!;
}
