using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.PropertyType;
using Domain.Models;
namespace Application.Interfaces.IRepositories
{
    public interface IPropertyTypeRepository : IRepository<PropertyType, int>
    {
        Task<List<PropertyType>> GetAllAsync();
        Task<PropertyType?> GetByIdAsync(int id);
        Task AddAsync(PropertyType entity);
        void Update(PropertyType entity);
        void Delete(PropertyType entity);



    }
}
