using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Domain.Models
{
    public class Booking
    {
        public int Id { get; set; }

        public DateTime CheckInDate { get; set; }

        public DateTime CheckOutDate { get; set; }

        public int NumberOfGuests { get; set; }

        [Precision(18,6)]
        public decimal TotalPrice { get; set; }

        public string BookingStatus { get; set; }
        public bool IsDeleted { get; set; } 


        [ForeignKey(nameof(Property))]
        public int PropertyId { get; set; }




        [ForeignKey(nameof(User))]
        public string UserId { get; set; } 
        
        public User User{ get; set; }
        public Property Property { get; set; }




    }
}
