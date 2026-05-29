using HotelManagerWpf.Data;
using HotelManagerWpf.Models;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;

namespace HotelManagerWpf.Views
{
    public partial class CheckOutView : UserControl
    {
        public CheckOutView()
        {
            InitializeComponent();
            Loaded += async (_, _) => await LoadBookingsAsync();
        }

        private async Task LoadBookingsAsync()
        {
            using var context = new AppDbContext();
            BookingsDataGrid.ItemsSource = await context.Bookings.Include(b => b.Guest).Include(b => b.Room).Where(b => b.Status == "Active").OrderByDescending(b => b.Id).ToListAsync();
        }

        private async void CheckOutButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (BookingsDataGrid.SelectedItem is not Booking selected)
                {
                    MessageBox.Show("Select a booking first");
                    return;
                }
                if (MessageBox.Show("Check out selected customer?", "Confirm", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;
                using var context = new AppDbContext();
                var booking = await context.Bookings.Include(b => b.Room).FirstOrDefaultAsync(b => b.Id == selected.Id);
                if (booking == null) return;
                booking.Status = "CheckedOut";
                booking.CheckOutDate = DateTime.Now;
                if (booking.Room != null) booking.Room.Status = "Available";
                await context.SaveChangesAsync();
                MessageBox.Show("Check out successfully");
                await LoadBookingsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Check out failed: " + ex.Message);
            }
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e) => await LoadBookingsAsync();
    }
}
