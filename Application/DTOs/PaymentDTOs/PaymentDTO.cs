using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums.Payment;

namespace Application.DTOs.PaymentDTOs
{
    public class PaymentDTO
    {
        public int Id { get; set; }
        public PaymentStatus Status { get; set; }
        public TransferStatus TransferStatus { get; set; }
        public decimal Amount { get; set; }
        public decimal PlatformFee { get; set; }
        public decimal HostAmount { get; set; }
        public string Currency { get; set; }
        public string? PaymentIntentId { get; set; }
        public string? SessionId { get; set; }
        public string? TransferId { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime? TransferDate { get; set; }
        public string? FailureReason { get; set; }
        public int BookingId { get; set; }
        public string? UserId { get; set; }
    }


    public class PaymentSummaryDTO
    {
        public decimal TotalEarnings { get; set; }
        public decimal PendingTransfers { get; set; }
        public decimal CompletedTransfers { get; set; }
        public decimal FailedTransfers { get; set; }
        public decimal TotalBookings { get; set; }
    }
}
