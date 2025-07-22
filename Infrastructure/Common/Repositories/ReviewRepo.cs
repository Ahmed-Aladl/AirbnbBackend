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
                .Include(r => r.Property)
                .ToListAsync();
        }
        

        public async Task<Review?> GetByIdAsync(int id)
        {
            return await Db.Reviews
                .Include(r => r.User)
                .Include(r => r.Property)
                .FirstOrDefaultAsync(r => r.Id == id);
        }


        public async Task<Review?> GetByBookingIdAsync(int BookingId)
        {
            return await Db.Reviews.Include(r => r.Booking).FirstOrDefaultAsync(r => r.BookingId == BookingId);

        } 

        public async Task AddAsync(Review entity)
        {

            await Db.Reviews.AddAsync(entity);
        }

        public async Task AddRangeAsync(ICollection<Review> entities)
        {
            await Db.Reviews.AddRangeAsync(entities);
        }


    }
}
