using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.BookingDTOS;
using Application.Interfaces;
using Application.Result;
using Domain.Enums.Booking;
using Domain.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.VisualBasic;

namespace Application.Services
{
    public class BookingService
    {

        private readonly IUnitOfWork uow;

        public BookingService(IUnitOfWork unitOfWork )
        { 

            this.uow = unitOfWork;  

        }

        public Result<List<Booking>> GetAllBookings()
        {
            var bookings = uow.Bookings.GetAll();
            return Result<List<Booking>>.Success(bookings);
        }




        public Result<Booking> GetBookingById(int id)
        {
            var booking = uow.Bookings.GetById(id);
            if (booking == null)
                return Result<Booking>.Fail("Booking not found", 404);

            return Result<Booking>.Success(booking);

        }



        public Result<string> DeleteBooking(int id)
        {
            var booking = uow.Bookings.GetById(id);
            if (booking == null)
                return Result<string>.Fail("Booking not found", 404);

            uow.Bookings.Delete(booking);
            uow.SaveChanges();
            return Result<string>.Success("Booking deleted successfully");
        } 



     public Result<bool> CheckClientAndProperty(/*string userId,*/ int propId)
{
    // var userExists = uow.UserRepo.GetById(userId) != null;
    var propertyExists = uow.PropertyRepo.GetById(propId) != null;

    if (!propertyExists)
        return Result<bool>.Fail("Property does not exist", 404);

    return Result<bool>.Success(true);
}


        public Result<bool> CheckAvailability(string userId, int propertyId, DateTime checkInDate, DateTime checkOutDate)
        {
            var checkResult = CheckClientAndProperty(/* userId, */ propertyId);

            if (!checkResult.IsSuccess)
                return Result<bool>.Fail(checkResult.Message, checkResult.StatusCode ?? 500); 

            var bookings = uow.Bookings 
                .GetAll()
                .Where(b => b.PropertyId == propertyId && !b.IsDeleted)
                .ToList();

            bool isAvailable = bookings.All(b =>
                checkOutDate <= b.CheckInDate || checkInDate >= b.CheckOutDate);

            string message = isAvailable ? "Property is available" : "Property is not available";
            return Result<bool>.Success(isAvailable, 200, message);
        }


        public Result<bool> CreateBooking(BookingDTO dto)
        {
            var availabilityResult = CheckAvailability(dto.UserId, dto.PropertyId, dto.CheckInDate, dto.CheckOutDate);

            if (!availabilityResult.IsSuccess)
                return Result<bool>.Fail(availabilityResult.Message, availabilityResult.StatusCode ?? 500);

            if (!availabilityResult.Data)
                return Result<bool>.Fail("The property is not available for the selected dates.", 400);

            var property = uow.PropertyRepo.GetById(dto.PropertyId);
            if (property == null)
                return Result<bool>.Fail("Property not found.", 404);

            var totalDays = (dto.CheckOutDate - dto.CheckInDate).Days;
            if (totalDays <= 0)
                return Result<bool>.Fail("Invalid check-in and check-out dates.", 400);

            var totalPrice = property.PricePerNight * totalDays;

            var booking = new Booking
            {
                UserId = dto.UserId,
                PropertyId = dto.PropertyId,
                CheckInDate = dto.CheckInDate,
                CheckOutDate = dto.CheckOutDate,
                NumberOfGuests = dto.NumberOfGuests,
                TotalPrice = totalPrice,
                BookingStatus = BookingStatus.Pending,
                IsDeleted = false
            };

            uow.Bookings.Add(booking);
            uow.SaveChanges();

            return Result<bool>.Success(true, 201, "Booking created successfully.");
        }










    }
}
