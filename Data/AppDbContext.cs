using HotelManagerWpf.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelManagerWpf.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Room> Rooms => Set<Room>();
        public DbSet<Guest> Guests => Set<Guest>();
        public DbSet<Booking> Bookings => Set<Booking>();
        public DbSet<Employee> Employees => Set<Employee>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=hotel_manager_wpf.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
            modelBuilder.Entity<Room>().HasIndex(r => r.RoomNumber).IsUnique();
            modelBuilder.Entity<Booking>().HasOne(b => b.Guest).WithMany().HasForeignKey(b => b.GuestId);
            modelBuilder.Entity<Booking>().HasOne(b => b.Room).WithMany().HasForeignKey(b => b.RoomId);
        }
    }
}
