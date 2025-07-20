using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.IRepositories;
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

        public async Task<List<Payment>> GetByStatusAsync(string status)
        {
            return await Db.Payments
                .Where(p => p.Status == status)
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

        public async Task<bool> UpdatePaymentStatusAsync(string paymentIntentId, string status)
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
    }
}