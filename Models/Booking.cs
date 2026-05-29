using System;
using System.ComponentModel.DataAnnotations;

namespace HotelManagerWpf.Models
{
    public class Booking : BaseEntity
    {
        public int GuestId { get; set; }
        public Guest? Guest { get; set; }
        public int RoomId { get; set; }
        public Room? Room { get; set; }
        public DateTime CheckInDate { get; set; } = DateTime.Now;
        public DateTime? CheckOutDate { get; set; }
        [Range(0, 999999999)]
        public decimal TotalPrice { get; set; }
        [Required, StringLength(30)]
        public string Status { get; set; } = "Active";
        public override string GetDisplayInfo() => $"{Guest?.FullName} - {Room?.RoomNumber} - {Status}";
    }
}
