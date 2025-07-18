using Microsoft.AspNetCore.Identity;

namespace Airbnb.Middleware
{
    public static class IdentityResultExtensions
    {
        public static List<string> ToErrorList(this IdentityResult result)
        {
            return result.Errors.Select(e => $"{e.Code}: {e.Description}").ToList();
        }
    }
}
