using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HSU.PTWeb.AnhPH.BookStore.Models.Scaffolded;

public partial class City
{
    [Key]
    public int CityId { get; set; }

    [StringLength(100)]
    public string CityName { get; set; } = null!;

    public bool IsActive { get; set; }

    [InverseProperty("City")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    [InverseProperty("City")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();

    [InverseProperty("City")]
    public virtual ICollection<Ward> Wards { get; set; } = new List<Ward>();
}
