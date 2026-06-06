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
    /// Interaction logic for LoanBooks.xaml
    /// </summary>
    public partial class LoanBooks : Window
    {
        string username;
        UserDAO userDAO;
        public LoanBooks(string username)
        {
            InitializeComponent();
            this.username = username;

            LoanDAO loanDAO = new LoanDAO();
            
            List<Loan> loans = loanDAO.getUserLoans(username);
            LoansGrid.ItemsSource = loans;
            loanDAO.calculateFine(loans);
        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                User targetUser = userDAO.getUserByUsername(username);
                if (targetUser == null)
                {
                    MessageBox.Show("Active session user not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string role = targetUser.Role;

                if (role.Equals("reader", StringComparison.OrdinalIgnoreCase))
                {
                    UserMenu menu = new UserMenu(username);
                    menu.Show();
                }
                else if (role.Equals("librarian", StringComparison.OrdinalIgnoreCase))
                {
                    LibrarianMenu menulib = new LibrarianMenu(username);
                    menulib.Show();
                }

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Navigation error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
