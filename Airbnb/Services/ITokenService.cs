using Domain.Models;

namespace Airbnb.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user,IList<string> roles);
        string GenerateRefreshToken(User user);
        DateTime GetRefreshTokenExpiryDate();
    }
}
