using System.ComponentModel.DataAnnotations;

namespace HSU.PTWeb.AnhPH.BookStore.Models
{
    public class Supplier
    {
        public int SupplierId { get; set; }

        [Required]
        [StringLength(200)]
        public string SupplierName { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Phone { get; set; }

        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(300)]
        public string? Address { get; set; }
    }
}
