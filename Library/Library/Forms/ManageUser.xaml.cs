using Library.DB;
using Library.Models;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Library.Forms
{
    /// <summary>
    /// Interaction logic for ManageUser.xaml
    /// </summary>
    public partial class ManageUser : Window
    {
        User user;
        string username;
        UserDAO userDAO;

        public ManageUser(string username, string title, User user)
        {
            InitializeComponent();

            this.user = user;
            this.username = username;

            userDAO = new UserDAO();
            LoadData();

            Title = title + " user";

            if (title == "Add")
            {
                manageBtn.Content = "Add user";

                passwordBox.Visibility = Visibility.Visible;
                compasswordBox.Visibility = Visibility.Visible;
            }
            else if (title == "Edit")
            {
                manageBtn.Content = "Edit user";

                firstnameBox.Text = user.FirstName;
                lastnameBox.Text = user.LastName;
                usernameBox.Text = user.Username;

                usernameBox.IsEnabled = true;

                passwordBox.Visibility = Visibility.Collapsed;
                compasswordBox.Visibility = Visibility.Collapsed;

                if (FindName("passwordLabel") is Label passLabel) passLabel.Visibility = Visibility.Collapsed;
                if (FindName("confirmPasswordLabel") is Label confirmLabel) confirmLabel.Visibility = Visibility.Collapsed;

                foreach (ComboBoxItem item in roleComboBox.Items)
                {
                    if (item.Content.ToString().Equals(user.Role, StringComparison.OrdinalIgnoreCase))
                    {
                        roleComboBox.SelectedItem = item;
                        break;
                    }
                }
            }
        }

        private void LoadData()
        {
            if (roleComboBox.SelectedIndex == -1 && roleComboBox.Items.Count > 0)
            {
                roleComboBox.SelectedIndex = 0;
            }
        }

        private void manageBtn_Click(object sender, RoutedEventArgs e)
        {
            string currentAction = manageBtn.Content.ToString();

            if (string.IsNullOrWhiteSpace(firstnameBox.Text) ||
                string.IsNullOrWhiteSpace(lastnameBox.Text) ||
                string.IsNullOrWhiteSpace(usernameBox.Text))
            {
                MessageBox.Show("Please fill in all basic fields (First name, Last name, Username).", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string firstName = firstnameBox.Text.Trim();
            string lastName = lastnameBox.Text.Trim();
            string targetUsername = usernameBox.Text.Trim();

            string password = "";
            string confirmPassword = "";

            if (currentAction == "Add user")
            {
                if (string.IsNullOrWhiteSpace(passwordBox.Password) || string.IsNullOrWhiteSpace(compasswordBox.Password))
                {
                    MessageBox.Show("Please fill in password fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                password = passwordBox.Password.Trim();
                confirmPassword = compasswordBox.Password.Trim();

                if (password != confirmPassword)
                {
                    MessageBox.Show("Passwords do not match. Please re-enter your password.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (password.Length < 4)
                {
                    MessageBox.Show("Password must be at least 4 characters long.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            string selectedRole = "reader";
            if (roleComboBox.SelectedItem is ComboBoxItem item)
            {
                selectedRole = item.Content.ToString().Trim().ToLower();
            }

            try
            {
                if (currentAction == "Add user")
                {
                    if (userDAO.getUserByUsername(targetUsername) != null)
                    {
                        MessageBox.Show($"Username '{targetUsername}' is already taken. Please choose another one.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    userDAO.addUser(targetUsername, password, firstName, lastName, selectedRole);
                    MessageBox.Show("The user account has been successfully created!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (currentAction == "Edit user")
                {
                    userDAO.editUser(user.IdUser, targetUsername, firstName, lastName, user.Password, selectedRole);
                    MessageBox.Show("The user account has been successfully updated!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                UserList listWindow = new UserList(username);
                listWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Operation failed: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            UserList listWindow = new UserList(username);
            listWindow.Show();
            this.Close();
        }
    }
}