using HotelManagerWpf.Models;
using HotelManagerWpf.PhanAiMoRong;
using HotelManagerWpf.Views;
using System;
using System.Windows;
using System.Windows.Controls;

namespace HotelManagerWpf
{
    public partial class MainWindow : Window
    {
        private readonly User _currentUser;
        private readonly UserPresenceService _presenceService = new UserPresenceService();
        private bool _isLoggingOut;

        public MainWindow(User currentUser)
        {
            _currentUser = currentUser;
            InitializeComponent();
            ApplyRolePermissions();
            NavigateTo(() => new DashboardView(), "Dashboard");
        }

        private void DashboardButton_Click(object sender, RoutedEventArgs e) => NavigateTo(() => new DashboardView(), "Dashboard", AppFeature.Dashboard);
        private void RoomsButton_Click(object sender, RoutedEventArgs e) => NavigateTo(() => new RoomsView(_currentUser.Role), "Rooms", AppFeature.Rooms);
        private void CustomerRegistrationButton_Click(object sender, RoutedEventArgs e) => NavigateTo(() => new CustomerRegistrationView(), "Customer Registration", AppFeature.CustomerRegistration);
        private void CustomerDetailsButton_Click(object sender, RoutedEventArgs e) => NavigateTo(() => new CustomerDetailsView(), "Customer Details", AppFeature.CustomerDetails);
        private void CheckOutButton_Click(object sender, RoutedEventArgs e) => NavigateTo(() => new CheckOutView(), "Check Out", AppFeature.CheckOut);
        private void EmployeesButton_Click(object sender, RoutedEventArgs e) => NavigateTo(() => new EmployeesView(), "Employees", AppFeature.Employees);
        private void UserStatusButton_Click(object sender, RoutedEventArgs e) => NavigateTo(() => new UserStatusView(_currentUser), "User Status", AppFeature.UserStatus);
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _isLoggingOut = true;
                _ = _presenceService.MarkOfflineAsync(_currentUser.Id);
                new LoginWindow().Show();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cannot return to login: " + ex.Message, "Navigation Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NavigateTo(Func<UserControl> createView, string pageName, AppFeature feature = AppFeature.Dashboard)
        {
            try
            {
                if (!RolePermissionService.CanAccess(_currentUser.Role, feature))
                {
                    MessageBox.Show("Your account does not have permission to open this feature.", "Access Denied", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                MainContent.Content = createView();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Cannot open {pageName}: {ex.Message}", "Navigation Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyRolePermissions()
        {
            CurrentUserTextBlock.Text = $"{_currentUser.Username} | {RolePermissionService.NormalizeRole(_currentUser.Role)}";
            DashboardButton.Visibility = GetMenuVisibility(AppFeature.Dashboard);
            RoomsButton.Visibility = GetMenuVisibility(AppFeature.Rooms);
            CustomerRegistrationButton.Visibility = GetMenuVisibility(AppFeature.CustomerRegistration);
            CustomerDetailsButton.Visibility = GetMenuVisibility(AppFeature.CustomerDetails);
            CheckOutButton.Visibility = GetMenuVisibility(AppFeature.CheckOut);
            EmployeesButton.Visibility = GetMenuVisibility(AppFeature.Employees);
            UserStatusButton.Visibility = GetMenuVisibility(AppFeature.UserStatus);
        }

        private Visibility GetMenuVisibility(AppFeature feature)
        {
            return RolePermissionService.CanAccess(_currentUser.Role, feature) ? Visibility.Visible : Visibility.Collapsed;
        }

        protected override async void OnClosed(EventArgs e)
        {
            if (!_isLoggingOut)
            {
                await _presenceService.MarkOfflineAsync(_currentUser.Id);
            }

            base.OnClosed(e);
        }
    }
}
