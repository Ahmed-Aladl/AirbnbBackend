//using Nestin.Core.Shared;
using System.ComponentModel.DataAnnotations;
using Application.Shared;
using Microsoft.AspNetCore.Http;

namespace Application.DTOs.UserDto
{
    public class UpdateUserProfileDto
    {
        [Required(ErrorMessage = "First name is required.")]
        [MinLength(2, ErrorMessage = "First name must be at least 2 characters.")]
        [MaxLength(100, ErrorMessage = "First name cannot exceed 100 characters.")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [MinLength(2, ErrorMessage = "Last name must be at least 2 characters.")]
        [MaxLength(100, ErrorMessage = "Last name cannot exceed 100 characters.")]
        public string? LastName { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format.")]
        [MinLength(3, ErrorMessage = "Phone number must be at least 3 digits.")]
        [MaxLength(15, ErrorMessage = "Phone number cannot exceed 15 digits.")]
        public string? PhoneNumber { get; set; }

        [MaxLength(500, ErrorMessage = "Bio cannot exceed 500 characters.")]
        public string? Bio { get; set; }
        public DateOnly? BirthDate { get; set; }
        public string? Country { get; set; }

        //[AllowedImageExtensionsAttribute(new[] { ".jpg", ".jpeg", ".png", ".gif" }, ErrorMessage = "Only image files (.jpg, .jpeg, .png, .gif) are allowed.")]
        //public IFormFile? ProfilePictureURL { get; set; }
    }
}
