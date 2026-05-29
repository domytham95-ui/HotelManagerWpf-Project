using System.ComponentModel.DataAnnotations;

namespace HotelManagerWpf.Models
{
    public class User : BaseEntity
    {
        [Required, StringLength(50)]
        public string Username { get; set; } = string.Empty;
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        [Required]
        public string PasswordSalt { get; set; } = string.Empty;
        [Required, StringLength(20)]
        public string Role { get; set; } = "Admin";
        public bool IsOnline { get; set; }
        public DateTime? LastSeenAt { get; set; }
        public override string GetDisplayInfo() => $"{Username} - {Role}";
    }
}
