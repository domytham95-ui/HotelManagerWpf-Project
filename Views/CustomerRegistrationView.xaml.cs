using HotelManagerWpf.Data;
using HotelManagerWpf.Models;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;

namespace HotelManagerWpf.Views
{
    public partial class CustomerRegistrationView : UserControl
    {
        public CustomerRegistrationView()
        {
            InitializeComponent();
            CheckInDatePicker.SelectedDate = DateTime.Now;
            Loaded += async (_, _) => await LoadRoomsAsync();
            RoomComboBox.SelectionChanged += (_, _) => UpdatePrice();
        }

        private async Task LoadRoomsAsync()
        {
            using var context = new AppDbContext();
            RoomComboBox.ItemsSource = await context.Rooms.Where(r => r.Status == "Available").OrderBy(r => r.RoomNumber).ToListAsync();
        }

        private void UpdatePrice()
        {
            if (RoomComboBox.SelectedItem is Room room) TotalPriceTextBox.Text = room.PricePerNight.ToString("0");
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (RoomComboBox.SelectedItem is not Room selectedRoom)
                {
                    MessageBox.Show("Please select a room");
                    return;
                }
                if (string.IsNullOrWhiteSpace(FullNameTextBox.Text) || string.IsNullOrWhiteSpace(PhoneTextBox.Text) || string.IsNullOrWhiteSpace(IdentityTextBox.Text))
                {
                    MessageBox.Show("Full name, phone, and identity number are required");
                    return;
                }
                using var context = new AppDbContext();
                var room = await context.Rooms.FindAsync(selectedRoom.Id);
                if (room == null || room.Status != "Available")
                {
                    MessageBox.Show("Room is no longer available");
                    await LoadRoomsAsync();
                    return;
                }
                var guest = new Guest
                {
                    FullName = FullNameTextBox.Text.Trim(),
                    PhoneNumber = PhoneTextBox.Text.Trim(),
                    Email = EmailTextBox.Text.Trim(),
                    IdentityNumber = IdentityTextBox.Text.Trim(),
                    Address = AddressTextBox.Text.Trim()
                };
                context.Guests.Add(guest);
                await context.SaveChangesAsync();
                context.Bookings.Add(new Booking
                {
                    GuestId = guest.Id,
                    RoomId = room.Id,
                    CheckInDate = CheckInDatePicker.SelectedDate ?? DateTime.Now,
                    TotalPrice = room.PricePerNight,
                    Status = "Active"
                });
                room.Status = "Occupied";
                await context.SaveChangesAsync();
                MessageBox.Show("Customer registered successfully");
                ClearForm();
                await LoadRoomsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Register failed: " + ex.Message);
            }
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e) => await LoadRoomsAsync();
        private void ClearForm()
        {
            FullNameTextBox.Text = PhoneTextBox.Text = EmailTextBox.Text = IdentityTextBox.Text = AddressTextBox.Text = TotalPriceTextBox.Text = string.Empty;
            RoomComboBox.SelectedItem = null;
            CheckInDatePicker.SelectedDate = DateTime.Now;
        }
    }
}
