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
    public class PropertyRepository: Repository<Property,int>,IPropertyRepository
    {
        public PropertyRepository(AirbnbContext _context):base(_context) { }


        
    }
}
