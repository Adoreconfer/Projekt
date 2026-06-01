using Library.DB;
using Library.Models;
using System;
using System.Windows;

namespace Library.Forms
{
    /// <summary>
    /// Interaction logic for ReturnBook.xaml
    /// </summary>
    public partial class ReturnBook : Window
    {
        string username;
        LoanDAO loanDAO; 

        public ReturnBook(string username)
        {
            InitializeComponent();
            this.username = username;

            loanDAO = new LoanDAO();
            RefreshLoansGrid();
        }

        private void RefreshLoansGrid()
        {
            try
            {
                List<Loan> loans = loanDAO.getUserLoans(username);
                LoansGrid.ItemsSource = loans;
                loanDAO.calculateFine(loans);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load loans: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            UserList userList = new UserList(username);
            userList.Show();
            this.Close();
        }

        private void returnBookBtn_Click(object sender, RoutedEventArgs e)
        {
            if (LoansGrid.SelectedItem is Loan selectedLoan)
            {
                try
                {
                    loanDAO.returnLoan(selectedLoan.IdLoan);

                    MessageBox.Show("The book has been successfully returned to the library.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    RefreshLoansGrid();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Operation failed: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a book from the list to return.", "Selection Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}