﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Interfaces.IRepositories;
using Domain.Models;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Common.Repositories
{
    public class BookingRepository : Repository<Booking, int>, IBookingRepo
    {
        private readonly AirbnbContext _db;

        public BookingRepository(AirbnbContext db)
            : base(db)
        {
            _db = db;
        }

        public async Task<List<Booking>> GetAllAsync()
        {
            return await _db.Bookings
                   .Include(b => b.Property).Include(b => b.User)

                   .ToListAsync();
        }

        public async Task<Booking?> GetByIdAsync(int id)
        {
            return await _db.Bookings
                   .Include(b => b.Property)
                   .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<List<Booking>> GetBookingByUserIdAsync(string userId)
        {
            return await _db.Bookings
         .Include(b => b.Property)
         .Where(b => b.UserId == userId && !b.IsDeleted)
         .ToListAsync();
        }

        public async Task<List<Booking>> GetBookingByPropertyIdAsync(int propertyId)
        {
            return await _db.Bookings
        .Include(b => b.Property)
        .Include(b => b.User) 
        .Where(b => b.PropertyId == propertyId && !b.IsDeleted)
        .ToListAsync();
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


        public async Task<Booking?> GetBookingWithPropertyAsync(int id)
        {
            return await _db.Bookings
                .Include(b => b.Property)
                .FirstOrDefaultAsync(b => b.Id == id);
        }


    }
}
