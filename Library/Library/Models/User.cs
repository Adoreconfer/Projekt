using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Models
{
    public class User
    {
        public int IdUser { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Role { get; set; }

        public virtual UserRole UserRole { get; set; }
        public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>();
    }
}
