using Library.DB;
using Library.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

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

        private void editUserBtn_Click(object sender, RoutedEventArgs e)
        {
            if (UsersGrid.SelectedItem is User selectedUser)
            {
                ManageUser manageWindow = new ManageUser(username, "Edit", selectedUser);
                manageWindow.Show();
                this.Close();
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
                ReturnBook returnBook = new ReturnBook(selectedUser.Username);
                returnBook.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Please select a user from the list to view information.", "Selection Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void addUserBtn_Click(object sender, RoutedEventArgs e)
        {
            ManageUser manageWindow = new ManageUser(username, "Add", null);
            manageWindow.Show();
            this.Close();
        }

        private void deleteUserBtn_Click(object sender, RoutedEventArgs e)
        {
            if (UsersGrid.SelectedItem is User selectedUser)
            {
                MessageBoxResult result = MessageBox.Show(
                    $"Are you sure you want to permanently delete user '{selectedUser.Username}' (ID: {selectedUser.IdUser})?",
                    "Confirm Deletion",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        userDAO.deleteUser(selectedUser.IdUser);

                        MessageBox.Show("The user has been successfully deleted.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                        searchBtn_Click(sender, e);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to delete user: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a user from the list to delete.", "Selection Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
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