using System.ComponentModel.DataAnnotations;

namespace BookshopApp.Models
{
    public class User
    {
        [Key]
        public int ID_user { get; set; }

        [Required, MaxLength(100)]
        public string login { get; set; } = "";

        [Required, MaxLength(256)]
        public string password { get; set; } = "";

        [Required, MaxLength(50)]
        public string role { get; set; } = "cashier"; // "admin" or "cashier"

        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

        public override string ToString() => $"{login} ({role})";
    }
}
