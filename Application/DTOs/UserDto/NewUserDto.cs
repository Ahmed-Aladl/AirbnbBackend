namespace Application.DTOs.UserDto
{
    public class NewUserDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public List<string> Roles { get; set; }
        public string Token { get; set; }
    }
}
