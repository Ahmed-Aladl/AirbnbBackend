using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.BookingDTOS
{
    public class BookingDetailsDTO
    {

        public int Id { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NumberOfGuests { get; set; }
        public decimal TotalPrice { get; set; }
        public string BookingStatus { get; set; }
        public string UserId { get; set; }


        public int PropertyId { get; set; }
        public string PropertyTitle { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

    }
}
