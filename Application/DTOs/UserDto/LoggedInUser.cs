namespace Application.DTOs.UserDto
{
    public record LoggedInUser
    {
        public string Id { get; init; }
        public string UserName { get; init; }
        public List<string> Roles { get; init; } = new List<string>();

        public bool IsInRole(string role)
        {
            if (string.IsNullOrWhiteSpace(role))
                return false;

            return Roles.Contains(role, StringComparer.OrdinalIgnoreCase);
        }
    }
}