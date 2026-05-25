using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Models
{
    public class Category
    {
        public int IdCategory { get; set; }
        public string Name { get; set; }
        public ICollection<Book> Books { get; set; }
    }
}
