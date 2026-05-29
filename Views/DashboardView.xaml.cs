using HotelManagerWpf.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace HotelManagerWpf.Views
{
    public partial class DashboardView : UserControl
    {
        public DashboardView()
        {
            InitializeComponent();
        }

        private async void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            await LoadDashboardAsync();
        }

        private async System.Threading.Tasks.Task LoadDashboardAsync()
        {
            try
            {
                using var context = new AppDbContext();

                var totalRooms = await context.Rooms.CountAsync();
                var availableRooms = await context.Rooms.CountAsync(r => r.Status == "Available");

                var activeBookings = await context.Bookings
                    .Include(b => b.Guest)
                    .Include(b => b.Room)
                    .Where(b => b.Status == "Active")
                    .OrderByDescending(b => b.CheckInDate)
                    .ToListAsync();

                var employees = await context.Employees.CountAsync();

                RoomsCountTextBlock.Text = totalRooms.ToString();
                AvailableRoomsCountTextBlock.Text = availableRooms.ToString();
                ActiveBookingsCountTextBlock.Text = activeBookings.Count.ToString();
                EmployeesCountTextBlock.Text = employees.ToString();

                var occupiedRooms = totalRooms - availableRooms;
                var occupancyRate = totalRooms == 0 ? 0 : (double)occupiedRooms / totalRooms * 100;
                var revenue = activeBookings.Sum(x => x.TotalPrice);

                OccupiedRoomsTextBlock.Text = occupiedRooms.ToString();
                OccupancyRateTextBlock.Text = $"{occupancyRate:0}%";
                OccupancyProgressBar.Value = occupancyRate;
                RevenueTextBlock.Text = revenue.ToString("N0");
                HeroRevenueTextBlock.Text = revenue.ToString("N0");

                TodayTextBlock.Text = $"Today: {DateTime.Now:dddd, dd/MM/yyyy}";

                RecentBookingsGrid.ItemsSource = activeBookings
                    .Take(6)
                    .Select(x => new
                    {
                        Customer = x.Guest != null ? x.Guest.FullName : "",
                        Room = x.Room != null ? x.Room.RoomNumber : "",
                        CheckIn = x.CheckInDate.ToString("dd/MM/yyyy"),
                        Total = x.TotalPrice.ToString("N0"),
                        Status = x.Status
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                RoomsCountTextBlock.Text = "0";
                AvailableRoomsCountTextBlock.Text = "0";
                ActiveBookingsCountTextBlock.Text = "0";
                EmployeesCountTextBlock.Text = "0";
                OccupiedRoomsTextBlock.Text = "0";
                OccupancyRateTextBlock.Text = "0%";
                OccupancyProgressBar.Value = 0;
                RevenueTextBlock.Text = "0";
                HeroRevenueTextBlock.Text = "0";
                MessageBox.Show("Cannot load dashboard: " + ex.Message, "Dashboard Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
