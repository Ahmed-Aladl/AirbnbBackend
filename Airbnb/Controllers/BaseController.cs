
using Application.Result; // Changed from Application.Responses
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Airbnb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseController : ControllerBase
    {
        protected IActionResult Success<T>(T data, string message = "", int statusCode = 200)
        {
            return StatusCode(statusCode, Result<T>.Success(data, statusCode, message));
        }

        protected IActionResult Fail(string message, int statusCode = 400)
        {
            return StatusCode(statusCode, Result<string>.Fail(message, statusCode));
        }

        protected IActionResult NotFoundResponse(string message = "Not found")
        {
            return StatusCode(404, Result<string>.Fail(message, 404));
        }

        protected IActionResult InternalError(string message = "Internal server error")
        {
            return StatusCode(500, Result<string>.Fail(message, 500));
        }

        protected IActionResult UnauthorizedResponse(string message = "Unauthorized")
        {
            return StatusCode(401, Result<string>.Fail(message, 401));
        }

        protected IActionResult ForbiddenResponse(string message = "Forbidden")
        {
            return StatusCode(403, Result<string>.Fail(message, 403));
        }

        // Helper method to handle Result<T> returns
        protected IActionResult HandleResult<T>(Result<T> result)
        {
            if (result == null) return NotFound();
            if (result.IsSuccess && result.Data != null)
                return StatusCode(result.StatusCode ?? 200, result);
            if (result.IsSuccess && result.Data == null)
                return NotFound();
            return StatusCode(result.StatusCode ?? 400, result);
        }
    }
}