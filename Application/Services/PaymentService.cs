using System.Threading.Tasks;
using Application.DTOs.PaymentDTOs;
using Application.Interfaces;
using Application.Interfaces.IRepositories;
using Application.Shared;
using AutoMapper;
using Domain.Enums.Payment;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class PaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration config, ILogger<PaymentService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _config = config;
            _logger = logger;
        }

        public async Task<PaymentDTO?> GetPaymentByIntentIdAsync(string intentId)
        {
            var payment = await _unitOfWork.paymentRepository.GetByPaymentIntentIdAsync(intentId);
            return payment != null ? _mapper.Map<PaymentDTO>(payment) : null;
        }

        public async Task<PaymentDTO?> GetPaymentByBookingIdAsync(int bookingId)
        {
            var payment = await _unitOfWork.paymentRepository.GetByBookingIdAsync(bookingId);
            return payment != null ? _mapper.Map<PaymentDTO>(payment) : null;
        }

        public async Task<bool> UpdatePaymentStatusAsync(string paymentIntentId, PaymentStatus status)
        {
            var payment = await _unitOfWork.paymentRepository.GetByPaymentIntentIdAsync(paymentIntentId);
            if (payment != null)
            {
                payment.Status = status;
                payment.PaymentDate = DateTime.UtcNow;
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<PaymentDTO>> GetPendingPaymentsForHostAsync(string userId)
        {
            var payments = await _unitOfWork.paymentRepository.GetPendingPaymentsForHostAsync(userId);
            return payments.Select(p => _mapper.Map<PaymentDTO>(p)).ToList();
        }

        public async Task<List<PaymentDTO>> GetHostPaymentsAsync(string hostId, PaymentStatus? status = null)
        {
            var payments = await _unitOfWork.paymentRepository.GetHostPayments(hostId, status);
            return payments.Select(p => _mapper.Map<PaymentDTO>(p)).ToList();
        }

        public async Task<List<PaymentDTO>> GetPaymentsByStatusAsync(PaymentStatus status)
        {
            var payments = await _unitOfWork.paymentRepository.GetByStatusAsync(status);
            return payments.Select(p => _mapper.Map<PaymentDTO>(p)).ToList();
        }

        public async Task<List<PaymentDTO>> GetFailedTransferPaymentsAsync()
        {
            var payments = await _unitOfWork.paymentRepository.GetFailedTransferPayments();
            return payments.Select(p => _mapper.Map<PaymentDTO>(p)).ToList();
        }

        public async Task<decimal> GetPlatformRevenueAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            return await _unitOfWork.paymentRepository.GetTotalPlatformRevenue(startDate, endDate);
        }

        public async Task MarkPaymentAsTransferredAsync(int paymentId)
        {
            var payment = await _unitOfWork.paymentRepository.GetByIdAsync(paymentId);
            if (payment != null)
            {
                payment.TransferStatus = TransferStatus.Transferred;
                payment.TransferDate = DateTime.UtcNow;
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<PaymentSummaryDTO> GetPaymentSummaryAsync(string hostId)
        {
            var allPayments = await _unitOfWork.paymentRepository.GetHostPayments(hostId);

            return new PaymentSummaryDTO
            {
                TotalEarnings = allPayments
                    .Where(p => p.Status == PaymentStatus.Succeeded)
                    .Sum(p => p.HostAmount),

                PendingTransfers = allPayments
                    .Where(p => p.TransferStatus == TransferStatus.NotTransferred || p.TransferStatus == TransferStatus.PendingTransfer)
                    .Sum(p => p.HostAmount),

                CompletedTransfers = allPayments
                    .Where(p => p.TransferStatus == TransferStatus.Transferred)
                    .Sum(p => p.HostAmount),

                FailedTransfers = allPayments
                    .Count(p => p.TransferStatus == TransferStatus.TransferFailed),

                TotalBookings = allPayments.Count
            };
        }



        public async Task<PaginatedResult<AdminPaymentDTO>> GetAllPaymentsForAdminAsync(int page, int pageSize)
        {
            return await _unitOfWork.paymentRepository.GetAllPaymentsForAdminAsync(page, pageSize);
        }

        public async Task<bool> IsStripeAccountCompletedAsync(string userId)
        {
            var user =  _unitOfWork.UserRepo.GetById(userId);
            return user != null && !string.IsNullOrEmpty(user.StripeAccountId);
        }


    }
}