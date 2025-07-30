using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace Application.Interfaces.IRepositories
{


    public interface IPropertyViolationRepo: IRepository<PropertyViolation, int>


    {

        Task<List<PropertyViolation>> GetAllAsync();
        Task<PropertyViolation?> GetByIdAsync(int id);
        Task AddAsync(PropertyViolation entity);
        void Delete(PropertyViolation entity);
        void Update(PropertyViolation entity); 
        Task<List<PropertyViolation>> GetViolationsByUserIdAsync(string userId);
        Task<List<PropertyViolation>> GetViolationsByPropertyIdAsync(int propertyId); 




    }
}
