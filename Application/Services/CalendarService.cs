using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<Result<List<CalendarAvailabilityDto>>> GetPropertyCalendar(int propertyId, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                startDate ??= DateTime.Today;
                endDate ??= DateTime.Today.AddMonths(3);

                var availabilities = await _unitOfWork.CalendarAvailabilities
                    .GetAvailabilityRangeAsync(propertyId, startDate.Value, endDate.Value);

                var result = _mapper.Map<List<CalendarAvailabilityDto>>(availabilities);
                return Result<List<CalendarAvailabilityDto>>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<List<CalendarAvailabilityDto>>.Fail($"Error retrieving calendar: {ex.Message}", 500);
            }
        }

        public async Task<Result<bool>> UpdatePropertyCalendar(int propertyId, List<CalendarUpdateDto> updates)
        {
            try
            {
                var availabilities = _mapper.Map<List<CalendarAvailability>>(updates);
                foreach (var availability in availabilities)
                {
                    availability.PropertyId = propertyId;
                }

                await _unitOfWork.CalendarAvailabilities.UpdateAvailabilityRangeAsync(propertyId, availabilities);
                await _unitOfWork.SaveChangesAsync();

                return Result<bool>.Success(true, message: "Calendar updated successfully");
            }
            catch (Exception ex)
            {
                return Result<bool>.Fail($"Error updating calendar: {ex.Message}", 500);
            }
        }

        public async Task<Result<AvailabilityCheckDto>> CheckAvailability(int propertyId, DateTime startDate, DateTime endDate, int guests)
        {
            try
            {
                var property = await _unitOfWork.PropertyRepo.GetByIdAsync(propertyId);
                if (property == null)
                    return Result<AvailabilityCheckDto>.Fail("Property not found", 404);

                if (guests > property.MaxGuests)
                    return Result<AvailabilityCheckDto>.Success(new AvailabilityCheckDto
                    {
                        IsAvailable = false,
                        Message = $"Property can only accommodate {property.MaxGuests} guests"
                    }, statusCode: 200);

                var dates = Enumerable.Range(0, (endDate - startDate).Days + 1)
                    .Select(offset => startDate.AddDays(offset))
                    .ToList();

                var availabilities = await _unitOfWork.CalendarAvailabilities
                    .GetAvailabilityRangeAsync(propertyId, startDate, endDate);

                var unavailableDates = dates
                    .Where(date =>
                        !availabilities.Any(a => a.Date == date && a.IsAvailable) ||
                        date < DateTime.Today)
                    .ToList();

                var isAvailable = !unavailableDates.Any();
                decimal totalPrice = availabilities
                    .Where(a => dates.Contains(a.Date))
                    .Sum(a => a.Price);

                return Result<AvailabilityCheckDto>.Success(new AvailabilityCheckDto
                {
                    IsAvailable = isAvailable,
                    UnavailableDates = unavailableDates,
                    TotalPrice = totalPrice,
                    Message = isAvailable ? "Property is available" : "Property is not available for selected dates"
                });
            }
            catch (Exception ex)
            {
                return Result<AvailabilityCheckDto>.Fail($"Error checking availability: {ex.Message}", 500);
            }
        }
    }
}