using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HSU.PTWeb.AnhPH.BookStore.Models
{
    /// <summary>
    /// Model Product đại diện cho sách trong cửa hàng
    /// Chứa thông tin đầy đủ về sách theo chuẩn quốc tế
    /// </summary>
    public class Product
    {
        // ===== BASIC INFO =====
        
        [Key]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên sách")]
        [StringLength(200, ErrorMessage = "Tên sách không được vượt quá 200 ký tự")]
        [Display(Name = "Tên sách")]
        public string ProductName { get; set; }

        // ===== BOOK-SPECIFIC INFO =====
        
        [StringLength(13)]
        [Display(Name = "ISBN")]
        public string? ISBN { get; set; }

        [StringLength(100)]
        [Display(Name = "Tác giả")]
        public string? Author { get; set; }

        [StringLength(100)]
        [Display(Name = "Nhà xuất bản")]
        public string? Publisher { get; set; }

        [Display(Name = "Năm xuất bản")]
        [Range(1900, 2100, ErrorMessage = "Năm xuất bản không hợp lệ")]
        public int? PublicationYear { get; set; }

        [StringLength(50)]
        [Display(Name = "Ngôn ngữ")]
        public string? Language { get; set; } = "Tiếng Việt";

        [Display(Name = "Số trang")]
        [Range(1, 10000, ErrorMessage = "Số trang phải từ 1 đến 10000")]
        public int? PageCount { get; set; }

        [StringLength(50)]
        [Display(Name = "Kích thước (cm)")]
        public string? Dimensions { get; set; } // Ví dụ: "20.5 x 14 x 2"

        [Display(Name = "Trọng lượng (gram)")]
        [Range(0, 5000, ErrorMessage = "Trọng lượng không hợp lệ")]
        public int? Weight { get; set; }

        [StringLength(50)]
        [Display(Name = "Hình thức bìa")]
        public string? CoverType { get; set; } // Bìa cứng, Bìa mềm, etc.

        // ===== PRICING & INVENTORY =====
        
        [Required(ErrorMessage = "Vui lòng nhập giá bán")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
        [Display(Name = "Giá bán")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Display(Name = "Giá gốc")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? OriginalPrice { get; set; }

        [Display(Name = "% Giảm giá")]
        [Range(0, 100)]
        public int? DiscountPercent { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số lượng tồn kho")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng không hợp lệ")]
        [Display(Name = "Số lượng tồn kho")]
        public int Stock { get; set; }

        [Display(Name = "Đã bán")]
        public int SoldCount { get; set; } = 0;

        // ===== CONTENT INFO =====
        
        [Display(Name = "Mô tả ngắn")]
        [StringLength(500)]
        public string? ShortDescription { get; set; }

        [Display(Name = "Mô tả chi tiết")]
        [Column(TypeName = "ntext")]
        public string? Description { get; set; }

        [Display(Name = "Mục lục")]
        [Column(TypeName = "ntext")]
        public string? TableOfContents { get; set; }

        // ===== MEDIA =====
        
        [Display(Name = "Ảnh bìa chính")]
        [StringLength(255)]
        public string? ImageUrl { get; set; }

        [Display(Name = "Ảnh bổ sung")]
        [StringLength(1000)]
        public string? AdditionalImages { get; set; } // JSON array hoặc CSV

        // ===== RATING & REVIEWS =====
        
        [Display(Name = "Đánh giá trung bình")]
        [Range(0, 5)]
        [Column(TypeName = "decimal(3,2)")]
        public decimal AverageRating { get; set; } = 0;

        [Display(Name = "Số lượt đánh giá")]
        public int ReviewCount { get; set; } = 0;

        // ===== STATUS & FLAGS =====
        
        [Display(Name = "Nổi bật")]
        public bool IsFeatured { get; set; } = false;

        [Display(Name = "Mới")]
        public bool IsNewArrival { get; set; } = false;

        [Display(Name = "Bán chạy")]
        public bool IsBestSeller { get; set; } = false;

        [Display(Name = "Còn hàng")]
        public bool IsAvailable { get; set; } = true;

        [Display(Name = "Đã ngừng kinh doanh")]
        public bool IsDiscontinued { get; set; } = false;

        // ===== RELATIONS =====
        
        [Required]
        [Display(Name = "Danh mục")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        // ===== METADATA =====
        
        [Display(Name = "Ngày tạo")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Display(Name = "Ngày cập nhật")]
        public DateTime? UpdatedDate { get; set; }

        [StringLength(100)]
        [Display(Name = "Người tạo")]
        public string? CreatedBy { get; set; }

        [StringLength(100)]
        [Display(Name = "Người cập nhật")]
        public string? UpdatedBy { get; set; }

        // ===== NAVIGATION PROPERTIES =====
        
        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

        // ===== COMPUTED PROPERTIES =====
        
        /// <summary>
        /// Kiểm tra sách có đang giảm giá không
        /// </summary>
        [NotMapped]
        public bool HasDiscount => OriginalPrice.HasValue && OriginalPrice.Value > Price;

        /// <summary>
        /// Tính giá sau khi giảm (nếu có)
        /// </summary>
        [NotMapped]
        public decimal FinalPrice
        {
            get
            {
                if (DiscountPercent.HasValue && DiscountPercent.Value > 0)
                {
                    return Price * (1 - DiscountPercent.Value / 100m);
                }
                return Price;
            }
        }

        /// <summary>
        /// Kiểm tra sách có sắp hết hàng không (≤ 10)
        /// </summary>
        [NotMapped]
        public bool IsLowStock => Stock > 0 && Stock <= 10;

        /// <summary>
        /// Kiểm tra sách có hết hàng không
        /// </summary>
        [NotMapped]
        public bool IsOutOfStock => Stock <= 0;

        /// <summary>
        /// Hiển thị rating dạng sao (★★★★☆)
        /// </summary>
        [NotMapped]
        public string RatingStars
        {
            get
            {
                var fullStars = (int)Math.Floor(AverageRating);
                var hasHalfStar = (AverageRating - fullStars) >= 0.5m;
                var emptyStars = 5 - fullStars - (hasHalfStar ? 1 : 0);

                return new string('★', fullStars) +
                       (hasHalfStar ? "½" : "") +
                       new string('☆', emptyStars);
            }
        }
    }
}
