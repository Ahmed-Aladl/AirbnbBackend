using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace Application.Interfaces.IRepositories
{
    public interface IPropertyRepository : IRepository<Property, int>
    {
        Task<Property> GetByIdAsync(int id);
        Task<Property> GetByIdWithCoverAsync(int id);
        Task<Property> GetByIdWithCalendarAsync(int id);
        Task<List<Property>> GetByHostIdAsync(string hostId);
        Task<List<Property>> GetByHostIdWithCoverAsync(string hostId);
    }
}
