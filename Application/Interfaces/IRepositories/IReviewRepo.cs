using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;


namespace Application.Interfaces.IRepositories
{
    public interface IReviewRepo : IRepository<Review,int>
    {
        Task<List<Review>> GetAllAsync();
        Task<Review?> GetByIdAsync(int id);
        Task AddAsync(Review entity);
        Task AddRangeAsync(ICollection<Review> entities);
        Task<Review?> GetByBookingIdAsync(int BookingId);

        Task<List<Review>> GetByPropertyIdAsync(int propertyId);
        Task<List<Review>> GetByUserIdAsync(string userId);
        Task<Review> GetByIdWithUserAsync(int id);


    }
}
