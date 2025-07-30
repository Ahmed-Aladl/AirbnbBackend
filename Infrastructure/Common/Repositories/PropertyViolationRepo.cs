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
    public class PropertyViolationRepo : Repository<PropertyViolation, int>, IPropertyViolationRepo
    {

        private readonly AirbnbContext _db;

        public PropertyViolationRepo(AirbnbContext db) :base(db)
        
        {
            _db = db;

        }

        public async Task<List<PropertyViolation>> GetAllAsync()
        {
            return await _db.PropertyViolations
                               .Include(p => p.Property).Include(p => p.User)
                               .ToListAsync();
        }

        public async Task<PropertyViolation?> GetByIdAsync(int id)
        {
            return await _db.PropertyViolations
          .Include(p => p.Property)
          .Include(p => p.User) 
          .FirstOrDefaultAsync(p => p.Id == id);
        }




        public async  Task<List<PropertyViolation>> GetViolationsByPropertyIdAsync(int propertyId)
        {
            return await _db.PropertyViolations
                  .Include(p => p.Property)
                  .Include(p => p.User)
                  .Where(  p => p.PropertyId == propertyId && !p.IsDeleted)
                  .ToListAsync();
        }

        public async Task<List<PropertyViolation>> GetViolationsByUserIdAsync(string userId)
        {
            return await _db.PropertyViolations
                    .Include(b => b.Property)
                    .Where(b => b.UserId == userId && !b.IsDeleted)
                    .ToListAsync();


        }

        public async  Task AddAsync(PropertyViolation entity)
        {
            await _db.PropertyViolations.AddAsync(entity);
        }

        //public override void Update(PropertyViolation entity)
        //{
        //    _db.PropertyViolations.Update(entity);
        //}
    }
}



