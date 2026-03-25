using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HSU.PTWeb.AnhPH.BookStore.Models.Scaffolded;

[Index("AppUserId", Name = "IX_Orders_AppUserId")]
[Index("Status", "OrderDate", Name = "IX_Orders_Status_OrderDate", IsDescending = new[] { false, true })]
[Index("UserId", Name = "IX_Orders_UserId")]
public partial class Order
{
    [Key]
    public int OrderId { get; set; }

    public int UserId { get; set; }

    public DateTime OrderDate { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal TotalAmount { get; set; }

    [StringLength(50)]
    public string Status { get; set; } = null!;

    [StringLength(100)]
    public string RecipientName { get; set; } = null!;

    [StringLength(15)]
    public string PhoneNumber { get; set; } = null!;

    [StringLength(100)]
    public string Email { get; set; } = null!;

    [StringLength(500)]
    public string ShippingAddress { get; set; } = null!;

    [StringLength(500)]
    public string? Notes { get; set; }

    [StringLength(50)]
    public string PaymentMethod { get; set; } = null!;

    [StringLength(50)]
    public string PaymentStatus { get; set; } = null!;

    public DateTime? PaidAt { get; set; }

    public int? CityId { get; set; }

    public int? WardId { get; set; }

    [StringLength(50)]
    public string Channel { get; set; } = null!;

    public int? AppUserId { get; set; }

    [ForeignKey("AppUserId")]
    [InverseProperty("OrderAppUsers")]
    public virtual User? AppUser { get; set; }

    [ForeignKey("CityId")]
    [InverseProperty("Orders")]
    public virtual City? City { get; set; }

    [InverseProperty("Order")]
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    [ForeignKey("UserId")]
    [InverseProperty("OrderUsers")]
    public virtual User User { get; set; } = null!;

    [ForeignKey("WardId")]
    [InverseProperty("Orders")]
    public virtual Ward? Ward { get; set; }
}
