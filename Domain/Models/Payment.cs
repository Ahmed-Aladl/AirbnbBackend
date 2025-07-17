using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Status { get; set; }

        public int BookingId{ get; set; }
    }
}
