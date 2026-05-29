using System.ComponentModel.DataAnnotations;

namespace HotelManagerWpf.Models
{
    public class Guest : BaseEntity
    {
        [Required, StringLength(100)]
        public string FullName { get; set; } = string.Empty;
        [Required, StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;
        [Required, StringLength(30)]
        public string IdentityNumber { get; set; } = string.Empty;
        [StringLength(200)]
        public string Address { get; set; } = string.Empty;
        public override string GetDisplayInfo() => $"{FullName} - {PhoneNumber}";
    }
}
