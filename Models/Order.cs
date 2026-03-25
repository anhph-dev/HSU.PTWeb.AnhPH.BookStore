using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HSU.PTWeb.AnhPH.BookStore.Models
{
    // Model Order đại diện cho đơn hàng
    // Chứa thông tin chủ đơn (User), ngày đặt và trạng thái đơn
    public class Order
    {
        // Khóa chính
        public int OrderId { get; set; }

        // Khóa ngoại tới User
        public int UserId { get; set; }

        public int? AppUserId { get; set; }

        [Display(Name = "Tỉnh/Thành phố")]
        public int? CityId { get; set; }

        [Display(Name = "Phường/Xã")]
        public int? WardId { get; set; }

        // Navigation tới User
        [ForeignKey(nameof(UserId))]
        [InverseProperty(nameof(Models.User.Orders))]
        public User User { get; set; }

        [ForeignKey(nameof(AppUserId))]
        [InverseProperty(nameof(Models.User.AppOrders))]
        public User? AppUser { get; set; }

        public City? CityNavigation { get; set; }

        public Ward? WardNavigation { get; set; }

        // Ngày đặt hàng
        [Display(Name = "Ngày đặt hàng")]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        // Tổng tiền đơn hàng
        [Display(Name = "Tổng tiền")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        // Trạng thái đơn hàng (ví dụ: Pending, Completed, Cancelled)
        [Required]
        [StringLength(50)]
        [Display(Name = "Trạng thái")]
        public string Status { get; set; } = "Pending";

        // Shipping Information
        [Required(ErrorMessage = "Vui lòng nhập họ tên người nhận")]
        [StringLength(100)]
        [Display(Name = "Người nhận")]
        public string RecipientName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone]
        [StringLength(15)]
        [Display(Name = "Số điện thoại")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress]
        [StringLength(100)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ giao hàng")]
        [StringLength(500)]
        [Display(Name = "Địa chỉ giao hàng")]
        public string ShippingAddress { get; set; }

        [NotMapped]
        [StringLength(100)]
        [Display(Name = "Tỉnh/Thành phố")]
        public string City
        {
            get => CityNavigation?.CityName ?? string.Empty;
            set { }
        }

        [NotMapped]
        [StringLength(100)]
        [Display(Name = "Phường/Xã")]
        public string? Ward
        {
            get => WardNavigation?.WardName;
            set { }
        }

        [StringLength(500)]
        [Display(Name = "Ghi chú")]
        public string? Notes { get; set; }

        [StringLength(50)]
        [Display(Name = "Kênh đặt hàng")]
        public string Channel { get; set; } = "Online";

        // Payment Information
        [Required]
        [StringLength(50)]
        [Display(Name = "Phương thức thanh toán")]
        public string PaymentMethod { get; set; } = "COD"; // COD, BankTransfer, VNPay

        [StringLength(50)]
        [Display(Name = "Trạng thái thanh toán")]
        public string PaymentStatus { get; set; } = "Pending"; // Pending, Paid, Failed

        [Display(Name = "Ngày thanh toán")]
        public DateTime? PaidAt { get; set; }

        // Navigation: 1 Order có nhiều OrderDetail
        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
