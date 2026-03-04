using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using BookshopApp.Data;
using BookshopApp.Dialogs;
using BookshopApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BookshopApp.Views
{
    public partial class MainWindow : Window
    {
        private User _currentUser;

        public MainWindow(User user)
        {
            InitializeComponent();
            _currentUser = user;
            TxtUserInfo.Text = $"Пользователь: {user.login}  |  Роль: {user.role}";

            // Скрыть вкладку пользователей для кассира
            if (user.role != "admin")
                TabUsers.Visibility = Visibility.Collapsed;

            LoadAll();
        }

        private void LoadAll()
        {
            LoadAuthors();
            LoadBooks();
            LoadAuthorBooks();
            LoadUsers();
            LoadTransactions();
        }

        // ===== АВТОРЫ =====

        private void LoadAuthors(string search = "")
        {
            using var db = new AppDbContext();
            var q = db.Authors.AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
                q = q.Where(a => a.last_name.Contains(search) || a.first_name.Contains(search));
            GridAuthors.ItemsSource = q.OrderBy(a => a.last_name).ToList();
        }

        private void BtnAuthorAdd_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new AuthorDialog();
            if (dlg.ShowDialog() == true) { SaveAuthor(dlg.Result); LoadAuthors(); }
        }

        private void BtnAuthorEdit_Click(object sender, RoutedEventArgs e)
        {
            if (GridAuthors.SelectedItem is not Author selected) { MessageBox.Show("Выберите автора."); return; }
            var dlg = new AuthorDialog(selected);
            if (dlg.ShowDialog() == true)
            {
                using var db = new AppDbContext();
                var a = db.Authors.Find(selected.ID_author)!;
                a.last_name = dlg.Result.last_name;
                a.first_name = dlg.Result.first_name;
                a.middle_name = dlg.Result.middle_name;
                a.birth_date = dlg.Result.birth_date;
                db.SaveChanges();
                LoadAuthors();
            }
        }

        private void BtnAuthorDelete_Click(object sender, RoutedEventArgs e)
        {
            if (GridAuthors.SelectedItem is not Author selected) { MessageBox.Show("Выберите автора."); return; }
            if (MessageBox.Show($"Удалить автора «{selected.FullName}»?", "Подтверждение",
                MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                using var db = new AppDbContext();
                var a = db.Authors.Include(x => x.AuthorBooks).FirstOrDefault(x => x.ID_author == selected.ID_author);
                if (a != null) { db.AuthorBooks.RemoveRange(a.AuthorBooks); db.Authors.Remove(a); db.SaveChanges(); }
                LoadAuthors(); LoadAuthorBooks();
            }
        }

        private void BtnAuthorRefresh_Click(object sender, RoutedEventArgs e) => LoadAuthors(TxtAuthorSearch.Text);
        private void TxtAuthorSearch_TextChanged(object sender, TextChangedEventArgs e) => LoadAuthors(TxtAuthorSearch.Text);

        private void SaveAuthor(Author a)
        {
            using var db = new AppDbContext();
            db.Authors.Add(a);
            db.SaveChanges();
        }

        // ===== КНИГИ =====

        private void LoadBooks(string search = "")
        {
            using var db = new AppDbContext();
            var q = db.Books.AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
                q = q.Where(b => b.title.Contains(search));
            GridBooks.ItemsSource = q.OrderBy(b => b.title).ToList();
        }

        private void BtnBookAdd_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new BookDialog();
            if (dlg.ShowDialog() == true) { SaveBook(dlg.Result); LoadBooks(); }
        }

        private void BtnBookEdit_Click(object sender, RoutedEventArgs e)
        {
            if (GridBooks.SelectedItem is not Book selected) { MessageBox.Show("Выберите книгу."); return; }
            var dlg = new BookDialog(selected);
            if (dlg.ShowDialog() == true)
            {
                using var db = new AppDbContext();
                var b = db.Books.Find(selected.ID_book)!;
                b.title = dlg.Result.title;
                b.description = dlg.Result.description;
                b.release_date = dlg.Result.release_date;
                b.unit_price = dlg.Result.unit_price;
                b.stock_quantity = dlg.Result.stock_quantity;
                db.SaveChanges();
                LoadBooks();
            }
        }

        private void BtnBookDelete_Click(object sender, RoutedEventArgs e)
        {
            if (GridBooks.SelectedItem is not Book selected) { MessageBox.Show("Выберите книгу."); return; }
            if (MessageBox.Show($"Удалить книгу «{selected.title}»?", "Подтверждение",
                MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                using var db = new AppDbContext();
                var b = db.Books.Include(x => x.AuthorBooks).FirstOrDefault(x => x.ID_book == selected.ID_book);
                if (b != null) { db.AuthorBooks.RemoveRange(b.AuthorBooks); db.Books.Remove(b); db.SaveChanges(); }
                LoadBooks(); LoadAuthorBooks();
            }
        }

        private void BtnBookRefresh_Click(object sender, RoutedEventArgs e) => LoadBooks(TxtBookSearch.Text);
        private void TxtBookSearch_TextChanged(object sender, TextChangedEventArgs e) => LoadBooks(TxtBookSearch.Text);

        private void SaveBook(Book b)
        {
            using var db = new AppDbContext();
            db.Books.Add(b);
            db.SaveChanges();
        }

        // ===== АВТОРЫ-КНИГИ =====

        private void LoadAuthorBooks()
        {
            using var db = new AppDbContext();
            GridAuthorBooks.ItemsSource = db.AuthorBooks
                .Include(ab => ab.Author)
                .Include(ab => ab.Book)
                .ToList();
        }

        private void BtnAbAdd_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new AuthorBookDialog();
            if (dlg.ShowDialog() == true)
            {
                using var db = new AppDbContext();
                // Проверка дубликата
                bool exists = db.AuthorBooks.Any(ab => ab.ID_author == dlg.Result.ID_author && ab.ID_book == dlg.Result.ID_book);
                if (exists) { MessageBox.Show("Такая связь уже существует."); return; }
                db.AuthorBooks.Add(dlg.Result);
                db.SaveChanges();
                LoadAuthorBooks();
            }
        }

        private void BtnAbDelete_Click(object sender, RoutedEventArgs e)
        {
            if (GridAuthorBooks.SelectedItem is not AuthorBook selected) { MessageBox.Show("Выберите запись."); return; }
            if (MessageBox.Show("Удалить связь автор-книга?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                using var db = new AppDbContext();
                var ab = db.AuthorBooks.Find(selected.ID_author_book);
                if (ab != null) { db.AuthorBooks.Remove(ab); db.SaveChanges(); }
                LoadAuthorBooks();
            }
        }

        private void BtnAbRefresh_Click(object sender, RoutedEventArgs e) => LoadAuthorBooks();

        // ===== ПОЛЬЗОВАТЕЛИ =====

        private void LoadUsers()
        {
            if (_currentUser.role != "admin") return;
            using var db = new AppDbContext();
            GridUsers.ItemsSource = db.Users.ToList();
        }

        private void BtnUserAdd_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new UserDialog();
            if (dlg.ShowDialog() == true)
            {
                using var db = new AppDbContext();
                if (db.Users.Any(u => u.login == dlg.Result.login)) { MessageBox.Show("Пользователь с таким логином уже существует."); return; }
                db.Users.Add(dlg.Result);
                db.SaveChanges();
                LoadUsers();
            }
        }

        private void BtnUserEdit_Click(object sender, RoutedEventArgs e)
        {
            if (GridUsers.SelectedItem is not User selected) { MessageBox.Show("Выберите пользователя."); return; }
            var dlg = new UserDialog(selected);
            if (dlg.ShowDialog() == true)
            {
                using var db = new AppDbContext();
                var u = db.Users.Find(selected.ID_user)!;
                u.login = dlg.Result.login;
                u.password = dlg.Result.password;
                u.role = dlg.Result.role;
                db.SaveChanges();
                LoadUsers();
            }
        }

        private void BtnUserDelete_Click(object sender, RoutedEventArgs e)
        {
            if (GridUsers.SelectedItem is not User selected) { MessageBox.Show("Выберите пользователя."); return; }
            if (selected.ID_user == _currentUser.ID_user) { MessageBox.Show("Нельзя удалить текущего пользователя."); return; }
            if (MessageBox.Show($"Удалить пользователя «{selected.login}»?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                using var db = new AppDbContext();
                var u = db.Users.Find(selected.ID_user);
                if (u != null) { db.Users.Remove(u); db.SaveChanges(); }
                LoadUsers();
            }
        }

        private void BtnUserRefresh_Click(object sender, RoutedEventArgs e) => LoadUsers();

        // ===== ТРАНЗАКЦИИ =====

        private void LoadTransactions()
        {
            using var db = new AppDbContext();
            var list = db.Transactions
                .Include(t => t.User)
                .Include(t => t.BookTransactions)
                .OrderByDescending(t => t.sale_date)
                .ToList()
                .Select(t => new TransactionViewModel(t))
                .ToList();
            GridTransactions.ItemsSource = list;
        }

        private void GridTransactions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GridTransactions.SelectedItem is TransactionViewModel vm)
            {
                using var db = new AppDbContext();
                GridTransactionItems.ItemsSource = db.BookTransactions
                    .Include(bt => bt.Book)
                    .Where(bt => bt.ID_transaction == vm.Transaction.ID_transaction)
                    .ToList();
            }
            else GridTransactionItems.ItemsSource = null;
        }

        private void BtnTransactionAdd_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new TransactionDialog(_currentUser);
            if (dlg.ShowDialog() == true) { LoadTransactions(); LoadBooks(); }
        }

        private void BtnTransactionView_Click(object sender, RoutedEventArgs e)
        {
            if (GridTransactions.SelectedItem is not TransactionViewModel vm) { MessageBox.Show("Выберите продажу."); return; }
            using var db = new AppDbContext();
            var items = db.BookTransactions.Include(bt => bt.Book).Where(bt => bt.ID_transaction == vm.Transaction.ID_transaction).ToList();
            string info = $"Дата: {vm.Transaction.sale_date:dd.MM.yyyy HH:mm}\n" +
                          $"Кассир: {vm.Transaction.User?.login}\n" +
                          $"Оплата: {vm.Transaction.payment_type}\n\n" +
                          string.Join("\n", items.Select(i => $"  {i.Book.title} x{i.quantity} = {i.total_price} руб.")) +
                          $"\n\nИтого: {items.Sum(i => i.total_price)} руб.";
            MessageBox.Show(info, "Детали продажи");
        }

        private void BtnTransactionDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUser.role != "admin") { MessageBox.Show("Только администратор может удалять продажи."); return; }
            if (GridTransactions.SelectedItem is not TransactionViewModel vm) { MessageBox.Show("Выберите продажу."); return; }
            if (MessageBox.Show("Удалить эту продажу?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                using var db = new AppDbContext();
                var t = db.Transactions.Include(x => x.BookTransactions).FirstOrDefault(x => x.ID_transaction == vm.Transaction.ID_transaction);
                if (t != null)
                {
                    // Вернуть остатки
                    foreach (var bt in t.BookTransactions)
                    {
                        var book = db.Books.Find(bt.ID_book);
                        if (book != null) book.stock_quantity += bt.quantity;
                    }
                    db.BookTransactions.RemoveRange(t.BookTransactions);
                    db.Transactions.Remove(t);
                    db.SaveChanges();
                }
                LoadTransactions(); LoadBooks();
            }
        }

        private void BtnTransactionRefresh_Click(object sender, RoutedEventArgs e) => LoadTransactions();

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            var login = new LoginWindow();
            login.Show();
            this.Close();
        }
    }

    // Вспомогательный класс для отображения итога транзакции
    public class TransactionViewModel
    {
        public Transaction Transaction { get; }
        public decimal TotalSum { get; }

        public TransactionViewModel(Transaction t)
        {
            Transaction = t;
            TotalSum = t.BookTransactions.Sum(bt => bt.total_price);
        }
    }
}
