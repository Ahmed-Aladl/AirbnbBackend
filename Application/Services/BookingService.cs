using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.BookingDTOS;
using Application.Interfaces;
using Application.Result;
using Domain.Enums.Booking;
using Domain.Models;

namespace Application.Services
{
    public class BookingService
    {
        private readonly IUnitOfWork uow;

        public BookingService(IUnitOfWork unitOfWork)
        {
            this.uow = unitOfWork;
        }

        public async Task<Result<List<Booking>>> GetAllBookingsAsync()
        {
            try
            {
                var bookings = await uow.Bookings.GetAllAsync();
                return Result<List<Booking>>.Success(bookings);
            }
            catch (Exception ex)
            {
                // Optionally log ex
                return Result<List<Booking>>.Fail("Failed to retrieve bookings.", 500);
            }
        }

        public async Task<Result<Booking>> GetBookingByIdAsync(int id)
        {
            try
            {
                var booking = await uow.Bookings.GetByIdAsync(id);
                if (booking == null)
                    return Result<Booking>.Fail("Booking not found", 404);

                return Result<Booking>.Success(booking);
            }
            catch (Exception ex)
            {
                return Result<Booking>.Fail("Failed to retrieve booking.", 500);
            }
        }

        public async Task<Result<string>> DeleteBookingAsync(int id)
        {
            try
            {
                var booking = await uow.Bookings.GetByIdAsync(id);
                if (booking == null)
                    return Result<string>.Fail("Booking not found", 404);

                uow.Bookings.Delete(booking);
                await uow.SaveChangesAsync();

                return Result<string>.Success("Booking deleted successfully");
            }
            catch (Exception ex)
            {
                return Result<string>.Fail("Failed to delete booking.", 500);
            }
        }

        public async Task<Result<bool>> CheckClientAndPropertyAsync(int propId,string userid)
        {
            try
            {
                var hostExists = uow.UserRepo.GetById(userid) !=null;

                var propertyExists = await uow.PropertyRepo.GetByIdAsync(propId) != null; 



                if (!propertyExists)
                    return Result<bool>.Fail("Property does not exist", 404);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Fail("Failed to check property.", 500);
            }
        }

        public async Task<Result<bool>> CheckAvailabilityAsync(string userId, int propertyId, DateTime checkInDate, DateTime checkOutDate)
        {
            try
            {
                var checkResult = await CheckClientAndPropertyAsync(propertyId, userId);

                if (!checkResult.IsSuccess)
                    return Result<bool>.Fail(checkResult.Message, checkResult.StatusCode ?? 500);

                var bookings = await uow.Bookings
                    .GetAllAsync();

                var propertyBookings = bookings
                    .Where(b => b.PropertyId == propertyId && !b.IsDeleted)
                    .ToList();

                bool isAvailable = propertyBookings.All(b =>
                    checkOutDate <= b.CheckInDate || checkInDate >= b.CheckOutDate);

                string message = isAvailable ? "Property is available" : "Property is not available";

                return Result<bool>.Success(isAvailable, 200, message);
            }
            catch (Exception ex)
            {
                return Result<bool>.Fail("Failed to check availability.", 500);
            }
        }

        public async Task<Result<bool>> CreateBookingAsync(BookingDTO dto)
        {
            try
            {
                var availabilityResult = await CheckAvailabilityAsync(dto.UserId, dto.PropertyId, dto.CheckInDate, dto.CheckOutDate);

                if (!availabilityResult.IsSuccess)
                    return Result<bool>.Fail(availabilityResult.Message, availabilityResult.StatusCode ?? 500);

                if (!availabilityResult.Data)
                    return Result<bool>.Fail("The property is not available for the selected dates.", 400);

                var property = await uow.PropertyRepo.GetByIdAsync(dto.PropertyId);
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

                await uow.Bookings.AddAsync(booking);
                await uow.SaveChangesAsync();

                return Result<bool>.Success(true, 201, "Booking created successfully.");
            }
            catch (Exception ex)
            {
                return Result<bool>.Fail("An unexpected error occurred while creating the booking.", 500);
            }
        }
    }
}
