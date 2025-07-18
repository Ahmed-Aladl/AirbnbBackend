using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.WishlistDTOs;
using Application.Interfaces;
using Application.Result;
using AutoMapper;
using Domain.Models;

namespace Application.Services
{
    public class WishlistService
    {

        public WishlistService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            UnitOfWork = unitOfWork;
            Mapper = mapper;
        }

        public IUnitOfWork UnitOfWork { get; }
        public IMapper Mapper { get; }

        public async Task<Result<List<WishlistDTO>>> GetUserWishlists(string userId)
        {
            var wishlists = await UnitOfWork.Wishlist.GetByUserIdAsync(userId);
            if (wishlists == null || !wishlists.Any())
                return Result<List<WishlistDTO>>.Fail("No wishlists found", (int)HttpStatusCode.NotFound);

            var mapped = Mapper.Map<List<WishlistDTO>>(wishlists);
            return Result<List<WishlistDTO>>.Success(mapped);
        }

        public async Task<Result<WishlistDTO>> AddToWishlist(string userId, int wishlistId, int propertyId)
        {
            if (await UnitOfWork.Wishlist.IsPropertyInWishlistAsync(userId, wishlistId, propertyId))
                return Result<WishlistDTO>.Fail("Property already in this wishlist", (int)HttpStatusCode.BadRequest);

            var wishlist = await UnitOfWork.Wishlist.GetByIdAsync(wishlistId);
            if (wishlist == null || wishlist.UserId != userId)
                return Result<WishlistDTO>.Fail("Wishlist not found or unauthorized", (int)HttpStatusCode.NotFound);

            if (wishlist.PropertyId == null)
            {
                wishlist.PropertyId = propertyId;
                UnitOfWork.Wishlist.Update(wishlist);
                var success = await UnitOfWork.SaveChangesAsync() > 0;
                if (!success)
                    return Result<WishlistDTO>.Fail("Couldn't add property to wishlist", (int)HttpStatusCode.BadRequest);

                var mapped = Mapper.Map<WishlistDTO>(wishlist);
                return Result<WishlistDTO>.Success(mapped);
            }
            return Result<WishlistDTO>.Fail("Wishlist already contains a property", (int)HttpStatusCode.BadRequest);
        }

        public async Task<Result<WishlistDTO>> RemoveFromWishlist(string userId, int wishlistId, int propertyId)
        {
            var wishlist = await UnitOfWork.Wishlist.GetByIdAsync(wishlistId);
            if (wishlist == null || wishlist.UserId != userId || wishlist.PropertyId != propertyId)
                return Result<WishlistDTO>.Fail("Wishlist not found, unauthorized, or property not in wishlist", (int)HttpStatusCode.NotFound);

            wishlist.PropertyId = null;
            UnitOfWork.Wishlist.Update(wishlist);
            var success = await UnitOfWork.SaveChangesAsync() > 0;
            if (!success)
                return Result<WishlistDTO>.Fail("Couldn't remove property from wishlist", (int)HttpStatusCode.BadRequest);

            var mapped = Mapper.Map<WishlistDTO>(wishlist);
            return Result<WishlistDTO>.Success(mapped);
        }

        public async Task<Result<WishlistDTO>> CreateWishlist(string userId, string name, string notes)
        {
            var wishlist = new Wishlist
            {
                UserId = userId,
                Name = name,
                Notes = notes,
                CreatedAt = DateTime.UtcNow
            };

            UnitOfWork.Wishlist.Add(wishlist);
            var success = await UnitOfWork.SaveChangesAsync() > 0;
            if (!success)
                return Result<WishlistDTO>.Fail("Couldn't create wishlist", (int)HttpStatusCode.BadRequest);

            var mapped = Mapper.Map<WishlistDTO>(wishlist);
            return Result<WishlistDTO>.Success(mapped);
        }

        public async Task<Result<WishlistDTO>> DeleteWishlist(string userId, int wishlistId)
        {
            var wishlist = await UnitOfWork.Wishlist.GetByIdAsync(wishlistId);
            if (wishlist == null || wishlist.UserId != userId)
                return Result<WishlistDTO>.Fail("Wishlist not found or unauthorized", (int)HttpStatusCode.NotFound);

            UnitOfWork.Wishlist.Delete(wishlist);
            var success = await UnitOfWork.SaveChangesAsync() > 0;
            if (!success)
                return Result<WishlistDTO>.Fail("Couldn't delete wishlist", (int)HttpStatusCode.BadRequest);

            var mapped = Mapper.Map<WishlistDTO>(wishlist);
            return Result<WishlistDTO>.Success(mapped);
        }
    }
}
