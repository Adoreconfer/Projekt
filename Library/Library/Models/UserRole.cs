using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Library.Models
{
    public class UserRole
    {
        public string RoleName { get; set; }
        public ICollection<User> Users { get; set; }
    }
}
