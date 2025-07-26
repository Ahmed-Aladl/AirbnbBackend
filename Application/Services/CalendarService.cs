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
                var blockedDates = new List<DateTime>();
                foreach (var update in updates)
                {
                    // Only update the single date provided in CalendarUpdateDto
                    var date = update.Date.Date;
                    var existing = (await _unitOfWork.CalendarAvailabilities.GetAvailabilityRangeAsync(propertyId, date, date)).FirstOrDefault();
                    if (existing != null)
                    {
                        if (existing.IsBooked)
                        {
                            blockedDates.Add(date);
                            continue;
                        }
                        existing.IsAvailable = update.IsAvailable;
                        existing.Price = update.Price;
                        _unitOfWork.CalendarAvailabilities.Update(existing);
                    }
                    else
                    {
                        var ca = new CalendarAvailability
                        {
                            PropertyId = propertyId,
                            Date = date,
                            IsAvailable = update.IsAvailable,
                            IsBooked = false,
                            Price = update.Price
                        };
                        _unitOfWork.CalendarAvailabilities.Add(ca);
                    }
                }
                await _unitOfWork.SaveChangesAsync();

                if (blockedDates.Count > 0)
                {
                    var msg = $"Some dates were booked and could not be updated: {string.Join(", ", blockedDates.Select(d => d.ToString("yyyy-MM-dd")))}";
                    return Result<bool>.Success(true, message: msg);
                }
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