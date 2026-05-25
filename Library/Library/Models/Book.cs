using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Models
{
    public class Book
    {
        public int IdBook { get; set; }
        public string Title { get; set; }
        public int IdAuthor { get; set; }
        public int? IdCategory { get; set; }
        public string ISBN { get; set; }
        public int TotalCopies { get; set; }
        public int AvailableCopies { get; set; }
        public int Year { get; set; }
        public virtual Author Author { get; set; }
        public virtual Category Category { get; set; }
        public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>();
    }
}
