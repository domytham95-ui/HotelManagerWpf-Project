using System;
using System.ComponentModel.DataAnnotations;

namespace HotelManagerWpf.Models
{
    public class Employee : BaseEntity
    {
        [Required, StringLength(100)]
        public string FullName { get; set; } = string.Empty;
        [Required, StringLength(20)]
        public string Gender { get; set; } = string.Empty;
        [Required, StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;
        [Required, StringLength(100)]
        public string Position { get; set; } = string.Empty;
        [Range(0, 999999999)]
        public decimal Salary { get; set; }
        public DateTime HireDate { get; set; } = DateTime.Now;
        public override string GetDisplayInfo() => $"{FullName} - {Position}";
    }
}
