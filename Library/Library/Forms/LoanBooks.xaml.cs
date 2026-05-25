using Library.DB;
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
        public LoanBooks(string username)
        {
            InitializeComponent();
            this.username = username;

            LoanDAO loan = new LoanDAO();

            LoansGrid.ItemsSource = loan.getUserLoans(username);
        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            UserMenu menu = new UserMenu(username);
            menu.Show();

            this.Close();
        }
    }
}
