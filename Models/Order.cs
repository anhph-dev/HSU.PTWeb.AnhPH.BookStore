using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace HSU.PTWeb.AnhPH.BookStore.Models
{
    // Model Order đại diện cho đơn hàng
    // Chứa thông tin chủ đơn (User), ngày đặt và trạng thái đơn
    public class Order
    {
        // Khóa chính
        public int OrderId { get; set; }

        // Tương thích: Id cũ
        [NotMapped]
        public int Id { get => OrderId; set => OrderId = value; }

        // Khóa ngoại tới User
        public int UserId { get; set; }

        // Navigation tới User
        public User User { get; set; }

        // Tên người đặt (tương thích với mã cũ)
        [NotMapped]
        public string UserName { get => User?.UserName; set { /* ignored */ } }

        // Ngày đặt hàng
        public DateTime OrderDate { get; set; }

        // Tổng tiền đơn hàng
        public decimal TotalAmount { get; set; }

        // Tương thích: Total cũ
        [NotMapped]
        public decimal Total { get => TotalAmount; set => TotalAmount = value; }

        // Trạng thái đơn hàng (ví dụ: Pending, Completed, Cancelled)
        public string Status { get; set; }

        // Navigation: 1 Order có nhiều OrderDetail
        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
