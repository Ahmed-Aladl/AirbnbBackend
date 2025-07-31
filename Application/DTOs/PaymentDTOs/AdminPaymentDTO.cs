using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.PaymentDTOs
{
    public class AdminPaymentDTO
    {
        public int PaymentId { get; set; }
        public bool HostAccountCompleted { get; set; }
        public string GuestName { get; set; }
        public string HostName { get; set; }
        public decimal Amount { get; set; }
        public decimal PlatformFee { get; set; }
        public decimal HostAmount { get; set; }
        public string PaymentStatus { get; set; }
        public string TransferStatus { get; set; }
        public DateTime PaymentDate { get; set; }
        public int BookingId { get; set; }
        public string HostId { get; set; }



    }

}
