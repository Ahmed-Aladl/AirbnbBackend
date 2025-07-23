using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace Application.DTOs.ReviewDTOs
{
    public class GuestReviewDTO
    {

                 //
            public string UserId { get; set; }
            public int PropertyId { get; set; }
            public int BookingId { get; set; }

            public string Comment { get; set; }
             public string PrivateComment { get; set; }

             // Ratings per category
           public int Cleanliness { get; set; }
            public int Accuracy { get; set; }
            public int Communication { get; set; }
            public int Location { get; set; }
            public int CheckIn { get; set; }
            public int Value { get; set; }
 

    }
}
