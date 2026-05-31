using Library.DB;
using Library.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Library.Forms
{
    /// <summary>
    /// Interaction logic for BookCatalog.xaml
    /// </summary>
    public partial class BookCatalog : Window
    {
        string username;
        LoanDAO loanDAO = new LoanDAO();
        UserDAO userDAO = new UserDAO();
        BookDAO bookDAO = new BookDAO();
        public BookCatalog(string username)
        {
            InitializeComponent();
            this.username = username; 

            LoadData();
        }

        private void LoadData()
        {

            List<string> category = bookDAO.getAllCategory();
            category.Insert(0, "All");
            classComboBox.ItemsSource = category;

            if (category.Count > 0)
            {
                classComboBox.SelectedIndex = 0;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            UserMenu menu = new UserMenu(username);
            menu.Show();

            this.Close();
        }

        private void searchBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string category = classComboBox.SelectedItem as string;
                string search = searchBox.Text;

                if (classComboBox.SelectedIndex == 0)
                {
                    category = "";
                }

                if (string.IsNullOrWhiteSpace(search) && allRadio.IsChecked == false)
                {
                    MessageBox.Show(
                        "Select 'All' or enter text to search",
                        "Information",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );
                }
                else
                {
                    List<Book> filteredBooks = new List<Book>();

                    if (AuthorRadio.IsChecked == true)
                    {
                        filteredBooks = bookDAO.searchBook("", search, category);
                    }
                    else if (TitleRadio.IsChecked == true)
                    {
                        filteredBooks = bookDAO.searchBook(search, "", category);
                    }
                    else if (allRadio.IsChecked == true)
                    {
                        filteredBooks = bookDAO.searchBook(search, search, category);
                    }

                    BooksGrid.ItemsSource = filteredBooks;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Database error: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        private void loanBtn_Click(object sender, RoutedEventArgs e)
        {
            if (BooksGrid.SelectedItem == null)
            {
                MessageBox.Show("Please select a book from the table first!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Book selectedBook = (Book)BooksGrid.SelectedItem;
            try 
            { 
                User user = userDAO.getUserByUsername(username);
                Book book = bookDAO.getBookByISBN(selectedBook.ISBN);

                if (book.AvailableCopies == 0)
                {
                    MessageBox.Show($"The book \"{selectedBook.Title}\" is currently unavailable for loan.", "Loan Book Info", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                loanDAO.addLoan(user, book);

                searchBtn_Click(sender, e);
                MessageBox.Show($"The book \"{selectedBook.Title}\" has been successfully loaned.", "Loan Book Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during loan process: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
