using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace Application.Interfaces.IRepositories
{
    public interface IPropertyImageRepository : IRepository<PropertyImage, int> 
    {
        List<PropertyImage> GetImagesByPropertyId(int propertyId);
        Task<List<PropertyImage>> GetRangeAsync(int[] imgIds,int propertyId);

        Task DeleteRangeAsync(List<PropertyImage> images);
        Task DeleteAsync(PropertyImage img);
    }
}
