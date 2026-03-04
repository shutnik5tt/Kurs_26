using BookshopApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BookshopApp.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<AuthorBook> AuthorBooks { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<BookTransaction> BookTransactions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                @"Server=localhost\SQLEXPRESS;Database=BookshopDB;Trusted_Connection=True;TrustServerCertificate=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // AuthorBook связи
            modelBuilder.Entity<AuthorBook>()
                .HasOne(ab => ab.Author)
                .WithMany(a => a.AuthorBooks)
                .HasForeignKey(ab => ab.ID_author);

            modelBuilder.Entity<AuthorBook>()
                .HasOne(ab => ab.Book)
                .WithMany(b => b.AuthorBooks)
                .HasForeignKey(ab => ab.ID_book);

            // BookTransaction связи
            modelBuilder.Entity<BookTransaction>()
                .HasOne(bt => bt.Book)
                .WithMany(b => b.BookTransactions)
                .HasForeignKey(bt => bt.ID_book);

            modelBuilder.Entity<BookTransaction>()
                .HasOne(bt => bt.Transaction)
                .WithMany(t => t.BookTransactions)
                .HasForeignKey(bt => bt.ID_transaction);

            // Transaction -> User
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.User)
                .WithMany(u => u.Transactions)
                .HasForeignKey(t => t.ID_user);

            // Seed: admin пользователь
            modelBuilder.Entity<User>().HasData(new User
            {
                ID_user = 1,
                login = "admin",
                password = "admin123",
                role = "admin"
            });
        }
    }
}
