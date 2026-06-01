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

        public void calculateFine(List<Loan> loans)
        {
            if (loans == null || loans.Count == 0) return;

            const decimal DailyFineAmount = 0.50m;

            DateTime today = DateTime.Today;

            using (DBConnection db = new DBConnection())
            {
                foreach (var loan in loans)
                {
                    DateTime endDate = loan.ReturnDate ?? today;

                    if (endDate > loan.DueDate)
                    {
                        int overdueDays = (endDate - loan.DueDate).Days;

                        decimal calculatedFine = overdueDays * DailyFineAmount;

                        db.Attach(loan);
                        loan.Fine = calculatedFine;
                    }
                    else
                    {
                        db.Attach(loan);
                        loan.Fine = 0.00m;
                    }
                }

                db.SaveChanges();
            }
        }

        public void returnLoan(int idLoan)
        {
            using (DBConnection db = new DBConnection())
            {
                var loan = db.Loan
                             .Include(l => l.Book)
                             .FirstOrDefault(l => l.IdLoan == idLoan);

                if (loan == null)
                {
                    throw new Exception("Loan record not found.");
                }

                if (loan.ReturnDate != null)
                {
                    throw new Exception("This book has already been returned.");
                }

                loan.ReturnDate = DateTime.Today;

                if (loan.Book != null)
                {
                    if (loan.Book.AvailableCopies < loan.Book.TotalCopies)
                    {
                        loan.Book.AvailableCopies += 1;
                    }
                }

                db.SaveChanges();
            }
        }
    }
}
