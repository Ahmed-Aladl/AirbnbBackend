using Domain.Models;
using Infrastructure.Contexts;
using Application.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Common.Repositories
{
    public class BookingRepository : Repository<Booking, int>, IBookingRepo
    {
        private readonly AirbnbContext _db;

        public BookingRepository(AirbnbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<List<Booking>> GetAllAsync()
        {
            return await _db.Bookings.ToListAsync();
        }

        public async Task<Booking?> GetByIdAsync(int id)
        {
            return await _db.Bookings.FindAsync(id);
        }

        public async Task AddAsync(Booking entity)
        {
            await _db.Bookings.AddAsync(entity);
        }

        public void Delete(Booking entity)
        {
            _db.Bookings.Remove(entity);
        }

        public void Update(Booking entity)
        {
            _db.Bookings.Update(entity);
        }
    }
}
