using HotelManagerWpf.Services;
using HotelManagerWpf.Views;
using HotelManagerWpf.PhanAiMoRong;
using System;
using System.Windows;

namespace HotelManagerWpf
{
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                var authService = new AuthService();
                await authService.SeedAsync();
                await new UserPresenceService().EnsurePresenceColumnsAsync();
                new LoginWindow().Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cannot start application: " + ex.Message, "Startup Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }
    }
}
