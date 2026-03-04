using System.ComponentModel.DataAnnotations;

namespace BookshopApp.Models
{
    public class AuthorBook
    {
        [Key]
        public int ID_author_book { get; set; }

        public int ID_author { get; set; }
        public Author Author { get; set; } = null!;

        public int ID_book { get; set; }
        public Book Book { get; set; } = null!;
    }
}
