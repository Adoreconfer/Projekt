using Library.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library.DB
{
    internal class LoanDAO : ILoanDAO
    {
        public void addLoan(User user, Book book)
        {
            using DBConnection db = new DBConnection();

            db.Book.Attach(book);

            if (book.AvailableCopies == 0) {
                return;
            }

            book.AvailableCopies--;

            Loan loan = new Loan
            {
                IdUser = user.IdUser,
                IdBook = book.IdBook,
                LoanDate = DateTime.Today,
                DueDate = DateTime.Today.AddDays(14),
                ReturnDate = null,
                Fine = 0.00m
            };

            db.Loan.Add(loan);
            db.SaveChanges();
        }

        public List<Loan> getUserLoans(string username) {
            using DBConnection db = new DBConnection();

            return db.Loan
                .Include(l => l.Book)
                    .ThenInclude(b => b.Author)
                .Include(l => l.User)
                .Where(l => l.User.Username == username)
                .ToList();
        }
    }
}
