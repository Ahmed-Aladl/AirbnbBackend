using Domain.Models;

namespace Airbnb.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken(User user);
        DateTime GetRefreshTokenExpiryDate();
    }
}
