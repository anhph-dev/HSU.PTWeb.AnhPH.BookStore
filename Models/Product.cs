using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HSU.PTWeb.AnhPH.BookStore.Models
{
    // Model Product đại diện cho sách trong cửa hàng
    // Chứa thông tin cơ bản của sách như tên, giá, mô tả, ảnh và kho
    public class Product
    {
        // Khóa chính cho bảng Product
        public int ProductId { get; set; }

        // Tương thích: Id cũ
        [NotMapped]
        public int Id { get => ProductId; set => ProductId = value; }

        // Tên sản phẩm (tên sách)
        [Required]
        public string ProductName { get; set; }

        // Tương thích: Title cũ
        [NotMapped]
        public string Title { get => ProductName; set => ProductName = value; }

        // Giá bán
        public decimal Price { get; set; }

        // Mô tả chi tiết về sản phẩm
        public string Description { get; set; }

        // Đường dẫn ảnh hiển thị sản phẩm
        public string ImageUrl { get; set; }

        // Tương thích: Image cũ
        [NotMapped]
        public string Image { get => ImageUrl; set => ImageUrl = value; }

        // Khóa ngoại tới Category
        public int CategoryId { get; set; }

        // Navigation tới Category
        public Category Category { get; set; }

        // Số lượng tồn kho
        public int Stock { get; set; }

        // Ngày tạo sản phẩm
        public DateTime CreatedDate { get; set; }

        // Navigation: 1 Product có thể xuất hiện trong nhiều OrderDetail
        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
