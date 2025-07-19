using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Domain.Models;
using Application.Interfaces.IRepositories;
using Infrastructure.Contexts;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Common.Repositories
{
    public class PropertyRepository : Repository<Property, int>, IPropertyRepository
    {
        public PropertyRepository(AirbnbContext context) : base(context) { }

        public async Task<List<Property>> GetByHostIdAsync(string hostId)
        {
            return await Db.Properties.Where(p=> p.HostId == hostId).ToListAsync();
        }

        public async Task<Property> GetByIdAsync(int id)
        {
            return await Db.Set<Property>()
                .Include(p => p.PropertyType)
                .Include(p => p.Host)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Property> GetByIdWithCalendarAsync(int id)
        {
            return await Db.Set<Property>()
                .Include(p => p.CalendarAvailabilities)
                .Include(p => p.PropertyType)
                .Include(p => p.Host)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}