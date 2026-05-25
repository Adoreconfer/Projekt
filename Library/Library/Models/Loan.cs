using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Models
{
    public class Loan
    {
        public int IdLoan { get; set; }
        public int IdBook { get; set; }
        public int IdUser { get; set; }
        public DateTime LoanDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public decimal Fine { get; set; }

        public virtual Book Book { get; set; }
        public virtual User User { get; set; }
    }
}
