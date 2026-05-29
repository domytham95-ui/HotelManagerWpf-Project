using HotelManagerWpf.Services;
using System.Windows;
using System.Windows.Controls;

namespace HotelManagerWpf.Views
{
    public partial class RegisterWindow : Window
    {
        private readonly AuthService _authService = new AuthService();

        public RegisterWindow()
        {
            InitializeComponent();
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var username = UsernameTextBox.Text.Trim();
            var password = PasswordBox.Password.Trim();
            var confirmPassword = ConfirmPasswordBox.Password.Trim();
            var role = (RoleComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Staff";

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter username and password");
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Password confirmation does not match");
                return;
            }

            var result = await _authService.RegisterAsync(username, password, role);

            if (!result)
            {
                MessageBox.Show("Username already exists or register failed");
                return;
            }

            MessageBox.Show("Register successfully. You can now login.");
            Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
