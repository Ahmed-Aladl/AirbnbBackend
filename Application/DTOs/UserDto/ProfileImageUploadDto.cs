using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Application.DTOs.UserDto
{
    public class ProfileImageUploadDto
    {
        public string UserId { get; set; }
        public IFormFile File { get; set; }

    }
}
