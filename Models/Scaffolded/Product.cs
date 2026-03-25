using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HSU.PTWeb.AnhPH.BookStore.Models.Scaffolded;

[Index("CategoryId", Name = "IX_Products_CategoryId")]
public partial class Product
{
    [Key]
    public int ProductId { get; set; }

    [StringLength(200)]
    public string ProductName { get; set; } = null!;

    [Column("ISBN")]
    [StringLength(13)]
    public string? Isbn { get; set; }

    [StringLength(100)]
    public string? Author { get; set; }

    [StringLength(100)]
    public string? Publisher { get; set; }

    public int? PublicationYear { get; set; }

    [StringLength(50)]
    public string? Language { get; set; }

    public int? PageCount { get; set; }

    [StringLength(50)]
    public string? Dimensions { get; set; }

    public int? Weight { get; set; }

    [StringLength(50)]
    public string? CoverType { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Price { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? OriginalPrice { get; set; }

    public int? DiscountPercent { get; set; }

    public int Stock { get; set; }

    public int SoldCount { get; set; }

    [StringLength(500)]
    public string? ShortDescription { get; set; }

    [Column(TypeName = "ntext")]
    public string? Description { get; set; }

    [Column(TypeName = "ntext")]
    public string? TableOfContents { get; set; }

    [StringLength(255)]
    public string? ImageUrl { get; set; }

    [StringLength(1000)]
    public string? AdditionalImages { get; set; }

    [Column(TypeName = "decimal(3, 2)")]
    public decimal AverageRating { get; set; }

    public int ReviewCount { get; set; }

    public bool IsFeatured { get; set; }

    public bool IsNewArrival { get; set; }

    public bool IsBestSeller { get; set; }

    public bool IsAvailable { get; set; }

    public bool IsDiscontinued { get; set; }

    public int CategoryId { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    [StringLength(100)]
    public string? CreatedBy { get; set; }

    [StringLength(100)]
    public string? UpdatedBy { get; set; }

    [ForeignKey("CategoryId")]
    [InverseProperty("Products")]
    public virtual Category Category { get; set; } = null!;

    [InverseProperty("Product")]
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
