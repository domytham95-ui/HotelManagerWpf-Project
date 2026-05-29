using HotelManagerWpf.PhanAiMoRong;
using HotelManagerWpf.Models;
using HotelManagerWpf.Services;
using System.Windows;
using System.Windows.Controls;

namespace HotelManagerWpf.Views
{
    public partial class RoomsView : UserControl
    {
        private readonly BaseCrudService<Room> _service = new BaseCrudService<Room>();
        private readonly string _currentRole;
        private Room? _selectedRoom;

        public RoomsView(string currentRole = UserRole.Admin)
        {
            _currentRole = RolePermissionService.NormalizeRole(currentRole);
            InitializeComponent();
            DeleteButton.Visibility = RolePermissionService.CanUseAction(_currentRole, AppFeature.Rooms, PermissionAction.Delete)
                ? Visibility.Visible
                : Visibility.Collapsed;
            Loaded += async (_, _) => await LoadRoomsAsync();
        }

        private async Task LoadRoomsAsync() => RoomsDataGrid.ItemsSource = await _service.GetAllAsync();

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!decimal.TryParse(PriceTextBox.Text, out var price)) { MessageBox.Show("Invalid price"); return; }
            var room = _selectedRoom ?? new Room();
            room.RoomNumber = RoomNumberTextBox.Text.Trim();
            room.RoomType = (RoomTypeComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Standard";
            room.BedType = (BedTypeComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Single";
            room.PricePerNight = price;
            room.Status = (StatusComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Available";
            var ok = _selectedRoom == null ? await _service.CreateAsync(room) : await _service.UpdateAsync(room);
            MessageBox.Show(ok ? "Saved successfully" : "Save failed");
            ClearForm();
            await LoadRoomsAsync();
        }

        private void RoomsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedRoom = RoomsDataGrid.SelectedItem as Room;
            if (_selectedRoom == null) return;
            RoomNumberTextBox.Text = _selectedRoom.RoomNumber;
            PriceTextBox.Text = _selectedRoom.PricePerNight.ToString();
            SelectCombo(RoomTypeComboBox, _selectedRoom.RoomType);
            SelectCombo(BedTypeComboBox, _selectedRoom.BedType);
            SelectCombo(StatusComboBox, _selectedRoom.Status);
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (!RolePermissionService.CanUseAction(_currentRole, AppFeature.Rooms, PermissionAction.Delete))
            {
                MessageBox.Show("Only Admin can delete rooms.", "Access Denied", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_selectedRoom == null) { MessageBox.Show("Select a room first"); return; }
            if (MessageBox.Show("Delete selected room?", "Confirm", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;
            var ok = await _service.DeleteAsync(_selectedRoom.Id);
            MessageBox.Show(ok ? "Deleted successfully" : "Delete failed");
            ClearForm();
            await LoadRoomsAsync();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e) => ClearForm();
        private void ClearForm()
        {
            _selectedRoom = null;
            RoomNumberTextBox.Text = PriceTextBox.Text = string.Empty;
            RoomTypeComboBox.SelectedIndex = BedTypeComboBox.SelectedIndex = StatusComboBox.SelectedIndex = -1;
            RoomsDataGrid.SelectedItem = null;
        }
        private static void SelectCombo(ComboBox comboBox, string value)
        {
            foreach (ComboBoxItem item in comboBox.Items)
                if (item.Content?.ToString() == value) comboBox.SelectedItem = item;
        }
    }
}
