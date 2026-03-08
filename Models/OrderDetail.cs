using System.ComponentModel.DataAnnotations.Schema;

namespace HSU.PTWeb.AnhPH.BookStore.Models
{
    // Model OrderDetail lưu chi tiết từng sản phẩm trong đơn hàng
    // Lưu cấu trúc thực tế của bảng OrderDetails trong database
    public class OrderDetail
    {
        // Khóa chính, identity
        // Tương ứng cột: OrderDetailId (int, PK, identity)
        public int OrderDetailId { get; set; }

        // Khóa ngoại tới Order
        // Tương ứng cột: OrderId (int)
        public int OrderId { get; set; }

        // Navigation tới Order
        public Order Order { get; set; }

        // Khóa ngoại tới Product
        // Tương ứng cột: ProductId (int)
        public int ProductId { get; set; }

        // Navigation tới Product
        public Product Product { get; set; }

        // Số lượng sản phẩm trong order detail
        // Tương ứng cột: Quantity (int)
        public int Quantity { get; set; }

        // Giá đơn vị tại thời điểm đặt hàng
        // Tương ứng cột: UnitPrice (decimal)
        // Chỉ định kiểu cột để tránh mất mát precision khi dùng SQL Server
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }
    }
}
