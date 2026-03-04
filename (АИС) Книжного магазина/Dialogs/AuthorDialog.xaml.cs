using System.Windows;
using BookshopApp.Models;

namespace BookshopApp.Dialogs
{
    public partial class AuthorDialog : Window
    {
        public Author Result { get; private set; } = new();

        public AuthorDialog(Author? existing = null)
        {
            InitializeComponent();
            if (existing != null)
            {
                TxtLastName.Text   = existing.last_name;
                TxtFirstName.Text  = existing.first_name;
                TxtMiddleName.Text = existing.middle_name ?? "";
                DpBirth.SelectedDate = existing.birth_date;
            }
            else
            {
                DpBirth.SelectedDate = new DateTime(1980, 1, 1);
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtLastName.Text) || string.IsNullOrWhiteSpace(TxtFirstName.Text))
            {
                MessageBox.Show("Фамилия и Имя обязательны.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (DpBirth.SelectedDate == null)
            {
                MessageBox.Show("Укажите дату рождения.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Result = new Author
            {
                last_name   = TxtLastName.Text.Trim(),
                first_name  = TxtFirstName.Text.Trim(),
                middle_name = string.IsNullOrWhiteSpace(TxtMiddleName.Text) ? null : TxtMiddleName.Text.Trim(),
                birth_date  = DpBirth.SelectedDate.Value
            };
            DialogResult = true;
        }
    }
}
