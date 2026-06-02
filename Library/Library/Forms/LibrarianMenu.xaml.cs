using Library.DB;
using Library.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;

namespace Library.Forms
{
    /// <summary>
    /// Interaction logic for LibrarianMenu.xaml
    /// </summary>
    public partial class LibrarianMenu : Window
    {
        string username;

        UserDAO userDAO = new UserDAO();
        BookDAO bookDAO = new BookDAO();
        LoanDAO loanDAO = new LoanDAO();

        public LibrarianMenu(string username)
        {
            InitializeComponent();
            welcomeText.Content += username;
            this.username = username;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
            this.Close();
        }

        private void changePasswordBtn_Click(object sender, RoutedEventArgs e)
        {
            ChangePassword changepass = new ChangePassword(username);
            changepass.Show();
            this.Close();
        }

        private void BookCatalog(object sender, RoutedEventArgs e)
        {
            BookCatalogLibrarian bookCatalog = new BookCatalogLibrarian(username);
            bookCatalog.Show();
            this.Close();
        }

        private void UserList(object sender, RoutedEventArgs e)
        {
            UserList userList = new UserList(username);
            userList.Show();
            this.Close();
        }

        private void exportDataBtn_Click(object sender, RoutedEventArgs e)
        {
            if (cbExpLoans.IsChecked == false && cbExpBooks.IsChecked == false && cbExpUsers.IsChecked == false)
            {
                MessageBox.Show("Please select at least one category to export.", "Export Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "JSON Files (*.json)|*.json",
                FileName = "LibraryBackup.json"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    var exportContainer = new Dictionary<string, object>();
                    var jsonOptions = new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        ReferenceHandler = ReferenceHandler.IgnoreCycles
                    };

                    if (cbExpUsers.IsChecked == true) exportContainer["Users"] = userDAO.getAllUsersRaw();
                    if (cbExpBooks.IsChecked == true) exportContainer["Books"] = bookDAO.getAllBooksRaw();
                    if (cbExpLoans.IsChecked == true) exportContainer["Loans"] = loanDAO.getAllLoansRaw();

                    string jsonString = JsonSerializer.Serialize(exportContainer, jsonOptions);
                    File.WriteAllText(saveFileDialog.FileName, jsonString);

                    MessageBox.Show("Data exported successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Export failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void importDataBtn_Click(object sender, RoutedEventArgs e)
        {
            if (cbImpLoans.IsChecked == false && cbImpBooks.IsChecked == false && cbImpUsers.IsChecked == false)
            {
                MessageBox.Show("Please select at least one category to import.", "Import Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBoxResult confirm = MessageBox.Show(
                "Importing data will completely overwrite selected existing tables in the database. Are you sure you want to proceed?",
                "Confirm Overwrite", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirm != MessageBoxResult.Yes) return;

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "JSON Files (*.json)|*.json"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string jsonString = File.ReadAllText(openFileDialog.FileName);
                    using JsonDocument doc = JsonDocument.Parse(jsonString);
                    var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                    using (DBConnection db = new DBConnection())
                    {
                        using (var transaction = db.Database.BeginTransaction())
                        {
                            if (cbImpLoans.IsChecked == true)
                            {
                                db.Loan.RemoveRange(db.Loan);
                                db.SaveChanges();
                            }
                            if (cbImpBooks.IsChecked == true)
                            {
                                db.Book.RemoveRange(db.Book);
                                db.SaveChanges();
                            }
                            if (cbImpUsers.IsChecked == true)
                            {
                                db.Users.RemoveRange(db.Users);
                                db.SaveChanges();
                            }

                            if (cbImpUsers.IsChecked == true && doc.RootElement.TryGetProperty("Users", out JsonElement usersElement))
                            {
                                var users = JsonSerializer.Deserialize<List<User>>(usersElement.GetRawText(), jsonOptions);
                                foreach (var u in users) { u.UserRole = null; u.Loans = new List<Loan>(); }

                                db.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Users ON");
                                db.Users.AddRange(users);
                                db.SaveChanges();
                                db.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Users OFF");
                            }

                            if (cbImpBooks.IsChecked == true && doc.RootElement.TryGetProperty("Books", out JsonElement booksElement))
                            {
                                var books = JsonSerializer.Deserialize<List<Book>>(booksElement.GetRawText(), jsonOptions);
                                foreach (var b in books) { b.Author = null; b.Category = null; b.Loans = new List<Loan>(); }

                                db.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Book ON");
                                db.Book.AddRange(books);
                                db.SaveChanges();
                                db.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Book OFF");
                            }

                            if (cbImpLoans.IsChecked == true && doc.RootElement.TryGetProperty("Loans", out JsonElement loansElement))
                            {
                                var loans = JsonSerializer.Deserialize<List<Loan>>(loansElement.GetRawText(), jsonOptions);
                                foreach (var l in loans) { l.Book = null; l.User = null; }

                                db.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Loan ON");
                                db.Loan.AddRange(loans);
                                db.SaveChanges();
                                db.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Loan OFF");
                            }

                            transaction.Commit();
                        }
                    }

                    MessageBox.Show("Data imported and database updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    string errorMsg = ex.Message;
                    if (ex.InnerException != null)
                    {
                        errorMsg += $"\nDetails: {ex.InnerException.Message}";
                    }
                    MessageBox.Show($"Import failed: {errorMsg}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}