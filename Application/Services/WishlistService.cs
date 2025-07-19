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
using Microsoft.EntityFrameworkCore;

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
                return Result<List<WishlistDTO>>.Fail(
                    "No wishlists found",
                    (int)HttpStatusCode.NotFound
                );

            var mapped = Mapper.Map<List<WishlistDTO>>(wishlists);
            return Result<List<WishlistDTO>>.Success(mapped);
        }

        public async Task<Result<WishlistDTO>> AddToWishlist(
            string userId,
            int wishlistId,
            int propertyId
        )
        {
            var wishlist = await UnitOfWork.Wishlist.GetByIdAsync(wishlistId);
            if (wishlist == null || wishlist.UserId != userId)
                return Result<WishlistDTO>.Fail(
                    "Not found or unauthorized",
                    (int)HttpStatusCode.BadRequest
                );

            if (await UnitOfWork.Wishlist.IsPropertyInWishlistAsync(userId, wishlistId, propertyId))
                return Result<WishlistDTO>.Fail(
                    "Property already exists",
                    (int)HttpStatusCode.BadRequest
                );

            await UnitOfWork.Wishlist.AddPropertyToWishlistAsync(userId, wishlistId, propertyId);
            var success = await UnitOfWork.SaveChangesAsync() > 0;
            if (!success)
                return Result<WishlistDTO>.Fail(
                    "Error adding property",
                    (int)HttpStatusCode.BadRequest
                );

            return Result<WishlistDTO>.Success(Mapper.Map<WishlistDTO>(wishlist));
        }

        public async Task<Result<WishlistDTO>> RemoveFromWishlist(
            string userId,
            int wishlistId,
            int propertyId
        )
        {
            var wishlist = await UnitOfWork.Wishlist.GetByIdAsync(wishlistId);
            if (wishlist == null || wishlist.UserId != userId)
                return Result<WishlistDTO>.Fail(
                    "Wishlist not found or unauthorized",
                    (int)HttpStatusCode.NotFound
                );

            if (
                !await UnitOfWork.Wishlist.IsPropertyInWishlistAsync(userId, wishlistId, propertyId)
            )
                return Result<WishlistDTO>.Fail(
                    "Property not in wishlist",
                    (int)HttpStatusCode.BadRequest
                );

            await UnitOfWork.Wishlist.RemovePropertyFromWishlistAsync(
                userId,
                wishlistId,
                propertyId
            );
            var success = await UnitOfWork.SaveChangesAsync() > 0;
            if (!success)
                return Result<WishlistDTO>.Fail(
                    "Couldn't remove property from wishlist",
                    (int)HttpStatusCode.BadRequest
                );

            return Result<WishlistDTO>.Success(Mapper.Map<WishlistDTO>(wishlist));
        }

        public async Task<Result<WishlistDTO>> CreateWishlist(
            string userId,
            string name,
            string notes
        )
        {
            var wishlist = new Wishlist
            {
                Name = name,
                Notes = notes,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
            };

            UnitOfWork.Wishlist.Add(wishlist);
            var success = await UnitOfWork.SaveChangesAsync() > 0;
            if (!success)
                return Result<WishlistDTO>.Fail(
                    "Couldn't create wishlist",
                    (int)HttpStatusCode.BadRequest
                );

            return Result<WishlistDTO>.Success(Mapper.Map<WishlistDTO>(wishlist));
        }

        public async Task<Result<WishlistDTO>> DeleteWishlist(string userId, int wishlistId)
        {
            var wishlist = await UnitOfWork.Wishlist.GetByIdAsync(wishlistId);
            if (wishlist == null || wishlist.UserId != userId)
                return Result<WishlistDTO>.Fail(
                    "Wishlist not found or unauthorized",
                    (int)HttpStatusCode.NotFound
                );

            UnitOfWork.Wishlist.Delete(wishlist);
            var success = await UnitOfWork.SaveChangesAsync() > 0;
            if (!success)
                return Result<WishlistDTO>.Fail(
                    "Couldn't delete wishlist",
                    (int)HttpStatusCode.BadRequest
                );

            return Result<WishlistDTO>.Success(Mapper.Map<WishlistDTO>(wishlist));
        }
    }
}
