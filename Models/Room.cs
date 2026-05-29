using System.ComponentModel.DataAnnotations;

namespace HotelManagerWpf.Models
{
    public class Room : BaseEntity
    {
        [Required, StringLength(20)]
        public string RoomNumber { get; set; } = string.Empty;
        [Required, StringLength(50)]
        public string RoomType { get; set; } = string.Empty;
        [Required, StringLength(50)]
        public string BedType { get; set; } = string.Empty;
        [Range(0, 999999999)]
        public decimal PricePerNight { get; set; }
        [Required, StringLength(30)]
        public string Status { get; set; } = "Available";
        public override string GetDisplayInfo() => $"{RoomNumber} - {RoomType} - {Status}";
    }
}
