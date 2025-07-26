
namespace Application.DTOs.UserDto
{
    public class UserProfileDto
    {
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public string Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Bio { get; set; }
        public DateOnly? BirthDate { get; set; }
        public string? Country { get; set; }
        public string? ProfilePictureURL { get; set; }
    }
}
