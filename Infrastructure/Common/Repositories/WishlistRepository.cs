using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.IRepositories;
using Domain.Models;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Common.Repositories
{
    public class WishlistRepository : Repository<Wishlist, int>, IWishlistRepository
    {
        public WishlistRepository(AirbnbContext _db) : base(_db)
        {
        }

        public async Task<Wishlist> GetByIdAsync(int id)
        {
            return await Db.Set<Wishlist>()
                .Include(w => w.WishlistProperties)
                .ThenInclude(wp => wp.Property)
                .FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task<List<Wishlist>> GetByUserIdAsync(string userId)
        {
            return await Db.Wishlists
                .Include(w => w.WishlistProperties)
                .ThenInclude(wp => wp.Property)
                .Where(w => w.UserId == userId)
                .ToListAsync();
        }

        public async Task<bool> IsPropertyInWishlistAsync(string userId, int wishlistId, int propertyId)
        {
            return await Db.Set<WishlistProperty>()
                .AnyAsync(wp => wp.WishlistId == wishlistId && wp.PropertyId == propertyId);
        }

        public async Task AddPropertyToWishlistAsync(string userId, int wishlistId, int propertyId)
        {
            var wishlist = await Db.Wishlists
                .FirstOrDefaultAsync(w => w.Id == wishlistId && w.UserId == userId);

            if (wishlist != null)
            {
                bool exists = await IsPropertyInWishlistAsync(userId, wishlistId, propertyId);
                if (!exists)
                {
                    var wishlistProperty = new WishlistProperty
                    {
                        WishlistId = wishlistId,
                        PropertyId = propertyId
                    };
                    await Db.Set<WishlistProperty>().AddAsync(wishlistProperty);
                }
            }
        }

        public async Task RemovePropertyFromWishlistAsync(string userId, int wishlistId, int propertyId)
        {
            var wishlistProperty = await Db.Set<WishlistProperty>()
                .FirstOrDefaultAsync(wp =>
                    wp.WishlistId == wishlistId &&
                    wp.PropertyId == propertyId &&
                    wp.Wishlist.UserId == userId);

            if (wishlistProperty != null)
            {
                Db.Set<WishlistProperty>().Remove(wishlistProperty);
            }
        }

        public async Task CreateWishlistAsync(string userId, string name, string notes)
        {
            var wishlist = new Wishlist
            {
                UserId = userId,
                Name = name,
                Notes = notes,
                CreatedAt = DateTime.UtcNow
            };
            await Db.Wishlists.AddAsync(wishlist);
            await Db.SaveChangesAsync();
        }

        public async Task DeleteWishlistAsync(string userId, int wishlistId)
        {
            var wishlist = await Db.Wishlists
                .FirstOrDefaultAsync(w => w.UserId == userId && w.Id == wishlistId);

            if (wishlist != null)
            {
                
                var related = Db.Set<WishlistProperty>()
                    .Where(wp => wp.WishlistId == wishlistId);
                Db.Set<WishlistProperty>().RemoveRange(related);

                Db.Wishlists.Remove(wishlist);
                await Db.SaveChangesAsync();
            }
        }
    }
}
