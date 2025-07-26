using Domain.Models;

namespace Application.Interfaces.IRepositories;

public interface IAmenityRepo : IRepository<Amenity, int>
{
    Task<Amenity> GetAmenityByIdAsync(int amenityId);
    Task<IEnumerable<Amenity>> GetAmenitiesByPropertyIdAsync(int propertyId);
    Task<IEnumerable<Amenity>> GetAllAmenitiesAsync();
    //Task<IEnumerable<Amenity>> GetByIdAsync(int propertyId);

    Task Assign(PropertyAmenity propertyAmenity);
    Task RemoveFromProperty(PropertyAmenity propertyAmenity);
}
