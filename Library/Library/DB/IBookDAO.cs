using Library.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library.DB
{
    interface IBookDAO
    {
        List<Book> searchBook(string title, string author, string category);
        List<string> getAllCategory();
        Book getBookByISBN(string isbn);
        bool deleteBook(string isbn);
        void addBook(string title, string authorFullName, string categoryName, string isbn, int year, int total);
        void editBook(int idBook, string title, string authorFullName, string categoryName, string isbn, int year, int total);
        bool categoryExists(string categoryName);
        Category getCategoryByName(string categoryName);
    }
}
