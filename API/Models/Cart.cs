using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace API.Models;

[Index("CustomerId", Name = "IX_Carts_CustomerId")]
public partial class Cart
{
    [Key]
    [Column("CartID")]
    public int CartId { get; set; }

    public int? CustomerId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int Status { get; set; }

    [InverseProperty("Cart")]
    public virtual ICollection<CartDetail> CartDetails { get; set; } = new List<CartDetail>();

    [ForeignKey("CustomerId")]
    [InverseProperty("Carts")]
    public virtual Customer? Customer { get; set; }
}
