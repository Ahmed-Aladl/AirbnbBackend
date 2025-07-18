
using Application.Responses;
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
            return StatusCode(statusCode, new ApiResponse<T>
            {
                IsSuccess = true,
                Message = message,
                StatusCode = statusCode,
                Data = data
            });
        }

        protected IActionResult Fail(string message, int statusCode = 400)
        {
            return StatusCode(statusCode, new ApiResponse<string>
            {
                IsSuccess = false,
                Message = message,
                StatusCode = statusCode,
                Data = null
            });
        }

        protected IActionResult NotFoundResponse(string message = "Not found")
        {
            return StatusCode(404, new ApiResponse<string>
            {
                IsSuccess = false,
                Message = message,
                StatusCode = 404,
                Data = null
            });
        }

        protected IActionResult InternalError(string message = "Internal server error")
        {
            return StatusCode(500, new ApiResponse<string>
            {
                IsSuccess = false,
                Message = message,
                StatusCode = 500,
                Data = null
            });
        }

        protected IActionResult UnauthorizedResponse(string message = "Unauthorized")
        {
            return StatusCode(401, new ApiResponse<string>
            {
                IsSuccess = false,
                Message = message,
                StatusCode = 401,
                Data = null
            });
        }

        protected IActionResult ForbiddenResponse(string message = "Forbidden")
        {
            return StatusCode(403, new ApiResponse<string>
            {
                IsSuccess = false,
                Message = message,
                StatusCode = 403,
                Data = null
            });
        }


        //may also add CurrentUser
    }
}