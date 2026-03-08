using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HSU.PTWeb.AnhPH.BookStore.Models
{
    // Model User đại diện cho người dùng của hệ thống
    // Chứa thông tin đăng nhập và thông tin cơ bản của người dùng
    public class User
    {
        // Khóa chính, định danh người dùng
        public int UserId { get; set; }

        // Tương thích: Id cũ trỏ tới UserId
        [NotMapped]
        public int Id { get => UserId; set => UserId = value; }

        // Tên đầy đủ của người dùng
        public string FullName { get; set; }

        // Email dùng để đăng nhập/liên hệ
        [Required]
        public string Email { get; set; }

        // Tương thích: UserName cũ ánh xạ tới Email
        [NotMapped]
        public string UserName { get => Email; set => Email = value; }

        // Mật khẩu đã được mã hóa (hash)
        [Required]
        public string Password { get; set; }

        // Vai trò của người dùng, ví dụ: "Admin" hoặc "Customer"
        public string Role { get; set; }

        // Ngày tạo tài khoản
        public DateTime CreatedDate { get; set; }

        // Navigation property: 1 User có thể có nhiều Order
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
