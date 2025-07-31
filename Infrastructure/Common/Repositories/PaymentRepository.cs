using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.PaymentDTOs;
using Application.Interfaces.IRepositories;
using Application.Shared;
using Domain.Enums.Payment;
using Domain.Models;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Common.Repositories
{
    public class PaymentRepository : Repository<Payment, int>, IPaymentRepository
    {
        public PaymentRepository(AirbnbContext db) : base(db) { }

        public async Task<Payment?> GetByBookingIdAsync(int bookingId)
        {
            return await Db.Payments.FirstOrDefaultAsync(p => p.BookingId == bookingId);
        }

        public async Task<Payment?> GetByIdAsync(int id)
        {
            return await Db.Payments.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Payment?> GetByPaymentIntentIdAsync(string intentId)
        {
            return await Db.Payments.FirstOrDefaultAsync(p => p.StripePaymentIntentId == intentId);
        }

        public async Task<Payment?> GetBySessionIdAsync(string sessionId)
        {
            return await Db.Payments.FirstOrDefaultAsync(p => p.StripeSessionId == sessionId);
        }

        public async Task<List<Payment>> GetAllByBookingIdAsync(int bookingId)
        {
            return await Db.Payments
                .Where(p => p.BookingId == bookingId)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<List<Payment>> GetByStatusAsync(PaymentStatus status)
        {
            return await Db.Payments
                .Where(p => p.Status == status)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<List<Payment>> GetByTransferStatusAsync(TransferStatus status)
        {
            return await Db.Payments
                .Where(p => p.TransferStatus == status)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<List<Payment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await Db.Payments
                .Where(p => p.PaymentDate >= startDate && p.PaymentDate <= endDate)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<bool> UpdatePaymentStatusAsync(string paymentIntentId, PaymentStatus status)
        {
            var payment = await GetByPaymentIntentIdAsync(paymentIntentId);
            if (payment != null)
            {
                payment.Status = status;
                payment.PaymentDate = DateTime.UtcNow;
                return true;
            }
            return false;
        }
        public async Task<List<Payment>> GetPendingPaymentsForHostAsync(string hostId)
        {
            Console.WriteLine($"Getting pending payments for HostId: {hostId}");

            var result = await Db.Payments
                .Include(p => p.Booking)
                .ThenInclude(b => b.Property)
                .Where(p =>
                    p.Status == PaymentStatus.Pending || p.Status == PaymentStatus.Processing || p.Status == PaymentStatus.Succeeded &&
                    (p.TransferStatus == TransferStatus.NotTransferred || p.TransferStatus == TransferStatus.PendingTransfer) &&
                    p.Booking.Property.HostId == hostId)
                .ToListAsync();

            Console.WriteLine($"Found {result.Count} pending payments");

            return result;
        }




        public async Task<List<Payment>> GetPendingTransferPayments()
        {
            return await Db.Payments
                .Include(p => p.Booking)
                .ThenInclude(b => b.Property)
                .Where(p => p.Status == PaymentStatus.Succeeded &&
                           p.TransferStatus == TransferStatus.PendingTransfer)
                .ToListAsync();
        }

        public async Task<List<Payment>> GetFailedTransferPayments()
        {
            return await Db.Payments
                .Include(p => p.Booking)
                .ThenInclude(b => b.Property)
                .Where(p => p.TransferStatus == TransferStatus.TransferFailed)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalPlatformRevenue(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = Db.Payments.Where(p => p.Status == PaymentStatus.Succeeded);

            if (startDate.HasValue)
                query = query.Where(p => p.PaymentDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(p => p.PaymentDate <= endDate.Value);

            return await query.SumAsync(p => p.PlatformFee);
        }

        public async Task<List<Payment>> GetHostPayments(string hostId, PaymentStatus? status = null)
        {
            var query = Db.Payments
             .Include(p => p.Booking)
             .ThenInclude(b => b.Property)
             .Where(p => p.Booking.Property.HostId == hostId);

             
            if (status.HasValue)
                query = query.Where(p => p.Status == status.Value);

            return await query
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }




        public async Task<PaginatedResult<AdminPaymentDTO>> GetAllPaymentsForAdminAsync(int page, int pageSize)
        {
            var query = Db.Payments
                .Include(p => p.Booking)
                    .ThenInclude(b => b.User) // Guest
                .Include(p => p.Booking)
                    .ThenInclude(b => b.Property)
                        .ThenInclude(prop => prop.Host); // Host

            var total = await query.CountAsync();

            var items = await query
                .OrderByDescending(p => p.PaymentDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new AdminPaymentDTO
                {
                    PaymentId = p.Id,
                    GuestName = p.Booking != null && p.Booking.User != null ? p.Booking.User.UserName : "N/A",
                    HostName = p.Booking != null && p.Booking.Property != null && p.Booking.Property.Host != null ? p.Booking.Property.Host.UserName : "N/A",
                    Amount = p.Amount/10,
                    PlatformFee = p.PlatformFee/10,
                    HostAmount = (p.Amount - p.PlatformFee) / 10,
                    PaymentStatus = p.Status.ToString(),
                    TransferStatus = p.TransferStatus.ToString(),
                    PaymentDate = p.PaymentDate,
                    HostAccountCompleted = !string.IsNullOrEmpty(p.Booking.Property.Host.StripeAccountId),

                    BookingId = p.BookingId,
                    HostId = p.Booking != null && p.Booking.Property != null && p.Booking.Property.Host != null
                    ? p.Booking.Property.Host.Id
                    : null,

                })
                .ToListAsync();

            return new PaginatedResult<AdminPaymentDTO>
            {
                Items = items,
                MetaData = new PaginationMetaData
                {
                    Page = page,
                    PageSize = pageSize,
                    Total = total
                }
            };
        }

    }
}