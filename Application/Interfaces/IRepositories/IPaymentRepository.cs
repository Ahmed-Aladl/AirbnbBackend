using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace Application.Interfaces.IRepositories
{
    public interface IPaymentRepository : IRepository<Payment, int>
    {
        Task<Payment> GetByPaymentIntentIdAsync(string intentId);

        Task<Payment?> GetBySessionIdAsync(string sessionId);
        Task<Payment?> GetByIdAsync(int id);
        Task<Payment?> GetByBookingIdAsync(int bookingId);


        Task<List<Payment>> GetAllByBookingIdAsync(int bookingId);
        Task<List<Payment>> GetByStatusAsync(string status);
        Task<List<Payment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<bool> UpdatePaymentStatusAsync(string paymentIntentId, string status);
    }
}
