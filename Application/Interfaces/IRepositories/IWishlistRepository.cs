using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace Application.Interfaces.IRepositories
{
    public interface IWishlistRepository : IRepository<Wishlist, int>
    {
        Task<Wishlist> GetByIdAsync(int id);
        Task<List<Wishlist>> GetByUserIdAsync(string userId);
        Task<bool> IsPropertyInWishlistAsync(string userId, int wishlistId, int propertyId);
        Task AddPropertyToWishlistAsync(string userId, int wishlistId, int propertyId);
        Task RemovePropertyFromWishlistAsync(string userId, int wishlistId, int propertyId);
        Task CreateWishlistAsync(string userId, string name, string notes);
        Task DeleteWishlistAsync(string userId, int wishlistId);

    }
}
