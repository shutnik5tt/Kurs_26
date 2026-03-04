using System.Windows;
using BookshopApp.Data;
using BookshopApp.Models;

namespace BookshopApp.Dialogs
{
    public partial class AuthorBookDialog : Window
    {
        public AuthorBook Result { get; private set; } = new();

        public AuthorBookDialog()
        {
            InitializeComponent();
            using var db = new AppDbContext();
            CmbAuthor.ItemsSource = db.Authors.OrderBy(a => a.last_name).ToList();
            CmbBook.ItemsSource   = db.Books.OrderBy(b => b.title).ToList();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (CmbAuthor.SelectedItem is not Author author)
            { MessageBox.Show("Выберите автора.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
            if (CmbBook.SelectedItem is not Book book)
            { MessageBox.Show("Выберите книгу.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning); return; }

            Result = new AuthorBook { ID_author = author.ID_author, ID_book = book.ID_book };
            DialogResult = true;
        }
    }
}
