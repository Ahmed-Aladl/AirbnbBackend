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

        public async Task<Result<List<BookingDetailsDTO>>> GetAllBookingsAsync()
        {
            try
            {
                var bookings = await uow.Bookings.GetAllAsync();

                var bookingDtos = bookings.Select(b => new BookingDetailsDTO
                {
                    Id = b.Id,
                    UserId = b.UserId,
                    CheckInDate = b.CheckInDate,
                    CheckOutDate = b.CheckOutDate,
                    NumberOfGuests = b.NumberOfGuests,
                    TotalPrice = b.TotalPrice,
                    BookingStatus = b.BookingStatus.ToString(),
                    PropertyId = b.Property?.Id ?? 0,
                    PropertyTitle = b.Property?.Title ?? "",
                    City = b.Property?.City ?? "",
                    Country = b.Property?.Country ?? ""
                }).ToList();

                return Result<List<BookingDetailsDTO>>.Success(bookingDtos);
            }
            catch (Exception)
            {
                return Result<List<BookingDetailsDTO>>.Fail("Failed to retrieve bookings.", 500);
            }
        }

        public async Task<Result<BookingDetailsDTO>> GetBookingByIdAsync(int id)
        {
            try
            {
                var booking = await uow.Bookings.GetByIdAsync(id);
                if (booking == null)
                    return Result<BookingDetailsDTO>.Fail("Booking not found", 404);

                var dto = new BookingDetailsDTO
                {
                    Id = booking.Id,
                    UserId = booking.UserId,
                    CheckInDate = booking.CheckInDate,
                    CheckOutDate = booking.CheckOutDate,
                    NumberOfGuests = booking.NumberOfGuests,
                    TotalPrice = booking.TotalPrice,
                    BookingStatus = booking.BookingStatus.ToString(),
                    PropertyId = booking.Property?.Id ?? 0,
                    PropertyTitle = booking.Property?.Title ?? "",
                    City = booking.Property?.City ?? "",
                    Country = booking.Property?.Country ?? ""
                };

                return Result<BookingDetailsDTO>.Success(dto);
            }
            catch (Exception)
            {
                return Result<BookingDetailsDTO>.Fail("Failed to retrieve booking.", 500);
            }
        }

        public async Task<Result<List<BookingDetailsDTO>>> GetBookingByUserIdAsync(string userId)
        {
            try
            {
                var bookings = await uow.Bookings.GetBookingByUserIdAsync(userId);

                if (bookings == null || !bookings.Any())
                    return Result<List<BookingDetailsDTO>>.Fail("No bookings found for this user.", 404);

                var bookingDtos = bookings.Select(b => new BookingDetailsDTO
                {
                    Id = b.Id,
                    UserId = b.UserId,
                    CheckInDate = b.CheckInDate,
                    CheckOutDate = b.CheckOutDate,
                    NumberOfGuests = b.NumberOfGuests,
                    TotalPrice = b.TotalPrice,
                    BookingStatus = b.BookingStatus.ToString(),
                    PropertyId = b.Property.Id,
                    PropertyTitle = b.Property.Title,
                    City = b.Property.City,

                    Country = b.Property.Country
                }).ToList();

                return Result<List<BookingDetailsDTO>>.Success(bookingDtos);
            } 
            catch (Exception)
            {
                return Result<List<BookingDetailsDTO>>.Fail("Failed to retrieve bookings.", 500);
            }
        }

        public async Task<Result<List<BookingDetailsDTO>>> GetBookingsByPropertyIdAsync(int propertyId)
        {
            try
            {

                var bookings = await uow.Bookings.GetBookingByPropertyIdAsync(propertyId);

                if (bookings == null || !bookings.Any())
                    return Result<List<BookingDetailsDTO>>.Fail("No bookings found for this property.", 404);

                var bookingDtos = bookings.Select(b => new BookingDetailsDTO
                {
                    Id = b.Id,
                    UserId = b.UserId,
                    FirstName = b.User?.FirstName,
                    LastName = b.User?.LastName,
                    PhoneNumber = b.User?.PhoneNumber,
                    UserCountry = b.User?.Country,

                    CheckInDate = b.CheckInDate,
                    CheckOutDate = b.CheckOutDate,
                    NumberOfGuests = b.NumberOfGuests,
                    TotalPrice = b.TotalPrice,
                    BookingStatus = b.BookingStatus.ToString(),

                    PropertyId = b.Property.Id,
                    PropertyTitle = b.Property.Title,
                    City = b.Property.City,
                    Country = b.Property.Country
                }).ToList();

                return Result<List<BookingDetailsDTO>>.Success(bookingDtos);
            }
            catch (Exception)
            {
                return Result<List<BookingDetailsDTO>>.Fail("Failed to retrieve bookings for this property.", 500);
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

        public async Task<Result<bool>> CheckClientAndPropertyAsync(int propId, string userId)
        {
            try
            {
                var user = uow.UserRepo.GetById(userId);
                if (user == null)
                {
                    return Result<bool>.Fail("User does not exist.", 404);
                }

                var property = await uow.PropertyRepo.GetByIdAsync(propId);
                if (property == null)
                {
                    return Result<bool>.Fail("Property does not exist.", 404);
                }

                if (property.HostId == userId) 
                {
                    return Result<bool>.Fail("You cannot reserve your own property.", 403);
                }

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Fail("Failed to check property.", 500);
            }
        }

        public async Task<Result<bool>> CheckAvailabilityAsync(
            string userId,
            int propertyId,
            DateTime checkInDate,
            DateTime checkOutDate
        )
        {
            try
            {
                var checkResult = await CheckClientAndPropertyAsync(propertyId, userId);

                if (!checkResult.IsSuccess)
                    return Result<bool>.Fail(checkResult.Message, checkResult.StatusCode ?? 500);

                var bookings = await uow.Bookings.GetAllAsync();

                var propertyBookings = bookings
                    .Where(b => b.PropertyId == propertyId && !b.IsDeleted)
                    .ToList();

                bool isAvailable = propertyBookings.All(b =>
                    checkOutDate <= b.CheckInDate || checkInDate >= b.CheckOutDate
                );

                string message = isAvailable
                    ? "Property is available"
                    : "Property is not available";

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
                var availabilityResult = await CheckAvailabilityAsync(
                    dto.UserId,
                    dto.PropertyId,
                    dto.CheckInDate,
                    dto.CheckOutDate
                );

                if (!availabilityResult.IsSuccess)
                    return Result<bool>.Fail(
                        availabilityResult.Message,
                        availabilityResult.StatusCode ?? 500
                    );

                if (!availabilityResult.Data)
                    return Result<bool>.Fail(
                        "The property is not available for the selected dates.",
                        400
                    );

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
                    IsDeleted = false,
                };

                await uow.Bookings.AddAsync(booking);
                await uow.SaveChangesAsync();

                return Result<bool>.Success(true, 201, "Booking created successfully.");
            }
            catch (Exception ex)
            {
                return Result<bool>.Fail(
                    "An unexpected error occurred while creating the booking.",
                    500
                );
            }
        }
    }
}
