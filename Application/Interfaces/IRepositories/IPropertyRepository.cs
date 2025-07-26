using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.PropertyDTOS;
using Application.Shared;
using Domain.Models;

namespace Application.Interfaces.IRepositories
{
    public interface IPropertyRepository : IRepository<Property, int>
    {
        Task<PaginatedResult<Property>> GetPageWithCoverAsync(int page, int pageSize,string userId);
        Task<PaginatedResult<Property>> GetFilteredPageAsync(PropertyFilterDto filterDto,string userId );
        Task<PaginatedResult<Property>> GetNearestPageWithCoverAsync(IpLocation ipLocation, int page, int pageSize, double maxDistanceKm);
        Task<Property> GetByIdAsync(int id);
        Task<Property> GetByIdWithAmenitiesAsync(int id);
        Task<Property> GetByIdWithCoverAsync(int id);
        Task<Property> GetByIdWithCalendarAsync(int id);
        Task<List<Property>> GetByHostIdAsync(string hostId);
        Task<List<Property>> GetByHostIdWithCoverAsync(string hostId);

    }
}
