using System.ComponentModel.DataAnnotations;

namespace BookshopApp.Models
{
    public class Transaction
    {
        [Key]
        public int ID_transaction { get; set; }

        public int ID_user { get; set; }
        public User User { get; set; } = null!;

        public DateTime sale_date { get; set; } = DateTime.Now;

        [Required, MaxLength(50)]
        public string payment_type { get; set; } = ""; // "cash", "card"

        public ICollection<BookTransaction> BookTransactions { get; set; } = new List<BookTransaction>();
    }
}
