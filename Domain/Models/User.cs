using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Domain.Models
{
    public class User:IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public DateTime CreateAt { get; set; }
        public DateTime UpdatedAt{ get; set; }
        public string ProfilePictureURL { get; set; }

    }
}
