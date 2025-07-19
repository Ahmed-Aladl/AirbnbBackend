using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.IRepositories;
using Domain.Models;
using Infrastructure.Contexts;

namespace Infrastructure.Common.Repositories
{
    class PropertyImageRepository : Repository<PropertyImage, int>, IPropertyImageRepository
    {
        public PropertyImageRepository(AirbnbContext _db)
            : base(_db) { }
    }
}
