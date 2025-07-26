using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.IRepositories;
using Domain.Models;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Common.Repositories
{
    class PropertyImageRepository : Repository<PropertyImage, int>, IPropertyImageRepository
    {
        private readonly AirbnbContext _context;

        public PropertyImageRepository(AirbnbContext context)
            : base(context)
        {
            _context = context;
        }
        public List<PropertyImage> GetImagesByPropertyId(int propertyId)
        {
            return _context.PropertyImages
                           .Where(img => img.PropertyId == propertyId && !img.IsDeleted)
                           .ToList();
        }
        public async Task DeleteAsync(PropertyImage img)
        {
            img.IsDeleted = true;
            Db.PropertyImages.Update(img);
        }

        public async Task DeleteRangeAsync(List<PropertyImage> images) 
        {
            foreach (var img in images) 
            {
                img.IsDeleted = true;
            }
            Db.PropertyImages.UpdateRange(images);
        }

        public async Task<List<PropertyImage>> GetRangeAsync(int[] imgIds, int propertyId)
        {
            return await Db.PropertyImages
                            .Where(img => imgIds.Contains(img.Id) && img.PropertyId == propertyId)
                            .ToListAsync();
        }

    }
}
