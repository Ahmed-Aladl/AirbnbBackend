using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.Interfaces.IRepositories;
using Infrastructure.Common;
using Infrastructure.Common.Repositories;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Common
{
    public class UnitOfWork : IUnitOfWork
    {
        private IPropertyRepository _propertyRepo;
        private IPropertyImageRepository _propertyImageRepo;
        private INotificationRepository _notificationRepo;
        private IUserRepository _userRepo;
        private IBookingRepo _bookingRepo;
        private IUserOtpRepository _userOtps;
        private IReviewRepo _reviewRepo;
        private IHostReviewRepo _hostReviewRepo;

        IPropertyTypeRepository _iPropertyTypeRepo;
        public UnitOfWork(AirbnbContext _context)
        {
            Context = _context;
        }
        public AirbnbContext Context { get; }
        public IBookingRepo Bookings
        {
            get
            {
                if (_bookingRepo != null)
                    return _bookingRepo;
                _bookingRepo = new BookingRepository(Context);
                return _bookingRepo;
            }
        }
        public IPropertyRepository PropertyRepo => _propertyRepo ?? new PropertyRepository(Context);
        public INotificationRepository NotificationRepo =>
            _notificationRepo ?? new NotificationRepository(Context);
        public IPropertyImageRepository PropertyImageRepo =>
            _propertyImageRepo ?? new PropertyImageRepository(Context);
        public IUserRepository UserRepo => _userRepo ?? new UserRepository(Context);
        public IHostReviewRepo HostReviewRepo => _hostReviewRepo ?? new HostReplyRepo(Context);
        public IUserOtpRepository UserOtps => _userOtps ?? new UserOtpRepository(Context);
        public IWishlistRepository _wishlistRepo;
        public IWishlistRepository Wishlist => _wishlistRepo ??= new WishlistRepository(Context);
        public IReviewRepo ReviewRepo => _reviewRepo ??= new ReviewRepo(Context);
        public IPropertyTypeRepository propertyType => _iPropertyTypeRepo ??= new PropertyTypeRepo(Context);

        private IPaymentRepository _paymentRepository;
        public IPaymentRepository paymentRepository=>_paymentRepository??=new PaymentRepository(Context);

        public int SaveChanges()
        {
            return Context.SaveChanges();
        }
        public async Task<int> SaveChangesAsync()
        {
            return await Context.SaveChangesAsync();
        }
        // Calendar
        private ICalendarAvailabilityRepo _calendarAvailabilityRepo;
        public ICalendarAvailabilityRepo CalendarAvailabilities =>
            _calendarAvailabilityRepo ??= new CalendarAvailabilityRepository(Context);
        // Amenities
        private IAmenityRepo _amenityRepo;
        public IAmenityRepo AmenitiesRepo => _amenityRepo ??= new AmenityRepo(Context);
    }
}
