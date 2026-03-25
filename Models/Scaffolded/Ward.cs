using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HSU.PTWeb.AnhPH.BookStore.Models.Scaffolded;

public partial class Ward
{
    [Key]
    public int WardId { get; set; }

    public int CityId { get; set; }

    [StringLength(100)]
    public string WardName { get; set; } = null!;

    public bool IsActive { get; set; }

    [ForeignKey("CityId")]
    [InverseProperty("Wards")]
    public virtual City City { get; set; } = null!;

    [InverseProperty("Ward")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    [InverseProperty("Ward")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
