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
    /// Interaction logic for Register.xaml
    /// </summary>
    public partial class Register : Window
    {
        public Register()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string username = usernameBox.Text.Trim();
            string firstname = firstnameBox.Text.Trim();
            string lastname = lastnameBox.Text.Trim();
            string password = passwordBox.Password.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(firstname) 
                || string.IsNullOrEmpty(lastname) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show(
                    "Enter your username and password",
                    "No data available",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );

                usernameBox.Clear();
                passwordBox.Clear();
                usernameBox.Focus();

                return;
            }

            UserDAO user = new UserDAO();

            try
            {
                user.addUser(username, password, firstname, lastname, "user");
                MainWindow main = new MainWindow();
                main.Show();

                this.Close();

                MessageBox.Show(
                    "Account has been created",
                    "Registration",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Info",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );

                firstnameBox.Clear();
                lastnameBox.Clear();
                usernameBox.Clear();
                passwordBox.Clear();
                firstnameBox.Focus();

                return;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();

            this.Close();
        }
    }
}
