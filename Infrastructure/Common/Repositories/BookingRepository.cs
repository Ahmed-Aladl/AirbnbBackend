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
    public class BookingRepository :Repository<Booking,int> ,IBookingRepo 
    {


        public BookingRepository (AirbnbContext _db ):base( _db ) 
        {

           

        }
        public void Add(Booking entity)
        {

            base.Add( entity );
        }

        public void Delete(Booking entity)
        {
            base.Delete( entity ); 
        }

        public List<Booking> GetAll()
        {
             return base.GetAll();
        }

        public Booking GetById(int id)
        {
            return base.GetById(id); 
        }

        public void Update(Booking entity)
        {
           base.Update( entity );

        }

    }
}
