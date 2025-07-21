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
    public class WishlistController : BaseController
    {
        private string UserId { get; set; } = "1a8e2c26-d117-4ac6-a796-e57fa4dd83ac";
        public WishlistService WishlistService { get; }

        public WishlistController(WishlistService wishlistService)
        {
            WishlistService = wishlistService;
        }
        // GET: api/<WishlistController>
        [HttpGet]
        //[Authorize]
        public async Task<IActionResult> Get()
        {
            //var userId = "1d1afe4d-301c-4f72-9e41-5773d0d27fa2";// User.GetUserId();
            var userId = UserId;
            return ToActionResult(await WishlistService.GetUserWishlists(userId));
        }


        // POST: api/<WishlistController>/create
        [HttpPost("create")]
        //[Authorize]
        public async Task<IActionResult> Create([FromBody] CreateWishlistDTO dto)
        {
            //var userId = "1d1afe4d-301c-4f72-9e41-5773d0d27fa2"; //User.GetUserId();
            var userId =UserId; //User.GetUserId();
            return (await WishlistService.CreateWishlist(userId, dto.Name, dto.Notes)).ToActionResult();
        }


        // POST: api/<WishlistController>/add/{wishlistId}/{propertyId}
        [HttpPost("add/{wishlistId}/{propertyId}")]
        //[Authorize]
        public async Task<IActionResult> AddToWishlist(int wishlistId, int propertyId)
        {
            //var userId = "1d1afe4d-301c-4f72-9e41-5773d0d27fa2"; //User.GetUserId();
            var userId = UserId; //User.GetUserId();
            return (await WishlistService.AddToWishlist(userId, wishlistId, propertyId)).ToActionResult();
        }



        // DELETE: api/<WishlistController>/remove/{wishlistId}/{propertyId}
        [HttpDelete("remove/{wishlistId}/{propertyId}")]
        //[Authorize]
        public async Task<IActionResult> RemoveFromWishlist(int wishlistId, int propertyId)
        {
            var userId = "1d1afe4d-301c-4f72-9e41-5773d0d27fa2"; //User.GetUserId();
            userId = UserId;
            return (await WishlistService.RemoveFromWishlist(userId, wishlistId, propertyId)).ToActionResult();
        }



        // DELETE: api/<WishlistController>/{wishlistId}
        [HttpDelete("{wishlistId}")]
        //[Authorize]
        public async Task<IActionResult> Delete(int wishlistId)
        {
            var userId = "1d1afe4d-301c-4f72-9e41-5773d0d27fa2"; //User.GetUserId();
            userId = UserId;
            return (await WishlistService.DeleteWishlist(userId, wishlistId)).ToActionResult();
        }


    }
}
