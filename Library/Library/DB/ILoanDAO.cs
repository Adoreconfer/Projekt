using System;
using System.Collections.Generic;
using System.Text;
using Library.Models;

namespace Library.DB
{
    internal interface ILoanDAO
    {
        void addLoan(User user, Book book);
        List<Loan> getUserLoans(string username);
    }
}
