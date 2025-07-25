﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ReviewDTOs
{
    public class AddReviewByGuestDTO
    {

        public string Comment { get; set; }

        public string PrivateComment { get; set; } = string.Empty;

        // Ratings per category
        public int Cleanliness { get; set; }
        public int Accuracy { get; set; }
        public int Communication { get; set; }
        public int Location { get; set; }
        public int CheckIn { get; set; }
        public int Value { get; set; }

        public int BookingId { get; set; }
        public string userId    { get; set; }
        public int PropertyId { get; set; }

    }
}
