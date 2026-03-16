using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HSU.PTWeb.AnhPH.BookStore.Models
{
    public class Ward
    {
        public int WardId { get; set; }

        [Required]
        [Display(Name = "Tỉnh/Thành phố")]
        public int CityId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Phường/Xã")]
        [StringLength(100)]
        [Display(Name = "Phường/Xã")]
        public string WardName { get; set; }

        public bool IsActive { get; set; } = true;

        [ForeignKey(nameof(CityId))]
        public City City { get; set; }
    }
}
