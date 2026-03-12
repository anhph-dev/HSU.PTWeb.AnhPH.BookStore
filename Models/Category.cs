using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HSU.PTWeb.AnhPH.BookStore.Models
{
    // Model Category đại diện cho danh mục sách
    // Chứa tên và mô tả của danh mục
    public class Category
    {
        // Khóa chính
        public int CategoryId { get; set; }

        // Tên danh mục
        [Required(ErrorMessage = "Vui lòng nhập tên danh mục")]
        [StringLength(100)]
        [Display(Name = "Tên danh mục")]
        public string CategoryName { get; set; }

        // Mô tả danh mục
        [StringLength(500)]
        [Display(Name = "Mô tả")]
        public string Description { get; set; }

        // Navigation: 1 Category có nhiều Product
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
