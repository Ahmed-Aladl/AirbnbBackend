using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces.IRepositories;
using Application.Shared;
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

        public async Task<PaginatedResult<Property>> GetPageWithCoverAsync(int page, int pageSize)
        {
            var filtered = Db.Properties
                                    .Where(p => !p.IsDeleted && p.IsActive);
            var totalCount = filtered.Count();
            
            var pageData= await filtered
                                .Skip((page - 1) * pageSize)
                                .Take(pageSize)
                                .Include (p => p.Images.Where(i=> !i.IsDeleted && i.IsCover))
                                .ToListAsync();

            return new PaginatedResult<Property>()
            {
                Items = pageData,
                MetaData = new()
                {
                    Page = page,
                    PageSize = pageSize,
                    Total = totalCount
                }
            };
        }

        public async Task<PaginatedResult<Property>> GetNearestPageWithCover(IpLocation ipLocation, int page, int pageSize, double maxDistanceKm)
        {
            var propertiesNearby = Db.Properties
                            .Where(p =>
                                6371 * Math.Acos(
                                    Math.Cos(Math.PI * ipLocation.Lat / 180) *
                                    Math.Cos(Math.PI * (double)p.Latitude / 180) *
                                    Math.Cos(Math.PI * ((double)p.Longitude - ipLocation.Lon) / 180) +
                                    Math.Sin(Math.PI * ipLocation.Lat / 180) *
                                    Math.Sin(Math.PI * (double)p.Latitude / 180)
                                ) < maxDistanceKm
                            );
            var totalCount = propertiesNearby.Count();

            var result = await propertiesNearby
                            .Skip((page-1)* pageSize)
                            .Take(pageSize)
                            .ToListAsync();
            return new PaginatedResult<Property>()
            {
                Items = result,
                MetaData = new()
                {
                    Page = page,
                    PageSize = pageSize,
                    Total = totalCount,
                }
            };


        }
    }
}
