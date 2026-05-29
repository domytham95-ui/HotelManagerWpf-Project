using HotelManagerWpf.Data;
using HotelManagerWpf.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelManagerWpf.PhanAiMoRong
{
    public class UserPresenceService
    {
        public async Task EnsurePresenceColumnsAsync()
        {
            await using var connection = new SqliteConnection("Data Source=hotel_manager_wpf.db");
            await connection.OpenAsync();

            var columns = new List<string>();
            await using (var command = connection.CreateCommand())
            {
                command.CommandText = "PRAGMA table_info('Users')";
                await using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    columns.Add(reader.GetString(1));
                }
            }

            if (!columns.Contains(nameof(User.IsOnline), StringComparer.OrdinalIgnoreCase))
            {
                await using var command = connection.CreateCommand();
                command.CommandText = "ALTER TABLE Users ADD COLUMN IsOnline INTEGER NOT NULL DEFAULT 0";
                await command.ExecuteNonQueryAsync();
            }

            if (!columns.Contains(nameof(User.LastSeenAt), StringComparer.OrdinalIgnoreCase))
            {
                await using var command = connection.CreateCommand();
                command.CommandText = "ALTER TABLE Users ADD COLUMN LastSeenAt TEXT NULL";
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task MarkOnlineAsync(int userId)
        {
            using var context = new AppDbContext();
            var user = await context.Users.FindAsync(userId);
            if (user == null) return;

            user.IsOnline = true;
            user.LastSeenAt = DateTime.Now;
            await context.SaveChangesAsync();
        }

        public async Task MarkOfflineAsync(int userId)
        {
            using var context = new AppDbContext();
            var user = await context.Users.FindAsync(userId);
            if (user == null) return;

            user.IsOnline = false;
            user.LastSeenAt = DateTime.Now;
            await context.SaveChangesAsync();
        }

        public async Task<List<UserPresenceRow>> GetSubordinateStatusesAsync(string currentRole, int currentUserId)
        {
            using var context = new AppDbContext();
            var visibleRoles = RolePermissionService.GetVisibleSubordinateRoles(currentRole);

            return await context.Users
                .Where(u => u.Id != currentUserId && visibleRoles.Contains(u.Role))
                .OrderBy(u => u.Role)
                .ThenBy(u => u.Username)
                .Select(u => new UserPresenceRow
                {
                    Username = u.Username,
                    Role = u.Role == UserRole.Receptionist ? UserRole.Staff : u.Role,
                    Status = u.IsOnline ? "Online" : "Offline",
                    LastSeen = u.LastSeenAt == null ? "" : u.LastSeenAt.Value.ToString("dd/MM/yyyy HH:mm")
                })
                .ToListAsync();
        }
    }

    public class UserPresenceRow
    {
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string LastSeen { get; set; } = string.Empty;
    }
}
