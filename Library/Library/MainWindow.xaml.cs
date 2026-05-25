using Library.DB;
using Library.Forms;
using Library.Models;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Library
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string username = usernameBox.Text.Trim();
            string password = passwordBox.Password.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show(
                    "Enter your username and password",
                    "No data available",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );

                usernameBox.Clear();
                passwordBox.Clear();
                usernameBox.Focus();

                return;
            }

            UserDAO userDAO = new UserDAO();

            User authenticatedUser = userDAO.authenticateUser(username, password);

            if (authenticatedUser != null)
            {
                switch (authenticatedUser.Role.ToLower())
                {
                    case "user":
                        new UserMenu(username).Show();
                        break;
                    case "librarian":
                        new LibrarianMenu(username).Show();
                        break;
                    default:
                        MessageBox.Show("Unknown role assigned to this user.");
                        return;
                }

                this.Close(); 
            }
            else
            {
                MessageBox.Show(
                    "Incorrect login or password",
                    "Login error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Register register = new Register();
            register.Show();

            this.Close();
        }
    }
}