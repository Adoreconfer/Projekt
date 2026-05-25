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
    /// Interaction logic for UserMenu.xaml
    /// </summary>
    public partial class UserMenu : Window
    {
        string username;
        public UserMenu(string username)
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

        private void BookCatalog(object sender, RoutedEventArgs e)
        {
            BookCatalog bookCatalog = new BookCatalog(username);
            bookCatalog.Show();
            this.Close();
        }

        private void changePasswordBtn_Click(object sender, RoutedEventArgs e)
        {
            ChangePassword changepass = new ChangePassword(username);
            changepass.Show();
            this.Close();
        }

        private void bookLoanBtn_Click(object sender, RoutedEventArgs e)
        {
            LoanBooks loanBooks = new LoanBooks(username);
            loanBooks.Show();
            this.Close();
        }
    }
}
