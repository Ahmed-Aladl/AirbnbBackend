using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Application.DTOs.UserDto
{
    public class TokenDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string? UserId { get; set; }
        public IList<IdentityRole>? Roles { get; set; }
    }
}
