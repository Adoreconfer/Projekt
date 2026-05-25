using Library.DB;
using Library.Models;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Library.Forms
{
    public partial class ManageBook : Window
    {
        Book book;
        string username;
        BookDAO bookDAO;

        public ManageBook(string username, string title, Book book)
        {
            InitializeComponent();

            this.book = book;
            this.username = username;

            bookDAO = new BookDAO();
            LoadData();

            Title = title + " book";

            if (title == "Add")
            {
                manageBtn.Content = "Add book";
            }
            if (title == "Edit")
            {
                manageBtn.Content = "Edit book";

                titleBox.Text = book.Title;

                if (book.Author != null)
                {
                    authorBox.Text = $"{book.Author.FirstName} {book.Author.LastName}";
                }

                if (book.Category != null)
                {
                    categoryBox.Text = book.Category.Name;
                    if (categoryComboBox.Items.Contains(book.Category.Name))
                    {
                        categoryComboBox.SelectedItem = book.Category.Name;
                    }
                }

                isbnBox.Text = book.ISBN;
                yearBox.Text = book.Year.ToString();
                copiesBox.Text = book.TotalCopies.ToString();
            }
        }

        private void LoadData()
        {
            List<string> category = bookDAO.getAllCategory();
            category.Insert(0, "New");
            categoryComboBox.ItemsSource = category;

            if (category.Count > 0)
            {
                categoryComboBox.SelectedIndex = 0;
            }
        }

        private void manageBtn_Click(object sender, RoutedEventArgs e)
        {
            string authorFullName = authorBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(authorFullName))
            {
                MessageBox.Show("Please enter the author's name.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string firstName = "";
            string lastName = authorFullName;
            int lastSpaceIndex = authorFullName.LastIndexOf(' ');
            if (lastSpaceIndex > 0)
            {
                firstName = authorFullName.Substring(0, lastSpaceIndex).Trim();
                lastName = authorFullName.Substring(lastSpaceIndex + 1).Trim();
            }

 
            string finalCategoryName = "";

            if (categoryComboBox.SelectedIndex == 0) 
            {
                finalCategoryName = categoryBox.Text.Trim();

                if (string.IsNullOrWhiteSpace(finalCategoryName))
                {
                    MessageBox.Show("Please enter the new category name in the text field.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (bookDAO.categoryExists(finalCategoryName))
                {
                    MessageBox.Show($"The category \"{finalCategoryName}\" already exists. The system will automatically use the existing record.", "Category Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                finalCategoryName = categoryComboBox.SelectedItem.ToString();
            }

            if (string.IsNullOrWhiteSpace(titleBox.Text) || string.IsNullOrWhiteSpace(isbnBox.Text) ||
                string.IsNullOrWhiteSpace(yearBox.Text) || string.IsNullOrWhiteSpace(copiesBox.Text))
            {
                MessageBox.Show("Please fill in all fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string isbn = isbnBox.Text.Trim();

            if (isbn.Length != 13)
            {
                MessageBox.Show($"The ISBN must be exactly 13 digits long. Current length: {isbn.Length} characters.", "Invalid ISBN", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!long.TryParse(isbn, out _))
            {
                MessageBox.Show("The ISBN must contain numbers only.", "Invalid ISBN", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(yearBox.Text, out int year) || !int.TryParse(copiesBox.Text, out int totalCopies))
            {
                MessageBox.Show("Year and Copies must be valid numbers.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                string currentAction = manageBtn.Content.ToString();

                if (currentAction == "Add book")
                {
                    bookDAO.addBook(titleBox.Text.Trim(), authorFullName, finalCategoryName, isbn, year, totalCopies);
                    MessageBox.Show("The book has been successfully added to the library!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (currentAction == "Edit book")
                {
                    bookDAO.editBook(book.IdBook, titleBox.Text.Trim(), authorFullName, finalCategoryName, isbn, year, totalCopies);
                    MessageBox.Show("The book has been successfully updated!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                BookCatalogLibrarian menu = new BookCatalogLibrarian(username);
                menu.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Operation failed: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            BookCatalogLibrarian menu = new BookCatalogLibrarian(username);
            menu.Show();
            this.Close();
        }
    }
}