using System;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Library.Models;

namespace Library.DB
{
    interface IUserDAO
    {
        User authenticateUser(string username, string password);
        void addUser(string username, string password, string firstname, string lastname, string role);
        void editUser(int id, string username, string firstname, string lastname, string password, string role);
        void deleteUser(int id);
        void changePassword(string username, string newPassword);
        User getUserByUsername(string username);
        List<User> searchUser(string first_name, string last_name, string username, string role);
        List<User> getAllUsersRaw();
        void importUsersRaw(List<User> users);
    }
}
