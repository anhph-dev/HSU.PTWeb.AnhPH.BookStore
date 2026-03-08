using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HSU.PTWeb.AnhPH.BookStore.Models
{
    // Model Category đại diện cho danh mục sách
    // Chứa tên và mô tả của danh mục
    public class Category
    {
        // Khóa chính
        public int CategoryId { get; set; }

        // Tương thích với mã cũ
        [NotMapped]
        public int Id { get => CategoryId; set => CategoryId = value; }

        // Tên danh mục
        [Required]
        public string CategoryName { get; set; }

        // Tương thích với mã cũ
        [NotMapped]
        public string Name { get => CategoryName; set => CategoryName = value; }

        // Mô tả danh mục
        public string Description { get; set; }

        // Navigation: 1 Category có nhiều Product
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
