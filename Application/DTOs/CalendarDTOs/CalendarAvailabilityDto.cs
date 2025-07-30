using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Calendar
{
    public class CalendarAvailabilityDto
    {
        public DateTime Date { get; set; }
        public bool IsAvailable { get; set; }
        public decimal Price { get; set; }
        public bool IsBooked { get; set; } = false;
    }
}