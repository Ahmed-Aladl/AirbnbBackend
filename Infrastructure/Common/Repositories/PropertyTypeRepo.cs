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
    public class PropertyTypeRepo : Repository<PropertyType, int>, IPropertyTypeRepository
    {

        private readonly AirbnbContext Db;

        public PropertyTypeRepo(AirbnbContext db)
            : base(db)
        {
            Db = db;
        }

        public void Add(PropertyType entity)
        {
            Db.propertyTypes.Add(entity);
        }

        public async Task AddAsync(PropertyType entity)
        {
            await Db.propertyTypes.AddAsync(entity);
        }

        public async Task<List<PropertyType>> GetAllAsync()
        {
            return await Db.propertyTypes.ToListAsync();
        }


        public async Task<PropertyType?> GetByIdAsync(int id)
        {
            return await Db.propertyTypes.FindAsync(id);
        }


        public async Task propertyTypes(Booking entity)
        {
            throw new NotImplementedException("The method 'propertyTypes' is not implemented.");
        }

        public void Update(PropertyType entity)
        {
            Db.propertyTypes.Update(entity);
        }

        public void Delete(PropertyType entity)
        {
            Db.propertyTypes.Remove(entity);
        }

        public Task<PropertyType?> GetByBookingIdAsync(int BookingId)
        {
            throw new NotImplementedException();
        }

        public void GetByIdAsync(PropertyType id)
        {
            throw new NotImplementedException();
        }



    }
}
 
