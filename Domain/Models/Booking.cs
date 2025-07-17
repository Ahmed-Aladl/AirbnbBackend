using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Domain.Models
{
    public class Booking
    {
        public int BookingID { get; set; }


        //[ForeignKey(nameof(Properties ))]
        public int PropertyID { get; set; }



        //[ForeignKey(nameof(Properties))]
        public int GuestID { get; set; } 
        public DateTime CheckInDate { get; set; }

        public DateTime CheckOutDate { get; set; }
         
        public int NumberOfGuests { get; set; } 

        public decimal TotalPrice { get; set; } 

        public string BookingStatus { get; set; } 




    }
}
