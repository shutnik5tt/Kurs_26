using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using BookshopApp.Data;
using BookshopApp.Models;

namespace BookshopApp.Dialogs
{
    public partial class TransactionDialog : Window
    {
        private readonly User _user;
        private readonly ObservableCollection<SaleItem> _items = new();
        private List<Book> _books = new();

        public TransactionDialog(User user)
        {
            InitializeComponent();
            _user = user;
            GridItems.ItemsSource = _items;
            _items.CollectionChanged += (_, _) => UpdateTotal();

            using var db = new AppDbContext();
            _books = db.Books.Where(b => b.stock_quantity > 0).OrderBy(b => b.title).ToList();
            CmbBook.ItemsSource = _books;
        }

        private void CmbBook_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbBook.SelectedItem is Book b)
                TxtPrice.Text = $"{b.unit_price} руб.";
        }

        private void BtnAddItem_Click(object sender, RoutedEventArgs e)
        {
            if (CmbBook.SelectedItem is not Book book)
            { MessageBox.Show("Выберите книгу."); return; }
            if (!int.TryParse(TxtQty.Text, out int qty) || qty <= 0)
            { MessageBox.Show("Укажите корректное количество."); return; }

            var existing = _items.FirstOrDefault(i => i.ID_book == book.ID_book);
            int alreadyInCart = existing?.Quantity ?? 0;
            if (alreadyInCart + qty > book.stock_quantity)
            { MessageBox.Show($"Недостаточно на складе. Доступно: {book.stock_quantity - alreadyInCart} шт."); return; }

            if (existing != null)
            {
                existing.Quantity += qty;
                existing.TotalPrice = existing.Quantity * existing.UnitPrice;
                GridItems.Items.Refresh();
            }
            else
            {
                _items.Add(new SaleItem
                {
                    ID_book    = book.ID_book,
                    BookTitle  = book.title,
                    Quantity   = qty,
                    UnitPrice  = book.unit_price,
                    TotalPrice = qty * book.unit_price
                });
            }
            UpdateTotal();
        }

        private void BtnRemoveItem_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is SaleItem item)
                _items.Remove(item);
        }

        private void UpdateTotal()
        {
            decimal total = _items.Sum(i => i.TotalPrice);
            TxtTotal.Text = $"Итого: {total} руб.";
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (_items.Count == 0)
            { MessageBox.Show("Добавьте хотя бы одну позицию."); return; }

            string paymentRaw = (CmbPayment.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Наличные";
            string payment = paymentRaw == "Наличные" ? "cash" : "card";

            using var db = new AppDbContext();
            var transaction = new Transaction
            {
                ID_user      = _user.ID_user,
                sale_date    = DateTime.Now,
                payment_type = payment
            };
            db.Transactions.Add(transaction);
            db.SaveChanges();

            foreach (var item in _items)
            {
                db.BookTransactions.Add(new BookTransaction
                {
                    ID_book        = item.ID_book,
                    ID_transaction = transaction.ID_transaction,
                    quantity       = item.Quantity,
                    total_price    = item.TotalPrice
                });
                var book = db.Books.Find(item.ID_book)!;
                book.stock_quantity -= item.Quantity;
            }
            db.SaveChanges();

            DialogResult = true;
        }
    }

    public class SaleItem
    {
        public int     ID_book    { get; set; }
        public string  BookTitle  { get; set; } = "";
        public int     Quantity   { get; set; }
        public decimal UnitPrice  { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
