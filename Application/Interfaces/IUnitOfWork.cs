using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.Interfaces.IRepositories;

namespace Application.Interfaces
{
    public interface IUnitOfWork
    {
        IBookingRepo Bookings { get; }
        IPropertyRepository PropertyRepo { get; }
        ICalendarAvailabilityRepo CalendarAvailabilities { get; }

        Task<int> SaveChangesAsync();
        int SaveChanges();
    }
}