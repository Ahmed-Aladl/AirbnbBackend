using System;
using System.Collections.Generic;
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
    }
}
