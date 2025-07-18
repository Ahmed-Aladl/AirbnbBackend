using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Application.DTOs.Calendar
{
    public class AvailabilityCheckDto
    {
        public bool IsAvailable { get; set; }
        public List<DateTime> UnavailableDates { get; set; } = new List<DateTime>();
        public decimal TotalPrice { get; set; }
        public string Message { get; set; }
    }
}