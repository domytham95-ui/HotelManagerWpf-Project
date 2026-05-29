using HotelManagerWpf.Models;
using HotelManagerWpf.Services;
using System.Windows;
using System.Windows.Controls;

namespace HotelManagerWpf.Views
{
    public partial class EmployeesView : UserControl
    {
        private readonly BaseCrudService<Employee> _service = new BaseCrudService<Employee>();
        private Employee? _selectedEmployee;

        public EmployeesView()
        {
            InitializeComponent();
            HireDatePicker.SelectedDate = DateTime.Now;
            Loaded += async (_, _) => await LoadEmployeesAsync();
        }

        private async Task LoadEmployeesAsync() => EmployeesDataGrid.ItemsSource = await _service.GetAllAsync();

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!decimal.TryParse(SalaryTextBox.Text, out var salary)) { MessageBox.Show("Invalid salary"); return; }
            var employee = _selectedEmployee ?? new Employee();
            employee.FullName = FullNameTextBox.Text.Trim();
            employee.Gender = (GenderComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Other";
            employee.PhoneNumber = PhoneTextBox.Text.Trim();
            employee.Position = PositionTextBox.Text.Trim();
            employee.Salary = salary;
            employee.HireDate = HireDatePicker.SelectedDate ?? DateTime.Now;
            var ok = _selectedEmployee == null ? await _service.CreateAsync(employee) : await _service.UpdateAsync(employee);
            MessageBox.Show(ok ? "Saved successfully" : "Save failed");
            ClearForm();
            await LoadEmployeesAsync();
        }

        private void EmployeesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedEmployee = EmployeesDataGrid.SelectedItem as Employee;
            if (_selectedEmployee == null) return;
            FullNameTextBox.Text = _selectedEmployee.FullName;
            PhoneTextBox.Text = _selectedEmployee.PhoneNumber;
            PositionTextBox.Text = _selectedEmployee.Position;
            SalaryTextBox.Text = _selectedEmployee.Salary.ToString();
            HireDatePicker.SelectedDate = _selectedEmployee.HireDate;
            SelectCombo(GenderComboBox, _selectedEmployee.Gender);
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedEmployee == null) { MessageBox.Show("Select an employee first"); return; }
            if (MessageBox.Show("Delete selected employee?", "Confirm", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;
            var ok = await _service.DeleteAsync(_selectedEmployee.Id);
            MessageBox.Show(ok ? "Deleted successfully" : "Delete failed");
            ClearForm();
            await LoadEmployeesAsync();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e) => ClearForm();
        private void ClearForm()
        {
            _selectedEmployee = null;
            FullNameTextBox.Text = PhoneTextBox.Text = PositionTextBox.Text = SalaryTextBox.Text = string.Empty;
            GenderComboBox.SelectedIndex = -1;
            HireDatePicker.SelectedDate = DateTime.Now;
            EmployeesDataGrid.SelectedItem = null;
        }
        private static void SelectCombo(ComboBox comboBox, string value)
        {
            foreach (ComboBoxItem item in comboBox.Items)
                if (item.Content?.ToString() == value) comboBox.SelectedItem = item;
        }
    }
}
