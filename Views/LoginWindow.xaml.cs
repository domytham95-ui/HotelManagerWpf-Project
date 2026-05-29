using HotelManagerWpf.PhanAiMoRong;
using HotelManagerWpf.Services;
using System.Windows;

namespace HotelManagerWpf.Views
{
    public partial class LoginWindow : Window
    {
        private readonly AuthService _authService = new AuthService();
        private readonly UserPresenceService _presenceService = new UserPresenceService();
        public LoginWindow() => InitializeComponent();

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var username = UsernameTextBox.Text.Trim();
                var password = PasswordBox.Password.Trim();
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    MessageBox.Show("Please enter username and password");
                    return;
                }

                var user = await _authService.LoginAsync(username, password);
                if (user == null)
                {
                    MessageBox.Show("Invalid username or password");
                    return;
                }

                await _presenceService.MarkOnlineAsync(user.Id);
                user.IsOnline = true;
                user.LastSeenAt = System.DateTime.Now;

                new MainWindow(user).Show();
                Close();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Login failed: " + ex.Message, "Login Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenRegisterButton_Click(object sender, RoutedEventArgs e)
        {
            new RegisterWindow().ShowDialog();
        }
    }
}
