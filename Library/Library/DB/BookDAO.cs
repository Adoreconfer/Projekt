using Library.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library.DB
{
    class BookDAO : IBookDAO
    {
        public List<string> getAllCategory()
        {
            using (DBConnection db = new DBConnection())
            {
                return db.Category
                         .Select(c => c.Name)
                         .ToList();
            }
        }


        public List<Book> searchBook(string title, string author, string category)
        {
            using (DBConnection db = new DBConnection())
            {
                var query = db.Book
                              .Include(b => b.Author)
                              .Include(b => b.Category)
                              .AsQueryable();

                if (!string.IsNullOrEmpty(title) && title == author)
                {
                    string search = title.Trim().ToLower();
                    query = query.Where(b => b.Title.ToLower().StartsWith(search) ||
                                             b.Author.FirstName.ToLower().StartsWith(search) ||
                                             b.Author.LastName.ToLower().StartsWith(search));
                }
                else
                {
                    if (!string.IsNullOrEmpty(title))
                    {
                        string searchTitle = title.Trim().ToLower();
                        query = query.Where(b => b.Title.ToLower().StartsWith(searchTitle));
                    }

                    if (!string.IsNullOrEmpty(author))
                    {
                        string searchAuthor = author.Trim().ToLower();
                        query = query.Where(b => b.Author.FirstName.ToLower().StartsWith(searchAuthor) ||
                                                 b.Author.LastName.ToLower().StartsWith(searchAuthor));
                    }
                }

                if (!string.IsNullOrEmpty(category) && !category.Equals("All", StringComparison.OrdinalIgnoreCase))
                {
                    string searchCategory = category.Trim().ToLower();
                    query = query.Where(b => b.Category.Name.ToLower().StartsWith(searchCategory));
                }

                return query.ToList();
            }
        }
        public Book getBookByISBN(string isbn)
        {
            using (DBConnection db = new DBConnection())
            {
                return db.Book
                         .Include(b => b.Author)   
                         .Include(b => b.Category) 
                         .FirstOrDefault(b => b.ISBN == isbn.Trim());
            }
        }

        public bool deleteBook(string isbn) {
            using (DBConnection db = new DBConnection())
            {
                var bookToDelete = db.Book.FirstOrDefault(b => b.ISBN == isbn.Trim());

                 if (bookToDelete != null)
                {
                    db.Book.Remove(bookToDelete);
                    db.SaveChanges(); 
                    return true; 
                }

                return false;
            }
        }

        public void addBook(string title, string authorFullName, string categoryName, string isbn, int year, int total)
        {
            using DBConnection db = new DBConnection();

            bool isbnExists = db.Book.Any(b => b.ISBN == isbn);
            if (isbnExists)
            {
                throw new Exception("A book with this ISBN already exists in the database.");
            }

            string firstName = "";
            string lastName = authorFullName;

            int lastSpaceIndex = authorFullName.Trim().LastIndexOf(' ');
            if (lastSpaceIndex > 0)
            {
                firstName = authorFullName.Substring(0, lastSpaceIndex).Trim();
                lastName = authorFullName.Substring(lastSpaceIndex + 1).Trim();
            }

            Author author = db.Author.FirstOrDefault(a => a.FirstName == firstName && a.LastName == lastName);
            if (author == null)
            {
                author = new Author { FirstName = firstName, LastName = lastName };
                db.Author.Add(author);
                db.SaveChanges(); 
            }

            Category category = db.Category.FirstOrDefault(c => c.Name.ToLower() == categoryName.Trim().ToLower());
            if (category == null && !string.IsNullOrWhiteSpace(categoryName))
            {
                category = new Category { Name = categoryName.Trim() };
                db.Category.Add(category);
                db.SaveChanges(); 
            }

            Book newBook = new Book
            {
                Title = title,
                IdAuthor = author.IdAuthor,
                IdCategory = category?.IdCategory, 
                ISBN = isbn,
                Year = year,
                TotalCopies = total,
                AvailableCopies = total
            };

            db.Book.Add(newBook);
            db.SaveChanges();
        }

        public void editBook(int idBook, string title, string authorFullName, string categoryName, string isbn, int year, int total)
        {
            using DBConnection db = new DBConnection();

            Book existingBook = db.Book.FirstOrDefault(b => b.IdBook == idBook);
            if (existingBook == null)
            {
                throw new Exception("The book you are trying to edit does not exist in the database.");
            }

            bool isbnExists = db.Book.Any(b => b.ISBN == isbn && b.IdBook != idBook);
            if (isbnExists)
            {
                throw new Exception("Another book with this ISBN already exists.");
            }

            string firstName = "";
            string lastName = authorFullName;
            int lastSpaceIndex = authorFullName.Trim().LastIndexOf(' ');
            if (lastSpaceIndex > 0)
            {
                firstName = authorFullName.Substring(0, lastSpaceIndex).Trim();
                lastName = authorFullName.Substring(lastSpaceIndex + 1).Trim();
            }

            Author author = db.Author.FirstOrDefault(a => a.FirstName == firstName && a.LastName == lastName);
            if (author == null)
            {
                author = new Author { FirstName = firstName, LastName = lastName };
                db.Author.Add(author);
                db.SaveChanges(); 
            }

            Category category = db.Category.FirstOrDefault(c => c.Name.ToLower() == categoryName.Trim().ToLower());
            if (category == null && !string.IsNullOrWhiteSpace(categoryName))
            {
                category = new Category { Name = categoryName.Trim() };
                db.Category.Add(category);
                db.SaveChanges(); 
            }

            int copiesDifference = total - existingBook.TotalCopies;
            int newAvailableCopies = existingBook.AvailableCopies + copiesDifference;

            if (newAvailableCopies < 0)
            {
                throw new Exception("Cannot reduce total copies. Some copies are currently loaned out.");
            }

            existingBook.Title = title;
            existingBook.IdAuthor = author.IdAuthor;
            existingBook.IdCategory = category?.IdCategory;
            existingBook.ISBN = isbn;
            existingBook.Year = year;
            existingBook.TotalCopies = total;
            existingBook.AvailableCopies = newAvailableCopies;

            db.SaveChanges();
        }
        public bool categoryExists(string categoryName)
        {
            using DBConnection db = new DBConnection();
            return db.Category.Any(c => c.Name.ToLower() == categoryName.Trim().ToLower());
        }
        public Category getCategoryByName(string categoryName)
        {
            using DBConnection db = new DBConnection();
            return db.Category.FirstOrDefault(c => c.Name.ToLower() == categoryName.Trim().ToLower());
        }
    }

}
