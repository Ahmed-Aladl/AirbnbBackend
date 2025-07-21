using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces.IRepositories;
using Domain.Models;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Common.Repositories
{
    public class PropertyRepository : Repository<Property, int>, IPropertyRepository
    {
        public PropertyRepository(AirbnbContext context)
            : base(context) { }

        

        public async Task<Property> GetByIdAsync(int id)
        {
            return await Db.Set<Property>()
                .Include(p => p.PropertyType)
                .Include(p => p.Host)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task<Property> GetByIdWithCoverAsync(int id)
        {
            return await Db.Set<Property>()
                .Include(p => p.Images.Where(i=> i.IsCover))
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

        public async Task<List<Property>> GetByHostIdAsync(string hostId)
        {
            return await Db.Properties.Where(p => p.HostId == hostId).ToListAsync();
        }
        public async Task<List<Property>> GetByHostIdWithCoverAsync(string hostId)
        {
            return await Db.Properties
                                    .AsNoTracking()
                                    .Where(p=> p.HostId == hostId && !p.IsDeleted && p.Images.Any(i=> i.IsCover))
                                    .Include(p=> p.Images.Where(i=> i.IsCover))
                                    .ToListAsync();
        }
    }
}
