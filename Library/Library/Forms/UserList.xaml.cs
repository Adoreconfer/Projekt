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
    /// Interaction logic for UserList.xaml
    /// </summary>
    public partial class UserList : Window
    {
        string username;
        UserDAO userDAO;
        public UserList(string username)
        {
            InitializeComponent();
            this.username = username;
            userDAO = new UserDAO();
            LoadAllUsers();
        }

        private void LoadAllUsers()
        {
            try
            {
                List<User> allUsers = userDAO.searchUser("", "", "", "All");
                UsersGrid.ItemsSource = allUsers;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load users: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            UserDAO user = new UserDAO();
            string role = user.getUserByUsername(username).Role;
            if (role == "user")
            {
                UserMenu menu = new UserMenu(username);
                menu.Show();
            }
            if (role == "librarian")
            {
                LibrarianMenu menulib = new LibrarianMenu(username);
                menulib.Show();
            }

            this.Close();
        }

        private void editUserBtn_Click(object sender, RoutedEventArgs e)
        {
            if (UsersGrid.SelectedItem is User selectedUser)
            {
                //ManageUser manageWindow = new ManageUser(username, "Edit", selectedUser);
                //manageWindow.Show();
                //this.Close();
            }
            else
            {
                MessageBox.Show("Please select a user from the list to edit.", "Selection Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void moreInfoBtn_Click(object sender, RoutedEventArgs e)
        {
            if (UsersGrid.SelectedItem is User selectedUser)
            {
                string info = $"User Information:\n\n" +
                              $"ID: {selectedUser.IdUser}\n" +
                              $"First Name: {selectedUser.FirstName}\n" +
                              $"Last Name: {selectedUser.LastName}\n" +
                              $"Username: {selectedUser.Username}\n" +
                              $"Account Role: {selectedUser.Role}";

                MessageBox.Show(info, "More Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Please select a user from the list to view information.", "Selection Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void addUserBtn_Click(object sender, RoutedEventArgs e)
        {
            //ManageUser manageWindow = new ManageUser(username, "Add", null);
            //manageWindow.Show();
            //this.Close();
        }

        private void searchBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string selectedRole = "All";
                if (roleComboBox.SelectedItem is ComboBoxItem item)
                {
                    selectedRole = item.Content.ToString();
                }

                string search = searchBox.Text.Trim();

                if (rbAll.IsChecked == false && string.IsNullOrWhiteSpace(search))
                {
                    MessageBox.Show(
                        "Please enter text to search or select 'All' to view everyone.",
                        "Information",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );
                    return;
                }

                List<User> filteredUsers = new List<User>();

                if (rbFirstName.IsChecked == true)
                {
                    filteredUsers = userDAO.searchUser(search, "", "", selectedRole);
                }
                else if (rbLastName.IsChecked == true)
                {
                    filteredUsers = userDAO.searchUser("", search, "", selectedRole);
                }
                else if (rbUsername.IsChecked == true)
                {
                    filteredUsers = userDAO.searchUser("", "", search, selectedRole);
                }
                else if (rbAll.IsChecked == true)
                {
                    filteredUsers = userDAO.searchUser("", "", "", selectedRole);
                }

                UsersGrid.ItemsSource = filteredUsers;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Database error: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }
    }
}
