using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace Airbnb.Middleware
{
    public class TokenValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly TokenValidationParameters _tokenValidationParameters;

        public TokenValidationMiddleware(RequestDelegate next, TokenValidationParameters tokenValidationParameters)
        {
            _next = next;
            _tokenValidationParameters = tokenValidationParameters;
        }

        public async Task Invoke(HttpContext context)
        {
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            var token = authHeader?.StartsWith("Bearer ") == true ? authHeader.Substring(7) : null;

            if (!string.IsNullOrEmpty(token))
            {
                var handler = new JwtSecurityTokenHandler();

                try
                {
                    handler.ValidateToken(token, _tokenValidationParameters, out _);
                }
                catch (SecurityTokenExpiredException)
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Access token expired");
                    return;
                }
                catch
                {
                    // token is invalid → let pipeline continue (optional)
                }
            }

            await _next(context);
        }
    }

}
