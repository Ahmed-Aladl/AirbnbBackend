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
                .FirstOrDefaultAsync(w => w.Id == id);
        }
        public async Task<List<Wishlist>> GetByUserIdAsync(string userId)
        {
            return await Db.Set<Wishlist>()
                .Include(w => w.Property)
                .Where(w => w.UserId == userId)
                .ToListAsync();
        }

        public async Task<bool> IsPropertyInWishlistAsync(string userId, int wishlistId, int propertyId)
        {
            return await Db.Wishlist
                .AnyAsync(w => w.UserId == userId && w.Id == wishlistId && w.PropertyId == propertyId);
        }

        public async Task AddPropertyToWishlistAsync(string userId, int wishlistId, int propertyId)
        {
            var wishlist = await Db.Wishlist
                .FirstOrDefaultAsync(w => w.UserId == userId && w.Id == wishlistId);
            if (wishlist != null && wishlist.PropertyId == null)
            {
                wishlist.PropertyId = propertyId;
                Update(wishlist);
            }
        }

        public async Task RemovePropertyFromWishlistAsync(string userId, int wishlistId, int propertyId)
        {
            var wishlist = await Db.Wishlist
                .FirstOrDefaultAsync(w => w.UserId == userId && w.Id == wishlistId && w.PropertyId == propertyId);
            if (wishlist != null)
            {
                wishlist.PropertyId = null;
                Update(wishlist);
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
            Db.Wishlist.Add(wishlist);
            await Db.SaveChangesAsync();
        }

        public async Task DeleteWishlistAsync(string userId, int wishlistId)
        {
            var wishlist = await Db.Wishlist
                .FirstOrDefaultAsync(w => w.UserId == userId && w.Id == wishlistId);
            if (wishlist != null)
            {
                Db.Wishlist.Remove(wishlist);
                await Db.SaveChangesAsync();
            }
        }
    }
}
