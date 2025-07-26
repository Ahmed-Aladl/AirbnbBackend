using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.IRepositories;
using Domain.Models;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Common.Repositories;

public class AmenityRepo : Repository<Amenity, int>, IAmenityRepo
{
    private readonly AirbnbContext _context;

    public AmenityRepo(AirbnbContext context)
        : base(context)
    {
        _context = context;
    }

    // This method retrieves all amenities related to a specific property ID.
    public async Task<IEnumerable<Amenity>> GetAmenitiesByPropertyIdAsync(int propertyId)
    {
        return await _context
            .PropertyAmenities.Where(pa => pa.PropertyId == propertyId)
            .Include(pa => pa.Amenity)
            .Select(pa => pa.Amenity)
            .OrderBy(a => a.AmenityName)
            .ToListAsync();
    }

    // This method retrieves amenities by their ID.
    public async Task<IEnumerable<Amenity>> GetByIdAsync(int amenityId)
    {
        return await _context.Amenities.Where(a => a.Id == amenityId).ToListAsync();
    }

    public async Task<Amenity?> GetAmenityByIdAsync(int amenityId)
    {
        return await _context.Amenities.FirstOrDefaultAsync(a => a.Id == amenityId);
    }

    //get all amenities to host to select from when creating a property
    public async Task<IEnumerable<Amenity>> GetAllAmenitiesAsync()
    {
        return await _context.Amenities.OrderBy(a => a.Id).ToListAsync();
    }

    public async Task Assign(PropertyAmenity propertyAmenity)
    {
        await Db.PropertyAmenities.AddAsync(propertyAmenity);
    }

    public async Task RemoveFromProperty(PropertyAmenity propertyAmenity)
    {
        Db.PropertyAmenities.Remove(propertyAmenity);
    }

    public Task RemoveFromProperty(int amenityId)
    {
        throw new NotImplementedException();
    }
}
