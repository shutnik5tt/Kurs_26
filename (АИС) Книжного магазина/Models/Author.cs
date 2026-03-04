using System.ComponentModel.DataAnnotations;

namespace BookshopApp.Models
{
    public class Author
    {
        [Key]
        public int ID_author { get; set; }

        [Required, MaxLength(100)]
        public string last_name { get; set; } = "";

        [Required, MaxLength(100)]
        public string first_name { get; set; } = "";

        [MaxLength(100)]
        public string? middle_name { get; set; }

        public DateTime birth_date { get; set; }

        public ICollection<AuthorBook> AuthorBooks { get; set; } = new List<AuthorBook>();

        public string FullName =>
            string.IsNullOrWhiteSpace(middle_name)
                ? $"{last_name} {first_name}"
                : $"{last_name} {first_name} {middle_name}";

        public override string ToString() => FullName;
    }
}
