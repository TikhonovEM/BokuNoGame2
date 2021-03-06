using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BokuNoGame2.Models
{
    public class User : IdentityUser
    {
        public string Nickname { get; set; }
        public string FullName { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime RegistrationDate { get; set; }
        public byte[] Photo { get; set; }
    }
}
