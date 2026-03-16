using System.ComponentModel.DataAnnotations;

namespace HSU.PTWeb.AnhPH.BookStore.Models
{
    public class City
    {
        public int CityId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Tỉnh/Thành phố")]
        [StringLength(100)]
        [Display(Name = "Tỉnh/Thành phố")]
        public string CityName { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<Ward> Wards { get; set; } = new List<Ward>();
    }
}
