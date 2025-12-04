using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace API.Models;

[Index("AccountId", Name = "IX_Customers_RoleID")]
[Index("Phone", Name = "UQ__Customer__5C7E359E9B9944BC", IsUnique = true)]
[Index("Email", Name = "UQ__Customer__A9D10534BCF9D50C", IsUnique = true)]
public partial class Customer
{
    [Key]
    [Column("CustomerID")]
    public int CustomerId { get; set; }

    [StringLength(100)]
    public string FullName { get; set; } = null!;

    [StringLength(50)]
    public string Email { get; set; } = null!;

    [StringLength(15)]
    [Unicode(false)]
    public string Phone { get; set; } = null!;

    public bool Gender { get; set; }

    [Column("DOB")]
    public DateOnly? Dob { get; set; }

    [StringLength(255)]
    public string? Address { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreateAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdateAt { get; set; }

    [StringLength(50)]
    public string? RankMember { get; set; }

    public int? Point { get; set; }

    [Column("AccountID")]
    public int AccountId { get; set; }

    [ForeignKey("AccountId")]
    //[InverseProperty("Customers")]
    public virtual Account Account { get; set; } = null!;

    [InverseProperty("Customer")]
    public virtual ICollection<Bill> Bills { get; set; } = new List<Bill>();

    [InverseProperty("Customer")]
    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();
}
