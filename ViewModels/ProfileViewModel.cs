using System.ComponentModel.DataAnnotations;

namespace HSU.PTWeb.AnhPH.BookStore.ViewModels
{
    public class ProfileViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [StringLength(100)]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [StringLength(15)]
        [Display(Name = "Số điện thoại")]
        public string? PhoneNumber { get; set; }

        [StringLength(500)]
        [Display(Name = "Địa chỉ giao hàng")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn Tỉnh/Thành phố")]
        [Display(Name = "Tỉnh/Thành phố")]
        public int? CityId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn Phường/Xã")]
        [Display(Name = "Phường/Xã")]
        public int? WardId { get; set; }

        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu mới tối thiểu 6 ký tự")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu mới")]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        [Display(Name = "Xác nhận mật khẩu mới")]
        public string? ConfirmPassword { get; set; }

        // Read-only display fields
        public string? Email { get; set; }
        public string? Role { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
