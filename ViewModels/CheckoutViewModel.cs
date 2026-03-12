using System.ComponentModel.DataAnnotations;

namespace HSU.PTWeb.AnhPH.BookStore.ViewModels
{
    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập họ tên người nhận")]
        [Display(Name = "Họ và tên người nhận")]
        [StringLength(100)]
        public string RecipientName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        [StringLength(15)]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        [StringLength(100)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ giao hàng")]
        [Display(Name = "Địa chỉ giao hàng")]
        [StringLength(500)]
        public string ShippingAddress { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn Tỉnh/Thành phố")]
        [Display(Name = "Tỉnh/Thành phố")]
        [StringLength(100)]
        public string City { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Quận/Huyện")]
        [Display(Name = "Quận/Huyện")]
        [StringLength(100)]
        public string District { get; set; }

        [Display(Name = "Phường/Xã")]
        [StringLength(100)]
        public string Ward { get; set; }

        [Display(Name = "Ghi chú")]
        [StringLength(500)]
        public string Notes { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn phương thức thanh toán")]
        [Display(Name = "Phương thức thanh toán")]
        public string PaymentMethod { get; set; } = "COD";
    }
}
