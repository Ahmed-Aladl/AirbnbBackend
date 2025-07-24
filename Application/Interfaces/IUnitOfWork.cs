using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.Interfaces.IRepositories;
using Application.Interfaces.IRepositories.Chat;
using Application.Interfaces.Services;
using Application.Services;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace Application.Interfaces
{
    public interface IUnitOfWork
    {
        IChatSessionRepository ChatSessionRepo{ get; }
        IMessageRepository MessageRepo{ get; }
        IMessageReadStatusRepository MessageReadRepo { get; }
        IReservationRequestRepository ReservationRepo { get; }
        IMessageReactionRepository ReactionRepo { get; }

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
