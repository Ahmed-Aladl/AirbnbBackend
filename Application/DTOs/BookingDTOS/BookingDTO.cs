using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums.Booking;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Application.DTOs.BookingDTOS
{
    public class BookingDTO
    {
        public string UserId { get; set; }
        public int PropertyId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NumberOfGuests { get; set; }

    }
}
