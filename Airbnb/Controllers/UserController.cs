using Application.DTOs.UserDto;
using Application.Interfaces;
using Application.Shared;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Airbnb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        //private readonly IServiceFactory _serviceFactory;
        public UserController(IUnitOfWork unitOfWork,
            //IServiceFactory serviceFactory,
            UserManager<User> userManager)
        //: base(unitOfWork)
        {
            //_serviceFactory = serviceFactory;
            userManager.FindByEmailAsync(email: "k@e.com");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [EndpointSummary("Get all user profiles (For Admins only).")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PaginatedResult<UserProfileDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAll([FromQuery] UserProfileQueryParamsDto dto)
        { }
    }
}
