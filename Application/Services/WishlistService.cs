using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.PropertyDTOS;
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

            foreach (var wishlist in mapped)
            {
                var firstPropertyId = wishlist.PropertyIds.FirstOrDefault();
                if (firstPropertyId != 0)
                {
                    var prop = await UnitOfWork.PropertyRepo.GetByIdWithCoverAsync(firstPropertyId);
                    var coverImage = prop?.Images?.FirstOrDefault(i => i.IsCover) ?? prop?.Images?.FirstOrDefault();
                    wishlist.CoverImageUrl = coverImage?.ImageUrl;
                }
            }

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
            string notes,
            List<int> propertyIds
        )
        {

            if (propertyIds == null || !propertyIds.Any())
                return Result<WishlistDTO>.Fail("At least one property is required", (int)HttpStatusCode.BadRequest);

            var wishlist = new Wishlist
            {
                Name = name,
                Notes = notes,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                WishlistProperties = propertyIds.Select(id => new WishlistProperty
                {
                    PropertyId = id
                }).ToList()
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








        public async Task<Result<WishlistWithPropertiesDTO>> GetPropertiesInWishlist(string userId, int wishlistId)
        {
            var wishlist = await UnitOfWork.Wishlist.GetByIdAsync(wishlistId);

            if (wishlist == null || wishlist.UserId != userId)
                return Result<WishlistWithPropertiesDTO>.Fail("Wishlist not found or unauthorized", (int)HttpStatusCode.NotFound);

            var properties = wishlist.WishlistProperties?.Select(wp => wp.Property)?.ToList();

            var dto = new WishlistWithPropertiesDTO
            {
                Id = wishlist.Id,
                Name = wishlist.Name,
                Notes = wishlist.Notes,
                Properties = properties != null ? Mapper.Map<List<PropertyDisplayDTO>>(properties) : new()
            };

            return Result<WishlistWithPropertiesDTO>.Success(dto);
        }

    }
}
