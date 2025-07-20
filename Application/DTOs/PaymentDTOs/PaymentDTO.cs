using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.PaymentDTOs
{
    public class PaymentDTO
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public int Amount { get; set; }
        public string Currency { get; set; }
        public string PaymentIntentId { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}
