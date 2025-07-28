using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Models;
using Domain.Enums.Payment;

namespace Application.Interfaces.IRepositories
{
    public interface IPaymentRepository : IRepository<Payment, int>
    {
        Task<Payment?> GetByPaymentIntentIdAsync(string intentId);
        Task<Payment?> GetBySessionIdAsync(string sessionId);
        Task<Payment?> GetByIdAsync(int id);
        Task<Payment?> GetByBookingIdAsync(int bookingId);
        Task<List<Payment>> GetAllByBookingIdAsync(int bookingId);
        Task<List<Payment>> GetByStatusAsync(PaymentStatus status);
        Task<List<Payment>> GetByTransferStatusAsync(TransferStatus status);
        Task<List<Payment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<bool> UpdatePaymentStatusAsync(string paymentIntentId, PaymentStatus status);
        Task<List<Payment>> GetPendingPaymentsForHostAsync(string hostId);
        Task<List<Payment>> GetPendingTransferPayments();
        Task<List<Payment>> GetFailedTransferPayments();
        Task<decimal> GetTotalPlatformRevenue(DateTime? startDate = null, DateTime? endDate = null);
        Task<List<Payment>> GetHostPayments(string hostId, PaymentStatus? status = null);
    }
}