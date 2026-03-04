using System.Windows;
using BookshopApp.Models;

namespace BookshopApp.Dialogs
{
    public partial class BookDialog : Window
    {
        public Book Result { get; private set; } = new();

        public BookDialog(Book? existing = null)
        {
            InitializeComponent();
            if (existing != null)
            {
                TxtTitle.Text        = existing.title;
                DpRelease.SelectedDate = existing.release_date;
                TxtPrice.Text        = existing.unit_price.ToString();
                TxtStock.Text        = existing.stock_quantity.ToString();
                TxtDesc.Text         = existing.description ?? "";
            }
            else
            {
                DpRelease.SelectedDate = DateTime.Today;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtTitle.Text))
            { MessageBox.Show("Введите название.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
            if (DpRelease.SelectedDate == null)
            { MessageBox.Show("Укажите дату выхода.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
            if (!decimal.TryParse(TxtPrice.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out decimal price) || price < 0)
            { MessageBox.Show("Некорректная цена.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
            if (!int.TryParse(TxtStock.Text, out int stock) || stock < 0)
            { MessageBox.Show("Некорректный остаток.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning); return; }

            Result = new Book
            {
                title          = TxtTitle.Text.Trim(),
                description    = string.IsNullOrWhiteSpace(TxtDesc.Text) ? null : TxtDesc.Text.Trim(),
                release_date   = DpRelease.SelectedDate.Value,
                unit_price     = price,
                stock_quantity = stock
            };
            DialogResult = true;
        }
    }
}
