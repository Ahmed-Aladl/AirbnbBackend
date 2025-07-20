using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.UserDto
{
    public class RegisterDto
    {
        [EmailAddress]
        [Required]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(12, ErrorMessage = "Password must be at least 12 characters long")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{12,}$",
            ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character")]
        public string Password { get; set; }

    }
}