using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Models;

namespace Application.Services
{
    public class BookingService
    {

        private readonly IUnitOfWork uow;

        public BookingService(IUnitOfWork unitOfWork )
        { 

            this.uow = unitOfWork;  

        }

        public List<Booking> GetAllBookings()
        {
            return uow.Bookings.GetAll();
        }

        public Booking GetBookingById(int id)
        {
            return uow.Bookings.GetById(id);
        }

        
        public void DeleteBooking(int id)
        {
            var booking = uow.Bookings.GetById(id);
            if (booking != null)
            {
                uow.Bookings.Delete(booking);
                uow.SaveChanges();
            }
        }





    }
}
