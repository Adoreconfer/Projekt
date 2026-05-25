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
        void changePassword(string username, string newPassword);
        User getUserByUsername(string username);
    }
}
