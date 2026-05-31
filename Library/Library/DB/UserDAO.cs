using Library.Models;
using Microsoft.AspNetCore.Identity;
using System.Data;
using System.Linq;

namespace Library.DB
{
    class UserDAO : IUserDAO
    {
        public User authenticateUser(string username, string password)
        {
            using DBConnection db = new DBConnection();

            User user = db.Users.FirstOrDefault(x => x.Username == username);

            if (user == null) return null;

            PasswordHasher<User> hasher = new PasswordHasher<User>();

            var result = hasher.VerifyHashedPassword(user, user.Password, password);

            if (result == PasswordVerificationResult.Success)
            {
                return user;
            }

            return null; 
        }

        public void addUser(string username, string password, string firstname, string lastname, string role)
        {
            using DBConnection db = new DBConnection();

            bool exists = db.Users.Any(x => x.Username == username);

            if (exists)
            {
                throw new Exception("A user with this login is already registered");
            }

            PasswordHasher<User> hasher = new PasswordHasher<User>();

            User user = new User
            {
                Username = username,
                FirstName = firstname,
                LastName = lastname,
                Role = role
            };

            user.Password = hasher.HashPassword(user, password);

            db.Users.Add(user);
            db.SaveChanges();
        }

        public User getUserByUsername(string username)
        {
            using DBConnection db = new DBConnection();
            return db.Users.FirstOrDefault(x => x.Username == username);
        }

        public void changePassword(string username, string newPassword)
        {
            using DBConnection db = new DBConnection();

            User user = db.Users.FirstOrDefault(x => x.Username == username);
            PasswordHasher<User> hasher = new PasswordHasher<User>();

            user.Password = hasher.HashPassword(user, newPassword);

            db.SaveChanges();
        }

        public List<User> searchUser(string first_name, string last_name, string username, string role)
        {
            using (DBConnection db = new DBConnection())
            {
                var query = db.Users.AsQueryable();


                if (!string.IsNullOrEmpty(first_name))
                {
                    string searchFirstName = first_name.Trim().ToLower();
                    query = query.Where(u => u.FirstName.ToLower().StartsWith(searchFirstName));
                }

                if (!string.IsNullOrEmpty(last_name))
                {
                    string searchLastName = last_name.Trim().ToLower();
                    query = query.Where(u => u.LastName.ToLower().StartsWith(searchLastName));
                }

                if (!string.IsNullOrEmpty(username))
                {
                    string searchUsername = username.Trim().ToLower();
                    query = query.Where(u => u.Username.ToLower().StartsWith(searchUsername));
                }

                if (!string.IsNullOrEmpty(role) && !role.Equals("All", StringComparison.OrdinalIgnoreCase))
                {
                    string searchRole = role.Trim().ToLower();
                    query = query.Where(u => u.Role.ToLower() == searchRole);
                }

                return query.ToList();
            }
        }
    }
}