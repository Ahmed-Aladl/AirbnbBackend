using Application.DTOs.Calendar;
using Application.Interfaces;
using Application.Result;
using AutoMapper;
using Domain.Models;

namespace Application.Services
{
    public class CalendarService
    {
        public IUnitOfWork _unitOfWork;
        public IMapper _mapper;

        public CalendarService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<List<CalendarAvailabilityDto>>> GetPropertyCalendar(
            int propertyId,
            DateTime? startDate,
            DateTime? endDate
        )
        {
            try
            {
                startDate ??= DateTime.Today;
                endDate ??= DateTime.Today.AddMonths(3);

                var availabilities =
                    await _unitOfWork.CalendarAvailabilities.GetAvailabilityRangeAsync(
                        propertyId,
                        startDate.Value,
                        endDate.Value
                    );

                var result = _mapper.Map<List<CalendarAvailabilityDto>>(availabilities);
                return Result<List<CalendarAvailabilityDto>>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<List<CalendarAvailabilityDto>>.Fail(
                    $"Error retrieving calendar: {ex.Message}",
                    500
                );
            }
        }

        public async Task<Result<bool>> UpdatePropertyCalendar(
            int propertyId,
            List<CalendarUpdateDto> updates
        )
        {
            try
            {
                var availabilities = _mapper.Map<List<CalendarAvailability>>(updates);
                foreach (var availability in availabilities)
                {
                    availability.PropertyId = propertyId;
                }

                await _unitOfWork.CalendarAvailabilities.UpdateAvailabilityRangeAsync(
                    propertyId,
                    availabilities
                );
                await _unitOfWork.SaveChangesAsync();

                return Result<bool>.Success(true, message: "Calendar updated successfully");
            }
            catch (Exception ex)
            {
                return Result<bool>.Fail($"Error updating calendar: {ex.Message}", 500);
            }
        }

        public async Task<Result<bool>> IsPropertyBookableAsync(
            int propertyId,
            DateTime checkInDate,
            DateTime checkOutDate
        )
        {
            try
            {
                if (checkInDate >= checkOutDate)
                {
                    return Result<bool>.Fail("Check-in date must be before check-out date.", 400);
                }

                var propertyBookings = (await _unitOfWork.Bookings.GetAllAsync())
                    .Where(b => b.PropertyId == propertyId && !b.IsDeleted)
                    .ToList();

                bool isAvailable = propertyBookings.All(b =>
                    checkOutDate <= b.CheckInDate || checkInDate >= b.CheckOutDate
                );

                string message = isAvailable
                    ? "Property is available for booking."
                    : "Property is not available for the selected dates due to existing bookings.";

                return Result<bool>.Success(isAvailable, 200, message);
            }
            catch (Exception ex)
            {
                return Result<bool>.Fail($"An error occurred while checking property bookability: {ex.Message}", 500);
            }
        }
    }
}