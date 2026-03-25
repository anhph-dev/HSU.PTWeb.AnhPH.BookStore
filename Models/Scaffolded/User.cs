using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HSU.PTWeb.AnhPH.BookStore.Models.Scaffolded;

[Index("CityId", Name = "IX_Users_CityId")]
[Index("WardId", Name = "IX_Users_WardId")]
public partial class User
{
    [Key]
    public int UserId { get; set; }

    [StringLength(100)]
    public string FullName { get; set; } = null!;

    [StringLength(100)]
    public string Email { get; set; } = null!;

    [StringLength(255)]
    public string PasswordHash { get; set; } = null!;

    [StringLength(20)]
    public string Role { get; set; } = null!;

    [StringLength(15)]
    public string PhoneNumber { get; set; } = null!;

    [StringLength(500)]
    public string Address { get; set; } = null!;

    public bool IsLocked { get; set; }

    public DateTime? LockedUntil { get; set; }

    public DateTime CreatedDate { get; set; }

    public int? CityId { get; set; }

    public int? WardId { get; set; }

    [ForeignKey("CityId")]
    [InverseProperty("Users")]
    public virtual City? City { get; set; }

    [InverseProperty("AppUser")]
    public virtual ICollection<Order> OrderAppUsers { get; set; } = new List<Order>();

    [InverseProperty("User")]
    public virtual ICollection<Order> OrderUsers { get; set; } = new List<Order>();

    [ForeignKey("WardId")]
    [InverseProperty("Users")]
    public virtual Ward? Ward { get; set; }
}
