using Application.Interfaces;
using Application.Interfaces.IRepositories;
using Application.DTOs.PaymentDTOs;
using AutoMapper;
using Domain.Models;
using Stripe;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Application.Services
{
    public class PaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public PaymentService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration config)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _config = config;
            StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];
        }

        //public async Task<PaymentDTO> CreatePaymentIntentAsync(CreatePaymentDTO dto)
        //{
        //    var options = new PaymentIntentCreateOptions
        //    {
        //        Amount = dto.Amount,
        //        Currency = dto.Currency,
        //        PaymentMethodTypes = new List<string> { "card" },
        //        Metadata = new Dictionary<string, string>
        //        {
        //            { "booking_id", dto.BookingId.ToString() }
        //        }
        //    };

        //    var service = new PaymentIntentService();
        //    var paymentIntent = await service.CreateAsync(options);

        //    var payment = new Payment
        //    {
        //        StripePaymentIntentId = paymentIntent.Id,
        //        Amount = dto.Amount,
        //        Currency = dto.Currency,
        //        Status = paymentIntent.Status,
        //        BookingId = dto.BookingId,
        //        StripeCustomerId = "N/A",
        //        PaymentDate = DateTime.UtcNow
        //    };

        //    _unitOfWork.paymentRepository.Add(payment);
        //    await _unitOfWork.SaveChangesAsync();

        //    return _mapper.Map<PaymentDTO>(payment);
        //}

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

        public async Task<bool> UpdatePaymentStatusAsync(string paymentIntentId, string status)
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
    }
}