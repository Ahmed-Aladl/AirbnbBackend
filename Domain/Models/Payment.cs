using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums.Payment;
namespace Domain.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public decimal PlatformFee { get; set; } // 10% platform fee
        public decimal HostAmount { get; set; } // Amount after platform fee
        public string Currency { get; set; } = "usd";
        public string? StripePaymentIntentId { get; set; }
        public string? StripeSessionId { get; set; }
        public string? StripeCustomerId { get; set; }
        public string? StripeTransferId { get; set; } // Track transfer ID
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public TransferStatus TransferStatus { get; set; } = TransferStatus.NotTransferred;
        public string? UserId { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime? TransferDate { get; set; }
        public string? FailureReason { get; set; }
        public bool IsDeleted { get; set; } = false;

        [ForeignKey(nameof(Booking))]
        public int BookingId { get; set; }
        public Booking? Booking { get; set; }

        // Calculated property for backward compatibility
        public bool IsTransferredToHost => TransferStatus == TransferStatus.Transferred;
    }
}
