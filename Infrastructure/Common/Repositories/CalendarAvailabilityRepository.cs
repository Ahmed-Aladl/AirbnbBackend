using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces.IRepositories;
using Domain.Models;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Common.Repositories
{
    public class CalendarAvailabilityRepository : Repository<CalendarAvailability, int>, ICalendarAvailabilityRepo
    {
        public CalendarAvailabilityRepository(AirbnbContext db) : base(db) { }

        public async Task<List<CalendarAvailability>> GetByPropertyIdAsync(int propertyId)
        {
            return await Db.Set<CalendarAvailability>()
                .Where(c => c.PropertyId == propertyId)
                .OrderBy(c => c.Date)
                .ToListAsync();
        }

        public async Task<List<CalendarAvailability>> GetAvailabilityRangeAsync(int propertyId, DateTime startDate, DateTime endDate)
        {
            return await Db.Set<CalendarAvailability>()
                .Where(c => c.PropertyId == propertyId && c.Date >= startDate && c.Date <= endDate)
                .OrderBy(c => c.Date)
                .ToListAsync();
        }

        public async Task<bool> IsPropertyAvailableAsync(int propertyId, DateTime date)
        {
            return await Db.Set<CalendarAvailability>()
                .AnyAsync(c => c.PropertyId == propertyId && c.Date == date && c.IsAvailable);
        }

        public async Task UpdateAvailabilityRangeAsync(int propertyId, List<CalendarAvailability> availabilities)
        {
            var existingDates = availabilities.Select(a => a.Date).ToList();

            // Remove existing entries for these dates
            var toRemove = await Db.Set<CalendarAvailability>()
                .Where(c => c.PropertyId == propertyId && existingDates.Contains(c.Date))
                .ToListAsync();

            Db.Set<CalendarAvailability>().RemoveRange(toRemove);

            // Add new entries
            await Db.Set<CalendarAvailability>().AddRangeAsync(availabilities);
        }
    }
}