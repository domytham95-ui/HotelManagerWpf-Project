using HotelManagerWpf.Data;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;

namespace HotelManagerWpf.Views
{
    public partial class CustomerDetailsView : UserControl
    {
        private string _filter = "All";
        public CustomerDetailsView()
        {
            InitializeComponent();
            Loaded += async (_, _) => await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                using var context = new AppDbContext();
                var query = context.Bookings.Include(b => b.Guest).Include(b => b.Room).AsQueryable();
                if (_filter == "Active") query = query.Where(b => b.Status == "Active");
                if (_filter == "CheckedOut") query = query.Where(b => b.Status == "CheckedOut");
                var data = await query.OrderByDescending(b => b.Id).Select(b => new
                {
                    BookingId = b.Id,
                    FullName = b.Guest!.FullName,
                    Phone = b.Guest!.PhoneNumber,
                    IdentityNumber = b.Guest!.IdentityNumber,
                    RoomNumber = b.Room!.RoomNumber,
                    RoomType = b.Room!.RoomType,
                    CheckInDate = b.CheckInDate.ToString("dd/MM/yyyy"),
                    CheckOutDate = b.CheckOutDate == null ? "" : b.CheckOutDate.Value.ToString("dd/MM/yyyy"),
                    TotalPrice = b.TotalPrice,
                    Status = b.Status
                }).ToListAsync();
                CustomersDataGrid.ItemsSource = data;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cannot load customer details: " + ex.Message);
            }
        }

        private async void AllButton_Click(object sender, RoutedEventArgs e) { _filter = "All"; await LoadDataAsync(); }
        private async void CurrentButton_Click(object sender, RoutedEventArgs e) { _filter = "Active"; await LoadDataAsync(); }
        private async void CheckedOutButton_Click(object sender, RoutedEventArgs e) { _filter = "CheckedOut"; await LoadDataAsync(); }
        private async void RefreshButton_Click(object sender, RoutedEventArgs e) => await LoadDataAsync();
    }
}
