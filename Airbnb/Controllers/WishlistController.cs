using Airbnb.Extensions;
using Application.DTOs.WishlistDTOs;
using Application.Services;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Airbnb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishlistController: BaseController
    {
        public WishlistService WishlistService { get;}

        public WishlistController(WishlistService wishlistService, UserManager<User> userManager)
        {
            WishlistService = wishlistService;
        }

        // GET: api/<WishlistController>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            var userId = User.GetUserId();
            return (await WishlistService.GetUserWishlists(userId)).ToActionResult();
        }


        // POST: api/<WishlistController>/create
        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] WishlistDTO dto)
        {
            var userId = User.GetUserId();
            return (await WishlistService.CreateWishlist(userId, dto.Name, dto.Notes)).ToActionResult();
        }


        // POST: api/<WishlistController>/add/{wishlistId}/{propertyId}
        [HttpPost("add/{wishlistId}/{propertyId}")]
        [Authorize]
        public async Task<IActionResult> AddToWishlist(int wishlistId, int propertyId)
        {
            var userId = User.GetUserId();
            return (await WishlistService.AddToWishlist(userId, wishlistId, propertyId)).ToActionResult();
        }



        // DELETE: api/<WishlistController>/remove/{wishlistId}/{propertyId}
        [HttpDelete("remove/{wishlistId}/{propertyId}")]
        [Authorize]
        public async Task<IActionResult> RemoveFromWishlist(int wishlistId, int propertyId)
        {
            var userId = User.GetUserId();
            return (await WishlistService.RemoveFromWishlist(userId, wishlistId, propertyId)).ToActionResult();
        }



        // DELETE: api/<WishlistController>/{wishlistId}
        [HttpDelete("{wishlistId}")]
        [Authorize]
        public async Task<IActionResult> Delete(int wishlistId)
        {
            var userId = User.GetUserId();
            return (await WishlistService.DeleteWishlist(userId, wishlistId)).ToActionResult();
        }


    }
}
