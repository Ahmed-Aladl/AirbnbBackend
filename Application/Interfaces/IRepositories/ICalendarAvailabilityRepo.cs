using Domain.Models;

namespace Application.Interfaces.IRepositories
{
    public interface ICalendarAvailabilityRepo : IRepository<CalendarAvailability, int>
    {
        Task<List<CalendarAvailability>> GetByPropertyIdAsync(int propertyId);
        Task<List<CalendarAvailability>> GetAvailabilityRangeAsync(
            int propertyId,
            DateTime startDate,
            DateTime endDate
        );
        Task<bool> IsPropertyAvailableAsync(int propertyId, DateTime date);
        Task UpdateAvailabilityRangeAsync(
            int propertyId,
            List<CalendarAvailability> availabilities
        );
    }
}
