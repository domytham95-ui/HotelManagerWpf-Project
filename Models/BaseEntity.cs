using System;

namespace HotelManagerWpf.Models
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual string GetDisplayInfo()
        {
            return $"ID: {Id}";
        }
    }
}
