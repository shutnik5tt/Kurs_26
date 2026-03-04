using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookshopApp.Models
{
    public class BookTransaction
    {
        [Key]
        public int ID_book_transaction { get; set; }

        public int ID_book { get; set; }
        public Book Book { get; set; } = null!;

        public int ID_transaction { get; set; }
        public Transaction Transaction { get; set; } = null!;

        public int quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal total_price { get; set; }
    }
}
