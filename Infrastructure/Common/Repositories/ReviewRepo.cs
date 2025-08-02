using Application.Interfaces.IRepositories;
using Domain.Models;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Common.Repositories
{
    public class ReviewRepo : Repository<Review, int>, IReviewRepo
    {
        public ReviewRepo(AirbnbContext db) : base(db) { }
     
        public async Task<List<Review>> GetAllAsync()
        {
            return await Db.Reviews
                .Include(r => r.User)
                .Include(r => r.Booking)
                .Include(r => r.Property)
                .ToListAsync();
        }
        

        public async Task<Review?> GetByIdAsync(int id)
        {
            return await Db.Reviews
                .Include(r => r.User)
                .Include(r => r.Booking)
                .Include(r => r.Property)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Review?> GetByBookingIdAsync(int BookingId)
        {
            return await Db.Reviews.Include(r => r.Booking).FirstOrDefaultAsync(r => r.BookingId == BookingId);

        }


        //not used for now 
        public async Task<Review> GetByIdWithUserAsync(int id)
        {
            return await Db.Reviews
                .Include(r => r.User) 
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        //host
        //public async Task<Review?> GetByHostIdAllProperties(string userId)
        //{
        //    return await Db.Reviews
        //        .Include(r => r.PropertyId)
        //        .FirstOrDefaultAsync(r => r.Property== userId.FirstOrDefault(userId);
        //}

        public async Task<List<Review>> GetReviewsByHostIdAsync(string hostId)
        {
            try
            {
                return await Db.Reviews
                    .Include(r => r.User)
                    .Include(r => r.Property)
                    .Include(r => r.Booking)
                    .Where(r => r.Property.HostId == hostId)
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetReviewsByHostIdAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Review>> GetByPropertyIdWithUserAsync(int propertyId)
        {
            return await Db.Reviews
                .Include(r => r.User) 
                .Include(r => r.Property) 
                .Where(r => r.PropertyId == propertyId)
                .OrderByDescending(r => r.CreatedAt) 
                .ToListAsync();
        }



        public async Task AddAsync(Review entity)
        {

            await Db.Reviews.AddAsync(entity);
        }

        public async Task AddRangeAsync(ICollection<Review> entities)
        {
            await Db.Reviews.AddRangeAsync(entities);
        }



        public async Task<List<Review>> GetByUserIdAsync(string userId)
        {
            try
            {
                return await Db.Reviews
                    .Where(r => r.UserId == userId)
                    .Include(r => r.User)
                    .Include(r => r.Property) 
                    .Include(r => r.Booking) 
                    .OrderByDescending(r => r.CreatedAt) 
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetByUserIdAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Review>> GetByPropertyIdAsync(int propertyId)
        {
            try
            {
                return await Db.Reviews
                    .Where(r => r.PropertyId == propertyId)
                    .Include(r => r.User) 
                    .Include(r => r.Property)
                    .Include(r => r.Booking) 
                    .OrderByDescending(r => r.CreatedAt) 
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetByPropertyIdAsync: {ex.Message}");
                throw;
            }
        }

    }
}
