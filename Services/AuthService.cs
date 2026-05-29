using HotelManagerWpf.Data;
using HotelManagerWpf.Models;
using HotelManagerWpf.PhanAiMoRong;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagerWpf.Services
{
    public class AuthService
    {
        public async Task<bool> RegisterAsync(string username, string password, string role = UserRole.Staff)
        {
            try
            {
                using var context = new AppDbContext();
                if (await context.Users.AnyAsync(u => u.Username == username)) return false;
                var salt = Guid.NewGuid().ToString();
                context.Users.Add(new User
                {
                    Username = username,
                    PasswordSalt = salt,
                    PasswordHash = HashPassword(password, salt),
                    Role = RolePermissionService.NormalizeRole(role)
                });
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }

        public async Task<User?> LoginAsync(string username, string password)
        {
            try
            {
                using var context = new AppDbContext();
                var user = await context.Users.FirstOrDefaultAsync(u => u.Username == username);
                if (user == null) return null;
                return HashPassword(password, user.PasswordSalt) == user.PasswordHash ? user : null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        public async Task SeedAsync()
        {
            using var context = new AppDbContext();
            await context.Database.EnsureCreatedAsync();
            await new UserPresenceService().EnsurePresenceColumnsAsync();
            if (!await context.Users.AnyAsync(u => u.Username == "admin"))
            {
                await RegisterAsync("admin", "123456", UserRole.Admin);
            }
            if (!await context.Rooms.AnyAsync())
            {
                context.Rooms.AddRange(
                    new Room { RoomNumber = "101", RoomType = "Standard", BedType = "Single", PricePerNight = 300000, Status = "Available" },
                    new Room { RoomNumber = "102", RoomType = "Standard", BedType = "Double", PricePerNight = 450000, Status = "Available" },
                    new Room { RoomNumber = "201", RoomType = "Deluxe", BedType = "Queen", PricePerNight = 700000, Status = "Available" },
                    new Room { RoomNumber = "301", RoomType = "VIP", BedType = "King", PricePerNight = 1200000, Status = "Available" }
                );
                await context.SaveChangesAsync();
            }
        }

        private static string HashPassword(string password, string salt)
        {
            var bytes = Encoding.UTF8.GetBytes(password + salt);
            return Convert.ToBase64String(SHA256.HashData(bytes));
        }
    }
}
