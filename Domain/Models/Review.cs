using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Review
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public string PrivateComment { get; set; } = string.Empty;
        public int Rating { get; set; } 
        public int Cleanliness { get; set; } = 0;
        public int Accuracy { get; set; } = 0;
        public int Communication { get; set; } = 0;
        public int CheckIn { get; set; } = 0;
        public int Location { get; set; } = 0;
        public int Value { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        //

        [ForeignKey(nameof(User))]
        public string UserId { get; set; }

        [ForeignKey(nameof(Property))]
        public int PropertyId { get; set; }

        [ForeignKey(nameof(Booking))]
        public int BookingId { get; set; }
        public User User { get; set; }
        public Property Property { get; set; }
        public Booking Booking { get; set; }
    }
}
