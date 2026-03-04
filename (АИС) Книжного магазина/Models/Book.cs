using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookshopApp.Models
{
    public class Book
    {
        [Key]
        public int ID_book { get; set; }

        [Required, MaxLength(300)]
        public string title { get; set; } = "";

        [MaxLength(2000)]
        public string? description { get; set; }

        public DateTime release_date { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal unit_price { get; set; }

        public int stock_quantity { get; set; }

        public ICollection<AuthorBook> AuthorBooks { get; set; } = new List<AuthorBook>();
        public ICollection<BookTransaction> BookTransactions { get; set; } = new List<BookTransaction>();

        public override string ToString() => title;
    }
}
