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
        IUserRepository UserRepo { get; }
        IBookingRepo Bookings { get; }
        IPropertyRepository PropertyRepo { get; }
        IPropertyImageRepository PropertyImageRepo { get; }
        ICalendarAvailabilityRepo CalendarAvailabilities { get; }
        IAmenityRepo AmenitiesRepo { get; }
        IWishlistRepository Wishlist { get; }
        IPaymentRepository paymentRepository { get; }
        IReviewRepo ReviewRepo { get; }
        IPropertyTypeRepository propertyType { get; }
        INotificationRepository NotificationRepo { get; }
        IHostReviewRepo HostReviewRepo { get; }
        Task<int> SaveChangesAsync();
        int SaveChanges();
    }
}
