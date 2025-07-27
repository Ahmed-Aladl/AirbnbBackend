using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.PropertyDTOS;
using Application.Interfaces.IRepositories;
using Application.Shared;
using Domain.Models;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using Stripe.Terminal;

namespace Infrastructure.Common.Repositories
{
    public class PropertyRepository : Repository<Property, int>, IPropertyRepository
    {
        public PropertyRepository(AirbnbContext context)
            : base(context) { }

        
        //public async  Task<Property> GetAllWithFavourite(string userId="1")
        //{
        //    var x = Db.Properties
        //                .Include(p => p.Images.Where(i => !i.IsDeleted && i.IsCover))
        //                .Include(p => p.WishlistProperties.Where(wp => wp.Wishlist.UserId == userId))
                        
        //}
        public async Task<Property> GetByIdAsync(int id)
        {
            return await Db.Set<Property>()
                .Include(p => p.PropertyType)
                .Include(p => p.Host)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Property> GetByIdWithAmenitiesAsync(int id)
        {
            return 
               await Db.Set<Property>()
                  .Include(p => p.Amenities)
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
                                    .Where(p=> p.HostId == hostId && !p.IsDeleted )
                                    .Include(p=> p.Images.Where(i=> i.IsCover))
                                    .Include(p=> p.Host)
                                    .ToListAsync();
        }

        public async Task<PaginatedResult<Property>> GetPageWithCoverAsync(int page, int pageSize, string userId)
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

        public async Task<PaginatedResult<Property>> GetNearestPageWithCoverAsync(IpLocation ipLocation, int page, int pageSize, double maxDistanceKm)
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
                            .Take(pageSize).OrderBy(p =>
                                6371 * Math.Acos(
                                    Math.Cos(Math.PI * ipLocation.Lat / 180) *
                                    Math.Cos(Math.PI * (double)p.Latitude / 180) *
                                    Math.Cos(Math.PI * ((double)p.Longitude - ipLocation.Lon) / 180) +
                                    Math.Sin(Math.PI * ipLocation.Lat / 180) *
                                    Math.Sin(Math.PI * (double)p.Latitude / 180)
                                ) )
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


        public async Task<PaginatedResult<Property>> GetFilteredPageAsync(PropertyFilterDto filterDto, string userId )
        {
            var query = Db.Properties
                //.Include(p => p.Reservations)
                .AsQueryable();

            if (!string.IsNullOrEmpty(filterDto.Country) )
                query = query.Where(p => p.Country.ToLower() == filterDto.Country.ToLower());
            if (filterDto.Longitude.HasValue && filterDto.Latitude.HasValue)
            {
                
                //double tolerance = 0.5; 
                //query = query.Where(p =>
                //    Math.Abs((double)p.Longitude -(double) filterDto.Longitude.Value) < tolerance &&
                //    Math.Abs((double)p.Latitude  -(double) filterDto.Latitude.Value) < tolerance);
                query = query.Where(p =>
                                6371 * Math.Acos(
                                    Math.Cos(Math.PI * (double)filterDto.Latitude.Value/ 180) *
                                    Math.Cos(Math.PI * (double)p.Latitude / 180) *
                                    Math.Cos(Math.PI * ((double)p.Longitude - (double)filterDto.Longitude.Value) / 180) +
                                    Math.Sin(Math.PI * (double)filterDto.Latitude.Value/ 180) *
                                    Math.Sin(Math.PI * (double)p.Latitude / 180)
                                ) < filterDto.maxDistanceKm
                            );

            }

            if (filterDto.GuestsCount.HasValue)
                query = query.Where(p => p.MaxGuests >= filterDto.GuestsCount.Value);

            // Filter by availability
            if (filterDto.StartDate.HasValue)
            {
                var start = filterDto.StartDate.Value.Date;
                var end = filterDto.EndDate?.Date ?? start.AddDays(1);

                query = query.Where(p =>
                    !p.Bookings.Any(r =>
                        r.CheckInDate <= end && r.CheckOutDate >= start));
            }

            var totalCount = await query.CountAsync();
            query =  query
                                    .Include(p => p.Images.Where(i => !i.IsDeleted && i.IsCover))
                                    .Skip((filterDto.Page - 1) * filterDto.PageSize)
                                    .Take(filterDto.PageSize);
            if (userId != null)
                query = query.Include(p => p.WishlistProperties.Where(wp => wp.Wishlist.UserId == userId));

            var pageData = await query.ToListAsync();


            return new() 
                    { 
                        Items= pageData,
                        MetaData = new()
                        {
                            Page = filterDto.Page,
                            PageSize = filterDto.PageSize,
                            Total = totalCount,
                        }

                    };



        }


    }
}
