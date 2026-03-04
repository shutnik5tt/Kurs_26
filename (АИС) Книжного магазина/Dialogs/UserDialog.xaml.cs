using System.Windows;
using System.Windows.Controls;
using BookshopApp.Models;

namespace BookshopApp.Dialogs
{
    public partial class UserDialog : Window
    {
        public User Result { get; private set; } = new();

        public UserDialog(User? existing = null)
        {
            InitializeComponent();
            if (existing != null)
            {
                TxtLogin.Text = existing.login;
                TxtPassword.Password = existing.password;
                foreach (ComboBoxItem item in CmbRole.Items)
                    if (item.Content.ToString() == existing.role) { item.IsSelected = true; break; }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtLogin.Text))
            { MessageBox.Show("Введите логин.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
            if (string.IsNullOrWhiteSpace(TxtPassword.Password))
            { MessageBox.Show("Введите пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning); return; }

            Result = new User
            {
                login    = TxtLogin.Text.Trim(),
                password = TxtPassword.Password,
                role     = (CmbRole.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "cashier"
            };
            DialogResult = true;
        }
    }
}
