using HotelManagerWpf.Models;
using HotelManagerWpf.PhanAiMoRong;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace HotelManagerWpf.Views
{
    public partial class UserStatusView : UserControl
    {
        private readonly User _currentUser;
        private readonly UserPresenceService _presenceService = new UserPresenceService();

        public UserStatusView(User currentUser)
        {
            _currentUser = currentUser;
            InitializeComponent();
            Loaded += async (_, _) => await LoadStatusesAsync();
        }

        private async System.Threading.Tasks.Task LoadStatusesAsync()
        {
            try
            {
                var visibleRoles = RolePermissionService
                    .GetVisibleSubordinateRoles(_currentUser.Role)
                    .Select(RolePermissionService.NormalizeRole)
                    .Distinct()
                    .ToArray();

                DescriptionTextBlock.Text = visibleRoles.Length == 0
                    ? "No subordinate roles available."
                    : $"Showing: {string.Join(", ", visibleRoles)}";

                UserStatusDataGrid.ItemsSource = await _presenceService.GetSubordinateStatusesAsync(_currentUser.Role, _currentUser.Id);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cannot load user statuses: " + ex.Message, "User Status Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            await LoadStatusesAsync();
        }
    }
}
