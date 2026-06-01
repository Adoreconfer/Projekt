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
    /// Interaction logic for ChangePassword.xaml
    /// </summary>
    public partial class ChangePassword : Window
    {
        string username;
        public ChangePassword(string username)
        {
            InitializeComponent();
            this.username = username;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            UserDAO user = new UserDAO();
            string role = user.getUserByUsername(username).Role;
            if (role == "reader")
            {
                UserMenu menu = new UserMenu(username);
                menu.Show();
            }
            if(role == "librarian") { 
                LibrarianMenu menulib = new LibrarianMenu(username);
                menulib.Show();
            }

            this.Close();
        }

        private void ChangeBtn(object sender, RoutedEventArgs e)
        {
            string password = passBox.Password.Trim();
            string changepass = changepassBox.Password.Trim();

            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(changepass))
            {
                MessageBox.Show(
                    "Enter your username and password",
                    "No data available",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );

                return;
            }

            if (!password.Equals(changepass)) 
            {
                MessageBox.Show(
                    "The passwords you entered do not match. Please try again.",
                    "Password Mismatch",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );

                passBox.Clear();
                changepassBox.Clear();
                passBox.Focus();

                return;
            }

            UserDAO user = new UserDAO();

            try
            {
                user.changePassword(username, password);

                MessageBox.Show(
                     "Your password has been successfully changed.",
                     "Success",
                     MessageBoxButton.OK,
                     MessageBoxImage.Information
                 );

                passBox.Clear();
                changepassBox.Clear();
                passBox.Focus();

            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Info",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );

                passBox.Clear();
                changepassBox.Clear();
                passBox.Focus();

                return;
            }
        }
    }
}
