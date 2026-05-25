using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Models
{
    public class Author
    {
        public int IdAuthor { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public virtual ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
